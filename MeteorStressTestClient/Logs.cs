using System;

namespace Meteor.StressTest
{
    public static class Logs
    {
        public enum ELogType
        {
            INFO,
            ERROR,
            DEBUG
        }

        public class EventArgs
        {
            public ELogType Type;
            public string Text;

            public EventArgs(ELogType type, string text)
            {
                this.Type = type;
                this.Text = text;
            }
        }

        public static EventHandler<EventArgs> OnLog;

        public static void Info(string text)
        {
            OnLog?.Invoke(null, new EventArgs(ELogType.INFO, text));
        }

        public static void Error(string text)
        {
            OnLog?.Invoke(null, new EventArgs(ELogType.ERROR, text));
        }

        public static void Debug(string text)
        {
#if DEBUG
            if (Config.SHOW_DEBUG)
                OnLog?.Invoke(null, new EventArgs(ELogType.DEBUG, text));
#endif
        }
    }
}
