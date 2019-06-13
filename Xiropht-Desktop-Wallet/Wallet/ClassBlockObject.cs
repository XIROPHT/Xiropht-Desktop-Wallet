using System;
using System.Globalization;
using Xiropht_Connector_All.Setting;

namespace Xiropht_Wallet.Wallet
{
    public class ClassBlockObject
    {
        public string BlockHeight { get; set; }
        public string BlockHash { get; set; }
        public string BlockTransactionHash { get; set; }
        public string BlockTimestampCreate { get; set; }
        public string BlockTimestampFound { get; set; }
        public string BlockDifficulty { get; set; }
        public string BlockReward { get; set; }

        /// <summary>
        /// Concat block information and return them.
        /// </summary>
        /// <returns></returns>
        public string ConcatBlockElement()
        {
            DateTime dateTimeCreate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTimeCreate = dateTimeCreate.AddSeconds(int.Parse(BlockTimestampCreate));
            dateTimeCreate = dateTimeCreate.ToLocalTime();
            DateTime dateTimeFound = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTimeFound = dateTimeFound.AddSeconds(int.Parse(BlockTimestampFound));
            dateTimeFound = dateTimeFound.ToLocalTime();

            return ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_ID_TEXT") + "=" + BlockHeight + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_HASH_TEXT") + "=" + BlockHash + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_TRANSACTION_HASH_TEXT") + "=" + BlockTransactionHash + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_CREATE_TEXT") + "=" + dateTimeCreate.ToString(CultureInfo.InvariantCulture) + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_FOUND_TEXT") + "=" + dateTimeFound.ToString(CultureInfo.InvariantCulture) + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DIFFICULTY_TEXT") + "=" + BlockDifficulty + "\n" +
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_REWARD_TEXT") + "=" + BlockReward + " " + ClassConnectorSetting.CoinNameMin;
        }
    }
}
