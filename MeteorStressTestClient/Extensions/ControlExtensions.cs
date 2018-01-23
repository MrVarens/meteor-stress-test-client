using System;
using System.Windows.Forms;

namespace Meteor.StressTest.Extensions
{
    public static class ControlExtensions
    {
        public static void UIThread(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
                @this.BeginInvoke(code);
            else
                code.Invoke();
        }
    }
}
