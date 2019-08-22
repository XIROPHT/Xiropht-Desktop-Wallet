using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.RPC;
using Xiropht_Connector_All.RPC.Token;
using Xiropht_Connector_All.Seed;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Debug;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.FormPhase;
using Xiropht_Wallet.FormPhase.ParallelForm;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet.Sync;
using Xiropht_Wallet.Wallet.Sync.Object;

#if WINDOWS
#endif

#if DEBUG
using Xiropht_Wallet.Threading;
#endif
namespace Xiropht_Wallet.Wallet.Tcp
{
    public enum ClassWalletSyncMode
    {
        WALLET_SYNC_DEFAULT = 0,
        WALLET_SYNC_PUBLIC_NODE = 1,
        WALLET_SYNC_MANUAL_NODE = 2
    }

    public class ClassWalletSeedNodeStats
    {
        public int PingTime;
        public int TotalError;
        public long LastBanError;
    }

    public class ClassWalletObject : IDisposable
    {
        public const long WalletMaxRemoteNodeDisconnectAllowed = 3;
        private long _lastBlockReceived;
        public bool BlockTransactionSync;

        /// <summary>
        ///     Object connection.
        /// </summary>
        public string Certificate;

        public string CoinCirculating;


        /// <summary>
        ///     Network stats.
        /// </summary>
        public string CoinMaxSupply;

        private bool disposed;


        /// <summary>
        ///     Object
        /// </summary>
        public bool EnableCheckRemoteNodeList;


        /// <summary>
        ///     object for remote node connection to sync the wallet.
        /// </summary>
        public bool EnableReceivePacketRemoteNode;

        public bool InCreateWallet;

        public bool InReceiveBlock;
        public bool InReceiveTransaction;
        public bool InReceiveTransactionAnonymity;

        /// <summary>
        ///     For the sync of blocks.
        /// </summary>
        public bool InSyncBlock;


        /// <summary>
        ///     For the sync of transactions.
        /// </summary>
        public bool InSyncTransaction;

        public bool InSyncTransactionAnonymity;
        public string LastBlockFound;
        public long LastRemoteNodePacketReceived;
        public Dictionary<string, long> ListRemoteNodeBanned = new Dictionary<string, long>();
        public Dictionary<string, long> ListRemoteNodeTotalDisconnect = new Dictionary<string, long>();
        public List<ClassWalletConnectToRemoteNode> ListWalletConnectToRemoteNode;
        public string NetworkDifficulty;
        public string NetworkHashrate;
        public int RemoteNodeTotalPendingTransactionInNetwork;

        public ClassSeedNodeConnector SeedNodeConnectorWallet;
        public int TotalBlockInSync;
        public string TotalBlockMined;
        public string TotalFee;
        public int TotalTransactionInSync;
        public int TotalTransactionInSyncAnonymity;
        public int TotalTransactionPendingOnReceive;
        public string WalletAmountInPending;
        public CancellationTokenSource WalletCancellationToken;
        public bool WalletClosed;
        public ClassWalletConnect WalletConnect;


        /// <summary>
        ///     For create a new wallet.
        /// </summary>
        public string WalletDataCreation;

        public string WalletDataCreationPath;
        public string WalletDataDecrypted;

        public string WalletDataPinCreation;
        private bool WalletInReconnect;
        public string WalletLastPathFile;
        public string WalletNewPassword;
        public bool WalletOnUseSync;
        public bool WalletPinDisabled;

        public string WalletPrivateKeyEncryptedQRCode;
        public CancellationTokenSource WalletSyncCancellationToken;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~ClassWalletObject()
        {
            Dispose(false);
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

        #region Desktop Wallet connection in Online Mode

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
                    if (!WalletCancellationToken.IsCancellationRequested)
                    {
                        WalletCancellationToken.Cancel();
                        WalletCancellationToken.Dispose();
                    }
            }
            catch
            {
            }

            WalletCancellationToken = new CancellationTokenSource();

            try
            {
                if (WalletSyncCancellationToken != null)
                    if (!WalletSyncCancellationToken.IsCancellationRequested)
                    {
                        WalletSyncCancellationToken.Cancel();
                        WalletSyncCancellationToken.Dispose();
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
                DisconnectRemoteNodeTokenSync(); // Disconnect and renew objects.


            if (!await SeedNodeConnectorWallet.StartConnectToSeedAsync(string.Empty, ClassConnectorSetting.SeedNodePort)
            )
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

            if (phase != ClassWalletPhase.Create && phase != ClassWalletPhase.Restore) LoadWalletSync();

#if DEBUG
            new Thread(delegate ()
            {
                try
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

                }
                catch
                {

                }
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

                if (!WalletClosed && !WalletInReconnect || manualDisconnection)
                {
                    if (manualDisconnection || WalletConnect.WalletPhase == ClassWalletPhase.Create ||
                        WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                    {
                        try
                        {
                            if (WalletCancellationToken != null)
                                if (!WalletCancellationToken.IsCancellationRequested)
                                {
                                    WalletCancellationToken.Cancel();
                                    WalletCancellationToken.Dispose();
                                }
                        }
                        catch
                        {
                        }

                        try
                        {
                            if (WalletSyncCancellationToken != null)
                                if (!WalletSyncCancellationToken.IsCancellationRequested)
                                {
                                    WalletSyncCancellationToken.Cancel();
                                    WalletSyncCancellationToken.Dispose();
                                }
                        }
                        catch
                        {
                        }

                        if (!obsolete)
                            try
                            {
                                ClassParallelForm.HidePinFormAsync();
                                ClassFormPhase.HideWalletMenu();
                                ClassParallelForm.HideWaitingCreateWalletFormAsync();
                            }
                            catch
                            {
                            }

                        BlockTransactionSync = false;
                        WalletDataDecrypted = string.Empty;
                        WalletClosed = true;
                        WalletConnect.WalletPhase = string.Empty;
                        WalletTokenIdReceived = false;
                        DisconnectWalletFromSeedNode(true);

                        DisconnectRemoteNodeTokenSync();

                        Program.WalletXiropht.CleanSyncInterfaceWallet();
                        if (!obsolete) ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);

                        Program.WalletXiropht.UpdateLabelSyncInformation(
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectdisconnectedtext));
                        Program.WalletXiropht.StopUpdateTransactionHistory(true, true);
                        Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                        ClassWalletTransactionCache.ClearWalletCache();
                        ClassWalletTransactionAnonymityCache.ClearWalletCache();
                        ClassConnectorSetting.NETWORK_GENESIS_KEY = ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                    }
                    else // Try to reconnect.
                    {
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();
                        Task.Factory.StartNew(() => ClassParallelForm.ShowWaitingReconnectFormAsync())
                            .ConfigureAwait(false);
                        var maxRetry = 5;

                        Task.Factory.StartNew(async () =>
                        {
                            while (maxRetry > 0)
                            {
                                try
                                {
                                    if (!WalletCancellationToken.IsCancellationRequested)
                                        WalletCancellationToken.Cancel();
                                }
                                catch
                                {
                                }

                                try
                                {
                                    if (WalletSyncCancellationToken != null)
                                        if (!WalletSyncCancellationToken.IsCancellationRequested)
                                        {
                                            WalletSyncCancellationToken.Cancel();
                                            WalletSyncCancellationToken.Dispose();
                                        }
                                }
                                catch
                                {
                                }

                                WalletSyncCancellationToken = new CancellationTokenSource();
                                try
                                {
                                    ClassConnectorSetting.NETWORK_GENESIS_KEY =
                                        ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
                                    ClassParallelForm.HidePinFormAsync();
                                    BlockTransactionSync = false;
                                    WalletDataDecrypted = string.Empty;
                                    WalletClosed = true;
                                    WalletInReconnect = true;
                                    WalletTokenIdReceived = false;
                                    DisconnectRemoteNodeTokenSync();
                                    DisconnectWalletFromSeedNode(false, true);

                                    await Task.Delay(1000);

                                    if (await InitializationWalletConnection(WalletConnect.WalletAddress,
                                        WalletConnect.WalletPassword,
                                        WalletConnect.WalletKey, ClassWalletPhase.Login))
                                    {
                                        ListenSeedNodeNetworkForWallet();
                                        if (await WalletConnect.SendPacketWallet(Certificate, string.Empty, false))
                                        {
                                            if (await WalletConnect.SendPacketWallet(
                                                ClassConnectorSettingEnumeration.WalletLoginType + "|" +
                                                WalletConnect.WalletAddress, Certificate, true))
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

                                                if (!WalletInReconnect) break;
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
                                Program.WalletXiropht.UpdateLabelSyncInformation(
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectdisconnectedtext));

                                Program.WalletXiropht.StopUpdateTransactionHistory(true, true);

                                Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                ClassConnectorSetting.NETWORK_GENESIS_KEY =
                                    ClassConnectorSetting.NETWORK_GENESIS_DEFAULT_KEY;
#if WINDOWS
                                await Task.Factory.StartNew(() =>
                                {
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectcannotconnectwalletcontenttext),
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectcannotconnectwallettitletext), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                }).ConfigureAwait(false);

#else
                                await Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectcannotconnectwalletcontenttext),
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectcannotconnectwallettitletext), MessageBoxButtons.OK,
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
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectsuccessconnectwalletcontenttext),
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectsuccessconnectwallettitletext), MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                }).ConfigureAwait(false);

#else
                                await Task.Factory.StartNew(() =>
                                {
                                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectsuccessconnectwalletcontenttext),
                                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                            .walletnetworkobjectsuccessconnectwallettitletext), MessageBoxButtons.OK,
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
                try
                {
                    WalletConnect = null;
                    WalletConnect = new ClassWalletConnect(SeedNodeConnectorWallet);
                }
                catch
                {
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
            EnableWalletAutoConnectToSync();
            try
            {
                Task.Factory.StartNew(async () =>
                    {
                        var packetNone = 0;
                        var packetNoneMax = 1000;
                        var packetAlgoErrorMax = 10;
                        var packetAlgoError = 0;
                        try
                        {
                            while (SeedNodeConnectorWallet.ReturnStatus() && !WalletClosed)
                                try
                                {
                                    var packetWallet =
                                        await WalletConnect.ListenPacketWalletAsync(Certificate, true);
                                    if (packetWallet.Length > 0)
                                    {
                                        if (packetWallet == ClassAlgoErrorEnumeration.AlgoError) break;

                                        if (packetWallet == ClassSeedNodeStatus.SeedNone)
                                            packetNone++;
                                        else
                                            packetNone = 0;

                                        if (packetWallet == ClassSeedNodeStatus.SeedError) break;

                                        if (packetNone == packetNoneMax && !InCreateWallet) break;

                                        if (packetAlgoError == packetAlgoErrorMax) break;

                                        if (packetWallet.Contains("*")) // Character separator.
                                        {
                                            var splitPacket = packetWallet.Split(new[] {"*"}, StringSplitOptions.None);
                                            foreach (var packetEach in splitPacket)
                                                if (!string.IsNullOrEmpty(packetEach))
                                                    if (packetEach.Length > 1)
                                                    {
                                                        if (packetEach == ClassAlgoErrorEnumeration.AlgoError)
                                                            packetAlgoError++;

                                                        await Task.Factory
                                                            .StartNew(
                                                                () => HandleWalletPacketAsync(
                                                                    packetEach.Replace("*", "")),
                                                                CancellationToken.None,
                                                                TaskCreationOptions.DenyChildAttach,
                                                                TaskScheduler.Current).ConfigureAwait(false);

#if DEBUG
                                                        Log.WriteLine("Packet wallet received: " + packetEach.Replace("*", ""));
#endif
                                                    }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                        }
                        catch
                        {
                        }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        try
                        {
                            Task.Factory.StartNew(delegate { FullDisconnection(false); }, WalletCancellationToken.Token,
                                TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                        }
                        catch
                        {
                        }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current)
                    .ConfigureAwait(false);
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
                                    .SendPacketToSeedNodeAsync(ClassWalletCommand.ClassWalletSendEnumeration.KeepAlive,
                                        Certificate,
                                        false, true))
                                {
#if DEBUG
                                Log.WriteLine("Can't send keep alive packet to seed node.");
#endif
                                    break;
                                }

                                await Task.Delay(5000);
                            }

                            await Task.Factory.StartNew(delegate { FullDisconnection(false); },
                                    WalletCancellationToken.Token, TaskCreationOptions.LongRunning,
                                    TaskScheduler.Current)
                                .ConfigureAwait(false);
                        }
                        catch
                        {
                        }
                    }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current)
                    .ConfigureAwait(false);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Handle packet wallet.
        /// </summary>
        /// <param name="packet"></param>
        private async void HandleWalletPacketAsync(string packet)
        {
            try
            {
#if DEBUG
                Log.WriteLine("Handle packet wallet: " + packet);
#endif
                var splitPacket = packet.Split(new[] {"|"}, StringSplitOptions.None);


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
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror1contenttext),
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectcreatewalletpassworderror1titletext), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectcreatewalletpassworderror1contenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectcreatewalletpassworderror1titletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
                        if (WalletConnect.WalletPhase == ClassWalletPhase.Create ||
                            WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                                TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedLetters:
                        ClassParallelForm.HideWaitingFormAsync();
                        ClassParallelForm.HideWaitingCreateWalletFormAsync();
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                            TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2contenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2titletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectcreatewalletpassworderror2contenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectcreatewalletpassworderror2titletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        if (WalletConnect.WalletPhase == ClassWalletPhase.Create ||
                            WalletConnect.WalletPhase == ClassWalletPhase.Restore)
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                                TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletInvalidAsk:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface("Invalid private key.", string.Empty, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke =
                                () => MessageBox.Show(Program.WalletXiropht, "Invalid private key inserted.");
                        }).ConfigureAwait(false);

#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletInvalidPacket:
                        if (WalletConnect.WalletPhase == ClassWalletPhase.Pin)
                        {
                            WalletConnect.SelectWalletPhase(ClassWalletPhase.Pin);

#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectpincoderefusedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectpincoderefusedtitletext),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
#else
                            await Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectpincoderefusedcontenttext),
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectpincoderefusedtitletext), MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                Program.WalletXiropht.BeginInvoke(invoke);
                            }).ConfigureAwait(false);

#endif
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
                                "Password not strong enough or network error.", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
#else
                            await Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                    "Your wallet password need to be stronger , if he is try again later.",
                                    "Password not strong enough or network error.", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
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
                                ClassAlgoEnumeration.Rijndael, splitPacket[1], WalletNewPassword,
                                ClassWalletNetworkSetting.KeySize);
                            WalletDataCreation = ClassUtils.DecompressData(decryptWalletDataCreation);


                            var splitWalletData = WalletDataCreation.Split(new[] {"\n"}, StringSplitOptions.None);
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
                            Program.WalletXiropht.BeginInvoke((MethodInvoker) delegate
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

                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                            TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

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
                                "Password not strong enough or network error.", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
#else
                            await Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                    "Your wallet password need to be stronger , if he is try again later.",
                                    "Password not strong enough or network error.", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
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


                            var splitWalletData =
                                decryptWalletDataCreation.Split(new[] {"\n"}, StringSplitOptions.None);
                            var publicKey = splitWalletData[2];
                            var privateKey = splitWalletData[3];
                            var pin = splitWalletData[4];

                            var walletDataToSave = splitWalletData[0] + "\n"; // Only wallet address
                            walletDataToSave += splitWalletData[2] + "\n"; // Only public key

                            var passwordEncrypted = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                WalletNewPassword, WalletNewPassword, ClassWalletNetworkSetting.KeySize);
                            var walletDataToSaveEncrypted = ClassAlgo.GetEncryptedResultManual(
                                ClassAlgoEnumeration.Rijndael, walletDataToSave, passwordEncrypted,
                                ClassWalletNetworkSetting.KeySize);
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
                            Program.WalletXiropht.BeginInvoke((MethodInvoker) delegate
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

                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                            TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.RightPhase:
#if DEBUG
                        Log.WriteLine("Wallet accepted to connect on blockchain. Send wallet address for login..");
#endif
                        if (!await WalletConnect.SendPacketWallet(
                            ClassWalletCommand.ClassWalletSendEnumeration.LoginPhase + "|" +
                            WalletConnect.WalletAddress,
                            Certificate, true))
                        {
                            await Task.Factory.StartNew(delegate { FullDisconnection(false); },
                                    WalletCancellationToken.Token, TaskCreationOptions.LongRunning,
                                    TaskScheduler.Current)
                                .ConfigureAwait(false);
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
                            await Task.Factory.StartNew(delegate { FullDisconnection(false); },
                                    WalletCancellationToken.Token, TaskCreationOptions.LongRunning,
                                    TaskScheduler.Current)
                                .ConfigureAwait(false);
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
                            await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                                TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
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
                        WalletTokenIdReceived = true;
#if DEBUG
                        Log.WriteLine("Wallet Anonymity id: " + WalletConnect.WalletIdAnonymity);
#endif

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.StatsPhase:
                        WalletConnect.SelectWalletPhase(ClassWalletPhase.Accepted);
                        WalletConnect.WalletAmount = splitPacket[1];
                        if (splitPacket.Length > 2) WalletAmountInPending = splitPacket[2];

                        await Task.Factory
                            .StartNew(
                                delegate
                                {
                                    ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress,
                                        WalletConnect.WalletAmount);
                                }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning,
                                TaskScheduler.Current).ConfigureAwait(false);

#if DEBUG
                        Log.WriteLine("Actual Balance: " + WalletConnect.WalletAmount);
                        Log.WriteLine("Pending amount in pending to receive: " + WalletAmountInPending);

#endif

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
                        ClassParallelForm.HidePinFormAsync();
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectpincodeacceptedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectpincodeacceptedtitletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectpincodeacceptedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectpincodeacceptedtitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
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

                        MetroMessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectpincoderefusedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectpincoderefusedtitletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectpincoderefusedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectpincoderefusedtitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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
                            MethodInvoker invoke =
                                () => MessageBox.Show(Program.WalletXiropht, splitPacket[1], "Information",
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
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactioninvalidamountcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactioninvalidamounttitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactioninvalidamountcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactioninvalidamounttitletext),
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
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtamountcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtamounttitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionnotenoughtamountcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionnotenoughtamounttitletext),
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

                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtfeecontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtfeetitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke =
                                () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtfeecontenttext),
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionnotenoughtfeetitletext),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusycontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusytitletext),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);
#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionbusycontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionbusytitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine("Transaction refused. The blockchain currently control your wallet balance health.");
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletReceiveTransactionBusy:
#if WINDOWS
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusycontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusytitletext),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionbusycontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionbusytitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionacceptedcontenttext) +
                                Environment.NewLine + "Hash: " + splitPacket[1].ToLower(),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.walletnetworkobjectsendtransactionacceptedtitletext),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Question);
                        }).ConfigureAwait(false);

#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke =
                                () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionacceptedcontenttext) + Environment.NewLine +
                                    "Hash: " + splitPacket[1].ToLower(),
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionacceptedtitletext), MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
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
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionaddressnotvalidcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration
                                        .walletnetworkobjectsendtransactionaddressnotvalidtitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }).ConfigureAwait(false);
#else
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(100);
                            ClassParallelForm.HideWaitingFormAsync();
                            await Task.Delay(100);

                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionaddressnotvalidcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectsendtransactionaddressnotvalidtitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("The wallet address is not valid, please check it.");
#endif

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletBanPhase:
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectbannedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectbannedtitletext),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectbannedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectbannedtitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
#if DEBUG
                        Log.WriteLine("Your wallet is banned for approximatively one hour, try to reconnect later.");
#endif
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                            TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAlreadyConnected:
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectalreadyconnectedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectalreadyconnectedtitletext),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectalreadyconnectedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectalreadyconnectedtitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
#if DEBUG
                        Log.WriteLine("Your wallet is already connected, try to reconnect later.");
#endif
                        await Task.Factory.StartNew(delegate { FullDisconnection(true); }, CancellationToken.None,
                            TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordAccepted:

                        WalletConnect.WalletPassword =
                            WalletNewPassword; // Update the network object for packet encryption.


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
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepasswordacceptedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepasswordacceptedtitletext),
                            MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepasswordacceptedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepasswordacceptedtitletext),
                                MessageBoxButtons.OK, MessageBoxIcon.Question);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletChangePasswordRefused:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepasswordrefusedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepasswordrefusedtitletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepasswordrefusedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepasswordrefusedtitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        WalletNewPassword = string.Empty;
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeAccepted:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepincodestatusacceptedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepincodestatusacceptedtitletext),
                            MessageBoxButtons.OK, MessageBoxIcon.Question);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke =
                                () => MessageBox.Show(Program.WalletXiropht,
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectchangepincodestatusacceptedcontenttext),
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .walletnetworkobjectchangepincodestatusacceptedtitletext), MessageBoxButtons.OK,
                                    MessageBoxIcon.Question);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        WalletPinDisabled = !WalletPinDisabled;

                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletDisablePinCodeRefused:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepincodestatusrefusedcontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectchangepincodestatusrefusedtitletext),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepincodestatusrefusedcontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectchangepincodestatusrefusedtitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);
#endif
                        break;
                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletWarningConnection:
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectwarningwalletconnectioncontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectwarningwalletconnectiontitletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectwarningwalletconnectioncontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .walletnetworkobjectwarningwalletconnectiontitletext), MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            Program.WalletXiropht.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
                        break;

                    case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletSendTotalPendingTransactionOnReceive:
                        if (int.TryParse(splitPacket[1], out var totalTransactionInPendingOnReceiveTmp))
                            TotalTransactionPendingOnReceive = totalTransactionInPendingOnReceiveTmp;
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
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .walletnetworkobjectcannotsendpackettext),
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                await Task.Factory.StartNew(() =>
                {
                    MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .walletnetworkobjectcannotsendpackettext));
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


        #endregion

        #region Desktop Wallet connection in Token Network Mode

        private Dictionary<string, ClassWalletSeedNodeStats>
            ListOfSeedNodesSpeed; // Key: IP of Seed Node, Value: ClassWalletSeedNodeStats

        private const string HttpPacketError = "ERROR";
        private const string TokenPacketNetworkNotExist = "not_exist";
        private bool WalletTokenIdReceived;
        private ClassTokenRemoteNode classTokenRemoteNode;
        private const int WalletUpdateBalanceInterval = 15 * 1000;
        private const int WalletCheckSyncInterval = 1 * 1000;
        private const int WalletSendPacketToNodeInterval = 5 * 1000;
        private int WalletTokenUpdateFailed;
        private int WalletMaxSeedNodeError = 2; // Max failure.
        private int WalletCleanSeedNodeError = 30; // Each 30 seconds


        /// <summary>
        ///     Initialization Wallet Token Mode.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="walletPassword"></param>
        public void InitializationWalletTokenMode(string walletAddress, string walletPublicKey, string walletPassword)
        {
            WalletTokenIdReceived = false;
            WalletClosed = false;
            WalletOnUseSync = false;
            SeedNodeConnectorWallet = new ClassSeedNodeConnector();
            WalletConnect = new ClassWalletConnect(null)
            {
                WalletAddress = walletAddress,
                WalletKey = walletPublicKey,
                WalletPassword = walletPassword
            };
            WalletTokenUpdateFailed = 0;
            EndCancellationTokenSource();
            WalletCancellationToken = new CancellationTokenSource();
            WalletSyncCancellationToken = new CancellationTokenSource();
            LoadWalletSync();
            EnableWalletTokenAutoUpdateStats();
            EnableWalletAutoConnectToSync();
            ClassFormPhase.ShowWalletMenu();
            ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress, WalletConnect.WalletAmount);
            Program.WalletXiropht.UpdateNetworkStats();
        }

        /// <summary>
        ///     Load the wallet sync
        /// </summary>
        private void LoadWalletSync()
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
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .walletnetworkobjecttransactioncacheerrortext), string.Empty, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ClassWalletTransactionCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
                Task.Factory.StartNew(() =>
                {
                    MessageBox.Show(Program.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .walletnetworkobjecttransactioncacheerrortext));
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
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .walletnetworkobjectanonymitytransactioncacheerrortext), string.Empty, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
#else
                Task.Factory.StartNew(() =>
                {
                    MessageBox.Show(Program.WalletXiropht,
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .walletnetworkobjectanonymitytransactioncacheerrortext));
                    ClassWalletTransactionAnonymityCache.RemoveWalletCache(WalletConnect.WalletAddress);
                }).ConfigureAwait(false);
#endif
            }

            try
            {
                ClassBlockCache.LoadBlockchainCache();
                TotalBlockInSync = ClassBlockCache.ListBlock.Count;
                Program.WalletXiropht.UpdateLabelSyncInformation(
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .walletnetworkobjectblockcachereadsuccesstext));
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("Can't read block cache, error: " + error.Message);
#endif
                ClassBlockCache.RemoveWalletBlockCache();
            }

            Program.WalletXiropht.StartUpdateTransactionHistory();

            if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                Program.WalletXiropht.StartUpdateBlockSync();
        }

        /// <summary>
        ///     Send packet to retrieve wallet current balance.
        /// </summary>
        public void EnableWalletTokenAutoUpdateStats()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    while (!WalletClosed)
                    {
                        try
                        {
                            await GetCurrentWalletBalanceAsync();
                        }
                        catch (Exception error)
                        {
#if DEBUG
                                Log.WriteLine("EnableWalletTokenAutoUpdateStats exception: "+error.Message);
#endif
                        }

                        await Task.Delay(WalletUpdateBalanceInterval);
                    }
                }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("EnableWalletTokenAutoUpdateStats exception: " + error.Message);
#endif
            }
        }

        /// <summary>
        /// Automatically connect the wallet for sync.
        /// </summary>
        public void EnableWalletAutoConnectToSync()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                    {
                        while (!WalletClosed)
                        {
                            try
                            {
                                if (WalletTokenIdReceived)
                                    if (!WalletOnUseSync)
                                    {
                                        DisconnectRemoteNodeTokenSync();
                                        if (Program.WalletXiropht.WalletSyncMode ==
                                            ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        {
                                            await GetRemoteNodeListAsync();
                                        }

                                        await EnableWalletTokenSync();

                                    }
                                    else
                                    {
                                        try
                                        {
                                            bool endCheck = false;
                                            if (ListWalletConnectToRemoteNode.Count > 0)
                                            {
                                                for (int i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                                {
                                                    if (!endCheck)
                                                    {
                                                        if (ListWalletConnectToRemoteNode[i] != null)
                                                        {
                                                            if (!ListWalletConnectToRemoteNode[i].RemoteNodeStatus ||
                                                                ListWalletConnectToRemoteNode[i].Disposed)
                                                            {
                                                                endCheck = true;
                                                                DisconnectRemoteNodeTokenSync();
                                                                WalletOnUseSync = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            endCheck = true;
                                                            DisconnectRemoteNodeTokenSync();
                                                            WalletOnUseSync = false;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                DisconnectRemoteNodeTokenSync();
                                                WalletOnUseSync = false;
                                            }
                                        }
                                        catch
                                        {
                                            DisconnectRemoteNodeTokenSync();
                                            WalletOnUseSync = false;
                                        }

                                        if (!WalletOnUseSync)
                                        {
                                            if (Program.WalletXiropht.WalletSyncMode ==
                                                ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                            {
                                                await GetRemoteNodeListAsync();
                                            }

                                            await EnableWalletTokenSync();

                                        }
                                    }
                            }
                            catch (Exception error)
                            {
#if DEBUG
                                Log.WriteLine("EnableWalletTokenAutoUpdateStats exception: "+error.Message);
#endif
                                DisconnectRemoteNodeTokenSync();
                                WalletOnUseSync = false;
                            }

                            await Task.Delay(WalletCheckSyncInterval);
                        }
                    }, WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current)
                    .ConfigureAwait(false);


            }
            catch
            {

            }
        }

        /// <summary>
        ///     Get current wallet balance.
        /// </summary>
        private async Task GetCurrentWalletBalanceAsync()
        {
            var seedNodeSelected = GetSeedNodeAlive();
            bool getBalanceStatus = false;
            if (seedNodeSelected.Item1)
            {
                var currentToken = await GetWalletTokenAsync(seedNodeSelected.Item2);

                if (currentToken != TokenPacketNetworkNotExist)
                {
                    var encryptedRequest = ClassRpcWalletCommand.TokenAskBalance + "|" + currentToken + "|" +
                                           (DateTimeOffset.Now.ToUnixTimeSeconds() + 1);
                    encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
                        encryptedRequest,
                        WalletConnect.WalletAddress + WalletConnect.WalletKey + WalletConnect.WalletPassword,
                        ClassWalletNetworkSetting.KeySize);
                    var responseWallet = await ProceedTokenRequestHttpAsync(
                        "http://" + seedNodeSelected.Item2 + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                        ClassConnectorSettingEnumeration.WalletTokenType + "|" + WalletConnect.WalletAddress + "|" +
                        encryptedRequest);

                    if (responseWallet != HttpPacketError)
                    {
                        try
                        {
                            var responseWalletJson = JObject.Parse(responseWallet);
                            responseWallet = responseWalletJson["result"].ToString();
                            if (responseWallet != TokenPacketNetworkNotExist)
                            {
                                responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                    responseWallet,
                                    WalletConnect.WalletAddress + WalletConnect.WalletKey +
                                    WalletConnect.WalletPassword +
                                    currentToken, ClassWalletNetworkSetting.KeySize);
                                if (responseWallet != ClassAlgoErrorEnumeration.AlgoError)
                                {
                                    var walletBalance = responseWallet;
                                    var splitWalletBalance = walletBalance.Split(new[] {"|"}, StringSplitOptions.None);
                                    if (long.Parse(splitWalletBalance[splitWalletBalance.Length - 1]) + 10 -
                                        DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                                        if (long.Parse(splitWalletBalance[splitWalletBalance.Length - 1]) + 10 >=
                                            DateTimeOffset.Now.ToUnixTimeSeconds())
                                        {
                                            WalletConnect.WalletAmount = splitWalletBalance[1].Replace(",", ".");
                                            WalletAmountInPending = splitWalletBalance[2].Replace(",", ".");
                                            WalletConnect.WalletId = splitWalletBalance[3];
                                            WalletConnect.WalletIdAnonymity = splitWalletBalance[4];
                                            WalletTokenIdReceived = true;
                                            ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress,
                                                WalletConnect.WalletAmount);
                                            getBalanceStatus = true;
                                            WalletTokenUpdateFailed = 0;
                                        }
                                }
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                        Log.WriteLine("GetCurrentWalletBalanceAsync exception: " + error.Message);
#endif
                        }
                    }
                    else
                    {
                        ListOfSeedNodesSpeed[seedNodeSelected.Item2].TotalError++;
                        if (ListOfSeedNodesSpeed[seedNodeSelected.Item2].TotalError >= WalletMaxSeedNodeError)
                        {
                            ListOfSeedNodesSpeed[seedNodeSelected.Item2].LastBanError =
                                DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                        }
                    }
                }
            }

            if (!getBalanceStatus)
            {
                WalletTokenUpdateFailed++;
                if (WalletTokenUpdateFailed >= 10)
                {
                    Program.WalletXiropht.UpdateLabelSyncInformation(
                        "Can't retrieve current balance from Token Network. Calculate balance from your sync, please wait a moment..");
                    await WalletTokenCalculateBalanceFromSync();
                    Program.WalletXiropht.UpdateLabelSyncInformation("Calculation done from your sync done.");
                    WalletTokenUpdateFailed = 0;
                }
            }
        }

        /// <summary>
        /// Calculate balance and pending balance from transaction synced.
        /// </summary>
        /// <returns></returns>
        private async Task WalletTokenCalculateBalanceFromSync()
        {
            decimal balance = 0;
            decimal pendingBalance = 0;
            if (ClassWalletTransactionCache.ListTransaction.Count > 0)
            {
                foreach (var transaction in ClassWalletTransactionCache.ListTransaction)
                {
                    switch (transaction.Value.TransactionType)
                    {
                        case ClassWalletTransactionType.SendTransaction:
                            balance -= transaction.Value.TransactionAmount;
                            balance -= transaction.Value.TransactionFee;
                            break;
                        case ClassWalletTransactionType.ReceiveTransaction:
                            if (DateTimeOffset.Now.ToUnixTimeSeconds() >= transaction.Value.TransactionTimestampRecv)
                            {
                                balance += transaction.Value.TransactionAmount;
                            }

                            break;
                    }
                }
            }

            if (ClassWalletTransactionAnonymityCache.ListTransaction.Count > 0)
            {
                foreach (var transaction in ClassWalletTransactionAnonymityCache.ListTransaction)
                {
                    switch (transaction.Value.TransactionType)
                    {
                        case ClassWalletTransactionType.SendTransaction:
                            balance -= transaction.Value.TransactionAmount;
                            balance -= transaction.Value.TransactionFee;
                            break;
                        case ClassWalletTransactionType.ReceiveTransaction:
                            if (DateTimeOffset.Now.ToUnixTimeSeconds() >= transaction.Value.TransactionTimestampRecv)
                            {
                                balance += transaction.Value.TransactionAmount;
                            }

                            break;
                    }
                }
            }

            WalletAmountInPending = pendingBalance.ToString().Replace(",", ".");
            ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress,
                balance.ToString().Replace(",", "."));
            await Task.Delay(1000);
        }

        /// <summary>
        ///     Get wallet token from token system.
        /// </summary>
        /// <param name="getSeedNodeRandom"></param>
        /// <returns></returns>
        private async Task<string> GetWalletTokenAsync(string getSeedNodeRandom)
        {
            var encryptedRequest = ClassRpcWalletCommand.TokenAsk + "|empty-token|" +
                                   (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
            encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, encryptedRequest,
                WalletConnect.WalletAddress + WalletConnect.WalletKey + WalletConnect.WalletPassword,
                ClassWalletNetworkSetting.KeySize);
            var responseWallet = await ProceedTokenRequestHttpAsync(
                "http://" + getSeedNodeRandom + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                ClassConnectorSettingEnumeration.WalletTokenType + "|" + WalletConnect.WalletAddress + "|" +
                encryptedRequest);
            if (responseWallet != HttpPacketError)
            {
                try
                {
                    var responseWalletJson = JObject.Parse(responseWallet);
                    responseWallet = responseWalletJson["result"].ToString();
                    if (responseWallet != TokenPacketNetworkNotExist)
                    {
                        responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            responseWallet,
                            WalletConnect.WalletAddress + WalletConnect.WalletKey + WalletConnect.WalletPassword,
                            ClassWalletNetworkSetting.KeySize);
                        var splitResponseWallet = responseWallet.Split(new[] {"|"}, StringSplitOptions.None);
                        if (long.Parse(splitResponseWallet[splitResponseWallet.Length - 1]) + 10 -
                            DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                        {
                            if (long.Parse(splitResponseWallet[splitResponseWallet.Length - 1]) + 60 >=
                                DateTimeOffset.Now.ToUnixTimeSeconds())
                                return splitResponseWallet[1];
                            return TokenPacketNetworkNotExist;
                        }

                        return TokenPacketNetworkNotExist;
                    }

                    return TokenPacketNetworkNotExist;
                }
                catch (Exception error)
                {
#if DEBUG
                Log.WriteLine("GetWalletTokenAsync exception: " + error.Message);
#endif
                }
            }
            else
            {
                ListOfSeedNodesSpeed[getSeedNodeRandom].TotalError++;
                if (ListOfSeedNodesSpeed[getSeedNodeRandom].TotalError >= WalletMaxSeedNodeError)
                {
                    ListOfSeedNodesSpeed[getSeedNodeRandom].LastBanError =
                        DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                }
            }

            return TokenPacketNetworkNotExist;
        }

        /// <summary>
        ///     Send a transaction with the token system.
        /// </summary>
        /// <param name="walletTarget"></param>
        /// <param name="amountSend"></param>
        /// <param name="feeSend"></param>
        /// <param name="anonymous"></param>
        public async Task SendWalletTokenTransaction(string walletTarget, decimal amountSend, decimal feeSend,
            string anonymous)
        {
            var busyOrError = false;
            var seedNodeSelected = GetSeedNodeAlive();
            if (seedNodeSelected.Item1)
            {
                var currentToken = await GetWalletTokenAsync(seedNodeSelected.Item2);

                if (currentToken != TokenPacketNetworkNotExist)
                {
                    var encryptedRequest = ClassRpcWalletCommand.TokenAskWalletSendTransaction + "|" + currentToken +
                                           "|" + walletTarget + "|" + amountSend + "|" + feeSend + "|" + anonymous +
                                           "|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
                    encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael,
                        encryptedRequest,
                        WalletConnect.WalletAddress + WalletConnect.WalletKey + WalletConnect.WalletPassword,
                        ClassWalletNetworkSetting.KeySize);
                    var responseWallet = await ProceedTokenRequestHttpAsync(
                        "http://" + seedNodeSelected.Item2 + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                        ClassConnectorSettingEnumeration.WalletTokenType + "|" + WalletConnect.WalletAddress + "|" +
                        encryptedRequest);

                    if (responseWallet != HttpPacketError)
                    {
                        try
                        {
                            var responseWalletJson = JObject.Parse(responseWallet);
                            responseWallet = responseWalletJson["result"].ToString();
                            if (responseWallet != TokenPacketNetworkNotExist)
                            {
                                responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                    responseWallet,
                                    WalletConnect.WalletAddress + WalletConnect.WalletKey +
                                    WalletConnect.WalletPassword +
                                    currentToken, ClassWalletNetworkSetting.KeySize);
                                if (responseWallet != ClassAlgoErrorEnumeration.AlgoError)
                                {
                                    var walletTransaction = responseWallet;
                                    if (responseWallet != TokenPacketNetworkNotExist)
                                    {
                                        var splitWalletTransaction =
                                            walletTransaction.Split(new[] {"|"}, StringSplitOptions.None);
                                        if (long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) + 10 -
                                            DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                                        {
                                            if (long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) +
                                                10 >= DateTimeOffset.Now.ToUnixTimeSeconds())
                                            {
                                                WalletConnect.WalletAmount =
                                                    splitWalletTransaction[1].Replace(",", ".");
                                                WalletAmountInPending = splitWalletTransaction[2].Replace(",", ".");
                                                ClassFormPhase.ShowWalletInformationInMenu(WalletConnect.WalletAddress,
                                                    WalletConnect.WalletAmount);

                                                switch (splitWalletTransaction[0])
                                                {
                                                    case ClassRpcWalletCommand.SendTokenTransactionBusy:
                                                        busyOrError = true;
                                                        break;
                                                    case ClassRpcWalletCommand.SendTokenTransactionConfirmed:
                                                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            ClassFormPhase.MessageBoxInterface(
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactionacceptedcontenttext) +
                                                                Environment.NewLine + "Hash: " +
                                                                splitWalletTransaction[3].ToLower(),
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactionacceptedtitletext),
                                                                MessageBoxButtons.OK, MessageBoxIcon.Question);
                                                        }).ConfigureAwait(false);

#else
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            MethodInvoker invoke =
                                                                () => MessageBox.Show(Program.WalletXiropht,
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactionacceptedcontenttext) +
                                                                    Environment.NewLine + "Hash: " +
                                                                    splitWalletTransaction[3].ToLower(),
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactionacceptedtitletext),
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                            Program.WalletXiropht.BeginInvoke(invoke);

                                                        }).ConfigureAwait(false);
#endif
#if DEBUG
                                                    Log.WriteLine(
                                                        "Transaction accepted on the blockchain side, your history will be updated has soon has possible by public remote nodes or manual node if you have select manual nodes.");
#endif
                                                        break;
                                                    case ClassRpcWalletCommand.SendTokenTransactionInvalidTarget:
                                                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            ClassFormPhase.MessageBoxInterface(
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactionaddressnotvalidcontenttext),
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactionaddressnotvalidtitletext),
                                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                        }).ConfigureAwait(false);
#else
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            MethodInvoker invoke =
                                                                () => MessageBox.Show(Program.WalletXiropht,
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactionaddressnotvalidcontenttext),
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactionaddressnotvalidtitletext),
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                            Program.WalletXiropht.BeginInvoke(invoke);
                                                        }).ConfigureAwait(false);

#endif
#if DEBUG
                                                    Log.WriteLine("The wallet address is not valid, please check it.");
#endif
                                                        break;
                                                    case ClassRpcWalletCommand.SendTokenTransactionRefused:
                                                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            ClassFormPhase.MessageBoxInterface(
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactioninvalidamountcontenttext),
                                                                ClassTranslation.GetLanguageTextFromOrder(
                                                                    ClassTranslationEnumeration
                                                                        .walletnetworkobjectsendtransactioninvalidamounttitletext),
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        }).ConfigureAwait(false);

#else
                                                        await Task.Factory.StartNew(async () =>
                                                        {
                                                            await Task.Delay(100);
                                                            ClassParallelForm.HideWaitingFormAsync();
                                                            await Task.Delay(100);

                                                            MethodInvoker invoke =
                                                                () => MessageBox.Show(Program.WalletXiropht,
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactioninvalidamountcontenttext),
                                                                    ClassTranslation.GetLanguageTextFromOrder(
                                                                        ClassTranslationEnumeration
                                                                            .walletnetworkobjectsendtransactioninvalidamounttitletext),
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                            Program.WalletXiropht.BeginInvoke(invoke);
                                                        }).ConfigureAwait(false);

#endif
#if DEBUG
                                                    Log.WriteLine("Transaction refused. You try input an invalid amount.");
#endif
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                busyOrError = true;
                                            }
                                        }
                                        else
                                        {
                                            busyOrError = true;
                                        }
                                    }
                                    else
                                    {
                                        busyOrError = true;
                                    }
                                }
                                else
                                {
                                    busyOrError = true;
                                }
                            }
                            else
                            {
                                busyOrError = true;
                            }
                        }
                        catch (Exception error)
                        {
#if DEBUG
                        Log.WriteLine("SendWalletTokenTransaction exception: " + error.Message);
#endif
                            busyOrError = true;
                        }
                    }
                    else
                    {
                        ListOfSeedNodesSpeed[seedNodeSelected.Item2].TotalError++;
                        if (ListOfSeedNodesSpeed[seedNodeSelected.Item2].TotalError >= WalletMaxSeedNodeError)
                        {
                            ListOfSeedNodesSpeed[seedNodeSelected.Item2].LastBanError =
                                DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                        }

                        busyOrError = true;
                    }
                }
                else
                {
                    busyOrError = true;
                }

                if (busyOrError)
                {
                    ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                    await Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(100);
                        ClassParallelForm.HideWaitingFormAsync();
                        await Task.Delay(100);

                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusycontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.walletnetworkobjectsendtransactionbusytitletext),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }).ConfigureAwait(false);

#else
                    await Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(100);
                        ClassParallelForm.HideWaitingFormAsync();
                        await Task.Delay(100);

                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectsendtransactionbusycontenttext),
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .walletnetworkobjectsendtransactionbusytitletext), MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        Program.WalletXiropht.BeginInvoke(invoke);
                    }).ConfigureAwait(false);

#endif
                }
            }
            else
            {
                ClassParallelForm.HideWaitingFormAsync();
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .createwalleterrorcantconnectmessagecontenttext),
                    string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MethodInvoker invoke = () => MessageBox.Show(Program.WalletXiropht,
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .createwalleterrorcantconnectmessagecontenttext));
                Program.WalletXiropht.BeginInvoke(invoke);
#endif
            }
        }

        /// <summary>
        ///     Get Seed Node list sorted by the faster to the slowest one.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ClassWalletSeedNodeStats> GetSeedNodeSpeedList()
        {
            if (ListOfSeedNodesSpeed == null) ListOfSeedNodesSpeed = new Dictionary<string, ClassWalletSeedNodeStats>();

            if (ListOfSeedNodesSpeed.Count == 0)
            {
                foreach (var seedNode in ClassConnectorSetting.SeedNodeIp.ToArray())
                    try
                    {
                        var seedNodeResponseTime = -1;
                        Task taskCheckSeedNode = Task.Run(() =>
                            seedNodeResponseTime = CheckPing.CheckPingHost(seedNode.Key, true));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxPingDelay);
                        if (seedNodeResponseTime == -1)
                            seedNodeResponseTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect;
#if DEBUG
                        Log.WriteLine(seedNode.Key + " response time: " + seedNodeResponseTime + " ms.");
#endif
                        if (!ListOfSeedNodesSpeed.ContainsKey(seedNode.Key))
                        {
                            ListOfSeedNodesSpeed.Add(seedNode.Key,
                                new ClassWalletSeedNodeStats()
                                    {LastBanError = 0, PingTime = seedNodeResponseTime, TotalError = 0});
                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                        Log.WriteLine("GetSeedNodeSpeedList exception: " + error.Message);
#endif
                        if (!ListOfSeedNodesSpeed.ContainsKey(seedNode.Key))
                        {
                            ListOfSeedNodesSpeed.Add(seedNode.Key, new ClassWalletSeedNodeStats()
                                {
                                    LastBanError = 0,
                                    PingTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect,
                                    TotalError = 0
                                }
                            ); // Max delay.
                        }
                    }
            }
            else if (ListOfSeedNodesSpeed.Count != ClassConnectorSetting.SeedNodeIp.Count)
            {
                var tmpListOfSeedNodesSpeed = new Dictionary<string, ClassWalletSeedNodeStats>();
                foreach (var seedNode in ClassConnectorSetting.SeedNodeIp.ToArray())
                    try
                    {
                        var seedNodeResponseTime = -1;
                        Task taskCheckSeedNode = Task.Run(() =>
                            seedNodeResponseTime = CheckPing.CheckPingHost(seedNode.Key, true));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxPingDelay);
                        if (seedNodeResponseTime == -1)
                            seedNodeResponseTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect;
#if DEBUG
                        Log.WriteLine(seedNode.Key + " response time: " + seedNodeResponseTime + " ms.");
#endif
                        tmpListOfSeedNodesSpeed.Add(seedNode.Key, new ClassWalletSeedNodeStats()
                            {LastBanError = 0, PingTime = seedNodeResponseTime, TotalError = 0});
                    }
                    catch (Exception error)
                    {
#if DEBUG
                        Log.WriteLine("GetSeedNodeSpeedList exception: " + error.Message);
#endif
                        tmpListOfSeedNodesSpeed.Add(seedNode.Key, new ClassWalletSeedNodeStats()
                        {
                            LastBanError = 0,
                            PingTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect,
                            TotalError = 0
                        }); // Max delay.
                    }

                ListOfSeedNodesSpeed = tmpListOfSeedNodesSpeed;
            }

            return ListOfSeedNodesSpeed.ToArray().OrderBy(u => u.Value.PingTime).ToDictionary(z => z.Key, y => y.Value);
        }

        /// <summary>
        ///     Return a seed node functional
        /// </summary>
        /// <returns></returns>
        private Tuple<bool, string> GetSeedNodeAlive()
        {
            var getSeedNodeRandom = string.Empty;
            var seedNodeSelected = false;
            foreach (var seedNode in GetSeedNodeSpeedList().ToArray())
            {
                bool testSeedNode = false;
                if (seedNode.Value.TotalError < WalletMaxSeedNodeError)
                {
                    testSeedNode = true;
                }
                else
                {
                    if (seedNode.Value.LastBanError <= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        testSeedNode = true;
                        ListOfSeedNodesSpeed[seedNode.Key].TotalError = 0;
                    }
                }

                if (testSeedNode)
                {
                    getSeedNodeRandom = seedNode.Key;
                    Task taskCheckSeedNode = Task.Run(async () =>
                        seedNodeSelected =
                            await CheckTcp.CheckTcpClientAsync(seedNode.Key, ClassConnectorSetting.SeedNodeTokenPort));
                    taskCheckSeedNode.Wait(ClassConnectorSetting.MaxTimeoutConnect);
                    if (seedNodeSelected) break;
                }

            }

            return new Tuple<bool, string>(seedNodeSelected, getSeedNodeRandom);
        }

        /// <summary>
        ///     Retrieve the list of remote nodes.
        /// </summary>
        private async Task GetRemoteNodeListAsync()
        {
            if (ListOfSeedNodesSpeed == null)
            {
                ListOfSeedNodesSpeed = GetSeedNodeSpeedList();
            }

            foreach (var seedNode in ListOfSeedNodesSpeed.ToArray())
            {
                try
                {
                    if (seedNode.Value.TotalError < WalletMaxSeedNodeError &&
                        seedNode.Value.LastBanError <= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        var responseWallet = await ProceedTokenRequestHttpAsync(
                            "http://" + seedNode.Key + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                            ClassRpcWalletCommand.TokenAskRemoteNode, 5);
                        if (responseWallet != HttpPacketError)
                        {
                            var nodeListJsonObject =
                                JsonConvert.DeserializeObject<ClassTokenRemoteNode>(responseWallet);
                            if (classTokenRemoteNode == null)
                            {
                                classTokenRemoteNode = new ClassTokenRemoteNode()
                                {
                                    remote_node_list = nodeListJsonObject.remote_node_list,
                                    seed_node_version = nodeListJsonObject.seed_node_version
                                };
                                foreach (var remoteNode in classTokenRemoteNode.remote_node_list)
                                {
                                    ClassPeerList.IncludeNewPeer(remoteNode);
                                }
                            }
                            else
                            {
                                foreach (var node in nodeListJsonObject.remote_node_list)
                                {
                                    if (!classTokenRemoteNode.remote_node_list.Contains(node))
                                    {
                                        ClassPeerList.IncludeNewPeer(node);
                                        classTokenRemoteNode.remote_node_list.Add(node);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ListOfSeedNodesSpeed == null)
                            {
                                ListOfSeedNodesSpeed = GetSeedNodeSpeedList();
                            }

                            ListOfSeedNodesSpeed[seedNode.Key].TotalError++;
                            if (ListOfSeedNodesSpeed[seedNode.Key].TotalError >= WalletMaxSeedNodeError)
                            {
                                ListOfSeedNodesSpeed[seedNode.Key].LastBanError =
                                    DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
#if DEBUG
                    Log.WriteLine("GetRemoteNodeListAsync exception: " + error.Message);
#endif
                }
            }
        }

        /// <summary>
        ///     Proceed token request throught http protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="maxTimeoutSecond"></param>
        /// <returns></returns>
        private async Task<string> ProceedTokenRequestHttpAsync(string url, int maxTimeoutSecond = 30)
        {
            try
            {

                var request = (HttpWebRequest) WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.ConnectionLimit = 65535;
                request.KeepAlive = true;
                request.Timeout = maxTimeoutSecond * 1000;
                request.UserAgent = ClassConnectorSetting.CoinName + " Desktop Wallet - " +
                                    Assembly.GetExecutingAssembly().GetName().Version + "R";
                request.Proxy = null;
                using (var response = (HttpWebResponse) await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (BufferedStream buffer = new BufferedStream(stream))
                {
                    using (StreamReader reader = new StreamReader(buffer))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            catch
            {
                return HttpPacketError;
            }
        }

        /// <summary>
        ///     Enable wallet token sync.
        /// </summary>
        public async Task EnableWalletTokenSync()
        {
            if (!WalletOnUseSync)
            {
                WalletOnUseSync = true;

                InitialisationRemoteNodeTokenSync();
                await Task.Delay(1000);
                if (!WalletClosed && WalletOnUseSync)
                {

                    try
                    {
                        if (Program.WalletXiropht.WalletSyncMode ==
                            ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                        {
                            var remoteNodeSelected = false;
                            if (classTokenRemoteNode != null)
                            {
                                if (classTokenRemoteNode.remote_node_list.Count > 0)
                                {
                                    var remoteNodeAlive = string.Empty;

                                    foreach (var remoteNode in classTokenRemoteNode.remote_node_list)
                                    {
                                        if (!remoteNodeSelected)
                                        {
                                            remoteNodeAlive = remoteNode;
                                            if (ClassPeerList.GetPeerStatus(remoteNodeAlive))
                                            {
                                                try
                                                {
                                                    Program.WalletXiropht.UpdateLabelSyncInformation(
                                                        "Currently on testing connectivity of Public Remote Node host: " +
                                                        remoteNode);

                                                    using (var tcpClient = new TcpClient())
                                                    {
                                                        var connectTask = tcpClient.ConnectAsync(remoteNodeAlive,
                                                            ClassConnectorSetting.RemoteNodePort);
                                                        var connectTaskDelay = Task.Delay(ClassConnectorSetting
                                                            .MaxTimeoutConnectRemoteNode);

                                                        var completedConnectTask =
                                                            await Task.WhenAny(connectTask, connectTaskDelay);
                                                        if (completedConnectTask != connectTask)
                                                        {
                                                            remoteNodeSelected = false;
                                                        }
                                                        else
                                                        {
                                                            remoteNodeSelected = true;
                                                        }

                                                        ClassPeerList.IncludeNewPeer(remoteNodeAlive);
                                                        if (!remoteNodeSelected)
                                                        {
                                                            ClassPeerList.IncrementPeerDisconnect(remoteNodeAlive);
                                                        }
                                                    }


                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                    }


                                    if (remoteNodeSelected)
                                    {
                                        if (!await ConnectToRemoteNodeSyncAsync(remoteNodeAlive))
                                        {
                                            ClassPeerList.IncrementPeerDisconnect(remoteNodeAlive);
                                            DisconnectRemoteNodeTokenSync();
                                            remoteNodeSelected = false;
                                        }
                                        else
                                        {
                                            await ListenRemoteNodeSyncPacketAsync();
                                            SendRemoteNodeSyncPacket();
                                        }
                                    }
                                }
                            }

                            if (!remoteNodeSelected)
                            {
                                var peerNodeSelected = false;
                                if (ClassPeerList.PeerList.Count > 0)
                                {
                                    var peerNodeAlive = string.Empty;
                                    foreach (var peerNode in ClassPeerList.PeerList.ToArray())
                                    {
                                        if (!peerNodeSelected)
                                        {
                                            peerNodeAlive = peerNode.Value.peer_host;
                                            if (ClassPeerList.GetPeerStatus(peerNodeAlive))
                                            {
                                                try
                                                {
                                                    Program.WalletXiropht.UpdateLabelSyncInformation(
                                                        "Currently on testing connectivity of Peer host: " +
                                                        peerNode.Key);

                                                    using (var tcpClient = new TcpClient())
                                                    {
                                                        var connectTask = tcpClient.ConnectAsync(peerNodeAlive,
                                                            ClassConnectorSetting.RemoteNodePort);
                                                        var connectTaskDelay = Task.Delay(ClassConnectorSetting
                                                            .MaxTimeoutConnectRemoteNode);

                                                        var completedConnectTask =
                                                            await Task.WhenAny(connectTask, connectTaskDelay);
                                                        if (completedConnectTask != connectTask)
                                                        {
                                                            peerNodeSelected = false;
                                                        }
                                                        else
                                                        {
                                                            peerNodeSelected = true;
                                                        }

                                                        if (!peerNodeSelected)
                                                        {
                                                            ClassPeerList.IncrementPeerDisconnect(peerNodeAlive);
                                                        }
                                                    }


                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                    }

                                    if (peerNodeSelected)
                                    {
                                        if (!await ConnectToRemoteNodeSyncAsync(peerNodeAlive))
                                        {
                                            ClassPeerList.IncrementPeerDisconnect(peerNodeAlive);
                                            DisconnectRemoteNodeTokenSync();
                                            peerNodeSelected = false;
                                        }
                                        else
                                        {
                                            await ListenRemoteNodeSyncPacketAsync();
                                            SendRemoteNodeSyncPacket();
                                        }
                                    }
                                }

                                if (!peerNodeSelected) // Use seed nodes if public nodes listed or peer list not work.
                                {
                                    var seedNodeAlive = GetSeedNodeAlive();
                                    if (seedNodeAlive.Item1)
                                    {
                                        Program.WalletXiropht.UpdateLabelSyncInformation(
                                            "Currently on testing connectivity of Seed Node host: " +
                                            seedNodeAlive.Item2);

                                        if (!await ConnectToRemoteNodeSyncAsync(seedNodeAlive.Item2))
                                        {
                                            DisconnectRemoteNodeTokenSync();
                                            WalletOnUseSync = false;
                                        }
                                        else
                                        {
                                            await ListenRemoteNodeSyncPacketAsync();
                                            SendRemoteNodeSyncPacket();
                                        }
                                    }
                                    else
                                    {
                                        WalletOnUseSync = false;
                                    }
                                }
                            }
                        }
                        else if (Program.WalletXiropht.WalletSyncMode ==
                                 ClassWalletSyncMode.WALLET_SYNC_DEFAULT)
                        {
                            var seedNodeAlive = GetSeedNodeAlive();
                            if (seedNodeAlive.Item1)
                            {
                                Program.WalletXiropht.UpdateLabelSyncInformation(
                                    "Currently on testing connectivity of Seed Node host: " + seedNodeAlive.Item2);
                                if (!await ConnectToRemoteNodeSyncAsync(seedNodeAlive.Item2))
                                {
                                    DisconnectRemoteNodeTokenSync();
                                    WalletOnUseSync = false;
                                }
                                else
                                {
                                    await ListenRemoteNodeSyncPacketAsync();
                                    SendRemoteNodeSyncPacket();
                                }
                            }
                            else
                            {
                                WalletOnUseSync = false;
                            }
                        }
                        else if (Program.WalletXiropht.WalletSyncMode ==
                                 ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE)
                        {
                            Program.WalletXiropht.UpdateLabelSyncInformation(
                                "Currently on testing connectivity of Manual Node host: " +
                                Program.WalletXiropht.WalletSyncHostname);

                            if (!await ConnectToRemoteNodeSyncAsync(
                                Program.WalletXiropht.WalletSyncHostname))
                            {
                                DisconnectRemoteNodeTokenSync();
                                WalletOnUseSync = false;
                            }
                            else
                            {
                                await ListenRemoteNodeSyncPacketAsync();
                                SendRemoteNodeSyncPacket();
                            }
                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                        Log.WriteLine("EnableWalletTokenSync exception: " + error.Message);
#endif
                        DisconnectRemoteNodeTokenSync();
                        WalletOnUseSync = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Disconnect remote node sync objects.
        /// </summary>
        public void DisconnectRemoteNodeTokenSync()
        {

            try
            {
                if (WalletSyncCancellationToken != null)
                    if (!WalletSyncCancellationToken.IsCancellationRequested)
                        WalletSyncCancellationToken.Cancel();
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("DisconnectRemoteNodeTokenSync exception: " + error.Message);
#endif
            }

            if (ListWalletConnectToRemoteNode != null)
                if (ListWalletConnectToRemoteNode.Count > 0)
                {
                    for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                        if (i < ListWalletConnectToRemoteNode.Count)
                            try
                            {
                                if (ListWalletConnectToRemoteNode[i] != null)
                                {
                                    if (!ListWalletConnectToRemoteNode[i].Disposed)
                                    {
                                        ListWalletConnectToRemoteNode[i].DisconnectRemoteNodeClient();
                                    }

                                }
                            }
                            catch (Exception error)
                            {
#if DEBUG
                                Log.WriteLine("DisconnectRemoteNodeTokenSync exception: " + error.Message);
#endif
                            }

                    ListWalletConnectToRemoteNode.Clear();
                }



            InSyncBlock = false;
            InReceiveTransaction = false;
            InReceiveTransactionAnonymity = false;
            InSyncTransaction = false;
            InSyncTransactionAnonymity = false;

            WalletSyncCancellationToken = new CancellationTokenSource();
        }

        /// <summary>
        ///     Initialize remote node sync objects.
        /// </summary>
        private void InitialisationRemoteNodeTokenSync()
        {
            DisconnectRemoteNodeTokenSync();
            ListWalletConnectToRemoteNode = new List<ClassWalletConnectToRemoteNode>
            {
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectTransaction),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectSupply),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectCirculating),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectFee),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectBlockMined),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectDifficulty),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectRate),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectPendingTransaction),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectAskWalletTransaction),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectAskBlock),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectAskLastBlockFound),
                new ClassWalletConnectToRemoteNode(ClassWalletConnectToRemoteNodeObject
                    .ObjectAskWalletAnonymityTransaction)
            };
        }

        /// <summary>
        ///     Attempt to connect to a remote node target.
        /// </summary>
        /// <param name="nodeTarget"></param>
        /// <returns></returns>
        private async Task<bool> ConnectToRemoteNodeSyncAsync(string nodeTarget)
        {
            try
            {
                if (ClassPeerList.GetPeerStatus(nodeTarget))
                {
                    for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                        if (i < ListWalletConnectToRemoteNode.Count)
                            if (ListWalletConnectToRemoteNode[i] != null)
                                if (!await ListWalletConnectToRemoteNode[i]
                                    .ConnectToRemoteNodeAsync(nodeTarget, ClassConnectorSetting.RemoteNodePort))
                                {
                                    ClassPeerList.IncrementPeerDisconnect(nodeTarget);
                                    return false;
                                }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("ConnectToRemoteNodeSyncAsync exception: " + error.Message);
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Listen packets received from a remote node object sync target.
        /// </summary>
        private async Task ListenRemoteNodeSyncPacketAsync()
        {
            try
            {
                for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                    if (i < ListWalletConnectToRemoteNode.Count)
                    {
                        var i1 = i;
                        await Task.Factory.StartNew(async () =>
                            {
                                string node = ListWalletConnectToRemoteNode[i1].RemoteNodeHost;
                                while (!WalletClosed && WalletOnUseSync)
                                {
                                    try
                                    {

                                        if (ListWalletConnectToRemoteNode[i1] != null)
                                        {
                                            if (ClassPeerList.GetPeerStatus(node))
                                            {
                                                if (ListWalletConnectToRemoteNode[i1].RemoteNodeStatus &&
                                                    !ListWalletConnectToRemoteNode[i1].Disposed)
                                                {
                                                    var packetReceived = await ListWalletConnectToRemoteNode[i1]
                                                        .ListenRemoteNodeNetworkAsync();
                                                    if (packetReceived.Contains("*"))
                                                    {
                                                        var splitPacketReceived =
                                                            packetReceived.Split(new[] {"*"}, StringSplitOptions.None);
                                                        foreach (var packet in splitPacketReceived)
                                                            if (!string.IsNullOrEmpty(packet))
                                                                await HandlePacketRemoteNodeSyncAsync(packet, node);
                                                    }
                                                    else
                                                    {

                                                        await HandlePacketRemoteNodeSyncAsync(
                                                            packetReceived, node);


                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }
                                    catch (Exception error)
                                    {
#if DEBUG
                                        Log.WriteLine("ListenRemoteNodeSyncPacket exception: " + error.Message);
#endif
                                        break;
                                    }
                                }

                                WalletOnUseSync = false;
                            }, WalletSyncCancellationToken.Token, TaskCreationOptions.RunContinuationsAsynchronously,
                            TaskScheduler.Current).ConfigureAwait(false);
                    }
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("ListenRemoteNodeSyncPacket exception: " + error.Message);
#endif
            }
        }

        /// <summary>
        ///     Send automatically packets for ask sync informations to remote node objects.
        /// </summary>
        private void SendRemoteNodeSyncPacket()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                    {
                        bool statusNode = true;
                        while (!WalletClosed && WalletOnUseSync && statusNode)
                        {
                            try
                            {
                                for (var i = 0; i < ListWalletConnectToRemoteNode.Count; i++)
                                    if (i < ListWalletConnectToRemoteNode.Count)
                                    {
                                        if (statusNode)
                                        {
                                            if (ClassPeerList.GetPeerStatus(
                                                ListWalletConnectToRemoteNode[i].RemoteNodeHost))
                                            {
                                                if (ListWalletConnectToRemoteNode[i].RemoteNodeStatus &&
                                                    !ListWalletConnectToRemoteNode[i].Disposed)
                                                {
                                                    if (i != ListWalletConnectToRemoteNode.Count - 1)
                                                    {

                                                        switch (i)
                                                        {
                                                            case 1: // max supply

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 2: // coin circulating

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 3: // total fee

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 4: // block mined

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 5: // difficulty

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 6: // hashrate

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 7: // total pending transaction

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            case 9: // block per id

                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                            default:
                                                                if (!await ListWalletConnectToRemoteNode[i]
                                                                    .SendPacketTypeRemoteNode(WalletConnect.WalletId))
                                                                    statusNode = false;

                                                                break;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (!await ListWalletConnectToRemoteNode[i]
                                                            .SendPacketTypeRemoteNode(WalletConnect.WalletIdAnonymity))
                                                        {
                                                            statusNode = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    statusNode = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                statusNode = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                            }
                            catch (Exception error)
                            {
#if DEBUG
                                Log.WriteLine("SendRemoteNodeSyncPacket exception: " + error.Message);
#endif
                                break;
                            }

                            await Task.Delay(WalletSendPacketToNodeInterval);
                        }

                        WalletOnUseSync = false;
                    }, WalletSyncCancellationToken.Token, TaskCreationOptions.RunContinuationsAsynchronously,
                    TaskScheduler.Current).ConfigureAwait(false);
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("SendRemoteNodeSyncPacket exception: " + error.Message);
#endif
            }
        }

        /// <summary>
        ///     Handle packet of sync receive from remote node objects.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private async Task<bool> HandlePacketRemoteNodeSyncAsync(string packet, string node)
        {
            if (!WalletClosed && WalletOnUseSync)
                try
                {
                    if (!string.IsNullOrEmpty(packet))
                    {
                        var splitPacket = packet.Split(new[] {"|"}, StringSplitOptions.None);

                        if (splitPacket.Length > 0)
                        {
#if DEBUG
                            Log.WriteLine("Packet of sync received: " + packet);
#endif
                            switch (splitPacket[0])
                            {
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeCoinMaxSupply:

                                    #region Receive the current max coin supply of the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckMaxSupply, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    CoinMaxSupply = splitPacket[1];

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeCoinCirculating:

                                    #region Receive the current amount of coins circulating on the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckCurrentCirculating, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    CoinCirculating = splitPacket[1];

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeCurrentDifficulty:

                                    #region Receive the current mining difficulty of the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckNetworkDifficulty, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    NetworkDifficulty = splitPacket[1];

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeCurrentRate:

                                    #region Receive the current mining hashrate of the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckNetworkHashrate, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    NetworkHashrate = splitPacket[1];

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeTotalBlockMined:

                                    #region Receive the total amount of blocks mined to sync.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckTotalBlockMined, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    TotalBlockMined = splitPacket[1];

                                    if (!Program.WalletXiropht.EnableUpdateBlockWallet)
                                        Program.WalletXiropht.StartUpdateBlockSync();

                                    if (!InSyncBlock)
                                    {
                                        var tryParseBlockMined =
                                            int.TryParse(splitPacket[1], out var totalBlockOfNetwork);

                                        if (!tryParseBlockMined) return false;

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
                                                await Task.Factory.StartNew(async () =>
                                                    {
                                                        for (var i = totalBlockInWallet; i < totalBlockOfNetwork; i++)
                                                        {
                                                            if (WalletClosed) break;

                                                            InReceiveBlock = true;
#if DEBUG
                                                    Log.WriteLine("Ask block id: " + i);
#endif

                                                            if (!await ListWalletConnectToRemoteNode[9]
                                                                .SendPacketRemoteNodeAsync(
                                                                    ClassRemoteNodeCommandForWallet
                                                                        .RemoteNodeSendPacketEnumeration
                                                                        .AskBlockPerId + "|" + WalletConnect.WalletId +
                                                                    "|" + i
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

                                                                if (WalletClosed) break;

                                                                await Task.Delay(10);
                                                            }
                                                        }

                                                        InSyncBlock = false;
                                                        InReceiveBlock = false;
                                                    }, WalletSyncCancellationToken.Token,
                                                    TaskCreationOptions.LongRunning,
                                                    TaskScheduler.Current).ConfigureAwait(false);
                                            }
                                            else
                                            {

                                                InSyncBlock = false;
                                                InReceiveBlock = false;
                                            }
                                        }
                                        else
                                        {
                                            if (int.TryParse(TotalBlockMined, out var totalBlockMined))
                                                if (totalBlockInWallet - 1 > totalBlockMined)
                                                {
                                                    if (ClassConnectorSetting.SeedNodeIp.ContainsKey(node))
                                                    {
                                                        Program.WalletXiropht.StopUpdateBlockHistory(false, false);
                                                        ClassBlockCache.RemoveWalletBlockCache();
                                                    }
                                                    else
                                                    {
                                                        ClassPeerList.IncrementPeerDisconnect(node);

                                                    }

                                                    return false;
                                                }
                                        }
                                    }

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeTotalFee:

                                    #region Receive the total amount of fee accumulated in the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckTotalTransactionFee, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    TotalFee = splitPacket[1];

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeTotalPendingTransaction:

                                    #region Receive the total amount of transaction in pending on the network.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckTotalPendingTransaction,
                                                splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();
                                    if (int.TryParse(splitPacket[1], out var totalPendingTransaction))
                                        RemoteNodeTotalPendingTransactionInNetwork = totalPendingTransaction;

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .WalletYourNumberTransaction:

                                    #region Receive total transaction to sync.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckWalletTotalTransaction,
                                                splitPacket[1] + "|" + WalletConnect.WalletId))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

                                    if (BlockTransactionSync) return true;

                                    if (!ClassWalletTransactionAnonymityCache.OnLoad &&
                                        !ClassWalletTransactionCache.OnLoad)
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
                                                            .WalletYourNumberTransaction, ""),
                                                out var totalTransactionOfWallet))
                                            {
                                                var totalTransactionInWallet =
                                                    ClassWalletTransactionCache.ListTransaction.Count;

                                                TotalTransactionInSync = totalTransactionOfWallet;

                                                if (totalTransactionInWallet > TotalTransactionInSync)
                                                {
                                                    if (ClassConnectorSetting.SeedNodeIp.ContainsKey(
                                                        ListWalletConnectToRemoteNode[0].RemoteNodeHost))
                                                    {
                                                        ClassWalletTransactionCache.RemoveWalletCache(WalletConnect
                                                            .WalletAddress);
                                                        Program.WalletXiropht
                                                            .StopUpdateTransactionHistory(false, false);
                                                        totalTransactionInWallet = 0;
                                                    }
                                                    else
                                                    {

                                                        ClassPeerList.IncrementPeerDisconnect(node);
                                                        return false;
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
                                                    await Task.Factory.StartNew(async () =>
                                                            {
                                                                try
                                                                {
                                                                    for (var i = totalTransactionInWallet;
                                                                        i < totalTransactionOfWallet;
                                                                        i++)
                                                                    {
                                                                        if (WalletClosed) break;

                                                                        var dateRequestTransaction =
                                                                            ClassUtils.DateUnixTimeNowSecond();
#if DEBUG
                                                            Log.WriteLine("Ask transaction id: " + i);
#endif
                                                                        InReceiveTransaction = true;
                                                                        try
                                                                        {
                                                                            if (!await ListWalletConnectToRemoteNode[8]
                                                                                .SendPacketRemoteNodeAsync(
                                                                                    ClassRemoteNodeCommandForWallet
                                                                                        .RemoteNodeSendPacketEnumeration
                                                                                        .WalletAskTransactionPerId +
                                                                                    "|" +
                                                                                    WalletConnect.WalletId +
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
                                                                            if (!InSyncTransaction || WalletClosed ||
                                                                                BlockTransactionSync ||
                                                                                dateRequestTransaction +
                                                                                ClassConnectorSetting
                                                                                    .MaxTimeoutConnect / 1000 <
                                                                                ClassUtils.DateUnixTimeNowSecond())
                                                                            {
                                                                                if (!WalletClosed)
                                                                                    if (!ClassConnectorSetting
                                                                                        .SeedNodeIp
                                                                                        .ContainsKey(
                                                                                            ListWalletConnectToRemoteNode
                                                                                                    [8]
                                                                                                .RemoteNodeHost))
                                                                                    {

                                                                                        ClassPeerList
                                                                                            .IncrementPeerDisconnect(
                                                                                                node);
                                                                                    }

                                                                                break;
                                                                            }

                                                                            if (WalletClosed) break;

                                                                            await Task.Delay(10);
                                                                        }

                                                                        if (BlockTransactionSync) break;
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
                                                            }, WalletSyncCancellationToken.Token,
                                                            TaskCreationOptions.LongRunning, TaskScheduler.Current)
                                                        .ConfigureAwait(false);
                                                }
                                                else
                                                {
                                                    InSyncTransaction = false;
                                                    InReceiveTransaction = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (TotalTransactionInSync ==
                                                ClassWalletTransactionCache.ListTransaction.Count)
                                                InSyncTransaction = false;
                                        }
                                    }

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .WalletYourAnonymityNumberTransaction:

                                    #region Receive total anonymous transaction to sync.

                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckWalletTotalTransaction,
                                                splitPacket[1] + "|" + WalletConnect.WalletIdAnonymity))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

                                    if (BlockTransactionSync) return true;

                                    if (!InSyncTransaction && !ClassWalletTransactionAnonymityCache.OnLoad &&
                                        !ClassWalletTransactionCache.OnLoad)
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
                                                    if (ClassConnectorSetting.SeedNodeIp.ContainsKey(node))
                                                    {
                                                        ClassWalletTransactionAnonymityCache.RemoveWalletCache(
                                                            WalletConnect
                                                                .WalletAddress);
                                                        Program.WalletXiropht
                                                            .StopUpdateTransactionHistory(false, false);
                                                        totalTransactionInWallet = 0;
                                                    }
                                                    else
                                                    {

                                                        ClassPeerList.IncrementPeerDisconnect(node);
                                                        return false;
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

                                                    await Task.Factory.StartNew(async () =>
                                                            {
                                                                for (var i = totalTransactionInWallet;
                                                                    i < totalTransactionOfWallet;
                                                                    i++)
                                                                {
                                                                    if (WalletClosed) break;

                                                                    var dateRequestTransaction =
                                                                        ClassUtils.DateUnixTimeNowSecond();
#if DEBUG
                                                        Log.WriteLine("Ask anonymity transaction id: " + i);
#endif
                                                                    InReceiveTransactionAnonymity = true;

                                                                    if (!await ListWalletConnectToRemoteNode[8]
                                                                        .SendPacketRemoteNodeAsync(
                                                                            ClassRemoteNodeCommandForWallet
                                                                                .RemoteNodeSendPacketEnumeration
                                                                                .WalletAskAnonymityTransactionPerId +
                                                                            "|" +
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
                                                                        if (!InSyncTransactionAnonymity ||
                                                                            WalletClosed ||
                                                                            BlockTransactionSync ||
                                                                            dateRequestTransaction +
                                                                            ClassConnectorSetting.MaxTimeoutConnect /
                                                                            1000 <
                                                                            ClassUtils.DateUnixTimeNowSecond())
                                                                        {
                                                                            if (!WalletClosed)
                                                                            {
                                                                                if (!ClassConnectorSetting.SeedNodeIp
                                                                                    .ContainsKey(
                                                                                        node))
                                                                                {

                                                                                    ClassPeerList
                                                                                        .IncrementPeerDisconnect(node);
                                                                                }
                                                                            }

                                                                            break;
                                                                        }

                                                                        await Task.Delay(10);
                                                                        if (WalletClosed) break;
                                                                    }

                                                                    if (BlockTransactionSync) break;
                                                                }

                                                                InSyncTransactionAnonymity = false;
                                                                InReceiveTransactionAnonymity = false;
                                                            }, WalletSyncCancellationToken.Token,
                                                            TaskCreationOptions.LongRunning, TaskScheduler.Current)
                                                        .ConfigureAwait(false);
                                                }
                                                else
                                                {
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
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeLastBlockFoundTimestamp:

                                    #region Receive last block found date.

#if DEBUG
                                    Log.WriteLine("Last block found date: " + splitPacket[1]
                                                      .Replace(
                                                          ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                              .SendRemoteNodeLastBlockFoundTimestamp, ""));
#endif
                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (!await CheckRemoteNodeInformationAsync(
                                                ClassRpcWalletCommand.TokenCheckLastBlockFoundDate, splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }
                                    }

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
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .SendRemoteNodeBlockPerId:



                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            var splitBlock = splitPacket[1].Split(new[] {"#"}, StringSplitOptions.None);
                                            if (long.TryParse(splitBlock[0], out _))
                                            {
                                                if (await CheckRemoteNodeInformationByCompareAsync(
                                                        ClassRpcWalletCommand.TokenCheckBlock, splitBlock[0]) !=
                                                    splitPacket[1])
                                                {

                                                    ClassPeerList.IncrementPeerDisconnect(node);
                                                    return false;
                                                }
                                            }
                                            else
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }

                                    }

                                    #region Receive block sync by ID.

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();

#if DEBUG
                                    Log.WriteLine("Block received: " + splitPacket[1].Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.SendRemoteNodeBlockPerId, ""));
#endif


                                    var blockLine = splitPacket[1]
                                        .Replace(
                                            ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                .SendRemoteNodeBlockPerId, "")
                                        .Split(new[] {"#"}, StringSplitOptions.None);

                                    if (!ClassBlockCache.ListBlock.ContainsKey(blockLine[1]))
                                    {
                                        var blockObject = new ClassBlockObject
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
                                        await ClassBlockCache.SaveWalletBlockCache(splitPacket[1]
                                            .Replace(
                                                ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                                    .SendRemoteNodeBlockPerId, ""));

                                        _lastBlockReceived = ClassUtils.DateUnixTimeNowSecond();
                                        InReceiveBlock = false;
                                    }

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .WalletTransactionPerId:

                                    #region Receive transaction by ID.

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();


                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (await CheckRemoteNodeInformationByCompareAsync(
                                                    ClassRpcWalletCommand.TokenCheckTransaction,
                                                    WalletConnect.WalletId + "|" +
                                                    ClassWalletTransactionCache.ListTransaction.Count) !=
                                                ClassUtils.ConvertStringtoSHA512(splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }
                                    }

                                    await ClassWalletTransactionCache.AddWalletTransactionAsync(splitPacket[1],
                                        node);

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                    .WalletAnonymityTransactionPerId:

                                    #region Receive anonymous transaction sync by ID.

                                    LastRemoteNodePacketReceived = ClassUtils.DateUnixTimeNowSecond();


                                    if (Program.WalletXiropht.WalletSyncMode ==
                                        ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE)
                                    {
                                        if (!ClassPeerList.GetPeerTrustStatus(node))
                                        {
                                            if (await CheckRemoteNodeInformationByCompareAsync(
                                                    ClassRpcWalletCommand.TokenCheckTransaction,
                                                    WalletConnect.WalletIdAnonymity + "|" +
                                                    ClassWalletTransactionAnonymityCache.ListTransaction
                                                        .Count) !=
                                                ClassUtils.ConvertStringtoSHA512(splitPacket[1]))
                                            {

                                                ClassPeerList.IncrementPeerDisconnect(node);
                                                return false;
                                            }

                                            ClassPeerList.IncrementPeerTrustPoint(node);
                                        }
                                    }

                                    await ClassWalletTransactionAnonymityCache.AddWalletTransactionAsync(
                                        splitPacket[1], node);

                                    #endregion

                                    break;
                                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                        .SendRemoteNodeKeepAlive
                                    : // This is a valid packet, but we don't take in count for update the datetime of the last packet received, only important packets update the datetime.
                                    break;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
#if DEBUG
                    Log.WriteLine("HandlePacketRemoteNodeSyncAsync exception: " + error.Message);
#endif
                    return false;
                }

            return true;
        }

        /// <summary>
        ///     Permit to ask seed nodes for check an information receive from a remote node or from a peer.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="information"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private async Task<bool> CheckRemoteNodeInformationAsync(string type, string information, int timeout = 5)
        {


            foreach (var seedNode in ListOfSeedNodesSpeed.ToArray())
            {
                if (seedNode.Value.TotalError < WalletMaxSeedNodeError &&
                    seedNode.Value.LastBanError <= DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    try
                    {
                        string responseWallet = await ProceedTokenRequestHttpAsync(
                            "http://" + seedNode.Key + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                            type + "|" + information, timeout);

                        if (responseWallet != HttpPacketError)
                        {
                            var resultJsonObject = JObject.Parse(responseWallet);
                            switch (resultJsonObject["result"].ToString())
                            {
                                case ClassRpcWalletCommand.SendTokenValidInformation:
                                    return true;
                                case ClassRpcWalletCommand.SendTokenInvalidInformation:
                                    return false;
                            }
                        }
                        else
                        {
                            if (ListOfSeedNodesSpeed.ContainsKey(seedNode.Key))
                            {
                                ListOfSeedNodesSpeed[seedNode.Key].TotalError++;
                                if (ListOfSeedNodesSpeed[seedNode.Key].TotalError >= WalletMaxSeedNodeError)
                                {
                                    ListOfSeedNodesSpeed[seedNode.Key].LastBanError =
                                        DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                                }
                            }

                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                    Log.WriteLine("CheckRemoteNodeInformationAsync exception: " + error.Message);
#endif
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Permit to ask seed nodes for check an information receive from a remote node or from a peer.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="information"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private async Task<string> CheckRemoteNodeInformationByCompareAsync(string type, string information,
            int timeout = 5)
        {
            foreach (var seedNode in ListOfSeedNodesSpeed.ToArray())
            {
                if (seedNode.Value.TotalError < WalletMaxSeedNodeError &&
                    seedNode.Value.LastBanError <= DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    try
                    {
                        string result = await ProceedTokenRequestHttpAsync(
                            "http://" + seedNode.Key + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" +
                            type + "|" +
                            information, timeout);

                        if (result != HttpPacketError)
                        {
                            var resultJsonObject = JObject.Parse(result);

                            return resultJsonObject["result"].ToString();
                        }

                        if (ListOfSeedNodesSpeed.ContainsKey(seedNode.Key))
                        {
                            ListOfSeedNodesSpeed[seedNode.Key].TotalError++;
                            if (ListOfSeedNodesSpeed[seedNode.Key].TotalError >= WalletMaxSeedNodeError)
                            {
                                ListOfSeedNodesSpeed[seedNode.Key].LastBanError =
                                    DateTimeOffset.Now.ToUnixTimeSeconds() + WalletCleanSeedNodeError;
                            }
                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                    Log.WriteLine("CheckRemoteNodeInformationAsync exception: " + error.Message);
#endif
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///     Stop cancellation token.
        /// </summary>
        private void EndCancellationTokenSource()
        {
            try
            {
                if (WalletCancellationToken != null)
                    if (!WalletCancellationToken.IsCancellationRequested)
                        WalletCancellationToken.Cancel();
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("EndCancellationTokenSource exception: " + error.Message);
#endif
            }

            try
            {
                if (WalletSyncCancellationToken != null)
                    if (!WalletSyncCancellationToken.IsCancellationRequested)
                        WalletSyncCancellationToken.Cancel();
            }
            catch (Exception error)
            {
#if DEBUG
                Log.WriteLine("EndCancellationTokenSource exception: " + error.Message);
#endif
            }
        }

        #endregion
    }
}