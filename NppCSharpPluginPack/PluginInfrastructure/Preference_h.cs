﻿// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
//
// This file should stay in sync with the CPP project file
// "notepad-plus-plus/scintilla/include/Scintilla.iface"
// found at
// https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/scintilla/include/Scintilla.iface

namespace Kbg.NppPluginNET.PluginInfrastructure
{
	public enum Preference
	{
		/* ++Autogenerated -- start of section automatically generated from preference_rc.h */

        IDD_PREFERENCE_BOX = 6000,
        IDC_BUTTON_CLOSE = IDD_PREFERENCE_BOX + 1,
        IDC_LIST_DLGTITLE = IDD_PREFERENCE_BOX + 2,

        IDD_PREFERENCE_BAR_BOX = 6100,
        IDC_TOOLBAR_GB_STATIC = IDD_PREFERENCE_BAR_BOX + 1,
        IDC_CHECK_HIDE = IDD_PREFERENCE_BAR_BOX + 2,
        IDC_RADIO_SMALLICON = IDD_PREFERENCE_BAR_BOX + 3,
        IDC_RADIO_BIGICON = IDD_PREFERENCE_BAR_BOX + 4,
        IDC_RADIO_STANDARD = IDD_PREFERENCE_BAR_BOX + 5,

        IDC_TABBAR_GB_STATIC = IDD_PREFERENCE_BAR_BOX + 6,
        IDC_CHECK_REDUCE = IDD_PREFERENCE_BAR_BOX + 7,
        IDC_CHECK_LOCK = IDD_PREFERENCE_BAR_BOX + 8,
        IDC_CHECK_DRAWINACTIVE = IDD_PREFERENCE_BAR_BOX + 9,
        IDC_CHECK_ORANGE = IDD_PREFERENCE_BAR_BOX + 10,
        IDC_CHECK_SHOWSTATUSBAR = IDD_PREFERENCE_BAR_BOX + 11,
        IDC_CHECK_ENABLETABCLOSE = IDD_PREFERENCE_BAR_BOX + 12,
        IDC_CHECK_DBCLICK2CLOSE = IDD_PREFERENCE_BAR_BOX + 13,
        IDC_CHECK_ENABLEDOCSWITCHER = IDD_PREFERENCE_BAR_BOX + 14,
        IDC_CHECK_MAINTAININDENT = IDD_PREFERENCE_BAR_BOX + 15,
        IDC_CHECK_KEEPINSAMEDIR = IDD_PREFERENCE_BAR_BOX + 16,
        IDC_CHECK_STYLEMRU = IDD_PREFERENCE_BAR_BOX + 17,
        IDC_CHECK_TAB_HIDE = IDD_PREFERENCE_BAR_BOX + 18,
        IDC_CHECK_TAB_MULTILINE = IDD_PREFERENCE_BAR_BOX + 19,
        IDC_CHECK_TAB_VERTICAL = IDD_PREFERENCE_BAR_BOX + 20,
        IDC_CHECK_TAB_LAST_EXIT = IDD_PREFERENCE_BAR_BOX + 21,
        IDC_CHECK_HIDEMENUBAR = IDD_PREFERENCE_BAR_BOX + 22,
        IDC_LOCALIZATION_GB_STATIC = IDD_PREFERENCE_BAR_BOX + 23,
        IDC_COMBO_LOCALIZATION = IDD_PREFERENCE_BAR_BOX + 24,
        IDC_DOCSWITCH_GB_STATIC = IDD_PREFERENCE_BAR_BOX + 25,
        IDC_CHECK_DOCSWITCH = IDD_PREFERENCE_BAR_BOX + 26,
        IDC_CHECK_DOCSWITCH_NOEXTCOLUMN = IDD_PREFERENCE_BAR_BOX + 27,

        IDD_PREFERENCE_MULTIINSTANCE_BOX = 6150,
        IDC_MULTIINST_GB_STATIC = IDD_PREFERENCE_MULTIINSTANCE_BOX + 1,
        IDC_SESSIONININST_RADIO = IDD_PREFERENCE_MULTIINSTANCE_BOX + 2,
        IDC_MULTIINST_RADIO = IDD_PREFERENCE_MULTIINSTANCE_BOX + 3,
        IDC_MONOINST_RADIO = IDD_PREFERENCE_MULTIINSTANCE_BOX + 4,
        IDD_STATIC_RESTARTNOTE = IDD_PREFERENCE_MULTIINSTANCE_BOX + 5,

        IDD_PREFERENCE_WORDCHARLIST_BOX = 6160,
        IDC_WORDCHARLIST_GB_STATIC = IDD_PREFERENCE_WORDCHARLIST_BOX + 1,
        IDC_RADIO_WORDCHAR_DEFAULT = IDD_PREFERENCE_WORDCHARLIST_BOX + 2,
        IDC_RADIO_WORDCHAR_CUSTOM = IDD_PREFERENCE_WORDCHARLIST_BOX + 3,
        IDC_WORDCHAR_CUSTOM_EDIT = IDD_PREFERENCE_WORDCHARLIST_BOX + 4,
        IDD_WORDCHAR_QUESTION_BUTTON = IDD_PREFERENCE_WORDCHARLIST_BOX + 5,
        IDD_STATIC_WORDCHAR_WARNING = IDD_PREFERENCE_WORDCHARLIST_BOX + 6,

        IDD_PREFERENCE_MARGEIN_BOX = 6200,
        IDC_FMS_GB_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 1,
        IDC_RADIO_SIMPLE = IDD_PREFERENCE_MARGEIN_BOX + 2,
        IDC_RADIO_ARROW = IDD_PREFERENCE_MARGEIN_BOX + 3,
        IDC_RADIO_CIRCLE = IDD_PREFERENCE_MARGEIN_BOX + 4,
        IDC_RADIO_BOX = IDD_PREFERENCE_MARGEIN_BOX + 5,

        IDC_CHECK_LINENUMBERMARGE = IDD_PREFERENCE_MARGEIN_BOX + 6,
        IDC_CHECK_BOOKMARKMARGE = IDD_PREFERENCE_MARGEIN_BOX + 7,

        IDC_VES_GB_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 11,

        IDC_CHECK_EDGEBGMODE = IDD_PREFERENCE_MARGEIN_BOX + 13,
        IDC_CHECK_CURRENTLINEHILITE = IDD_PREFERENCE_MARGEIN_BOX + 14,
        IDC_CHECK_SMOOTHFONT = IDD_PREFERENCE_MARGEIN_BOX + 15,

        IDC_CARETSETTING_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 16,
        IDC_WIDTH_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 17,
        IDC_WIDTH_COMBO = IDD_PREFERENCE_MARGEIN_BOX + 18,
        IDC_BLINKRATE_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 19,
        IDC_CARETBLINKRATE_SLIDER = IDD_PREFERENCE_MARGEIN_BOX + 20,
        IDC_CARETBLINKRATE_F_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 21,
        IDC_CARETBLINKRATE_S_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 22,
        IDC_CHECK_DOCCHANGESTATEMARGE = IDD_PREFERENCE_MARGEIN_BOX + 23,
        IDC_MULTISELECTION_GB_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 24,
        IDC_CHECK_MULTISELECTION = IDD_PREFERENCE_MARGEIN_BOX + 25,

        IDC_RADIO_FOLDMARGENONE = IDD_PREFERENCE_MARGEIN_BOX + 26,

        IDC_LW_GB_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 27,
        IDC_RADIO_LWDEF = IDD_PREFERENCE_MARGEIN_BOX + 28,
        IDC_RADIO_LWALIGN = IDD_PREFERENCE_MARGEIN_BOX + 29,
        IDC_RADIO_LWINDENT = IDD_PREFERENCE_MARGEIN_BOX + 30,

        IDC_BORDERWIDTH_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 31,
        IDC_BORDERWIDTHVAL_STATIC = IDD_PREFERENCE_MARGEIN_BOX + 32,
        IDC_BORDERWIDTH_SLIDER = IDD_PREFERENCE_MARGEIN_BOX + 33,
        IDC_CHECK_DISABLEADVANCEDSCROLL = IDD_PREFERENCE_MARGEIN_BOX + 34,
        IDC_CHECK_NOEDGE = IDD_PREFERENCE_MARGEIN_BOX + 35,
        IDC_CHECK_SCROLLBEYONDLASTLINE = IDD_PREFERENCE_MARGEIN_BOX + 36,

        IDC_STATIC_MULTILNMODE_TIP = IDD_PREFERENCE_MARGEIN_BOX + 37,
        IDC_COLUMNPOS_EDIT = IDD_PREFERENCE_MARGEIN_BOX + 38,
        IDC_CHECK_RIGHTCLICKKEEPSSELECTION = IDD_PREFERENCE_MARGEIN_BOX + 39,

        IDD_PREFERENCE_DELIMITERSETTINGS_BOX = 6250,
        IDC_DELIMITERSETTINGS_GB_STATIC = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 1,
        IDD_STATIC_OPENDELIMITER = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 2,
        IDC_EDIT_OPENDELIMITER = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 3,
        IDC_EDIT_CLOSEDELIMITER = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 4,
        IDD_STATIC_CLOSEDELIMITER = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 5,
        IDD_SEVERALLINEMODEON_CHECK = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 6,
        IDD_STATIC_BLABLA = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 7,
        IDD_STATIC_BLABLA2NDLINE = IDD_PREFERENCE_DELIMITERSETTINGS_BOX + 8,

        IDD_PREFERENCE_SETTINGSONCLOUD_BOX = 6260,
        IDC_SETTINGSONCLOUD_WARNING_STATIC = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 1,
        IDC_SETTINGSONCLOUD_GB_STATIC = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 2,
        IDC_NOCLOUD_RADIO = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 3,

        IDC_WITHCLOUD_RADIO = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 7,
        IDC_CLOUDPATH_EDIT = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 8,
        IDD_CLOUDPATH_BROWSE_BUTTON = IDD_PREFERENCE_SETTINGSONCLOUD_BOX + 9,

        IDD_PREFERENCE_SEARCHENGINE_BOX = 6270,
        IDC_SEARCHENGINES_GB_STATIC = IDD_PREFERENCE_SEARCHENGINE_BOX + 1,
        IDC_SEARCHENGINE_DUCKDUCKGO_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 2,
        IDC_SEARCHENGINE_GOOGLE_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 3,
        IDC_SEARCHENGINE_BING_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 4,
        IDC_SEARCHENGINE_YAHOO_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 5,
        IDC_SEARCHENGINE_CUSTOM_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 6,
        IDC_SEARCHENGINE_EDIT = IDD_PREFERENCE_SEARCHENGINE_BOX + 7,
        IDD_SEARCHENGINE_NOTE_STATIC = IDD_PREFERENCE_SEARCHENGINE_BOX + 8,
        IDC_SEARCHENGINE_STACKOVERFLOW_RADIO = IDD_PREFERENCE_SEARCHENGINE_BOX + 9,

        IDD_PREFERENCE_SETTING_BOX = 6300,
        IDC_TABSETTING_GB_STATIC = IDD_PREFERENCE_SETTING_BOX + 1,
        IDC_CHECK_REPLACEBYSPACE = IDD_PREFERENCE_SETTING_BOX + 2,
        IDC_TABSIZE_STATIC = IDD_PREFERENCE_SETTING_BOX + 3,
        IDC_HISTORY_GB_STATIC = IDD_PREFERENCE_SETTING_BOX + 4,
        IDC_CHECK_DONTCHECKHISTORY = IDD_PREFERENCE_SETTING_BOX + 5,
        IDC_MAXNBFILE_STATIC = IDD_PREFERENCE_SETTING_BOX + 6,
        IDC_CHECK_MIN2SYSTRAY = IDD_PREFERENCE_SETTING_BOX + 8,
        IDC_CHECK_REMEMBERSESSION = IDD_PREFERENCE_SETTING_BOX + 9,
        IDC_TABSIZEVAL_STATIC = IDD_PREFERENCE_SETTING_BOX + 10,
        IDC_MAXNBFILEVAL_STATIC = IDD_PREFERENCE_SETTING_BOX + 11,
        IDC_FILEAUTODETECTION_STATIC = IDD_PREFERENCE_SETTING_BOX + 12,
        IDC_CHECK_UPDATESILENTLY = IDD_PREFERENCE_SETTING_BOX + 13,
        IDC_RADIO_BKNONE = IDD_PREFERENCE_SETTING_BOX + 15,
        IDC_RADIO_BKSIMPLE = IDD_PREFERENCE_SETTING_BOX + 16,
        IDC_RADIO_BKVERBOSE = IDD_PREFERENCE_SETTING_BOX + 17,
        IDC_CLICKABLELINK_STATIC = IDD_PREFERENCE_SETTING_BOX + 18,
        IDC_CHECK_CLICKABLELINK_ENABLE = IDD_PREFERENCE_SETTING_BOX + 19,
        IDC_CHECK_CLICKABLELINK_NOUNDERLINE = IDD_PREFERENCE_SETTING_BOX + 20,
        IDC_EDIT_SESSIONFILEEXT = IDD_PREFERENCE_SETTING_BOX + 21,
        IDC_SESSIONFILEEXT_STATIC = IDD_PREFERENCE_SETTING_BOX + 22,
        IDC_CHECK_AUTOUPDATE = IDD_PREFERENCE_SETTING_BOX + 23,
        IDC_DOCUMENTSWITCHER_STATIC = IDD_PREFERENCE_SETTING_BOX + 24,
        IDC_CHECK_UPDATEGOTOEOF = IDD_PREFERENCE_SETTING_BOX + 25,
        IDC_CHECK_ENABLSMARTHILITE = IDD_PREFERENCE_SETTING_BOX + 26,
        IDC_CHECK_ENABLTAGSMATCHHILITE = IDD_PREFERENCE_SETTING_BOX + 27,
        IDC_CHECK_ENABLTAGATTRHILITE = IDD_PREFERENCE_SETTING_BOX + 28,
        IDC_TAGMATCHEDHILITE_STATIC = IDD_PREFERENCE_SETTING_BOX + 29,
        IDC_CHECK_HIGHLITENONEHTMLZONE = IDD_PREFERENCE_SETTING_BOX + 30,
        IDC_CHECK_SHORTTITLE = IDD_PREFERENCE_SETTING_BOX + 31,
        IDC_CHECK_SMARTHILITECASESENSITIVE = IDD_PREFERENCE_SETTING_BOX + 32,
        IDC_SMARTHILITING_STATIC = IDD_PREFERENCE_SETTING_BOX + 33,
        IDC_CHECK_DETECTENCODING = IDD_PREFERENCE_SETTING_BOX + 34,
        IDC_CHECK_BACKSLASHISESCAPECHARACTERFORSQL = IDD_PREFERENCE_SETTING_BOX + 35,
        IDC_EDIT_WORKSPACEFILEEXT = IDD_PREFERENCE_SETTING_BOX + 36,
        IDC_WORKSPACEFILEEXT_STATIC = IDD_PREFERENCE_SETTING_BOX + 37,
        IDC_CHECK_SMARTHILITEWHOLEWORDONLY = IDD_PREFERENCE_SETTING_BOX + 38,
        IDC_CHECK_SMARTHILITEUSEFINDSETTINGS = IDD_PREFERENCE_SETTING_BOX + 39,
        IDC_CHECK_SMARTHILITEANOTHERRVIEW = IDD_PREFERENCE_SETTING_BOX + 40,

        IDC_CHECK_REMEMBEREDITVIEWPERFILE = IDD_PREFERENCE_SETTING_BOX + 41,
        IDC_REMEMBEREDITVIEWPERFILE_STATIC = IDD_PREFERENCE_SETTING_BOX + 42,
        IDC_EDIT_REMEMBEREDITVIEWPERFILE = IDD_PREFERENCE_SETTING_BOX + 43,

        IDC_DOCUMENTPEEK_STATIC = IDD_PREFERENCE_SETTING_BOX + 44,
        IDC_CHECK_ENABLEDOCPEEKER = IDD_PREFERENCE_SETTING_BOX + 45,
        IDC_CHECK_ENABLEDOCPEEKONMAP = IDD_PREFERENCE_SETTING_BOX + 46,
        IDC_COMBO_FILEUPDATECHOICE = IDD_PREFERENCE_SETTING_BOX + 47,
        IDC_CHECK_DIRECTWRITE_ENABLE = IDD_PREFERENCE_SETTING_BOX + 49,

        IDD_PREFERENCE_NEWDOCSETTING_BOX = 6400,
        IDC_FORMAT_GB_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 1,
        IDC_RADIO_F_WIN = IDD_PREFERENCE_NEWDOCSETTING_BOX + 2,
        IDC_RADIO_F_UNIX = IDD_PREFERENCE_NEWDOCSETTING_BOX + 3,
        IDC_RADIO_F_MAC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 4,
        IDC_ENCODING_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 5,
        IDC_RADIO_ANSI = IDD_PREFERENCE_NEWDOCSETTING_BOX + 6,
        IDC_RADIO_UTF8SANSBOM = IDD_PREFERENCE_NEWDOCSETTING_BOX + 7,
        IDC_RADIO_UTF8 = IDD_PREFERENCE_NEWDOCSETTING_BOX + 8,
        IDC_RADIO_UCS2BIG = IDD_PREFERENCE_NEWDOCSETTING_BOX + 9,
        IDC_RADIO_UCS2SMALL = IDD_PREFERENCE_NEWDOCSETTING_BOX + 10,
        IDC_DEFAULTLANG_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 11,
        IDC_COMBO_DEFAULTLANG = IDD_PREFERENCE_NEWDOCSETTING_BOX + 12,
        IDC_OPENSAVEDIR_GR_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 13,
        IDC_OPENSAVEDIR_FOLLOWCURRENT_RADIO = IDD_PREFERENCE_NEWDOCSETTING_BOX + 14,
        IDC_OPENSAVEDIR_REMEMBERLAST_RADIO = IDD_PREFERENCE_NEWDOCSETTING_BOX + 15,
        IDC_OPENSAVEDIR_ALWAYSON_RADIO = IDD_PREFERENCE_NEWDOCSETTING_BOX + 16,
        IDC_OPENSAVEDIR_ALWAYSON_EDIT = IDD_PREFERENCE_NEWDOCSETTING_BOX + 17,
        IDD_OPENSAVEDIR_ALWAYSON_BROWSE_BUTTON = IDD_PREFERENCE_NEWDOCSETTING_BOX + 18,
        IDC_NEWDOCUMENT_GR_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 19,
        IDC_CHECK_OPENANSIASUTF8 = IDD_PREFERENCE_NEWDOCSETTING_BOX + 20,
        IDC_RADIO_OTHERCP = IDD_PREFERENCE_NEWDOCSETTING_BOX + 21,
        IDC_COMBO_OTHERCP = IDD_PREFERENCE_NEWDOCSETTING_BOX + 22,
        IDC_GP_STATIC_RECENTFILES = IDD_PREFERENCE_NEWDOCSETTING_BOX + 23,
        IDC_CHECK_INSUBMENU = IDD_PREFERENCE_NEWDOCSETTING_BOX + 24,
        IDC_RADIO_ONLYFILENAME = IDD_PREFERENCE_NEWDOCSETTING_BOX + 25,
        IDC_RADIO_FULLFILENAMEPATH = IDD_PREFERENCE_NEWDOCSETTING_BOX + 26,
        IDC_RADIO_CUSTOMIZELENTH = IDD_PREFERENCE_NEWDOCSETTING_BOX + 27,
        IDC_CUSTOMIZELENGTHVAL_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 28,
        IDC_DISPLAY_STATIC = IDD_PREFERENCE_NEWDOCSETTING_BOX + 29,
        IDC_OPENSAVEDIR_CHECK_USENEWSTYLESAVEDIALOG = IDD_PREFERENCE_NEWDOCSETTING_BOX + 30,
        IDC_OPENSAVEDIR_CHECK_DRROPFOLDEROPENFILES = IDD_PREFERENCE_NEWDOCSETTING_BOX + 31,

        IDD_PREFERENCE_DEFAULTDIRECTORY_BOX = 6450,
        IDD_PREFERENCE_RECENTFILESHISTORY_BOX = 6460,

        IDD_PREFERENCE_LANG_BOX = 6500,
        IDC_LIST_ENABLEDLANG = IDD_PREFERENCE_LANG_BOX + 1,
        IDC_LIST_DISABLEDLANG = IDD_PREFERENCE_LANG_BOX + 2,
        IDC_BUTTON_REMOVE = IDD_PREFERENCE_LANG_BOX + 3,
        IDC_BUTTON_RESTORE = IDD_PREFERENCE_LANG_BOX + 4,
        IDC_ENABLEDITEMS_STATIC = IDD_PREFERENCE_LANG_BOX + 5,
        IDC_DISABLEDITEMS_STATIC = IDD_PREFERENCE_LANG_BOX + 6,
        IDC_CHECK_LANGMENUCOMPACT = IDD_PREFERENCE_LANG_BOX + 7,
        IDC_CHECK_LANGMENU_GR_STATIC = IDD_PREFERENCE_LANG_BOX + 8,
        IDC_LIST_TABSETTNG = IDD_PREFERENCE_LANG_BOX + 9,
        IDC_CHECK_DEFAULTTABVALUE = IDD_PREFERENCE_LANG_BOX + 10,
        IDC_GR_TABVALUE_STATIC = IDD_PREFERENCE_LANG_BOX + 11,
        IDC_TABSIZEVAL_DISABLE_STATIC = IDD_PREFERENCE_LANG_BOX + 12,
        IDD_PREFERENCE_HILITE_BOX = 6550,

        IDD_PREFERENCE_PRINT_BOX = 6600,
        IDC_CHECK_PRINTLINENUM = IDD_PREFERENCE_PRINT_BOX + 1,
        IDC_COLOUROPT_STATIC = IDD_PREFERENCE_PRINT_BOX + 2,
        IDC_RADIO_WYSIWYG = IDD_PREFERENCE_PRINT_BOX + 3,
        IDC_RADIO_INVERT = IDD_PREFERENCE_PRINT_BOX + 4,
        IDC_RADIO_BW = IDD_PREFERENCE_PRINT_BOX + 5,
        IDC_RADIO_NOBG = IDD_PREFERENCE_PRINT_BOX + 6,
        IDC_MARGESETTINGS_STATIC = IDD_PREFERENCE_PRINT_BOX + 7,
        IDC_EDIT_ML = IDD_PREFERENCE_PRINT_BOX + 8,
        IDC_EDIT_MT = IDD_PREFERENCE_PRINT_BOX + 9,
        IDC_EDIT_MR = IDD_PREFERENCE_PRINT_BOX + 10,
        IDC_EDIT_MB = IDD_PREFERENCE_PRINT_BOX + 11,
        IDC_ML_STATIC = IDD_PREFERENCE_PRINT_BOX + 12,
        IDC_MT_STATIC = IDD_PREFERENCE_PRINT_BOX + 13,
        IDC_MR_STATIC = IDD_PREFERENCE_PRINT_BOX + 14,
        IDC_MB_STATIC = IDD_PREFERENCE_PRINT_BOX + 15,

        IDD_PREFERENCE_PRINT2_BOX = 6700,
        IDC_EDIT_HLEFT = IDD_PREFERENCE_PRINT2_BOX + 1,
        IDC_EDIT_HMIDDLE = IDD_PREFERENCE_PRINT2_BOX + 2,
        IDC_EDIT_HRIGHT = IDD_PREFERENCE_PRINT2_BOX + 3,
        IDC_COMBO_HFONTNAME = IDD_PREFERENCE_PRINT2_BOX + 4,
        IDC_COMBO_HFONTSIZE = IDD_PREFERENCE_PRINT2_BOX + 5,
        IDC_CHECK_HBOLD = IDD_PREFERENCE_PRINT2_BOX + 6,
        IDC_CHECK_HITALIC = IDD_PREFERENCE_PRINT2_BOX + 7,
        IDC_HGB_STATIC = IDD_PREFERENCE_PRINT2_BOX + 8,
        IDC_HL_STATIC = IDD_PREFERENCE_PRINT2_BOX + 9,
        IDC_HM_STATIC = IDD_PREFERENCE_PRINT2_BOX + 10,
        IDC_HR_STATIC = IDD_PREFERENCE_PRINT2_BOX + 11,
        IDC_EDIT_FLEFT = IDD_PREFERENCE_PRINT2_BOX + 12,
        IDC_EDIT_FMIDDLE = IDD_PREFERENCE_PRINT2_BOX + 13,
        IDC_EDIT_FRIGHT = IDD_PREFERENCE_PRINT2_BOX + 14,
        IDC_COMBO_FFONTNAME = IDD_PREFERENCE_PRINT2_BOX + 15,
        IDC_COMBO_FFONTSIZE = IDD_PREFERENCE_PRINT2_BOX + 16,
        IDC_CHECK_FBOLD = IDD_PREFERENCE_PRINT2_BOX + 17,
        IDC_CHECK_FITALIC = IDD_PREFERENCE_PRINT2_BOX + 18,
        IDC_FGB_STATIC = IDD_PREFERENCE_PRINT2_BOX + 19,
        IDC_FL_STATIC = IDD_PREFERENCE_PRINT2_BOX + 20,
        IDC_FM_STATIC = IDD_PREFERENCE_PRINT2_BOX + 21,
        IDC_FR_STATIC = IDD_PREFERENCE_PRINT2_BOX + 22,
        IDC_BUTTON_ADDVAR = IDD_PREFERENCE_PRINT2_BOX + 23,
        IDC_COMBO_VARLIST = IDD_PREFERENCE_PRINT2_BOX + 24,
        IDC_VAR_STATIC = IDD_PREFERENCE_PRINT2_BOX + 25,
        IDC_VIEWPANEL_STATIC = IDD_PREFERENCE_PRINT2_BOX + 26,
        IDC_WHICHPART_STATIC = IDD_PREFERENCE_PRINT2_BOX + 27,
        IDC_HEADERFPPTER_GR_STATIC = IDD_PREFERENCE_PRINT2_BOX + 28,

        IDD_PREFERENCE_BACKUP_BOX = 6800,
        IDC_BACKUPDIR_GRP_STATIC = IDD_PREFERENCE_BACKUP_BOX + 1,
        IDC_BACKUPDIR_CHECK = IDD_PREFERENCE_BACKUP_BOX + 2,
        IDD_BACKUPDIR_STATIC = IDD_PREFERENCE_BACKUP_BOX + 3,
        IDC_BACKUPDIR_USERCUSTOMDIR_GRPSTATIC = IDD_PREFERENCE_BACKUP_BOX + 4,
        IDC_BACKUPDIR_EDIT = IDD_PREFERENCE_BACKUP_BOX + 5,
        IDD_BACKUPDIR_BROWSE_BUTTON = IDD_PREFERENCE_BACKUP_BOX + 6,
        IDD_AUTOC_GRPSTATIC = IDD_PREFERENCE_BACKUP_BOX + 7,
        IDD_AUTOC_ENABLECHECK = IDD_PREFERENCE_BACKUP_BOX + 8,
        IDD_AUTOC_FUNCRADIO = IDD_PREFERENCE_BACKUP_BOX + 9,
        IDD_AUTOC_WORDRADIO = IDD_PREFERENCE_BACKUP_BOX + 10,
        IDD_AUTOC_STATIC_FROM = IDD_PREFERENCE_BACKUP_BOX + 11,
        IDD_AUTOC_STATIC_N = IDD_PREFERENCE_BACKUP_BOX + 12,
        IDD_AUTOC_STATIC_CHAR = IDD_PREFERENCE_BACKUP_BOX + 13,
        IDD_AUTOC_STATIC_NOTE = IDD_PREFERENCE_BACKUP_BOX + 14,
        IDD_FUNC_CHECK = IDD_PREFERENCE_BACKUP_BOX + 15,
        IDD_AUTOC_BOTHRADIO = IDD_PREFERENCE_BACKUP_BOX + 16,
        IDC_BACKUPDIR_RESTORESESSION_GRP_STATIC = IDD_PREFERENCE_BACKUP_BOX + 17,
        IDC_BACKUPDIR_RESTORESESSION_CHECK = IDD_PREFERENCE_BACKUP_BOX + 18,
        IDD_BACKUPDIR_RESTORESESSION_STATIC1 = IDD_PREFERENCE_BACKUP_BOX + 19,
        IDC_BACKUPDIR_RESTORESESSION_EDIT = IDD_PREFERENCE_BACKUP_BOX + 20,
        IDD_BACKUPDIR_RESTORESESSION_STATIC2 = IDD_PREFERENCE_BACKUP_BOX + 21,
        IDD_BACKUPDIR_RESTORESESSION_PATHLABEL_STATIC = IDD_PREFERENCE_BACKUP_BOX + 22,
        IDD_BACKUPDIR_RESTORESESSION_PATH_EDIT = IDD_PREFERENCE_BACKUP_BOX + 23,
        IDD_AUTOC_IGNORENUMBERS = IDD_PREFERENCE_BACKUP_BOX + 24,

        IDD_PREFERENCE_AUTOCOMPLETION_BOX = 6850,
        IDD_AUTOCINSERT_GRPSTATIC = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 1,
        IDD_AUTOCPARENTHESES_CHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 2,
        IDD_AUTOCBRACKET_CHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 3,
        IDD_AUTOCCURLYBRACKET_CHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 4,
        IDD_AUTOC_DOUBLEQUOTESCHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 5,
        IDD_AUTOC_QUOTESCHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 6,
        IDD_AUTOCTAG_CHECK = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 7,
        IDC_MACHEDPAIROPEN_STATIC = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 8,
        IDC_MACHEDPAIRCLOSE_STATIC = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 9,
        IDC_MACHEDPAIR_STATIC1 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 10,
        IDC_MACHEDPAIROPEN_EDIT1 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 11,
        IDC_MACHEDPAIRCLOSE_EDIT1 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 12,
        IDC_MACHEDPAIR_STATIC2 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 13,
        IDC_MACHEDPAIROPEN_EDIT2 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 14,
        IDC_MACHEDPAIRCLOSE_EDIT2 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 15,
        IDC_MACHEDPAIR_STATIC3 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 16,
        IDC_MACHEDPAIROPEN_EDIT3 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 17,
        IDC_MACHEDPAIRCLOSE_EDIT3 = IDD_PREFERENCE_AUTOCOMPLETION_BOX + 18,

        IDD_PREFERENCE_SEARCHINGSETTINGS_BOX = 6900,
        IDC_CHECK_STOPFILLINGFINDFIELD = IDD_PREFERENCE_SEARCHINGSETTINGS_BOX + 1,
        IDC_CHECK_MONOSPACEDFONT_FINDDLG = IDD_PREFERENCE_SEARCHINGSETTINGS_BOX + 2,

		/* --Autogenerated -- end of section automatically generated from preference_rc.h */
	}
}
