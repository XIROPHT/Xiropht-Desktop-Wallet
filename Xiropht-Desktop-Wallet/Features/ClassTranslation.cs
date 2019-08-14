using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xiropht_Connector_All.Setting;
using Xiropht_Wallet.FormPhase;
using Xiropht_Wallet.Utility;

namespace Xiropht_Wallet.Features
{
    public class ClassTranslationEnumeration
    {
        public const string languagedefaultname = "english";
        public const string languagename = "LANGUAGE_NAME";
        public const string contributor = "CONTRIBUTOR";
        public const string menufiletext = "MENU_FILE_TEXT";
        public const string menufilemainmenutext = "MENU_FILE_MAIN_MENU_TEXT";
        public const string menufilecreatewalletmenutext = "MENU_FILE_CREATE_WALLET_MENU_TEXT";
        public const string menufileopenwalletmenutext = "MENU_FILE_OPEN_WALLET_MENU_TEXT";
        public const string menufilefunctionclosewallettext = "MENU_FILE_FUNCTION_CLOSE_WALLET_TEXT";
        public const string menufilerestorewalletmenutext = "MENU_FILE_RESTORE_WALLET_MENU_TEXT";
        public const string menufilefunctionexittext = "MENU_FILE_FUNCTION_EXIT_TEXT";
        public const string menulanguagetext = "MENU_LANGUAGE_TEXT";
        public const string menusettingtext = "MENU_SETTING_TEXT";
        public const string menusettingsecuritytext = "MENU_SETTING_SECURITY_TEXT";

        public const string submenusettingsecurityconfigchangepasswordtext =
            "SUBMENU_SETTING_SECURITY_CONFIG_CHANGE_PASSWORD_TEXT";

        public const string submenusettingsecurityconfigpincodetext = "SUBMENU_SETTING_SECURITY_CONFIG_PIN_CODE_TEXT";
        public const string menusettingsynctext = "MENU_SETTING_SYNC_TEXT";
        public const string submenusettingsyncremotenodeconfigtext = "SUBMENU_SETTING_SYNC_REMOTE_NODE_CONFIG_TEXT";
        public const string submenusettingresynctransactiontext = "SUBMENU_SETTING_RESYNC_TRANSACTION_TEXT";
        public const string submenusettingresyncblocktext = "SUBMENU_SETTING_RESYNC_BLOCK_TEXT";
        public const string resynctransactionhistorytitletext = "RESYNC_TRANSACTION_HISTORY_TITLE_TEXT";
        public const string resynctransactionhistorycontenttext = "RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT";
        public const string resyncblockexplorertitletext = "RESYNC_BLOCK_EXPLORER_TITLE_TEXT";
        public const string resyncblockexplorercontenttext = "RESYNC_BLOCK_EXPLORER_CONTENT_TEXT";
        public const string submenuhelptext = "SUBMENU_HELP_TEXT";
        public const string submenuhelpabouttext = "SUBMENU_HELP_ABOUT_TEXT";
        public const string panelwalletpendingbalancetext = "PANEL_WALLET_PENDING_BALANCE_TEXT";
        public const string panelwalletbalancetext = "PANEL_WALLET_BALANCE_TEXT";
        public const string panelwalletaddresstext = "PANEL_WALLET_ADDRESS_TEXT";

        public const string panelwallettotalpendingtransactiononreceivetext =
            "PANEL_WALLET_TOTAL_PENDING_TRANSACTION_ON_RECEIVE_TEXT";

        public const string buttonwalletoverviewtext = "BUTTON_WALLET_OVERVIEW_TEXT";
        public const string buttonwalletsendtransactiontext = "BUTTON_WALLET_SEND_TRANSACTION_TEXT";
        public const string buttonwallettransactionhistorytext = "BUTTON_WALLET_TRANSACTION_HISTORY_TEXT";
        public const string buttonwalletblockexplorertext = "BUTTON_WALLET_BLOCK_EXPLORER_TEXT";
        public const string buttonwalletcontacttext = "BUTTON_WALLET_CONTACT_TEXT";
        public const string gridblockexplorercolumnidtext = "GRID_BLOCK_EXPLORER_COLUMN_ID_TEXT";
        public const string gridblockexplorercolumnhashtext = "GRID_BLOCK_EXPLORER_COLUMN_HASH_TEXT";
        public const string gridblockexplorercolumnrewardtext = "GRID_BLOCK_EXPLORER_COLUMN_REWARD_TEXT";
        public const string gridblockexplorercolumndifficultytext = "GRID_BLOCK_EXPLORER_COLUMN_DIFFICULTY_TEXT";
        public const string gridblockexplorercolumndatecreatetext = "GRID_BLOCK_EXPLORER_COLUMN_DATE_CREATE_TEXT";
        public const string gridblockexplorercolumndatefoundtext = "GRID_BLOCK_EXPLORER_COLUMN_DATE_FOUND_TEXT";

        public const string gridblockexplorercolumntransactionhashtext =
            "GRID_BLOCK_EXPLORER_COLUMN_TRANSACTION_HASH_TEXT";

        public const string createwalletlabeltitletext = "CREATE_WALLET_LABEL_TITLE_TEXT";
        public const string createwalletlabelpasswordrequirementtext = "CREATE_WALLET_LABEL_PASSWORD_REQUIREMENT_TEXT";
        public const string createwalletlabelselectwalletfiletext = "CREATE_WALLET_LABEL_SELECT_WALLET_FILE_TEXT";
        public const string createwalletlabelinputwalletpasswordtext = "CREATE_WALLET_LABEL_INPUT_WALLET_PASSWORD_TEXT";
        public const string createwalletbuttonsubmitcreatetext = "CREATE_WALLET_BUTTON_SUBMIT_CREATE_TEXT";

        public const string createwalleterrorcantconnectmessagecontenttext =
            "CREATE_WALLET_ERROR_CANT_CONNECT_MESSAGE_CONTENT_TEXT";

        public const string mainwalletlabelwelcomeinformationtext = "MAIN_WALLET_LABEL_WELCOME_INFORMATION_TEXT";
        public const string mainwalletlabelhelpinformationtext = "MAIN_WALLET_LABEL_HELP_INFORMATION_TEXT";
        public const string mainwalletbuttonopenwalletmenutext = "MAIN_WALLET_BUTTON_OPEN_WALLET_MENU_TEXT";
        public const string mainwalletbuttoncreatewalletmenutext = "MAIN_WALLET_BUTTON_CREATE_WALLET_MENU_TEXT";
        public const string openwalletlabeltitletext = "OPEN_WALLET_LABEL_TITLE_TEXT";
        public const string openwalletlabelfileselectedtext = "OPEN_WALLET_LABEL_FILE_SELECTED_TEXT";
        public const string openwalletlabelyourpasswordtext = "OPEN_WALLET_LABEL_YOUR_PASSWORD_TEXT";
        public const string openwalletbuttonsearchwalletfiletext = "OPEN_WALLET_BUTTON_SEARCH_WALLET_FILE_TEXT";
        public const string openwalletbuttonsubmitwalletfiletext = "OPEN_WALLET_BUTTON_SUBMIT_WALLET_FILE_TEXT";

        public const string openwalleterrormessagenopasswordwrittedtitletext =
            "OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_TITLE_TEXT";

        public const string openwalleterrormessagenopasswordwrittedcontenttext =
            "OPEN_WALLET_ERROR_MESSAGE_NO_PASSWORD_WRITTED_CONTENT_TEXT";

        public const string openwalleterrormessagewrongpasswordwrittedtitletext =
            "OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_TITLE_TEXT";

        public const string openwalleterrormessagewrongpasswordwrittedcontenttext =
            "OPEN_WALLET_ERROR_MESSAGE_WRONG_PASSWORD_WRITTED_CONTENT_TEXT";

        public const string openwalleterrormessagenetworktitletext = "OPEN_WALLET_ERROR_MESSAGE_NETWORK_TITLE_TEXT";
        public const string openwalleterrormessagenetworkcontenttext = "OPEN_WALLET_ERROR_MESSAGE_NETWORK_CONTENT_TEXT";

        public const string openwalleterrormessagenetworkwrongpasswordwrittedtitletext =
            "OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_TITLE_TEXT";

        public const string openwalleterrormessagenetworkwrongpasswordwrittedcontenttext =
            "OPEN_WALLET_ERROR_MESSAGE_NETWORK_WRONG_PASSWORD_WRITTED_CONTENT_TEXT";

        public const string overviewwalletlabeltitletext = "OVERVIEW_WALLET_LABEL_TITLE_TEXT";
        public const string overviewwalletlabelcoinmaxsupplytext = "OVERVIEW_WALLET_LABEL_COIN_MAX_SUPPLY_TEXT";
        public const string overviewwalletlabelcoincirculatingtext = "OVERVIEW_WALLET_LABEL_COIN_CIRCULATING_TEXT";

        public const string overviewwalletlabeltransactionfeeaccumulatedtext =
            "OVERVIEW_WALLET_LABEL_TRANSACTION_FEE_ACCUMULATED_TEXT";

        public const string overviewwalletlabeltotalcoinminedtext = "OVERVIEW_WALLET_LABEL_TOTAL_COIN_MINED_TEXT";
        public const string overviewwalletlabelblockchainheighttext = "OVERVIEW_WALLET_LABEL_BLOCKCHAIN_HEIGHT_TEXT";
        public const string overviewwalletlabeltotalblockminedtext = "OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_MINED_TEXT";
        public const string overviewwalletlabeltotalblocklefttext = "OVERVIEW_WALLET_LABEL_TOTAL_BLOCK_LEFT_TEXT";
        public const string overviewwalletlabelnetworkdifficultytext = "OVERVIEW_WALLET_LABEL_NETWORK_DIFFICULTY_TEXT";
        public const string overviewwalletlabelnetworkhashratetext = "OVERVIEW_WALLET_LABEL_NETWORK_HASHRATE_TEXT";
        public const string overviewwalletlabellastblockfoundtext = "OVERVIEW_WALLET_LABEL_LAST_BLOCK_FOUND_TEXT";
        public const string overviewwalletlabeltotalcoinpending = "OVERVIEW_WALLET_LABEL_TOTAL_COIN_PENDING";

        public const string overviewwalletbuttontooltiptransactionfeeaccumulatedcontenttext =
            "OVERVIEW_WALLET_BUTTON_TOOLTIP_TRANSACTION_FEE_ACCUMULATED_CONTENT_TEXT";

        public const string overviewwalletbuttonmessagetransactionfeeaccumulatedtitletext =
            "OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_TITLE_TEXT";

        public const string overviewwalletbuttonmessagetransactionfeeaccumulatedcontenttext =
            "OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_CONTENT_TEXT";

        public const string sendtransactionwalletlabeltitletext = "SEND_TRANSACTION_WALLET_LABEL_TITLE_TEXT";

        public const string sendtransactionwalletlabelwalletaddresstargettext =
            "SEND_TRANSACTION_WALLET_LABEL_WALLET_ADDRESS_TARGET_TEXT";

        public const string sendtransactionwalletlabelamounttext = "SEND_TRANSACTION_WALLET_LABEL_AMOUNT_TEXT";
        public const string sendtransactionwalletlabelfeetext = "SEND_TRANSACTION_WALLET_LABEL_FEE_TEXT";

        public const string sendtransactionwalletlabelestimatedreceivetimetext =
            "SEND_TRANSACTION_WALLET_LABEL_ESTIMATED_RECEIVE_TIME_TEXT";

        public const string sendtransactionwalletcheckboxoptionanonymitytext =
            "SEND_TRANSACTION_WALLET_CHECKBOX_OPTION_ANONYMITY_TEXT";

        public const string sendtransactionwalletbuttonsubmittransactiontext =
            "SEND_TRANSACTION_WALLET_BUTTON_SUBMIT_TRANSACTION_TEXT";

        public const string sendtransactionwalletmessageoptionanonymitytitletext =
            "SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT";

        public const string sendtransactionwalletmessageoptionanonymitycontent1text =
            "SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT";

        public const string sendtransactionwalletmessagefeeinformationtitletext =
            "SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT";

        public const string sendtransactionwalletmessagefeeinformationcontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT";

        public const string sendtransactionwallettooltipfeeinformationcontenttext =
            "SEND_TRANSACTION_WALLET_TOOLTIP_FEE_INFORMATION_CONTENT_TEXT";

        public const string sendtransactionwalletmessagetimereceiveinformationtitletext =
            "SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT";

        public const string sendtransactionwalletmessagetimereceiveinformationcontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT";

        public const string sendtransactionwalletmessagesubmittitletext =
            "SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT";

        public const string sendtransactionwalletmessagesubmitcontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT";

        public const string sendtransactionwalletmessageerrorfeecontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT";

        public const string sendtransactionwalletmessageerroramountcontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT";

        public const string sendtransactionwalletmessageerrortargetcontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT";

        public const string sendtransactionwalletmessageerrorconnectiontitletext =
            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_CONNECTION_TITLE_TEXT";

        public const string sendtransactionwalletmessageerrorconnectioncontenttext =
            "SEND_TRANSACTION_WALLET_MESSAGE_ERROR_CONNECTION_CONTENT_TEXT";

        public const string sendtransactionwallettimesecondtext = "SEND_TRANSACTION_WALLET_TIME_SECOND_TEXT";
        public const string sendtransactionwallettimeminutetext = "SEND_TRANSACTION_WALLET_TIME_MINUTE_TEXT";
        public const string sendtransactionwallettimehourtext = "SEND_TRANSACTION_WALLET_TIME_HOUR_TEXT";
        public const string sendtransactionwallettimedaytext = "SEND_TRANSACTION_WALLET_TIME_DAY_TEXT";
        public const string transactionhistorywalletcolumnid = "TRANSACTION_HISTORY_WALLET_COLUMN_ID";
        public const string transactionhistorywalletcolumndate = "TRANSACTION_HISTORY_WALLET_COLUMN_DATE";
        public const string transactionhistorywalletcolumntype = "TRANSACTION_HISTORY_WALLET_COLUMN_TYPE";
        public const string transactionhistorywalletcolumnhash = "TRANSACTION_HISTORY_WALLET_COLUMN_HASH";
        public const string transactionhistorywalletcolumnamount = "TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT";
        public const string transactionhistorywalletcolumnfee = "TRANSACTION_HISTORY_WALLET_COLUMN_FEE";
        public const string transactionhistorywalletcolumnaddress = "TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS";

        public const string transactionhistorywalletcolumndatereceived =
            "TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED";

        public const string transactionhistorywalletcolumnblockheightsrc =
            "TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC";

        public const string transactionhistorywalletwaitingmessagesynctext =
            "TRANSACTION_HISTORY_WALLET_WAITING_MESSAGE_SYNC_TEXT";

        public const string transactionhistorywalletcopytext = "TRANSACTION_HISTORY_WALLET_COPY_TEXT";

        public const string transactionhistorywallettabnormalsendtransactionlisttext =
            "TRANSACTION_HISTORY_WALLET_TAB_NORMAL_SEND_TRANSACTION_LIST_TEXT";

        public const string transactionhistorywallettabanonymoussendtransactionlisttext =
            "TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_SEND_TRANSACTION_LIST_TEXT";

        public const string transactionhistorywallettabnormalreceivedtransactionlisttext =
            "TRANSACTION_HISTORY_WALLET_TAB_NORMAL_RECEIVED_TRANSACTION_LIST_TEXT";

        public const string transactionhistorywallettabanonymousreceivetransactionlisttext =
            "TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_RECEIVE_TRANSACTION_LIST_TEXT";

        public const string transactionhistorywallettabblockrewardreceivedlisttext =
            "TRANSACTION_HISTORY_WALLET_TAB_BLOCK_REWARD_RECEIVED_LIST_TEXT";

        public const string restorewalletlabeltitletext = "RESTORE_WALLET_LABEL_TITLE_TEXT";
        public const string restorewalletlabelselectpathfiletext = "RESTORE_WALLET_LABEL_SELECT_PATH_FILE_TEXT";
        public const string restorewalletlabelprivatekeytext = "RESTORE_WALLET_LABEL_PRIVATE_KEY_TEXT";
        public const string restorewalletlabelpasswordtext = "RESTORE_WALLET_LABEL_PASSWORD_TEXT";
        public const string restorewalletbuttonsubmitrestoretext = "RESTORE_WALLET_BUTTON_SUBMIT_RESTORE_TEXT";
        public const string changepasswordwalletlabeloldpasswordtext = "CHANGE_PASSWORD_WALLET_LABEL_OLD_PASSWORD_TEXT";
        public const string changepasswordwalletlabelpincodetext = "CHANGE_PASSWORD_WALLET_LABEL_PIN_CODE_TEXT";
        public const string changepasswordwalletlabelnewpasswordtext = "CHANGE_PASSWORD_WALLET_LABEL_NEW_PASSWORD_TEXT";
        public const string createwalletsubmenulabelinformationtext = "CREATE_WALLET_SUBMENU_LABEL_INFORMATION_TEXT";
        public const string createwalletsubmenulabelpublickeytext = "CREATE_WALLET_SUBMENU_LABEL_PUBLIC_KEY_TEXT";
        public const string createwalletsubmenulabelprivatekeytext = "CREATE_WALLET_SUBMENU_LABEL_PRIVATE_KEY_TEXT";
        public const string createwalletsubmenulabelpincodetext = "CREATE_WALLET_SUBMENU_LABEL_PIN_CODE_TEXT";

        public const string createwalletsubmenubuttoncopywalletinformationtext =
            "CREATE_WALLET_SUBMENU_BUTTON_COPY_WALLET_INFORMATION_TEXT";

        public const string createwalletsubmenubuttonacceptwalletinformationtext =
            "CREATE_WALLET_SUBMENU_BUTTON_ACCEPT_WALLET_INFORMATION_TEXT";

        public const string createwalletsubmenubuttonacceptwalletinformationmessagetitletext =
            "CREATE_WALLET_SUBMENU_BUTTON_ACCEPT_WALLET_INFORMATION_MESSAGE_TITLE_TEXT";

        public const string createwalletsubmenubuttonacceptwalletinformationmessagecontenttext =
            "CREATE_WALLET_SUBMENU_BUTTON_ACCEPT_WALLET_INFORMATION_MESSAGE_CONTENT_TEXT";

        public const string createwalletsubmenubuttoncopywalletinformationtitletext =
            "CREATE_WALLET_SUBMENU_BUTTON_COPY_WALLET_INFORMATION_TITLE_TEXT";

        public const string createwalletsubmenubuttoncopywalletinformationcontenttext =
            "CREATE_WALLET_SUBMENU_BUTTON_COPY_WALLET_INFORMATION_CONTENT_TEXT";

        public const string createwalletsubmenubuttonacceptwalletinformationmessagesafecontenttext =
            "CREATE_WALLET_SUBMENU_BUTTON_ACCEPT_WALLET_INFORMATION_MESSAGE_SAFE_CONTENT_TEXT";

        public const string pincodesettingmenulabelstatusinformationtext =
            "PIN_CODE_SETTING_MENU_LABEL_STATUS_INFORMATION_TEXT";

        public const string pincodesettingmenulabelstatusenabledtext =
            "PIN_CODE_SETTING_MENU_LABEL_STATUS_ENABLED_TEXT";

        public const string pincodesettingmenubuttonstatusenabletext =
            "PIN_CODE_SETTING_MENU_BUTTON_STATUS_ENABLE_TEXT";

        public const string pincodesettingmenulabelstatusdisabledtext =
            "PIN_CODE_SETTING_MENU_LABEL_STATUS_DISABLED_TEXT";

        public const string pincodesettingmenubuttonstatusdisabletext =
            "PIN_CODE_SETTING_MENU_BUTTON_STATUS_DISABLE_TEXT";

        public const string pincodesettingmenulabelinformationtext = "PIN_CODE_SETTING_MENU_LABEL_INFORMATION_TEXT";

        public const string pincodesettingmenulabelwritepasswordtext =
            "PIN_CODE_SETTING_MENU_LABEL_WRITE_PASSWORD_TEXT";

        public const string pincodesettingmenulabelwritepincodetext = "PIN_CODE_SETTING_MENU_LABEL_WRITE_PIN_CODE_TEXT";

        public const string pincodesettingmenubuttonsubmitchangetext =
            "PIN_CODE_SETTING_MENU_BUTTON_SUBMIT_CHANGE_TEXT";

        public const string pincodesubmitmenulabelinformationtext = "PIN_CODE_SUBMIT_MENU_LABEL_INFORMATION_TEXT";
        public const string pincodesubmitmenuwarningtext = "PIN_CODE_SUBMIT_MENU_WARNING_TEXT";
        public const string pincodesubmitmenunetworkerrortext = "PIN_CODE_SUBMIT_MENU_NETWORK_ERROR_TEXT";

        public const string remotenodesettingmenuuseseednodenetworkonlytext =
            "REMOTE_NODE_SETTING_MENU_USE_SEED_NODE_NETWORK_ONLY_TEXT";

        public const string remotenodesettingmenuuseremotenodetext = "REMOTE_NODE_SETTING_MENU_USE_REMOTE_NODE_TEXT";

        public const string remotenodesettingmenuuseremotenodeinformationtext =
            "REMOTE_NODE_SETTING_MENU_USE_REMOTE_NODE_INFORMATION_TEXT";

        public const string remotenodesettingmenuusemanualnodetext = "REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_TEXT";

        public const string remotenodesettingmenuusemanualnodeinformationtext =
            "REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_INFORMATION_TEXT";

        public const string remotenodesettingmenuusemanualnodehostnametext =
            "REMOTE_NODE_SETTING_MENU_USE_MANUAL_NODE_HOSTNAME_TEXT";

        public const string remotenodesettingmenusavesettingtext = "REMOTE_NODE_SETTING_MENU_SAVE_SETTING_TEXT";
        public const string waitingmenulabeltext = "WAITING_MENU_LABEL_TEXT";
        public const string networkwaitingmenulabeltext = "NETWORK_WAITING_MENU_LABEL_TEXT";
        public const string waitingcreatewalletmenulabeltext = "WAITING_CREATE_WALLET_MENU_LABEL_TEXT";
        public const string contactbuttonaddcontacttext = "CONTACT_BUTTON_ADD_CONTACT_TEXT";
        public const string contactlistcolumnnametext = "CONTACT_LIST_COLUMN_NAME_TEXT";
        public const string contactlistcolumnaddresstext = "CONTACT_LIST_COLUMN_ADDRESS_TEXT";
        public const string contactlistcolumnactiontext = "CONTACT_LIST_COLUMN_ACTION_TEXT";
        public const string contactlistbuttonremovetext = "CONTACT_LIST_BUTTON_REMOVE_TEXT";
        public const string contactlistcopyactioncontenttext = "CONTACT_LIST_COPY_ACTION_CONTENT_TEXT";
        public const string contactlistremoveactioncontenttext = "CONTACT_LIST_REMOVE_ACTION_CONTENT_TEXT";
        public const string contactsubmenulabelcontactnametext = "CONTACT_SUBMENU_LABEL_CONTACT_NAME_TEXT";

        public const string contactsubmenulabelcontactwalletaddresstext =
            "CONTACT_SUBMENU_LABEL_CONTACT_WALLET_ADDRESS_TEXT";

        public const string contactsubmenuerroremptycontactnametitletext =
            "CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_TITLE_TEXT";

        public const string contactsubmenuerroremptycontactnamecontenttext =
            "CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_NAME_CONTENT_TEXT";

        public const string contactsubmenuerrorinvalidcontactnametitletext =
            "CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_TITLE_TEXT";

        public const string contactsubmenuerrorinvalidcontactnamecontenttext =
            "CONTACT_SUBMENU_ERROR_INVALID_CONTACT_NAME_CONTENT_TEXT";

        public const string contactsubmenuerroremptycontactwalletaddresstitletext =
            "CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_TITLE_TEXT";

        public const string contactsubmenuerroremptycontactwalletaddresscontenttext =
            "CONTACT_SUBMENU_ERROR_EMPTY_CONTACT_WALLET_ADDRESS_CONTENT_TEXT";

        public const string contactsubmenuerrorinvalidcontactwalletaddresstitletext =
            "CONTACT_SUBMENU_ERROR_INVALID_CONTACT_WALLET_ADDRESS_TITLE_TEXT";

        public const string contactsubmenuerrorinvalidcontactwalletaddresscontenttext =
            "CONTACT_SUBMENU_ERROR_INVALID_CONTACT_WALLET_ADDRESS_CONTENT_TEXT";

        public const string contactsubmenuerrorinsertcontacttitletext =
            "CONTACT_SUBMENU_ERROR_INSERT_CONTACT_TITLE_TEXT";

        public const string contactsubmenuerrorinsertcontactcontenttext =
            "CONTACT_SUBMENU_ERROR_INSERT_CONTACT_CONTENT_TEXT";

        public const string contactsubmenusuccessinsertcontacttitletext =
            "CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_TITLE_TEXT";

        public const string contactsubmenusuccessinsertcontactcontenttext =
            "CONTACT_SUBMENU_SUCCESS_INSERT_CONTACT_CONTENT_TEXT";

        public const string firststartmenulabelwelcome = "FIRST_START_MENU_LABEL_WELCOME";

        public const string walletnetworkobjecttransactioncacheerrortext =
            "WALLET_NETWORK_OBJECT_TRANSACTION_CACHE_ERROR_TEXT";

        public const string walletnetworkobjectanonymitytransactioncacheerrortext =
            "WALLET_NETWORK_OBJECT_ANONYMITY_TRANSACTION_CACHE_ERROR_TEXT";

        public const string walletnetworkobjectblockcachereadsuccesstext =
            "WALLET_NETWORK_OBJECT_BLOCK_CACHE_READ_SUCCESS_TEXT";

        public const string walletnetworkobjectdisconnectedtext = "WALLET_NETWORK_OBJECT_DISCONNECTED_TEXT";

        public const string walletnetworkobjectcannotconnectwalletcontenttext =
            "WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_CONTENT_TEXT";

        public const string walletnetworkobjectcannotconnectwallettitletext =
            "WALLET_NETWORK_OBJECT_CANNOT_CONNECT_WALLET_TITLE_TEXT";

        public const string walletnetworkobjectsuccessconnectwalletcontenttext =
            "WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_CONTENT_TEXT";

        public const string walletnetworkobjectsuccessconnectwallettitletext =
            "WALLET_NETWORK_OBJECT_SUCCESS_CONNECT_WALLET_TITLE_TEXT";

        public const string walletnetworkobjectcreatewalletpassworderror1contenttext =
            "WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_CONTENT_TEXT";

        public const string walletnetworkobjectcreatewalletpassworderror1titletext =
            "WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR1_TITLE_TEXT";

        public const string walletnetworkobjectcreatewalletpassworderror2contenttext =
            "WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_CONTENT_TEXT";

        public const string walletnetworkobjectcreatewalletpassworderror2titletext =
            "WALLET_NETWORK_OBJECT_CREATE_WALLET_PASSWORD_ERROR2_TITLE_TEXT";

        public const string walletnetworkobjectpincodeacceptedcontenttext =
            "WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_CONTENT_TEXT";

        public const string walletnetworkobjectpincodeacceptedtitletext =
            "WALLET_NETWORK_OBJECT_PIN_CODE_ACCEPTED_TITLE_TEXT";

        public const string walletnetworkobjectpincoderefusedcontenttext =
            "WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_CONTENT_TEXT";

        public const string walletnetworkobjectpincoderefusedtitletext =
            "WALLET_NETWORK_OBJECT_PIN_CODE_REFUSED_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactioninvalidamountcontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactioninvalidamounttitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_INVALID_AMOUNT_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionnotenoughtamountcontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionnotenoughtamounttitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_AMOUNT_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionnotenoughtfeecontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionnotenoughtfeetitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_NOT_ENOUGHT_FEE_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionbusycontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionbusytitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionbusyreceivecontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_RECEIVE_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionbusyreceivetitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_BUSY_RECEIVE_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionacceptedcontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionacceptedtitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ACCEPTED_TITLE_TEXT";

        public const string walletnetworkobjectsendtransactionaddressnotvalidcontenttext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_CONTENT_TEXT";

        public const string walletnetworkobjectsendtransactionaddressnotvalidtitletext =
            "WALLET_NETWORK_OBJECT_SEND_TRANSACTION_ADDRESS_NOT_VALID_TITLE_TEXT";

        public const string walletnetworkobjectbannedcontenttext = "WALLET_NETWORK_OBJECT_BANNED_CONTENT_TEXT";
        public const string walletnetworkobjectbannedtitletext = "WALLET_NETWORK_OBJECT_BANNED_TITLE_TEXT";

        public const string walletnetworkobjectalreadyconnectedcontenttext =
            "WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_CONTENT_TEXT";

        public const string walletnetworkobjectalreadyconnectedtitletext =
            "WALLET_NETWORK_OBJECT_ALREADY_CONNECTED_TITLE_TEXT";

        public const string walletnetworkobjectchangepasswordacceptedcontenttext =
            "WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_CONTENT_TEXT";

        public const string walletnetworkobjectchangepasswordacceptedtitletext =
            "WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_ACCEPTED_TITLE_TEXT";

        public const string walletnetworkobjectchangepasswordrefusedcontenttext =
            "WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_CONTENT_TEXT";

        public const string walletnetworkobjectchangepasswordrefusedtitletext =
            "WALLET_NETWORK_OBJECT_CHANGE_PASSWORD_REFUSED_TITLE_TEXT";

        public const string walletnetworkobjectchangepincodestatusacceptedcontenttext =
            "WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_CONTENT_TEXT";

        public const string walletnetworkobjectchangepincodestatusacceptedtitletext =
            "WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_ACCEPTED_TITLE_TEXT";

        public const string walletnetworkobjectchangepincodestatusrefusedcontenttext =
            "WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_CONTENT_TEXT";

        public const string walletnetworkobjectchangepincodestatusrefusedtitletext =
            "WALLET_NETWORK_OBJECT_CHANGE_PIN_CODE_STATUS_REFUSED_TITLE_TEXT";

        public const string walletnetworkobjectwarningwalletconnectioncontenttext =
            "WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_CONTENT_TEXT";

        public const string walletnetworkobjectwarningwalletconnectiontitletext =
            "WALLET_NETWORK_OBJECT_WARNING_WALLET_CONNECTION_TITLE_TEXT";

        public const string walletnetworkobjectcannotsendpackettext = "WALLET_NETWORK_OBJECT_CANNOT_SEND_PACKET_TEXT";
    }

    public class ClassTranslation
    {
        public const string LanguageFolderName = "\\Language\\";

        /// <summary>
        ///     List of command orders to replace when it's possible.
        /// </summary>
        public const string CoinNameOrder = "%CoinName";

        public const string CoinMinNameOrder = "%CoinMinName";
        public const string AmountSendOrder = "%AmountSend";
        public const string TargetAddressOrder = "%TargetWallet";
        public const string DateOrder = "%Date";
        public static string CurrentLanguage;


        public static Dictionary<string, List<string>> LanguageContributors = new Dictionary<string, List<string>>();

        public static Dictionary<string, Dictionary<string, string>> LanguageDatabases =
            new Dictionary<string, Dictionary<string, string>>(); // Dictionnary content format -> {string:Language Name|Dictionnary:{string:text name|string:text content}}

        /// <summary>
        ///     Read every language files, insert them to language database.
        /// </summary>
        public static void InitializationLanguage()
        {
            if (CurrentLanguage == null || string.IsNullOrEmpty(CurrentLanguage))
                CurrentLanguage = ClassTranslationEnumeration.languagedefaultname; // By Default on initialization.
            if (Directory.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName)))
            {
                var languageFilesList = Directory
                    .GetFiles(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName),
                        "*.xirlang").Select(Path.GetFileName).ToArray();
                if (languageFilesList.Length == 0)
                {
#if DEBUG
                    Log.WriteLine("No language files found, please reinstall your gui wallet.");
#endif

                    MessageBox.Show("No language files found, please reinstall your gui wallet.",
                        "No language files found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    for (var i = 0; i < languageFilesList.Length; i++)
                        if (i < languageFilesList.Length)
                            if (languageFilesList[i] != null)
                                if (!string.IsNullOrEmpty(languageFilesList[i]))
                                {
                                    var currentLanguage = string.Empty;
#if DEBUG
                                    Log.WriteLine("Read language file: "+languageFilesList[i]);
#endif

                                    using (var fs =
                                        File.Open(
                                            ClassUtility.ConvertPath(
                                                AppDomain.CurrentDomain.BaseDirectory + LanguageFolderName + "\\" +
                                                languageFilesList[i]), FileMode.Open, FileAccess.Read,
                                            FileShare.ReadWrite))
                                    using (var bs = new BufferedStream(fs))
                                    using (var sr = new StreamReader(bs))
                                    {
                                        string line;
                                        while ((line = sr.ReadLine()) != null)
                                            if (!line.Contains("#") && !string.IsNullOrEmpty(line)
                                            ) // Ignore lines who contains # character.
                                            {
                                                if (line.Contains(ClassTranslationEnumeration.languagename + "="))
                                                {
                                                    currentLanguage =
                                                        line.Replace(ClassTranslationEnumeration.languagename + "=", "")
                                                            .ToLower();
#if DEBUG
                                                    Log.WriteLine("Language name detected: " + currentLanguage);
#endif
                                                    if (!LanguageDatabases.ContainsKey(currentLanguage))
                                                        LanguageDatabases.Add(currentLanguage,
                                                            new Dictionary<string, string>());
                                                }
                                                else if (line.Contains(ClassTranslationEnumeration.contributor + "="))
                                                {
                                                    if (!LanguageContributors.ContainsKey(currentLanguage))
                                                        LanguageContributors.Add(currentLanguage, new List<string>());
                                                    LanguageContributors[currentLanguage]
                                                        .Add(line.Replace(ClassTranslationEnumeration.contributor + "=",
                                                            ""));
                                                }
                                                else
                                                {
                                                    if (currentLanguage != string.Empty
                                                    ) // Ignore lines if the current language name of the file is not found.
                                                    {
                                                        var splitLanguageText = line.Split(new[] {"="},
                                                            StringSplitOptions.None);
                                                        var orderLanguageText = splitLanguageText[0];
                                                        var contentLanguageText = splitLanguageText[1];

                                                        // Replace commands.
                                                        contentLanguageText = contentLanguageText.Replace(CoinNameOrder,
                                                            ClassConnectorSetting.CoinName);
                                                        contentLanguageText =
                                                            contentLanguageText.Replace(CoinMinNameOrder,
                                                                ClassConnectorSetting.CoinNameMin);
                                                        contentLanguageText =
                                                            contentLanguageText.Replace("\\n", Environment.NewLine);

                                                        // Insert.
                                                        LanguageDatabases[currentLanguage].Add(orderLanguageText,
                                                            contentLanguageText);

#if DEBUG
                                                        Log.WriteLine("Insert order language text: " + orderLanguageText + " with content language text: " + contentLanguageText + " for language name: " + currentLanguage);
#endif
                                                    }
                                                }
                                            }
                                    }
                                }
                }
            }
            else
            {
#if DEBUG
                Log.WriteLine("No language folder found, please reinstall your gui wallet.");
#endif
#if WINDOWS
                ClassFormPhase.MessageBoxInterface("No language folder found, please reinstall your gui wallet.",
                    "No folder language found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(Program.WalletXiropht, "No language folder found, please reinstall your gui wallet.", "No folder language found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        ///     Return language text from order text.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string GetLanguageTextFromOrder(string order)
        {
            if (LanguageDatabases.ContainsKey(CurrentLanguage))
            {
                if (LanguageDatabases[CurrentLanguage].ContainsKey(order))
                    return LanguageDatabases[CurrentLanguage][order];
                return "LANGUAGE ORDER MISSING: " + order;
            }

            return "LANGUAGE NAME MISSING: " + CurrentLanguage;
        }

        /// <summary>
        ///     Change current language.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool ChangeCurrentLanguage(string language)
        {
            language = language.ToLower();
            if (LanguageDatabases.ContainsKey(language))
            {
#if DEBUG
                Log.WriteLine("Old current language: "+CurrentLanguage+" to new language: "+language+" success.");
#endif
                CurrentLanguage = language;
                return true;
            }
#if DEBUG
            Log.WriteLine(language+" not exist.");
#endif
            return false;
        }

        /// <summary>
        ///     Replace first letter by a upper letter.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}