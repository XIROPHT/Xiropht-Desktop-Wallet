using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Wallet.FormPhase.ParallelForm;

namespace Xiropht_Wallet.FormPhase
{
    public class ClassParallelForm
    {
        public static bool PinFormShowed;
        public static PinFormWallet PinForm = new PinFormWallet();
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
        public static async void ShowPinFormAsync()
        {
            await Task.Factory.StartNew(() =>
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
            }).ConfigureAwait(false);
        }

        public static async void HidePinFormAsync()
        {
            await Task.Factory.StartNew(() =>
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
             }).ConfigureAwait(false);
        }

        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static async void ShowWaitingFormAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                if (!WaitingFormShowed)
                {
                    WaitingFormShowed = true;
#if WINDOWS
                    try
                    {
                        if (WaitingForm.Visible)
                        {
                            HideWaitingFormAsync();
                        }
                        ClassFormPhase.WalletXiropht.Invoke((MethodInvoker)delegate ()
                        {
                            WaitingForm.StartPosition = FormStartPosition.CenterParent;
                            WaitingForm.TopMost = false;
                            WaitingForm.ShowDialog(ClassFormPhase.WalletXiropht);
                        });
                    }
                    catch
                    {

                    }
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
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static async void HideWaitingFormAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                if (WaitingFormShowed)
                {
                    WaitingFormShowed = false;
                    try
                    {

                        MethodInvoker invoke = () => WaitingForm.Hide();
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }
                    catch
                    {

                    }
                }
            }).ConfigureAwait(false);
        }


        /// <summary>
        /// Show waiting reconnect form network.
        /// </summary>
        public static async void ShowWaitingReconnectFormAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                if (!WaitingFormReconnectShowed)
                {
                    WaitingFormReconnectShowed = true;
                    try
                    {
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
                    catch
                    {
                        WaitingFormReconnectShowed = false;
                    }
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Hide waiting reconnect form.
        /// </summary>
        public static async void HideWaitingReconnectFormAsync()
        {
            await Task.Factory.StartNew(() =>
             {
                 if (WaitingFormReconnectShowed)
                 {
                     WaitingFormReconnectShowed = false;
                     try
                     {
                         WaitingFormReconnect.Invoke((MethodInvoker)delegate { WaitingFormReconnect.Hide(); WaitingFormReconnect.Refresh(); });
                     }
                     catch
                     {

                     }
                 }
             }).ConfigureAwait(false);
        }


        /// <summary>
        /// Show waiting form network.
        /// </summary>
        public static async void ShowWaitingForm2Async()
        {
            await Task.Factory.StartNew(() =>
            {
                if (!WaitingForm2Showed)
                {
                    try
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
                    catch
                    {

                    }
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Hide waiting form.
        /// </summary>
        public static async Task HideWaitingForm2Async()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (WaitingForm2Showed)
                    {
                        WaitingForm2Showed = false;
                        WaitingForm2.Invoke((MethodInvoker)delegate () { WaitingForm2.Hide(); WaitingForm2.Refresh(); });
                    }
                }
                catch
                {

                }
            }).ConfigureAwait(false);
        }


        /// <summary>
        /// Show waiting dialog of create wallet.
        /// </summary>
        public static async void ShowWaitingCreateWalletFormAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                try
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
                }
                catch
                {

                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Hide waiting dialog of create wallet.
        /// </summary>
        public static async void HideWaitingCreateWalletFormAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (WaitingCreateWalletFormShowed)
                    {
                        WaitingCreateWalletFormShowed = false;

                        WaitingCreateWalletForm.Invoke((MethodInvoker)delegate { WaitingCreateWalletForm.Hide(); });
                    }
                }
                catch
                {

                }
            }).ConfigureAwait(false);
        }
    }
}