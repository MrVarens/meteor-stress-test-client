using DdpClient;
using DdpClient.EJson;
using System;

namespace Meteor.StressTest.Documents
{
    /// <summary>
    /// All variable in main class have to be nullable in other way "changed" event will be unable to parse. Also all variable have to be properties ({ get; set; }).
    /// </summary>
    public class ControlDoc : DdpDocument
    {
        public bool? isRunning { get; set; }
        public DdpDate startEnabledAt { get; set; }
    }
}
