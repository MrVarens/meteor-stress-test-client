using DdpClient.Models.Server;
using Meteor.StressTest.Documents;

namespace Meteor.StressTest.Collections
{
    public class ControlCol : Collection<ControlDoc>
    {
        public override void Added(SubAddedModel<ControlDoc> added)
        {
            base.Added(added);
            
            this.SetTestState(added.Object.isRunning.GetValueOrDefault());
        }

        public override void Changed(SubChangedModel<ControlDoc> changed)
        {
            base.Changed(changed);
            
            if (changed.Object.isRunning.HasValue)
                this.SetTestState(changed.Object.isRunning.Value);
        }

        private void SetTestState(bool isRunning)
        {
            if (isRunning)
                TestManager.Instance.Start();
            else
                TestManager.Instance.Stop();
        }
    }
}
