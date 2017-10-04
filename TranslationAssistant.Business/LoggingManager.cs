// // ----------------------------------------------------------------------
// // <copyright file="LoggingManager.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>LoggingManager.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.Business
{
    #region

    using System;
    using System.IO;

    // using global::TranslationAssistant.AutomationToolkit.BasePlugin;

    #endregion

    public static class LoggingManager
    {
        #region Public Methods and Operators

        public static void LogError(string message)
        {
            WriteMessage(LogLevel.Error, message);
        }

        public static void LogMessage(string message)
        {
            WriteMessage(LogLevel.Msg, message);
        }

        private static void WriteMessage(LogLevel logLevel, string message)
        {
            var timestamp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " : ";
            var messageToLog = timestamp + message;
            new ConsoleLogger().WriteLine(logLevel, messageToLog);
            try
            {
                File.AppendAllText("Log.txt", messageToLog + Environment.NewLine);
            }
            catch (IOException e)
            {
                new ConsoleLogger().WriteLine(LogLevel.Warning, "Warning: The process cannot access the log file because it is being used by another process.");
            }
        }

        #endregion
    }

    public enum LogLevel
    {
        /// <summary>
        ///     The none.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The debug.
        /// </summary>
        Debug = 1,

        /// <summary>
        ///     The msg.
        /// </summary>
        Msg = 2,

        /// <summary>
        ///     The warning.
        /// </summary>
        Warning = 3,

        /// <summary>
        ///     The error.
        /// </summary>
        Error = 4
    }


    public interface ILogger
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The write.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="msg">
        ///     The msg.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        void Write(LogLevel level, string msg, params object[] arguments);

        /// <summary>
        ///     The write exception.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        void WriteException(Exception ex);

        /// <summary>
        ///     The write line.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="msg">
        ///     The msg.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        void WriteLine(LogLevel level, string msg, params object[] arguments);

        #endregion
    }
    /// <summary>
    ///     The console logger.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The write.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="line">
        ///     The line.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        public void Write(LogLevel level, string line, params object[] args)
        {
            this.SetConsoleColor(level);
            if (args != null && args.Length > 0)
            {
                Console.Write(line, args);
            }
            else
            {
                Console.Write(line);
            }

            Console.ResetColor();
        }

        /// <summary>
        ///     The write exception.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        public void WriteException(Exception e)
        {
            this.WriteLine(LogLevel.Error, "ERROR: An error has occurred.");
            this.WriteLine(LogLevel.Error, "**************************************");
            this.WriteLine(LogLevel.Error, "MESSAGE  : {0}", e.Message);
            this.WriteLine(LogLevel.Error, "SOURCE   : {0}", e.Source);
            this.WriteLine(LogLevel.Error, "STACK    : {0}", e.StackTrace);
            this.WriteLine(LogLevel.Error, "TARGET   : {0}", e.TargetSite);
            Exception ie = e.InnerException;
            while (ie != null)
            {
                this.WriteLine(LogLevel.Debug, string.Empty);
                this.WriteLine(LogLevel.Debug, "<<INNER EXCEPTION INFO>>");
                this.WriteLine(LogLevel.Debug, "INNER MESSAGE: {0}", ie.Message);
                this.WriteLine(LogLevel.Debug, "INNER SOURCE : {0}", ie.Source);
                this.WriteLine(LogLevel.Debug, "INNER STACK  : {0}", ie.StackTrace);
                this.WriteLine(LogLevel.Debug, "INNER TARGET : {0}", ie.TargetSite);
                ie = ie.InnerException;
            }

            this.WriteLine(LogLevel.Error, "**************************************");
        }

        /// <summary>
        ///     The write line.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        /// <param name="line">
        ///     The line.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        public void WriteLine(LogLevel level, string line, params object[] args)
        {
            this.SetConsoleColor(level);
            if (args != null && args.Length > 0)
            {
                Console.WriteLine(line, args);
            }
            else
            {
                Console.WriteLine(line);
            }

            Console.ResetColor();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The set console color.
        /// </summary>
        /// <param name="level">
        ///     The level.
        /// </param>
        private void SetConsoleColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case LogLevel.Msg:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        #endregion
    }


}