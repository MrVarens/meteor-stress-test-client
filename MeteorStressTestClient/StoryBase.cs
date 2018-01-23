using DdpClient;
using DdpClient.Models;
using DdpClient.Models.Server;
using HtmlAgilityPack;
using Meteor.StressTest.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Meteor.StressTest
{
    /// <summary>
    /// DDP package docs: https://github.com/JohnnyCrazy/DDPClient-NET
    /// </summary>
    public abstract class StoryBase : IDisposable
    {
        protected string URL;
        protected bool SSL;
        public TestUserDoc User;

        protected Thread Thread;
        protected DdpConnection Connection;
        private List<DdpSubHandler> Subscriptions;
        
        public EventHandler<EventArgs> End;

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
                    Logs.Debug("[Story] Connected");
                else
                    Logs.Debug("[Story] Disconnected");
            }
        }

        public bool AllSubscriptionsReady
        {
            get
            {
                return !this.Subscriptions.Any(x => !x.IsReady);
            }
        }

        public StoryBase()
        {
            this.Thread = new Thread(this.Run);
            this.Subscriptions = new List<DdpSubHandler>();
        }

        private void Run()
        {
            try
            {
                this.Connect();

                while (!this.IsConnected)
                    Thread.Sleep(100);

                this.Handle();
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Logs.Error("[Story] Thread error: " + e.Message);
            }
            finally
            {
                if (this.Connection != null)
                {
                    this.Connection.Close();
                    this.Connection = null;
                }

                this.End?.Invoke(this, new EventArgs());
            }
        }

        public void Start(string url, bool ssl, TestUserDoc user = null)
        {
            if (this.Thread.IsAlive)
                return;

            this.URL = url;
            this.SSL = ssl;
            this.User = user;

            this.Thread.Start();
        }

        public void Stop()
        {
            if (!this.Thread.IsAlive)
                return;

            this.Thread.Abort();
            this.Thread.Join();
        }

        private void Connect()
        {
            this.Connection = new DdpConnection();

            this.Connection.Error += this.OnError;
            this.Connection.Connected += this.OnConnected;
            this.Connection.Closed += this.OnClose;
            this.Connection.Login += this.OnLogin;
            this.Connection.Message += this.OnMessage;

            this.Connection.ConnectAsync(this.URL, this.SSL);
        }

        protected abstract void Handle();
        
        protected string[] DownloadPageFiles()
        {
            List<string> fileContents = new List<string>();

            WebClient web = new WebClient();

            string host = (this.SSL ? "https://" : "http://") + this.URL;
            string content = web.DownloadString(host);
            fileContents.Add(content);

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            
            List<string> files = new List<string>();

            HtmlNodeCollection links = html.DocumentNode.SelectNodes("//head/link");
            foreach (HtmlNode node in links)
            {
                if (node.Attributes.Contains("href"))
                    files.Add(node.Attributes["href"].Value);
            }

            HtmlNodeCollection scripts = html.DocumentNode.SelectNodes("//head/script");
            foreach (HtmlNode node in scripts)
            {
                if (node.Attributes.Contains("src"))
                    files.Add(node.Attributes["src"].Value);
            }

            string fileContent;
            foreach (string file in files)
            {
                fileContent = web.DownloadString(host + file);
                fileContents.Add(fileContent);
            }

            return fileContents.ToArray();
        }

        private DdpSubHandler _Subscribe(string name, params object[] param)
        {
            DdpSubHandler handler = this.Connection.GetSubHandler(name, param);
            handler.NoSub += this.OnNoSub;

            this.Subscriptions.Add(handler);

            return handler;
        }

        public DdpSubHandler Subscribe(string name, params object[] param)
        {
            DdpSubHandler handler = this._Subscribe(name, param);
            handler.Sub();
            return handler;
        }

        public DdpSubHandler SubscribeSync(string name, params object[] param)
        {
            DdpSubHandler handler = this._Subscribe(name, param);
            handler.SubSync();
            return handler;
        }

        public void LoginWithUsernameSync()
        {
            if (this.User == null)
            {
                Logs.Error("[Story] Unable to login because user data not exists");
                return;
            }

            this.Connection.LoginWithUsernameSync(this.User.login, this.User.password);
        }

        public void LoginWithEmailSync()
        {
            if (this.User == null)
            {
                Logs.Error("[Story] Unable to login because user data not exists");
                return;
            }

            this.Connection.LoginWithEmailSync(this.User.login, this.User.password);
        }

        public void LoginWithTokenSync()
        {
            if (this.User == null)
            {
                Logs.Error("[Story] Unable to login because user data not exists");
                return;
            }

            this.Connection.LoginWithTokenSync(this.User.login);
        }

        private void OnError(object sender, Exception e)
        {
            string error;
            if (e == null)
                error = "Unknown error!";
            else
            {
                if (e is ThreadAbortException)
                    return;

                error = e.Message;
            }

            Logs.Error("[Story] Ddp connection error: " + error);
            this.Stop();
        }

        private void OnConnected(object sender, ConnectResponse connectResponse)
        {
            if (connectResponse.DidFail())
            {
                Logs.Error("[Story] Connection fail: " + connectResponse.Failed.Version);
                this.Stop();
            }
            else
                this.IsConnected = true;
        }

        private void OnClose(object sender, EventArgs eventArgs)
        {
            this.Subscriptions.Clear();

            this.Connection = null;
            this.IsConnected = false;
            this.Stop();
        }

        private void OnLogin(object sender, LoginResponse loginResponse)
        {
            if (loginResponse.HasError())
            {
                Logs.Error("[Story] Login error: " + loginResponse.Error.Reason);
                this.Stop();
            }
        }

        private void OnMessage(object sender, DdpMessage e)
        {
            Logs.Debug(e.Body.ToString());
        }

        private void OnNoSub(object sender, NoSubModel model)
        {
            if (model.Error != null)
                Logs.Error("[Test] Unable to subscribe into \"" + ((DdpSubHandler)sender).Name + "\": " + model.Error.Reason);

            this.Subscriptions.Remove((DdpSubHandler)sender);
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
