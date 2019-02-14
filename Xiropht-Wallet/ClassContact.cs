using System;
using System.Collections.Generic;
using System.IO;

namespace Xiropht_Wallet
{
    public class ClassContact
    {
        public static Dictionary<string, string> ListContactWallet = new Dictionary<string, string>();
        private static string ContactFileName = "\\contact.xirdb";

        /// <summary>
        /// Create or read contact list file of the wallet gui.
        /// </summary>
        public static void InitializationContactList()
        {
            if (!File.Exists(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName)))
            {
                File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.
            }
            else
            {
                using (FileStream fs = File.Open(ClassUtils.ConvertPath(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName)), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    bool errorRead = false;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            var splitContactLine = line.Split(new[] { "|" }, StringSplitOptions.None);
                            var contactName = splitContactLine[0];
                            var contactWalletAddress = splitContactLine[1];
                            if (!ListContactWallet.ContainsKey(contactName))
                            {
                                ListContactWallet.Add(contactName, contactWalletAddress);
                            }
#if DEBUG
                            else
                            {
                                Log.WriteLine("Contact name: "+contactName+" already exist on the list.");
                            }
#endif
                        }
                        catch
                        {
                            errorRead = true;
                            break;
                        }
                    }
                    if(errorRead) // Replace file corrupted by a cleaned one.
                    {
                        ListContactWallet.Clear(); // Clean dictionnary just in case.
#if DEBUG
                        Log.WriteLine("Database contact list file corrupted, remake it");
#endif
                        File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.
                    }
                }
            }
        }

        /// <summary>
        /// Insert a new contact to the list and save the database file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static bool InsertContact(string name, string walletAddress)
        {
            if (ListContactWallet.ContainsKey(name))
            {
#if DEBUG
                Log.WriteLine("Contact name: " + name + " already exist.");
#endif
                return false;
            }
            if (ListContactWallet.ContainsValue(walletAddress))
            {
#if DEBUG
                Log.WriteLine("Contact wallet address: " + walletAddress + " already exist.");
#endif
                return false;
            }
            ListContactWallet.Add(name, walletAddress);
            using (StreamWriter writerContact = new StreamWriter(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName), true))
            {
                writerContact.WriteLine(name + "|" + walletAddress);
            }
            return true;
        }

        /// <summary>
        /// Remove by contact name, because they should are unique.
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveContact(string name)
        {
            if (ListContactWallet.ContainsKey(name))
            {
                ListContactWallet.Remove(name);
            }

            File.Create(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.

            foreach (var contact in ListContactWallet)
            {
                using (StreamWriter writerContact = new StreamWriter(ClassUtils.ConvertPath(Directory.GetCurrentDirectory() + ContactFileName), true))
                {
                    writerContact.WriteLine(contact.Key + "|" + contact.Value);
                }
            }
        }
    }
}
