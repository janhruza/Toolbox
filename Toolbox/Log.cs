using System;
using System.IO;
using System.Text;

namespace Toolbox;

/// <summary>
/// Representing the internal logging class.
/// The resulting log file is in the comma-separated values (CSV) format separated by semicolons (;).
/// </summary>
public static class Log
{
    private enum LogType : byte
    {
        Error = 0,
        Warning,
        Information,
        Exception,
        Critical,
        Success
    };

    const string FILENAME = "Toolbox.log";
    const char SEPARATOR = ';';

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
            StringBuilder sb = new StringBuilder();

            // add the date
            sb.Append(DateTime.Now);
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
            sb.Append(tag ?? "GENERIC");

            // add the message + the new line separator
            sb.AppendLine(message);

            // write the entry to the log file
            File.AppendAllText(FILENAME, sb.ToString(), Encoding.Unicode);
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
