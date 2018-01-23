using DdpClient;

namespace Meteor.StressTest.Documents
{
    /// <summary>
    /// All variable in main class have to be nullable in other way "changed" event will be unable to parse. Also all variable have to be properties ({ get; set; }).
    /// </summary>
    public class TestUserDoc : DdpDocument
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}
