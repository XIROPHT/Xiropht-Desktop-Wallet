using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
#if WINDOWS
using MetroFramework;
#endif
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet.FormPhase.MainForm
{
    public partial class OpenWallet : Form
    {
        private string _walletFileData;
        public string _fileSelectedPath;

        public OpenWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        private void ButtonSearchWalletFile_Click(object sender, EventArgs e)
        {
            var openWalletFile = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Xiropht Wallet (*.xir) | *.xir",
                FilterIndex = 2,
                DereferenceLinks = false
            };
            if (openWalletFile.ShowDialog() == DialogResult.OK)
            {
                var threadReadWalletFileData = new Thread(delegate()
                {

                    _fileSelectedPath = openWalletFile.FileName;
                    labelOpenFileSelected.BeginInvoke((MethodInvoker) delegate()
                    {
                        labelOpenFileSelected.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_FILE_SELECTED_TEXT") + " " + openWalletFile.FileName;
                    });
                    try
                    {
                        var streamReaderWalletFile = new StreamReader(openWalletFile.FileName);
                        _walletFileData = streamReaderWalletFile.ReadToEnd();
                        streamReaderWalletFile.Close();
                        ClassWalletObject.WalletLastPathFile = openWalletFile.FileName;
                    }
                    catch
                    {
                    }
                });
                threadReadWalletFileData.Start();
            }
        }

        private void ButtonOpenYourWallet_Click(object sender, EventArgs e)
        {
             OpenAndConnectWallet();
        }

        /// <summary>
        /// Open and connect the wallet.
        /// </summary>
        /// <returns></returns>
        private void OpenAndConnectWallet()
        {
            if (textBoxPasswordWallet.Text == "")
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface( ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_CONTENT_TEXT"),
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_CONTENT_TEXT"),
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                return;
            }

            bool error = false;

            string passwordEncrypted = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael,
                textBoxPasswordWallet.Text, textBoxPasswordWallet.Text, ClassWalletNetworkSetting.KeySize);

            ClassWalletObject.WalletDataDecrypted = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                _walletFileData, passwordEncrypted, ClassWalletNetworkSetting.KeySize); // AES
            if (ClassWalletObject.WalletDataDecrypted == "WRONG")
            {
                error = true;
            }

            if (error)
            {
                ClassWalletObject.WalletDataDecrypted = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                    _walletFileData, textBoxPasswordWallet.Text, ClassWalletNetworkSetting.KeySize); // AES
            }

            try
            {
                if (ClassWalletObject.WalletDataDecrypted == "WRONG")
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    MessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    return;
                }

                var splitWalletFileDecrypted =
                    ClassWalletObject.WalletDataDecrypted.Split(new[] { "\n" }, StringSplitOptions.None);
                string walletAddress = splitWalletFileDecrypted[0];
                string walletKey = splitWalletFileDecrypted[1];
                new Thread(async delegate ()
                {
                    if (!await ClassWalletObject.InitializationWalletConnection(walletAddress, textBoxPasswordWallet.Text,
                    walletKey, ClassWalletPhase.Login))
                    {
                        MethodInvoker invoker = () => textBoxPasswordWallet.Text = "";
                        BeginInvoke(invoker);
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_TITLE_TEXT"), MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
#else
                        MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_TITLE_TEXT"), MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
#endif
                        return;
                    }

                    MethodInvoker invoke = () => textBoxPasswordWallet.Text = "";
                    BeginInvoke(invoke);

                    ClassWalletObject.ListenSeedNodeNetworkForWallet();

                    Stopwatch packetSpeedCalculator = new Stopwatch();
                    packetSpeedCalculator.Start();
                    if (await ClassWalletObject.WalletConnect.SendPacketWallet(ClassWalletObject.Certificate, string.Empty, false))
                    {

                        packetSpeedCalculator.Stop();
                        if (packetSpeedCalculator.ElapsedMilliseconds > 0)
                        {
                            Thread.Sleep((int)packetSpeedCalculator.ElapsedMilliseconds);
                        }
                        await ClassWalletObject.WalletConnect.SendPacketWallet(
                            "WALLET|" + ClassWalletObject.WalletConnect.WalletAddress, ClassWalletObject.Certificate, true);
                        _walletFileData = string.Empty;
                        _fileSelectedPath = string.Empty;
                        invoke = () => labelOpenFileSelected.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_FILE_SELECTED_TEXT");
                        BeginInvoke(invoke);
                    }
                }).Start();
            }
            catch
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_CONTENT_TEXT"),
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassFormPhase.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_CONTENT_TEXT"),
                    ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }
        }

        /// <summary>
        /// Get each control of the interface.
        /// </summary>
        public void GetListControl()
        {
            if (ClassFormPhase.WalletXiropht.ListControlSizeOpenWallet.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXiropht.ListControlSizeOpenWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }

        private void OpenWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXiropht.ResizeWalletInterface();
        }

        private void OpenWallet_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void TextBoxPasswordWallet_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Open wallet on press enter key.
            {
                OpenAndConnectWallet();
            }
        }
    }
}