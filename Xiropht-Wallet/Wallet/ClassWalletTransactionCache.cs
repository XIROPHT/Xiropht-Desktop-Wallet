using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;

namespace Xiropht_Wallet.Wallet
{
    public class ClassWalletTransactionCache
    {
        private const string WalletTransMethodInvokerCacheDirectory = "/Cache/";
        private const string WalletTransMethodInvokerCacheFileExtension = "transaction.xirtra";
        private const string WalletTransMethodInvokerCacheTmpFileExtension = "transaction_tmp.xirtra";
        private static bool _inClearCache;
        public static List<Tuple<string, string>> ListTransaction; // hash, transaction
        public static List<Tuple<long, string>> ListTransactionTmp; // date expiration, transaction

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
                ListTransactionTmp.Clear();
            }
            else
            {
                ListTransaction = new List<Tuple<string, string>>();
                ListTransactionTmp = new List<Tuple<long, string>>();
            }

            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    using (FileStream fs = File.Open(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListTransaction.Add(new Tuple<string, string>(Xiropht_Connector_All.Utils.ClassUtils.ConvertStringtoSHA512(line), line));
                        }
                    }
                }
                else
                {
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                }

            }
            else
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }


            if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension)))
                {
                    using (FileStream fs = File.Open(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            long expiredTime = long.Parse(line.Split(new[] { "*" }, StringSplitOptions.None)[0]);

                            bool alreadyExist = false;
                            for (int i = 0; i < ListTransaction.Count; i++)
                            {
                                if (ListTransaction[i].Item2 == line)
                                {
                                    alreadyExist = true;
                                }
                            }
                            if (!alreadyExist)
                            {
                                ListTransactionTmp.Add(new Tuple<long, string>(expiredTime, line));
                            }
                        }
                    }
                }
                else
                {
                    File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension)).Close();
                }

            }
            else
            {
                if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress));
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
                    if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                    {
                        Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                    }

                    if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                         walletAddress)) == false)
                    {
                        Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                                  walletAddress));
                    }


                    if (!File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                    walletAddress +
                                    "\\" + WalletTransMethodInvokerCacheFileExtension)))
                    {
                        File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)).Close();
                    }
                    try
                    {

                        using (var transMethodInvokerFile = new StreamWriter(ClassUtils.ConvertPath(
                            Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
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
            else
            {

                if (ListTransactionTmp.Count > 0 && _inClearCache == false)
                {
                    if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory)) == false)
                    {
                        Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory));
                    }

                    if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                         walletAddress)) == false)
                    {
                        Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                                  walletAddress));
                    }


                    if (!File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                    walletAddress +
                                    "\\" + WalletTransMethodInvokerCacheFileExtension)))
                    {
                        File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension)).Close();
                    }
                    try
                    {

                        using (var transMethodInvokerFile = new StreamWriter(ClassUtils.ConvertPath(
                            Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                            walletAddress +
                            "\\" + WalletTransMethodInvokerCacheTmpFileExtension), true))
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
        public static void RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (Directory.Exists(
                ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension)))
                {
                    File.Delete(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheFileExtension));
                }

                Directory.Delete(
                    ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\"), true);
                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory +
                                          walletAddress));
            }

            ListTransaction.Clear();

            if (Directory.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\")))
            {
                if (File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension)))
                {
                    File.Delete(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\" + WalletTransMethodInvokerCacheTmpFileExtension));
                }

                Directory.Delete(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress + "\\"), true);
                Directory.CreateDirectory(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + WalletTransMethodInvokerCacheDirectory + walletAddress));
            }

            ListTransactionTmp.Clear();
            _inClearCache = false;


        }

        /// <summary>
        /// Add transaction to the list.
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task AddWalletTransactionAsync(string transaction, bool temporaly)
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

            if (amountAndFeeDecrypted != "NULL" && amountAndFeeDecrypted != "WRONG")
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
                    if (!temporaly)
                    {
                        ClassWalletObject.ListRemoteNodeBanned.Add(ClassWalletObject.ListWalletConnectToRemoteNode[8].RemoteNodeHost);
                    }
                }
                else
                {

                    if (!temporaly)
                    {
                        var existTransaction = false;
                        for (var i = 0; i < ListTransaction.Count; i++)
                            if (i < ListTransaction.Count)
                                if (ListTransaction[i].Item2 == finalTransactionEncrypted)
                                    existTransaction = true;

                        if (!existTransaction)
                        {
                            bool transactionTmpExist = false;
                            int idTransactionTmp = 0;
                            int counter = 0;
                            foreach (var transactionTmp in ListTransactionTmp)
                            {
                                if (transactionTmp != null)
                                {
                                    if (transactionTmp.Item2 == finalTransactionEncrypted)
                                    {
                                        transactionTmpExist = true;
                                        idTransactionTmp = counter;
                                    }
                                }
                                counter++;
                            }
                            if (transactionTmpExist)
                            {
                                ListTransactionTmp[idTransactionTmp] = null;
                            }

                            ListTransaction.Add(new Tuple<string, string>(Xiropht_Connector_All.Utils.ClassUtils.ConvertStringtoSHA512(finalTransactionEncrypted), finalTransactionEncrypted));

                            await SaveWalletCache(ClassWalletObject.WalletConnect.WalletAddress, finalTransactionEncrypted, false);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               ClassWalletObject.TotalTransactionInSync + ".");
#endif
                        }
                    }
                    else
                    {
                        var existTransaction = false;
                        for (var i = 0; i < ListTransactionTmp.Count; i++)
                            if (i < ListTransactionTmp.Count)
                                if (ListTransactionTmp[i].Item2 == finalTransactionEncrypted)
                                    existTransaction = true;

                        if (!existTransaction)
                        {

                            ListTransactionTmp.Add(new Tuple<long, string>(DateTimeOffset.Now.ToUnixTimeSeconds()+84600, finalTransactionEncrypted)); // Save in temporaly list for 24hours 

                            await SaveWalletCache(ClassWalletObject.WalletConnect.WalletAddress, finalTransactionEncrypted, true);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               ClassWalletObject.TotalTransactionInSync + ".");
#endif
                        }
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
            ClassWalletObject.InReceiveTransaction = false;
        }
    }
}