using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xiropht_Wallet.Wallet
{
    public class ClassBlockCache
    {
        private const string WalletBlockCacheDirectory = "/Blockchain/";
        private const string WalletBlockCacheFileExtension = ".xirblock";
        public static List<string> ListBlock;

        /// <summary>
        /// Load block in cache.
        /// </summary>
        /// <returns></returns>
        public static void LoadBlockchainCache()
        {
            if (ListBlock != null)
            {
                ListBlock.Clear();
            }
            else
            {
                ListBlock = new List<string>();
            }

            if (Directory.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory + "\\")))
            {
                if (
                    File.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                "/blockchain" + WalletBlockCacheFileExtension)))
                {

                    int counter = 0;
                    using (FileStream fs = File.Open(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                     "/blockchain" + WalletBlockCacheFileExtension), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ListBlock.Add(line);
                            counter++;
                        }
                    }
                }
            }
            else
            {
                if (Directory.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory)) == false)
                {
                    Directory.CreateDirectory(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
                }

                Directory.CreateDirectory(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
            }
        }

        /// <summary>
        /// Save each block into cache
        /// </summary>
        /// <param name="block"></param>
        public static async Task SaveWalletBlockCache(string block)
        {
            if (Directory.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory)) == false)
            {
                Directory.CreateDirectory(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
            }
            if (File.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                            "/blockchain" + WalletBlockCacheFileExtension)) == false)
            {
                try
                {
                    File.Create(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                "/blockchain" + WalletBlockCacheFileExtension)).Close();

                    using (var transactionFile = new StreamWriter(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                           "/blockchain" + WalletBlockCacheFileExtension), true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    using (var transactionFile = new StreamWriter(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                           "/blockchain" + WalletBlockCacheFileExtension), true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    //
                }
            }
        }


        /// <summary>
        /// Clear each block into cache.
        /// </summary>
        public static bool RemoveWalletBlockCache()
        {
            try
            {
                if (Directory.Exists(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory + "\\")))
                {
                    File.Delete(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                "/blockchain." + WalletBlockCacheFileExtension));
                    Directory.Delete(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory + "\\"), true);
                    Directory.CreateDirectory(ClassUtils.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
                }
            }
            catch
            {
                //
            }

            ListBlock.Clear();
            return true;
        }
    }
}