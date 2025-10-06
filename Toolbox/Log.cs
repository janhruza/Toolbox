using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Toolbox.UI;

using static Toolbox.ANSI;

namespace Toolbox;

/// <summary>
/// Representing the internal logging class.
/// The resulting log file is in the comma-separated values (CSV) format separated by semicolons (;).
/// </summary>
public static class Log
{
    /// <summary>
    /// Representing the log entry type.
    /// </summary>
    public enum LogType : byte
    {
        /// <summary>
        /// Other unspecified entry type.
        /// </summary>
        Other = 0,

        /// <summary>
        /// The error entry type.
        /// </summary>
        Error,

        /// <summary>
        /// The warning entry type.
        /// </summary>
        Warning,

        /// <summary>
        /// The informational entry type.
        /// </summary>
        Information,

        /// <summary>
        /// The unhandled exception entry type.
        /// </summary>
        Exception,

        /// <summary>
        /// The critical entry type.
        /// </summary>
        Critical,

        /// <summary>
        /// The success entry type.
        /// </summary>
        Success
    };

    /// <summary>
    /// Representing the names for the log entry types (<see cref="LogType"/>).
    /// </summary>
    public static Dictionary<LogType, string> TypeNames => new Dictionary<LogType, string>
    {
        { LogType.Other, "GENERIC" },
        { LogType.Error, "ERROR" },
        { LogType.Warning, "WARNING" },
        { LogType.Information, "INFORMATION" },
        { LogType.Exception, "EXCEPTION" },
        { LogType.Critical, "CRITICAL" },
        { LogType.Success, "SUCCESS" },
    };

    /// <summary>
    /// Representing the ANSI formatted names/abbreviations for the log entry types (<see cref="LogType"/>).
    /// </summary>
    public static Dictionary<LogType, string> TypeNamesFormatted => new Dictionary<LogType, string>
    {
        { LogType.Other, $"{Terminal.AccentHighlightStyle} GNRL {ANSI_RESET}" },
        { LogType.Error, $"\e[48;5;197m FAIL {ANSI_RESET}" },
        { LogType.Warning, $"\e[48;5;226m\e[38;5;0m WARN {ANSI_RESET}" },
        { LogType.Information, $"\e[48;5;21m INFO {ANSI_RESET}" },
        { LogType.Exception, $"\e[48;5;200m EXCP {ANSI_RESET}" },
        { LogType.Critical, $"\e[48;5;196m CRIT {ANSI_RESET}" },
        { LogType.Success, $"\e[48;5;46m\e[38;5;0m GOOD {ANSI_RESET}" },
    };

    /// <summary>
    /// Representing a log entry.
    /// </summary>
    public struct Entry
    {
        /// <summary>
        /// Representing the timestamp of the entry.
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// Representing the log type.
        /// </summary>
        public LogType EntryType;

        /// <summary>
        /// Representing the entry's tag.
        /// </summary>
        public string Tag;

        /// <summary>
        /// Representing the log message.
        /// </summary>
        public string Message;
    }

    const string FILENAME = "Toolbox.log";
    const char SEPARATOR = ';';

    private static Entry _lastEntry;

    /// <summary>
    /// Retrieves the last logged entry.
    /// </summary>
    /// <returns>
    /// The last logged entry or an empty <see cref="Entry"/> object if no log entry was written yet.
    /// </returns>
    public static Entry GetLastEntry() => _lastEntry;

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

            StringBuilder sb = new StringBuilder();

            // add the date
            sb.Append(entry.Timestamp);
            sb.Append(SEPARATOR);

            // add the type
            switch (type)
            {
                case LogType.Error:
                    sb.Append("ERROR");
                    break;

                case LogType.Warning:
                    sb.Append("WARNING");
                    break;

                case LogType.Information:
                    sb.Append("INFORMATION");
                    break;

                case LogType.Exception:
                    sb.Append("EXCEPTION");
                    break;

                case LogType.Critical:
                    sb.Append("CRITICAL");
                    break;

                case LogType.Success:
                    sb.Append("SUCCESS");
                    break;

                default:
                    sb.Append("GENERIC");
                    break;
            }

            sb.Append(SEPARATOR);

            // add the tag
            sb.Append(entry.Tag);
            sb.Append(SEPARATOR);

            // add the message + the new line separator
            sb.AppendLine(entry.Message);

            // write the entry to the log file
            File.AppendAllText(FILENAME, sb.ToString(), Encoding.Unicode);

            // assign the last entry
            _lastEntry = entry;
            return true;
        }

        catch
        {
            // error so severe it cannot even log itself
            return false;
        }
    }

    /// <summary>
    /// Writes an error message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Error(string message, string tag)
    {
        return WriteEntry(LogType.Error, tag, message);
    }

    /// <summary>
    /// Writes an error message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Error(string message)
    {
        return WriteEntry(LogType.Error, null, message);
    }

    /// <summary>
    /// Writes a warning message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Warning(string message, string tag)
    {
        return WriteEntry(LogType.Warning, tag, message);
    }

    /// <summary>
    /// Writes a warning message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Warning(string message)
    {
        return WriteEntry(LogType.Warning, null, message);
    }

    /// <summary>
    /// Writes an informational message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Information(string message, string tag)
    {
        return WriteEntry(LogType.Information, tag, message);
    }

    /// <summary>
    /// Writes an informational message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Information(string message)
    {
        return WriteEntry(LogType.Information, null, message);
    }

    /// <summary>
    /// Writes an exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Exception(Exception exception, string tag)
    {
        return WriteEntry(LogType.Exception, tag, exception.ToString());
    }

    /// <summary>
    /// Writes an exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <returns>Operation result.</returns>
    public static bool Exception(Exception exception)
    {
        return WriteEntry(LogType.Exception, null, exception.ToString());
    }

    /// <summary>
    /// Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(string message, string tag)
    {
        return WriteEntry(LogType.Critical, tag, message);
    }

    /// <summary>
    /// Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(string message)
    {
        return WriteEntry(LogType.Critical, null, message);
    }

    /// <summary>
    /// Writes a critical exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(Exception exception, string tag)
    {
        return WriteEntry(LogType.Critical, tag, exception.ToString());
    }

    /// <summary>
    /// Writes a critical exception to the log file.
    /// </summary>
    /// <param name="exception">Unhandled exception.</param>
    /// <returns>Operation result.</returns>
    public static bool Critical(Exception exception)
    {
        return WriteEntry(LogType.Critical, null, exception.ToString());
    }

    /// <summary>
    /// Writes a successful message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <param name="tag">Entry tag.</param>
    /// <returns>Operation result.</returns>
    public static bool Success(string message, string tag)
    {
        return WriteEntry(LogType.Success, tag, message);
    }

    /// <summary>
    /// Writes a critical message to the log file.
    /// </summary>
    /// <param name="message">Entry message.</param>
    /// <returns>Operation result.</returns>
    public static bool Success(string message)
    {
        return WriteEntry(LogType.Success, null, message);
    }
}
