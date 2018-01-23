using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using DdpClient;
using Meteor.StressTest.Extensions;
using Meteor.StressTest.Documents;
using System.Linq;
using System.Net;

namespace Meteor.StressTest.Forms
{
    /// <summary>
    /// DDP package docs: https://github.com/JohnnyCrazy/DDPClient-NET
    /// </summary>
    public partial class Main : Form
    {
        public static DdpConnection _client;

        public Main()
        {
            InitializeComponent();

#if DEBUG
            this.cbShowDebug.Visible = true;
#endif

            Logs.OnLog += this.OnLog;
            ServerManager.Instance.State.OnChange += this.OnStateChangeServer;
            TestManager.Instance.State.OnChange += this.OnStateChangeTest;
            TestManager.Instance.OnStoriesChange += this.OnStoriesChange;

            string host = ConfigurationManager.AppSettings["serverHost"] ?? "localhost:3000";
            bool ssl;
            bool.TryParse(ConfigurationManager.AppSettings["serverSsl"], out ssl);
            ServerManager.Instance.Config(host, ssl);

            this.SetState(this.lServerState, ServerManager.Instance.State);
            this.SetState(this.lTestState, TestManager.Instance.State);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // Pass all HTTPS certificates
        }
        
        private void Main_Load(object sender, EventArgs e)
        {
            ServerManager.Instance.Connect();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TestManager.Instance.IsRunning)
                TestManager.Instance.Stop();

            if (ServerManager.Instance.IsConnected)
            {
                ServerManager.Instance.Disconnect();

                e.Cancel = true;

                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.SynchronizingObject = this;
                timer.Interval = 100;
                timer.Elapsed += (s, a) =>
                {
                    if (ServerManager.Instance.IsConnected)
                        timer.Start();
                    else
                        this.Close();
                };
                timer.Start();
            }
        }

        private void cbShowDebug_CheckedChanged(object sender, EventArgs e)
        {
            Config.SHOW_DEBUG = this.cbShowDebug.Checked;
        }

        private void OnLog(object sender, Logs.EventArgs e)
        {
            Color color;
            switch (e.Type)
            {
                case Logs.ELogType.ERROR:
                    color = Color.Red;
                    break;
                case Logs.ELogType.DEBUG:
                    color = Color.Blue;
                    break;
                default:
                    color = this.rtbLogs.ForeColor;
                    break;
            }

            this.UIThread(() =>
            {
                this.rtbLogs.AppendText("[" + DateTime.Now.ToShortTimeString() + "] " + e.Text + "\r\n", color);
                this.rtbLogs.ScrollToCaret();
            });
        }

        private void OnStateChangeServer(object sender, EventArgs args)
        {
            this.UIThread(() => {
                this.SetState(this.lServerState, (State)sender);
            });
        }

        private void OnStateChangeTest(object sender, EventArgs args)
        {
            this.UIThread(() => {
                this.SetState(this.lTestState, (State)sender);
            });
        }

        private void OnStoriesChange(object sender, TestManager.StoriesStateEventArgs args)
        {
            this.UIThread(() =>
            {
                string runningText;
                string executedText;

                if (args.IsRunning)
                {
                    runningText = args.Running.ToString();
                    executedText = args.Executed.ToString();

                    ConfigDoc config = ServerManager.Instance.Collections.Config.Values.FirstOrDefault();
                    if (config != null)
                    {
                        int threadsPerClient = config.threadsPerClient.GetValueOrDefault();
                        if (threadsPerClient > 0)
                            runningText += "/" + threadsPerClient;
                    }
                }
                else
                {
                    runningText = "-";
                    executedText = "-";
                }

                this.lRunningCount.Text = runningText;
                this.lExecutedCount.Text = executedText;
            });
        }

        private void SetState(Label label, State state)
        {
            label.Text = state.Text;

            Color color;
            switch (state.Type)
            {
                case State.EType.FAILED:
                    color = Color.DarkRed;
                    break;
                case State.EType.SUCCESS:
                    color = Color.ForestGreen;
                    break;
                default:
                    color = SystemColors.ControlText;
                    break;
            }

            label.ForeColor = color;
        }
    }
}
