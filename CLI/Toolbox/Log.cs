using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toolbox.UI;

namespace Toolbox;

/// <summary>
///     Representing the internal logging class.
///     The resulting log file is in the comma-separated values (CSV) format separated by semicolons (;).
/// </summary>
public static class Log
{
    /// <summary>
    ///     Representing the log entry type.
    /// </summary>
    public enum LogType : byte
    {
        /// <summary>
        ///     Other unspecified entry type.
        /// </summary>
        Other = 0,

        /// <summary>
        ///     The error entry type.
        /// </summary>
        Error,

        /// <summary>
        ///     The warning entry type.
        /// </summary>
        Warning,

        /// <summary>
        ///     The informational entry type.
        /// </summary>
        Information,

        /// <summary>
        ///     The unhandled exception entry type.
        /// </summary>
        Exception,

        /// <summary>
        ///     The critical entry type.
        /// </summary>
        Critical,

        /// <summary>
        ///     The success entry type.
        /// </summary>
        Success
    }

    private const string FILENAME = "Toolbox.log";
    private const char SEPARATOR = ';';

    private static Entry _lastEntry;

    /// <summary>
    ///     Representing the names for the log entry types (<see cref="LogType" />).
    /// </summary>
    public static Dictionary<LogType, string> TypeNames => new()
    {
        { LogType.Other, "GENERIC" },
        { LogType.Error, "ERROR" },
        { LogType.Warning, "WARNING" },
        { LogType.Information, "INFORMATION" },
        { LogType.Exception, "EXCEPTION" },
        { LogType.Critical, "CRITICAL" },
        { LogType.Success, "SUCCESS" }
    };

    /// <summary>
    ///     Representing the ANSI formatted names/abbreviations for the log entry types (<see cref="LogType" />).
    /// </summary>
    public static Dictionary<LogType, string> TypeNamesFormatted => new()
    {
        { LogType.Other, $"{Terminal.AccentHighlightStyle} GNRL {ANSI.ANSI_RESET}" },
        { LogType.Error, $"\e[48;5;197m FAIL {ANSI.ANSI_RESET}" },
        { LogType.Warning, $"\e[48;5;226m\e[38;5;0m WARN {ANSI.ANSI_RESET}" },
        { LogType.Information, $"\e[48;5;21m INFO {ANSI.ANSI_RESET}" },
        { LogType.Exception, $"\e[48;5;200m EXCP {ANSI.ANSI_RESET}" },
        { LogType.Critical, $"\e[48;5;196m CRIT {ANSI.ANSI_RESET}" },
        { LogType.Success, $"\e[48;5;46m\e[38;5;0m GOOD {ANSI.ANSI_RESET}" }
    };

    /// <summary>
    ///     Retrieves the last logged entry.
    /// </summary>
    /// <returns>
    ///     The last logged entry or an empty <see cref="Entry" /> object if no log entry was written yet.
    /// </returns>
    public static Entry GetLastEntry()
    {
        return Log._lastEntry;
    }

    private static bool WriteEntry(LogType type, string? tag, string message)
    {
        /*
         *  Format: DATE;TYPE;TAG;MESSAGE
         *  Where
         *      DATE:       The date and time at the moment of the method call
         *      TYPE:       Type of the log entry
         *      TAG:        Information about the origin of the log entry, i.e. name of the calling method
         *      MESSAGE:    The log message itself
         */

        try
        {
            Entry entry;
            entry.Timestamp = DateTime.Now;
            entry.EntryType = type;
            entry.Tag = tag ?? "GENERIC";
            entry.Message = message;

            StringBuilder sb = new();

            // add the date
            _ = sb.Append(value: entry.Timestamp);
            _ = sb.Append(value: Log.SEPARATOR);

            // add the type
            switch (type)
            {
                case LogType.Error:
                    _ = sb.Append(value: "ERROR");
                    break;

                case LogType.Warning:
                    _ = sb.Append(value: "WARNING");
                    break;

                case LogType.Information:
                    _ = sb.Append(value: "INFORMATION");
                    break;

                case LogType.Exception:
                    _ = sb.Append(value: "EXCEPTION");
                    break;

                case LogType.Critical:
                    _ = sb.Append(value: "CRITICAL");
                    break;

                case LogType.Success:
                    _ = sb.Append(value: "SUCCESS");
                    break;

                default:
                    _ = sb.Append(value: "GENERIC");
                    break;
            }

            _ = sb.Append(value: Log.SEPARATOR);

            // add the tag
            _ = sb.Append(value: entry.Tag);
            _ = sb.Append(value: Log.SEPARATOR);

            // add the message + the new line separator
            _ = sb.AppendLine(value: entry.Message);

            // write the entry to the log file
            File.AppendAllText(path: Log.FILENAME, contents: sb.ToString(), encoding: Encoding.Unicode);

            // assign the last entry
            Log._lastEntry = entry;
            return true;
        }

        catch
        {
            // error so severe it cannot even log itself
            return false;
        }
    }

    /// <summary>
    ///     Writes an error message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Error(string message, string tag)
    {
        return Log.WriteEntry(type: LogType.Error, tag: tag, message: message);
    }

    /// <summary>
    ///     Writes an error message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Error(string message)
    {
        return Log.WriteEntry(type: LogType.Error, tag: null, message: message);
    }

    /// <summary>
    ///     Writes a warning message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Warning(string message, string tag)
    {
        return Log.WriteEntry(type: LogType.Warning, tag: tag, message: message);
    }

    /// <summary>
    ///     Writes a warning message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Warning(string message)
    {
        return Log.WriteEntry(type: LogType.Warning, tag: null, message: message);
    }

    /// <summary>
    ///     Writes an informational message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Information(string message, string tag)
    {
        return Log.WriteEntry(type: LogType.Information, tag: tag, message: message);
    }

    /// <summary>
    ///     Writes an informational message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Information(string message)
    {
        return Log.WriteEntry(type: LogType.Information, tag: null, message: message);
    }

    /// <summary>
    ///     Writes an exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Exception(Exception exception, string tag)
    {
        return Log.WriteEntry(type: LogType.Exception, tag: tag, message: exception.ToString());
    }

    /// <summary>
    ///     Writes an exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <returns>Operation result.</returns>
    public static bool Exception(Exception exception)
    {
        return Log.WriteEntry(type: LogType.Exception, tag: null, message: exception.ToString());
    }

    /// <summary>
    ///     Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(string message, string tag)
    {
        return Log.WriteEntry(type: LogType.Critical, tag: tag, message: message);
    }

    /// <summary>
    ///     Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(string message)
    {
        return Log.WriteEntry(type: LogType.Critical, tag: null, message: message);
    }

    /// <summary>
    ///     Writes a critical exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(Exception exception, string tag)
    {
        return Log.WriteEntry(type: LogType.Critical, tag: tag, message: exception.ToString());
    }

    /// <summary>
    ///     Writes a critical exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(Exception exception)
    {
        return Log.WriteEntry(type: LogType.Critical, tag: null, message: exception.ToString());
    }

    /// <summary>
    ///     Writes a successful message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Success(string message, string tag)
    {
        return Log.WriteEntry(type: LogType.Success, tag: tag, message: message);
    }

    /// <summary>
    ///     Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Success(string message)
    {
        return Log.WriteEntry(type: LogType.Success, tag: null, message: message);
    }

    /// <summary>
    ///     Representing a log entry.
    /// </summary>
    public struct Entry
    {
        /// <summary>
        ///     Representing the timestamp of the entry.
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        ///     Representing the log type.
        /// </summary>
        public LogType EntryType;

        /// <summary>
        ///     Representing the entry's tag.
        /// </summary>
        public string Tag;

        /// <summary>
        ///     Representing the log message.
        /// </summary>
        public string Message;
    }
}