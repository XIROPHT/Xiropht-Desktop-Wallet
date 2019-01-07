using System;
using System.Windows.Forms;
using Xiropht_Wallet.FormPhase.ParallelForm;

namespace Xiropht_Wallet.FormPhase
{
    public class ClassParallelForm
    {
        public static bool PinFormShowed;
        public static PinForm PinForm = new PinForm();
        public static bool WaitingFormShowed;
        public static WaitingForm WaitingForm = new WaitingForm();
        public static bool WaitingCreateWalletFormShowed;
        public static WaitingCreateWalletForm WaitingCreateWalletForm = new WaitingCreateWalletForm();

        public static bool WaitingForm2Showed;
        public static WaitingForm WaitingForm2 = new WaitingForm();

        public static bool WaitingFormReconnectShowed;
        public static WaitingFormReconnect WaitingFormReconnect = new WaitingFormReconnect();


        /// <summary>
        /// Show pin form only if he is not showed.
        /// </summary>
        public static void ShowPinForm()
        {
            try
            {
                if (!PinFormShowed)
                {
                    PinFormShowed = true;
                    ClassFormPhase.WalletXiropht.Invoke((Action) delegate()
                    {
                        PinForm.StartPosition = FormStartPosition.CenterParent;
                        PinForm.TopMost = false;
                        PinForm.ShowDialog(ClassFormPhase.WalletXiropht);
                    });
                }
            }
            catch
            {
                PinFormShowed = false;
            }
        }

        public static void HidePinForm()
        {
            try
            {
                if (PinFormShowed)
                {
                    PinForm.Invoke((Action) delegate { PinForm.Hide(); });
                    PinFormShowed = false;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static void ShowWaitingForm()
        {
            if (!WaitingFormShowed)
            {
                WaitingFormShowed = true;
                ClassFormPhase.WalletXiropht.Invoke((Action) delegate()
                {
                    WaitingForm.StartPosition = FormStartPosition.CenterParent;
                    WaitingForm.TopMost = false;
                    WaitingForm.ShowDialog(ClassFormPhase.WalletXiropht);
                });
            }
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static void HideWaitingForm()
        {
            if (WaitingFormShowed)
            {
                WaitingFormShowed = false;
                if (WaitingForm.InvokeRequired)
                {
                    WaitingForm.Invoke((Action) delegate { WaitingForm.Hide(); });
                }
                else
                {
                    WaitingForm.Hide();
                }
            }
        }


        /// <summary>
        /// Show waiting reconnect form network.
        /// </summary>
        public static void ShowWaitingReconnectForm()
        {
            if (!WaitingFormReconnectShowed)
            {
                WaitingFormReconnectShowed = true;
                ClassFormPhase.WalletXiropht.Invoke((Action)delegate ()
                {
                    WaitingFormReconnect.StartPosition = FormStartPosition.CenterParent;
                    WaitingFormReconnect.TopMost = false;
                    WaitingFormReconnect.ShowDialog(ClassFormPhase.WalletXiropht);
                });
            }
        }

        /// <summary>
        /// Hide waiting reconnect form.
        /// </summary>
        public static void HideWaitingReconnectForm()
        {
            if (WaitingFormReconnectShowed)
            {
                WaitingFormReconnectShowed = false;
                if (WaitingFormReconnect.InvokeRequired)
                {
                    WaitingFormReconnect.Invoke((Action)delegate { WaitingFormReconnect.Hide(); });
                }
                else
                {
                    WaitingFormReconnect.Hide();
                }
            }
        }


        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static void ShowWaitingForm2()
        {
            if (!WaitingForm2Showed)
            {
                WaitingForm2Showed = true;
                ClassFormPhase.WalletXiropht.Invoke((Action) delegate()
                {
                    WaitingForm2.StartPosition = FormStartPosition.CenterParent;
                    WaitingForm2.TopMost = false;
                    WaitingForm2.ShowDialog(ClassFormPhase.WalletXiropht);
                });
            }
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static void HideWaitingForm2()
        {
            if (WaitingForm2Showed)
            {
                WaitingForm2Showed = false;
                ClassFormPhase.WalletXiropht.Invoke((Action) delegate() { WaitingForm2.Hide(); });
            }
        }


        /// <summary>
        /// Show waiting dialog of create wallet.
        /// </summary>
        public static void ShowWaitingCreateWalletForm()
        {
            if (!WaitingCreateWalletFormShowed)
            {
                WaitingCreateWalletFormShowed = true;
                ClassFormPhase.WalletXiropht.Invoke((Action) delegate()
                {
                    WaitingCreateWalletForm.StartPosition = FormStartPosition.CenterParent;
                    WaitingCreateWalletForm.TopMost = false;
                    WaitingCreateWalletForm.ShowDialog(ClassFormPhase.WalletXiropht);
                });
            }
        }

        /// <summary>
        /// Hide waiting dialog of create wallet.
        /// </summary>
        public static void HideWaitingCreateWalletForm()
        {
            if (WaitingCreateWalletFormShowed)
            {
                WaitingCreateWalletFormShowed = false;
                if (WaitingCreateWalletForm.InvokeRequired)
                {
                    WaitingCreateWalletForm.Invoke((Action) delegate { WaitingCreateWalletForm.Hide(); });
                }
                else
                {
                    WaitingCreateWalletForm.Hide();
                }
            }
        }
    }
}