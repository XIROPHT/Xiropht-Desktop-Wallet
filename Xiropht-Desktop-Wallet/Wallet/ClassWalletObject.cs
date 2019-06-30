using MetroFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
#if WINDOWS
#endif
using Xiropht_Connector.Remote;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Seed;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.FormPhase;
using Xiropht_Wallet.FormPhase.ParallelForm;
#if DEBUG
using Xiropht_Wallet.Threading;
#endif
namespace Xiropht_Wallet.Wallet
{

    public enum ClassWalletSyncMode
    {
        WALLET_SYNC_DEFAULT = 0,
        WALLET_SYNC_PUBLIC_NODE = 1,
        WALLET_SYNC_MANUAL_NODE = 2
    }


    public class ClassWalletObject : IDisposable
    {

        private bool disposed;


        ~ClassWalletObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                WalletConnect = null;
                SeedNodeConnectorWallet = null;
            }
            disposed = true;
        }

        /// <summary>
        ///  Object connection.
        /// </summary>
        public string Certificate;
        public ClassSeedNodeConnector SeedNodeConnectorWallet;
        public ClassWalletConnect WalletConnect;
        public List<ClassWalletConnectToRemoteNode> ListWalletConnectToRemoteNode;
        public Dictionary<string, long> ListRemoteNodeBanned = new Dictionary<string, long>();
        public Dictionary<string, long> ListRemoteNodeTotalDisconnect = new Dictionary<string, long>();
        public string WalletLastPathFile;
        public bool WalletPinDisabled;
        public const long WalletMaxRemoteNodeDisconnectAllowed = 3;


        /// <summary>
        ///     Object
        /// </summary>
        public bool EnableCheckRemoteNodeList;


        /// <summary>
        ///     For create a new wallet.
        /// </summary>
        public string WalletDataCreation;
        public string WalletDataPinCreation;
        public string WalletDataCreationPath;
        public bool InCreateWallet;
        public string WalletDataDecrypted;
        public string WalletNewPassword;


        /// <summary>
        ///     Network stats.
        /// </summary>
        public string CoinMaxSupply;
        public string CoinCirculating;
        public string TotalFee;
        public string TotalBlockMined;
        public string NetworkDifficulty;
        public string NetworkHashrate;
        public int RemoteNodeTotalPendingTransactionInNetwork;

        /// <summary>
        /// Check network stats.
        /// </summary>
        private int WalletCheckMaxSupply;
        private int WalletCheckCoinCirculating;
        private int WalletCheckTotalTransactionFee;
        private int WalletCheckTotalBlockMined;
        private int WalletCheckNetworkHashrate;
        private int WalletCheckNetworkDifficulty;
        private int WalletCheckTotalPendingTransaction;
        private int WalletCheckBlockPerId;


        /// <summary>
        ///  For the sync of transactions.
        /// </summary>
        public bool InSyncTransaction;
        public bool InSyncTransactionAnonymity;
        public bool BlockTransactionSync;
        public bool InReceiveTransaction;
        public int TotalTransactionInSync;
        public bool InReceiveTransactionAnonymity;
        public int TotalTransactionInSyncAnonymity;
        public int TotalTransactionPendingOnReceive;

        /// <summary>
        /// For the sync of blocks.
        /// </summary>
        public bool InSyncBlock;
        public bool InReceiveBlock;
        public int TotalBlockInSync;
        public string LastBlockFound;
        private long _lastBlockReceived;


        /// <summary>
        ///  object for remote node connection to sync the wallet.
        /// </summary>
        public bool EnableReceivePacketRemoteNode;
        public int WalletSyncMode;
        public string WalletSyncHostname;
        public bool WalletClosed;
        public bool WalletOnSendingPacketRemoteNode;
        private bool WalletOnUseSync;
        private bool WalletInReconnect;
        public long LastRemoteNodePacketReceived;
        public string WalletAmountInPending;
        public int WalletPacketSpeedTime;

        public string WalletPrivateKeyEncryptedQRCode;
        public CancellationTokenSource WalletCancellationToken;
        public CancellationTokenSource WalletSyncCancellationToken;

        #region Initialization

        /// <summary>
        ///     Start to connect on blockchain.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitializationWalletConnection(string walletAddress, string walletPassword,
            string walletKey,
            string phase)
        {
            try
            {
                if (WalletCancellationToken != null)
                {
                    if (!WalletCancellationToken.IsCancellationRequested)
                    {
                        WalletCancellationToken.Cancel();
                        WalletCancellationToken.Dispose();
                    }
                }
            }
            catch
            {

            }
            WalletCancellationToken = new CancellationTokenSource();

            try
            {
                if (WalletSyncCancellationToken != null)
                {
                    if (!WalletSyncCancellationToken.IsCancellationRequested)
                    {
                        WalletSyncCancellationToken.Cancel();
                        WalletSyncCancellationToken.Dispose();
                    }
                }
            }
            catch
            {

            }
            WalletSyncCancellationToken = new CancellationTokenSource();
            WalletClosed = false;
            Certificate = ClassUtils.GenerateCertificate();
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
                DisconnectWholeRemoteNodeSyncAsync(true, false); // Disconnect and renew objects.


            if (!await SeedNodeConnectorWallet.StartConnectToSeedAsync(string.Empty, ClassConnectorSetting.SeedNodePort))
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
            Program.WalletXiropht.UpdateNetworkStats();

            if (phase != ClassWalletPhase.Create && phase != ClassWalletPhase.Restore)
            {
                try
                {

                    ClassWalletTransactionCache.LoadWalletCache(WalletConnect.WalletAddress);
                }
                catch (Exception error)
                {
#if DEBUG
                Log.WriteLine("Can't read wallet cache, error: " + error.Message);
#endif
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_TRANSACTION_CACHE_ERROR_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
                    await Task.Factory.StartNew(() =>
                    {
                        MessageBox.Show(Program.WalletXiropht,
                          ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_TRANSACTION_CACHE_ERROR_TEXT"));
                        ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
                    }).ConfigureAwait(false);
#endif

                }

                try
                {

                    ClassWalletTransactionAnonymityCache.LoadWalletCache(WalletConnect.WalletAddress);

                }
                catch (Exception error)
                {
#if DEBUG
                Log.WriteLine("Can't read wallet cache, error: " + error.Message);
#endif
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ANONYMITY_TRANSACTION_CACHE_ERROR_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
                    await Task.Factory.StartNew(() =>
                    {
                        MessageBox.Show(Program.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ANONYMITY_TRANSACTION_CACHE_ERROR_TEXT"));
                        ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
                    }).ConfigureAwait(false);
#endif
                }

                try
                {

                    ClassBlockCache.LoadBlockchainCache();
                    TotalBlockInSync = ClassBlockCache.ListBlock.Count;
                    Program.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BLOCK_CACHE_READ_SUCCESS_TEXT"));

                }
                catch (Exception error)
                {
#if DEBUG
                Log.WriteLine("Can't read block cache, error: " + error.Message);
#endif
                    ClassBlockCache.RemoveWalletBlockCache();
                }

                /*
                await Task.Factory.StartNew(async () =>
                {
                    while(ClassWalletTransactionCache.OnLoad || ClassWalletTransactionAnonymityCache.OnLoad)
                    {
                        await Task.Delay(100);
                    }
                    Program.WalletXiropht.StartUpdateTransactionHistory();

                }, WalletCancellationToken.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                */
                Program.WalletXiropht.StartUpdateTransactionHistory();

                if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                    Program.WalletXiropht.StartUpdateBlockSync();


            }

#if DEBUG
            new Thread(delegate ()
            {
                while (SeedNodeConnectorWallet == null)
                {
                    Thread.Sleep(100);
                }

                while (!SeedNodeConnectorWallet.ReturnStatus())
                {
                    Thread.Sleep(100);
                }

                int totalTimeConnected = 0;
                while (SeedNodeConnectorWallet.ReturnStatus())
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
        public bool FullDisconnection(bool manualDisconnection, bool obsolete = false)
        {
            if (WalletConnect != null && SeedNodeConnectorWallet != null)
            {
                Program.WalletXiropht.HideWalletAddressQRCode();

                if ((!WalletClosed && !WalletInReconnect) || manualDisconnection)
                {
                    if (manualDisconnection || WalletConnect.WalletPhase == ClassWalletPhase.Create || WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                    {
                        try
                        {
                            if (WalletCancellationToken != null)
                            {
                                if (!WalletCancellationToken.IsCancellationRequested)
                                {
                                    WalletCancellationToken.Cancel();
                                    WalletCancellationToken.Dispose();
                                }
                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            if (WalletSyncCancellationToken != null)
                            {
                                if (!WalletSyncCancellationToken.IsCancellationRequested)
                                {
                                    WalletSyncCancellationToken.Cancel();
                                    WalletSyncCancellationToken.Dispose();
                                }
                            }
                        }
                        catch
                        {

                        }

                        if (!obsolete)
                        {
                            try
                            {
                                ClassParallelForm.HidePinFormAsync();
                                ClassFormPhase.HideWalletMenu();
                                ClassParallelForm.HideWaitingCreateWalletFormAsync();

                            }
                            catch
                            {

                            }
                        }
                        BlockTransactionSync = false;
                        WalletDataDecrypted = string.Empty;
                        WalletClosed = true;
                        WalletConnect.WalletPhase = string.Empty;
                        DisconnectWalletFromSeedNode(true);

                        DisconnectWholeRemoteNodeSyncAsync(true, false);


                        Program.WalletXiropht.CleanSyncInterfaceWallet();
                        if (!obsolete)
                        {
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
                        }
                        Program.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_DISCONNECTED_TEXT"));
                        Program.WalletXiropht.StopUpdateTransactionHistory(true, true);
                        Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                        ClassWalletTransactionCache.ClearWalletCache();
                        ClassWalletTransactionAnonymityCache.ClearWalletCache();
                        ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                    }
                    else // Try to reconnect.
                    {
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();
                        Task.Factory.StartNew(() => ClassParallelForm.ShowWaitingReconnectFormAsync()).ConfigureAwait(false);
                        int maxRetry = 5;

                        Task.Factory.StartNew(async () =>
                        {
                            while (maxRetry > 0)
                            {
                                try
                                {
                                    if (!WalletCancellationToken.IsCancellationRequested)
                                    {
                                        WalletCancellationToken.Cancel();
                                    }
                                }
                                catch
                                {

                                }

                                try
                                {
                                    if (WalletSyncCancellationToken != null)
                                    {
                                        if (!WalletSyncCancellationToken.IsCancellationRequested)
                                        {
                                            WalletSyncCancellationToken.Cancel();
                                            WalletSyncCancellationToken.Dispose();
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                WalletSyncCancellationToken = new CancellationTokenSource();
                                try
                                {
                                    ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                                    ClassParallelForm.HidePinFormAsync();
                                    BlockTransactionSync = false;
                                    WalletDataDecrypted = string.Empty;
                                    WalletClosed = true;
                                    WalletInReconnect = true;

                                    DisconnectWholeRemoteNodeSyncAsync(true, false);

                                    DisconnectWalletFromSeedNode(false, true);

                                    await Task.Delay(1000);

                                    if (await InitializationWalletConnection(WalletConnect.WalletAddress, WalletConnect.WalletPassword,
                                        WalletConnect.WalletKey, ClassWalletPhase.Login))
                                    {
                                        ListenSeedNodeNetworkForWallet();
                                        if (await WalletConnect.SendPacketWallet(Certificate, string.Empty, false))
                                        {
                                            if (await WalletConnect.SendPacketWallet(
                                                ClassConnectorSettingEnumeration.WalletLoginType + "|" + WalletConnect.WalletAddress, Certificate, true))
                                            {
                                                var timeoutDate = ClassUtils.DateUnixTimeNowSecond();
                                                while (WalletInReconnect)
                                                {
                                                    if (timeoutDate + 5 < ClassUtils.DateUnixTimeNowSecond())
                                                    {
                                                        maxRetry--;
                                                        break;
                                                    }
                                                    await Task.Delay(100);
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

                            ClassParallelForm.HideWaitingReconnectFormAsync();
                            if (maxRetry <= 0)
                            {
                                DisconnectWalletFromSeedNode(true);
                                ClassWalletTransactionCache.ClearWalletCache();
                                ClassWalletTransactionAnonymityCache.ClearWalletCache();
                                ClassFormPhase.HideWalletMenu();

                                Program.WalletXiropht.CleanSyncInterfaceWallet();
                                ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
                                Program.WalletXiropht.UpdateLabelSyncInformation(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_DISCONNECTED_TEXT"));

                                Program.WalletXiropht.StopUpdateTransactionHistory(true, true);

                                Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
#if WINDOWS
                                 await Task.Factory.StartNew(() =>
                                 {
                                     ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                 }).ConfigureAwait(false);

#else
                                await Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                    Program.WalletXiropht.BeginInvoke(invoke);
                                }).ConfigureAwait(false);
#endif
                                WalletInReconnect = false;
                            }
                            else
                            {

                                ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
#if WINDOWS
                                 await Task.Factory.StartNew(() =>
                                 {
                                     MetroMessageBox.Show(Program.WalletXiropht,
                                         ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
                                 }).ConfigureAwait(false);

#else
                                await Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_TITLE_TEXT"), MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                    Program.WalletXiropht.BeginInvoke(invoke);
                                }).ConfigureAwait(false);
#endif
                                WalletInReconnect = false;
                            }
                        });
                    }
                }
            }
            return true;
        }

        #endregion

        #region Wallet Connection

        /// <summary>
        ///     Disconnect wallet from seed nodes.
        /// </summary>
        private void DisconnectWalletFromSeedNode(bool clean, bool reconnect = false)
        {
            try
            {
                SeedNodeConnectorWallet?.DisconnectToSeed();
                SeedNodeConnectorWallet?.Dispose();
            }
            catch
            {

            }

            if (clean)
                CleanUpWalletConnnection(reconnect);

        }

        /// <summary>
        ///     clean up the objects of seed and wallet.
        /// </summary>
        private void CleanUpWalletConnnection(bool reconnect = false)
        {

            WalletPinDisabled = true;
            InCreateWallet = false;
            WalletAmountInPending = string.Empty;
            TotalTransactionPendingOnReceive = 0;
            SeedNodeConnectorWallet?.Dispose();
            SeedNodeConnectorWallet = new ClassSeedNodeConnector();
            if (!reconnect)
            {
                try
                {

                    WalletConnect = null;
                    WalletConnect = new ClassWalletConnect(SeedNodeConnectorWallet);
                }
                catch
                {

                }
            }
            try
            {
                ClassWalletTransactionAnonymityCache.ClearWalletCache();
            }
            catch
            {

            }
            try
            {
                ClassWalletTransactionCache.ClearWalletCache();
            }
            catch
            {

            }
            try
            {
                ClassBlockCache.ListBlock.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Initialization of wallet object.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPassword"></param>
        /// <param name="walletKey"></param>
        /// <param name="phase"></param>
        private void InitializeWalletObject(string walletAddress, string walletPassword, string walletKey,
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
        public void ListenSeedNodeNetworkForWallet()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    var packetNone = 0;
                    var packetNoneMax = 100;
                    var packetAlgoErrorMax = 10;
                    var packetAlgoError = 0;
                    while (SeedNodeConnectorWallet.ReturnStatus() && !WalletClosed)
                    {
                        try
                        {

                            string packetWallet = await WalletConnect.ListenPacketWalletAsync(Certificate, true);
                            if (packetWallet.Length > 0)
                            {
                                if (packetWallet == ClassAlgoErrorEnumeration.AlgoError)
                                {
                                    packetAlgoError++;
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
                                    break;
                                }

                                if (packetNone == packetNoneMax && !InCreateWallet) break;

                                if (packetAlgoError == packetAlgoErrorMax) break;

                                if (packetWallet.Contains("*")) // Character separator.
                                {
                                    var splitPacket = packetWallet.Split(new[] { "*" }, StringSplitOptions.None);
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
                                                        packetAlgoError++;

                                                    }

                                                    await HandleWalletPacketAsync(packetEach.Replace("*", ""));

#if DEBUG
                                                   Log.WriteLine("Packet wallet received: " + packetEach.Replace("*", ""));
#endif
                                                }
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    try
                    {
                        Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Enable keep alive packet for wallet.
        /// </summary>
        private void EnableKeepAliveWalletAsync()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    try
                    {
                        while (SeedNodeConnectorWallet.ReturnStatus() && !WalletClosed)
                        {
                            if (!await SeedNodeConnectorWallet
                                .SendPacketToSeedNodeAsync(ClassWalletCommand.ClassWalletSendEnumeration.KeepAlive, Certificate,
                                    false, true))
                            {
#if DEBUG
                            Log.WriteLine("Can't send keep alive packet to seed node.");
#endif
                                break;
                            }
                            await Task.Delay(5000);
                        }
                        await Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                    }
                    catch
                    {
                    }
                }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Handle packet wallet.
        /// </summary>
        /// <param name="packet"></param>
        private async Task HandleWalletPacketAsync(string packet)
        {

            try
            {
#if DEBUG
            Log.WriteLine("Handle packet wallet: " + packet);
#endif
                var splitPacket = packet.Split(new[] { "|" }, StringSplitOptions.None);

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
                        ClassParallelForm.ShowWaitingFormAsync();
#if DEBUG
                        Log.WriteLine("Loading, please wait a little moment.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WaitingCreatePhase:
                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.ShowWaitingCreateWalletFormAsync();
#if DEBUG
                        Log.WriteLine("Waiting wallet creation finish..");
#endif

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletNewGenesisKey:
#if DEBUG
                        Log.WriteLine("New genesis key received: " + splitPacket[1]);
#endif
                        ClassConnectorSetting.NETWORK_GENESIS_KEY = splitPacket[1];

                        WalletConnect.UpdateWalletIv();
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedMoreCharacters:
                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();


#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
                        if (WalletConnect.WalletPhase == ClassWalletPhase.Create || WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                        {
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                        }
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedLetters:
                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        if (WalletConnect.WalletPhase == ClassWalletPhase.Create || WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                        {
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                        }
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.CreatePhase:

                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();


                        if (splitPacket[1] == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            WalletNewPassword = string.Empty;
                            GC.SuppressFinalize(WalletDataCreation);
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                "Your wallet password need to be stronger , if he is try again later.",
                                "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            await Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                "Your wallet password need to be stronger , if he is try again later.",
                                "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Program.WalletXiropht.BeginInvoke(invoke);
                            }).ConfigureAwait(false);

#endif
                        }
                        else
                        {
#if DEBUG
                            Log.WriteLine("Packet create wallet data: " + WalletDataCreation);
#endif
                            var decryptWalletDataCreation = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, splitPacket[1], WalletNewPassword, ClassWalletNetworkSetting.KeySize);
                            WalletDataCreation = ClassUtils.DecompressData(decryptWalletDataCreation);




                            var splitWalletData = WalletDataCreation.Split(new[] { "\n" }, StringSplitOptions.None);
                            var pin = splitPacket[2];
                            var publicKey = splitWalletData[2];
                            var privateKey = splitWalletData[3];

                            var walletDataToSave = splitWalletData[0] + "\n"; // Only wallet address
                            walletDataToSave += splitWalletData[2] + "\n"; // Only public key

                            var passwordEncrypted = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                WalletNewPassword, WalletNewPassword,
                                ClassWalletNetworkSetting.KeySize);
                            var walletDataToSaveEncrypted = ClassAlgo.GetEncryptedResultManual(
                                    ClassAlgoEnumeration.Rijndael,
                                    walletDataToSave, passwordEncrypted, ClassWalletNetworkSetting.KeySize);

                            using (TextWriter writerWallet = new StreamWriter(WalletDataCreationPath))
                            {
                                writerWallet.Write(walletDataToSaveEncrypted, false);
                            }

                            WalletDataCreation = string.Empty;
                            WalletDataCreationPath = string.Empty;
                            WalletDataPinCreation = string.Empty;
                            WalletNewPassword = string.Empty;
                            var key = publicKey;
                            var key1 = privateKey;
                            var pin1 = pin;
                            Program.WalletXiropht.BeginInvoke((MethodInvoker)delegate
                            {
                                var createWalletSuccessForm = new CreateWalletSuccessFormWallet
                                {
                                    PublicKey = key,
                                    PrivateKey = key1,
                                    PinCode = pin1,
                                    StartPosition = FormStartPosition.CenterParent,
                                    TopMost = false
                                };
                                createWalletSuccessForm.ShowDialog(Program.WalletXiropht);
                            });

                        }
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;

                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAskSuccess:
                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();

                        WalletDataCreation = splitPacket[1];

                        if (splitPacket[1] == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            WalletNewPassword = string.Empty;
                            WalletPrivateKeyEncryptedQRCode = string.Empty;
                            GC.SuppressFinalize(WalletDataCreation);
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                "Your wallet password need to be stronger , if he is try again later.",
                                "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            await Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                "Your wallet password need to be stronger , if he is try again later.",
                                "Password not strong enough or network error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Program.WalletXiropht.BeginInvoke(invoke);
                            }).ConfigureAwait(false);

#endif
                        }
                        else
                        {
#if DEBUG
                            Log.WriteLine("Packet create wallet data: " + WalletDataCreation);
#endif
                            var decryptWalletDataCreation = ClassAlgo.GetDecryptedResultManual(
                                    ClassAlgoEnumeration.Rijndael,
                                    WalletDataCreation, WalletPrivateKeyEncryptedQRCode, ClassWalletNetworkSetting.KeySize);


                            var splitWalletData = decryptWalletDataCreation.Split(new[] { "\n" }, StringSplitOptions.None);
                            var publicKey = splitWalletData[2];
                            var privateKey = splitWalletData[3];
                            var pin = splitWalletData[4];

                            var walletDataToSave = splitWalletData[0] + "\n"; // Only wallet address
                            walletDataToSave += splitWalletData[2] + "\n"; // Only public key

                            var passwordEncrypted = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, WalletNewPassword, WalletNewPassword, ClassWalletNetworkSetting.KeySize);
                            var walletDataToSaveEncrypted = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, walletDataToSave, passwordEncrypted, ClassWalletNetworkSetting.KeySize);
                            using (TextWriter writerWallet = new StreamWriter(WalletDataCreationPath))
                            {

                                writerWallet.Write(walletDataToSaveEncrypted, false);
                            }


                            WalletDataCreation = string.Empty;
                            WalletDataCreationPath = string.Empty;
                            WalletDataPinCreation = string.Empty;
                            WalletNewPassword = string.Empty;
                            WalletPrivateKeyEncryptedQRCode = string.Empty;
                            var key = publicKey;
                            var key1 = privateKey;
                            var pin1 = pin;
                            Program.WalletXiropht.BeginInvoke((MethodInvoker)delegate
                            {
                                var createWalletSuccessForm = new CreateWalletSuccessFormWallet
                                {
                                    PublicKey = key,
                                    PrivateKey = key1,
                                    PinCode = pin1,
                                    StartPosition = FormStartPosition.CenterParent,
                                    TopMost = false
                                };
                                createWalletSuccessForm.ShowDialog(Program.WalletXiropht);
                            });

                        }
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.RightPhase:
#if DEBUG
                        Log.WriteLine("Wallet accepted to connect on blockchain. Send wallet address for login..");
#endif
                        if (!await WalletConnect.SendPacketWallet(
                            ClassWalletCommand.ClassWalletSendEnumeration.LoginPhase + "|" + WalletConnect.WalletAddress,
                            Certificate, true))
                        {
                            await Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
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

                        EnableKeepAliveWalletAsync();
                        WalletConnect.SelectWalletPhase(ClassWalletPhase.Password);
                        if (!await WalletConnect.SendPacketWallet(
                            ClassWalletCommand.ClassWalletSendEnumeration.PasswordPhase + "|" +
                            WalletConnect.WalletPassword, Certificate, true))
                        {
                            await Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
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
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if DEBUG
                            Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                        }
                        Program.WalletXiropht.ShowWalletAddressQRCode(WalletConnect.WalletAddress);

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
                        await Task.Factory.StartNew(delegate { ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress, WalletConnect.WalletAmount); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

#if DEBUG
                        Log.WriteLine("Actual Balance: " + WalletConnect.WalletAmount);
                        Log.WriteLine("Pending amount in pending to receive: " + WalletAmountInPending);

#endif
                        if (LastRemoteNodePacketReceived + 120 < ClassUtils.DateUnixTimeNowSecond())
                        {
                            DisconnectWholeRemoteNodeSyncAsync(true, false);
                            LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                            EnableReceivePacketRemoteNode = false;
                            if (!await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletAskRemoteNode, Certificate, false, true))
                            {
                                await Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
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
                        ClassParallelForm.ShowPinFormAsync();
#if DEBUG
                        Log.WriteLine(
                            "The blockchain ask your pin code. You need to write it for continue to use your wallet:");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.PinAcceptedPhase:
                        WalletConnect.SelectWalletPhase(ClassWalletPhase.Accepted);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine("Pin code accepted, the blockchain will ask your pin code every 15 minutes.");
#endif


                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.PinRefusedPhase:
                        WalletConnect.SelectWalletPhase(ClassWalletPhase.Pin);

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                 ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_CONTENT_TEXT"),
                                 ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        ClassParallelForm.ShowPinFormAsync();

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendMessage:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(splitPacket[1], "Information",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht, splitPacket[1], "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        break;

                    case ClassWalletCommand.ClassWalletReceiveEnumeration.AmountNotValid:

#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassParallelForm.HideWaitingFormAsync();
                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. You try input an invalid amount.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.AmountInsufficient:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. Your amount is insufficient.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.FeeInsufficient:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. Your fee is insufficient.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTransactionBusy:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);
#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. The blockchain currently control your wallet balance health.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletReceiveTransactionBusy:
                        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        dateTime = dateTime.AddSeconds(int.Parse(splitPacket[1]));
                        dateTime = dateTime.ToLocalTime();
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. Your fee is insufficient.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.TransactionAccepted:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                              ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_CONTENT_TEXT") + Environment.NewLine + "Hash: " + splitPacket[1].ToLower(),
                              ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_CONTENT_TEXT") + Environment.NewLine + "Hash: " + splitPacket[1].ToLower(), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Program.WalletXiropht.BeginInvoke(invoke);

                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine(
                            "Transaction accepted on the blockchain side, your history will be updated has soon has possible by public remote nodes or manual node if you have select manual nodes.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.AddressNotValid:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);
#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("The wallet address is not valid, please check it.");
#endif
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletBanPhase:
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_BANNED_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Your wallet is banned for approximatively one hour, try to reconnect later.");
#endif
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAlreadyConnected:
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine("Your wallet is already connected, try to reconnect later.");
#endif
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordAccepted:

                        WalletConnect.WalletPassword = WalletNewPassword; // Update the network object for packet encryption.


                        var encryptedPassword = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            WalletNewPassword, WalletNewPassword, ClassWalletNetworkSetting.KeySize);
                        var encryptWalletDataSave = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
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
                        WalletConnect.UpdateWalletIv();

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_TITLE_TEXT"),
                            MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_CONTENT_TEXT"), ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_TITLE_TEXT"),
                                MessageBoxButtons.OK, MessageBoxIcon.Question);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordRefused:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        WalletNewPassword = string.Empty;
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeAccepted:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht, ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        WalletPinDisabled = !WalletPinDisabled;

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeRefused:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletWarningConnection:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_CONTENT_TEXT"),
                            ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        break;

                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTotalPendingTransactionOnReceive:
                        if (int.TryParse(splitPacket[1], out var totalTransactionInPendingOnReceiveTmp))
                            TotalTransactionPendingOnReceive = totalTransactionInPendingOnReceiveTmp;
                        break;
                    case ClassSeedNodeCommand.ClassReceiveSeedEnumeration.WalletSendRemoteNode:
                        if (!WalletOnUseSync && !EnableReceivePacketRemoteNode)
                        {
                            try
                            {
                                if (ClassRemoteNodeChecker.ListRemoteNodeChecked == null)
                                {
                                    ClassRemoteNodeChecker.ListRemoteNodeChecked = new List<Tuple<string, int>>();
                                }
                                LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

                                if (ListRemoteNodeBanned.Count > 0)
                                {
                                    foreach (var seedNodeIp in ClassConnectorSetting.SeedNodeIp)
                                    {
                                        if (ListRemoteNodeBanned.ContainsKey(seedNodeIp.Key))
                                        {
                                            ListRemoteNodeBanned[seedNodeIp.Key] = 0;
                                        }
                                    }
                                }

                                bool noPublicNode = false;

                                if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                {
                                    bool nodeTargetFound = false;
                                    foreach (var remoteNodeObj in packet.Split(new[] { "|" }, StringSplitOptions.None))
                                    {
                                        if (!nodeTargetFound)
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
                                                            if (!ListRemoteNodeBanned.ContainsKey(remoteNode))
                                                            {
                                                                if (!ClassRemoteNodeChecker.CheckRemoteNodeHostExist(remoteNode))
                                                                {
                                                                    if (!SeedNodeConnectorWallet.ReturnStatus())
                                                                    {
                                                                        await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (WalletOnUseSync)
                                                                        {
                                                                            return;
                                                                        }
                                                                    }
                                                                    Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                        "Start to check remote node host: " + remoteNode);
#if DEBUG
                                                                    Log.WriteLine("Start to check remote node host: " + remoteNode);
#endif
                                                                    switch (await ClassRemoteNodeChecker.CheckNewRemoteNodeHostAsync(remoteNode))
                                                                    {
                                                                        case ClassRemoteNodeStatus.StatusAlive:
                                                                            Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                "Remote node host: " + remoteNode +
                                                                                " is alive and already exist on the list.");
#if DEBUG
                                                                            Log.WriteLine(
                                                                                "Remote node host: " + remoteNode +
                                                                                " is alive and already exist on the list.");
#endif
                                                                            nodeTargetFound = true;
                                                                            break;
                                                                        case ClassRemoteNodeStatus.StatusNew:
                                                                            Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                "Remote node host: " + remoteNode +
                                                                                " is alive and included on the list.");
#if DEBUG
                                                                            Log.WriteLine(
                                                                                "Remote node host: " + remoteNode +
                                                                                " is alive and included on the list.");
#endif
                                                                            nodeTargetFound = true;
                                                                            break;
                                                                        case ClassRemoteNodeStatus.StatusDead:
                                                                            Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                "Remote node host: " + remoteNode + " is dead.");
#if DEBUG
                                                                            Log.WriteLine("Remote node host: " + remoteNode + " is dead.");
#endif
                                                                            InsertBanRemoteNode(remoteNode);
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
                                                            else
                                                            {

                                                                if (ListRemoteNodeBanned[remoteNode] + ClassConnectorSetting.MaxRemoteNodeBanTime < ClassUtils.DateUnixTimeNowSecond())
                                                                {

                                                                    if (!ClassRemoteNodeChecker.CheckRemoteNodeHostExist(remoteNode))
                                                                    {
                                                                        if (!SeedNodeConnectorWallet.ReturnStatus())
                                                                        {
                                                                            await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                                            return;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (WalletOnUseSync)
                                                                            {
                                                                                return;
                                                                            }
                                                                        }
                                                                        Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                            "Start to check remote node host: " + remoteNode);
#if DEBUG
                                                                        Log.WriteLine("Start to check remote node host: " + remoteNode);
#endif
                                                                        switch (await ClassRemoteNodeChecker.CheckNewRemoteNodeHostAsync(remoteNode))
                                                                        {
                                                                            case ClassRemoteNodeStatus.StatusAlive:
                                                                                Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                    "Remote node host: " + remoteNode +
                                                                                    " is alive and already exist on the list.");
#if DEBUG
                                                                                Log.WriteLine(
                                                                                    "Remote node host: " + remoteNode +
                                                                                    " is alive and already exist on the list.");
#endif
                                                                                if (ListRemoteNodeTotalDisconnect.ContainsKey(remoteNode))
                                                                                {
                                                                                    ListRemoteNodeTotalDisconnect[remoteNode] = 0;
                                                                                }
                                                                                nodeTargetFound = true;
                                                                                break;
                                                                            case ClassRemoteNodeStatus.StatusNew:
                                                                                Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                    "Remote node host: " + remoteNode +
                                                                                    " is alive and included on the list.");
#if DEBUG
                                                                                Log.WriteLine(
                                                                                    "Remote node host: " + remoteNode +
                                                                                    " is alive and included on the list.");
#endif
                                                                                if (ListRemoteNodeTotalDisconnect.ContainsKey(remoteNode))
                                                                                {
                                                                                    ListRemoteNodeTotalDisconnect[remoteNode] = 0;
                                                                                }
                                                                                nodeTargetFound = true;
                                                                                break;
                                                                            case ClassRemoteNodeStatus.StatusDead:
                                                                                Program.WalletXiropht.UpdateLabelSyncInformation(
                                                                                    "Remote node host: " + remoteNode + " is dead.");
#if DEBUG
                                                                                Log.WriteLine("Remote node host: " + remoteNode + " is dead.");
#endif
                                                                                InsertBanRemoteNode(remoteNode);
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
                                }

                                if (ClassRemoteNodeChecker.ListRemoteNodeChecked.Count == 0)
                                {
                                    ClassRemoteNodeChecker.ListRemoteNodeChecked.Add(new Tuple<string, int>(SeedNodeConnectorWallet.ReturnCurrentSeedNodeHost(), 30));
                                    //ClassRemoteNodeChecker.ListRemoteNodeChecked.Distinct();
                                }
                                if (ListWalletConnectToRemoteNode == null)
                                {
                                    ListWalletConnectToRemoteNode = new List<ClassWalletConnectToRemoteNode>();
                                }
                                else
                                {
                                    try
                                    {
                                        for (int i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                        {
                                            if (i < ListWalletConnectToRemoteNode.Count)
                                            {
                                                if (ListWalletConnectToRemoteNode[i] != null)
                                                {
                                                    ListWalletConnectToRemoteNode[i].DisconnectRemoteNodeClient();
                                                }
                                            }
                                        }
                                        ListWalletConnectToRemoteNode.Clear();
                                    }
                                    catch
                                    {
                                        ListWalletConnectToRemoteNode = new List<ClassWalletConnectToRemoteNode>();
                                    }
                                }

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


                                if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_DEFAULT || noPublicNode) // Seed node sync.
                                {
                                    int randomSeedNode = ClassUtils.GetRandomBetween(0, ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);

                                    for (int i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                    {
                                        if (i < ListWalletConnectToRemoteNode.Count)
                                        {
                                            if (ListWalletConnectToRemoteNode[i] != null)
                                            {
                                                if (!await ListWalletConnectToRemoteNode[i].ConnectToRemoteNodeAsync(SeedNodeConnectorWallet.ReturnCurrentSeedNodeHost(), ClassConnectorSetting.RemoteNodePort))
                                                {
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                if (!SeedNodeConnectorWallet.ReturnStatus())
                                                {
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (WalletOnUseSync)
                                                    {
                                                        await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE) // Public remote node sync.
                                {
                                    var randomNodeId = ClassUtils.GetRandomBetween(0, ClassRemoteNodeChecker.ListRemoteNodeChecked.Count - 1);
                                    string randomNode = ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNodeId].Item1;

                                    for (int i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                    {
                                        if (i < ListWalletConnectToRemoteNode.Count)
                                        {
                                            if (ListWalletConnectToRemoteNode[i] != null)
                                            {
                                                if (!await ListWalletConnectToRemoteNode[i].ConnectToRemoteNodeAsync(randomNode, ClassConnectorSetting.RemoteNodePort))
                                                {
                                                    InsertBanRemoteNode(ClassRemoteNodeChecker.ListRemoteNodeChecked[randomNodeId].Item1);
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                if (!SeedNodeConnectorWallet.ReturnStatus())
                                                {
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (WalletOnUseSync)
                                                    {
                                                        await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE) // Manual sync mode
                                {
                                    for (int i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                    {
                                        if (i < ListWalletConnectToRemoteNode.Count)
                                        {
                                            if (ListWalletConnectToRemoteNode[i] != null)
                                            {
                                                if (!await ListWalletConnectToRemoteNode[i].ConnectToRemoteNodeAsync(WalletSyncHostname, ClassConnectorSetting.RemoteNodePort))
                                                {
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                if (!SeedNodeConnectorWallet.ReturnStatus())
                                                {
                                                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (WalletOnUseSync)
                                                    {
                                                        await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, true); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
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
                                ListenRemoteNodeNetworkAsync();
#if DEBUG
                                Log.WriteLine("Enable send packet remote node list.");
#endif
                                SendRemoteNodeNetworkAsync();

#if DEBUG
                                Log.WriteLine("Enable check packet remote node list.");
#endif

                                CheckRemoteNodeNetwork();
                                var remoteNodeMessageSync = "Wallet sync with remote node:";
                                var tmplistNodeSync = new List<string>();
                                for (var i = 0; i < ListWalletConnectToRemoteNode.Count - 1; i++)
                                {
                                    if (!tmplistNodeSync.Contains(ListWalletConnectToRemoteNode[i]
                                        .RemoteNodeHost))
                                    {
                                        tmplistNodeSync.Add(ListWalletConnectToRemoteNode[i].RemoteNodeHost);
                                        remoteNodeMessageSync += " " + ListWalletConnectToRemoteNode[i].RemoteNodeHost;
                                    }
                                }

                                Program.WalletXiropht.UpdateLabelSyncInformation(remoteNodeMessageSync);

                            }
                            catch
                            {
                                DisconnectWholeRemoteNodeSyncAsync(true, true);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[1].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[2].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[5].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[6].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[4].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[3].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[7].RemoteNodeHost);
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
                                    InsertBanRemoteNode(ListWalletConnectToRemoteNode[9].RemoteNodeHost);
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
            catch
            {

            }
        }

        /// <summary>
        ///     Send a packet to the seed node network.
        /// </summary>
        /// <param name="packet"></param>
        public async Task<bool> SendPacketWalletToSeedNodeNetwork(string packet, bool encrypted = true)
        {

            if (!await WalletConnect.SendPacketWallet(packet, Certificate, encrypted))
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                   ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_SEND_PACKET_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                await Task.Factory.StartNew(() =>
                {
                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder("WALLET_NETWORK_OBJECT_CANNOT_SEND_PACKET_TEXT"));
                    Program.WalletXiropht.BeginInvoke(invoke);
                }).ConfigureAwait(false);

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

        public void CheckRemoteNodeNetwork()
        {
            Task.Factory.StartNew(async () =>
            {
                var dead = false;
                LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                while (EnableReceivePacketRemoteNode && SeedNodeConnectorWallet.ReturnStatus())
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
                                        if (!ListRemoteNodeTotalDisconnect.ContainsKey(ListWalletConnectToRemoteNode[i].RemoteNodeHost))
                                        {
                                            ListRemoteNodeTotalDisconnect.Add(ListWalletConnectToRemoteNode[i].RemoteNodeHost, 1);
                                        }
                                        else
                                        {
                                            ListRemoteNodeTotalDisconnect[ListWalletConnectToRemoteNode[i].RemoteNodeHost]++;
                                            if (ListRemoteNodeTotalDisconnect[ListWalletConnectToRemoteNode[i].RemoteNodeHost] >= WalletMaxRemoteNodeDisconnectAllowed)
                                            {
                                                InsertBanRemoteNode(ListWalletConnectToRemoteNode[i].RemoteNodeHost);
                                            }
                                        }
                                        dead = true;
                                        break;
                                    }
                                    await Task.Delay(1000);
                                }
                    }
                    catch
                    {
                        dead = true;
                        break;
                    }
                    if (LastRemoteNodePacketReceived + 30 < ClassUtils.DateUnixTimeNowSecond() || !EnableReceivePacketRemoteNode)
                    {
                        dead = true;
                        break;
                    }

                    if (dead) break;

                }

                if (dead)
                {
#if DEBUG
                    Log.WriteLine("Remote node connection dead or stuck.");
#endif
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                }
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
        }

        /// <summary>
        ///     Send each packet on each connected remote node.
        /// </summary>
        public async void SendRemoteNodeNetworkAsync()
        {
            if (!WalletOnSendingPacketRemoteNode)
            {
                WalletOnSendingPacketRemoteNode = true;

                try
                {
                    for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                    {
                        if (i < ListWalletConnectToRemoteNode.Count)
                        {
                            if (ListWalletConnectToRemoteNode[i] != null)
                            {
                                await Task.Factory.StartNew(delegate { SendRemoteNodePacketTarget(i); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Send packet to remote node target.
        /// </summary>
        /// <param name="idNode"></param>
        private async void SendRemoteNodePacketTarget(int idNode)
        {
            while (EnableReceivePacketRemoteNode && SeedNodeConnectorWallet.ReturnStatus())
            {
                if (!WalletOnSendingPacketRemoteNode)
                {
                    break;
                }
                try
                {
                    if (idNode != 11)
                    {
                        switch (idNode)
                        {
                            case 1: // max supply
                                if (WalletCheckMaxSupply != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {
                                        break;
                                    }
                                }
                                break;
                            case 2: // coin circulating
                                if (WalletCheckCoinCirculating != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {
                                        break;
                                    }
                                }
                                break;
                            case 3: // total fee
                                if (WalletCheckTotalTransactionFee != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {
                                        break;
                                    }
                                }
                                break;
                            case 4: // block mined
                                if (WalletCheckTotalBlockMined != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {
                                        break;
                                    }
                                }
                                break;
                            case 5: // difficulty
                                if (WalletCheckNetworkDifficulty != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {

                                        break;
                                    }
                                }
                                break;
                            case 6: // hashrate
                                if (WalletCheckNetworkHashrate != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode]
                                        .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {
                                        break;
                                    }
                                }
                                break;
                            case 7: // total pending transaction
                                if (WalletCheckTotalPendingTransaction != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode].SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {

                                        break;
                                    }
                                }
                                break;
                            case 9: // block per id
                                if (WalletCheckBlockPerId != 0)
                                {
                                    if (!await ListWalletConnectToRemoteNode[idNode].SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                    {

                                        break;
                                    }
                                }
                                break;
                            default:
                                if (!await ListWalletConnectToRemoteNode[idNode].SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                {

                                    break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (!await ListWalletConnectToRemoteNode[idNode].SendPacketTypeRemoteNode(WalletConnect.WalletIdAnonymity))
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    break;
                }
                await Task.Delay(1000);
            }
            try
            {
                await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Listen each remote node from the list.
        /// </summary>
        public async void ListenRemoteNodeNetworkAsync()
        {

            try
            {
                for (int j = 0; j < ListWalletConnectToRemoteNode.Count; j++)
                {
                    if (j < ListWalletConnectToRemoteNode.Count)
                    {
                        if (ListWalletConnectToRemoteNode[j] != null)
                        {
                            try
                            {
                                await Task.Factory.StartNew(delegate { ListenRemoteNodeNetworkTarget(j); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
#if DEBUG
                            Log.WriteLine("Exception error on listen remote packet: " + error.Message);
#endif
                try
                {
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletSyncCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }

            }

            #region Old Sync Listen System
            /*
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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

                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
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
                                                        await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                }
                catch
                {

                }
#if DEBUG
                Log.WriteLine("Disconnect remote node connection");
#endif
            }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            */
            #endregion
        }

        /// <summary>
        /// Listen packet receive from remote node target
        /// </summary>
        /// <param name="idNode"></param>
        private async void ListenRemoteNodeNetworkTarget(int idNode)
        {
            while (WalletOnUseSync && SeedNodeConnectorWallet.ReturnStatus())
            {
                try
                {
                    var packet = await ListWalletConnectToRemoteNode[idNode].ListenRemoteNodeNetworkAsync();

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
                                                await HandlePacketRemoteNodeAsync(splitPacket[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    await Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                    break;
                }
            }
        }

        /// <summary>
        ///     Handle packet from remote node.
        /// </summary>
        /// <param name="packet"></param>
        public async Task HandlePacketRemoteNodeAsync(string packet)
        {
            if (SeedNodeConnectorWallet.ReturnStatus() && !WalletClosed)
            {
                try
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
                                    #region Receive the current max coin supply of the network.
                                    try
                                    {
                                        if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        {
                                            if (WalletCheckMaxSupply != 0)
                                            {
                                                if ((ListWalletConnectToRemoteNode[1].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[1].RemoteNodeHost) || splitPacket[1] != CoinMaxSupply))
                                                {
                                                    WalletCheckMaxSupply = 0;

                                                    await Task.Delay(100);
                                                    if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckMaxSupply + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[1].RemoteNodeHost, Certificate, false, true))
                                                    {
                                                        var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                        while (WalletCheckMaxSupply == 0)
                                                        {
                                                            if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                            {
                                                                WalletCheckMaxSupply = -1;
                                                                break;
                                                            }
#if DEBUG
                                                        Log.WriteLine("Waiting check coin max supply response..");
#endif
                                                            await Task.Delay(100);
                                                            if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                            {
                                                                return;
                                                            }
                                                        }
                                                        if (WalletCheckMaxSupply == 1)
                                                        {
#if DEBUG
                                                        Log.WriteLine("Coin max supply information is good.");
#endif
                                                            LastRemoteNodePacketReceived =
                                                                ClassUtils.DateUnixTimeNowSecond();
                                                            ListWalletConnectToRemoteNode[1].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
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
                                                        ClassUtils.DateUnixTimeNowSecond();
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
                                                ClassUtils.DateUnixTimeNowSecond();
                                            CoinMaxSupply = splitPacket[1]
                                                .Replace(
                                                    ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinMaxSupply,
                                                    "");
                                        }
                                    }
                                    catch
                                    {

                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCoinCirculating:
                                    #region Receive the current amount of coins circulating on the network.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckCoinCirculating != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[2].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[2].RemoteNodeHost)) || CoinCirculating != splitPacket[1])
                                            {
                                                WalletCheckCoinCirculating = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckCoinCirculating + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[2].RemoteNodeHost, Certificate, false, true))
                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckCoinCirculating == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckCoinCirculating = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckCoinCirculating == 1)
                                                    {
                                                        LastRemoteNodePacketReceived =
                                                        ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[2].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
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
                                                    ClassUtils.DateUnixTimeNowSecond();
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
                                            ClassUtils.DateUnixTimeNowSecond();
                                        CoinCirculating = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeCoinCirculating, "");
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentDifficulty:
                                    #region Receive the current mining difficulty of the network.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckNetworkDifficulty != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[5].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[5].RemoteNodeHost)) || NetworkDifficulty != splitPacket[1])
                                            {
                                                WalletCheckNetworkDifficulty = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckNetworkDifficulty + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[5].RemoteNodeHost, Certificate, false, true))

                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckNetworkDifficulty == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckNetworkDifficulty = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckNetworkDifficulty == 1)
                                                    {
                                                        LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[5].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
                                                        NetworkDifficulty = splitPacket[1]
                                                            .Replace(
                                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                    .SendRemoteNodeCurrentDifficulty, "");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                                NetworkDifficulty = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeCurrentDifficulty, "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                        NetworkDifficulty = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeCurrentDifficulty, "");
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate:
                                    #region Receive the current mining hashrate of the network.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckNetworkHashrate != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[6].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[6].RemoteNodeHost)) || NetworkHashrate != splitPacket[1])
                                            {
                                                WalletCheckNetworkHashrate = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckNetworkHashrate + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[6].RemoteNodeHost, Certificate, false, true))

                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckNetworkHashrate == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckNetworkHashrate = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckNetworkHashrate == 1)
                                                    {
                                                        LastRemoteNodePacketReceived =
                                                ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[6].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
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
                                                    ClassUtils.DateUnixTimeNowSecond();
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
                                            ClassUtils.DateUnixTimeNowSecond();
                                        NetworkHashrate = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeCurrentRate,
                                                "");
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalBlockMined:
                                    #region Receive the total amount of blocks mined to sync.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckTotalBlockMined != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[4].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[4].RemoteNodeHost)) || TotalBlockMined != splitPacket[1])
                                            {
                                                WalletCheckTotalBlockMined = 0;
                                                await Task.Delay(10);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalBlockMined + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[4].RemoteNodeHost, Certificate, false, true))
                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckTotalBlockMined == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckTotalBlockMined = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(10);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckTotalBlockMined == 1)
                                                    {
                                                        LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[4].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
                                                        TotalBlockMined = splitPacket[1]
                                                            .Replace(
                                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                    .SendRemoteNodeTotalBlockMined, "");

                                                        if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                                                            Program.WalletXiropht.StartUpdateBlockSync();


                                                        if (!InSyncBlock)
                                                        {
                                                            var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                                            if (!tryParseBlockMined) return;

                                                            var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                                            if (totalBlockInWallet < totalBlockOfNetwork)
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
                                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                                        {
                                                                            return;
                                                                        }
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

                                                                            if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                                            {
                                                                                return;
                                                                            }
                                                                            await Task.Delay(10);
                                                                        }
                                                                    }
                                                                }

                                                                InSyncBlock = false;
                                                                InReceiveBlock = false;


                                                            }
                                                            else
                                                            {
                                                                if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                                {
                                                                    if (totalBlockInWallet - 1 > totalBlockMined)
                                                                    {
                                                                        if (ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[4].RemoteNodeHost))
                                                                        {
                                                                            Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                                                            ClassBlockCache.RemoveWalletBlockCache();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

                                                TotalBlockMined = splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeTotalBlockMined, "");

                                                if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                                                    Program.WalletXiropht.StartUpdateBlockSync();


                                                if (!InSyncBlock)
                                                {
                                                    var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                                    if (!tryParseBlockMined) return;

                                                    var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                                    if (totalBlockInWallet < totalBlockOfNetwork)
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
                                                                if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                                {
                                                                    return;
                                                                }
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

                                                                    await Task.Delay(10);
                                                                    if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                                    {
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        InSyncBlock = false;
                                                        InReceiveBlock = false;

                                                    }
                                                    else
                                                    {
                                                        if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                        {
                                                            if (totalBlockInWallet - 1 > totalBlockMined)
                                                            {
                                                                if (ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[4].RemoteNodeHost))
                                                                {
                                                                    Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                                                    ClassBlockCache.RemoveWalletBlockCache();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                        TotalBlockMined = splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeTotalBlockMined, "");

                                        if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                                            Program.WalletXiropht.StartUpdateBlockSync();

                                        if (!InSyncBlock)
                                        {
                                            var tryParseBlockMined = int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                            if (!tryParseBlockMined) return;

                                            var totalBlockInWallet = ClassBlockCache.ListBlock.Count;
                                            if (totalBlockInWallet < totalBlockOfNetwork)
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
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                        InReceiveBlock = true;
#if DEBUG
                                                    Log.WriteLine("Ask block id: " + i);
#endif

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

                                                        while (InReceiveBlock)
                                                        {
                                                            if (!InSyncBlock) break;

                                                            if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                            {
                                                                return;
                                                            }
                                                            await Task.Delay(10);
                                                        }
                                                    }
                                                }

                                                InSyncBlock = false;
                                                InReceiveBlock = false;


                                            }
                                            else
                                            {
                                                if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                {
                                                    if (totalBlockInWallet - 1 > totalBlockMined)
                                                    {
                                                        if (ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[4].RemoteNodeHost))
                                                        {
                                                            Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                                            ClassBlockCache.RemoveWalletBlockCache();
                                                        }
                                                        else
                                                        {
                                                            InsertBanRemoteNode(ListWalletConnectToRemoteNode[4].RemoteNodeHost);
                                                            DisconnectWholeRemoteNodeSyncAsync(true, true);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee:
                                    #region Receive the total amount of fee accumulated in the network.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckTotalTransactionFee != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[3].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[3].RemoteNodeHost)) || TotalFee != splitPacket[1])
                                            {
                                                WalletCheckTotalTransactionFee = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalTransactionFee + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[3].RemoteNodeHost, Certificate, false, true))
                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckTotalTransactionFee == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckTotalTransactionFee = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckTotalTransactionFee == 1)
                                                    {
                                                        LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[3].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();
                                                        TotalFee = splitPacket[1]
                                                            .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                                                "");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LastRemoteNodePacketReceived =
                                                    ClassUtils.DateUnixTimeNowSecond();
                                                TotalFee = splitPacket[1]
                                                    .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                                        "");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        LastRemoteNodePacketReceived =
                                            ClassUtils.DateUnixTimeNowSecond();
                                        TotalFee = splitPacket[1]
                                            .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalFee,
                                                "");
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeTotalPendingTransaction:
                                    #region Receive the total amount of transaction in pending on the network.
                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckTotalPendingTransaction != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[7].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[7].RemoteNodeHost)) || "" + RemoteNodeTotalPendingTransactionInNetwork != splitPacket[1])
                                            {
                                                WalletCheckTotalPendingTransaction = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckTotalPendingTransaction + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[7].RemoteNodeHost, Certificate, false, true))

                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckTotalPendingTransaction == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckTotalPendingTransaction = -1;
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckTotalPendingTransaction == 1)
                                                    {
                                                        LastRemoteNodePacketReceived =
                                                            ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[7].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();

                                                        RemoteNodeTotalPendingTransactionInNetwork = int.Parse(splitPacket[1]
                                                            .Replace(
                                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                    .SendRemoteNodeTotalPendingTransaction, ""));

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LastRemoteNodePacketReceived =
                                                    ClassUtils.DateUnixTimeNowSecond();
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
                                            ClassUtils.DateUnixTimeNowSecond();

                                        RemoteNodeTotalPendingTransactionInNetwork = int.Parse(splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeTotalPendingTransaction, ""));
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourNumberTransaction:
                                    #region Receive total transaction to sync.
                                    LastRemoteNodePacketReceived =
                                        ClassUtils.DateUnixTimeNowSecond();
                                    if (BlockTransactionSync)
                                    {
                                        return;
                                    }
                                    if (!ClassWalletTransactionAnonymityCache.OnLoad && !ClassWalletTransactionCache.OnLoad)
                                    {
                                        if (!InSyncTransaction)
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

                                                if (totalTransactionInWallet > TotalTransactionInSync)
                                                {
                                                    if (ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[0].RemoteNodeHost))
                                                    {
                                                        ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
                                                        Program.WalletXiropht.StopUpdateTransactionHistory(false, false);
                                                        totalTransactionInWallet = 0;
                                                    }
                                                    else
                                                    {
                                                        InsertBanRemoteNode(ListWalletConnectToRemoteNode[0].RemoteNodeHost);
                                                        DisconnectWholeRemoteNodeSyncAsync(true, true);
                                                    }
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
                                                            if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                            {
                                                                return;
                                                            }
                                                            var dateRequestTransaction = ClassUtils.DateUnixTimeNowSecond();
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
                                                                if (!InSyncTransaction || WalletClosed || BlockTransactionSync || dateRequestTransaction + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                                {
                                                                    if (!WalletClosed)
                                                                    {
                                                                        if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                                                                        {
                                                                            InsertBanRemoteNode(ListWalletConnectToRemoteNode[8].RemoteNodeHost);
                                                                        }
                                                                    }
                                                                    break;
                                                                }

                                                                if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                                {
                                                                    return;
                                                                }
                                                                await Task.Delay(10);
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

                                        }
                                        else
                                        {
                                            if (TotalTransactionInSync == ClassWalletTransactionCache.ListTransaction.Count)
                                                InSyncTransaction = false;
                                        }
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourAnonymityNumberTransaction:
                                    #region Receive total anonymous transaction to sync.
                                    LastRemoteNodePacketReceived =
                                        ClassUtils.DateUnixTimeNowSecond();
                                    if (BlockTransactionSync)
                                    {
                                        return;
                                    }
                                    if (!InSyncTransaction && !ClassWalletTransactionAnonymityCache.OnLoad && !ClassWalletTransactionCache.OnLoad)
                                    {
                                        if (!InSyncTransactionAnonymity)
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

                                                if (totalTransactionInWallet > TotalTransactionInSyncAnonymity)
                                                {
                                                    if (ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[11].RemoteNodeHost))
                                                    {
                                                        ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect
                                                            .WalletAddress);
                                                        Program.WalletXiropht.StopUpdateTransactionHistory(false, false);
                                                        totalTransactionInWallet = 0;
                                                    }
                                                    else
                                                    {
                                                        InsertBanRemoteNode(ListWalletConnectToRemoteNode[11].RemoteNodeHost);
                                                        DisconnectWholeRemoteNodeSyncAsync(true, true);
                                                    }
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



                                                    for (var i = totalTransactionInWallet; i < totalTransactionOfWallet; i++)
                                                    {
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                        var dateRequestTransaction = ClassUtils.DateUnixTimeNowSecond();
#if DEBUG
                                                Log.WriteLine("Ask anonymity transaction id: " + i);
#endif
                                                        InReceiveTransactionAnonymity = true;

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


                                                        while (InReceiveTransactionAnonymity)
                                                        {
                                                            if (!InSyncTransactionAnonymity || WalletClosed || BlockTransactionSync || dateRequestTransaction + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                            {
                                                                if (!WalletClosed)
                                                                {
                                                                    if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                                                                    {
                                                                        InsertBanRemoteNode(ListWalletConnectToRemoteNode[8].RemoteNodeHost);
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                            await Task.Delay(10);
                                                            if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                            {
                                                                return;
                                                            }
                                                        }

                                                        if (BlockTransactionSync)
                                                        {
                                                            return;
                                                        }

                                                    }

                                                    InSyncTransactionAnonymity = false;
                                                    InReceiveTransactionAnonymity = false;


                                                    InSyncTransactionAnonymity = false;
                                                    InReceiveTransactionAnonymity = false;
                                                }
                                            }


                                        }
                                        else
                                        {
                                            if (TotalTransactionInSyncAnonymity ==
                                                ClassWalletTransactionAnonymityCache.ListTransaction.Count)
                                                InSyncTransactionAnonymity = false;
                                        }
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeLastBlockFoundTimestamp:
                                    #region Receive last block found date.
#if DEBUG
                                Log.WriteLine("Last block found date: " + splitPacket[1]
                                                  .Replace(
                                                      ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                          .SendRemoteNodeLastBlockFoundTimestamp, ""));
#endif
                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
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
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId:
                                    #region Receive block sync by ID.
                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

#if DEBUG
                                Log.WriteLine("Block received: " + splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, ""));
#endif

                                    if (WalletSyncMode == (int)ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (WalletCheckBlockPerId != 0)
                                        {
                                            if ((ListWalletConnectToRemoteNode[9].LastTrustDate + ClassConnectorSetting.MaxDelayRemoteNodeTrust < ClassUtils.DateUnixTimeNowSecond() && !ClassConnectorSetting.SeedNodeIp.ContainsKey(ListWalletConnectToRemoteNode[9].RemoteNodeHost)))
                                            {
                                                WalletCheckBlockPerId = 0;
                                                await Task.Delay(100);
                                                if (await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletCheckBlockPerId + "|" + splitPacket[1] + "|" + ListWalletConnectToRemoteNode[9].RemoteNodeHost, Certificate, false, true))
                                                {
                                                    var dateSend = ClassUtils.DateUnixTimeNowSecond();
                                                    while (WalletCheckBlockPerId == 0)
                                                    {
                                                        if (dateSend + (ClassConnectorSetting.MaxTimeoutConnect / 1000) < ClassUtils.DateUnixTimeNowSecond())
                                                        {
                                                            WalletCheckBlockPerId = -1;
                                                            await Task.Factory.StartNew(() => DisconnectWholeRemoteNodeSyncAsync(true, true), WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                                                            break;
                                                        }
                                                        await Task.Delay(100);
                                                        if (!SeedNodeConnectorWallet.ReturnStatus() || WalletClosed)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    if (WalletCheckBlockPerId == 1)
                                                    {
                                                        LastRemoteNodePacketReceived =
                                                            ClassUtils.DateUnixTimeNowSecond();
                                                        ListWalletConnectToRemoteNode[9].LastTrustDate = ClassUtils.DateUnixTimeNowSecond();

                                                        var blockLine = splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, "").Split(new[] { "#" }, StringSplitOptions.None);

                                                        if (!ClassBlockCache.ListBlock.ContainsKey(blockLine[1]))
                                                        {
                                                            ClassBlockObject blockObject = new ClassBlockObject
                                                            {
                                                                BlockHeight = blockLine[0],
                                                                BlockHash = blockLine[1],
                                                                BlockTransactionHash = blockLine[2],
                                                                BlockTimestampCreate = blockLine[3],
                                                                BlockTimestampFound = blockLine[4],
                                                                BlockDifficulty = blockLine[5],
                                                                BlockReward = blockLine[6]
                                                            };
                                                            ClassBlockCache.ListBlock.Add(blockLine[1], blockObject);
                                                            await ClassBlockCache
                                                                .SaveWalletBlockCache(splitPacket[1]
                                                                    .Replace(
                                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                            .SendRemoteNodeBlockPerId, ""));

                                                        }
                                                        else
                                                        {
                                                            InsertBanRemoteNode(ListWalletConnectToRemoteNode[9].RemoteNodeHost);
                                                        }

                                                        _lastBlockReceived = ClassUtils.DateUnixTimeNowSecond();
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
                                                LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

                                                var blockLine = splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, "").Split(new[] { "#" }, StringSplitOptions.None);

                                                if (!ClassBlockCache.ListBlock.ContainsKey(blockLine[1]))
                                                {
                                                    ClassBlockObject blockObject = new ClassBlockObject
                                                    {
                                                        BlockHeight = blockLine[0],
                                                        BlockHash = blockLine[1],
                                                        BlockTransactionHash = blockLine[2],
                                                        BlockTimestampCreate = blockLine[3],
                                                        BlockTimestampFound = blockLine[4],
                                                        BlockDifficulty = blockLine[5],
                                                        BlockReward = blockLine[6]
                                                    };
                                                    ClassBlockCache.ListBlock.Add(blockLine[1], blockObject);
                                                    await ClassBlockCache
                                                        .SaveWalletBlockCache(splitPacket[1]
                                                            .Replace(
                                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                                    .SendRemoteNodeBlockPerId, ""));
                                                }

                                                _lastBlockReceived = ClassUtils.DateUnixTimeNowSecond();
                                                InReceiveBlock = false;
                                            }
                                        }
                                    }
                                    else
                                    {

                                        var blockLine = splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, "").Split(new[] { "#" }, StringSplitOptions.None);

                                        if (!ClassBlockCache.ListBlock.ContainsKey(blockLine[1]))
                                        {
                                            ClassBlockObject blockObject = new ClassBlockObject
                                            {
                                                BlockHeight = blockLine[0],
                                                BlockHash = blockLine[1],
                                                BlockTransactionHash = blockLine[2],
                                                BlockTimestampCreate = blockLine[3],
                                                BlockTimestampFound = blockLine[4],
                                                BlockDifficulty = blockLine[5],
                                                BlockReward = blockLine[6]
                                            };
                                            ClassBlockCache.ListBlock.Add(blockLine[1], blockObject);
                                            await ClassBlockCache
                                                .SaveWalletBlockCache(splitPacket[1]
                                                    .Replace(
                                                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                            .SendRemoteNodeBlockPerId, ""));
                                        }

                                        _lastBlockReceived = ClassUtils.DateUnixTimeNowSecond();
                                        InReceiveBlock = false;
                                    }
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId:
                                    #region Receive transaction by ID.
                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    await ClassWalletTransactionCache.AddWalletTransactionAsync(splitPacket[1]);
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId:
                                    #region Receive anonymous transaction sync by ID.
                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    await ClassWalletTransactionAnonymityCache.AddWalletTransactionAsync(splitPacket[1]);
                                    #endregion
                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeKeepAlive: // This is a valid packet, but we don't take in count for update the datetime of the last packet received, only important packets update the datetime.
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    DisconnectWholeRemoteNodeSyncAsync(true, false);
                }
            }
        }

        /// <summary>
        /// Insert or update remote node host to ban.
        /// </summary>
        /// <param name="host"></param>
        private void InsertBanRemoteNode(string host)
        {
            if (WalletSyncMode != (int)ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE)
            {
                if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(host))
                {
                    if (ListRemoteNodeBanned.ContainsKey(host))
                    {
                        ListRemoteNodeBanned[host] = ClassUtils.DateUnixTimeNowSecond();
                    }
                    else
                    {
                        ListRemoteNodeBanned.Add(host, ClassUtils.DateUnixTimeNowSecond());
                    }
                    Task.Factory.StartNew(delegate { DisconnectWholeRemoteNodeSyncAsync(true, false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
            }
        }


        /// <summary>
        ///     Disconnect wallet from every remote nodes.
        /// </summary>
        public async void DisconnectWholeRemoteNodeSyncAsync(bool clean, bool resync)
        {
            try
            {
                if (WalletSyncCancellationToken != null)
                {
                    if (!WalletSyncCancellationToken.IsCancellationRequested)
                    {
                        WalletSyncCancellationToken.Cancel();
                        WalletSyncCancellationToken.Dispose();
                    }
                }
            }
            catch
            {

            }
            WalletSyncCancellationToken = new CancellationTokenSource();
            if (ListWalletConnectToRemoteNode != null)
            {
                try
                {
                    for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                    {
                        if (i < ListWalletConnectToRemoteNode.Count)
                        {
                            ListWalletConnectToRemoteNode[i].DisconnectRemoteNodeClient();
                            ListWalletConnectToRemoteNode[i].Dispose();
                        }
                    }
                }
                catch
                {

                }
                CleanUpObjectRemoteNode();
            }


            try
            {
                ClassRemoteNodeChecker.ListRemoteNodeChecked.Clear();
            }
            catch
            {

            }
            ClassRemoteNodeChecker.ListRemoteNodeChecked = null;
            WalletCheckMaxSupply = 1;
            WalletCheckCoinCirculating = 1;
            WalletCheckTotalTransactionFee = 1;
            WalletCheckTotalBlockMined = 1;
            WalletCheckNetworkHashrate = 1;
            WalletCheckNetworkDifficulty = 1;
            WalletCheckTotalPendingTransaction = 1;
            WalletCheckBlockPerId = 1;
            EnableReceivePacketRemoteNode = false;
            WalletOnUseSync = false;
            WalletOnSendingPacketRemoteNode = false;
            LastRemoteNodePacketReceived = 0;
            EnableCheckRemoteNodeList = false;
            EnableReceivePacketRemoteNode = false;
            InSyncTransaction = false;
            InSyncTransactionAnonymity = false;
            InReceiveTransaction = false;
            InReceiveTransactionAnonymity = false;
            InSyncBlock = false;
            WalletOnSendingPacketRemoteNode = false;
            WalletOnUseSync = false;
            if (resync)
            {
                if (!await SeedNodeConnectorWallet.SendPacketToSeedNodeAsync(ClassSeedNodeCommand.ClassSendSeedEnumeration.WalletAskRemoteNode, Certificate, false, true))
                {
                    try
                    {
                        await Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
#if DEBUG
                Log.WriteLine("Cannot send packet, your wallet has been disconnected.");
#endif
                }
            }
        }

        /// <summary>
        ///     Clean up the whole list of object connection of remote nodes.
        /// </summary>
        private void CleanUpObjectRemoteNode()
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