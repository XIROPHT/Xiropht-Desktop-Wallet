using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Wallet.Features;
using Xiropht_Wallet.Utility;
using Xiropht_Wallet.Wallet.Sync.Object;
#if DEBUG
using Xiropht_Wallet.Debug;
#endif
namespace Xiropht_Wallet.Wallet.Sync
{
    public class ClassWalletTransactionCache
    {
        private const string WalletTransMethodInvokerCacheDirectory = "/Cache/";
        private const string WalletTransMethodInvokerCacheFileExtension = "transaction.xirtra";
        private static bool _inClearCache;

        public static Dictionary<string, ClassWalletTransactionObject> ListTransaction =
            new Dictionary<string, ClassWalletTransactionObject>(); // hash, transaction object

        public static bool OnLoad;

        /// <summary>
        ///     Load transMethodInvoker in cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static void LoadWalletCache(string walletAddress)
        {
            if (ListTransaction != null)
                ListTransaction.Clear();
            else
                ListTransaction = new Dictionary<string, ClassWalletTransactionObject>();
            if (!string.IsNullOrEmpty(walletAddress))
            {
                OnLoad = true;
                try
                {
                    Task.Factory.StartNew(() =>
                        {
                            if (Directory.Exists(
                                ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                         WalletTransMethodInvokerCacheDirectory + walletAddress +
                                                         "\\")))
                            {
                                if (File.Exists(ClassUtility.ConvertPath(
                                    AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                    walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                                {
                                    byte[] AesKeyIv = null;
                                    byte[] AesSalt = null;
                                    using (var password = new PasswordDeriveBytes(
                                        Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress +
                                        Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey,
                                        ClassUtils.GetByteArrayFromString(ClassUtils.FromHex(
                                            (Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress +
                                             Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey)
                                            .Substring(0, 8)))))
                                    {
                                        AesKeyIv = password.GetBytes(
                                            ClassConnectorSetting.MAJOR_UPDATE_1_SECURITY_CERTIFICATE_SIZE / 8);
                                        AesSalt = password.GetBytes(16);
                                    }

                                    var listTransactionEncrypted = new List<string>();
                                    using (var fs = File.Open(
                                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                 WalletTransMethodInvokerCacheDirectory +
                                                                 walletAddress +
                                                                 "\\" + WalletTransMethodInvokerCacheFileExtension),
                                        FileMode.Open, FileAccess.Read, FileShare.Read))
                                    using (var bs = new BufferedStream(fs))
                                    using (var sr = new StreamReader(bs))
                                    {
                                        string line;
                                        while ((line = sr.ReadLine()) != null) listTransactionEncrypted.Add(line);
                                    }

                                    if (listTransactionEncrypted.Count > 0)
                                    {
                                        var line = 0;
                                        foreach (var transactionEncrypted in listTransactionEncrypted)
                                        {
                                            line++;
                                            var walletTransactionDecrypted =
                                                ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                                                    transactionEncrypted, ClassWalletNetworkSetting.KeySize, AesKeyIv,
                                                    AesSalt);
                                            if (walletTransactionDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                                            {
                                                var splitTransaction =
                                                    walletTransactionDecrypted.Split(new[] {"#"},
                                                        StringSplitOptions.None);
                                                if (!ListTransaction.ContainsKey(splitTransaction[1]))
                                                {
                                                    var splitBlockchainHeight = splitTransaction[7]
                                                        .Split(new[] {"~"}, StringSplitOptions.None);

                                                    var transactionObject = new ClassWalletTransactionObject
                                                    {
                                                        TransactionType = splitTransaction[0],
                                                        TransactionHash = splitTransaction[1],
                                                        TransactionWalletAddress = splitTransaction[2],
                                                        TransactionAmount =
                                                            decimal.Parse(
                                                                splitTransaction[3].ToString(Program.GlobalCultureInfo),
                                                                NumberStyles.Currency, Program.GlobalCultureInfo),
                                                        TransactionFee = decimal.Parse(
                                                            splitTransaction[4].ToString(Program.GlobalCultureInfo),
                                                            NumberStyles.Currency, Program.GlobalCultureInfo),
                                                        TransactionTimestampSend = long.Parse(splitTransaction[5]),
                                                        TransactionTimestampRecv = long.Parse(splitTransaction[6]),
                                                        TransactionBlockchainHeight =
                                                            splitBlockchainHeight[0].Replace("{", "")
                                                    };
                                                    ListTransaction.Add(splitTransaction[1], transactionObject);

                                                    Program.WalletXiropht.UpdateLabelSyncInformation(
                                                        "On load transaction database - total transactions loaded and decrypted: " +
                                                        (ListTransaction.Count + ClassWalletTransactionAnonymityCache
                                                             .ListTransaction.Count));
                                                }
#if DEBUG
                                            else
                                            {
                                                Log.WriteLine("Duplicate anonymous transaction: " + walletTransactionDecrypted);
                                            }
#endif
                                            }

#if DEBUG
                                        else
                                        {
                                            Log.WriteLine("Wrong transaction at line: " + line);
                                        }
#endif
                                        }
                                    }

                                    listTransactionEncrypted.Clear();
                                    Program.WalletXiropht.ClassWalletObject.TotalTransactionInSync =
                                        ListTransaction.Count;
                                    AesKeyIv = null;
                                    AesSalt = null;
                                }
                                else
                                {
                                    File.Create(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                        walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                                }
                            }
                            else
                            {
                                if (Directory.Exists(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory +
                                        WalletTransMethodInvokerCacheDirectory)) ==
                                    false)
                                    Directory.CreateDirectory(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory +
                                        WalletTransMethodInvokerCacheDirectory));

                                Directory.CreateDirectory(ClassUtility.ConvertPath(
                                    AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                    walletAddress));
                            }

                            Program.WalletXiropht.ClassWalletObject.TotalTransactionInSync = ListTransaction.Count;
                            OnLoad = false;
                        }, Program.WalletXiropht.WalletCancellationToken.Token,
                        TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {
                    Program.WalletXiropht.ClassWalletObject.TotalTransactionInSync = 0;
                    ListTransaction.Clear();
                    OnLoad = false;
                }
            }
        }

        /// <summary>
        ///     Save each transMethodInvoker into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="transaction"></param>
        public static async Task SaveWalletCache(string walletAddress, string transaction)
        {
            if (!string.IsNullOrEmpty(walletAddress))
                if (ListTransaction.Count > 0 && _inClearCache == false)
                {
                    if (Directory.Exists(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory)) ==
                        false)
                        Directory.CreateDirectory(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory));

                    if (Directory.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                  WalletTransMethodInvokerCacheDirectory +
                                                                  walletAddress)) == false)
                        Directory.CreateDirectory(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                            walletAddress));


                    if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                              WalletTransMethodInvokerCacheDirectory +
                                                              walletAddress +
                                                              "\\" + WalletTransMethodInvokerCacheFileExtension)))
                        File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransMethodInvokerCacheDirectory + walletAddress +
                                                             "\\" + WalletTransMethodInvokerCacheFileExtension))
                            .Close();
                    try
                    {
                        using (var transMethodInvokerFile = new StreamWriter(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                            walletAddress +
                            "\\" + WalletTransMethodInvokerCacheFileExtension), true))
                        {
                            await transMethodInvokerFile.WriteAsync(transaction + "\n").ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
        }

        /// <summary>
        ///     Clear each transMethodInvoker into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static bool RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (!string.IsNullOrEmpty(walletAddress))
            {
                if (Directory.Exists(
                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                             WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
                {
                    if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransMethodInvokerCacheDirectory + walletAddress +
                                                             "\\" + WalletTransMethodInvokerCacheFileExtension)))
                        File.Delete(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransMethodInvokerCacheDirectory + walletAddress +
                                                             "\\" + WalletTransMethodInvokerCacheFileExtension));

                    Directory.Delete(
                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                 WalletTransMethodInvokerCacheDirectory + walletAddress + "\\"), true);
                    Directory.CreateDirectory(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                       WalletTransMethodInvokerCacheDirectory +
                                                                       walletAddress));
                }

                ListTransaction.Clear();
            }

            _inClearCache = false;
            return true;
        }

        /// <summary>
        ///     Clear wallet transaction cache.
        /// </summary>
        public static void ClearWalletCache()
        {
            try
            {
                ListTransaction.Clear();
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Add transaction to the list.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="node"></param>
        public static async Task AddWalletTransactionAsync(string transaction, string node)
        {
#if DEBUG
                Log.WriteLine("Wallet transaction history received: " + transaction
                                       .Replace(
                                           ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                               .WalletTransactionPerId, ""));
#endif
            bool errorSyncTransaction = false;

            if (!OnLoad)
            {
                var splitTransaction = transaction
                    .Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId,
                        string.Empty).Split(new[] {"#"}, StringSplitOptions.None);
                var hashTransaction = splitTransaction[4]; // Transaction Hash.
                if (!ListTransaction.ContainsKey(hashTransaction))
                {
                    var type = splitTransaction[0];
                    var timestamp = splitTransaction[3]; // Timestamp Send CEST.
                    var realFeeAmountSend = splitTransaction[7]; // Real fee and amount crypted for sender.
                    var realFeeAmountRecv = splitTransaction[8]; // Real fee and amount crypted for sender.

                    var amountAndFeeDecrypted = ClassAlgoErrorEnumeration.AlgoError;
                    if (type == "SEND")
                        amountAndFeeDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountSend,
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES
                    else if (type == "RECV")
                        amountAndFeeDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountRecv,
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES

                    if (amountAndFeeDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                    {
                        var splitDecryptedAmountAndFee =
                            amountAndFeeDecrypted.Split(new[] {"-"}, StringSplitOptions.None);
                        var amountDecrypted = splitDecryptedAmountAndFee[0];
                        var feeDecrypted = splitDecryptedAmountAndFee[1];
                        var walletDstOrSrc = splitDecryptedAmountAndFee[2];


                        var timestampRecv = splitTransaction[5]; // Timestamp Recv CEST.
                        var blockchainHeight = splitTransaction[6]; // Blockchain height.

                        var finalTransaction = type + "#" + hashTransaction + "#" + walletDstOrSrc + "#" +
                                               amountDecrypted + "#" + feeDecrypted + "#" + timestamp + "#" +
                                               timestampRecv + "#" + blockchainHeight;

                        var finalTransactionEncrypted = ClassAlgo.GetEncryptedResultManual(
                            ClassAlgoEnumeration.Rijndael, finalTransaction,
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES

                        if (finalTransactionEncrypted == ClassAlgoErrorEnumeration.AlgoError) // Ban bad remote node.
                        {
                            errorSyncTransaction = true;
                        }
                        else
                        {
                            var splitBlockchainHeight = blockchainHeight.Split(new[] {"~"}, StringSplitOptions.None);

                            var transactionObject = new ClassWalletTransactionObject
                            {
                                TransactionType = type,
                                TransactionHash = hashTransaction,
                                TransactionWalletAddress = walletDstOrSrc,
                                TransactionAmount = decimal.Parse(amountDecrypted.ToString(Program.GlobalCultureInfo),
                                    NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionFee = decimal.Parse(feeDecrypted.ToString(Program.GlobalCultureInfo),
                                    NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionTimestampSend = long.Parse(timestamp),
                                TransactionTimestampRecv = long.Parse(timestampRecv),
                                TransactionBlockchainHeight = splitBlockchainHeight[0].Replace("{", "")
                            };

                            ListTransaction.Add(hashTransaction, transactionObject);


                            await SaveWalletCache(Program.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress, finalTransactionEncrypted);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               Program.WalletXiropht.ClassWalletObject.TotalTransactionInSync + ".");
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                    Log.WriteLine("Can't decrypt transaction: " + transaction + " result: " +
                                  amountAndFeeDecrypted);
#endif
                        errorSyncTransaction = true;
                    }
                }
                else
                {
#if DEBUG
                    Log.WriteLine("Wallet transaction hash: "+ hashTransaction+" already exist on database.");
#endif
                    errorSyncTransaction = true;
                }
            }

            if (errorSyncTransaction)
            {
                if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(node))
                {
                    if (!Program.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.ContainsKey(node))
                    {
                        Program.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.Add(node,
                            ClassUtils.DateUnixTimeNowSecond());
                        ClassPeerList.IncrementPeerDisconnect(node);
                    }
                    else
                    {
                        Program.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned[node] =
                            ClassUtils.DateUnixTimeNowSecond();
                        ClassPeerList.IncrementPeerDisconnect(node);
                    }
                }


                await Program.WalletXiropht.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                Program.WalletXiropht.ClassWalletObject.WalletOnUseSync = false;

            }

            Program.WalletXiropht.ClassWalletObject.InReceiveTransaction = false;
        }
    }
}