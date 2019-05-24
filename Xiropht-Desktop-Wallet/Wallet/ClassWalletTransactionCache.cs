using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionCache
    {
        private const string WalletTransMethodInvokerCacheDirectory = "/Cache/";
        private const string WalletTransMethodInvokerCacheFileExtension = "transaction.xirtra";
        private static bool _inClearCache;
        public static Dictionary<string, long> ListTransaction; // hash, transaction

        /// <summary>
        /// Load transMethodInvoker in cache.
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
                ListTransaction = new Dictionary<string, long>();
            }

            if (Directory.Exists(
                ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    using (FileStream fs = File.Open(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListTransaction.Add(line, ListTransaction.Count);
                        }
                    }
                }
                else
                {
                    File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                }

            }
            else
            {
                if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }


        }

        /// <summary>
        /// Save each transMethodInvoker into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        public static async Task SaveWalletCache(string walletAddress, string transMethodInvoker, bool temporaly)
        {
            if (!temporaly)
            {
                if (ListTransaction.Count > 0 && _inClearCache == false)
                {
                    if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory)) == false)
                    {
                        Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory));
                    }

                    if (Directory.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                         walletAddress)) == false)
                    {
                        Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                                  walletAddress));
                    }


                    if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                    walletAddress +
                                    "\\" + WalletTransMethodInvokerCacheFileExtension)))
                    {
                        File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                    }
                    try
                    {

                        using (var transMethodInvokerFile = new StreamWriter(ClassUtility.ConvertPath(
                            System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                            walletAddress +
                            "\\" + WalletTransMethodInvokerCacheFileExtension), true))
                        {
                            await transMethodInvokerFile.WriteAsync(transMethodInvoker + "\n").ConfigureAwait(false);
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
        /// Clear each transMethodInvoker into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static bool RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (Directory.Exists(
                ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    File.Delete(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension));
                }

                Directory.Delete(
                    ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\"), true);
                Directory.CreateDirectory(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }

            ListTransaction.Clear();

            _inClearCache = false;

            return true;
        }

        /// <summary>
        /// Add transaction to the list.
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task AddWalletTransactionAsync(string transaction)
        {
            try
            {
#if DEBUG
                Log.WriteLine("Wallet transaction history received: " + transaction
                                       .Replace(
                                           ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                               .WalletTransactionPerId, ""));
#endif
                var splitTransaction = transaction.Replace(
                             ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId,
                             "").Split(new[] { "#" }, StringSplitOptions.None);
                var type = splitTransaction[0];
                var timestamp = splitTransaction[3]; // Timestamp Send CEST.
                var hashTransaction = splitTransaction[4]; // Transaction Hash.
                var realFeeAmountSend = splitTransaction[7]; // Real fee and amount crypted for sender.
                var realFeeAmountRecv = splitTransaction[8]; // Real fee and amount crypted for sender.


                var decryptKey =
                         ClassWalletObject.WalletConnect.WalletAddress + ClassWalletObject.WalletConnect.WalletKey; // Wallet Address + Wallet Public Key

                var amountAndFeeDecrypted = "NULL";
                if (type == "SEND")
                    amountAndFeeDecrypted = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountSend, decryptKey, ClassWalletNetworkSetting.KeySize); // AES
                else if (type == "RECV")
                    amountAndFeeDecrypted = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountRecv, decryptKey, ClassWalletNetworkSetting.KeySize); // AES

                if (amountAndFeeDecrypted != "NULL" && amountAndFeeDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                {
                    var splitDecryptedAmountAndFee =
                        amountAndFeeDecrypted.Split(new[] { "-" }, StringSplitOptions.None);
                    var amountDecrypted = splitDecryptedAmountAndFee[0];
                    var feeDecrypted = splitDecryptedAmountAndFee[1];
                    var walletDstOrSrc = splitDecryptedAmountAndFee[2];


                    var timestampRecv = splitTransaction[5]; // Timestamp Recv CEST.
                    var blockchainHeight = splitTransaction[6]; // Blockchain height.

                    var finalTransaction = type + "#" + hashTransaction + "#" + walletDstOrSrc + "#" +
                                                amountDecrypted + "#" + feeDecrypted + "#" + timestamp + "#" +
                                                timestampRecv + "#" + blockchainHeight;

                    var finalTransactionEncrypted =
                         ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, finalTransaction,
                            ClassWalletObject.WalletConnect.WalletAddress + ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES

                    if (finalTransactionEncrypted == ClassAlgoErrorEnumeration.AlgoError) // Ban bad remote node.
                    {
                        if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                        {
                            if (!ClassWalletObject.ListRemoteNodeBanned.ContainsKey(ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost))
                            {
                                ClassWalletObject.ListRemoteNodeBanned.Add(ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost, DateTimeOffset.Now.ToUnixTimeSeconds());
                            }
                            else
                            {
                                ClassWalletObject.ListRemoteNodeBanned[ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost] = DateTimeOffset.Now.ToUnixTimeSeconds();
                            }
                        }
                        ClassWalletObject.DisconnectWholeRemoteNodeSync(true, true);
                    }
                    else
                    {

                        var existTransaction = false;
                       
                        if (ListTransaction.ContainsKey(finalTransactionEncrypted))
                        {
                            existTransaction = true;
                        }

                        if (!existTransaction)
                        {


                            ListTransaction.Add(finalTransactionEncrypted, ListTransaction.Count);


                            await SaveWalletCache(ClassWalletObject.WalletConnect.WalletAddress, finalTransactionEncrypted, false);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               ClassWalletObject.TotalTransactionInSync + ".");
#endif

                        }
                    }
                }
#if DEBUG
                else
                {
                    Log.WriteLine("Impossible to decrypt transaction: " + transaction + " result: " +
                                  amountAndFeeDecrypted);
                }
#endif
            }
            catch
            {

            }
            ClassWalletObject.InReceiveTransaction = false;
        }
    }
}