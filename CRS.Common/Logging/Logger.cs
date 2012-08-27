using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace CRS.Common.Logging
{
    /// <summary>
    /// Performs logging operations
    /// </summary>
    public static class Logger
    {
        private const string ErrorCategory = "Error";
        private const string GeneralCategory = "General";

        public static void Log(string message, ICollection<string> categories, TraceEventType severity)
        {
            LogEntry logEntry = new LogEntry { Message = message, Categories = categories, Severity = severity };
            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
        }

        /// <summary>
        /// Logs a message as error
        /// </summary>
        public static void Error(string message)
        {
            Log(message, new[] { ErrorCategory }, TraceEventType.Error);
        }

        /// <summary>
        /// Logs an exception as error
        /// </summary>
        public static void Error(Exception exception)
        {
            Error(exception.ToString());
        }

        /// <summary>
        /// Logs a message as warning
        /// </summary>
        public static void Warning(string message)
        {
            Log(message, new[] { GeneralCategory }, TraceEventType.Warning);
        }

        /// <summary>
        /// Logs a message as info
        /// </summary>
        public static void Info(string message)
        {
            Log(message, new[] { GeneralCategory }, TraceEventType.Information);
        }
    }
}