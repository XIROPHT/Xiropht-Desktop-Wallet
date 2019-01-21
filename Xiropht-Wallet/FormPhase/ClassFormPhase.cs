using System;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;

namespace Xiropht_Wallet.FormPhase
{
    public class ClassFormPhaseEnumeration
    {
        public const string Main = "MAIN";
        public const string OpenWallet = "OPEN";
        public const string CreateWallet = "CREATE";
        public const string Overview = "OVERVIEW";
        public const string SendTransaction = "SEND";
        public const string TransactionHistory = "TRANSACTION";
        public const string BlockExplorer = "BLOCK";
        public const string RestoreWallet = "RESTORE";
    }

    public class ClassFormPhase
    {
        public static string FormPhase;
        public static WalletXiropht WalletXiropht;

        /// <summary>
        /// Initialize the public static object of the interface for share the object with every class.
        /// </summary>
        /// <param name="wallet"></param>
        public static void InitializeMainInterface(WalletXiropht wallet)
        {
            WalletXiropht = wallet;
            WalletXiropht.SwitchForm(ClassFormPhaseEnumeration.Main);

        }

        /// <summary>
        /// Change form phase.
        /// </summary>
        /// <param name="phase"></param>
        public static void SwitchFormPhase(string phase)
        {
            WalletXiropht.SwitchForm(phase);
        }

        /// <summary>
        /// Show wallet menu.
        /// </summary>
        public static void ShowWalletMenu()
        {
          WalletXiropht.BeginInvoke((MethodInvoker) delegate { WalletXiropht.panelControlWallet.Visible = true; });
        }

        /// <summary>
        /// Show amount and wallet address.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletAmount"></param>
        public static void ShowWalletInformationInMenu(string walletAddress, string walletAmount)
        {

            WalletXiropht.BeginInvoke((MethodInvoker)delegate { WalletXiropht.labelNoticeWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_ADDRESS_TEXT") + " " + walletAddress; });
            WalletXiropht.BeginInvoke((MethodInvoker)delegate
            {
               WalletXiropht.labelNoticeWalletBalance.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT") + " " + walletAmount + " " + ClassConnectorSetting.CoinNameMin;

            });

        }

        /// <summary>
        /// Hide wallet menu.
        /// </summary>
        public static void HideWalletMenu()
        {
            void Invoke() => WalletXiropht.panelControlWallet.Visible = false;
            WalletXiropht.BeginInvoke((MethodInvoker) Invoke);
            SwitchFormPhase(ClassFormPhaseEnumeration.Main);
            ClassParallelForm.HidePinForm();
            ClassParallelForm.HideWaitingForm();
            ClassParallelForm.HideWaitingCreateWalletForm();
        }
    }
}