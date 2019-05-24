using System.Windows.Forms;

namespace Xiropht_Wallet.FormCustom
{
    public class ClassPanel : Panel
    {

        public ClassPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        }


    }
}