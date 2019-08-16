using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;

namespace Xiropht_Wallet.Utility
{
    public class ClassUtility
    {
        private static readonly List<string> ListOfSpecialCharactersIgnored = new List<string> {"|", "*"};

        /// <summary>
        ///     Convert path from windows to linux or Mac
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
            ) path = path.Replace("\\", "/");
            return path;
        }

        /// <summary>
        ///     Remove special characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
                    sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        ///     Format amount with the max decimal place.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatAmount(string amount)
        {
            var newAmount = string.Empty;
            var splitAmount = amount.Split(new[] {"."}, StringSplitOptions.None);
            var newPointNumber = ClassConnectorSetting.MaxDecimalPlace - splitAmount[1].Length;
            if (newPointNumber > 0)
            {
                newAmount = splitAmount[0] + "." + splitAmount[1];
                for (var i = 0; i < newPointNumber; i++) newAmount += "0";
                amount = newAmount;
            }
            else if (newPointNumber < 0)
            {
                newAmount = splitAmount[0] + "." + splitAmount[1].Substring(0, splitAmount[1].Length + newPointNumber);
                amount = newAmount;
            }

            return amount;
        }

        /// <summary>
        ///     Check password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password)
        {
            var containLetter = false;
            var containNumber = false;
            var containSpecialCharacter = false;
            foreach (var character in password)
                if (char.IsLetter(character))
                    containLetter = true;
                else if (char.IsDigit(character))
                    containNumber = true;
                else if (!ListOfSpecialCharactersIgnored.Contains(character.ToString()))
                    if (!char.IsLetterOrDigit(character))
                        containSpecialCharacter = true;
            if (containLetter && containNumber && containSpecialCharacter) return true;
            return false;
        }

        /// <summary>
        /// Test tcp connect.
        /// </summary>
        /// <returns></returns>
        public static bool TestTcpHost(string host, int port, int timeout)
        {
            bool status = false;
            Task taskCheckSeedNode = Task.Run(async () => status = await CheckTcp.CheckTcpClientAsync(host, port));
            taskCheckSeedNode.Wait(timeout);
            return status;
        }
    }
}