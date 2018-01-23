using System.Threading;

namespace Meteor.StressTest
{
    public class TestStateUpdate
    {
        private Thread Thread;

        private int Running;
        private int Executed;
        private int LastRunning;
        private int LastExecuted;

        public TestStateUpdate() { }

        public void Run()
        {
            while (this.Thread.IsAlive)
            {
                Thread.Sleep(5000);
                this.UpdateStories();
            }
        }

        public void Start()
        {
            if (this.Thread != null && this.Thread.IsAlive)
                return;

            this.Thread = new Thread(this.Run);
            this.Thread.Start();
        }

        public void Stop()
        {
            if (this.Thread == null || !this.Thread.IsAlive)
                return;

            this.Thread.Abort();
            this.Thread.Join();
            this.Thread = null;
        }

        public void UpdateState(string type, params object[] value)
        {
            ServerManager.Instance.UpdateState(type, value);
        }

        public void UpdateStories()
        {
            TestManager.Instance.GetStoriesStats(out this.Running, out this.Executed);

            if (this.Running == this.LastRunning && this.Executed == this.LastExecuted)
                return;

            this.UpdateState(ServerManager.EStateUpdate.STORIES, this.Running, this.Executed);

            this.LastRunning = this.Running;
            this.LastExecuted = this.Executed;
        }
    }
}
