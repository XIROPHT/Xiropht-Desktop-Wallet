using System;
using System.Text;
using Xiropht_Connector_All.Setting;

namespace Xiropht_Wallet
{
    public class ClassUtility
    {
        /// <summary>
        /// Convert path from windows to linux or Mac
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                path = path.Replace("\\", "/");
            }
            return path;
        }

        /// <summary>
        /// Remove special characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Format amount with the max decimal place.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatAmount(string amount)
        {
            string newAmount = string.Empty;
            var splitAmount = amount.Split(new[] { "." }, StringSplitOptions.None);
            var newPointNumber = ClassConnectorSetting.MaxDecimalPlace - splitAmount[1].Length;
            if (newPointNumber > 0)
            {
                newAmount = splitAmount[0] + "." + splitAmount[1];
                for (int i = 0; i < newPointNumber; i++)
                {
                    newAmount += "0";
                }
                amount = newAmount;
            }
            else if (newPointNumber < 0)
            {
                newAmount = splitAmount[0] + "." + splitAmount[1].Substring(0, splitAmount[1].Length + newPointNumber);
                amount = newAmount;
            }

            return amount;
        }

    }
}
