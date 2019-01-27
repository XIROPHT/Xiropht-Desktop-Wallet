using System;
using System.Threading;
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
            new Thread(delegate ()
            {
                try
                {
                    if (!PinFormShowed)
                    {
                        PinFormShowed = true;
#if WINDOWS
                        ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                        {
                            PinForm.StartPosition = FormStartPosition.CenterParent;
                            PinForm.TopMost = false;
                            PinForm.ShowDialog(ClassFormPhase.WalletXiropht);
                        });
#else
                        ClassFormPhase.WalletXiropht.BeginInvoke((MethodInvoker)delegate ()
                       {
                           PinForm.StartPosition = FormStartPosition.CenterParent;
                           PinForm.TopMost = true;
                           PinForm.Show(ClassFormPhase.WalletXiropht);
                       });
#endif
                    }
                }
                catch
                {
                    PinFormShowed = false;
                }
            }).Start();
        }

        public static void HidePinForm()
        {
            new Thread(delegate ()
            {
                try
                {
                    if (PinFormShowed)
                    {
                        ClassFormPhase.WalletXiropht.BeginInvoke((MethodInvoker)delegate { PinForm.Hide(); });
                        PinFormShowed = false;
                    }
                }
                catch
                {
                }
            }).Start();
        }

        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static void ShowWaitingForm()
        {
            new Thread(delegate ()
            {
                if (!WaitingFormShowed)
                {
                    WaitingFormShowed = true;
#if WINDOWS
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                    {
                        WaitingForm.StartPosition = FormStartPosition.CenterParent;
                        WaitingForm.TopMost = false;
                        WaitingForm.ShowDialog(ClassFormPhase.WalletXiropht);
                    });
#else
                    MethodInvoker invoke = () =>
                    {
                        WaitingForm.StartPosition = FormStartPosition.CenterParent;
                        WaitingForm.TopMost = true;
                        WaitingForm.Show(ClassFormPhase.WalletXiropht);
                    };
                    ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
#endif
                }
            }).Start();
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static void HideWaitingForm()
        {
            new Thread(delegate ()
            {
                if (WaitingFormShowed)
                {
                    WaitingFormShowed = false;

                    try
                    {
                        WaitingForm.Invoke((MethodInvoker)delegate { WaitingForm.Hide(); });
                    }
                    catch
                    {

                    }
                }
            }).Start();
        }


        /// <summary>
        /// Show waiting reconnect form network.
        /// </summary>
        public static void ShowWaitingReconnectForm()
        {
            new Thread(delegate ()
            {
                if (!WaitingFormReconnectShowed)
                {
                    WaitingFormReconnectShowed = true;
#if WINDOWS
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                    {
                        WaitingFormReconnect.StartPosition = FormStartPosition.CenterParent;
                        WaitingFormReconnect.TopMost = false;
                        WaitingFormReconnect.ShowDialog(ClassFormPhase.WalletXiropht);
                    });
#else
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                    {
                        WaitingFormReconnect.StartPosition = FormStartPosition.CenterParent;
                        WaitingFormReconnect.TopMost = true;
                        WaitingFormReconnect.Show(ClassFormPhase.WalletXiropht);
                    });
#endif
                }
            }).Start();
        }

        /// <summary>
        /// Hide waiting reconnect form.
        /// </summary>
        public static void HideWaitingReconnectForm()
        {
            new Thread(delegate ()
            {
                if (WaitingFormReconnectShowed)
                {
                    WaitingFormReconnectShowed = false;
                    try
                    {
                        WaitingFormReconnect.Invoke((MethodInvoker)delegate { WaitingFormReconnect.Hide(); });
                    }
                    catch
                    {

                    }
                }
            }).Start();
        }


        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static void ShowWaitingForm2()
        {
            new Thread(delegate ()
            {
                if (!WaitingForm2Showed)
                {
                    WaitingForm2Showed = true;
#if WINDOWS
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                    {
                        WaitingForm2.StartPosition = FormStartPosition.CenterParent;
                        WaitingForm2.TopMost = false;
                        WaitingForm2.ShowDialog(ClassFormPhase.WalletXiropht);
                    });
#else
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                   {
                       WaitingForm2.StartPosition = FormStartPosition.CenterParent;
                       WaitingForm2.TopMost = true;
                       WaitingForm2.Show(ClassFormPhase.WalletXiropht);
                   });
#endif
                }
            }).Start();
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static void HideWaitingForm2()
        {
            new Thread(delegate ()
            {
                if (WaitingForm2Showed)
                {
                    WaitingForm2Showed = false;
                    WaitingForm2.Invoke((MethodInvoker)delegate () { WaitingForm2.Hide(); });
                }
            }).Start();
        }


        /// <summary>
        /// Show waiting dialog of create wallet.
        /// </summary>
        public static void ShowWaitingCreateWalletForm()
        {
            new Thread(delegate ()
            {
                if (!WaitingCreateWalletFormShowed)
                {
                    WaitingCreateWalletFormShowed = true;
#if WINDOWS
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                    {
                        WaitingCreateWalletForm.StartPosition = FormStartPosition.CenterParent;
                        WaitingCreateWalletForm.TopMost = false;
                        WaitingCreateWalletForm.ShowDialog(ClassFormPhase.WalletXiropht);
                    });
#else
                    ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                   {
                       WaitingCreateWalletForm.StartPosition = FormStartPosition.CenterParent;
                       WaitingCreateWalletForm.TopMost = true;
                       WaitingCreateWalletForm.Show(ClassFormPhase.WalletXiropht);
                   });
#endif
                }
            }).Start();
        }

        /// <summary>
        /// Hide waiting dialog of create wallet.
        /// </summary>
        public static void HideWaitingCreateWalletForm()
        {
            new Thread(delegate ()
            {
                if (WaitingCreateWalletFormShowed)
                {
                    WaitingCreateWalletFormShowed = false;

                    WaitingCreateWalletForm.Invoke((MethodInvoker)delegate { WaitingCreateWalletForm.Hide(); });

                }
            }).Start();
        }
    }
}