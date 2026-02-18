using System;
using System.Windows.Forms;

namespace UiTools.Controls.ExtendedDataGridView
{
    public class UIThreadRunner
    {
        private readonly Control _ctl; // any Control from the UI thread

        public UIThreadRunner(Control ctl)
        {
            _ctl = ctl;
        }

        public void RunSafe(Action action)
        {
            if (_ctl.InvokeRequired)
                _ctl.Invoke(action);
            else
                action.Invoke();
        }

        public void DelayedRunSafe(int timeout, Action action)
        {
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                timer.Dispose();
                RunSafe(action);
            },
            null, timeout, System.Threading.Timeout.Infinite);
        }
    }
}
