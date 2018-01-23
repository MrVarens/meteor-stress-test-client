using DdpClient;
using DdpClient.Models.Server;
using Microsoft.CSharp;
using Meteor.StressTest.Collections;
using Meteor.StressTest.Documents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meteor.StressTest
{
    public class ServerManager
    {
        public static class EStateUpdate
        {
            public static string STATE = "state";
            public static string STORIES = "stories";
        }

        private static ServerManager _Instance;

        private string URL;
        private bool SSL;

        public State State { get; private set; }

        private DdpConnection Connection;
        private string ConnectionID;

        private List<DdpSubHandler> Subscriptions;
        public CollectionList Collections;
        
        private Thread ReconnectThread;

        private CompilerResults StoryCompiler;
        
        private bool IsClosing;

        private bool _IsConnected;
        public bool IsConnected
        {
            get
            {
                return this._IsConnected;
            }

            private set
            {
                this._IsConnected = value;

                if (value)
                    this.State.Set("Connected", State.EType.SUCCESS);
                else
                    this.State.Set("Disconnected", State.EType.FAILED);
            }
        }

        public bool AllSubscriptionsReady
        {
            get
            {
                return !this.Subscriptions.Any(x => !x.IsReady);
            }
        }
       
        public static ServerManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ServerManager();

                return _Instance;
            }
        }

        private ServerManager()
        {
            this.State = new State();
            this.Subscriptions = new List<DdpSubHandler>();
            this.Collections = new CollectionList();
        }

        public void Connect()
        {
            if (this.IsConnected)
                return;
            
            this.State.Set("Connecting...");

            this.Connection = new DdpConnection();
            this.Connection.Error += this.OnError;
            this.Connection.Connected += this.OnConnected;
            this.Connection.Closed += this.OnClose;
            
            this.Connection.GetSubscriber<ConfigDoc>("config").Subscribers.Add(this.Collections.Config);
            this.Connection.GetSubscriber<ControlDoc>("control").Subscribers.Add(this.Collections.Control);
            this.Connection.GetSubscriber<TestUserDoc>("test-users").Subscribers.Add(this.Collections.TestUsers);

            this.Connection.ConnectAsync(this.URL, this.SSL);
        }

        public void Config(string url, bool ssl)
        {
            this.URL = url;
            this.SSL = ssl;
        }

        public void Disconnect()
        {
            if (!this.IsConnected)
                return;

            this.IsClosing = true;
            this.Connection.Close();
        }

        private void Reconnect()
        {
            if (this.IsClosing || (this.ReconnectThread != null && this.ReconnectThread.IsAlive))
                return;

            this.ReconnectThread = new Thread(async () =>
            {
                this.State.Set("Reconnecting in 5 seconds...");
                await Task.Delay(5000);

                this.Connect();
            });

            this.ReconnectThread.Start();
        }

        private DdpSubHandler _Subscribe(string name, params object[] param)
        {
            DdpSubHandler handler = this.Connection.GetSubHandler(name, param);
            handler.NoSub += this.OnNoSub;

            this.Subscriptions.Add(handler);
            
            return handler;
        }

        private DdpSubHandler Subscribe(string name, params object[] param)
        {
            DdpSubHandler handler = this._Subscribe(name, param);
            handler.Sub();
            return handler;
        }

        private DdpSubHandler SubscribeSync(string name, params object[] param)
        {
            DdpSubHandler handler = this._Subscribe(name, param);
            handler.SubSync();
            return handler;
        }

        public void CreateStoryCompiler(string source)
        {
            if (source == null)
            {
                this.StoryCompiler = null;
                return;
            }

            Dictionary<string, string> providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !a.Location.Contains("Microsoft.CSharp.dll"))
                .Select(a => a.Location);
            CompilerParameters compilerParams = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };
            compilerParams.ReferencedAssemblies.Add("HtmlAgilityPack.dll");
            compilerParams.ReferencedAssemblies.Add("DDPClient.dll");
            compilerParams.ReferencedAssemblies.Add("Newtonsoft.Json.dll");
            compilerParams.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParams.ReferencedAssemblies.AddRange(assemblies.ToArray());

            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                    Logs.Error("[Test] Story compile error: " + error.ToString());

                this.StoryCompiler = null;
                return;
            }

            this.StoryCompiler = results;
        }

        public StoryBase CreateStory()
        {
            if (this.StoryCompiler == null)
                return null;

            return (StoryBase)this.StoryCompiler.CompiledAssembly.CreateInstance("Meteor.StressTest.Story");
        }

        public DdpSubHandler SubscribeTestUsers(int count)
        {
            this.Connection.CallSync("client.reserveTestUsers");
            DdpSubHandler handler = this.SubscribeSync("client.testUsers");

            int current;
            while ((current = this.Collections.TestUsers.Count) < count)
            {
                TestManager.Instance.State.Set("Waiting for test users " + current + "/" + count + "...");
                Thread.Sleep(600);
                this.Connection.CallSync("client.reserveTestUsers");
            }

            return handler;
        }
        
        public void UpdateState(string type, params object[] value)
        {
            if (this.Connection != null)
                this.Connection.Call("client.stateUpdate", type, value);
        }

        private void OnError(object sender, Exception e)
        {
            string error;
            if (e == null)
                error = "Unknown error!";
            else
                error = e.Message;

            Logs.Error("[Server] Ddp connection error: " + error);
        }

        private void OnConnected(object sender, ConnectResponse connectResponse)
        {
            if (connectResponse.DidFail())
            {
                Logs.Error("[Server] Connection fail: " + connectResponse.Failed.Version);
                this.Reconnect();
            }
            else
            {
                this.ConnectionID = connectResponse.Session;
                this.IsConnected = true;

                this.Subscribe("client.init", StressTest.Config.PROTECTION_TOKEN);
            }
        }

        private void OnClose(object sender, EventArgs eventArgs)
        {
            this.IsConnected = false;

            if (TestManager.Instance.IsRunning)
                TestManager.Instance.Stop();

            this.Subscriptions.Clear();
            this.Collections.Clear();

            this.Connection = null;
            this.ConnectionID = null;
            this.Reconnect();

            if (this.IsClosing)
                this.IsClosing = false;
        }
        
        private void OnNoSub(object sender, NoSubModel model)
        {
            if (model.Error != null)
                Logs.Error("[Server] Unable to subscribe into \"" + ((DdpSubHandler)sender).Name + "\": " + model.Error.Reason);

            this.Subscriptions.Remove((DdpSubHandler)sender);
        }
    }
}
