using System;

namespace Meteor.StressTest
{
    public class State
    {
        public enum EType
        {
            INFO = 0,
            FAILED = 1,
            SUCCESS = 2
        };

        public EType Type;
        public string Text;
        
        public EventHandler<EventArgs> OnChange;

        public State()
        {
            this.Type = EType.INFO;
            this.Text = "";
        }

        public void Set(string text, EType type = EType.INFO)
        {
            this.Type = type;
            this.Text = text;

            this.OnChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
