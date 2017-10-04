using System;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Interface for Logging Service
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Trace method
        /// </summary>
        /// <param name="message">Trace message</param>
        void Trace(string message);

        /// <summary>
        /// Trace method with additional parameters
        /// </summary>
        /// <param name="message">Trace message</param>
        /// <param name="args">Additional parameters</param>
        void Trace(string message, params object[] args);

        /// <summary>
        /// Debug method
        /// </summary>
        /// <param name="message">Debug message</param>
        void Debug(string message);

        /// <summary>
        /// Debug method with additional parameters
        /// </summary>
        /// <param name="message">Debug message</param>
        /// <param name="args">Additional parameters</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Info method
        /// </summary>
        /// <param name="message">Info message</param>
        void Info(string message);

        /// <summary>
        /// Info method with additional parameters
        /// </summary>
        /// <param name="message">Info message</param>
        /// <param name="args">Additional parameters</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Warning  method
        /// </summary>
        /// <param name="message">Warning message</param>
        void Warning(string message);

        /// <summary>
        /// Warning method with additional parameters
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="args">Additional parameters</param>
        void Warning(string message, params object[] args);

        /// <summary>
        /// Error method
        /// </summary>
        /// <param name="message">Error message</param>
        void Error(string message);

        /// <summary>
        /// Error method with additional parameters
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="args">Additional parameters</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Error method with exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="message">Error message</param>
        /// <param name="isStackTraceIncluded">Boolean marker for including stack trace</param>
        void Error(Exception exception, string message = null, bool isStackTraceIncluded = true);


        /// <summary>
        /// Fatal method
        /// </summary>
        /// <param name="message">Fatal message</param>
        void Fatal(string message);

        /// <summary>
        /// Fatal method with additional parameters
        /// </summary>
        /// <param name="message">Fatal message</param>
        /// <param name="args">Additional parameters</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Fatal method with exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="message">Fatal message</param>
        /// <param name="isStackTraceIncluded">Boolean marker for including stack trace</param>
        void Fatal(Exception exception, string message = null, bool isStackTraceIncluded = true);
    }
}
