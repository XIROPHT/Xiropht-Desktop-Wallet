using System;
using System.Windows.Forms;

namespace Xiropht_Wallet.FormCustom
{
    public class ClassPanel : Panel
    {
        private Timer _timeUpdateStyle;

        public ClassPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            _timeUpdateStyle = new Timer { Interval = 100, Enabled = true };
            _timeUpdateStyle.Tick += TimeUpdateStyleEvent;
            _timeUpdateStyle.Start();
        }

        private void TimeUpdateStyleEvent(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}