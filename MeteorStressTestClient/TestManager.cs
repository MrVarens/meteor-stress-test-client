using DdpClient;
using Microsoft.CSharp;
using Meteor.StressTest.Documents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Meteor.StressTest
{
    public class TestManager
    {
        public class StoriesStateEventArgs
        {
            public bool IsRunning;
            public int Running;
            public int Executed;

            public StoriesStateEventArgs(int running, int executed, bool isRunning = true)
            {
                this.IsRunning = isRunning;
                this.Running = running;
                this.Executed = executed;
            }
        }

        private static TestManager _Instance;

        public State State { get; private set; }

        private Thread Thread;

        private List<StoryBase> Stories;
        private Queue<TestUserDoc> Users;

        private int ExecutedStoriesCount;
        
        private TestStateUpdate StateUpdate;

        private Random RND;

        public EventHandler<StoriesStateEventArgs> OnStoriesChange;

        public bool IsRunning
        {
            get
            {
                return this.Thread != null && this.Thread.IsAlive;
            }
        }
        
        public static TestManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new TestManager();

                return _Instance;
            }
        }

        private TestManager()
        {
            this.State = new State();
            this.State.Set("Idle", State.EType.FAILED);
            this.State.OnChange += this.OnStateChange;

            this.StateUpdate = new TestStateUpdate();

            this.Stories = new List<StoryBase>();
            this.RND = new Random();
        }

        private void Run()
        {
            DdpSubHandler testUsersSub = null;

            try
            {
                this.State.Set("Waiting for subscriptions...");

                while (this.Thread.IsAlive && !ServerManager.Instance.AllSubscriptionsReady)
                    Thread.Sleep(100);

                ConfigDoc config = ServerManager.Instance.Collections.Config.Values.FirstOrDefault();
                if (config == null)
                {
                    Logs.Error("[Test] Unable to get configuration");
                    this.Stop();
                }
                
                int threadsPerClient = config.threadsPerClient.GetValueOrDefault();
                bool sendTestUsers = config.sendTestUsers.GetValueOrDefault();

                if (sendTestUsers)
                {
                    testUsersSub = ServerManager.Instance.SubscribeTestUsers(threadsPerClient);
                    this.Users = new Queue<TestUserDoc>(ServerManager.Instance.Collections.TestUsers.Values.Take(threadsPerClient));
                }

                this.StateUpdate.Start();

                this.State.Set("Running", State.EType.SUCCESS);

                while (this.Thread.IsAlive)
                {
                    for (int i = this.Stories.Count; this.Thread.IsAlive && i < threadsPerClient; i++)
                    {
                        StoryBase story = ServerManager.Instance.CreateStory();
                        if (story == null)
                        {
                            Logs.Error("[Test] Script not exists");
                            this.Stop();
                        }

                        TestUserDoc user = null;
                        if (sendTestUsers)
                        {
                            if (this.Users.Count == 0)
                            {
                                Logs.Error("[Test] User account is missing");
                                this.Stop();
                            }

                            user = this.Users.Dequeue();
                        }

                        story.End += OnEnd;
                        this.Stories.Add(story);
                        story.Start(config.server.host, config.server.ssl, user);

                        this.OnStoriesChangeInvoke();

                        if (config.delay.to.HasValue)
                            Thread.Sleep(this.RND.Next(config.delay.from, config.delay.to.Value));
                        else
                            Thread.Sleep(config.delay.from);
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { }
            finally
            {
                this.StateUpdate.Stop();

                StoryBase[] temp = this.Stories.ToArray();
                foreach (StoryBase story in temp)
                {
                    if (story != null)
                        story.Stop();
                }

                this.Stories.Clear();
                this.ExecutedStoriesCount = 0;

                if (this.Users != null)
                {
                    this.Users.Clear();
                    this.Users = null;
                }

                if (ServerManager.Instance.IsConnected && testUsersSub != null)
                    testUsersSub.Unsub();

                this.OnStoriesChangeInvoke(false);

                this.StateUpdate.UpdateStories();
                this.State.Set("Idle", State.EType.FAILED);
            }
        }

        public void Start()
        {
            if (this.IsRunning)
                return;

            this.Thread = new Thread(this.Run);
            this.Thread.Start();
        }

        public void Stop()
        {
            if (!this.IsRunning)
                return;

            this.Thread.Abort();
            this.Thread.Join();
            this.Thread = null;
        }

        public void GetStoriesStats(out int running, out int executed)
        {
            running = this.Stories.Count;
            executed = this.ExecutedStoriesCount;
        }

        private void OnStoriesChangeInvoke(bool isRunning = true)
        {
            this.OnStoriesChange?.Invoke(this, new StoriesStateEventArgs(this.Stories.Count, this.ExecutedStoriesCount, isRunning));
        }

        private void OnStateChange(object sender, EventArgs args)
        {
            State state = (State)sender;
            this.StateUpdate.UpdateState(ServerManager.EStateUpdate.STATE, state.Type, state.Text);
        }

        private void OnEnd(object sender, EventArgs args)
        {
            if (!this.IsRunning)
                return;

            StoryBase story = (StoryBase)sender;
            this.Stories.Remove(story);

            if (this.Users != null)
                this.Users.Enqueue(story.User);

            this.ExecutedStoriesCount++;

            this.OnStoriesChangeInvoke();
        }
    }
}
