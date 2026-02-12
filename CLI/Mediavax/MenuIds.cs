namespace Mediavax;

/// <summary>
///     Menu item IDs.
/// </summary>
internal enum MenuIds
{
    // main menu
    ID_EXIT = 0,
    ID_SELECT_URL,
    ID_SELECT_MODE,
    ID_SELECT_QUALITY,
    ID_SELECT_FORMAT,
    ID_SELECT_LOCATION,
    ID_START_DOWNLOAD,
    ID_EXTRAS,

    // extras
    ID_IMPORT_COOKIES,
    ID_BROWSER_NONE,
    ID_BROWSER_CHROME,
    ID_BROWSER_CHROMIUM,
    ID_BROWSER_EDGE,
    ID_BROWSER_OPERA,
    ID_BROWSER_VIVALDI,
    ID_BROWSER_FIREFOX,
    ID_BROWSER_SAFARI,
    ID_BROWSER_WHALE,
    ID_BROWSER_BRAVE,

    // more options
    ID_UPDATE_YTDLP,
    ID_CUSTOM_ARGUMENTS,
    ID_YT_DLP_VERSION,

    // formats options
    ID_FORMATS_COMBINED,
    ID_FORMATS_AUDIO,
    ID_FORMATS_VIDEO,
    ID_FORMATS_EXTRA,

    // other options
    ID_SELECT_THEME
}