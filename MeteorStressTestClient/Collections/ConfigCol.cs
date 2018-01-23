using DdpClient.Models.Server;
using Meteor.StressTest.Documents;
using System.Linq;

namespace Meteor.StressTest.Collections
{
    public class ConfigCol : Collection<ConfigDoc>
    {
        public override void Added(SubAddedModel<ConfigDoc> added)
        {
            base.Added(added);
            
            ServerManager.Instance.CreateStoryCompiler(added.Object.script);
        }

        public override void Changed(SubChangedModel<ConfigDoc> changed)
        {
            base.Changed(changed);

            if (changed.Cleared != null && changed.Cleared.Contains("script"))
                ServerManager.Instance.CreateStoryCompiler(null);
            else if (changed.Object.script != null)
                ServerManager.Instance.CreateStoryCompiler(changed.Object.script);
        }
    }
}
