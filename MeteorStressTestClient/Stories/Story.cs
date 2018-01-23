//using System.Threading;
//using Newtonsoft.Json.Linq;

//namespace Meteor.StressTest
//{
//    public class Story : StoryBase
//    {
//        protected override void Handle()
//        {
//            this.Connection.LoginWithEmailSync("email", "password");

//            Logs.Info("Logged in");

//            this.SubscribeSync("subscription_name", "arguments");

//            Logs.Info("Subscribed");
            
//            this.Connection.CallSync("method_name", (response) => {
//                Logs.Info("Result: " + ((JObject)response.Result).ToString());
//            }, "arguments");
            
//            Logs.Info("Story done");
//        }
//    }
//}
