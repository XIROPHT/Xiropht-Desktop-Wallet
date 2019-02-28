#if WINDOWS
using MetroFramework.Forms;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
#if WINDOWS
using MetroFramework;
using MetroFramework.Controls;
#endif
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.FormPhase;
using Xiropht_Wallet.FormPhase.MainForm;
using Xiropht_Wallet.FormPhase.ParallelForm;
using Xiropht_Wallet.Wallet;

namespace Xiropht_Wallet
{


#if WINDOWS
    public sealed partial class WalletXiropht : MetroForm
#else
    public sealed partial class WalletXiropht : Form
#endif
    {
        /// <summary>
        /// Form objects
        /// </summary>
        public OpenWallet OpenWalletForm;
        public MainWallet MainWalletForm;
        public OverviewWallet OverviewWalletForm;
        public TransactionHistoryWallet TransactionHistoryWalletForm;
        public SendTransactionWallet SendTransactionWalletForm;
        public CreateWallet CreateWalletForm;
        public BlockExplorerWallet BlockWalletForm;
        public RestoreWallet RestoreWalletForm;
        public ContactWallet ContactWalletForm;

        /// <summary>
        /// Form resize objects
        /// </summary>
        public int CurrentInterfaceWidth;
        public int CurrentInterfaceHeight;
        public int BaseInterfaceWidth;
        public int BaseInterfaceHeight;
        public List<Tuple<Size, Point>> ListControlSizeBase = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeMain = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeBlock = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeCreateWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeOpenWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeOverview = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeSendTransaction = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeTransaction = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeTransactionTabPage = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeRestoreWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeContactWallet = new List<Tuple<Size, Point>>();

        /// <summary>
        /// Threading
        /// </summary>
        private Thread _threadUpdateNetworkStats;
        private const int ThreadUpdateTransactionWalletInterval = 1000;
        private const int ThreadUpdateNetworkStatsInterval = 1000;
        private const int MaxTransactionPerPage = 100;
        private const int MaxBlockPerPage = 100;

        /// <summary>
        /// Boolean objects
        /// </summary>
        public bool EnableUpdateTransactionWallet;
        public bool EnableUpdateBlockWallet;
        private bool _isCopyWalletAddress;

        /// <summary>
        /// Objects transaction history & block explorer
        /// </summary>
        public Dictionary<int, string> ListTransactionHashShowed = new Dictionary<int, string>();
        public Dictionary<int, string> ListTransactionAnonymityHashShowed = new Dictionary<int, string>();
        public Dictionary<int, string> ListBlockHashShowed = new Dictionary<int, string>();
        public List<string> copyListTransaction = new List<string>();
        public List<string> copyListAnonymousTransaction = new List<string>();
        public int TotalTransactionRead;
        public int TotalAnonymityTransactionRead;
        public int TotalBlockRead;
        public int CurrentTransactionHistoryPageAnonymousReceived;
        public int CurrentTransactionHistoryPageAnonymousSend;
        public int CurrentTransactionHistoryPageNormalSend;
        public int CurrentTransactionHistoryPageNormalReceive;
        public int CurrentTransactionHistoryPageBlockReward;
        public int CurrentBlockExplorerPage;
        public int TotalTransactionAnonymousReceived;
        public int TotalTransactionAnonymousSend;
        public int TotalTransactionNormalReceived;
        public int TotalTransactionNormalSend;
        public int TotalTransactionBlockReward;
        private bool _normalTransactionLoaded;
        private bool _anonymousTransactionLoaded;
        private bool _firstStart;

     
        /// <summary>
        /// Constructor.
        /// </summary>
        public WalletXiropht(bool firstStart)
        {
            _firstStart = firstStart;
            ClassFormPhase.WalletXiropht = this;
            MainWalletForm = new MainWallet();
            OpenWalletForm = new OpenWallet();
            OverviewWalletForm = new OverviewWallet();
            TransactionHistoryWalletForm = new TransactionHistoryWallet();
            SendTransactionWalletForm = new SendTransactionWallet();
            CreateWalletForm = new CreateWallet();
            BlockWalletForm = new BlockExplorerWallet();
            RestoreWalletForm = new RestoreWallet();
            ContactWalletForm = new ContactWallet();
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// Update drawing of GDI+
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #region About Form Phase

        /// <summary>
        /// Change form from actual Form Phase.
        /// </summary>
        public void SwitchForm(string formPhase)
        {

            if (ClassFormPhase.FormPhase != formPhase)
            {
                HideAllFormAsync();
                ClassFormPhase.FormPhase = formPhase;
                MethodInvoker invoke;
                switch (formPhase)
                {
                    case ClassFormPhaseEnumeration.Main:
                        invoke = () =>
                        {
                            MainWalletForm.TopLevel = false;
                            MainWalletForm.AutoScroll = false;
                            MainWalletForm.Parent = panelMainForm;
                            MainWalletForm.Show();
                            MainWalletForm.Refresh();
                        };

                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.CreateWallet:
                        invoke = () =>
                        {
                            CreateWalletForm.TopLevel = false;
                            CreateWalletForm.AutoScroll = false;
                            CreateWalletForm.Parent = panelMainForm;
                            CreateWalletForm.Show();
                            CreateWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.OpenWallet:
                        invoke = () =>
                        {
                            OpenWalletForm.TopLevel = false;
                            OpenWalletForm.AutoScroll = false;
                            OpenWalletForm.Parent = panelMainForm;
                            OpenWalletForm.Show();
                            OpenWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.Overview:
                        invoke = () =>
                        {
                            OverviewWalletForm.TopLevel = false;
                            OverviewWalletForm.AutoScroll = false;
                            OverviewWalletForm.Parent = panelMainForm;
                            OverviewWalletForm.Show();
                            OverviewWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.SendTransaction:
                        invoke = () =>
                        {
                            SendTransactionWalletForm.TopLevel = false;
                            SendTransactionWalletForm.AutoScroll = false;
                            SendTransactionWalletForm.Parent = panelMainForm;
                            SendTransactionWalletForm.Show();
                            SendTransactionWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.TransactionHistory:
                        invoke = () =>
                        {
                            TransactionHistoryWalletForm.TopLevel = false;
                            TransactionHistoryWalletForm.AutoScroll = false;
                            TransactionHistoryWalletForm.Parent = panelMainForm;
                            TransactionHistoryWalletForm.Show();
                            TransactionHistoryWalletForm.Refresh();
                            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Refresh();
                            buttonPreviousPage.Show();
                            buttonNextPage.Show();
                            buttonFirstPage.Show();
                            buttonLastPage.Show();
                            labelNoticeCurrentPage.Show();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.BlockExplorer:
                        invoke = () =>
                        {
                            BlockWalletForm.TopLevel = false;
                            BlockWalletForm.AutoScroll = false;
                            BlockWalletForm.Parent = panelMainForm;
                            BlockWalletForm.Show();
                            BlockWalletForm.Refresh();
                            BlockWalletForm.listViewBlockExplorer.Refresh();
                            buttonPreviousPage.Show();
                            buttonNextPage.Show();
                            buttonFirstPage.Show();
                            buttonLastPage.Show();
                            labelNoticeCurrentPage.Show();
                            labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.RestoreWallet:
                        invoke = () =>
                        {
                            RestoreWalletForm.TopLevel = false;
                            RestoreWalletForm.AutoScroll = false;
                            RestoreWalletForm.Parent = panelMainForm;
                            RestoreWalletForm.Show();
                            RestoreWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.ContactWallet:
                        invoke = () =>
                        {
                            ContactWalletForm.TopLevel = false;
                            ContactWalletForm.AutoScroll = false;
                            ContactWalletForm.Parent = panelMainForm;
                            ContactWalletForm.Show();
                            ContactWalletForm.Refresh();
                        };
                        BeginInvoke(invoke);
                        break;
                }
            }
        }

        /// <summary>
        /// Hide all form.
        /// </summary>
        private void HideAllFormAsync()
        {
            BeginInvoke((MethodInvoker)delegate ()
            {
                MainWalletForm.Hide();

                panelMainForm.Controls.Clear();

                OpenWalletForm.Hide();

                OverviewWalletForm.Hide();

                TransactionHistoryWalletForm.Hide();

                SendTransactionWalletForm.Hide();

                BlockWalletForm.Hide();

                RestoreWalletForm.Hide();

                ContactWalletForm.Hide();

                buttonPreviousPage.Hide();

                buttonNextPage.Hide();

                buttonFirstPage.Hide();

                buttonLastPage.Hide();

                labelNoticeCurrentPage.Hide();

            });
        }

        /// <summary>
        /// Update current page number of transaction history
        /// </summary>
        public void UpdateCurrentPageNumberTransactionHistory()
        {
            MethodInvoker invoke = null;
            if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
            {
                invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
            }
            if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible) // Normal transaction received list
            {
                invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
            }
            if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
            {
                invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
            }
            if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
            {
                invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
            }
            if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible) // block reward transaction list 
            {
                invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
            }

            BeginInvoke(invoke);
            
        }

        /// <summary>
        /// Update overview network stats automaticaly.
        /// </summary>
        public void UpdateNetworkStats()
        {
            if (_threadUpdateNetworkStats != null &&
                (_threadUpdateNetworkStats.IsAlive || _threadUpdateNetworkStats != null))
            {
                _threadUpdateNetworkStats.Abort();
                GC.SuppressFinalize(_threadUpdateNetworkStats);
            }

            _threadUpdateNetworkStats = new Thread(delegate ()
            {
                while (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {

                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            if (!string.IsNullOrEmpty(ClassWalletObject.TotalBlockMined) &&
                                !string.IsNullOrEmpty(ClassWalletObject.CoinCirculating) &&
                                !string.IsNullOrEmpty(ClassWalletObject.CoinMaxSupply) &&
                                !string.IsNullOrEmpty(ClassWalletObject.NetworkDifficulty) &&
                                !string.IsNullOrEmpty(ClassWalletObject.NetworkHashrate) &&
                                !string.IsNullOrEmpty(ClassWalletObject.TotalFee) &&
                                !string.IsNullOrEmpty(ClassWalletObject.LastBlockFound))
                            {

                                UpdateOverviewLabelBlockMined(ClassWalletObject.TotalBlockMined);
                                UpdateOverviewLabelCoinCirculating(ClassWalletObject.CoinCirculating);
                                UpdateOverviewLabelCoinMaxSupply(ClassWalletObject.CoinMaxSupply);
                                UpdateOverviewLabelNetworkDifficulty(ClassWalletObject.NetworkDifficulty);
                                UpdateOverviewLabelNetworkHashrate(ClassWalletObject.NetworkHashrate);
                                UpdateOverviewLabelTransactionFee(ClassWalletObject.TotalFee);
                                UpdateOverviewLabelLastBlockFound(ClassWalletObject.LastBlockFound);

                                MethodInvoker invoke = () => labelNoticeTotalPendingTransactionOnReceive.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_TOTAL_PENDING_TRANSACTION_ON_RECEIVE_TEXT") + " " + ClassWalletObject.TotalTransactionPendingOnReceive;
                                BeginInvoke(invoke);
                            }
                        }
                    }


                    Thread.Sleep(ThreadUpdateNetworkStatsInterval);
                }
            })
            {
                IsBackground = true
            };
            _threadUpdateNetworkStats.Start();
        }

#endregion

        #region Event

        /// <summary>
        /// Event activated on load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXiropht_Load(object sender, EventArgs e)
        {
            Text += Assembly.GetExecutingAssembly().GetName().Version;

            // Initialize form size.
            CurrentInterfaceWidth = Width;
            CurrentInterfaceHeight = Height;
            BaseInterfaceHeight = Height;
            BaseInterfaceWidth = Width;
            ClassFormPhase.InitializeMainInterface(this);
            labelCoinName.Text = "Coin Name: " + ClassConnectorSetting.CoinName;
            labelNetworkPhase.Text = "Network Phase: " + ClassConnectorSetting.NetworkPhase;
            if (ListControlSizeBase.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ListControlSizeBase.Add(new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
            CurrentTransactionHistoryPageAnonymousReceived = 1;
            CurrentTransactionHistoryPageAnonymousSend = 1;
            CurrentTransactionHistoryPageNormalSend = 1;
            CurrentTransactionHistoryPageNormalReceive = 1;
            CurrentTransactionHistoryPageBlockReward = 1;
            CurrentBlockExplorerPage = 1;
            Width += 10;
            Height += 10;
            // Initialize language.
            foreach (string key in ClassTranslation.LanguageDatabases.Keys)
            {
                languageToolStripMenuItem.DropDownItems.Add(ClassTranslation.UppercaseFirst(key), null, LanguageSubMenuItem_Click);
            }
            UpdateGraphicLanguageText();
            new Thread(delegate()
            {
#if WINDOWS
                MethodInvoker invoke = () =>
                {
                    if(ClassFormPhase.MessageBoxInterface(ClassConnectorSetting.CoinName + " is currently in private test, we suggest to not invest your money on it, invest your time only because we are in private test and we need something stable, usefull, secure for provide a real trust on this coin before. Thank you for your understanding.", "Important information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        if (_firstStart)
                        {
                            var firstStartForm = new FirstStartWallet();
                            firstStartForm.ShowDialog(this);
                        }
                    }
                };
                BeginInvoke(invoke);
#else
                MethodInvoker invoke = () => 
                {
                    if (MessageBox.Show(this, ClassConnectorSetting.CoinName + " is currently in private test, we suggest to not invest your money on it, invest your time only because we are in private test and we need something stable, usefull, secure for provide a real trust on this coin before. Thank you for your understanding.", "Important information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        if (_firstStart)
                        {
                            var firstStartForm = new FirstStartWallet();
                            firstStartForm.ShowDialog(this);
                        }
                    }
                };
               BeginInvoke(invoke);
#endif
            }).Start();
            UpdateColorStyle(Color.White, Color.Black, Color.White);

        }

        /// <summary>
        /// Event click for change current language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSubMenuItem_Click(object sender, EventArgs e)
        {
            var tooltipitem = (ToolStripItem)sender;
            if(ClassTranslation.ChangeCurrentLanguage(tooltipitem.Text))
            {
                ClassWalletSetting.SaveSetting();
                UpdateGraphicLanguageText();
            }
        }

        /// <summary>
        /// Update graphic language text.
        /// </summary>
        public void UpdateGraphicLanguageText()
        {
            // Main Interface.
            fileToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_TEXT");
            mainMenuToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_MAIN_MENU_TEXT");
            createWalletToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_CREATE_WALLET_MENU_TEXT");
            openWalletToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_OPEN_WALLET_MENU_TEXT");
            restoreWalletToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_RESTORE_WALLET_MENU_TEXT");
            closeWalletToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_FUNCTION_CLOSE_WALLET_TEXT");
            exitToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_FILE_FUNCTION_EXIT_TEXT");
            languageToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_LANGUAGE_TEXT");
            settingToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_SETTING_TEXT");
            securityToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_SETTING_SECURITY_TEXT");
            changePasswordToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SECURITY_CONFIG_CHANGE_PASSWORD_TEXT");
            settingPinCodeToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SECURITY_CONFIG_PIN_CODE_TEXT");
            syncToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_SETTING_SYNC_TEXT");
            remoteNodeSettingToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SYNC_REMOTE_NODE_CONFIG_TEXT");
            resyncTransactionToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_RESYNC_TRANSACTION_TEXT");
            resyncBlockToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_RESYNC_BLOCK_TEXT");
            helpToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_HELP_TEXT");
            aboutToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_HELP_ABOUT_TEXT");
            if (ClassWalletObject.WalletConnect != null)
            {
                bool showPendingAmount = false;
                if (ClassWalletObject.WalletAmountInPending != null)
                {
                    if (!string.IsNullOrEmpty(ClassWalletObject.WalletAmountInPending))
                    {
                        showPendingAmount = true;
                    }
                }
                if (!showPendingAmount)
                {
                    labelNoticeWalletBalance.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT") + " " + ClassWalletObject.WalletConnect.WalletAmount + " " + ClassConnectorSetting.CoinNameMin;
                }
                else
                {
                    labelNoticeWalletBalance.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT") + " " + ClassWalletObject.WalletConnect.WalletAmount + " " + ClassConnectorSetting.CoinNameMin + " | " + ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_PENDING_BALANCE_TEXT") + " " + ClassWalletObject.WalletAmountInPending + " " + ClassConnectorSetting.CoinNameMin;
                }
                labelNoticeWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_ADDRESS_TEXT") + " " + ClassWalletObject.WalletConnect.WalletAddress;
                labelNoticeTotalPendingTransactionOnReceive.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_TOTAL_PENDING_TRANSACTION_ON_RECEIVE_TEXT") + " " + ClassWalletObject.TotalTransactionPendingOnReceive;
            }
            else
            {
                labelNoticeWalletBalance.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT");
                labelNoticeWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_ADDRESS_TEXT");
                labelNoticeTotalPendingTransactionOnReceive.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_TOTAL_PENDING_TRANSACTION_ON_RECEIVE_TEXT");
            }
            buttonOverviewWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_OVERVIEW_TEXT");
            metroButtonSendTransactionWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_SEND_TRANSACTION_TEXT");
            metroButtonTransactionWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_TRANSACTION_HISTORY_TEXT");
            metroButtonBlockExplorerWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_BLOCK_EXPLORER_TEXT");
            buttonContactWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_CONTACT_TEXT");
#if WINDOWS
            Refresh();
#endif
            // Block explorer menu.
            BlockWalletForm.listViewBlockExplorer.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_ID_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_HASH_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_REWARD_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DIFFICULTY_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_CREATE_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_FOUND_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_TRANSACTION_HASH_TEXT");
#if WINDOWS
            BlockWalletForm.Refresh();
#endif
            // Create wallet menu.
            CreateWalletForm.labelCreateYourWallet.Text = ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_TITLE_TEXT");
            CreateWalletForm.labelCreateNoticePasswordRequirement.Text = ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_PASSWORD_REQUIREMENT_TEXT");
            CreateWalletForm.labelCreateSelectSavingPathWallet.Text = ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_SELECT_WALLET_FILE_TEXT");
            CreateWalletForm.labelCreateSelectWalletPassword.Text = ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_INPUT_WALLET_PASSWORD_TEXT");
            CreateWalletForm.buttonCreateYourWallet.Text = ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_BUTTON_SUBMIT_CREATE_TEXT");
#if WINDOWS
            CreateWalletForm.Refresh();
#endif
            // Main wallet menu.
            MainWalletForm.labelNoticeWelcomeWallet.Text = ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_LABEL_WELCOME_INFORMATION_TEXT");
            MainWalletForm.labelNoticeLanguageAndIssue.Text = ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_LABEL_HELP_INFORMATION_TEXT");
            MainWalletForm.buttonMainOpenMenuWallet.Text = ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_BUTTON_OPEN_WALLET_MENU_TEXT");
            MainWalletForm.buttonMainCreateWallet.Text = ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_BUTTON_CREATE_WALLET_MENU_TEXT");
#if WINDOWS
            MainWalletForm.Refresh();
#endif
            // Open wallet menu.
            OpenWalletForm.labelOpenYourWallet.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_TITLE_TEXT");
            OpenWalletForm.labelOpenFileSelected.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_FILE_SELECTED_TEXT") + " " + OpenWalletForm._fileSelectedPath;

            OpenWalletForm.labelWriteYourWalletPassword.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_YOUR_PASSWORD_TEXT");
            OpenWalletForm.buttonSearchWalletFile.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_BUTTON_SEARCH_WALLET_FILE_TEXT");
            OpenWalletForm.buttonOpenYourWallet.Text = ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_BUTTON_SUBMIT_WALLET_FILE_TEXT");
#if WINDOWS
            OpenWalletForm.Refresh();
#endif

            OverviewWalletForm.labelTextNetworkStats.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TITLE_TEXT");
            try
            {
                // Overview wallet menu.
                if (ClassWalletObject.WalletConnect != null)
                {
                    UpdateOverviewLabelBlockMined(ClassWalletObject.TotalBlockMined);
                    UpdateOverviewLabelCoinCirculating(ClassWalletObject.CoinCirculating);
                    UpdateOverviewLabelCoinMaxSupply(ClassWalletObject.CoinMaxSupply);
                    UpdateOverviewLabelNetworkDifficulty(ClassWalletObject.NetworkDifficulty);
                    UpdateOverviewLabelNetworkHashrate(ClassWalletObject.NetworkHashrate);
                    UpdateOverviewLabelTransactionFee(ClassWalletObject.TotalFee);
                    UpdateOverviewLabelLastBlockFound(ClassWalletObject.LastBlockFound);
                }
                else
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_MAX_SUPPLY_TEXT");
                    OverviewWalletForm.labelTextCoinCirculating.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_CIRCULATING_TEXT");
                    OverviewWalletForm.labelTextTransactionFee.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TRANSACTION_FEE_ACCUMULATED_TEXT");
                    OverviewWalletForm.labelTextCoinMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_MINED_TEXT");
                    OverviewWalletForm.labelTextBlockchainHeight.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_BLOCKCHAIN_HEIGHT_TEXT");
                    OverviewWalletForm.labelTextTotalBlockMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_MINED_TEXT");
                    OverviewWalletForm.labelTextTotalBlockLeft.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_LEFT_TEXT");
                    OverviewWalletForm.labelTextNetworkDifficulty.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_DIFFICULTY_TEXT");
                    OverviewWalletForm.labelTextNetworkHashrate.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_HASHRATE_TEXT");
                    OverviewWalletForm.labelTextLastBlockFound.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_LAST_BLOCK_FOUND_TEXT");
                    OverviewWalletForm.labelTextTotalCoinInPending.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_PENDING");
                }
            }
            catch
            {

            }
#if WINDOWS
            OverviewWalletForm.Refresh();
#endif

            // Send transaction wallet menu.
            SendTransactionWalletForm.labelSendTransaction.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_TITLE_TEXT");
            SendTransactionWalletForm.labelWalletDestination.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_WALLET_ADDRESS_TARGET_TEXT");
            SendTransactionWalletForm.labelAmount.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_AMOUNT_TEXT");
            SendTransactionWalletForm.labelFeeTransaction.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_FEE_TEXT");
            SendTransactionWalletForm.metroLabelEstimatedTimeReceived.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_ESTIMATED_RECEIVE_TIME_TEXT");
            SendTransactionWalletForm.checkBoxHideWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_CHECKBOX_OPTION_ANONYMITY_TEXT");
            SendTransactionWalletForm.buttonSendTransaction.Text = ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_BUTTON_SUBMIT_TRANSACTION_TEXT");
#if WINDOWS
            SendTransactionWalletForm.Refresh();
#endif

            // Transaction history wallet menu.
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[7].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[8].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[7].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[8].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[7].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[8].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[7].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[8].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[3].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[4].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[5].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[6].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[7].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[8].Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.tabPageNormalTransactionSend.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_TAB_NORMAL_SEND_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_SEND_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_TAB_NORMAL_RECEIVED_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_RECEIVE_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_TAB_BLOCK_REWARD_RECEIVED_LIST_TEXT");
#if WINDOWS
            TransactionHistoryWalletForm.Refresh();
#endif
            // Restore wallet menu.
            RestoreWalletForm.labelTextRestore.Text = ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_TITLE_TEXT");
            RestoreWalletForm.labelCreateSelectSavingPathWallet.Text = ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_SELECT_PATH_FILE_TEXT");
            RestoreWalletForm.labelPrivateKey.Text = ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_PRIVATE_KEY_TEXT");
            RestoreWalletForm.labelPassword.Text = ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_PASSWORD_TEXT");
            RestoreWalletForm.buttonRestoreYourWallet.Text = ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_BUTTON_SUBMIT_RESTORE_TEXT");
#if WINDOWS
            RestoreWalletForm.Refresh();
#endif

            // Contact wallet menu.
            ContactWalletForm.buttonAddContact.Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_BUTTON_ADD_CONTACT_TEXT");
            ContactWalletForm.listViewExContact.Columns[0].Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_NAME_TEXT");
            ContactWalletForm.listViewExContact.Columns[1].Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_ADDRESS_TEXT");
            ContactWalletForm.listViewExContact.Columns[2].Text = ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_ACTION_TEXT");
            ContactWalletForm.Refresh();
        }

        /// <summary>
        /// Event click from menu for switch to main phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(delegate ()
            {
                if (ClassWalletObject.WalletConnect != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            ClassWalletObject.FullDisconnection(true);
                        }
                    }
                }
            }).ConfigureAwait(false);

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
        }

        /// <summary>
        /// Event click from menu for switch to create wallet phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(delegate ()
            {
                if (ClassWalletObject.WalletConnect != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            ClassWalletObject.FullDisconnection(true);
                        }
                    }
                }
            }).ConfigureAwait(false);
            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.CreateWallet);
        }

        /// <summary>
        /// Event click from menu for switch to open wallet phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(delegate ()
            {
                if (ClassWalletObject.WalletConnect != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            ClassWalletObject.FullDisconnection(true);
                        }
                    }
                }
            }).ConfigureAwait(false);

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.OpenWallet);
        }

        /// <summary>
        /// Event click from menu for exit the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Event enable when the interface form of the wallet is closed, used in case for force to kill the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXiropht_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Event enable when the interface form of the wallet is closing, used in case for force to kill the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXiropht_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Switch to overview menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOverviewWallet_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.SeedNodeConnectorWallet != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
                }
            }
        }

        /// <summary>
        /// Close wallet and switch to main menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(delegate ()
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                    {
                        ClassWalletObject.FullDisconnection(true);
                    }
                }
            });
            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
        }

        /// <summary>
        /// Switch to send transaction menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonSendTransactionWallet_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.SeedNodeConnectorWallet != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.SendTransaction);
                }
            }
        }

        /// <summary>
        /// Switch to transaction history menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonTransactionWallet_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.SeedNodeConnectorWallet != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.TransactionHistory);
                }
            }
        }

        /// <summary>
        /// Switch to block explorer menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonBlockExplorerWallet_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.SeedNodeConnectorWallet != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.BlockExplorer);
                }
            }
        }


        /// <summary>
        /// Refresh automaticaly the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            UpdateStyles();
#if WINDOWS
            if (Width < BaseInterfaceWidth)
            {
                Width = BaseInterfaceWidth;
            }
            else if (Width == BaseInterfaceWidth)
            {
                Width += 10;
            }

            if (Height < BaseInterfaceHeight)
            {
                Height = BaseInterfaceHeight;
            }
            else if (Height == BaseInterfaceHeight)
            {
                Height += 10;
            }
#endif
            if (ClassFormPhase.WalletXiropht != null) // Get list of all controls of each menu.
            {
                MainWalletForm.GetListControl();
                OverviewWalletForm.GetListControl();
                CreateWalletForm.GetListControl();
                BlockWalletForm.GetListControl();
                OpenWalletForm.GetListControl();
                SendTransactionWalletForm.GetListControl();
                TransactionHistoryWalletForm.GetListControl();
                RestoreWalletForm.GetListControl();
                ContactWalletForm.GetListControl();
            }
        }

        /// <summary>
        /// Open change password menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                var changeWalletPassword = new ChangeWalletPasswordWallet { StartPosition = FormStartPosition.CenterParent };
                changeWalletPassword.ShowDialog(this);
            }
        }

        /// <summary>
        /// Open pin code setting menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingPinCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                var pinCodeSetting = new PinCodeSettingWallet { StartPosition = FormStartPosition.CenterParent };
                pinCodeSetting.ShowDialog(this);
            }
        }

        /// <summary>
        /// Resync manually transaction history.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resyncTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                    {

#if WINDOWS
                        if (MetroMessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                        {
                            TransactionHistoryWalletForm.ResyncTransaction();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Open remote node setting menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteNodeSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                var remoteNodeSetting = new RemoteNodeSettingWallet { StartPosition = FormStartPosition.CenterParent };
                remoteNodeSetting.ShowDialog(this);
            }
        }

        /// <summary>
        /// Detect when the size of the interface change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXiropht_SizeChanged(object sender, EventArgs e)
        {
#if WINDOWS
            if (Width < BaseInterfaceWidth)
            {
                Width = BaseInterfaceWidth;
            }

            if (Height < BaseInterfaceHeight)
            {
                Height = BaseInterfaceHeight;
            }
#endif
            //Refresh();
        }

        /// <summary>
        /// Event call when the user want to resize the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXiropht_Resize(object sender, EventArgs e)
        {
            ResizeWalletInterface();
        }

        /// <summary>
        /// Resync block explorer manually.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resyncBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                    {
#if WINDOWS
                        if (MetroMessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_BLOCK_EXPLORER_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_BLOCK_EXPLORER_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_BLOCK_EXPLORER_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_BLOCK_EXPLORER_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                        {
                            BlockWalletForm.ResyncBlock();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Go to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
                    {
                        if (CurrentTransactionHistoryPageNormalSend > 1)
                        {
                            CurrentTransactionHistoryPageNormalSend--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible) // Normal transaction received list
                    {
                        if (CurrentTransactionHistoryPageNormalReceive > 1)
                        {
                            CurrentTransactionHistoryPageNormalReceive--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
                    {
                        if (CurrentTransactionHistoryPageAnonymousSend > 1)
                        {
                            CurrentTransactionHistoryPageAnonymousSend--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
                    {
                        if (CurrentTransactionHistoryPageAnonymousReceived > 1)
                        {
                            CurrentTransactionHistoryPageAnonymousReceived--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible) // block reward transaction list 
                    {
                        if (CurrentTransactionHistoryPageBlockReward > 1)
                        {
                            CurrentTransactionHistoryPageBlockReward--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                if (CurrentBlockExplorerPage > 1)
                {
                    CurrentBlockExplorerPage--;
                    labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                    StopUpdateBlockHistory(false, true);
                }
            }
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
                    {
                        int difference = (TotalTransactionNormalSend + MaxTransactionPerPage) - TotalTransactionNormalSend;
                        if ((CurrentTransactionHistoryPageNormalSend + 1) * MaxTransactionPerPage <= (TotalTransactionNormalSend + difference))
                        {
                            CurrentTransactionHistoryPageNormalSend++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible) // Normal transaction received list
                    {
                        int difference = (TotalTransactionNormalReceived + MaxTransactionPerPage) - TotalTransactionNormalReceived;

                        if ((CurrentTransactionHistoryPageNormalReceive + 1) * MaxTransactionPerPage <= (TotalTransactionNormalReceived + difference))
                        {
                            CurrentTransactionHistoryPageNormalReceive++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
                    {
                        int difference = (TotalTransactionAnonymousSend + MaxTransactionPerPage) - TotalTransactionAnonymousSend;

                        if ((CurrentTransactionHistoryPageAnonymousSend + 1) * MaxTransactionPerPage <= TotalTransactionAnonymousSend + difference)
                        {
                            CurrentTransactionHistoryPageAnonymousSend++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
                    {
                        int difference = (TotalTransactionAnonymousReceived + MaxTransactionPerPage) - TotalTransactionAnonymousReceived;
                        if ((CurrentTransactionHistoryPageAnonymousReceived + 1) * MaxTransactionPerPage <= TotalTransactionAnonymousReceived + difference)
                        {
                            CurrentTransactionHistoryPageAnonymousReceived++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible) // block reward transaction list 
                    {
                        int difference = (TotalTransactionBlockReward + MaxTransactionPerPage) - TotalTransactionBlockReward;

                        if ((CurrentTransactionHistoryPageBlockReward + 1) * MaxTransactionPerPage <= TotalTransactionBlockReward + difference)
                        {
                            CurrentTransactionHistoryPageBlockReward++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                            StopUpdateTransactionHistory(false, false, true);
                        }
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                int difference = (TotalBlockRead + MaxBlockPerPage) - TotalBlockRead;
                if ((CurrentBlockExplorerPage + 1) * MaxBlockPerPage <= TotalBlockRead + difference)
                {
                    CurrentBlockExplorerPage++;
                    labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                    StopUpdateBlockHistory(false, true);
                }
            }
        }

        /// <summary>
        /// Go to the first page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFirstPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
                    {
                        CurrentTransactionHistoryPageNormalSend = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible) // Normal transaction received list
                    {

                        CurrentTransactionHistoryPageNormalReceive = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
                    {

                        CurrentTransactionHistoryPageAnonymousSend = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
                    {

                        CurrentTransactionHistoryPageAnonymousReceived = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible) // block reward transaction list 
                    {

                        CurrentTransactionHistoryPageBlockReward = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                        StopUpdateTransactionHistory(false, false, true);

                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                CurrentBlockExplorerPage = 1;
                labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                StopUpdateBlockHistory(false, true);
            }
        }

        /// <summary>
        /// Got to the last page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLastPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
                    {
                        float numberParge = ((float)TotalTransactionNormalSend / MaxTransactionPerPage);
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0)
                        {
                            numberParge = 1;
                        }
                        CurrentTransactionHistoryPageNormalSend = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible) // Normal transaction received list
                    {
                        float numberParge = ((float)TotalTransactionNormalReceived / MaxTransactionPerPage);
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0)
                        {
                            numberParge = 1;
                        }
                        CurrentTransactionHistoryPageNormalReceive = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
                    {
                        float numberParge = ((float)TotalTransactionAnonymousSend / MaxTransactionPerPage);
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0)
                        {
                            numberParge = 1;
                        }
                        CurrentTransactionHistoryPageAnonymousSend = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
                    {
                        float numberParge = ((float)TotalTransactionAnonymousReceived / MaxTransactionPerPage);
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0)
                        {
                            numberParge = 1;
                        }
                        CurrentTransactionHistoryPageAnonymousReceived = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousReceived;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible) // block reward transaction list 
                    {

                        float numberParge = ((float)TotalTransactionBlockReward / MaxTransactionPerPage);
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0)
                        {
                            numberParge = 1;
                        }
                        CurrentTransactionHistoryPageBlockReward = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                        StopUpdateTransactionHistory(false, false, true);
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                float numberParge = ((float)(ClassBlockCache.ListBlock.Count - 1) / MaxBlockPerPage);
                numberParge += 0.5f;
                numberParge = (float)Math.Round(numberParge, 0);
                if (numberParge <= 0)
                {
                    numberParge = 1;
                }
                CurrentBlockExplorerPage = (int)numberParge;
                labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                StopUpdateBlockHistory(false, true);

            }
        }

        /// <summary>
        /// Copy the wallet address by click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelNoticeWalletAddress_Click(object sender, EventArgs e)
        {

            if (!_isCopyWalletAddress)
            {
                _isCopyWalletAddress = true;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    LinuxClipboard.SetText(ClassWalletObject.WalletConnect.WalletAddress);
                }
                else // Windows (normaly)
                {
                    Clipboard.SetText(ClassWalletObject.WalletConnect.WalletAddress);
                }
                new Thread(delegate ()
                {
                    string oldText = labelNoticeWalletAddress.Text;
                    var oldColor = labelNoticeWalletAddress.ForeColor;
                    MethodInvoker invoke = () => labelNoticeWalletAddress.Text = oldText + @" copied.";
                    BeginInvoke(invoke);
                    invoke = () => labelNoticeWalletAddress.ForeColor = Color.Lime;
                    BeginInvoke(invoke);
                    Thread.Sleep(1000);
                    invoke = () => labelNoticeWalletAddress.ForeColor = oldColor;
                    BeginInvoke(invoke);
                    invoke = () => labelNoticeWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_ADDRESS_TEXT") + " " + ClassWalletObject.WalletConnect.WalletAddress;
                     _isCopyWalletAddress = false;
                    
                }).Start();
            }
        }

        /// <summary>
        /// Switch to wallet restore menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restoreWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(delegate ()
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                    {
                        ClassWalletObject.FullDisconnection(true);
                    }
                }
            });
            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.RestoreWallet);
        }

        /// <summary>
        /// Open about menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutMenu = new AboutWallet { StartPosition = FormStartPosition.CenterParent };
            aboutMenu.ShowDialog(this);
        }

        /// <summary>
        /// Open about menu by clicking on copyright.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelCopyright_Click(object sender, EventArgs e)
        {
            var aboutMenu = new AboutWallet { StartPosition = FormStartPosition.CenterParent };
            aboutMenu.ShowDialog(this);
        }

        /// <summary>
        /// Reach the official website.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://xiropht.com/");
        }


        /// <summary>
        /// Switch to contact menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactWallet_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.SeedNodeConnectorWallet != null)
            {
                if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                {
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.ContactWallet);
                }
            }
        }
    
        #endregion

        #region About Wallet Sync

        /// <summary>
        /// Update overview label coin max supply
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelCoinMaxSupply(string info)
        {

            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_MAX_SUPPLY_TEXT") + " " +
                        info.ToString(Program.GlobalCultureInfo) + " " + ClassConnectorSetting.CoinNameMin;
                }
                catch
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_MAX_SUPPLY_TEXT");
                }
            }

            BeginInvoke((MethodInvoker)MethodInvoker);

        }

        /// <summary>
        /// Update overview label coin circulating
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelCoinCirculating(string info)
        {
            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextCoinCirculating.Text =
                        ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_CIRCULATING_TEXT") + " " +
                        info.ToString(Program.GlobalCultureInfo) + " " + ClassConnectorSetting.CoinNameMin;
                }
                catch
                {
                    ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_CIRCULATING_TEXT");
                }
            }


            BeginInvoke((MethodInvoker)MethodInvoker);

        }

        /// <summary>
        /// Update overview label block mined and some others
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelBlockMined(string info)
        {
            try
            {
                var totalBlockLeft =
                    Math.Round(
                        (Decimal.Parse(ClassWalletObject.CoinMaxSupply.Replace(".", ","), NumberStyles.Any,
                             Program.GlobalCultureInfo) / 10) - Decimal.Parse(info), 0);
                Decimal totalCoinMined = Decimal.Parse(info) * 10;
                Decimal totalInPending = totalCoinMined - (Decimal.Parse(ClassWalletObject.CoinCirculating.Replace(".", ","), NumberStyles.Any, Program.GlobalCultureInfo) + Decimal.Parse(ClassWalletObject.TotalFee.Replace(".", ","), NumberStyles.Any, Program.GlobalCultureInfo));
                int blockchainHeight = (int.Parse(info) + 1);


                MethodInvoker invoke = () =>
                {
                    OverviewWalletForm.labelTextCoinMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_MINED_TEXT") + " " + totalCoinMined.ToString(Program.GlobalCultureInfo) + " " + ClassConnectorSetting.CoinNameMin;
                    OverviewWalletForm.labelTextBlockchainHeight.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_BLOCKCHAIN_HEIGHT_TEXT") + " " + blockchainHeight;
                    OverviewWalletForm.labelTextTotalBlockMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_MINED_TEXT") + " " + info;
                    OverviewWalletForm.labelTextTotalBlockLeft.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_LEFT_TEXT") + " " + totalBlockLeft;
                    OverviewWalletForm.labelTextTotalCoinInPending.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_PENDING") + " " + totalInPending.ToString(Program.GlobalCultureInfo) + " " + ClassConnectorSetting.CoinNameMin;
                };
                BeginInvoke(invoke);
            }
            catch
            {
                OverviewWalletForm.labelTextCoinMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_MINED_TEXT");
                OverviewWalletForm.labelTextBlockchainHeight.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_BLOCKCHAIN_HEIGHT_TEXT");
                OverviewWalletForm.labelTextTotalBlockMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_MINED_TEXT");
                OverviewWalletForm.labelTextTotalBlockLeft.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_LEFT_TEXT");
                OverviewWalletForm.labelTextTotalCoinInPending.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_PENDING");
            }

        }

        /// <summary>
        /// Update overview label network difficulty
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelNetworkDifficulty(string info)
        {

            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextNetworkDifficulty.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_DIFFICULTY_TEXT") + " " + info;
                }
                catch
                {
                    OverviewWalletForm.labelTextNetworkDifficulty.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_DIFFICULTY_TEXT");
                }
            }
            BeginInvoke((MethodInvoker)MethodInvoker);

        }

        /// <summary>
        /// Update overview label network hashrate;
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelNetworkHashrate(string info)
        {
            try
            {
                info = Xiropht_Connector_All.Utils.ClassUtils.GetTranslateHashrate(info, 2);
                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextNetworkHashrate.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_HASHRATE_TEXT") + " " + info;
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextNetworkHashrate.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_HASHRATE_TEXT");
                    }
                }
                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Update overview label total fee.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelTransactionFee(string info)
        {
            try
            {
                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextTransactionFee.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TRANSACTION_FEE_ACCUMULATED_TEXT") + " " +
                        info.ToString(Program.GlobalCultureInfo) + " " + ClassConnectorSetting.CoinNameMin;
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextTransactionFee.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TRANSACTION_FEE_ACCUMULATED_TEXT");
                    }

                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Update label sync information.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateLabelSyncInformation(string info)
        {
            void MethodInvoker() => labelSyncInformation.Text = info;
            BeginInvoke((MethodInvoker) MethodInvoker);
        }

        /// <summary>
        /// Update overview label last block found.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelLastBlockFound(string info)
        {
            try
            {
                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextLastBlockFound.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_LAST_BLOCK_FOUND_TEXT") + " " + info;
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextLastBlockFound.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_LAST_BLOCK_FOUND_TEXT");
                    }
                }
                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Clean wallet sync interface
        /// </summary>
        public void CleanSyncInterfaceWallet()
        {
            MethodInvoker invoke = () =>
            {
               
                labelNoticeWalletAddress.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_ADDRESS_TEXT");
                labelNoticeWalletBalance.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT");
                OverviewWalletForm.labelTextCoinMaxSupply.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_MAX_SUPPLY_TEXT") + " In Sync";
                OverviewWalletForm.labelTextCoinCirculating.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_COIN_CIRCULATING_TEXT") + " In Sync";
                OverviewWalletForm.labelTextTransactionFee.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TRANSACTION_FEE_ACCUMULATED_TEXT") + " In Sync";
                OverviewWalletForm.labelTextCoinMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_COIN_MINED_TEXT") + " In Sync";
                OverviewWalletForm.labelTextBlockchainHeight.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_BLOCKCHAIN_HEIGHT_TEXT") + " In Sync";
                OverviewWalletForm.labelTextTotalBlockMined.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_MINED_TEXT") + " In Sync";
                OverviewWalletForm.labelTextTotalBlockLeft.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_LEFT_TEXT") + " In Sync";
                OverviewWalletForm.labelTextNetworkDifficulty.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_DIFFICULTY_TEXT") + " In Sync";
                OverviewWalletForm.labelTextNetworkHashrate.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_NETWORK_HASHRATE_TEXT") + " In Sync";
                OverviewWalletForm.labelTextLastBlockFound.Text = ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_LAST_BLOCK_FOUND_TEXT") + " In Sync";
                labelSyncInformation.Text = "Sync & Wallet disconnected.";
                labelNoticeTotalPendingTransactionOnReceive.Text = ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_TOTAL_PENDING_TRANSACTION_ON_RECEIVE_TEXT");
            };
            BeginInvoke(invoke);
        }

        /// <summary>
        /// Start update transaction history.
        /// </summary>
        public void StartUpdateTransactionHistory()
        {
            if (ClassWalletObject.SeedNodeConnectorWallet == null)
                return;
            try
            {
                if (!EnableUpdateTransactionWallet)
                {
                    EnableUpdateTransactionWallet = true;


                    // Show transaction decrypted from cache.
                    ThreadPool.QueueUserWorkItem(delegate
                    {

                        while (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            try
                            {
                                UpdateCurrentPageNumberTransactionHistory();



                                if (TransactionHistoryWalletForm.IsShowed)
                                {
                                    if (ListTransactionHashShowed.Count ==
                                        ClassWalletTransactionCache.ListTransaction.Count &&
                                        ClassWalletTransactionCache.ListTransaction.Count ==
                                        ClassWalletObject.TotalTransactionInSync &&
                                        ClassWalletTransactionAnonymityCache.ListTransaction.Count ==
                                        ClassWalletObject.TotalTransactionInSyncAnonymity)
                                    {
                                        if (ListTransactionAnonymityHashShowed.Count ==
                                            ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                        {
                                            if (ClassWalletTransactionAnonymityCache.ListTransaction.Count ==
                                                ClassWalletObject.TotalTransactionInSyncAnonymity)
                                            {
                                                if (ClassWalletTransactionCache.ListTransaction.Count ==
                                                    ClassWalletObject.TotalTransactionInSync)
                                                {
                                                    TransactionHistoryWalletForm.HideWaitingSyncTransactionPanel();
                                                }
                                            }
                                        }

                                        #region show transaction received/sent by the normal unique wallet id of the wallet
                                        if (TotalTransactionRead != ListTransactionHashShowed.Count)
                                        {

                                            for (int i = ListTransactionHashShowed.Count - 1; i >= TotalTransactionRead;
                                                i--)
                                            {
                                                string[] splitTransaction = ListTransactionHashShowed[i]
                                                    .Split(new[] { "#" }, StringSplitOptions.None);
                                                string type = splitTransaction[0];
                                                string hash = splitTransaction[1];
                                                string wallet = splitTransaction[2];
                                                Decimal amount = Decimal.Parse(
                                                    splitTransaction[3].ToString(Program.GlobalCultureInfo),
                                                    NumberStyles.Currency, Program.GlobalCultureInfo);
                                                Decimal fee = Decimal.Parse(
                                                    splitTransaction[4].ToString(Program.GlobalCultureInfo),
                                                    NumberStyles.Currency, Program.GlobalCultureInfo);
                                                string timestamp = splitTransaction[5];
                                                if (ClassConnectorSetting.MAJOR_UPDATE_1)
                                                {
                                                    string timestampRecv = splitTransaction[6];
                                                    string blockchainHeight = splitTransaction[7];

                                                    blockchainHeight = blockchainHeight.Replace("{", "");
                                                    blockchainHeight = blockchainHeight.Replace("}", "");
                                                    if (blockchainHeight.Contains(";"))
                                                    {
                                                        var splitBlockchainHeight =
                                                            blockchainHeight.Split(new[] { ";" },
                                                                StringSplitOptions.None);
                                                        blockchainHeight = string.Empty;
                                                        foreach (var height in splitBlockchainHeight)
                                                        {
                                                            var blockId =
                                                                height.Split(new[] { "~" }, StringSplitOptions.None)[0];

                                                            blockchainHeight += blockId + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        blockchainHeight = blockchainHeight.Split(new[] { "~" },
                                                            StringSplitOptions.None)[0];
                                                    }


                                                    DateTime dateTimeSend = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                    dateTimeSend = dateTimeSend.AddSeconds(long.Parse(timestamp));
                                                    dateTimeSend = dateTimeSend.ToLocalTime();

                                                    try
                                                    {
                                                        var dateTimeRecv = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                        dateTimeRecv =
                                                            dateTimeRecv.AddSeconds(long.Parse(timestampRecv));
                                                        dateTimeRecv = dateTimeRecv.ToLocalTime();

                                                        #region show anonymous transaction received
                                                        if (wallet == "ANONYMOUS")
                                                        {

                                                            int minShow = (CurrentTransactionHistoryPageAnonymousReceived - 1) * MaxTransactionPerPage;
                                                            int maxShow = CurrentTransactionHistoryPageAnonymousReceived * MaxTransactionPerPage;

                                                            if (!_normalTransactionLoaded)
                                                            {
                                                                if (TotalTransactionAnonymousReceived >= minShow && TotalTransactionAnonymousReceived < maxShow && !_normalTransactionLoaded)
                                                                {
                                                                    string[] row =
                                                                    {
                                                                    (TotalTransactionAnonymousReceived+1).ToString(),
                                                                    dateTimeSend.ToString(),
                                                                    type, hash, "" + amount, "" + fee, wallet,
                                                                    dateTimeRecv.ToString(), blockchainHeight
                                                                };
                                                                    var listViewItem = new ListViewItem(row);


                                                                    MethodInvoker invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory.Items
                                                                            .Add(listViewItem);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewAnonymityReceivedTransactionHistory
                                                                        .BeginInvoke(invoke);
                                                                    DateTimeOffset transactionOffset =
                                                                        new DateTimeOffset(dateTimeRecv);
                                                                    if (transactionOffset.ToUnixTimeSeconds() >
                                                                        DateTimeOffset.Now.ToUnixTimeSeconds())
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewAnonymityReceivedTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(255, 153, 102);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }
                                                                    else
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewAnonymityReceivedTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(0, 255, 153);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }

                                                                }
                                                            }

                                                            if (_normalTransactionLoaded && CurrentTransactionHistoryPageAnonymousReceived == 1)
                                                            {
                                                                string[] row =
                                                                {
                                                                    (TotalTransactionAnonymousReceived+1).ToString(),
                                                                    dateTimeSend.ToString(),
                                                                    type, hash, "" + amount, "" + fee, wallet,
                                                                    dateTimeRecv.ToString(), blockchainHeight
                                                                };
                                                                var listViewItem = new ListViewItem(row);


                                                                MethodInvoker invoke = () =>
                                                                    TransactionHistoryWalletForm
                                                                        .listViewAnonymityReceivedTransactionHistory.Items
                                                                        .Insert(0, listViewItem);
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymityReceivedTransactionHistory
                                                                    .BeginInvoke(invoke);

                                                            }

                                                            if (TotalTransactionAnonymousReceived == minShow)
                                                            {
                                                                void Invoker() =>
                                                                    TransactionHistoryWalletForm.AutoResizeColumns(
                                                                        TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory);

                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymityReceivedTransactionHistory
                                                                    .BeginInvoke((MethodInvoker)Invoker);

                                                            }

                                                            TotalTransactionAnonymousReceived++;
                                                        }
                                                        #endregion
                                                        #region Show block reward transaction.
                                                        else if (wallet.Contains("BLOCKCHAIN["))
                                                        {
                                                            int minShow = (CurrentTransactionHistoryPageBlockReward - 1) * MaxTransactionPerPage;
                                                            int maxShow = CurrentTransactionHistoryPageBlockReward * MaxTransactionPerPage;

                                                            if (TotalTransactionBlockReward >= minShow && TotalTransactionBlockReward < maxShow && !_normalTransactionLoaded)
                                                            {
                                                                string[] row =
                                                                {
                                                                    (TotalTransactionBlockReward+1).ToString(), dateTimeSend.ToString(),
                                                                    type,
                                                                    hash, "" + amount, "" + fee, wallet,
                                                                    dateTimeRecv.ToString(), blockchainHeight
                                                                };
                                                                var listViewItem = new ListViewItem(row);



                                                                MethodInvoker invoke = () =>
                                                                    TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory.Items
                                                                        .Add(listViewItem);
                                                                TransactionHistoryWalletForm
                                                                    .listViewBlockRewardTransactionHistory
                                                                    .BeginInvoke(invoke);
                                                                DateTimeOffset transactionOffset =
                                                                    new DateTimeOffset(dateTimeRecv);
                                                                if (transactionOffset.ToUnixTimeSeconds() >
                                                                    DateTimeOffset.Now.ToUnixTimeSeconds())
                                                                {
                                                                    invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                                .listViewBlockRewardTransactionHistory
                                                                                .Items[
                                                                                    TransactionHistoryWalletForm
                                                                                        .listViewBlockRewardTransactionHistory
                                                                                        .Items
                                                                                        .Count - 1].BackColor =
                                                                            Color.FromArgb(255, 153, 102);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory
                                                                        .BeginInvoke(invoke);
                                                                }
                                                                else
                                                                {
                                                                    invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                                .listViewBlockRewardTransactionHistory
                                                                                .Items[
                                                                                    TransactionHistoryWalletForm
                                                                                        .listViewBlockRewardTransactionHistory
                                                                                        .Items
                                                                                        .Count - 1].BackColor =
                                                                            Color.FromArgb(0, 255, 153);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory
                                                                        .BeginInvoke(invoke);
                                                                }

                                                                
                                                            }
                                                            if (_normalTransactionLoaded && CurrentTransactionHistoryPageBlockReward == 1)
                                                            {
                                                                string[] row =
                                                                {
                                                                    (TotalTransactionBlockReward+1).ToString(), dateTimeSend.ToString(),
                                                                    type,
                                                                    hash, "" + amount, "" + fee, wallet,
                                                                    dateTimeRecv.ToString(), blockchainHeight
                                                                };
                                                                var listViewItem = new ListViewItem(row);



                                                                MethodInvoker invoke = () =>
                                                                    TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory.Items
                                                                        .Insert(0, listViewItem);
                                                                TransactionHistoryWalletForm
                                                                    .listViewBlockRewardTransactionHistory
                                                                    .BeginInvoke(invoke);

                                                                
                                                            }

                                                            if (TotalTransactionBlockReward == minShow)
                                                            {
                                                                void Invoker() =>
                                                                    TransactionHistoryWalletForm.AutoResizeColumns(
                                                                        TransactionHistoryWalletForm
                                                                            .listViewBlockRewardTransactionHistory);

                                                                TransactionHistoryWalletForm
                                                                    .listViewBlockRewardTransactionHistory
                                                                    .BeginInvoke((MethodInvoker)Invoker);
                                                            }

                                                            TotalTransactionBlockReward++;
                                                        }
                                                        #endregion
                                                        else
                                                        {
                                                            #region show normal transaction sent
                                                            if (type == "SEND")
                                                            {
                                                                int minShow = (CurrentTransactionHistoryPageNormalSend - 1) * MaxTransactionPerPage;
                                                                int maxShow = CurrentTransactionHistoryPageNormalSend * MaxTransactionPerPage;

                                                                if (TotalTransactionNormalSend >= minShow && TotalTransactionNormalSend < maxShow && !_normalTransactionLoaded)
                                                                {
                                                                    DateTimeOffset transactionOffset =
                                                                        new DateTimeOffset(dateTimeRecv);


                                                                    string[] row =
                                                                    {
                                                                        (TotalTransactionNormalSend+1).ToString(), dateTimeSend.ToString(),
                                                                        type, hash, "" + amount, "" + fee, wallet,
                                                                        dateTimeRecv.ToString(), blockchainHeight
                                                                    };
                                                                    var listViewItem = new ListViewItem(row);


                                                                    MethodInvoker invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalSendTransactionHistory.Items
                                                                            .Add(listViewItem);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalSendTransactionHistory
                                                                        .BeginInvoke(invoke);
                                                                    ;
                                                                    if (transactionOffset.ToUnixTimeSeconds() >
                                                                        DateTimeOffset.Now.ToUnixTimeSeconds())
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewNormalSendTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewNormalSendTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(255, 153, 102);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalSendTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }
                                                                    else
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewNormalSendTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewNormalSendTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(0, 255, 153);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalSendTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }

                                                                    
                                                                }
                                                                if (_normalTransactionLoaded && CurrentTransactionHistoryPageNormalSend == 1)
                                                                {
                                                                    string[] row =
                                                                    {
                                                                        (TotalTransactionNormalSend+1).ToString(), dateTimeSend.ToString(),
                                                                        type, hash, "" + amount, "" + fee, wallet,
                                                                        dateTimeRecv.ToString(), blockchainHeight
                                                                    };
                                                                    var listViewItem = new ListViewItem(row);


                                                                    MethodInvoker invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalSendTransactionHistory.Items
                                                                            .Insert(0, listViewItem);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalSendTransactionHistory
                                                                        .BeginInvoke(invoke);

                                                                    
                                                                }


                                                                if (TotalTransactionNormalSend == minShow)
                                                                {
                                                                    void Invoker() =>
                                                                        TransactionHistoryWalletForm.AutoResizeColumns(
                                                                            TransactionHistoryWalletForm
                                                                                .listViewNormalSendTransactionHistory);

                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalSendTransactionHistory
                                                                        .BeginInvoke((MethodInvoker)Invoker);
                                                                }


                                                                TotalTransactionNormalSend++;
                                                            }
                                                            #endregion
                                                            #region Show normal transaction received.
                                                            else if (type == "RECV")
                                                            {
                                                                int minShow = (CurrentTransactionHistoryPageNormalReceive - 1) * MaxTransactionPerPage;
                                                                int maxShow = CurrentTransactionHistoryPageNormalReceive * MaxTransactionPerPage;

                                                                if (TotalTransactionNormalReceived >= minShow && TotalTransactionNormalReceived < maxShow && !_normalTransactionLoaded)
                                                                {

                                                                    string[] row =
                                                                    {
                                                                        (TotalTransactionNormalReceived+1).ToString(),
                                                                        dateTimeSend.ToString(),
                                                                        type, hash, "" + amount, "" + fee, wallet,
                                                                        dateTimeRecv.ToString(), blockchainHeight
                                                                    };
                                                                    var listViewItem = new ListViewItem(row);



                                                                    MethodInvoker invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory.Items
                                                                            .Add(listViewItem);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalReceivedTransactionHistory
                                                                        .BeginInvoke(invoke);
                                                                    DateTimeOffset transactionOffset =
                                                                        new DateTimeOffset(dateTimeRecv);
                                                                    if (transactionOffset.ToUnixTimeSeconds() >
                                                                        DateTimeOffset.Now.ToUnixTimeSeconds())
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewNormalReceivedTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewNormalReceivedTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(255, 153, 102);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }
                                                                    else
                                                                    {
                                                                        invoke = () =>
                                                                            TransactionHistoryWalletForm
                                                                                    .listViewNormalReceivedTransactionHistory
                                                                                    .Items[
                                                                                        TransactionHistoryWalletForm
                                                                                            .listViewNormalReceivedTransactionHistory
                                                                                            .Items.Count - 1].BackColor =
                                                                                Color.FromArgb(0, 255, 153);
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory
                                                                            .BeginInvoke(invoke);
                                                                    }

                                                                    
                                                                }

                                                                if (_normalTransactionLoaded && CurrentTransactionHistoryPageNormalReceive == 1)
                                                                {
                                                                    string[] row =
                                                                    {
                                                                        (TotalTransactionNormalReceived+1).ToString(),
                                                                        dateTimeSend.ToString(),
                                                                        type, hash, "" + amount, "" + fee, wallet,
                                                                        dateTimeRecv.ToString(), blockchainHeight
                                                                    };
                                                                    var listViewItem = new ListViewItem(row);

                                                                    MethodInvoker invoke = () =>
                                                                        TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory.Items
                                                                            .Insert(0, listViewItem);
                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalReceivedTransactionHistory
                                                                        .BeginInvoke(invoke);

                                                                    
                                                                }
                                                                if (TotalTransactionNormalReceived == minShow)
                                                                {
                                                                    void Invoker() =>
                                                                        TransactionHistoryWalletForm.AutoResizeColumns(
                                                                            TransactionHistoryWalletForm
                                                                                .listViewNormalReceivedTransactionHistory);

                                                                    TransactionHistoryWalletForm
                                                                        .listViewNormalReceivedTransactionHistory
                                                                        .BeginInvoke((MethodInvoker)Invoker);
                                                                }

                                                                TotalTransactionNormalReceived++;
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        Log.WriteLine(
                                                            "Error on transactions show date: " + error.Message +
                                                            " for timestamp recv: " + timestamp + " timestamp send: " +
                                                            timestampRecv);
                                                    }
                                                }
                                            }

                                            TotalTransactionRead = ListTransactionHashShowed.Count;
                                            _normalTransactionLoaded = true;

                                        }
                                        #endregion

                                        #region Update transaction normal send color

                                        for (int i = 0;
                                        i < TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items
                                            .Count;
                                        i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {
                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewNormalSendTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewNormalSendTransactionHistory.Items[i1] != null)
                                                        {

                                                            if (CurrentTransactionHistoryPageNormalSend == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalSendTransactionHistory.Items[i1].SubItems[0].Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalSendTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;

                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {


                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalSendTransactionHistory.Items[i1]
                                                                    .BackColor = Color.FromArgb(255, 153, 102);


                                                            }
                                                            else
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalSendTransactionHistory.Items[i1]
                                                                    .BackColor = Color.FromArgb(0, 255, 153);
                                                            }
                                                            
                                                            

                                                        }

                                                    }
                                                }
                                                catch
                                                {

                                                }

                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion

                                        #region Update transaction anonymity received color

                                        for (int i = 0;
                                            i < TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory
                                                .Items.Count;
                                            i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {
                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewAnonymityReceivedTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewAnonymityReceivedTransactionHistory.Items[i1] !=
                                                            null)
                                                        {
                                                            if (CurrentTransactionHistoryPageAnonymousReceived == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymityReceivedTransactionHistory.Items[i1]
                                                                    .SubItems[0].Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymityReceivedTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewAnonymityReceivedTransactionHistory
                                                                        .Items[i1].BackColor !=
                                                                    Color.FromArgb(255, 153, 102))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewAnonymityReceivedTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(255, 153, 102);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                            .Items.Count;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewAnonymityReceivedTransactionHistory
                                                                        .Items[i1].BackColor != Color.FromArgb(0, 255, 153))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewAnonymityReceivedTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(0, 255, 153);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewAnonymityReceivedTransactionHistory
                                                                            .Items.Count;
                                                                    }
                                                                }
                                                            }
                                                            
                                                            
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion

                                        #region Update transaction block reward color

                                        for (int i = 0;
                                            i < TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items
                                                .Count;
                                            i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {

                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewBlockRewardTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewBlockRewardTransactionHistory.Items[i1] != null)
                                                        {
                                                            if (CurrentTransactionHistoryPageBlockReward == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewBlockRewardTransactionHistory.Items[i1]
                                                                    .SubItems[0].Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewBlockRewardTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory.Items[i1]
                                                                        .BackColor != Color.FromArgb(255, 153, 102))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                            .listViewBlockRewardTransactionHistory.Items[i1]
                                                                            .BackColor = Color.FromArgb(255, 153, 102);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewBlockRewardTransactionHistory.Items
                                                                            .Count;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewBlockRewardTransactionHistory.Items[i1]
                                                                        .BackColor != Color.FromArgb(0, 255, 153))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                            .listViewBlockRewardTransactionHistory.Items[i1]
                                                                            .BackColor = Color.FromArgb(0, 255, 153);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewBlockRewardTransactionHistory.Items
                                                                            .Count;
                                                                    }
                                                                }
                                                            }
                                                            
                                                            
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion

                                        #region Update normal transaction received color.

                                        for (int i = 0;
                                            i < TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items
                                                .Count;
                                            i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {

                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewNormalReceivedTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewNormalReceivedTransactionHistory.Items[i1] != null)
                                                        {
                                                            if (CurrentTransactionHistoryPageNormalReceive == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalReceivedTransactionHistory.Items[i1]
                                                                    .SubItems[0]
                                                                    .Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewNormalReceivedTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewNormalReceivedTransactionHistory.Items[i1]
                                                                        .BackColor != Color.FromArgb(255, 153, 102))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewNormalReceivedTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(255, 153, 102);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory.Items
                                                                            .Count;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewNormalReceivedTransactionHistory.Items[i1]
                                                                        .BackColor != Color.FromArgb(0, 255, 153))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewNormalReceivedTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(0, 255, 153);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewNormalReceivedTransactionHistory.Items
                                                                            .Count;
                                                                    }
                                                                }
                                                            }
                                                            
                                                            
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion


                                        #region Update transaction anonymity sent color

                                        for (int i = 0;
                                            i < TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory
                                                .Items.Count;
                                            i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {
                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewAnonymitySendTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewAnonymitySendTransactionHistory.Items[i1] !=
                                                            null)
                                                        {
                                                            if (CurrentTransactionHistoryPageAnonymousSend == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                    .SubItems[0].Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewAnonymitySendTransactionHistory
                                                                        .Items[i1].BackColor !=
                                                                    Color.FromArgb(255, 153, 102))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewAnonymitySendTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(255, 153, 102);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory
                                                                            .Items.Count;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewAnonymitySendTransactionHistory
                                                                        .Items[i1].BackColor != Color.FromArgb(0, 255, 153))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                                .listViewAnonymitySendTransactionHistory
                                                                                .Items[i1].BackColor =
                                                                            Color.FromArgb(0, 255, 153);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory
                                                                            .Items.Count;
                                                                    }
                                                                }
                                                            }
                                                            
                                                            
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        TransactionHistoryWalletForm.ShowWaitingSyncTransactionPanel();
                                    }


                                    if (ListTransactionAnonymityHashShowed.Count ==
                                        ClassWalletTransactionAnonymityCache.ListTransaction.Count &&
                                        ClassWalletTransactionAnonymityCache.ListTransaction.Count ==
                                        ClassWalletObject.TotalTransactionInSyncAnonymity &&
                                        ClassWalletTransactionCache.ListTransaction.Count ==
                                        ClassWalletObject.TotalTransactionInSync)
                                    {
                                        if (ListTransactionHashShowed.Count ==
                                            ClassWalletTransactionCache.ListTransaction.Count)
                                        {
                                            if (ClassWalletTransactionAnonymityCache.ListTransaction.Count ==
                                                ClassWalletObject.TotalTransactionInSyncAnonymity)
                                            {
                                                if (ClassWalletTransactionCache.ListTransaction.Count ==
                                                    ClassWalletObject.TotalTransactionInSync)
                                                {
                                                    TransactionHistoryWalletForm.HideWaitingSyncTransactionPanel();
                                                }
                                            }
                                        }


                                        #region Show transaction anonymity sent with the unique wallet anonymity id of the wallet
                                        if (TotalAnonymityTransactionRead != ListTransactionAnonymityHashShowed.Count)
                                        {


                                            for (int i = ListTransactionAnonymityHashShowed.Count - 1;
                                                i >= TotalAnonymityTransactionRead;
                                                i--)
                                            {
                                                string[] splitTransaction = ListTransactionAnonymityHashShowed[i]
                                                    .Split(new[] { "#" }, StringSplitOptions.None);
                                                string type = splitTransaction[0];
                                                string hash = splitTransaction[1];
                                                string wallet = splitTransaction[2];
                                                Decimal amount = Decimal.Parse(
                                                    splitTransaction[3].ToString(Program.GlobalCultureInfo),
                                                    NumberStyles.Currency, Program.GlobalCultureInfo);
                                                Decimal fee = Decimal.Parse(
                                                    splitTransaction[4].ToString(Program.GlobalCultureInfo),
                                                    NumberStyles.Currency, Program.GlobalCultureInfo);
                                                string timestamp = splitTransaction[5];
                                                if (ClassConnectorSetting.MAJOR_UPDATE_1)
                                                {
                                                    string timestampRecv = splitTransaction[6];
                                                    string blockchainHeight = splitTransaction[7];

                                                    blockchainHeight = blockchainHeight.Replace("{", "");
                                                    blockchainHeight = blockchainHeight.Replace("}", "");
                                                    if (blockchainHeight.Contains(";"))
                                                    {
                                                        var splitBlockchainHeight =
                                                            blockchainHeight.Split(new[] { ";" },
                                                                StringSplitOptions.None);
                                                        blockchainHeight = string.Empty;
                                                        foreach (var height in splitBlockchainHeight)
                                                        {
                                                            var blockId =
                                                                height.Split(new[] { "~" }, StringSplitOptions.None)[0];

                                                            blockchainHeight += blockId + " ";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        blockchainHeight = blockchainHeight.Split(new[] { "~" },
                                                            StringSplitOptions.None)[0];
                                                    }


                                                    DateTime dateTimeSend = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                    dateTimeSend = dateTimeSend.AddSeconds(long.Parse(timestamp));
                                                    dateTimeSend = dateTimeSend.ToLocalTime();

                                                    DateTime dateTimeRecv;
                                                    try
                                                    {
                                                        dateTimeRecv = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                        dateTimeRecv =
                                                            dateTimeRecv.AddSeconds(long.Parse(timestampRecv));
                                                        dateTimeRecv = dateTimeRecv.ToLocalTime();

                                                        int minShow = (CurrentTransactionHistoryPageAnonymousSend - 1) * MaxTransactionPerPage;
                                                        int maxShow = CurrentTransactionHistoryPageAnonymousSend * MaxTransactionPerPage;

                                                        if (TotalTransactionAnonymousSend >= minShow && TotalTransactionAnonymousSend < maxShow && !_anonymousTransactionLoaded)
                                                        {
                                                            string[] row =
                                                            {
                                                                (TotalTransactionAnonymousSend+1).ToString(), dateTimeSend.ToString(),
                                                                type, hash, "" + amount, "" + fee, wallet,
                                                                dateTimeRecv.ToString(), blockchainHeight
                                                            };
                                                            var listViewItem = new ListViewItem(row);


                                                            MethodInvoker invoke = () =>
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items
                                                                    .Add(listViewItem);
                                                            TransactionHistoryWalletForm
                                                                .listViewAnonymitySendTransactionHistory
                                                                .BeginInvoke(invoke);
                                                            DateTimeOffset transactionOffset =
                                                                new DateTimeOffset(dateTimeRecv);
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                invoke = () =>
                                                                    TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory
                                                                            .Items[
                                                                                TransactionHistoryWalletForm
                                                                                    .listViewAnonymitySendTransactionHistory
                                                                                    .Items
                                                                                    .Count - 1].BackColor =
                                                                        Color.FromArgb(255, 153, 102);
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory
                                                                    .BeginInvoke(invoke);
                                                            }
                                                            else
                                                            {
                                                                invoke = () =>
                                                                    TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory
                                                                            .Items[
                                                                                TransactionHistoryWalletForm
                                                                                    .listViewAnonymitySendTransactionHistory
                                                                                    .Items
                                                                                    .Count - 1].BackColor =
                                                                        Color.FromArgb(0, 255, 153);
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory
                                                                    .BeginInvoke(invoke);
                                                            }
                                                            

                                                        }
                                                        if (_anonymousTransactionLoaded && CurrentTransactionHistoryPageAnonymousSend == 1)
                                                        {
                                                            string[] row =
                                                            {
                                                                (TotalTransactionAnonymousSend+1).ToString(), dateTimeSend.ToString(),
                                                                type, hash, "" + amount, "" + fee, wallet,
                                                                dateTimeRecv.ToString(), blockchainHeight
                                                            };
                                                            var listViewItem = new ListViewItem(row);


                                                            MethodInvoker invoke = () =>
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items
                                                                    .Insert(0, listViewItem);
                                                            TransactionHistoryWalletForm
                                                                .listViewAnonymitySendTransactionHistory
                                                                .BeginInvoke(invoke);
                                                            

                                                        }
                                                        if (TotalTransactionAnonymousSend == minShow)
                                                        {
                                                            void Invoker() =>
                                                                TransactionHistoryWalletForm.AutoResizeColumns(
                                                                    TransactionHistoryWalletForm
                                                                        .listViewAnonymitySendTransactionHistory);

                                                            TransactionHistoryWalletForm
                                                                .listViewAnonymitySendTransactionHistory
                                                                .BeginInvoke((MethodInvoker)Invoker);
                                                        }

                                                        TotalTransactionAnonymousSend++;
                                                    }
                                                    catch (Exception error)
                                                    {
                                                        Log.WriteLine(
                                                            "Error on transactions show date: " + error.Message +
                                                            " for timestamp recv: " + timestamp + " timestamp send: " +
                                                            timestampRecv);
                                                    }
                                                }
                                            }

                                            TotalAnonymityTransactionRead = ListTransactionAnonymityHashShowed.Count;
                                            _anonymousTransactionLoaded = true;

                                        }

                                        #region Update transaction anonymity send color.

                                        for (int i = 0;
                                        i < TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items
                                            .Count;
                                        i++)
                                        {
                                            var i1 = i;

                                            void MethodInvoker()
                                            {

                                                try
                                                {
                                                    if (i1 < TransactionHistoryWalletForm
                                                            .listViewAnonymitySendTransactionHistory.Items.Count)
                                                    {
                                                        if (TransactionHistoryWalletForm
                                                                .listViewAnonymitySendTransactionHistory.Items[i1] != null)
                                                        {
                                                            if (CurrentTransactionHistoryPageAnonymousSend == 1)
                                                            {
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                    .SubItems[0]
                                                                    .Text = "" + (i1 + 1);
                                                            }
                                                            var transactionWalletDateRecv = DateTime.Parse(
                                                                TransactionHistoryWalletForm
                                                                    .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                    .SubItems[7]
                                                                    .Text.ToString());
                                                            DateTimeOffset transactionOffset = transactionWalletDateRecv;
                                                            if (transactionOffset.ToUnixTimeSeconds() >
                                                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                                            {
                                                                if (TransactionHistoryWalletForm
                                                                        .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                        .BackColor != Color.FromArgb(255, 153, 102))
                                                                {
                                                                    try
                                                                    {
                                                                        TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory
                                                                            .Items[i1]
                                                                            .BackColor = Color.FromArgb(255, 153, 102);
                                                                    }
                                                                    catch
                                                                    {
                                                                        i = TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory.Items
                                                                            .Count;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (TransactionHistoryWalletForm
                                                                            .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                            .BackColor != Color.FromArgb(0, 255, 153))
                                                                    {
                                                                        try
                                                                        {
                                                                            TransactionHistoryWalletForm
                                                                                .listViewAnonymitySendTransactionHistory.Items[i1]
                                                                                .BackColor = Color.FromArgb(0, 255, 153);
                                                                        }
                                                                        catch
                                                                        {
                                                                            i = TransactionHistoryWalletForm
                                                                                .listViewAnonymitySendTransactionHistory.Items
                                                                                .Count;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    StopUpdateTransactionHistory(false, false);
                                                }
                                            }

                                            BeginInvoke((MethodInvoker)MethodInvoker);
                                        }

                                        #endregion

                                        #endregion
                                    }
                                    else
                                    {
                                        TransactionHistoryWalletForm.ShowWaitingSyncTransactionPanel();
                                    }

                                    if (!ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                                    {
                                        break;
                                    }

                                    if (TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items.Count +
                                        TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Items
                                            .Count +
                                        TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items.Count +
                                        TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items.Count >
                                        ClassWalletTransactionCache.ListTransaction.Count)
                                    {
                                        _anonymousTransactionLoaded = false;
                                        _normalTransactionLoaded = false;
                                        StopUpdateTransactionHistory(false, false);
                                    }

                                    if (TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items.Count >
                                        ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                    {
                                        _anonymousTransactionLoaded = false;
                                        _normalTransactionLoaded = false;
                                        StopUpdateTransactionHistory(false, false);
                                    }

                                }


                                Thread.Sleep(ThreadUpdateTransactionWalletInterval);
                            }
                            catch
                            {

                            }
                        }

                    });

                    // Decrypt transaction from cache.
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        while (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        {
                            try
                            {
                                if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
                                {
                                    if (ClassWalletTransactionCache.ListTransaction.Count +
                                        ClassWalletTransactionAnonymityCache.ListTransaction.Count <
                                        ClassWalletObject.TotalTransactionInSync +
                                        ClassWalletObject.TotalTransactionInSyncAnonymity)
                                    {
                                        UpdateLabelSyncInformation(
                                            "Total transactions downloaded: " +
                                            (ClassWalletTransactionCache.ListTransaction.Count +
                                             ClassWalletTransactionAnonymityCache.ListTransaction.Count) + "/" +
                                            (ClassWalletObject.TotalTransactionInSync +
                                             ClassWalletObject.TotalTransactionInSyncAnonymity));
                                    }
                                }
                                else if (ClassFormPhase.FormPhase != ClassFormPhaseEnumeration.TransactionHistory && ClassFormPhase.FormPhase != ClassFormPhaseEnumeration.BlockExplorer)
                                {
                                    string listOfNodes = string.Empty;
                                    if (ClassWalletObject.ListWalletConnectToRemoteNode != null)
                                    {
                                        var tmpListNode = new List<string>();
                                        for (int i = 0; i < ClassWalletObject.ListWalletConnectToRemoteNode.Count; i++)
                                        {
                                            if (i < ClassWalletObject.ListWalletConnectToRemoteNode.Count)
                                            {
                                                if (ClassWalletObject.ListWalletConnectToRemoteNode[i] != null)
                                                {
                                                    if (!tmpListNode.Contains(ClassWalletObject
                                                        .ListWalletConnectToRemoteNode[i].RemoteNodeHost))
                                                    {
                                                        tmpListNode.Add(ClassWalletObject
                                                            .ListWalletConnectToRemoteNode[i].RemoteNodeHost);
                                                        listOfNodes +=
                                                            ClassWalletObject.ListWalletConnectToRemoteNode[i]
                                                                .RemoteNodeHost + "|";
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(listOfNodes))
                                    {
                                        listOfNodes = listOfNodes.Substring(0, listOfNodes.Length - 1);
                                        if (ClassWalletObject.InSyncTransaction ||
                                            ClassWalletObject.InSyncTransactionAnonymity)
                                        {
                                            try
                                            {
                                                UpdateLabelSyncInformation(
                                                    "Your wallet currently sync transactions with node: " +
                                                    ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost +
                                                    " " + (ClassWalletTransactionCache.ListTransaction.Count +
                                                           ClassWalletTransactionAnonymityCache.ListTransaction.Count) +
                                                    "/" + (ClassWalletObject.TotalTransactionInSync +
                                                           ClassWalletObject.TotalTransactionInSyncAnonymity) + ".");
                                            }
                                            catch (Exception error)
                                            {
                                                Log.WriteLine("Error on seed node list: " + error.Message);
                                            }
                                        }
                                        else
                                        {
                                            if (!ClassWalletObject.InSyncBlock && !ClassWalletObject.InSyncTransaction &&
                                                !ClassWalletObject.InSyncTransactionAnonymity)
                                            {
                                                UpdateLabelSyncInformation(
                                                    "Your wallet sync with node(s): " + listOfNodes + " " +
                                                    ClassWalletObject.ListWalletConnectToRemoteNode.Count +
                                                    " connections are open.");
                                            }
                                        }
                                    }
                                }


                                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                {
                                    copyListTransaction.Clear();
                                    copyListAnonymousTransaction.Clear();
                                    copyListTransaction = new List<string>(ClassWalletTransactionCache.ListTransaction);
                                    copyListAnonymousTransaction = new List<string>(ClassWalletTransactionAnonymityCache.ListTransaction);

                                    if (ListTransactionHashShowed.Count < copyListTransaction.Count)
                                    {
                                        long loadingDate = DateTimeOffset.Now.ToUnixTimeSeconds();

                                        MethodInvoker invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = false;
                                        BeginInvoke(invokeLockButton);
                                        for (int i = ListTransactionHashShowed.Count;
                                        i < copyListTransaction.Count;
                                        i++)
                                        {

                                            if (!ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                                            {
                                                ListTransactionHashShowed.Clear();
                                                ListTransactionAnonymityHashShowed.Clear();
                                                break;
                                            }
                                            if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                            {
                                                if (i < copyListTransaction.Count)
                                                {
                                                    string decryptedTransaction = ClassAlgo
                                                        .GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                                                            copyListTransaction[i],
                                                            ClassWalletObject.WalletConnect.WalletAddress +
                                                            ClassWalletObject.WalletConnect.WalletKey,
                                                            ClassWalletNetworkSetting.KeySize); // AES

                                                    if (decryptedTransaction == ClassAlgoErrorEnumeration.AlgoError)
                                                    {
                                                        TransactionHistoryWalletForm.ResyncTransaction();
                                                        break;
                                                    }
                                                    else
                                                    {

                                                        if (!ListTransactionHashShowed.ContainsValue(decryptedTransaction))
                                                        {
                                                            loadingDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                            ListTransactionHashShowed.Add(ListTransactionHashShowed.Count,
                                                                decryptedTransaction);
                                                        }
                                                    }
                                                }
                                                if (loadingDate + 10 < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                                {
                                                    ListTransactionHashShowed.Clear();
                                                    ListTransactionAnonymityHashShowed.Clear();
                                                    new Thread(() => StopUpdateTransactionHistory(true, false)).Start();
                                                    break;
                                                }
                                                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                                {
                                                    UpdateLabelSyncInformation(
                                                    "Total transactions loaded and decrypted: " +
                                                    (ListTransactionHashShowed.Count +
                                                     ListTransactionAnonymityHashShowed.Count) + "/" +
                                                    (copyListTransaction.Count +
                                                     copyListAnonymousTransaction.Count));
                                                }
                                            }
                                            else
                                            {
                                                UpdateLabelSyncInformation("Total transactions downloaded: " + (ClassWalletTransactionCache.ListTransaction.Count +
                                                                             ClassWalletTransactionAnonymityCache.ListTransaction.Count) + "/" +
                                                                             (ClassWalletObject.TotalTransactionInSync + ClassWalletObject.TotalTransactionInSyncAnonymity));
                                            }
                                        }
                                        invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = true;
                                        BeginInvoke(invokeLockButton);
                                    }
                                    else
                                    {
                                        MethodInvoker invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = true;
                                        BeginInvoke(invokeLockButton);
                                    }

                                    if (ListTransactionAnonymityHashShowed.Count < copyListAnonymousTransaction.Count)
                                    {
                                        long loadingDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                        MethodInvoker invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = false;
                                        BeginInvoke(invokeLockButton);
                                        for (int i = ListTransactionAnonymityHashShowed.Count;
                                            i < copyListAnonymousTransaction.Count;
                                            i++)
                                        {
   
                                            if (!ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                                            {
                                                ListTransactionHashShowed.Clear();
                                                ListTransactionAnonymityHashShowed.Clear();
                                                break;
                                            }
                                            if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                            {
                                                if (i < copyListAnonymousTransaction.Count)
                                                {
                                                    string decryptedTransaction = ClassAlgo
                                                        .GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                                                           copyListAnonymousTransaction[i],
                                                            ClassWalletObject.WalletConnect.WalletAddress +
                                                            ClassWalletObject.WalletConnect.WalletKey,
                                                            ClassWalletNetworkSetting.KeySize); // AES

                                                    if (decryptedTransaction == ClassAlgoErrorEnumeration.AlgoError)
                                                    {
                                                        TransactionHistoryWalletForm.ResyncTransaction();
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (!ListTransactionAnonymityHashShowed.ContainsValue(decryptedTransaction))
                                                        {
                                                            loadingDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                            ListTransactionAnonymityHashShowed.Add(
                                                                ListTransactionAnonymityHashShowed.Count, decryptedTransaction);
                                                        }
                                                    }
                                                }
                                                if (loadingDate + 10 < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                                                {
                                                    ListTransactionHashShowed.Clear();
                                                    ListTransactionAnonymityHashShowed.Clear();
                                                    new Thread(() => StopUpdateTransactionHistory(true, false)).Start();
                                                    break;
                                                }
                                                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncBlock && !ClassWalletObject.InSyncTransactionAnonymity)
                                                {
                                                    UpdateLabelSyncInformation(
                                                        "Total transactions loaded and decrypted: " +
                                                        (ListTransactionHashShowed.Count +
                                                         ListTransactionAnonymityHashShowed.Count) + "/" +
                                                        (copyListTransaction.Count +
                                                         copyListAnonymousTransaction.Count));
                                                }

                                            }
                                            else
                                            {
                                                UpdateLabelSyncInformation("Total transactions downloaded: " + (ClassWalletTransactionCache.ListTransaction.Count +
                                                                             ClassWalletTransactionAnonymityCache.ListTransaction.Count) + "/" +
                                                                             (ClassWalletObject.TotalTransactionInSync + ClassWalletObject.TotalTransactionInSyncAnonymity));
                                            }
                                        }
                                        invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = true;
                                        BeginInvoke(invokeLockButton);
                                    }
                                    else
                                    {
                                        MethodInvoker invokeLockButton = () => SendTransactionWalletForm.buttonSendTransaction.Enabled = true;
                                        BeginInvoke(invokeLockButton);
                                    }

                                    if (!ClassWalletObject.InSyncTransaction &&
                                        !ClassWalletObject.InSyncTransactionAnonymity &&
                                        (ListTransactionHashShowed.Count + ListTransactionAnonymityHashShowed.Count) ==
                                        copyListTransaction.Count +
                                        copyListAnonymousTransaction.Count)
                                    {
                                        if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
                                        {
                                            UpdateLabelSyncInformation(
                                                "Total transactions showed: " +
                                                (ListTransactionHashShowed.Count +
                                                 ListTransactionAnonymityHashShowed.Count) + "/" +
                                                (ClassWalletObject.TotalTransactionInSync +
                                                 ClassWalletObject.TotalTransactionInSyncAnonymity));
                                        }
                                    }

                                }
                            }
                            catch (Exception error)
                            {
                                Log.WriteLine("Error on transactions update: " + error.Message);
                                new Thread(() => StopUpdateTransactionHistory(true, false)).Start();
                                break;
                            }

                            if (!ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                            {

                                break;
                            }

                            Thread.Sleep(ThreadUpdateTransactionWalletInterval*2);
                        }
                    });
                }
            }
            catch
            {
                new Thread(() => StopUpdateTransactionHistory(true, false)).Start();
            }
        }

        /// <summary>
        /// Start update block sync.
        /// </summary>
        public void StartUpdateBlockSync()
        {
            if (ClassWalletObject.SeedNodeConnectorWallet == null)
                return;
            if (!EnableUpdateBlockWallet)
            {
                EnableUpdateBlockWallet = true;


                ThreadPool.QueueUserWorkItem(async delegate
                {
                    while (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                    {


                            try
                            {


                                for (int i = TotalBlockRead; i < ClassBlockCache.ListBlock.Count; i++)
                                {

                                    if (i < ClassBlockCache.ListBlock.Count)
                                    {
                                        string[] splitBlock = ClassBlockCache.ListBlock[i]
                                            .Split(new[] { "#" }, StringSplitOptions.None);
                                        string hash = splitBlock[1];
                                        if (!ListBlockHashShowed.ContainsValue(hash))
                                        {
                                            ListBlockHashShowed.Add(i, hash);
                                            int minShow = (CurrentBlockExplorerPage - 1) * MaxBlockPerPage;
                                            int maxShow = CurrentBlockExplorerPage * MaxBlockPerPage;

                                            if (i >= minShow && i < maxShow)
                                            {
                                                string blockHeight = splitBlock[0];
                                                string transactionHash = splitBlock[2];
                                                string timestampCreate = splitBlock[3];
                                                string timestampFound = splitBlock[4];
                                                string difficulty = splitBlock[5];
                                                string reward = splitBlock[6];
                                                DateTime dateTimeCreate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                dateTimeCreate = dateTimeCreate.AddSeconds(int.Parse(timestampCreate));
                                                dateTimeCreate = dateTimeCreate.ToLocalTime();
                                                DateTime dateTimeFound = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                dateTimeFound = dateTimeFound.AddSeconds(int.Parse(timestampFound));
                                                dateTimeFound = dateTimeFound.ToLocalTime();

                                                string[] row =
                                                {
                                                    blockHeight, hash,
                                                    reward, difficulty, dateTimeCreate.ToString(CultureInfo.InvariantCulture),
                                                    dateTimeFound.ToString(CultureInfo.InvariantCulture), transactionHash
                                                };
                                                var listViewItem = new ListViewItem(row);

                                                void Invoker() =>
                                                    BlockWalletForm.listViewBlockExplorer.Items.Add(listViewItem);

                                                BeginInvoke((MethodInvoker)Invoker);

                                            }
                                            if (TotalBlockRead == minShow)
                                            {
                                                void MethodInvoker() =>
                                                    BlockWalletForm.AutoResizeColumns(BlockWalletForm
                                                        .listViewBlockExplorer);

                                                BeginInvoke((MethodInvoker)MethodInvoker);
                                            }
                                        }
                                        
                                    }
                                    TotalBlockRead++;


                                    if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                                    {
                                        if (ClassWalletObject.InSyncBlock)
                                        {
                                            UpdateLabelSyncInformation(
                                                "Total blocks downloaded: " + ClassBlockCache.ListBlock.Count + "/" +
                                                ClassWalletObject.TotalBlockInSync + ".");
                                        }
                                        else
                                        {
                                            UpdateLabelSyncInformation(
                                                "Total blocks loaded: " + ListBlockHashShowed.Count + "/" +
                                                ClassBlockCache.ListBlock.Count + ".");
                                        }

                                    }
                                }
                            }
                            catch
                            {
                                await Task.Run(delegate ()
                                {
                                    StopUpdateBlockHistory(false);
                                }).ConfigureAwait(false);
                            }

                            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                            {
                                if (ClassWalletObject.InSyncBlock)
                                {
                                    UpdateLabelSyncInformation(
                                        "Total blocks downloaded: " + ClassBlockCache.ListBlock.Count + "/" +
                                        ClassWalletObject.TotalBlockInSync + ".");
                                }
                                else
                                {
                                    UpdateLabelSyncInformation(
                                        "Total blocks loaded: " + ListBlockHashShowed.Count + "/" +
                                        ClassBlockCache.ListBlock.Count + ".");
                                }
                            }
                            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.Overview)
                            {
                                if (ClassWalletObject.InSyncBlock && !ClassWalletObject.InSyncTransactionAnonymity && !ClassWalletObject.InSyncTransaction)
                                {
                                    UpdateLabelSyncInformation(
                                        "Total blocks downloaded: " + ClassBlockCache.ListBlock.Count + "/" +
                                        ClassWalletObject.TotalBlockInSync + " from node: " + ClassWalletObject.ListWalletConnectToRemoteNode[9].RemoteNodeHost + ".");
                                }
                            }


                            if (BlockWalletForm.listViewBlockExplorer.Items.Count-1 > ClassBlockCache.ListBlock.Count)
                            {
                                await Task.Run(delegate ()
                                {
                                    StopUpdateBlockHistory(false);
                                }).ConfigureAwait(false);
                            }

                            Thread.Sleep(ThreadUpdateTransactionWalletInterval);
                        }


        
                    
                });
            }
        }

        /// <summary>
        /// Disable update transaction history.
        /// </summary>
        public void StopUpdateTransactionHistory(bool fullStop, bool clean, bool switchPage = false)
        {

            if (clean)
            {
                CurrentTransactionHistoryPageAnonymousReceived = 1;
                CurrentTransactionHistoryPageAnonymousSend = 1;
                CurrentTransactionHistoryPageNormalSend = 1;
                CurrentTransactionHistoryPageNormalReceive = 1;
                CurrentTransactionHistoryPageBlockReward = 1;
                _normalTransactionLoaded = false;
                _anonymousTransactionLoaded = false;
                ListTransactionHashShowed.Clear();
                copyListTransaction.Clear();
                copyListAnonymousTransaction.Clear();
                                // Transaction normal
                MethodInvoker invoke = () =>
                {
                    TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items.Clear();

                    // Transaction anonymity
                    TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Items.Clear();

                    // Transaction block reward
                    TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items.Clear();
                };
                BeginInvoke(invoke);
                void MethodInvoker() => labelNoticeCurrentPage.Text = "1";
                BeginInvoke((MethodInvoker) MethodInvoker);
            }
            if (switchPage)
            {
                TotalTransactionAnonymousReceived = 0;
                TotalTransactionAnonymousSend = 0;
                TotalTransactionNormalReceived = 0;
                TotalTransactionNormalSend = 0;
                TotalTransactionBlockReward = 0;
                TotalTransactionRead = 0;
                TotalAnonymityTransactionRead = 0;
                _normalTransactionLoaded = false;
                _anonymousTransactionLoaded = false;

                // Transaction normal
                MethodInvoker invoke = () =>
                {
                    TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items.Clear();

                    // Transaction anonymity
                    TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Items.Clear();

                    // Transaction block reward
                    TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items.Clear();
                };
                BeginInvoke(invoke);


            }
            else
            {

                TotalTransactionAnonymousReceived = 0;
                TotalTransactionAnonymousSend = 0;
                TotalTransactionNormalReceived = 0;
                TotalTransactionNormalSend = 0;
                TotalTransactionBlockReward = 0;
                TotalTransactionRead = 0;
                TotalAnonymityTransactionRead = 0;
                ListTransactionAnonymityHashShowed.Clear();
                ListTransactionHashShowed.Clear();
                EnableUpdateTransactionWallet = false;
                _normalTransactionLoaded = false;
                _anonymousTransactionLoaded = false;

                // Transaction normal
                MethodInvoker invoke = () =>
                {
                    TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items.Clear();
                    // Transaction anonymity
                    TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items.Clear();
                    TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Items.Clear();
                    // Transaction block reward
                    TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items.Clear();
                    labelNoticeCurrentPage.Text = "1";
                };
                BeginInvoke(invoke);


            }
        }

        /// <summary>
        /// Disable update block history.
        /// </summary>
        public void StopUpdateBlockHistory(bool fullStop, bool switchPage = false)
        {


            void invoke () => labelNoticeCurrentPage.Text = "1";
            if (!switchPage)
            {
                CurrentBlockExplorerPage = 1;
                TotalBlockRead = 0;
                ListBlockHashShowed.Clear();
                EnableUpdateBlockWallet = false;
                void MethodInvoker() => BlockWalletForm.listViewBlockExplorer.Items.Clear();
                BeginInvoke((MethodInvoker)MethodInvoker);
                BeginInvoke((MethodInvoker)invoke);
            }
            else
            {
                TotalBlockRead = 0;
                ListBlockHashShowed.Clear();
                void MethodInvoker() => BlockWalletForm.listViewBlockExplorer.Items.Clear();
                BeginInvoke((MethodInvoker)MethodInvoker);
                StartUpdateBlockSync();
            }

        }

        #endregion

        #region  Wallet Resize Interface Functions.

        /// <summary>
        /// Resize interface and each controls inside automaticaly.
        /// </summary>
        public void ResizeWalletInterface()
        {
#if WINDOWS
            try
            {
                void MethodInvoker()
                {
                    if (Height > BaseInterfaceHeight || Width > BaseInterfaceWidth)
                    {
                        if (ClassFormPhase.WalletXiropht != null)
                        {
                            if ((CurrentInterfaceWidth != Width && Width >= BaseInterfaceWidth) ||
                                (CurrentInterfaceHeight != Height && Height >= BaseInterfaceHeight))
                            {

                                #region Update Width

                                for (int i = 0; i < ListControlSizeBase.Count; i++)
                                {
                                    if (i < ListControlSizeBase.Count)
                                    {
                                        if (i < Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentWidth = BaseInterfaceWidth;

                                            float ratioWidth = ((float)Width / currentWidth);
                                            float controlWitdh = ListControlSizeBase[i1].Item1.Width * ratioWidth;
                                            float controlLocationX = ListControlSizeBase[i1].Item2.X * ratioWidth;
                                            float controlLocationY = Controls[i1].Location.Y;
                                            bool ignore =
                                                Controls[i1] is DataGridView || Controls[i1] is ListView ||
                                                Controls[i1] is TabPage;

                                            if (!ignore)
                                            {
                                                if (Controls[i1] is Label
#if WINDOWS
                                                            || Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    Controls[i1].Font = new Font(Controls[i1].Font.FontFamily,
                                                        ((float)(Width * 1.000003f) / 100), Controls[i1].Font.Style);
                                                    Controls[i1].Location = new Point((int)controlLocationX,
                                                        (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    Controls[i1].Width = (int)controlWitdh;
                                                    Controls[i1].Location = new Point((int)controlLocationX,
                                                        (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeMain.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeMain.Count; i++)
                                    {
                                        if (i < ListControlSizeMain.Count)
                                        {
                                            if (i < MainWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh = ListControlSizeMain[i1].Item1.Width * ratioWidth;
                                                float controlLocationX = ListControlSizeMain[i1].Item2.X * ratioWidth;
                                                float controlLocationY = MainWalletForm.Controls[i1].Location.Y;
                                                if (MainWalletForm.Controls[i1] is Label
#if WINDOWS
                                                            ||
                                                    MainWalletForm.Controls[i1] is MetroLabel
#endif
                                                        )
                                                {
                                                    MainWalletForm.Controls[i1].Font =
                                                        new Font(MainWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)MainWalletForm.Width * 1.0014f) / 100),
                                                            MainWalletForm.Controls[i1].Font.Style);
                                                    MainWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    MainWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    MainWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeOpenWallet.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                    {
                                        if (i < ListControlSizeOpenWallet.Count)
                                        {
                                            if (i < OpenWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeOpenWallet[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeOpenWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = OpenWalletForm.Controls[i1].Location.Y;
                                                if (OpenWalletForm.Controls[i1] is Label
#if WINDOWS
                                                             ||
                                                    OpenWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    OpenWalletForm.Controls[i1].Font =
                                                        new Font(OpenWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)OpenWalletForm.Width * 1.0014f) / 100),
                                                            OpenWalletForm.Controls[i1].Font.Style);
                                                    OpenWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    OpenWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    OpenWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeOverview.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeOverview.Count; i++)
                                    {
                                        if (i < ListControlSizeOverview.Count)
                                        {
                                            if (i < OverviewWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeOverview[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeOverview[i1].Item2.X * ratioWidth;
                                                float controlLocationY = OverviewWalletForm.Controls[i1].Location.Y;

                                                if (OverviewWalletForm.Controls[i1] is Label
#if WINDOWS
                                                             ||
                                                    OverviewWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    OverviewWalletForm.Controls[i1].Font =
                                                        new Font(OverviewWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)OverviewWalletForm.Width * 1.0014f) / 100),
                                                            OverviewWalletForm.Controls[i1].Font.Style);
                                                    OverviewWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    OverviewWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    OverviewWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeTransaction.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeTransaction.Count; i++)
                                    {
                                        if (i < ListControlSizeTransaction.Count)
                                        {
                                            if (i < TransactionHistoryWalletForm.Controls.Count)
                                            {
                                                var i1 = i;

                                                bool ignore =
                                                    TransactionHistoryWalletForm.Controls[i1] is DataGridView ||
                                                    TransactionHistoryWalletForm.Controls[i1] is ListView ||
                                                    TransactionHistoryWalletForm.Controls[i1] is TabPage ||
                                                    TransactionHistoryWalletForm.Controls[i1] is Panel;

                                                if (!ignore)
                                                {
                                                    var currentWidth = BaseInterfaceWidth;

                                                    float ratioWidth = ((float)Width / currentWidth);
                                                    float controlWitdh =
                                                        ListControlSizeTransaction[i1].Item1.Width * ratioWidth;
                                                    float controlLocationX =
                                                        ListControlSizeTransaction[i1].Item2.X * ratioWidth;
                                                    float controlLocationY = TransactionHistoryWalletForm.Controls[i1]
                                                        .Location.Y;

                                                    if (TransactionHistoryWalletForm.Controls[i1] is Label
#if WINDOWS
                                                                ||
                                                        TransactionHistoryWalletForm.Controls[i1] is MetroLabel
#endif
                                                                )
                                                    {
                                                        TransactionHistoryWalletForm.Controls[i1].Font = new Font(
                                                            TransactionHistoryWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)TransactionHistoryWalletForm.Width * 1.0014f) /
                                                             100),
                                                            TransactionHistoryWalletForm.Controls[i1].Font.Style);
                                                        TransactionHistoryWalletForm.Controls[i1].Location =
                                                            new Point((int)controlLocationX, (int)controlLocationY);
                                                    }
                                                    else
                                                    {
                                                        TransactionHistoryWalletForm.Controls[i1].Width =
                                                            (int)controlWitdh;
                                                        TransactionHistoryWalletForm.Controls[i1].Location =
                                                            new Point((int)controlLocationX, (int)controlLocationY);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                if (ListControlSizeSendTransaction.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                    {
                                        if (i < ListControlSizeSendTransaction.Count)
                                        {
                                            if (i < SendTransactionWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeSendTransaction[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeSendTransaction[i1].Item2.X * ratioWidth;
                                                float controlLocationY =
                                                    SendTransactionWalletForm.Controls[i1].Location.Y;
                                                if (SendTransactionWalletForm.Controls[i1] is Label
#if WINDOWS
                                                            ||
                                                    SendTransactionWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    SendTransactionWalletForm.Controls[i1].Font = new Font(
                                                        SendTransactionWalletForm.Controls[i1].Font.FontFamily,
                                                        (((float)SendTransactionWalletForm.Width * 1.0014f) / 100),
                                                        SendTransactionWalletForm.Controls[i1].Font.Style);
                                                    SendTransactionWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    SendTransactionWalletForm.Controls[i1].Font = new Font(
                                                        SendTransactionWalletForm.Controls[i1].Font.FontFamily,
                                                        (((float)SendTransactionWalletForm.Width * 1.0014f) / 100),
                                                        SendTransactionWalletForm.Controls[i1].Font.Style);
                                                    SendTransactionWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    SendTransactionWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeCreateWallet.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                    {
                                        if (i < ListControlSizeCreateWallet.Count)
                                        {
                                            if (i < CreateWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeCreateWallet[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeCreateWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = CreateWalletForm.Controls[i1].Location.Y;
                                                if (CreateWalletForm.Controls[i1] is Label
#if WINDOWS
                                                            ||
                                                    CreateWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    CreateWalletForm.Controls[i1].Font =
                                                        new Font(CreateWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)CreateWalletForm.Width * 1.0014f) / 100),
                                                            CreateWalletForm.Controls[i1].Font.Style);
                                                    CreateWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    CreateWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    CreateWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeRestoreWallet.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                    {
                                        if (i < ListControlSizeRestoreWallet.Count)
                                        {
                                            if (i < RestoreWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeRestoreWallet[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeRestoreWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = RestoreWalletForm.Controls[i1].Location.Y;
                                                if (RestoreWalletForm.Controls[i1] is Label
#if WINDOWS
                                                            ||
                                                    RestoreWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    RestoreWalletForm.Controls[i1].Font =
                                                        new Font(RestoreWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)RestoreWalletForm.Width * 1.0014f) / 100),
                                                            RestoreWalletForm.Controls[i1].Font.Style);
                                                    RestoreWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    RestoreWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    RestoreWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeContactWallet.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeContactWallet.Count; i++)
                                    {
                                        if (i < ListControlSizeContactWallet.Count)
                                        {
                                            if (i < ContactWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                float ratioWidth = ((float)Width / currentWidth);
                                                float controlWitdh =
                                                    ListControlSizeContactWallet[i1].Item1.Width * ratioWidth;
                                                float controlLocationX =
                                                    ListControlSizeContactWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = ContactWalletForm.Controls[i1].Location.Y;
                                                if (ContactWalletForm.Controls[i1] is Label
#if WINDOWS
                                                            ||
                                                    ContactWalletForm.Controls[i1] is MetroLabel
#endif
                                                            )
                                                {
                                                    ContactWalletForm.Controls[i1].Font =
                                                        new Font(ContactWalletForm.Controls[i1].Font.FontFamily,
                                                            (((float)ContactWalletForm.Width * 1.0014f) / 100),
                                                            ContactWalletForm.Controls[i1].Font.Style);
                                                    ContactWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    ContactWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    ContactWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ListControlSizeBlock.Count > 0)
                                {
                                    for (int i = 0; i < ListControlSizeBlock.Count; i++)
                                    {
                                        if (i < ListControlSizeBlock.Count)
                                        {
                                            if (i < BlockWalletForm.Controls.Count)
                                            {
                                                var i1 = i;

                                                bool ignore =
                                                    BlockWalletForm.Controls[i1] is DataGridView ||
                                                    BlockWalletForm.Controls[i1] is ListView ||
                                                    BlockWalletForm.Controls[i1] is TabPage;
                                                if (!ignore)
                                                {
                                                    var currentWidth = BaseInterfaceWidth;

                                                    float ratioWidth = ((float)Width / currentWidth);
                                                    float controlWitdh =
                                                        ListControlSizeBlock[i1].Item1.Width * ratioWidth;
                                                    float controlLocationX =
                                                        ListControlSizeBlock[i1].Item2.X * ratioWidth;
                                                    float controlLocationY = BlockWalletForm.Controls[i1].Location.Y;
                                                    BlockWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    BlockWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion


                                #region Update Height

                                for (int i = 0; i < ListControlSizeBase.Count; i++)
                                {
                                    if (i < ListControlSizeBase.Count)
                                    {
                                        if (i < Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh = ListControlSizeBase[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = Controls[i1].Location.X;
                                            float controlLocationY = ListControlSizeBase[i1].Item2.Y * ratioHeight;
                                            Controls[i1].Height = (int)controlWitdh;
                                            Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeMain.Count; i++)
                                {
                                    if (i < ListControlSizeMain.Count)
                                    {
                                        if (i < MainWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh = ListControlSizeMain[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = MainWalletForm.Controls[i1].Location.X;
                                            float controlLocationY = ListControlSizeMain[i1].Item2.Y * ratioHeight;
                                            MainWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            MainWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                {
                                    if (i < ListControlSizeOpenWallet.Count)
                                    {
                                        if (i < OpenWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh =
                                                ListControlSizeOpenWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = OpenWalletForm.Controls[i1].Location.X;
                                            float controlLocationY =
                                                ListControlSizeOpenWallet[i1].Item2.Y * ratioHeight;
                                            OpenWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            OpenWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeOverview.Count; i++)
                                {
                                    if (i < ListControlSizeOverview.Count)
                                    {
                                        if (i < OverviewWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh = ListControlSizeOverview[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = OverviewWalletForm.Controls[i1].Location.X;
                                            float controlLocationY = ListControlSizeOverview[i1].Item2.Y * ratioHeight;
                                            OverviewWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            OverviewWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeTransaction.Count; i++)
                                {
                                    if (i < ListControlSizeTransaction.Count)
                                    {
                                        if (i < TransactionHistoryWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            bool ignore = TransactionHistoryWalletForm.Controls[i1] is DataGridView ||
                                                          TransactionHistoryWalletForm.Controls[i1] is ListView ||
                                                          TransactionHistoryWalletForm.Controls[i1] is TabPage ||
                                                          TransactionHistoryWalletForm.Controls[i1] is Panel;
                                            if (!ignore)
                                            {
                                                var currentHeight = BaseInterfaceHeight;

                                                float ratioHeight = ((float)Height / currentHeight);
                                                float controlWitdh =
                                                    ListControlSizeTransaction[i1].Item1.Height * ratioHeight;
                                                float controlLocationX =
                                                    TransactionHistoryWalletForm.Controls[i1].Location.X;
                                                float controlLocationY =
                                                    ListControlSizeTransaction[i1].Item2.Y * ratioHeight;
                                                TransactionHistoryWalletForm.Controls[i1].Height = (int)controlWitdh;
                                                TransactionHistoryWalletForm.Controls[i1].Location =
                                                    new Point((int)controlLocationX, (int)controlLocationY);
                                            }
                                        }
                                    }
                                }


                                for (int i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                {
                                    if (i < ListControlSizeSendTransaction.Count)
                                    {
                                        if (i < SendTransactionWalletForm.Controls.Count)
                                        {
                                            var i1 = i;

                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh =
                                                ListControlSizeSendTransaction[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = SendTransactionWalletForm.Controls[i1].Location.X;
                                            float controlLocationY =
                                                ListControlSizeSendTransaction[i1].Item2.Y * ratioHeight;
                                            SendTransactionWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            SendTransactionWalletForm.Controls[i1].Location =
                                                new Point((int)controlLocationX, (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                {
                                    if (i < ListControlSizeCreateWallet.Count)
                                    {
                                        if (i < CreateWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh =
                                                ListControlSizeCreateWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = CreateWalletForm.Controls[i1].Location.X;
                                            float controlLocationY =
                                                ListControlSizeCreateWallet[i1].Item2.Y * ratioHeight;
                                            CreateWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            CreateWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                {
                                    if (i < ListControlSizeRestoreWallet.Count)
                                    {
                                        if (i < RestoreWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh =
                                                ListControlSizeRestoreWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = RestoreWalletForm.Controls[i1].Location.X;
                                            float controlLocationY =
                                                ListControlSizeRestoreWallet[i1].Item2.Y * ratioHeight;
                                            RestoreWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            RestoreWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeContactWallet.Count; i++)
                                {
                                    if (i < ListControlSizeContactWallet.Count)
                                    {
                                        if (i < ContactWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            float ratioHeight = ((float)Height / currentHeight);
                                            float controlWitdh =
                                                ListControlSizeContactWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = ContactWalletForm.Controls[i1].Location.X;
                                            float controlLocationY =
                                                ListControlSizeContactWallet[i1].Item2.Y * ratioHeight;
                                            ContactWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            ContactWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }
                                    }
                                }

                                for (int i = 0; i < ListControlSizeBlock.Count; i++)
                                {
                                    if (i < ListControlSizeBlock.Count)
                                    {
                                        if (i < BlockWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            bool ignore =
                                                BlockWalletForm.Controls[i1] is DataGridView ||
                                                BlockWalletForm.Controls[i1] is ListView ||
                                                BlockWalletForm.Controls[i1] is TabPage;

                                            if (!ignore)
                                            {
                                                var currentHeight = BaseInterfaceHeight;

                                                float ratioHeight = ((float)Height / currentHeight);
                                                float controlWitdh =
                                                    ListControlSizeBlock[i1].Item1.Height * ratioHeight;
                                                float controlLocationX = BlockWalletForm.Controls[i1].Location.X;
                                                float controlLocationY = ListControlSizeBlock[i1].Item2.Y * ratioHeight;
                                                BlockWalletForm.Controls[i1].Height = (int)controlWitdh;
                                                BlockWalletForm.Controls[i1].Location =
                                                    new Point((int)controlLocationX, (int)controlLocationY);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                CurrentInterfaceHeight = Height;
                                OpenWalletForm.Size = panelMainForm.Size;
                                MainWalletForm.Size = panelMainForm.Size;
                                OverviewWalletForm.Size = panelMainForm.Size;
                                TransactionHistoryWalletForm.Size = panelMainForm.Size;
                                SendTransactionWalletForm.Size = panelMainForm.Size;
                                CreateWalletForm.Size = panelMainForm.Size;
                                BlockWalletForm.Size = panelMainForm.Size;
                                RestoreWalletForm.Size = panelMainForm.Size;
                                ContactWalletForm.Size = panelMainForm.Size;
                            }
                            else
                            {
                                if (Height < BaseInterfaceHeight)
                                {
                                    Height = BaseInterfaceHeight;
                                }

                                if (Width < BaseInterfaceWidth)
                                {
                                    Width = BaseInterfaceWidth;
                                }
                            }
                        }
                    }
                    else // Restore interface size.
                    {
                        if (ClassFormPhase.WalletXiropht != null)
                        {
                            if (ListControlSizeBase.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeBase.Count; i++)
                                {
                                    if (i < ListControlSizeBase.Count)
                                    {
                                        if (i < Controls.Count)
                                        {
                                            Controls[i].Size = ListControlSizeBase[i].Item1;
                                            Controls[i].Location = ListControlSizeBase[i].Item2;
                                            if (Controls[i] is Label
#if WINDOWS
                                                || Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                Controls[i].Font = new Font(Controls[i].Font.FontFamily,
                                                    ((float)(Width * 1.000003f) / 100), Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeMain.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeMain.Count; i++)
                                {
                                    if (i < ListControlSizeMain.Count)
                                    {
                                        if (i < MainWalletForm.Controls.Count)
                                        {
                                            MainWalletForm.Controls[i].Size = ListControlSizeMain[i].Item1;
                                            MainWalletForm.Controls[i].Location = ListControlSizeMain[i].Item2;
                                            if (MainWalletForm.Controls[i] is Label
#if WINDOWS
                                               || MainWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                MainWalletForm.Controls[i].Font =
                                                    new Font(MainWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(MainWalletForm.Width * 1.0014f) / 100),
                                                        MainWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeOpenWallet.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                {
                                    if (i < ListControlSizeOpenWallet.Count)
                                    {
                                        if (i < OpenWalletForm.Controls.Count)
                                        {
                                            OpenWalletForm.Controls[i].Size = ListControlSizeOpenWallet[i].Item1;
                                            OpenWalletForm.Controls[i].Location = ListControlSizeOpenWallet[i].Item2;
                                            if (OpenWalletForm.Controls[i] is Label
#if WINDOWS
                                                    || OpenWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                OpenWalletForm.Controls[i].Font =
                                                    new Font(OpenWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(OpenWalletForm.Width * 1.0014f) / 100),
                                                        OpenWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeOverview.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeOverview.Count; i++)
                                {
                                    if (i < ListControlSizeOverview.Count)
                                    {
                                        if (i < OverviewWalletForm.Controls.Count)
                                        {
                                            OverviewWalletForm.Controls[i].Size = ListControlSizeOverview[i].Item1;
                                            OverviewWalletForm.Controls[i].Location = ListControlSizeOverview[i].Item2;
                                            if (OverviewWalletForm.Controls[i] is Label
#if WINDOWS
                                                || OverviewWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                OverviewWalletForm.Controls[i].Font =
                                                    new Font(OverviewWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(OverviewWalletForm.Width * 1.0014f) / 100),
                                                        OverviewWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeTransaction.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeTransaction.Count; i++)
                                {
                                    if (i < ListControlSizeTransaction.Count)
                                    {
                                        if (i < TransactionHistoryWalletForm.Controls.Count)
                                        {
                                            TransactionHistoryWalletForm.Controls[i].Size =
                                                ListControlSizeTransaction[i].Item1;
                                            TransactionHistoryWalletForm.Controls[i].Location =
                                                ListControlSizeTransaction[i].Item2;
                                            if (TransactionHistoryWalletForm.Controls[i] is Label
#if WINDOWS
                                                || TransactionHistoryWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                TransactionHistoryWalletForm.Controls[i].Font = new Font(
                                                    TransactionHistoryWalletForm.Controls[i].Font.FontFamily,
                                                    ((float)(TransactionHistoryWalletForm.Width * 1.0014f) / 100),
                                                    TransactionHistoryWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeSendTransaction.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                {
                                    if (i < ListControlSizeSendTransaction.Count)
                                    {
                                        if (i < SendTransactionWalletForm.Controls.Count)
                                        {
                                            SendTransactionWalletForm.Controls[i].Size =
                                                ListControlSizeSendTransaction[i].Item1;
                                            SendTransactionWalletForm.Controls[i].Location =
                                                ListControlSizeSendTransaction[i].Item2;
                                            if (SendTransactionWalletForm.Controls[i] is Label
#if WINDOWS
                                                || SendTransactionWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                SendTransactionWalletForm.Controls[i].Font = new Font(
                                                    SendTransactionWalletForm.Controls[i].Font.FontFamily,
                                                    ((float)(SendTransactionWalletForm.Width * 1.0014f) / 100),
                                                    SendTransactionWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeCreateWallet.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                {
                                    if (i < ListControlSizeCreateWallet.Count)
                                    {
                                        if (i < CreateWalletForm.Controls.Count)
                                        {
                                            CreateWalletForm.Controls[i].Size = ListControlSizeCreateWallet[i].Item1;
                                            CreateWalletForm.Controls[i].Location =
                                                ListControlSizeCreateWallet[i].Item2;
                                            if (CreateWalletForm.Controls[i] is Label
#if WINDOWS
                                                || CreateWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                CreateWalletForm.Controls[i].Font =
                                                    new Font(CreateWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(CreateWalletForm.Width * 1.0014f) / 100),
                                                        CreateWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeBlock.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeBlock.Count; i++)
                                {
                                    if (i < ListControlSizeBlock.Count)
                                    {
                                        if (i < BlockWalletForm.Controls.Count)
                                        {
                                            BlockWalletForm.Controls[i].Size = ListControlSizeBlock[i].Item1;
                                            BlockWalletForm.Controls[i].Location = ListControlSizeBlock[i].Item2;
                                            if (BlockWalletForm.Controls[i] is Label
#if WINDOWS
                                                || BlockWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                BlockWalletForm.Controls[i].Font =
                                                    new Font(BlockWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(BlockWalletForm.Width * 1.0014f) / 100),
                                                        BlockWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeRestoreWallet.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                {
                                    if (i < ListControlSizeRestoreWallet.Count)
                                    {
                                        if (i < RestoreWalletForm.Controls.Count)
                                        {
                                            RestoreWalletForm.Controls[i].Size = ListControlSizeRestoreWallet[i].Item1;
                                            RestoreWalletForm.Controls[i].Location =
                                                ListControlSizeRestoreWallet[i].Item2;
                                            if (RestoreWalletForm.Controls[i] is Label
#if WINDOWS
                                                || RestoreWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                RestoreWalletForm.Controls[i].Font =
                                                    new Font(RestoreWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(RestoreWalletForm.Width * 1.0014f) / 100),
                                                        RestoreWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }

                            if (ListControlSizeContactWallet.Count > 0)
                            {
                                for (int i = 0; i < ListControlSizeContactWallet.Count; i++)
                                {
                                    if (i < ListControlSizeContactWallet.Count)
                                    {
                                        if (i < ContactWalletForm.Controls.Count)
                                        {
                                            ContactWalletForm.Controls[i].Size = ListControlSizeContactWallet[i].Item1;
                                            ContactWalletForm.Controls[i].Location =
                                                ListControlSizeContactWallet[i].Item2;
                                            if (ContactWalletForm.Controls[i] is Label
#if WINDOWS
                                                || ContactWalletForm.Controls[i] is MetroLabel
#endif
                                                )
                                            {
                                                ContactWalletForm.Controls[i].Font =
                                                    new Font(ContactWalletForm.Controls[i].Font.FontFamily,
                                                        ((float)(ContactWalletForm.Width * 1.0014f) / 100),
                                                        ContactWalletForm.Controls[i].Font.Style);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        Height = BaseInterfaceHeight;
                        CurrentInterfaceHeight = Height;
                        Width = BaseInterfaceWidth;
                        CurrentInterfaceWidth = Width;
                    }
                }
                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
#endif
        }

        #endregion

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColorStyle(Color.Black, Color.White, Color.PaleTurquoise);
        }


        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColorStyle(Color.White, Color.Black, Color.White);
        }

        /// <summary>
        /// Change colors of all controls of the wallet interface.
        /// </summary>
        /// <param name="background"></param>
        /// <param name="text"></param>
        private void UpdateColorStyle(Color background, Color text, Color backgroundList)
        {
#if WINDOWS
            if (background == Color.Black)
            {
                Theme = MetroThemeStyle.Dark;
            }
            else if (background == Color.White)
            {
                Theme = MetroThemeStyle.Light;
            }
#else
            BackColor = background;
#endif
            for (int i = 0; i < Controls.Count; i++)
            {
                if (i < Controls.Count)
                {
                    Controls[i].ForeColor = text;
                }
            }
            OpenWalletForm.BackColor = background;
            for (int i = 0; i < OpenWalletForm.Controls.Count; i++)
            {
                if (i < OpenWalletForm.Controls.Count)
                {
                    if (!(OpenWalletForm.Controls[i] is TextBox))
                    {
                        OpenWalletForm.Controls[i].BackColor = background;
                        OpenWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            MainWalletForm.BackColor = background;
            for (int i = 0; i < MainWalletForm.Controls.Count; i++)
            {
                if (i < MainWalletForm.Controls.Count)
                {
                    if (!(MainWalletForm.Controls[i] is TextBox))
                    {
                        MainWalletForm.Controls[i].BackColor = background;
                        MainWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            OverviewWalletForm.BackColor = background;
            for (int i = 0; i < OverviewWalletForm.Controls.Count; i++)
            {
                if (i < OverviewWalletForm.Controls.Count)
                {
                    if (!(OverviewWalletForm.Controls[i] is TextBox))
                    {
                        OverviewWalletForm.Controls[i].BackColor = background;
                        OverviewWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            SendTransactionWalletForm.BackColor = background;
            for (int i = 0; i < SendTransactionWalletForm.Controls.Count; i++)
            {
                if (i < SendTransactionWalletForm.Controls.Count)
                {
                    if (!(SendTransactionWalletForm.Controls[i] is TextBox))
                    {
                        SendTransactionWalletForm.Controls[i].BackColor = background;
                        SendTransactionWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            CreateWalletForm.BackColor = background;
            for (int i = 0; i < CreateWalletForm.Controls.Count; i++)
            {
                if (i < CreateWalletForm.Controls.Count)
                {
                    if (!(CreateWalletForm.Controls[i] is TextBox))
                    {
                        CreateWalletForm.Controls[i].BackColor = background;
                        CreateWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            RestoreWalletForm.BackColor = background;
            for (int i = 0; i < RestoreWalletForm.Controls.Count; i++)
            {
                if (i < RestoreWalletForm.Controls.Count)
                {
                    if (!(RestoreWalletForm.Controls[i] is TextBox))
                    {
                        RestoreWalletForm.Controls[i].BackColor = background;
                        RestoreWalletForm.Controls[i].ForeColor = text;
                    }
                }
            }
            for (int i = 0; i < panelControlWallet.Controls.Count; i++)
            {
                if (i < panelControlWallet.Controls.Count)
                {
                    panelControlWallet.Controls[i].ForeColor = text;
                }
            }
        }

    }
}