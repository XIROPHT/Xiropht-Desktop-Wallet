using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xiropht_Wallet
{
    public class ClassContact
    {
        public static Dictionary<string, Tuple<string, string>> ListContactWallet = new Dictionary<string, Tuple<string, string>>();
        private static string ContactFileName = "\\contact.xirdb";

        /// <summary>
        /// Create or read contact list file of the wallet gui.
        /// </summary>
        public static void InitializationContactList()
        {
            if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName)))
            {
                File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.
            }
            else
            {
                using (FileStream fs = File.Open(ClassUtility.ConvertPath(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName)), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                                ListContactWallet.Add(contactName.ToLower(), new Tuple<string, string>(contactName, contactWalletAddress));
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
                    if (errorRead) // Replace file corrupted by a cleaned one.
                    {
                        ListContactWallet.Clear(); // Clean dictionnary just in case.
#if DEBUG
                        Log.WriteLine("Database contact list file corrupted, remake it");
#endif
                        File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.
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
            if (ListContactWallet.ContainsKey(name.ToLower()))
            {
#if DEBUG
                Log.WriteLine("Contact name: " + name + " already exist.");
#endif
                return false;
            }
            foreach (var contactList in ListContactWallet)
            {
                if (contactList.Value.Item2.ToLower() == walletAddress.ToLower())
                {
#if DEBUG
                Log.WriteLine("Contact wallet address: " + walletAddress + " already exist.");
#endif
                    return false;
                }

                if (contactList.Value.Item1.ToLower() == name.ToLower())
                {
#if DEBUG
                Log.WriteLine("Contact wallet address: " + walletAddress + " already exist.");
#endif
                    return false;
                }
            }
            ListContactWallet.Add(name.ToLower(), new Tuple<string, string>(name, walletAddress));
            using (StreamWriter writerContact = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName), true))
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
            if (ListContactWallet.ContainsKey(name.ToLower()))
            {
                ListContactWallet.Remove(name.ToLower());
            }

            File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName)).Close(); // Create and close the file for don't make in busy permissions.

            foreach (var contact in ListContactWallet)
            {
                using (StreamWriter writerContact = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + ContactFileName), true))
                {
                    writerContact.WriteLine(contact.Value.Item1 + "|" + contact.Value.Item2);
                }
            }
        }

        /// <summary>
        /// Get contact name from wallet address.
        /// </summary>
        /// <param name="walletContactInfo"></param>
        public static string GetContactNameFromWalletAddress(string walletContactInfo)
        {
            foreach (var contact in ListContactWallet.ToArray())
            {
                if (contact.Value.Item2 == walletContactInfo) // Compare with wallet address
                {
                    return contact.Value.Item1;
                }
                if (contact.Value.Item1.ToLower() == walletContactInfo.ToLower()) // Compare with contact name
                {
                    return contact.Value.Item1;
                }
            }
            return walletContactInfo;
        }

        /// <summary>
        /// Get contact name from wallet address.
        /// </summary>
        /// <param name="walletContactInfo"></param>
        public static string GetWalletAddressFromContactName(string contactName)
        {
            if (ListContactWallet.ContainsKey(contactName.ToLower()))
            {
                return ListContactWallet[contactName.ToLower()].Item2;
            }
            return contactName;
        }

        /// <summary>
        /// Check if wallet address exist.
        /// </summary>
        /// <param name="walletContactInfo"></param>
        public static bool CheckContactNameFromWalletAddress(string walletContactInfo)
        {
            if (ListContactWallet.ContainsKey(walletContactInfo.ToLower())) // Compare with contact name
            {
                return true;
            }
            foreach (var contact in ListContactWallet.ToArray())
            {
                if (contact.Value.Item2 == walletContactInfo) // Compare with wallet address
                {
                    return true;
                }
                if (contact.Value.Item1.ToLower() == walletContactInfo.ToLower()) // Compare with contact name
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Check if wallet address exist.
        /// </summary>
        /// <param name="walletContactInfo"></param>
        public static bool CheckContactName(string walletContactInfo)
        {
            if (ListContactWallet.ContainsKey(walletContactInfo.ToLower())) // Compare with contact name
            {
                return true;
            }
            return false;
        }
    }
}
