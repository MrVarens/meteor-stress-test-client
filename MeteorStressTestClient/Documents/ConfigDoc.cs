using DdpClient;

namespace Meteor.StressTest.Documents
{
    /// <summary>
    /// All variable in main class have to be nullable in other way "changed" event will be unable to parse. Also all variable have to be properties ({ get; set; }).
    /// </summary>
    public class ConfigDoc : DdpDocument
    {
        public class Server
        {
            public string host { get; set; }
            public bool ssl { get; set; }
        }

        public class Delay
        {
            public int from { get; set; }
            public int? to { get; set; }
        }
        
        public Server server { get; set; }
        public Delay delay { get; set; }
        public string script { get; set; }
        public int? threadsPerClient { get; set; }
        public bool? sendTestUsers { get; set; }
    }
}
