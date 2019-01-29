using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
#if WINDOWS
using MetroFramework;
#endif
using Xiropht_Connector.Remote;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Seed;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.FormPhase;
using Xiropht_Wallet.FormPhase.ParallelForm;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletObject
    {
        /// <summary>
        ///  Object connection.
        /// </summary>
        public static string Certificate;
        public static ClassSeedNodeConnector SeedNodeConnectorWallet;
        public static ClassWalletConnect WalletConnect;
        public static List<ClassWalletConnectToRemoteNode> ListWalletConnectToRemoteNode;
        public static List<string> ListRemoteNodeBanned = new List<string>();
        public static string WalletLastPathFile;
        public static bool WalletPinDisabled;


        /// <summary>
        ///     Object
        /// </summary>
        public static bool EnableCheckRemoteNodeList;


        /// <summary>
        ///     For create a new wallet.
        /// </summary>
        public static string WalletDataCreation;
        public static string WalletDataPinCreation;
        public static string WalletDataCreationPath;
        public static bool InCreateWallet;
        public static string WalletDataDecrypted;
        public static string WalletNewPassword;


        /// <summary>
        ///     Network stats.
        /// </summary>
        public static string CoinMaxSupply;
        public static string CoinCirculating;
        public static string TotalFee;
        public static string TotalBlockMined;
        public static string NetworkDifficulty;
        public static string NetworkHashrate;
        public static int RemoteNodeTotalPendingTransactionInNetwork;

        /// <summary>
        /// Check network stats.
        /// </summary>
        private static int WalletCheckMaxSupply;
        private static int WalletCheckCoinCirculating;
        private static int WalletCheckTotalTransactionFee;
        private static int WalletCheckTotalBlockMined;
        private static int WalletCheckNetworkHashrate;
        private static int WalletCheckNetworkDifficulty;
        private static int WalletCheckTotalPendingTransaction;
        private static int WalletCheckBlockPerId;


        /// <summary>
        ///  For the sync of transactions.
        /// </summary>
        public static bool InSyncTransaction;
        public static bool InSyncTransactionAnonymity;
        public static bool BlockTransactionSync;
        public static bool InReceiveTransaction;
        public static int TotalTransactionInSync;
        public static bool InReceiveTransactionAnonymity;
        public static int TotalTransactionInSyncAnonymity;
        private static Thread _threadWalletSyncTransaction;
        private static Thread _threadWalletSyncTransactionAnonymity;
        public static int TotalTransactionPendingOnReceive;

        /// <summary>
        /// For the sync of blocks.
        /// </summary>
        public static bool InSyncBlock;
        public static bool InReceiveBlock;
        public static int TotalBlockInSync;
        public static string LastBlockFound;
        private static Thread _threadWalletSyncBlock;
        private static long _lastBlockReceived;


        /// <summary>
        ///     Threading for network connection of the wallet.
        /// </summary>
        private static Thread _threadListenSeedNodeNetwork;
        private static Thread _threadWalletKeepAlive;

        /// <summary>
        ///  object for remote node connection to sync the wallet.
        /// </summary>
        public static bool EnableReceivePacketRemoteNode;
        private static Thread _threadListenRemoteNodeNetwork1;
        private static Thread _threadListenRemoteNodeNetwork2;
        private static Thread _threadListenRemoteNodeNetwork3;
        private static Thread _threadListenRemoteNodeNetwork4;
        private static Thread _threadListenRemoteNodeNetwork5;
        private static Thread _threadListenRemoteNodeNetwork6;
        private static Thread _threadListenRemoteNodeNetwork7;
        private static Thread _threadListenRemoteNodeNetwork8;
        private static Thread _threadListenRemoteNodeNetwork9;
        private static Thread _threadListenRemoteNodeNetwork10;
        private static Thread _threadListenRemoteNodeNetwork11;
        private static Thread _threadListenRemoteNodeNetwork12;
        private static Thread _threadSendRemoteNodePacketNetwork;
        private static Thread _threadCheckRemoteNodePacketNetwork;
        public static int WalletSyncMode;
        public static string WalletSyncHostname;
        public static bool WalletClosed;
        public static bool WalletOnSendingPacketRemoteNode;
        private static bool OnWaitingRemoteNodeList;
        private static bool WalletOnUseSync;
        private static bool WalletInReconnect;
        public static bool SettingManualRemoteNode;
        public static long LastRemoteNodePacketReceived;
        public static string WalletAmountInPending;
        public static int WalletPacketSpeedTime;

        #region Initialization

        /// <summary>
        ///     Start to connect on blockchain.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InitializationWalletConnection(string walletAddress, string walletPassword,
            string walletKey,
            string phase)
        {
            WalletClosed = false;
            Certificate = Xiropht_Connector_All.Utils.ClassUtils.GenerateCertificate();
            if (SeedNodeConnectorWallet == null) // First initialization
            {
                SeedNodeConnectorWallet = new ClassSeedNodeConnector();
                WalletConnect = new ClassWalletConnect(SeedNodeConnectorWallet);
                InitializeWalletObject(walletAddress, walletPassword, walletKey,
                    phase); // Initialization of wallet information.
            }
            else // Renew initialization.
            {
                DisconnectWalletFromSeedNode(true); // Disconnect and renew objects.
                InitializeWalletObject(walletAddress, walletPassword, walletKey,
                    phase); // Initialization of wallet information.
            }

            if (ListWalletConnectToRemoteNode == null) // First initialization
                ListWalletConnectToRemoteNode = new List<ClassWalletConnectToRemoteNode>();
            else // Renew initialization.
                await DisconnectWholeRemoteNodeSync(true, false); // Disconnect and renew objects.


            if (!await SeedNodeConnectorWallet.StartConnectToSeedAsync(string.Empty, ClassConnectorSetting.SeedNodePort, Program.IsLinux))
            {
#if DEBUG
                Log.WriteLine("Connection error with seed node network.");
#endif
                return false;
            }
#if DEBUG
            Log.WriteLine("Connection successfully establised with seed node network.");
#endif
            WalletPinDisabled = true;
            ClassFormPhase.WalletXiropht.UpdateNetworkStats();
            try
            {
                ClassWalletTransactionCache.LoadWalletCache(WalletConnect.WalletAddress);
                TotalTransactionInSync = ClassWalletTransactionCache.ListTransaction.Count;

                if (!ClassFormPhase.WalletXiropht.EnableUpdateTransactionWallet)
                    ClassFormPhase.WalletXiropht.StartUpdateTransactionHistory();
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("Can't read wallet cache, error: " + error.Message);
#endif
#if WINDOWS
                MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_TRANSACTION_CACHE_ERROR_TEXT"));
                ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
               new Thread(delegate()
               {
                   MessageBox.Show(ClassFormPhase.WalletXiropht,
                      ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_TRANSACTION_CACHE_ERROR_TEXT"));
                   ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
               }).Start();
#endif

            }

            try
            {
                ClassWalletTransactionAnonymityCache.LoadWalletCache(WalletConnect.WalletAddress);
                TotalTransactionInSyncAnonymity = ClassWalletTransactionAnonymityCache.ListTransaction.Count;

                if (!ClassFormPhase.WalletXiropht.EnableUpdateTransactionWallet)
                    ClassFormPhase.WalletXiropht.StartUpdateTransactionHistory();
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("Can't read wallet cache, error: " + error.Message);
#endif
#if WINDOWS
                MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ANONYMITY_TRANSACTION_CACHE_ERROR_TEXT"));
                ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
                new Thread(delegate ()
                {
                    MessageBox.Show(ClassFormPhase.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ANONYMITY_TRANSACTION_CACHE_ERROR_TEXT"));
                    ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
                }).Start();
#endif
            }

            try
            {
                ClassBlockCache.LoadBlockchainCache();
                TotalBlockInSync = ClassBlockCache.ListBlock.Count;

                if (!ClassFormPhase.WalletXiropht.EnableUpdateBlockWallet)
                    ClassFormPhase.WalletXiropht.StartUpdateBlockSync();
                ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BLOCK_CACHE_READ_SUCCESS_TEXT"));
            }
            catch(Exception error)
            {
#if DEBUG
                Log.WriteLine("Can't read block cache, error: "+error.Message);
#endif
                ClassBlockCache.RemoveWalletBlockCache();
            }

#if DEBUG
            new Thread(delegate ()
            {
                while (SeedNodeConnectorWallet == null)
                {
                    Thread.Sleep(100);
                }

                while (!SeedNodeConnectorWallet.GetStatusConnectToSeed(Program.IsLinux))
                {
                    Thread.Sleep(100);
                }

                int totalTimeConnected = 0;
                while (SeedNodeConnectorWallet.GetStatusConnectToSeed(Program.IsLinux))
                {
                    Thread.Sleep(1000);
                    totalTimeConnected++;
                }
                Log.WriteLine("Total time connected: " + totalTimeConnected + " second(s).");
            }).Start();
#endif
            return true;
        }

#endregion

        #region Disconnection functions.

        /// <summary>
        ///     Disconnect wallet from remote nodes, seed nodes connections.
        /// </summary>
        public static async Task<bool> FullDisconnection(bool manualDisconnection)
        {
            
            if (!WalletClosed && !WalletInReconnect || manualDisconnection)
            {
                if (manualDisconnection || WalletConnect.WalletPhase == ClassWalletPhase.Create) 
                {
                    try
                    {
                        ClassParallelForm.HidePinForm();
                        ClassFormPhase.HideWalletMenu();
                        ClassParallelForm.HideWaitingCreateWalletForm();

                    }
                    catch
                    {

                    }
                    BlockTransactionSync = false;
                    WalletDataDecrypted = string.Empty;
                    WalletClosed = true;
                    OnWaitingRemoteNodeList = false;

                    DisconnectWalletFromSeedNode(true);

                    await DisconnectWholeRemoteNodeSync(true, false);


                    ClassFormPhase.WalletXiropht.CleanSyncInterfaceWallet();
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
                    ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_DISCONNECTED_TEXT"));

                    ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(true, true);

                    ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(true);
                    ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                }
                else // Try to reconnect.
                {
                    ClassParallelForm.HideWaitingCreateWalletForm();
                    new Thread(() => ClassParallelForm.ShowWaitingReconnectForm()).Start();
                    int maxRetry = 5;

                    new Thread(async delegate ()
                    {
                        while (maxRetry > 0)
                        {
                            try
                            {
                                ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                                ClassParallelForm.HidePinForm();
                                BlockTransactionSync = false;
                                WalletDataDecrypted = string.Empty;
                                WalletClosed = true;
                                WalletInReconnect = true;

                                await DisconnectWholeRemoteNodeSync(true, false);

                                DisconnectWalletFromSeedNode(false, true);

                                Thread.Sleep(1000);

                                if (await InitializationWalletConnection(WalletConnect.WalletAddress, WalletConnect.WalletPassword,
                                    WalletConnect.WalletKey, ClassWalletPhase.Login))
                                {
                                    ListenSeedNodeNetworkForWallet();
                                    if (await WalletConnect.SendPacketWallet(Certificate, string.Empty, false))
                                    {
                                        if (await WalletConnect.SendPacketWallet(
                                            "WALLET|" + WalletConnect.WalletAddress, Certificate, true))
                                        {
                                            var timeoutDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletInReconnect)
                                            {
                                                if (timeoutDate + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    maxRetry--;
                                                    break;
                                                }
                                                Thread.Sleep(100);
                                            }
                                            if (!WalletInReconnect)
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            maxRetry--;
                                        }

                                    }
                                    else
                                    {
                                        maxRetry--;
                                    }
                                }
                                else
                                {
                                    maxRetry--;
                                }
                            }
                            catch
                            {
                                maxRetry--;
                            }
                        }

                        ClassParallelForm.HideWaitingReconnectForm();
                        if (maxRetry <= 0)
                        {
                            DisconnectWalletFromSeedNode(true);
                            ClassFormPhase.HideWalletMenu();

                            ClassFormPhase.WalletXiropht.CleanSyncInterfaceWallet();
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
                            ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_DISCONNECTED_TEXT"));

                            ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(true, true);

                            ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(true);
                            ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
#if WINDOWS
                            MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
#else
                            new Thread(delegate ()
                            {
                                MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                                ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                            }).Start();
#endif
                            WalletInReconnect = false;
                        }
                        else
                        {
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
#if WINDOWS
                            MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
#else
                            new Thread(delegate ()
                            {
                                MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                                ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                            }).Start();
#endif
                            WalletInReconnect = false;
                        }
                    }).Start();
                }
            }

            return true;
        }

#endregion

        #region Wallet Connection

        /// <summary>
        ///     Disconnect wallet from seed nodes.
        /// </summary>
        private static void DisconnectWalletFromSeedNode(bool clean, bool reconnect = false)
        {
            SeedNodeConnectorWallet?.DisconnectToSeed();
            if (clean)
                CleanUpWalletConnnection(reconnect);

            ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(true, false);
            ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(true);
        }

        /// <summary>
        ///     clean up the objects of seed and wallet.
        /// </summary>
        private static void CleanUpWalletConnnection(bool reconnect = false)
        {

            WalletPinDisabled = true;
            InCreateWallet = false;
            WalletAmountInPending = string.Empty;
            TotalTransactionPendingOnReceive = 0;
            GC.SuppressFinalize(SeedNodeConnectorWallet);
            SeedNodeConnectorWallet = new ClassSeedNodeConnector();
            if (!reconnect)
            {
                GC.SuppressFinalize(WalletConnect);
                WalletConnect = new ClassWalletConnect(SeedNodeConnectorWallet);
            }
            if (_threadWalletKeepAlive != null && (_threadWalletKeepAlive.IsAlive || _threadWalletKeepAlive != null))
            {
                _threadWalletKeepAlive.Abort();
                GC.SuppressFinalize(_threadWalletKeepAlive);
            }

            if (_threadListenSeedNodeNetwork != null &&
                (_threadListenSeedNodeNetwork.IsAlive || _threadListenSeedNodeNetwork != null))
            {
                _threadListenSeedNodeNetwork.Abort();
                GC.SuppressFinalize(_threadListenSeedNodeNetwork);
            }
            ClassWalletTransactionAnonymityCache.ListTransaction.Clear();
            ClassWalletTransactionCache.ListTransaction.Clear();
            ClassBlockCache.ListBlock.Clear();
        }

        /// <summary>
        ///     Initialization of wallet object.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPassword"></param>
        /// <param name="walletKey"></param>
        /// <param name="phase"></param>
        private static void InitializeWalletObject(string walletAddress, string walletPassword, string walletKey,
            string phase)
        {
            WalletConnect.WalletAddress = walletAddress;
            WalletConnect.WalletPassword = walletPassword;
            WalletConnect.WalletKey = walletKey;
            WalletConnect.WalletPhase = phase;
        }

        /// <summary>
        ///     Listen seed node network.
        /// </summary>
        public static void ListenSeedNodeNetworkForWallet()
        {
            _threadListenSeedNodeNetwork = new Thread(async delegate()
            {
                var packetNone = 0;
                var packetError = 10;
                var packetNoneMax = 100;
                var packetErrorMax = 1;
                while (SeedNodeConnectorWallet.GetStatusConnectToSeed())
                {
                    var packetWallet = await WalletConnect.ListenPacketWalletAsync(Certificate, true);

                    if (packetWallet == ClassAlgoErrorEnumeration.AlgoError)
                    {
                        break;
                    }
                    if (packetWallet == ClassSeedNodeStatus.SeedNone)
                    {
                        packetNone++;
                    }
                    else
                    {
                        packetNone = 0;
                    }

                    if (packetWallet == ClassSeedNodeStatus.SeedError)
                    {
                        packetError++;
                    }
                    else
                    {
                        packetError = 0;
                    }

                    if (packetNone == packetNoneMax && !InCreateWallet) break;

                    if (packetError == packetErrorMax) break;


                    if (packetWallet.Contains("*")) // Character separator.
                    {
                        var splitPacket = packetWallet.Split(new[] {"*"}, StringSplitOptions.None);
                        foreach (var packetEach in splitPacket)
                        {
                            if (packetEach != null)
                            {
                                if (!string.IsNullOrEmpty(packetEach))
                                {
                                    if (packetEach.Length > 1)
                                    {
                                        if (packetEach == ClassAlgoErrorEnumeration.AlgoError)
                                        {
                                            break;
                                        }

                                        new Thread(async () =>
                                                await HandleWalletPacket(packetEach.Replace("*", "")))
                                            .Start();
#if DEBUG
                                        Log.WriteLine("Packet wallet received: " + packetEach.Replace("*", ""));
#endif
                                    }
                                }
                            }
                        }
                        GC.SuppressFinalize(splitPacket);
                        GC.SuppressFinalize(packetWallet);
                    }
                    else
                    {
                        if (packetWallet == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            break;
                        }

                        new Thread(async () => await HandleWalletPacket(packetWallet)).Start();
#if DEBUG
                        Log.WriteLine("Packet wallet received: " + packetWallet);
#endif
                    }
                }

                new Thread(async() => await FullDisconnection(false)).Start();
            });
            _threadListenSeedNodeNetwork.Start();
        }

        /// <summary>
        ///     Enable keep alive packet for wallet.
        /// </summary>
        private static void EnableKeepAliveWallet()
        {
            _threadWalletKeepAlive = new Thread(async delegate()
            {
                Thread.Sleep(2000);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (!await SeedNodeConnectorWallet
                        .SendPacketToSeedNodeAsync(ClassWalletCommand.ClassWalletSendEnumeration.KeepAlive, Certificate,
                            false, true))
                    {
#if DEBUG
                        Log.WriteLine("Can't send keep alive packet to seed node.");
#endif
                        break;
                    }
                }
            });
            _threadWalletKeepAlive.Start();
        }

        /// <summary>
        ///     Handle packet wallet.
        /// </summary>
        /// <param name="packet"></param>
        private static async Task HandleWalletPacket(string packet)
        {
#if DEBUG
            Log.WriteLine("Handle packet wallet: " + packet);

#endif
            var splitPacket = packet.Split(new[] {"|"}, StringSplitOptions.None);

            if (splitPacket.Length <= 0)
            {
                return;
            }
            switch (splitPacket[0])
            {
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WaitingHandlePacket:
#if DEBUG
                    Log.WriteLine("Wallet network waiting phase received, showing Waiting Network Form.");
#endif
                    ClassParallelForm.ShowWaitingForm();
#if DEBUG
                    Log.WriteLine("Loading, please wait a little moment.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WaitingCreatePhase:
                    ClassParallelForm.HideWaitingForm();
                    ClassParallelForm.ShowWaitingCreateWalletForm();
#if DEBUG
                    Log.WriteLine("Waiting wallet creation finish..");
#endif

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletNewGenesisKey:
#if DEBUG
                    Log.WriteLine("New genesis key received: " + splitPacket[1]);
#endif
                    ClassConnectorSetting.NETWORK_GENESIS_KEY = splitPacket[1];
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedMoreCharacters:
                    ClassParallelForm.HideWaitingForm();
                    ClassParallelForm.HideWaitingCreateWalletForm();
                    new Thread(async () => await FullDisconnection(true)).Start();

#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedLetters:
                    ClassParallelForm.HideWaitingForm();
                    ClassParallelForm.HideWaitingCreateWalletForm();
                    new Thread(async () => await FullDisconnection(true)).Start();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.CreatePhase:
                    ClassParallelForm.HideWaitingForm();
                    ClassParallelForm.HideWaitingCreateWalletForm();

                    WalletDataCreation = Xiropht_Connector_All.Utils.ClassUtils.DecompressWallet(splitPacket[1]);

                    if (splitPacket[1] == "WRONG")
                    {
                        WalletNewPassword = string.Empty;
                        GC.SuppressFinalize(WalletDataCreation);
#if WINDOWS
                        MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                            "Your wallet password need to be stronger , if he is try again later.",
                            "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        new Thread(delegate ()
                        {
                            MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            "Your wallet password need to be stronger , if he is try again later.",
                            "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                        }).Start();

#endif
                        new Thread(async () => await FullDisconnection(true)).Start();
                    }
                    else
                    {
#if DEBUG
                        Log.WriteLine("Packet create wallet data: " + WalletDataCreation);
#endif
                        var decryptWalletDataCreation = ClassAlgo.GetDecryptedResult(
                                ClassAlgoEnumeration.Rijndael,
                                WalletDataCreation, WalletNewPassword, ClassWalletNetworkSetting.KeySize);


                        var splitWalletData = decryptWalletDataCreation.Split(new[] {"\n"}, StringSplitOptions.None);
                        var pin = splitPacket[2];
                        var publicKey = splitWalletData[2];
                        var privateKey = splitWalletData[3];

                        var walletDataToSave = splitWalletData[0] + "\n"; // Only wallet address
                        walletDataToSave += splitWalletData[2] + "\n"; // Only public key

                        var passwordEncrypted = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael,
                            WalletNewPassword, WalletNewPassword,
                            ClassWalletNetworkSetting.KeySize);
                        var walletDataToSaveEncrypted = ClassAlgo.GetEncryptedResult(
                                ClassAlgoEnumeration.Rijndael,
                                walletDataToSave, passwordEncrypted, ClassWalletNetworkSetting.KeySize);
                        TextWriter writerWallet = new StreamWriter(WalletDataCreationPath);

                        writerWallet.Write(walletDataToSaveEncrypted, false);
                        writerWallet.Close();


                        WalletDataCreation = string.Empty;
                        WalletDataCreationPath = string.Empty;
                        WalletDataPinCreation = string.Empty;
                        WalletNewPassword = string.Empty;
                        var key = publicKey;
                        var key1 = privateKey;
                        var pin1 = pin;
                        ClassFormPhase.WalletXiropht.BeginInvoke((MethodInvoker)delegate
                        {
                            var createWalletSuccessForm = new CreateWalletSuccessForm
                            {
                                PublicKey = key,
                                PrivateKey = key1,
                                PinCode = pin1,
                                StartPosition = FormStartPosition.CenterParent,
                                TopMost = false
                            };
                            createWalletSuccessForm.ShowDialog(ClassFormPhase.WalletXiropht);
                        });

                    }

                    break;

                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAskSuccess:
                    ClassParallelForm.HideWaitingForm();
                    ClassParallelForm.HideWaitingCreateWalletForm();

                    WalletDataCreation = Xiropht_Connector_All.Utils.ClassUtils.DecompressWallet(splitPacket[1]);

                    if (splitPacket[1] == "WRONG")
                    {
                        WalletNewPassword = string.Empty;
                        GC.SuppressFinalize(WalletDataCreation);
#if WINDOWS
                        MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                            "Your wallet password need to be stronger , if he is try again later.",
                            "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        new Thread(delegate ()
                        {
                            MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            "Your wallet password need to be stronger , if he is try again later.",
                            "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                        }).Start();

#endif
                        new Thread(async () => await FullDisconnection(true)).Start();
                    }
                    else
                    {
#if DEBUG
                        Log.WriteLine("Packet create wallet data: " + WalletDataCreation);
#endif
                        var decryptWalletDataCreation = ClassAlgo.GetDecryptedResult(
                                ClassAlgoEnumeration.Rijndael,
                                WalletDataCreation, WalletNewPassword, ClassWalletNetworkSetting.KeySize);


                        var splitWalletData = decryptWalletDataCreation.Split(new[] { "\n" }, StringSplitOptions.None);
                        var pin = splitPacket[2];
                        var publicKey = splitWalletData[2];
                        var privateKey = splitWalletData[3];

                        var walletDataToSave = splitWalletData[0] + "\n"; // Only wallet address
                        walletDataToSave += splitWalletData[2] + "\n"; // Only public key

                        var passwordEncrypted = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael,
                            WalletNewPassword, WalletNewPassword,
                            ClassWalletNetworkSetting.KeySize);
                        var walletDataToSaveEncrypted = ClassAlgo.GetEncryptedResult(
                                ClassAlgoEnumeration.Rijndael,
                                walletDataToSave, passwordEncrypted, ClassWalletNetworkSetting.KeySize);
                        TextWriter writerWallet = new StreamWriter(WalletDataCreationPath);

                        writerWallet.Write(walletDataToSaveEncrypted, false);
                        writerWallet.Close();


                        WalletDataCreation = string.Empty;
                        WalletDataCreationPath = string.Empty;
                        WalletDataPinCreation = string.Empty;
                        WalletNewPassword = string.Empty;
                        var key = publicKey;
                        var key1 = privateKey;
                        var pin1 = pin;
                        ClassFormPhase.WalletXiropht.BeginInvoke((MethodInvoker)delegate
                        {
                            var createWalletSuccessForm = new CreateWalletSuccessForm
                            {
                                PublicKey = key,
                                PrivateKey = key1,
                                PinCode = pin1,
                                StartPosition = FormStartPosition.CenterParent,
                                TopMost = false
                            };
                            createWalletSuccessForm.ShowDialog(ClassFormPhase.WalletXiropht);
                        });

                    }

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.RightPhase:
#if DEBUG
                    Log.WriteLine("Wallet accepted to connect on blockchain. Send wallet address for login..");
#endif
                    if (!await WalletConnect.SendPacketWallet(
                        ClassWalletCommand.ClassWalletSendEnumeration.LoginPhase + "|" + WalletConnect.WalletAddress,
                        Certificate, true))
                    {
                        new Thread(async () => await FullDisconnection(true)).Start();
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if DEBUG
                        Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                    }

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.PasswordPhase:
#if DEBUG
                    Log.WriteLine("Wallet accepted to login on the blockchain, submit password..");
#endif

                    EnableKeepAliveWallet();
                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Password);
                    if (!await WalletConnect.SendPacketWallet(
                        ClassWalletCommand.ClassWalletSendEnumeration.PasswordPhase + "|" +
                        WalletConnect.WalletPassword, Certificate, true))
                    {
                        new Thread(async () => await FullDisconnection(true)).Start();
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if DEBUG
                        Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                    }
                    else
                    {
                        WalletInReconnect = false;
                        if (ClassFormPhase.FormPhase != ClassFormPhaseEnumeration.Overview)
                        {
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
                            ClassFormPhase.ShowWalletMenu();
                        }
                    }

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.KeyPhase:
#if DEBUG
                    Log.WriteLine("Wallet password to login on the blockchain accepted, submit key..");
#endif

                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Key);
                    if (!await WalletConnect.SendPacketWallet(
                        ClassWalletCommand.ClassWalletSendEnumeration.KeyPhase + "|" + WalletConnect.WalletKey,
                        Certificate, true))
                    {
                        new Thread(async () => await FullDisconnection(true)).Start();
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if DEBUG
                        Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                    }

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.LoginAcceptedPhase:
#if DEBUG
                    Log.WriteLine("Wallet key to login on the blockchain accepted, login accepted successfully..");
#endif

                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Key);
                    WalletConnect.WalletId = splitPacket[1];
                    WalletConnect.WalletIdAnonymity = splitPacket[2];
#if DEBUG
                    Log.WriteLine("Wallet Anonymity id: " + WalletConnect.WalletIdAnonymity);
#endif

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.StatsPhase:
                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Accepted);
                    WalletConnect.WalletAmount = splitPacket[1];
                    if (splitPacket.Length > 2)
                    {
                        WalletAmountInPending = splitPacket[2];
                    }
                    new Thread(() => ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress, WalletConnect.WalletAmount)).Start();

#if DEBUG
                    Log.WriteLine("Actual Balance: " + WalletConnect.WalletAmount);
                    Log.WriteLine("Pending amount in pending to receive: " + WalletAmountInPending);

#endif
                    if (LastRemoteNodePacketReceived + 15 < DateTimeOffset.Now.ToUnixTimeSeconds() || !EnableReceivePacketRemoteNode && !OnWaitingRemoteNodeList)
                    {
                        LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                        await DisconnectWholeRemoteNodeSync(true, false);
                        EnableReceivePacketRemoteNode = false;
                        OnWaitingRemoteNodeList = true;
                        if (!await SeedNodeConnectorWallet
                            .SendPacketToSeedNodeAsync(
                                ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletAskRemoteNode, Certificate, false,
                                true))
                        {
                            new Thread(async () => await FullDisconnection(false)).Start();
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if DEBUG
                            Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                        }
                    }
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.PinPhase:
#if DEBUG
                    Log.WriteLine("Blockhain ask pin code.");
#endif
                    WalletPinDisabled = false;
                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Pin);
                    ClassParallelForm.ShowPinForm();
#if DEBUG
                    Log.WriteLine(
                        "The blockchain ask your pin code. You need to write it for continue to use your wallet:");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.PinAcceptedPhase:
                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Accepted);
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif
#if DEBUG
                    Log.WriteLine("Pin code accepted, the blockchain will ask your pin code every 15 minutes.");
#endif


                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.PinRefusedPhase:
                    WalletConnect.SelectWalletPhase(ClassWalletPhase.Pin);

#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                       MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    ClassParallelForm.ShowPinForm();

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendMessage:
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht, splitPacket[1], "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht, splitPacket[1], "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    break;

                case ClassWalletCommand.ClassWalletReceiveEnumeration.AmountNotValid:
                    ClassParallelForm.HideWaitingForm();

#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("Transaction refused. You try input an invalid amount.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.AmountInsufficient:
                    ClassParallelForm.HideWaitingForm();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                       MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("Transaction refused. Your amount is insufficient.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.FeeInsufficient:
                    ClassParallelForm.HideWaitingForm();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("Transaction refused. Your fee is insufficient.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTransactionBusy:
                    ClassParallelForm.HideWaitingForm();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif
#if DEBUG
                    Log.WriteLine("Transaction refused. The blockchain currently control your wallet balance health.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletReceiveTransactionBusy:
                    ClassParallelForm.HideWaitingForm();
                    var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(int.Parse(splitPacket[1]));
                    dateTime = dateTime.ToLocalTime();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("Transaction refused. Your fee is insufficient.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.TransactionAccepted:
                    ClassParallelForm.HideWaitingForm();
#if WINDOWS
                    new Thread(() => MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question)).Start();
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif
#if DEBUG
                    Log.WriteLine(
                        "Transaction accepted on the blockchain side, your history will be updated has soon has possible by public remote nodes or manual node if you have select manual nodes.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.AddressNotValid:
                    ClassParallelForm.HideWaitingForm();
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("The wallet address is not valid, please check it.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletBanPhase:
                    DisconnectWalletFromSeedNode(true);
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
#if DEBUG
                    Log.WriteLine("Your wallet is banned for approximatively one hour, try to reconnect later.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAlreadyConnected:
                    await FullDisconnection(true);
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif
#if DEBUG
                    Log.WriteLine("Your wallet is already connected, try to reconnect later.");
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordAccepted:

                    WalletConnect.WalletPassword = WalletNewPassword; // Update the network object for packet encryption.


                    var encryptedPassword =  ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael,
                        WalletNewPassword, WalletNewPassword, ClassWalletNetworkSetting.KeySize);
                    var encryptWalletDataSave =  ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael,
                            WalletDataDecrypted, encryptedPassword, ClassWalletNetworkSetting.KeySize); // AES

                    if (File.Exists(WalletLastPathFile))
                    {
                        File.Delete(WalletLastPathFile);
                        File.Create(WalletLastPathFile).Close();
                    }

                    WalletDataDecrypted = string.Empty;
                    var writerWalletNew = new StreamWriter(WalletLastPathFile);
                    writerWalletNew.Write(encryptWalletDataSave);
                    writerWalletNew.Flush();
                    writerWalletNew.Close();

                    WalletNewPassword = string.Empty;
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_TITLE_TEXT"),
                        MessageBoxButtons.OK, MessageBoxIcon.Question);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordRefused:
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    WalletNewPassword = string.Empty;
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeAccepted:
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    WalletPinDisabled = !WalletPinDisabled;

                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeRefused:
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();
#endif
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletWarningConnection:
#if WINDOWS
                    MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_CONTENT_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                    new Thread(delegate ()
                    {
                       MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                    }).Start();

#endif
                    break;

                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTotalPendingTransactionOnReceive:
                    if (int.TryParse(splitPacket[1], out var totalTransactionInPendingOnReceiveTmp))
                        TotalTransactionPendingOnReceive = totalTransactionInPendingOnReceiveTmp;
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTransactionData:

                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletSendRemoteNode:
                    if (!WalletOnUseSync)
                    {
                        LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                        OnWaitingRemoteNodeList = true;
                        try
                        {
                            bool noPublicNode = false;
                            if (!SettingManualRemoteNode)
                            {
                                if (WalletSyncMode == 1)
                                {
                                    packet += "|127.0.0.1";
                                    foreach (var remoteNodeObj in packet.Split(new[] { "|" }, StringSplitOptions.None))
                                    {
                                        if (remoteNodeObj != null)
                                        {
                                            if (!string.IsNullOrEmpty(remoteNodeObj))
                                            {
                                                if (remoteNodeObj != "WALLET-SEND-REMOTE-NODE")
                                                {
                                                    var remoteNode = remoteNodeObj.Replace("WALLET-SEND-REMOTE-NODE", "");

                                                    remoteNode = remoteNode.Replace("|", "");
                                                    if (remoteNode != "NONE")
                                                    {
                                                        if (!ListRemoteNodeBanned.Contains(remoteNode))
                                                        {
                                                            if (!ClassRemoteNodeChecker.CheckRemoteNodeHostExist(remoteNode))
                                                            {
                                                                ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(
                                                                    "Start to check remote node host: " + remoteNode);
#if DEBUG
                                                                Log.WriteLine("Start to check remote node host: " + remoteNode);
#endif
                                                                switch (await ClassRemoteNodeChecker.CheckNewRemoteNodeHostAsync(remoteNode))
                                                                {
                                                                    case ClassRemoteNodeStatus.StatusAlive:
                                                                        ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(
                                                                            "Remote node host: " + remoteNode +
                                                                            " is alive and already exist on the list.");
#if DEBUG
                                                                        Log.WriteLine(
                                                                            "Remote node host: " + remoteNode +
                                                                            " is alive and already exist on the list.");
#endif
                                                                        break;
                                                                    case ClassRemoteNodeStatus.StatusNew:
                                                                        ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(
                                                                            "Remote node host: " + remoteNode +
                                                                            " is alive and included on the list.");
#if DEBUG
                                                                        Log.WriteLine(
                                                                            "Remote node host: " + remoteNode +
                                                                            " is alive and included on the list.");
#endif
                                                                        break;
                                                                    case ClassRemoteNodeStatus.StatusDead:
                                                                        ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(
                                                                            "Remote node host: " + remoteNode + " is dead.");
#if DEBUG
                                                                        Log.WriteLine("Remote node host: " + remoteNode + " is dead.");
#endif
                                                                        ListRemoteNodeBanned.Add(remoteNode);
                                                                        break;
                                                                }
                                                            }
#if DEBUG
                                                            else
                                                            {
                                                                Log.WriteLine("Remote node host: " + remoteNode + " already exist.");
                                                            }
#endif
                                                        }
#if DEBUG
                                                        else
                                                        {
                                                            Log.WriteLine("Remote node host: " + remoteNode + " is banned.");
                                                        }
#endif
                                                    }
                                                    else
                                                    {
                                                        noPublicNode = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                for (var i = 0; i < ClassConnectorSetting.SeedNodeIp.Count; i++)
                                    if (i < ClassConnectorSetting.SeedNodeIp.Count)
                                        ClassRemoteNodeChecker.ListRemoteNodeChecked.Add(
                                            new Tuple<string, int>(ClassConnectorSetting.SeedNodeIp[i], 30));


                                if (ListWalletConnectToRemoteNode == null)
                                {
                                    ListWalletConnectToRemoteNode = new List<ClassWalletConnectToRemoteNode>();
                                }

                                if (ClassRemoteNodeChecker.ListRemoteNodeChecked.Count > 0)
                                {
                                    if (!EnableReceivePacketRemoteNode)
                                    {
                                        ListWalletConnectToRemoteNode.Clear();
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectTransaction));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectSupply));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectCirculating));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectFee));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectBlockMined));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectDifficulty));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectRate));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectPendingTransaction));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectAskWalletTransaction));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectAskBlock));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectAskLastBlockFound));
                                        ListWalletConnectToRemoteNode.Add(
                                            new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                                                .ObjectAskWalletAnonymityTransaction));

                                        if (WalletSyncMode == 0 || noPublicNode) // Seed node sync.
                                        {
                                            var randomSeedNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            if (!await ListWalletConnectToRemoteNode[0]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[1]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[2]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[3]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[4]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[5]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[6]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[7]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[8]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[9]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[10]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[11]
                                                .ConnectToRemoteNodeAsync(
                                                    ClassRemoteNodeChecker.ListRemoteNodeChecked[randomSeedNode].Item1,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }
                                        }
                                        else if (WalletSyncMode == 1) // Public remote node sync.
                                        {
                                            ClassRemoteNodeChecker.ListRemoteNodeChecked = ClassRemoteNodeChecker
                                                .ListRemoteNodeChecked.Distinct().ToList();

                                            string previousNode;
                                            var randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[0]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[1]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[2]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }
                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[3]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[4]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[5]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[6]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[7]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            // Take the same remote node host who give the number of transactions owned by the wallet, for be sure to sync at the accurate number of transactions.
                                            if (!await ListWalletConnectToRemoteNode[8]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            // Take the same remote node host who give the number of blocks mined, for be sure to sync at the accurate number of blocks.
                                            if (!await ListWalletConnectToRemoteNode[9]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }


                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[10]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            randomNode = Xiropht_Connector_All.Utils.ClassUtils.GetRandomBetween(0,
                                                ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                            previousNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode]
                                                .Item1;
                                            if (!await ListWalletConnectToRemoteNode[11]
                                                .ConnectToRemoteNodeAsync(previousNode,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1))
                                                {
                                                    ListRemoteNodeBanned.Add(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNode].Item1);
                                                }
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }
                                        }
                                        else if (WalletSyncMode == 2) // Manual sync mode
                                        {
                                            if (!await ListWalletConnectToRemoteNode[0]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[1]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[2]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[3]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[4]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[5]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[6]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[7]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[8]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[9]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[10]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }

                                            if (!await ListWalletConnectToRemoteNode[11]
                                                .ConnectToRemoteNodeAsync(WalletSyncHostname,
                                                    ClassConnectorSetting.RemoteNodePort, Program.IsLinux))
                                            {
                                                await DisconnectWholeRemoteNodeSync(true, true);
                                                return;
                                            }
                                        }

                                        WalletCheckMaxSupply = 1;
                                        WalletCheckCoinCirculating = 1;
                                        WalletCheckTotalTransactionFee = 1;
                                        WalletCheckTotalBlockMined = 1;
                                        WalletCheckNetworkHashrate = 1;
                                        WalletCheckNetworkDifficulty = 1;
                                        WalletCheckTotalPendingTransaction = 1;
                                        WalletCheckBlockPerId = 1;
                                        EnableReceivePacketRemoteNode = true;
                                        WalletOnUseSync = true;
#if DEBUG
                                        Log.WriteLine("Enable receive packet remote node list.");
#endif
                                        ListenRemoteNodeNetwork();
#if DEBUG
                                        Log.WriteLine("Enable send packet remote node list.");
#endif
                                        SendRemoteNodeNetwork();

#if DEBUG
                                        Log.WriteLine("Enable check packet remote node list.");
#endif

                                        CheckRemoteNodeNetwork();
                                        var remoteNodeMessageSync = "Wallet sync with remote node:";
                                        var tmplistNodeSync = new List<string>();
                                        for (var i = 0; i < ListWalletConnectToRemoteNode.Count - 1; i++)
                                            if (!tmplistNodeSync.Contains(ListWalletConnectToRemoteNode[i]
                                                .RemoteNodeHost))
                                            {
                                                tmplistNodeSync.Add(ListWalletConnectToRemoteNode[i].RemoteNodeHost);
                                                remoteNodeMessageSync +=
                                                    " " + ListWalletConnectToRemoteNode[i].RemoteNodeHost;
                                            }

                                        ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation(remoteNodeMessageSync);
                                    }
                                }
                                else
                                {
#if DEBUG
                                    Log.WriteLine("No public remote node available.");
#endif
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            WalletOnUseSync = false;
                            OnWaitingRemoteNodeList = false;
#if DEBUG
                            Log.WriteLine("Exception error on connect to public remote node: " + error.Message);
#endif
                            new Thread(async () => await DisconnectWholeRemoteNodeSync(true, true)).Start();
                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultMaxSupply:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckMaxSupply = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[1].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for coin max supply provided by remote node host: " + ListWalletConnectToRemoteNode[1].RemoteNodeHost);
#endif
                            if (ListWalletConnectToRemoteNode[1].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[1].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[1].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about coin max supply provided by host: " + ListWalletConnectToRemoteNode[1].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        WalletCheckMaxSupply = 1; // Good
                        try
                        {
                            ListWalletConnectToRemoteNode[1].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("Good remote node information for coin max supply provided by remote node host: " + ListWalletConnectToRemoteNode[1].RemoteNodeHost);

#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultCoinCirculating:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckCoinCirculating = -1; // Bad

                        try
                        {
                            ListWalletConnectToRemoteNode[2].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for coin circulating provided by remote node host: " + ListWalletConnectToRemoteNode[2].RemoteNodeHost);
#endif
                            if (ListWalletConnectToRemoteNode[2].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[2].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[2].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about coin circulating provided by host: " + ListWalletConnectToRemoteNode[2].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckCoinCirculating = 1; // Good

                            ListWalletConnectToRemoteNode[2].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for coin circulating provided by remote node host: " + ListWalletConnectToRemoteNode[2].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultNetworkDifficulty:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckNetworkDifficulty = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[5].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for network difficulty provided by remote node host: " + ListWalletConnectToRemoteNode[5].RemoteNodeHost);
#endif
                            if (ListWalletConnectToRemoteNode[5].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[5].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[5].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about network difficulty provided by host: " + ListWalletConnectToRemoteNode[5].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckNetworkDifficulty = 1; // Good
                            ListWalletConnectToRemoteNode[5].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for network difficulty provided by remote node host: " + ListWalletConnectToRemoteNode[5].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultNetworkHashrate:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckNetworkHashrate = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[6].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for network hashrate provided by remote node host: " + ListWalletConnectToRemoteNode[6].RemoteNodeHost);
#endif
                            if (ListWalletConnectToRemoteNode[6].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[6].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[6].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about network hashrate provided by host: " + ListWalletConnectToRemoteNode[6].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckNetworkHashrate = 1; // Good
                            ListWalletConnectToRemoteNode[6].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for network hashrate provided by remote node host: " + ListWalletConnectToRemoteNode[6].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultTotalBlockMined:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckTotalBlockMined = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[4].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for total block mined provided by remote node host: " + ListWalletConnectToRemoteNode[4].RemoteNodeHost);
#endif
                            if (ListWalletConnectToRemoteNode[4].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[4].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[4].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about total block mined provided by host: " + ListWalletConnectToRemoteNode[4].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckTotalBlockMined = 1; // Good
                            ListWalletConnectToRemoteNode[4].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for total block mined provided by remote node host: " + ListWalletConnectToRemoteNode[4].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultTotalTransactionFee:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckTotalTransactionFee = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[3].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for total transaction fee provided by remote node host: " + ListWalletConnectToRemoteNode[3].RemoteNodeHost);
#endif
                            
                            if (ListWalletConnectToRemoteNode[3].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[3].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[3].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about total transaction fee provided by host: " + ListWalletConnectToRemoteNode[3].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckTotalTransactionFee = 1; // Good
                            ListWalletConnectToRemoteNode[3].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for total transaction fee provided by remote node host: " + ListWalletConnectToRemoteNode[3].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultTotalPendingTransaction:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckTotalPendingTransaction = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[7].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for total pending transaction provided by remote node host: " + ListWalletConnectToRemoteNode[7].RemoteNodeHost);
#endif

                            if (ListWalletConnectToRemoteNode[7].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[7].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[7].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about total pending transaction provided by host: " + ListWalletConnectToRemoteNode[7].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckTotalPendingTransaction = 1; // Good
                            ListWalletConnectToRemoteNode[7].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for total pending transaction provided by remote node host: " + ListWalletConnectToRemoteNode[7].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
                case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletResultBlockPerId:
                    if (splitPacket[1] != "1")
                    {
                        WalletCheckBlockPerId = -1; // Bad
                        try
                        {
                            ListWalletConnectToRemoteNode[9].TotalInvalidPacket++;
#if DEBUG
                            Log.WriteLine("Bad remote node information for block per id asked provided by remote node host: " + ListWalletConnectToRemoteNode[9].RemoteNodeHost);
#endif

                            if (ListWalletConnectToRemoteNode[9].TotalInvalidPacket >= ClassConnectorSetting.MaxRemoteNodeInvalidPacket)
                            {
                                if (!ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[9].RemoteNodeHost))
                                {
                                    ListRemoteNodeBanned.Add(ListWalletConnectToRemoteNode[9].RemoteNodeHost);

#if DEBUG
                                    Log.WriteLine("remote node banned for too much bad information about block per id asked provided by host: " + ListWalletConnectToRemoteNode[9].RemoteNodeHost);
#endif
                                }
                                await DisconnectWholeRemoteNodeSync(true, true);
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            WalletCheckBlockPerId = 1; // Good
                            ListWalletConnectToRemoteNode[9].TotalInvalidPacket = 0;
#if DEBUG
                            Log.WriteLine("good remote node information for block per id asked by remote node host: " + ListWalletConnectToRemoteNode[9].RemoteNodeHost);
#endif
                        }
                        catch
                        {

                        }
                    }
                    break;
            }
        }

        /// <summary>
        ///     Send a packet to the seed node network.
        /// </summary>
        /// <param name="packet"></param>
        public static async Task<bool> SendPacketWalletToSeedNodeNetwork(string packet)
        {

            if (!await WalletConnect.SendPacketWallet(packet, Certificate, true))
            {
                ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                   ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_SEND_PACKET_TEXT"));
#else
                new Thread(delegate() 
                {
                    MethodInvoker invoke = () => MessageBox.Show(ClassFormPhase.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_SEND_PACKET_TEXT"));
                    ClassFormPhase.WalletXiropht.BeginInvoke(invoke);
                }).Start();

#endif
#if DEBUG
                Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                return false;
            }

            return true;
        }

#endregion

        #region Remote node Sync

        public static void CheckRemoteNodeNetwork()
        {
            if (_threadCheckRemoteNodePacketNetwork != null &&
                (_threadCheckRemoteNodePacketNetwork.IsAlive || _threadCheckRemoteNodePacketNetwork != null))
            {
                _threadCheckRemoteNodePacketNetwork.Abort();
                GC.SuppressFinalize(_threadCheckRemoteNodePacketNetwork);
            }
            _threadCheckRemoteNodePacketNetwork = new Thread(delegate ()
            {
                var dead = false;
                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                while (EnableReceivePacketRemoteNode)
                {
                    try
                    {
                        for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                            if (i < ListWalletConnectToRemoteNode.Count)
                                if (ListWalletConnectToRemoteNode[i] != null)
                                {
                                    if (!ListWalletConnectToRemoteNode[i].RemoteNodeStatus)
                                    {
#if DEBUG
                                        Log.WriteLine("Remote node " + ListWalletConnectToRemoteNode[i].RemoteNodeHost + " id:" + i + " connection dead or stuck.");
#endif
                                        dead = true;
                                        break;
                                    }
#if LINUX
                                    if (!ListWalletConnectToRemoteNode[i].CheckRemoteNode())
                                    {
#if DEBUG
                                        Log.WriteLine("Remote node " + ListWalletConnectToRemoteNode[i].RemoteNodeHost + " id:" + i + " connection dead or stuck.");
#endif
                                        dead = true;
                                        break;
                                    }
#endif
                                }
                    }
                    catch
                    {
                        dead = true;
                        break;
                    }
                    if (LastRemoteNodePacketReceived + 10 < DateTimeOffset.Now.ToUnixTimeSeconds() || !EnableReceivePacketRemoteNode || !WalletOnUseSync)
                    {
                        dead = true;
                        break;
                    }

                    if (dead) break;

                    Thread.Sleep(2000);
                }

                if (dead)
                {
#if DEBUG
                    Log.WriteLine("Remote node connection dead or stuck.");
#endif
                    new Thread(async () =>
                    {
                        await DisconnectWholeRemoteNodeSync(true, true);
                    }).Start();
                }
            });
            _threadCheckRemoteNodePacketNetwork.Start();
        }

        /// <summary>
        ///     Send each packet on each connected remote node.
        /// </summary>
        public static void SendRemoteNodeNetwork()
        {
            if (_threadSendRemoteNodePacketNetwork != null &&
                (_threadSendRemoteNodePacketNetwork.IsAlive || _threadSendRemoteNodePacketNetwork != null))
            {
                _threadSendRemoteNodePacketNetwork.Abort();
                GC.SuppressFinalize(_threadSendRemoteNodePacketNetwork);
            }

            if (!WalletOnSendingPacketRemoteNode)
            {
                WalletOnSendingPacketRemoteNode = true;
                _threadSendRemoteNodePacketNetwork = new Thread(async delegate ()
                {
                    while (EnableReceivePacketRemoteNode)
                    {
                        try
                        {
                            for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                            {
                                if (i < ListWalletConnectToRemoteNode.Count)
                                {
                                    if (ListWalletConnectToRemoteNode[i] != null)
                                    {
                                        if (i != 11)
                                        {
                                            switch (i)
                                            {
                                                case 1: // max supply
                                                    if (WalletCheckMaxSupply != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 2: // coin circulating
                                                    if (WalletCheckCoinCirculating != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 3: // total fee
                                                    if (WalletCheckTotalTransactionFee != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 4: // block mined
                                                    if (WalletCheckTotalBlockMined != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 5: // difficulty
                                                    if (WalletCheckNetworkDifficulty != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 6: // hashrate
                                                    if (WalletCheckNetworkHashrate != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 7: // total pending transaction
                                                    if (WalletCheckTotalPendingTransaction != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case 9: // block per id
                                                    if (WalletCheckBlockPerId != 0)
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                        {

                                                            EnableReceivePacketRemoteNode = false;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    if (!await ListWalletConnectToRemoteNode[i]
                                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                    {

                                                        EnableReceivePacketRemoteNode = false;
                                                        break;
                                                    }
                                                    break;
                                            }

                                        }
                                        else
                                        {
                                            if (!await ListWalletConnectToRemoteNode[i]
                                                .SendPacketTypeRemoteNode(WalletConnect.WalletIdAnonymity))
                                            {
                                                EnableReceivePacketRemoteNode = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                Thread.Sleep(200);
                            }
                            Thread.Sleep(5000);
                        }
                        catch
                        {
                            break;
                        }
                       
                    }
                    WalletOnSendingPacketRemoteNode = false;
                });
                _threadSendRemoteNodePacketNetwork.Start();
            }
        }

        /// <summary>
        ///     Listen each remote node from the list.
        /// </summary>
        public static void ListenRemoteNodeNetwork()
        {

            _threadListenRemoteNodeNetwork1 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                    {
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[0].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                       
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                    }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync total number of transaction owned by the wallet.
            _threadListenRemoteNodeNetwork1.Start();
            _threadListenRemoteNodeNetwork2 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[1].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync max supply.
            _threadListenRemoteNodeNetwork2.Start();
            _threadListenRemoteNodeNetwork3 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[2].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync coin circulating of the network.
            _threadListenRemoteNodeNetwork3.Start();
            _threadListenRemoteNodeNetwork4 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[3].ListenRemoteNodeNetworkAsync();

                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync total fee in the network.
            _threadListenRemoteNodeNetwork4.Start();
            _threadListenRemoteNodeNetwork5 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[4].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync total block mined.
            _threadListenRemoteNodeNetwork5.Start();
            _threadListenRemoteNodeNetwork6 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[5].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync current mining difficulty.
            _threadListenRemoteNodeNetwork6.Start();
            _threadListenRemoteNodeNetwork7 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[6].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                        
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync current mining hashrate.
            _threadListenRemoteNodeNetwork7.Start();
            _threadListenRemoteNodeNetwork8 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[7].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

                                                     
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // to sync total pending transaction of the network.
            _threadListenRemoteNodeNetwork8.Start();
            _threadListenRemoteNodeNetwork9 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[8].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

                                                       
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // For sync transactions of the wallet.
            _threadListenRemoteNodeNetwork9.Start();
            _threadListenRemoteNodeNetwork10 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[9].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

                                                       
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);

                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);

                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // For sync blocks mined.
            _threadListenRemoteNodeNetwork10.Start();
            _threadListenRemoteNodeNetwork11 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[10].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);
                                                        
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // For sync last block found timestamp of the network.
            _threadListenRemoteNodeNetwork11.Start();
            _threadListenRemoteNodeNetwork12 = new Thread(async delegate()
            {
                try
                {
                    while (WalletOnUseSync)
                        try
                        {
                            var packet = await ListWalletConnectToRemoteNode[11].ListenRemoteNodeNetworkAsync();
                            if (packet == ClassWalletConnectToRemoteNodeObjectError.ObjectError) break;

                            if (packet != ClassWalletConnectToRemoteNodeObjectError.ObjectNone)
                            {
                                if (packet.Contains("*"))
                                {
                                    var splitPacket = packet.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacket.Length > 1)
                                    {
                                        for (int i = 0; i < splitPacket.Length; i++)
                                        {
                                            if (i < splitPacket.Length)
                                            {
                                                if (splitPacket[i] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(splitPacket[i]))
                                                    {
                                                        string packetHandle = splitPacket[i];
                                                        await Task.Factory
                                                            .StartNew(() => HandlePacketRemoteNodeAsync(packetHandle),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                                                          ;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet.Replace("*", "")),
                                            CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                            TaskScheduler.Current).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketRemoteNodeAsync(packet),
                                        CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                        TaskScheduler.Current).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                            break;
                        }
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }); // For sync last block found timestamp of the network.
            _threadListenRemoteNodeNetwork12.Start();
        }

        /// <summary>
        ///     Handle packet from remote node.
        /// </summary>
        /// <param name="packet"></param>
        public static async Task HandlePacketRemoteNodeAsync(string packet)
        {

            if (!string.IsNullOrEmpty(packet) && packet != null)
            {
                var splitPacket = packet.Split(new[] { "|" }, StringSplitOptions.None);

                if (splitPacket.Length > 0)
                {
#if DEBUG
                    Log.WriteLine("Packet of sync received: " + packet);
#endif
                    switch (splitPacket[0])
                    {
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinMaxSupply:
                            try
                            {
                                if (WalletSyncMode == 1)
                                {
                                    if (WalletCheckMaxSupply != 0)
                                    {
                                        if ((ListWalletConnectToRemoteNode[1].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[1].RemoteNodeHost) || splitPacket[1] != CoinMaxSupply))
                                        {
                                            WalletCheckMaxSupply = 0;

                                            await Task.Delay(100);
                                            if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckMaxSupply + "|" + splitPacket[1] +"|"+ ListWalletConnectToRemoteNode[1].RemoteNodeHost, Certificate, false, true))
                                            {
                                                var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                while (WalletCheckMaxSupply == 0)
                                                {
                                                    if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                    {
                                                        WalletCheckMaxSupply = -1;
                                                        break;
                                                    }
#if DEBUG
                                                    Log.WriteLine("Waiting check coin max supply response..");
#endif
                                                    await Task.Delay(100);
                                                }
                                                if (WalletCheckMaxSupply == 1)
                                                {
#if DEBUG
                                                    Log.WriteLine("Coin max supply information is good.");
#endif
                                                    LastRemoteNodePacketReceived =
                                                        DateTimeOffset.Now.ToUnixTimeSeconds();
                                                    ListWalletConnectToRemoteNode[1].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                    CoinMaxSupply = splitPacket[1]
                                                        .Replace(
                                                            ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinMaxSupply,
                                                            "");
                                                }
#if DEBUG
                                                else
                                                {
                                                    Log.WriteLine("Coin max supply information is bad.");
                                                }
#endif
                                            }
                                        }
                                        else
                                        {
                                            LastRemoteNodePacketReceived =
                                                DateTimeOffset.Now.ToUnixTimeSeconds();
                                            CoinMaxSupply = splitPacket[1]
                                                .Replace(
                                                    ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinMaxSupply,
                                                    "");
                                        }
                                    }
                                }
                                else
                                {
                                    LastRemoteNodePacketReceived =
                                        DateTimeOffset.Now.ToUnixTimeSeconds();
                                    CoinMaxSupply = splitPacket[1]
                                        .Replace(
                                            ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinMaxSupply,
                                            "");
                                }
                            }
                            catch
                            {

                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinCirculating:
                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckCoinCirculating != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[2].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[2].RemoteNodeHost)) || CoinCirculating != splitPacket[1])
                                    {
                                        WalletCheckCoinCirculating = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckCoinCirculating + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[2].RemoteNodeHost, Certificate, false, true))
                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckCoinCirculating == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckCoinCirculating = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckCoinCirculating == 1)
                                            {
                                                LastRemoteNodePacketReceived =
                                                DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[2].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                CoinCirculating = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeCoinCirculating, "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived =
                                            DateTimeOffset.Now.ToUnixTimeSeconds();
                                        CoinCirculating = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeCoinCirculating, "");
                                    }
                                }
                            }
                            else
                            {
                                LastRemoteNodePacketReceived =
                                    DateTimeOffset.Now.ToUnixTimeSeconds();
                                CoinCirculating = splitPacket[1]
                                    .Replace(
                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                            .SendRemoteNodeCoinCirculating, "");
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentDifficulty:
                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckNetworkDifficulty != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[5].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[5].RemoteNodeHost)) || NetworkDifficulty != splitPacket[1])
                                    {
                                        WalletCheckNetworkDifficulty = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckNetworkDifficulty + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[5].RemoteNodeHost, Certificate, false, true))

                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckNetworkDifficulty == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckNetworkDifficulty = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckNetworkDifficulty == 1)
                                            {
                                                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[5].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                NetworkDifficulty = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeCurrentDifficulty, "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                        NetworkDifficulty = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeCurrentDifficulty, "");
                                    }
                                }
                            }
                            else
                            {
                                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                NetworkDifficulty = splitPacket[1]
                                    .Replace(
                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                            .SendRemoteNodeCurrentDifficulty, "");
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate:
                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckNetworkHashrate != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[6].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[6].RemoteNodeHost)) || NetworkHashrate != splitPacket[1])
                                    {
                                        WalletCheckNetworkHashrate = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckNetworkHashrate + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[6].RemoteNodeHost, Certificate, false, true))

                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckNetworkHashrate == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckNetworkHashrate = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckNetworkHashrate == 1)
                                            {
                                                LastRemoteNodePacketReceived =
                                        DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[6].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                NetworkHashrate = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate,
                                                        "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived =
                                            DateTimeOffset.Now.ToUnixTimeSeconds();
                                        NetworkHashrate = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate,
                                                "");
                                    }
                                }
                            }
                            else
                            {
                                LastRemoteNodePacketReceived =
                                    DateTimeOffset.Now.ToUnixTimeSeconds();
                                NetworkHashrate = splitPacket[1]
                                    .Replace(
                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate,
                                        "");
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalBlockMined:

                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckTotalBlockMined != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[4].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[4].RemoteNodeHost)) || TotalBlockMined != splitPacket[1])
                                    {
                                        WalletCheckTotalBlockMined = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalBlockMined + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[4].RemoteNodeHost, Certificate, false, true))
                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckTotalBlockMined == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckTotalBlockMined = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckTotalBlockMined == 1)
                                            {
                                                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[4].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                TotalBlockMined = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeTotalBlockMined, "");

                                                if (!ClassFormPhase.WalletXiropht.EnableUpdateBlockWallet)
                                                    ClassFormPhase.WalletXiropht.StartUpdateBlockSync();

                                                if (_lastBlockReceived + 30 <= DateTimeOffset.Now.ToUnixTimeSeconds()) InSyncBlock = false;

                                                if (!InSyncBlock)
                                                {
                                                    var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                                    if (!tryParseBlockMined) return;

                                                    var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                                    if (totalBlockInWallet < totalBlockOfNetwork)
                                                    {
                                                        if (_threadWalletSyncBlock != null &&
                                                            (_threadWalletSyncBlock.IsAlive || _threadWalletSyncBlock != null))
                                                            _threadWalletSyncBlock.Abort();

                                                        try
                                                        {
                                                            _threadWalletSyncBlock = new Thread(async delegate ()
                                                            {
#if DEBUG
                                                                Log.WriteLine("Their is " + splitPacket[1] + " block mined to sync.");
#endif

                                                                TotalBlockInSync = totalBlockOfNetwork;


#if DEBUG
                                                                Log.WriteLine("Total block synced: " + totalBlockInWallet + "/" + TotalBlockInSync +
                                                                                                  " .");
#endif
                                                                if (totalBlockInWallet < totalBlockOfNetwork)
                                                                {
                                                                    InSyncBlock = true;
                                                                    for (var i = totalBlockInWallet; i < totalBlockOfNetwork; i++)
                                                                    {
                                                                        InReceiveBlock = true;
#if DEBUG
                                                                        Log.WriteLine("Ask block id: " + i);
#endif
                                                                        try
                                                                        {
                                                                            if (!await ListWalletConnectToRemoteNode[9]
                                                                                    .SendPacketRemoteNodeAsync(
                                                                                        ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration
                                                                                            .AskBlockPerId + "|" + WalletConnect.WalletId + "|" + i
                                                                                        ))
                                                                            {
                                                                                LastRemoteNodePacketReceived = 0;
                                                                                InSyncBlock = false;
                                                                                InReceiveBlock = false;
#if DEBUG
                                                                                Log.WriteLine("Can't sync block wallet.");
#endif
                                                                                break;
                                                                            }

                                                                        }
                                                                        catch
                                                                        {
                                                                            InSyncBlock = false;
                                                                            InReceiveBlock = false;
                                                                            break;
                                                                        }
                                                                        while (InReceiveBlock)
                                                                        {
                                                                            if (!InSyncBlock) break;

                                                                            Thread.Sleep(100);
                                                                        }
                                                                    }
                                                                }

                                                                InSyncBlock = false;
                                                                InReceiveBlock = false;
                                                            });
                                                            _threadWalletSyncBlock.Start();
                                                        }
                                                        catch (Exception error)
                                                        {
#if DEBUG
                                                            Log.WriteLine("Exception error on sync block: " + error.Message);
#endif
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                        {
                                                            if (totalBlockInWallet - 1 > totalBlockMined)
                                                            {
                                                                ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(false);
                                                                ClassBlockCache.RemoveWalletBlockCache();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();

                                        TotalBlockMined = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeTotalBlockMined, "");

                                        if (!ClassFormPhase.WalletXiropht.EnableUpdateBlockWallet)
                                            ClassFormPhase.WalletXiropht.StartUpdateBlockSync();

                                        if (_lastBlockReceived + 30 <= DateTimeOffset.Now.ToUnixTimeSeconds()) InSyncBlock = false;

                                        if (!InSyncBlock)
                                        {
                                            var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                            if (!tryParseBlockMined) return;

                                            var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                            if (totalBlockInWallet < totalBlockOfNetwork)
                                            {
                                                if (_threadWalletSyncBlock != null &&
                                                    (_threadWalletSyncBlock.IsAlive || _threadWalletSyncBlock != null))
                                                    _threadWalletSyncBlock.Abort();

                                                try
                                                {
                                                    _threadWalletSyncBlock = new Thread(async delegate ()
                                                    {
#if DEBUG
                                                        Log.WriteLine("Their is " + splitPacket[1] + " block mined to sync.");
#endif

                                                        TotalBlockInSync = totalBlockOfNetwork;


#if DEBUG
                                                        Log.WriteLine("Total block synced: " + totalBlockInWallet + "/" + TotalBlockInSync +
                                                                                          " .");
#endif
                                                        if (totalBlockInWallet < totalBlockOfNetwork)
                                                        {
                                                            InSyncBlock = true;
                                                            for (var i = totalBlockInWallet; i < totalBlockOfNetwork; i++)
                                                            {
                                                                InReceiveBlock = true;
#if DEBUG
                                                                Log.WriteLine("Ask block id: " + i);
#endif
                                                                try
                                                                {
                                                                    if (!await ListWalletConnectToRemoteNode[9]
                                                                            .SendPacketRemoteNodeAsync(
                                                                                ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration
                                                                                    .AskBlockPerId + "|" + WalletConnect.WalletId + "|" + i
                                                                                ))
                                                                    {
                                                                        LastRemoteNodePacketReceived = 0;
                                                                        InSyncBlock = false;
                                                                        InReceiveBlock = false;
#if DEBUG
                                                                        Log.WriteLine("Can't sync block wallet.");
#endif
                                                                        break;
                                                                    }

                                                                }
                                                                catch
                                                                {
                                                                    InSyncBlock = false;
                                                                    InReceiveBlock = false;
                                                                    break;
                                                                }
                                                                while (InReceiveBlock)
                                                                {
                                                                    if (!InSyncBlock) break;

                                                                    Thread.Sleep(100);
                                                                }
                                                            }
                                                        }

                                                        InSyncBlock = false;
                                                        InReceiveBlock = false;
                                                    });
                                                    _threadWalletSyncBlock.Start();
                                                }
                                                catch (Exception error)
                                                {
#if DEBUG
                                                    Log.WriteLine("Exception error on sync block: " + error.Message);
#endif
                                                }
                                            }
                                            else
                                            {
                                                if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                {
                                                    if (totalBlockInWallet - 1 > totalBlockMined)
                                                    {
                                                        ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(false);
                                                        ClassBlockCache.RemoveWalletBlockCache();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                TotalBlockMined = splitPacket[1]
                                    .Replace(
                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                            .SendRemoteNodeTotalBlockMined, "");

                                if (!ClassFormPhase.WalletXiropht.EnableUpdateBlockWallet)
                                    ClassFormPhase.WalletXiropht.StartUpdateBlockSync();

                                if (_lastBlockReceived + 30 <= DateTimeOffset.Now.ToUnixTimeSeconds()) InSyncBlock = false;

                                if (!InSyncBlock)
                                {
                                    var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                    if (!tryParseBlockMined) return;

                                    var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                    if (totalBlockInWallet < totalBlockOfNetwork)
                                    {
                                        if (_threadWalletSyncBlock != null &&
                                            (_threadWalletSyncBlock.IsAlive || _threadWalletSyncBlock != null))
                                            _threadWalletSyncBlock.Abort();

                                        try
                                        {
                                            _threadWalletSyncBlock = new Thread(async delegate ()
                                            {
#if DEBUG
                                                Log.WriteLine("Their is " + splitPacket[1] + " block mined to sync.");
#endif

                                                TotalBlockInSync = totalBlockOfNetwork;


#if DEBUG
                                                Log.WriteLine("Total block synced: " + totalBlockInWallet + "/" + TotalBlockInSync +
                                                                                  " .");
#endif
                                                if (totalBlockInWallet < totalBlockOfNetwork)
                                                {
                                                    InSyncBlock = true;
                                                    for (var i = totalBlockInWallet; i < totalBlockOfNetwork; i++)
                                                    {
                                                        InReceiveBlock = true;
#if DEBUG
                                                        Log.WriteLine("Ask block id: " + i);
#endif
                                                        try
                                                        {
                                                            if (!await ListWalletConnectToRemoteNode[9]
                                                                    .SendPacketRemoteNodeAsync(
                                                                        ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration
                                                                            .AskBlockPerId + "|" + WalletConnect.WalletId + "|" + i
                                                                        ))
                                                            {
                                                                LastRemoteNodePacketReceived = 0;
                                                                InSyncBlock = false;
                                                                InReceiveBlock = false;
#if DEBUG
                                                                Log.WriteLine("Can't sync block wallet.");
#endif
                                                                break;
                                                            }

                                                        }
                                                        catch
                                                        {
                                                            InSyncBlock = false;
                                                            InReceiveBlock = false;
                                                            break;
                                                        }
                                                        while (InReceiveBlock)
                                                        {
                                                            if (!InSyncBlock) break;

                                                            Thread.Sleep(100);
                                                        }
                                                    }
                                                }

                                                InSyncBlock = false;
                                                InReceiveBlock = false;
                                            });
                                            _threadWalletSyncBlock.Start();
                                        }
                                        catch (Exception error)
                                        {
#if DEBUG
                                            Log.WriteLine("Exception error on sync block: " + error.Message);
#endif
                                        }
                                    }
                                    else
                                    {
                                        if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                        {
                                            if (totalBlockInWallet - 1 > totalBlockMined)
                                            {
                                                ClassFormPhase.WalletXiropht.StopUpdateBlockHistory(false);
                                                ClassBlockCache.RemoveWalletBlockCache();
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee:
                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckTotalTransactionFee != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[3].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[3].RemoteNodeHost)) || TotalFee != splitPacket[1])
                                    {
                                        WalletCheckTotalTransactionFee = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalTransactionFee + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[3].RemoteNodeHost, Certificate, false, true))
                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckTotalTransactionFee == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckTotalTransactionFee = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckTotalTransactionFee == 1)
                                            {
                                                LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[3].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                TotalFee = splitPacket[1]
                                                    .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                                        "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived =
                                            DateTimeOffset.Now.ToUnixTimeSeconds();
                                        TotalFee = splitPacket[1]
                                            .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                                "");
                                    }
                                }

                            }
                            else
                            {
                                LastRemoteNodePacketReceived =
                                    DateTimeOffset.Now.ToUnixTimeSeconds();
                                TotalFee = splitPacket[1]
                                    .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                        "");
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalPendingTransaction:
                            if (WalletSyncMode == 1)
                            {
                                if (WalletCheckTotalPendingTransaction != 0)
                                {
                                    if ((ListWalletConnectToRemoteNode[7].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[7].RemoteNodeHost)) || ""+RemoteNodeTotalPendingTransactionInNetwork != splitPacket[1])
                                    {
                                        WalletCheckTotalPendingTransaction = 0;
                                        await Task.Delay(100);
                                        if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalPendingTransaction + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[7].RemoteNodeHost, Certificate, false, true))

                                        {
                                            var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            while (WalletCheckTotalPendingTransaction == 0)
                                            {
                                                if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    WalletCheckTotalPendingTransaction = -1;
                                                    break;
                                                }
                                                await Task.Delay(100);
                                            }
                                            if (WalletCheckTotalPendingTransaction == 1)
                                            {
                                                LastRemoteNodePacketReceived =
                                                    DateTimeOffset.Now.ToUnixTimeSeconds();
                                                ListWalletConnectToRemoteNode[7].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                try
                                                {
                                                    RemoteNodeTotalPendingTransactionInNetwork = int.Parse(splitPacket[1]
                                                        .Replace(
                                                            ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                .SendRemoteNodeTotalPendingTransaction, ""));
                                                }
                                                catch (Exception error)
                                                {
#if DEBUG
                                                    Log.WriteLine("Exception error on receive Total Pending Transaction on sync: " + error.Message);
#endif
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived =
                                            DateTimeOffset.Now.ToUnixTimeSeconds();
                                        try
                                        {
                                            RemoteNodeTotalPendingTransactionInNetwork = int.Parse(splitPacket[1]
                                                .Replace(
                                                    ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                        .SendRemoteNodeTotalPendingTransaction, ""));
                                        }
                                        catch (Exception error)
                                        {
#if DEBUG
                                            Log.WriteLine("Exception error on receive Total Pending Transaction on sync: " + error.Message);
#endif
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LastRemoteNodePacketReceived =
                                    DateTimeOffset.Now.ToUnixTimeSeconds();
                                try
                                {
                                    RemoteNodeTotalPendingTransactionInNetwork = int.Parse(splitPacket[1]
                                        .Replace(
                                            ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                .SendRemoteNodeTotalPendingTransaction, ""));
                                }
                                catch (Exception error)
                                {
#if DEBUG
                                    Log.WriteLine("Exception error on receive Total Pending Transaction on sync: " + error.Message);
#endif
                                }
                            }
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourNumberTransaction:

                            LastRemoteNodePacketReceived =
                                DateTimeOffset.Now.ToUnixTimeSeconds();
                            if (BlockTransactionSync)
                            {
                                return;
                            }

                            if (!InSyncTransaction)
                            {
                                if (_threadWalletSyncTransaction != null &&
                                    (_threadWalletSyncTransaction.IsAlive || _threadWalletSyncTransaction != null))
                                {
                                    InSyncTransaction = false;
                                    _threadWalletSyncTransaction.Abort();
                                }

                                try
                                {
                                    _threadWalletSyncTransaction = new Thread(async delegate ()
                                    {
#if DEBUG
                                        Log.WriteLine("Their is " +
                                                          splitPacket[1]
                                                              .Replace(
                                                                  ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                      .WalletYourNumberTransaction, "") +
                                                          " to sync on the transaction history.");
#endif

                                        if (int.TryParse(
                                                splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .WalletYourNumberTransaction, ""), out var totalTransactionOfWallet))
                                        {
                                            var totalTransactionInWallet = ClassWalletTransactionCache.ListTransaction.Count;

                                            TotalTransactionInSync = totalTransactionOfWallet;

                                            if (!ClassFormPhase.WalletXiropht.EnableUpdateTransactionWallet)
                                                ClassFormPhase.WalletXiropht.StartUpdateTransactionHistory();

                                            if (totalTransactionInWallet > TotalTransactionInSync)
                                            {
                                                ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
                                                ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(false, false);
                                                totalTransactionInWallet = 0;
                                            }
#if DEBUG
                                            Log.WriteLine("Total transaction synced: " + totalTransactionInWallet + "/" +
                                                              TotalTransactionInSync + " .");
#endif
                                            if (totalTransactionInWallet < totalTransactionOfWallet)
                                            {
#if DEBUG
                                                Log.WriteLine("Start to sync: " + totalTransactionInWallet + "/" +
                                                                  totalTransactionOfWallet + " transactions.");
#endif
                                                InSyncTransaction = true;
                                                try
                                                {



                                                    for (var i = totalTransactionInWallet; i < totalTransactionOfWallet; i++)
                                                    {
                                                        var dateRequestTransaction = DateTimeOffset.Now.ToUnixTimeSeconds();
#if DEBUG
                                                        Log.WriteLine("Ask transaction id: " + i);
#endif
                                                        InReceiveTransaction = true;
                                                        try
                                                        {
                                                            if (!await ListWalletConnectToRemoteNode[8]
                                                                .SendPacketRemoteNodeAsync(
                                                                    ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration
                                                                        .WalletAskTransactionPerId + "|" + WalletConnect.WalletId +
                                                                    "|" + i))
                                                            {
                                                                InSyncTransaction = false;
                                                                InReceiveTransaction = false;
                                                                EnableReceivePacketRemoteNode = false;
                                                                WalletOnUseSync = false;
                                                                LastRemoteNodePacketReceived = 0;

#if DEBUG
                                                                Log.WriteLine("Can't sync transaction wallet.");
#endif
                                                                break;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            InSyncTransaction = false;
                                                            InReceiveTransaction = false;
                                                            break;
                                                        }

                                                        while (InReceiveTransaction)
                                                        {
                                                            if (!InSyncTransaction || BlockTransactionSync || dateRequestTransaction + 5 < DateTimeOffset.Now.ToUnixTimeSeconds()) break;

                                                            Thread.Sleep(100);
                                                        }

                                                        if (BlockTransactionSync)
                                                        {
                                                            return;
                                                        }

                                                    }

                                                    InSyncTransaction = false;
                                                    InReceiveTransaction = false;
                                                }
                                                catch (Exception error)
                                                {
                                                    InSyncTransaction = false;
                                                    InReceiveTransaction = false;
#if DEBUG
                                                    Log.WriteLine("Error to ask transaction: " + error.Message);
#endif
                                                }

                                                InSyncTransaction = false;
                                                InReceiveTransaction = false;
                                            }
                                        }
                                    });
                                    _threadWalletSyncTransaction.Start();
                                }
                                catch (Exception error)
                                {
#if DEBUG
                                    Log.WriteLine("Exception error on sync transaction: " + error.Message);
#endif
                                    if (_threadWalletSyncTransaction != null &&
                                        (_threadWalletSyncTransaction.IsAlive || _threadWalletSyncTransaction != null))
                                    {
                                        InSyncTransaction = false;
                                        _threadWalletSyncTransaction.Abort();
                                    }
                                }
                            }
                            else
                            {
                                if (TotalTransactionInSync == ClassWalletTransactionCache.ListTransaction.Count)
                                    InSyncTransaction = false;
                            }

                            break;

                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                            .WalletYourAnonymityNumberTransaction:

                            LastRemoteNodePacketReceived =
                                DateTimeOffset.Now.ToUnixTimeSeconds();
                            if (BlockTransactionSync)
                            {
                                return;
                            }

                            if (!InSyncTransactionAnonymity)
                            {
                                if (_threadWalletSyncTransactionAnonymity != null &&
                                    (_threadWalletSyncTransactionAnonymity.IsAlive ||
                                     _threadWalletSyncTransactionAnonymity != null))
                                {
                                    InSyncTransactionAnonymity = false;
                                    _threadWalletSyncTransactionAnonymity.Abort();
                                }

                                try
                                {
                                    _threadWalletSyncTransactionAnonymity = new Thread(async delegate ()
                                    {
#if DEBUG
                                        Log.WriteLine("Their is " +
                                                          splitPacket[1]
                                                              .Replace(
                                                                  ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                      .WalletYourAnonymityNumberTransaction, "") +
                                                          " to sync on the anonymity transaction history.");
#endif

                                        if (int.TryParse(
                                                splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .WalletYourAnonymityNumberTransaction, ""),
                                                out var totalTransactionOfWallet))
                                        {
                                            var totalTransactionInWallet =
                                                ClassWalletTransactionAnonymityCache.ListTransaction.Count;

                                            TotalTransactionInSyncAnonymity = totalTransactionOfWallet;

                                            if (!ClassFormPhase.WalletXiropht.EnableUpdateTransactionWallet)
                                                ClassFormPhase.WalletXiropht.StartUpdateTransactionHistory();

                                            if (totalTransactionInWallet > TotalTransactionInSyncAnonymity)
                                            {
                                                ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect
                                                    .WalletAddress);
                                                ClassFormPhase.WalletXiropht.StopUpdateTransactionHistory(false, false);
                                                totalTransactionInWallet = 0;
                                            }
#if DEBUG
                                            Log.WriteLine("Total transaction synced: " + totalTransactionInWallet + "/" +
                                                              TotalTransactionInSyncAnonymity + " .");
#endif
                                            if (totalTransactionInWallet < totalTransactionOfWallet)
                                            {
#if DEBUG
                                                Log.WriteLine("Start to sync: " + totalTransactionInWallet + "/" +
                                                                  totalTransactionOfWallet + " anonymity transactions.");
#endif
                                                InSyncTransactionAnonymity = true;
                                                try
                                                {



                                                    for (var i = totalTransactionInWallet; i < totalTransactionOfWallet; i++)
                                                    {
                                                        var dateRequestTransaction = DateTimeOffset.Now.ToUnixTimeSeconds();
#if DEBUG
                                                        Log.WriteLine("Ask anonymity transaction id: " + i);
#endif
                                                        InReceiveTransactionAnonymity = true;
                                                        try
                                                        {
                                                            if (!await ListWalletConnectToRemoteNode[8]
                                                                .SendPacketRemoteNodeAsync(
                                                                    ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration
                                                                        .WalletAskAnonymityTransactionPerId + "|" +
                                                                    WalletConnect.WalletIdAnonymity +
                                                                    "|" + i))
                                                            {
                                                                InSyncTransactionAnonymity = false;
                                                                InReceiveTransactionAnonymity = false;
                                                                LastRemoteNodePacketReceived = 0;
                                                                EnableReceivePacketRemoteNode = false;
                                                                WalletOnUseSync = false;

#if DEBUG
                                                                Log.WriteLine("Can't sync anonymity transaction wallet.");
#endif
                                                                break;
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            InSyncTransactionAnonymity = false;
                                                            InReceiveTransactionAnonymity = false;
                                                            break;
                                                        }

                                                        while (InReceiveTransactionAnonymity)
                                                        {
                                                            if (!InSyncTransactionAnonymity || BlockTransactionSync || dateRequestTransaction + 5 < DateTimeOffset.Now.ToUnixTimeSeconds()) break;

                                                            Thread.Sleep(100);
                                                        }

                                                        if (BlockTransactionSync)
                                                        {
                                                            return;
                                                        }

                                                    }

                                                    InSyncTransactionAnonymity = false;
                                                    InReceiveTransactionAnonymity = false;
                                                }
                                                catch (Exception error)
                                                {
                                                    InSyncTransactionAnonymity = false;
                                                    InReceiveTransactionAnonymity = false;
#if DEBUG
                                                    Log.WriteLine("Error to ask anonymity transaction: " + error.Message);
#endif
                                                }

                                                InSyncTransactionAnonymity = false;
                                                InReceiveTransactionAnonymity = false;
                                            }
                                        }
                                    });
                                    _threadWalletSyncTransactionAnonymity.Start();
                                }
                                catch (Exception error)
                                {
#if DEBUG
                                    Log.WriteLine("Exception error on sync anonymity transaction: " + error.Message);
#endif
                                    if (_threadWalletSyncTransactionAnonymity != null &&
                                        (_threadWalletSyncTransactionAnonymity.IsAlive ||
                                         _threadWalletSyncTransactionAnonymity != null))
                                    {
                                        InSyncTransactionAnonymity = false;
                                        _threadWalletSyncTransactionAnonymity.Abort();
                                    }
                                }
                            }
                            else
                            {
                                if (TotalTransactionInSyncAnonymity ==
                                    ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                    InSyncTransactionAnonymity = false;
                            }

                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                            .SendRemoteNodeLastBlockFoundTimestamp:
#if DEBUG
                            Log.WriteLine("Last block found date: " + splitPacket[1]
                                              .Replace(
                                                  ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                      .SendRemoteNodeLastBlockFoundTimestamp, ""));
#endif
                            LastRemoteNodePacketReceived =
                                DateTimeOffset.Now.ToUnixTimeSeconds();
                            if (int.TryParse(
                                splitPacket[1]
                                    .Replace(
                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                            .SendRemoteNodeLastBlockFoundTimestamp, ""), out var seconds))
                            {
                                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                dateTime = dateTime.AddSeconds(seconds);
                                dateTime = dateTime.ToLocalTime();
                                LastBlockFound = "" + dateTime;
                            }

                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId:
                            LastRemoteNodePacketReceived =
                                DateTimeOffset.Now.ToUnixTimeSeconds();
                             await Task.Run(async delegate
                             {
#if DEBUG
                                 Log.WriteLine("Block received: " + splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, ""));
#endif
                                 if (WalletSyncMode == 1)
                                 {
                                     if (WalletCheckBlockPerId != 0)
                                     {
                                         if ((ListWalletConnectToRemoteNode[9].LastTrustDate + 5 < DateTimeOffset.Now.ToUnixTimeSeconds() && !ClassConnectorSetting.SeedNodeIp.Contains(ListWalletConnectToRemoteNode[9].RemoteNodeHost)))
                                         {
                                             WalletCheckBlockPerId = 0;
                                             await Task.Delay(100);
                                             if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckBlockPerId + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[9].RemoteNodeHost, Certificate, false, true))
                                             {
                                                 var dateSend = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                 while (WalletCheckBlockPerId == 0)
                                                 {
                                                     if (dateSend + 5 < DateTimeOffset.Now.ToUnixTimeSeconds())
                                                     {
                                                         WalletCheckBlockPerId = -1;
                                                         break;
                                                     }
                                                     await Task.Delay(100);
                                                 }
                                                 if (WalletCheckBlockPerId == 1)
                                                 {
                                                     LastRemoteNodePacketReceived =
                                                         DateTimeOffset.Now.ToUnixTimeSeconds();
                                                     ListWalletConnectToRemoteNode[9].LastTrustDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                     var exist = false;
                                                     for (var i = 0; i < ClassBlockCache.ListBlock.Count; i++)
                                                         if (i < ClassBlockCache.ListBlock.Count)
                                                             if (ClassBlockCache.ListBlock[i] == splitPacket[1]
                                                                     .Replace(
                                                                         ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                             .SendRemoteNodeBlockPerId, ""))
                                                                 exist = true;

                                                     if (!exist)
                                                     {
                                                         ClassBlockCache.ListBlock.Add(
                                                             splitPacket[1]
                                                                 .Replace(
                                                                     ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                         .SendRemoteNodeBlockPerId, ""));
                                                         await ClassBlockCache
                                                             .SaveWalletBlockCache(splitPacket[1]
                                                                 .Replace(
                                                                     ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                         .SendRemoteNodeBlockPerId, ""));
                                                     }

                                                     _lastBlockReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                                     InReceiveBlock = false;
                                                 }
                                                 else
                                                 {
                                                     InReceiveBlock = false;
                                                 }
                                             }
                                         }
                                         else
                                         {
                                             LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                             var exist = false;
                                             for (var i = 0; i < ClassBlockCache.ListBlock.Count; i++)
                                                 if (i < ClassBlockCache.ListBlock.Count)
                                                     if (ClassBlockCache.ListBlock[i] == splitPacket[1]
                                                             .Replace(
                                                                 ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                     .SendRemoteNodeBlockPerId, ""))
                                                         exist = true;

                                             if (!exist)
                                             {
                                                 ClassBlockCache.ListBlock.Add(
                                                     splitPacket[1]
                                                         .Replace(
                                                             ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                 .SendRemoteNodeBlockPerId, ""));
                                                 await ClassBlockCache
                                                     .SaveWalletBlockCache(splitPacket[1]
                                                         .Replace(
                                                             ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                 .SendRemoteNodeBlockPerId, ""));
                                             }

                                             _lastBlockReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                             InReceiveBlock = false;
                                         }
                                     }
                                 }
                                 else
                                 {
                                     var exist = false;
                                     for (var i = 0; i < ClassBlockCache.ListBlock.Count; i++)
                                         if (i < ClassBlockCache.ListBlock.Count)
                                             if (ClassBlockCache.ListBlock[i] == splitPacket[1]
                                                     .Replace(
                                                         ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                             .SendRemoteNodeBlockPerId, ""))
                                                 exist = true;

                                     if (!exist)
                                     {
                                         ClassBlockCache.ListBlock.Add(
                                             splitPacket[1]
                                                 .Replace(
                                                     ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                         .SendRemoteNodeBlockPerId, ""));
                                         await ClassBlockCache
                                             .SaveWalletBlockCache(splitPacket[1]
                                                 .Replace(
                                                     ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                         .SendRemoteNodeBlockPerId, ""));
                                     }

                                     _lastBlockReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                     InReceiveBlock = false;
                                 }
                             }).ConfigureAwait(false);
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId:
                            LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                            await ClassWalletTransactionCache.AddWalletTransactionAsync(splitPacket[1], false);
                            InReceiveTransaction = false;
                            break;

                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId:
                            LastRemoteNodePacketReceived =
                                DateTimeOffset.Now.ToUnixTimeSeconds();
                            await ClassWalletTransactionAnonymityCache.AddWalletTransactionAsync(splitPacket[1]);
                              InReceiveTransactionAnonymity = false;
                            break;
                        case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeKeepAlive:
                            //LastRemoteNodePacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                            break;
                    }
                }
            }
        }


        /// <summary>
        ///     Disconnect wallet from every remote nodes.
        /// </summary>
        [SecurityPermission(SecurityAction.Assert, ControlThread = true)]
        public static async Task DisconnectWholeRemoteNodeSync(bool clean, bool resync)
        {
            if (resync)
            {
                Thread.Sleep(1000);
            }
            try
            {
                if (_threadWalletSyncTransaction != null &&
                    (_threadWalletSyncTransaction.IsAlive || _threadWalletSyncTransaction != null))
                {
                    _threadWalletSyncTransaction.Abort();
                    GC.SuppressFinalize(_threadWalletSyncTransactionAnonymity);
                }

                if (_threadWalletSyncTransactionAnonymity != null &&
                    (_threadWalletSyncTransactionAnonymity.IsAlive || _threadWalletSyncTransactionAnonymity != null))
                {
                    _threadWalletSyncTransactionAnonymity.Abort();
                    GC.SuppressFinalize(_threadWalletSyncTransactionAnonymity);
                }

                if (_threadCheckRemoteNodePacketNetwork != null &&
                    (_threadCheckRemoteNodePacketNetwork.IsAlive || _threadCheckRemoteNodePacketNetwork != null))
                {
                    _threadCheckRemoteNodePacketNetwork.Abort();
                    GC.SuppressFinalize(_threadCheckRemoteNodePacketNetwork);
                }

                if (_threadSendRemoteNodePacketNetwork != null && (_threadSendRemoteNodePacketNetwork.IsAlive || _threadSendRemoteNodePacketNetwork != null))
                {
                    _threadSendRemoteNodePacketNetwork.Abort();
                    GC.SuppressFinalize(_threadSendRemoteNodePacketNetwork);
                }

                if (_threadWalletSyncBlock != null && (_threadWalletSyncBlock.IsAlive || _threadWalletSyncBlock != null))
                {
                    _threadWalletSyncBlock.Abort();
                    GC.SuppressFinalize(_threadWalletSyncBlock);
                }

                if (_threadListenRemoteNodeNetwork1 != null &&
                    (_threadListenRemoteNodeNetwork1.IsAlive || _threadListenRemoteNodeNetwork1 != null))
                {
                    _threadListenRemoteNodeNetwork1.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork1);
                }

                if (_threadListenRemoteNodeNetwork2 != null &&
                    (_threadListenRemoteNodeNetwork2.IsAlive || _threadListenRemoteNodeNetwork2 != null))
                {
                    _threadListenRemoteNodeNetwork2.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork2);
                }

                if (_threadListenRemoteNodeNetwork3 != null &&
                    (_threadListenRemoteNodeNetwork3.IsAlive || _threadListenRemoteNodeNetwork3 != null))
                {
                    _threadListenRemoteNodeNetwork3.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork3);
                }

                if (_threadListenRemoteNodeNetwork4 != null &&
                    (_threadListenRemoteNodeNetwork4.IsAlive || _threadListenRemoteNodeNetwork4 != null))
                {
                    _threadListenRemoteNodeNetwork4.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork4);
                }

                if (_threadListenRemoteNodeNetwork5 != null &&
                    (_threadListenRemoteNodeNetwork5.IsAlive || _threadListenRemoteNodeNetwork5 != null))
                {
                    _threadListenRemoteNodeNetwork5.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork5);
                }

                if (_threadListenRemoteNodeNetwork6 != null &&
                    (_threadListenRemoteNodeNetwork6.IsAlive || _threadListenRemoteNodeNetwork6 != null))
                {
                    _threadListenRemoteNodeNetwork6.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork6);
                }

                if (_threadListenRemoteNodeNetwork7 != null &&
                    (_threadListenRemoteNodeNetwork7.IsAlive || _threadListenRemoteNodeNetwork7 != null))
                {
                    _threadListenRemoteNodeNetwork7.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork7);
                }

                if (_threadListenRemoteNodeNetwork8 != null &&
                    (_threadListenRemoteNodeNetwork8.IsAlive || _threadListenRemoteNodeNetwork8 != null))
                {
                    _threadListenRemoteNodeNetwork8.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork8);
                }

                if (_threadListenRemoteNodeNetwork9 != null &&
                    (_threadListenRemoteNodeNetwork9.IsAlive || _threadListenRemoteNodeNetwork9 != null))
                {
                    _threadListenRemoteNodeNetwork9.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork9);
                }

                if (_threadListenRemoteNodeNetwork10 != null &&
                    (_threadListenRemoteNodeNetwork10.IsAlive || _threadListenRemoteNodeNetwork10 != null))
                {
                    _threadListenRemoteNodeNetwork10.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork10);
                }

                if (_threadListenRemoteNodeNetwork11 != null &&
                    (_threadListenRemoteNodeNetwork11.IsAlive || _threadListenRemoteNodeNetwork11 != null))
                {
                    _threadListenRemoteNodeNetwork11.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork11);
                }

                if (_threadListenRemoteNodeNetwork12 != null &&
                    (_threadListenRemoteNodeNetwork12.IsAlive || _threadListenRemoteNodeNetwork12 != null))
                {
                    _threadListenRemoteNodeNetwork12.Abort();
                    GC.SuppressFinalize(_threadListenRemoteNodeNetwork12);
                }
            }
            catch
            {
                Thread.Sleep(1000);
            }

            if (ListWalletConnectToRemoteNode != null)
            {
                try
                {
                    for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                        if (i < ListWalletConnectToRemoteNode.Count)
                            ListWalletConnectToRemoteNode[i].DisconnectRemoteNodeClient();
                }
                catch
                {

                }
                CleanUpObjectRemoteNode();
            }



            ClassRemoteNodeChecker.ListRemoteNodeChecked.Clear();
            EnableCheckRemoteNodeList = false;
            EnableReceivePacketRemoteNode = false;
            InSyncTransaction = false;
            InSyncTransactionAnonymity = false;
            InReceiveTransaction = false;
            InReceiveTransactionAnonymity = false;
            InSyncBlock = false;
            WalletOnSendingPacketRemoteNode = false;
            WalletOnUseSync = false;
            OnWaitingRemoteNodeList = false;
            WalletCheckMaxSupply = 1;
            WalletCheckCoinCirculating = 1;
            WalletCheckTotalTransactionFee = 1;
            WalletCheckTotalBlockMined = 1;
            WalletCheckNetworkHashrate = 1;
            WalletCheckNetworkDifficulty = 1;
            WalletCheckTotalPendingTransaction = 1;
            WalletCheckBlockPerId = 1;
            if (resync && !OnWaitingRemoteNodeList)
            {
                if (SeedNodeConnectorWallet != null)
                {
                    if (SeedNodeConnectorWallet.GetStatusConnectToSeed())
                    {
                        if (!await SeedNodeConnectorWallet
                            .SendPacketToSeedNodeAsync(
                                ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletAskRemoteNode, Certificate, false,
                                true))
                        {
                            await FullDisconnection(false);
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                            MetroMessageBox.Show(ClassFormPhase.WalletXiropht,
                                "Cannot send packet, your wallet has been disconnected.");
#else
                            MessageBox.Show(ClassFormPhase.WalletXiropht,
                                "Cannot send packet, your wallet has been disconnected.");
#endif
#if DEBUG
                            Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                        }
                    }
                    else
                    {
                        await FullDisconnection(false);
                    }
                }
            }
        }

        /// <summary>
        ///     Clean up the whole list of object connection of remote nodes.
        /// </summary>
        private static void CleanUpObjectRemoteNode()
        {
            try
            {
                ClassRemoteNodeChecker.CleanUpRemoteNodeHost();
            }
            catch
            {

            }
            EnableCheckRemoteNodeList = false;
            try
            {
                ListWalletConnectToRemoteNode.Clear();
            }
            catch
            {

            }
            ListWalletConnectToRemoteNode = null;
        }

#endregion
    }
}