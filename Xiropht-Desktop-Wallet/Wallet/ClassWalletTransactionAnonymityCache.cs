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
using Xiropht_Wallet.FormPhase;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionAnonymityCache
    {
        private const string WalletTransactionCacheDirectory = "/Cache/";
        private const string WalletTransactionCacheFileExtension = "transaction.xirtra";
        private static bool _inClearCache;
        public static bool OnLoad;
        public static Dictionary<string, ClassWalletTransactionObject> ListTransaction = new Dictionary<string, ClassWalletTransactionObject>(); // hash, transaction object

        /// <summary>
        /// Load transaction in cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static void LoadWalletCache(string walletAddress)
        {
            if (ListTransaction != null)
            {
                ListTransaction.Clear();
            }
            else
            {
                ListTransaction = new Dictionary<string, ClassWalletTransactionObject>();
            }
            if (!string.IsNullOrEmpty(walletAddress))
            {
                OnLoad = true;
                try
                {
                    Task.Factory.StartNew(() =>
                    {

                        walletAddress += "ANONYMITY";

                        if (Directory.Exists(
                        ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\")))
                        {
                            if (File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension)))
                            {
                                byte[] AesKeyIv = null;
                                byte[] AesSalt = null;
                                using (PasswordDeriveBytes password = new PasswordDeriveBytes(ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress + ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey, ClassUtils.GetByteArrayFromString(ClassUtils.FromHex((ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress + ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey).Substring(0, 8)))))
                                {
                                    AesKeyIv = password.GetBytes(ClassConnectorSetting.MAJOR_UPDATE_1_SECURITY_CERTIFICATE_SIZE / 8);
                                    AesSalt = password.GetBytes(16);
                                }
                                var listTransactionEncrypted = new List<string>();

                                using (FileStream fs = File.Open(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.Read))
                                using (BufferedStream bs = new BufferedStream(fs))
                                using (StreamReader sr = new StreamReader(bs))
                                {
                                    string line;
                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        listTransactionEncrypted.Add(line);
                                    }
                                }
                                if (listTransactionEncrypted.Count > 0)
                                {
                                    int line = 0;

                                    foreach (var transactionEncrypted in listTransactionEncrypted)
                                    {
                                        line++;

                                        OnLoad = true;

                                        string walletTransactionDecrypted = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, transactionEncrypted, ClassWalletNetworkSetting.KeySize, AesKeyIv, AesSalt);
                                        if (walletTransactionDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                                        {
                                            string[] splitTransaction = walletTransactionDecrypted.Split(new[] { "#" }, StringSplitOptions.None);
                                            if (!ListTransaction.ContainsKey(splitTransaction[1]))
                                            {
                                                string[] splitBlockchainHeight = splitTransaction[7].Split(new[] { "~" }, StringSplitOptions.None);
                                                ClassWalletTransactionObject transactionObject = new ClassWalletTransactionObject
                                                {
                                                    TransactionType = splitTransaction[0],
                                                    TransactionHash = splitTransaction[1],
                                                    TransactionWalletAddress = splitTransaction[2],
                                                    TransactionAmount = decimal.Parse(splitTransaction[3].ToString(Program.GlobalCultureInfo), NumberStyles.Currency, Program.GlobalCultureInfo),
                                                    TransactionFee = decimal.Parse(splitTransaction[4].ToString(Program.GlobalCultureInfo), NumberStyles.Currency, Program.GlobalCultureInfo),
                                                    TransactionTimestampSend = long.Parse(splitTransaction[5]),
                                                    TransactionTimestampRecv = long.Parse(splitTransaction[6]),
                                                    TransactionBlockchainHeight = splitBlockchainHeight[0].Replace("{", "")
                                                };
                                                ListTransaction.Add(splitTransaction[1], transactionObject);
                                                ClassFormPhase.WalletXiropht.UpdateLabelSyncInformation("On load transaction database - total transactions loaded and decrypted: " + (ClassWalletTransactionCache.ListTransaction.Count + ClassWalletTransactionAnonymityCache.ListTransaction.Count));
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
                                            Log.WriteLine("Wrong anonymous transaction at line: " + line);
                                        }
#endif
                                    }
                                }
                                ClassFormPhase.WalletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity = ListTransaction.Count;
                                listTransactionEncrypted.Clear();
                                AesKeyIv = null;
                                AesSalt = null;
                            }
                            else
                            {
                                File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension)).Close();
                            }

                        }
                        else
                        {
                            if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory)) == false)
                            {
                                Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory));
                            }

                            Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                                  walletAddress));
                        }


                        ClassFormPhase.WalletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity = ListTransaction.Count;
                        OnLoad = false;

                    }, ClassFormPhase.WalletXiropht.ClassWalletObject.WalletCancellationToken.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {
                    ClassFormPhase.WalletXiropht.ClassWalletObject.TotalTransactionInSyncAnonymity = 0;
                    ListTransaction.Clear();
                    OnLoad = false;
                }
            }
        }

        /// <summary>
        /// Save each transaction into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        public static async Task SaveWalletCacheAsync(string walletAddress, string transaction)
        {
            if (!string.IsNullOrEmpty(walletAddress))
            {
                walletAddress += "ANONYMITY";
                if (ListTransaction.Count > 0 && _inClearCache == false)
                {
                    if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory)) == false)
                    {
                        Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory));
                    }

                    if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                         walletAddress)) == false)
                    {
                        Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                                  walletAddress));
                    }


                    if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                    walletAddress +
                                    "\\" + WalletTransactionCacheFileExtension)))
                    {
                        File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension)).Close();
                    }
                    try
                    {

                        using (var transactionFile = new StreamWriter(ClassUtility.ConvertPath(
                            System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                            walletAddress +
                            "\\" + WalletTransactionCacheFileExtension), true))
                        {
                            await transactionFile.WriteAsync(transaction + "\n").ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                }
            }
        }

        /// <summary>
        /// Clear each transaction into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static bool RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (!string.IsNullOrEmpty(walletAddress))
            {
                walletAddress += "ANONYMITY";
                if (Directory.Exists(
                    ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\")))
                {
                    if (File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension)))
                    {
                        File.Delete(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\" + WalletTransactionCacheFileExtension));
                    }

                    Directory.Delete(
                        ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory + walletAddress + "\\"), true);
                    Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                              walletAddress));
                }

                ListTransaction.Clear();
            }
            _inClearCache = false;
            return true;
        }

        /// <summary>
        /// Clear wallet transaction cache.
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
        /// Add transaction to the list.
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task AddWalletTransactionAsync(string transaction)
        {

#if DEBUG
                Log.WriteLine("Wallet transaction history received: " + transaction
                                       .Replace(
                                           ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                               .WalletTransactionPerId, ""));
#endif
            if (!OnLoad)
            {
                var splitTransaction = transaction.Replace(ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId, string.Empty).Split(new[] { "#" }, StringSplitOptions.None);
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
                                realFeeAmountSend, ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress + ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey, ClassWalletNetworkSetting.KeySize); // AES
                    else if (type == "RECV")
                        amountAndFeeDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                                realFeeAmountRecv, ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress + ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey, ClassWalletNetworkSetting.KeySize); // AES

                    if (amountAndFeeDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                    {
                        var splitDecryptedAmountAndFee = amountAndFeeDecrypted.Split(new[] { "-" }, StringSplitOptions.None);
                        var amountDecrypted = splitDecryptedAmountAndFee[0];
                        var feeDecrypted = splitDecryptedAmountAndFee[1];
                        var walletDstOrSrc = splitDecryptedAmountAndFee[2];


                        var timestampRecv = splitTransaction[5]; // Timestamp Recv CEST.
                        var blockchainHeight = splitTransaction[6]; // Blockchain height.

                        var finalTransaction = type + "#" + hashTransaction + "#" + walletDstOrSrc + "#" +
                                                    amountDecrypted + "#" + feeDecrypted + "#" + timestamp + "#" +
                                                    timestampRecv + "#" + blockchainHeight;

                        var finalTransactionEncrypted = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, finalTransaction, ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress + ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletKey, ClassWalletNetworkSetting.KeySize); // AES

                        if (finalTransactionEncrypted == ClassAlgoErrorEnumeration.AlgoError) // Ban bad remote node.
                        {
                            if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                            {
                                if (!ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                                {
                                    ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.Add(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost, ClassUtils.DateUnixTimeNowSecond());
                                }
                                else
                                {
                                    ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned[ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost] = ClassUtils.DateUnixTimeNowSecond();
                                }
                            }
                            ClassFormPhase.WalletXiropht.ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
                        }
                        else
                        {
                            string[] splitBlockchainHeight = blockchainHeight.Split(new[] { "~" }, StringSplitOptions.None);

                            ClassWalletTransactionObject transactionObject = new ClassWalletTransactionObject
                            {
                                TransactionType = type,
                                TransactionHash = hashTransaction,
                                TransactionWalletAddress = walletDstOrSrc,
                                TransactionAmount = decimal.Parse(amountDecrypted.ToString(Program.GlobalCultureInfo), NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionFee = decimal.Parse(feeDecrypted.ToString(Program.GlobalCultureInfo), NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionTimestampSend = long.Parse(timestamp),
                                TransactionTimestampRecv = long.Parse(timestampRecv),
                                TransactionBlockchainHeight = splitBlockchainHeight[0].Replace("{", "")
                            };



                            ListTransaction.Add(hashTransaction, transactionObject);


                            await SaveWalletCacheAsync(ClassFormPhase.WalletXiropht.ClassWalletObject.WalletConnect.WalletAddress, finalTransactionEncrypted);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               ClassFormPhase.WalletXiropht.ClassWalletObject.TotalTransactionInSync + ".");
#endif

                        }
                    }
                    else
                    {
#if DEBUG
                    Log.WriteLine("Can't decrypt transaction: " + transaction + " result: " +
                                  amountAndFeeDecrypted);
#endif
                        if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                        {
                            if (!ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                            {
                                ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.Add(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost, ClassUtils.DateUnixTimeNowSecond());
                            }
                            else
                            {
                                ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned[ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost] = ClassUtils.DateUnixTimeNowSecond();
                            }
                        }
                        ClassFormPhase.WalletXiropht.ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
                    }
                }
                else
                {
#if DEBUG
                    Log.WriteLine("Wallet anonymous transaction hash: " + hashTransaction + " already exist on database.");
#endif
                    if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                    {
                        if (!ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.ContainsKey(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                        {
                            ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned.Add(ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost, ClassUtils.DateUnixTimeNowSecond());
                        }
                        else
                        {
                            ClassFormPhase.WalletXiropht.ClassWalletObject.ListRemoteNodeBanned[ClassFormPhase.WalletXiropht.ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost] = ClassUtils.DateUnixTimeNowSecond();
                        }
                    }
                    ClassFormPhase.WalletXiropht.ClassWalletObject.DisconnectWholeRemoteNodeSyncAsync(true, true);
                }
            }
            ClassFormPhase.WalletXiropht.ClassWalletObject.InReceiveTransactionAnonymity = false;

        }
    }
}