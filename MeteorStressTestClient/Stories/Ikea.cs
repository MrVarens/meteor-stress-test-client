using System;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DdpClient;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace Meteor.StressTest
{
    public class Story : StoryBase
    {
        private class ContactMethodUpdate
        {
            public string contactMethod;

            public ContactMethodUpdate(string contactMethod)
            {
                this.contactMethod = contactMethod;
            }
        }

        public class ServiceCodeDoc : DdpDocument
        {
            public string name { get; set; }
            public string description { get; set; }
            public string group { get; set; }
            public int order { get; set; }
            public bool isSeparator { get; set; }
            public string length { get; set; }
            public string type { get; set; }
        }

        private class MeteorObj
        {
            public string id;

            public MeteorObj(string id)
            {
                this.id = id;
            }
        }

        private class Inquire
        {
            public class Rfc
            {
                public MeteorObj[] i_am_calling_to;
                public MeteorObj[] products;
                public MeteorObj[] offered_service;
                public MeteorObj[] channel;
                public MeteorObj[] about;

                public Rfc()
                {
                    this.i_am_calling_to = new MeteorObj[0];
                    this.products = new MeteorObj[0];
                    this.offered_service = new MeteorObj[0];
                    this.channel = new MeteorObj[0];
                    this.about = new MeteorObj[0];
                }
            }

            public DateTime startedAt;
            public DateTime sentAt;
            public Rfc rfc;
            public string contactMethod;

            public Inquire(DateTime started, DateTime sent, string contact)
            {
                this.startedAt = started;
                this.sentAt = sent;
                this.rfc = new Rfc();
                this.contactMethod = contact;
            }
        }

        private class ClientSuggestion
        {
            public string suggestion;
            public string lang;

            public ClientSuggestion(string suggestion, string lang)
            {
                this.suggestion = suggestion;
                this.lang = lang;
            }
        }

        private static string[] CONTACT_METHODS = new string[] { "call", "mail", "chat", "social media" };

        private string Lang;
        private string ContactMethod;
        private Collection<ServiceCodeDoc> ServiceCodes;
        private Random RNG;

        protected override void Handle()
        {
            this.Config();
            this.OpenPage();
            Thread.Sleep(5000);
            this.Login();
            Thread.Sleep(3000);
            this.UserActions();
        }

        private void Config()
        {
            this.ServiceCodes = new Collection<ServiceCodeDoc>();
            this.Connection.GetSubscriber<ServiceCodeDoc>("Service_codes").Subscribers.Add(this.ServiceCodes);
            this.RNG = new Random();

            this.ContactMethod = CONTACT_METHODS[this.RNG.Next(CONTACT_METHODS.Length)];
        }

        private void OpenPage()
        {
            this.DownloadPageFiles();

            // Actions on client app start
            this.SubscribeSync("meteor.loginServiceConfiguration");
            this.SubscribeSync("_roles");
            this.SubscribeSync("meteor_autoupdate_clientVersions");

            this.Connection.CallSync("accounts.passReqInfo");
        }

        private void Login()
        {
            this.LoginWithEmailSync();

            // Actions after "Consultant" login
            this.Connection.CallSync("accounts.isUserLogged", this.User.login, Utils.SHA256(this.User.password).ToString());
            this.Connection.CallSync("common.openTechAnnouncement");
            this.Connection.CallSync("accounts.getUserLanguage");
            this.Lang = (string)this.Connection.CallSync("accounts.getUserLanguage").Result;
            this.SubscribeSync("_roles");
            this.SubscribeSync("_roles");
            for (int i = 0; i < 4; i++)
                this.AfterLoginRouterSubscribe();
            this.Connection.CallSync("common.getLangsForUser");
            for (int i = 0; i < 5; i++)
                this.SubscribeSync("stores", this.Lang);
        }

        private void UserActions()
        {
            this.Connection.CallSync("consultant.dashboard.updateSettings", new ContactMethodUpdate(this.ContactMethod));

            while (this.Thread.IsAlive)
            {
                Thread.Sleep(this.RNG.Next(240000, 360000));
                
                IEnumerable<ServiceCodeDoc> serviceCodes = this.ServiceCodes.Values.Where(x => !x.isSeparator);
                IEnumerable<ServiceCodeDoc> iAmCallingTo = serviceCodes.Where(x => x.group == "i_am_calling_to");
                IEnumerable<ServiceCodeDoc> products = serviceCodes.Where(x => x.group == "products");
                IEnumerable<ServiceCodeDoc> offeredService = serviceCodes.Where(x => x.group == "offered_service");
                IEnumerable<ServiceCodeDoc> channel = serviceCodes.Where(x => x.group == "channel");
                IEnumerable<ServiceCodeDoc> about = serviceCodes.Where(x => x.group == "about");

                // Generate clicked blocks
                Inquire inquire = new Inquire(DateTime.Now.Subtract(new TimeSpan(0, 0, 1)), DateTime.Now, this.ContactMethod);
                inquire.rfc.i_am_calling_to = new MeteorObj[] { this.GetRandomObject(iAmCallingTo) };
                inquire.rfc.products = this.GetRandomObjects(products);
                inquire.rfc.offered_service = this.GetRandomObjects(offeredService);
                inquire.rfc.channel = this.GetRandomObjects(channel);
                inquire.rfc.about = this.GetRandomObjects(about, 1);

                this.SimulateGetProducts();

                // Generate suggestion
                string suggestionText = "";
                if (this.RNG.Next(100) < 20)
                    suggestionText = Utils.RandomString(this.RNG.Next(50, 200));

                ClientSuggestion suggestion = new ClientSuggestion(suggestionText, this.Lang);

                this.Connection.CallSync("consultant.saveInquiry", inquire, suggestion);
            }
        }

        private void AfterLoginRouterSubscribe()
        {
            this.SubscribeSync("service_codes", this.Lang);
            this.SubscribeSync("current_user");
            this.SubscribeSync("extra_info_csc");
            this.SubscribeSync("countries");
            this.SubscribeSync("active-rules-consultant");
        }

        private MeteorObj GetRandomObject(IEnumerable<ServiceCodeDoc> list)
        {
            return new MeteorObj(list.ElementAt(this.RNG.Next(list.Count())).Id);
        }

        private MeteorObj[] GetRandomObjects(IEnumerable<ServiceCodeDoc> list, int min = 0, int max = 3)
        {
            int count = this.RNG.Next(min, Math.Min(max, list.Count()));

            MeteorObj[] value = new MeteorObj[count];
            for (int i = 0; i < count; i++)
                value[i] = this.GetRandomObject(list);

            return value;
        }

        private void SimulateGetProducts()
        {
            if (this.RNG.Next(100) < 50)
                return;

            string text = Utils.RandomString(this.RNG.Next(2, 5));
            for (int i = 1; i <= text.Length; i++)
            {
                this.Connection.CallSync("search.articles.byPhrase", text.Substring(0, i), 10);
                Thread.Sleep(200);
            }
        }
    }
}
