using System;
using System.Globalization;
using Xiropht_Connector_All.Setting;
using Xiropht_Wallet.Features;

namespace Xiropht_Wallet.Wallet.Sync.Object
{
    public class ClassWalletTransactionObject
    {
        public string TransactionType { get; set; }
        public string TransactionHash { get; set; }
        public string TransactionWalletAddress { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TransactionFee { get; set; }
        public long TransactionTimestampSend { get; set; }
        public long TransactionTimestampRecv { get; set; }
        public string TransactionBlockchainHeight { get; set; }

        /// <summary>
        ///     Concat block information and return them.
        /// </summary>
        /// <returns></returns>
        public string ConcatTransactionElement(string page = null)
        {
            var dateTimeSend = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTimeSend = dateTimeSend.AddSeconds(TransactionTimestampSend);
            dateTimeSend = dateTimeSend.ToLocalTime();
            var dateTimeRecv = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTimeRecv = dateTimeRecv.AddSeconds(TransactionTimestampRecv);
            dateTimeRecv = dateTimeRecv.ToLocalTime();

            if (page != null)
                return ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE") + "=" +
                       TransactionType + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH") + "=" +
                       TransactionHash + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS") + "=" +
                       TransactionWalletAddress + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT") + "=" +
                       TransactionAmount + " " + ClassConnectorSetting.CoinNameMin + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE") + "=" +
                       TransactionFee + " " + ClassConnectorSetting.CoinNameMin + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC") +
                       "=" + TransactionBlockchainHeight + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE") + "=" +
                       dateTimeSend.ToString(CultureInfo.InvariantCulture) + "\n" +
                       ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED") +
                       "=" + dateTimeRecv.ToString(CultureInfo.InvariantCulture) + "\n" +
                       "Page " + page + "\n";
            return ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE") + "=" +
                   TransactionType + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH") + "=" +
                   TransactionHash + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS") + "=" +
                   TransactionWalletAddress + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT") + "=" +
                   TransactionAmount + " " + ClassConnectorSetting.CoinNameMin + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE") + "=" +
                   TransactionFee + " " + ClassConnectorSetting.CoinNameMin + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC") +
                   "=" + TransactionBlockchainHeight + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE") + "=" +
                   dateTimeSend.ToString(CultureInfo.InvariantCulture) + "\n" +
                   ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED") + "=" +
                   dateTimeRecv.ToString(CultureInfo.InvariantCulture) + "\n";
        }
    }
}