using Meteor.StressTest.Collections;

namespace Meteor.StressTest
{
    public class CollectionList
    {
        public ConfigCol Config;
        public ControlCol Control;
        public TestUsersCol TestUsers;

        public CollectionList()
        {
            this.Config = new ConfigCol();
            this.Control = new ControlCol();
            this.TestUsers = new TestUsersCol();
        }

        public void Clear()
        {
            this.Config.Clear();
            this.Control.Clear();
            this.TestUsers.Clear();
        }
    }
}
