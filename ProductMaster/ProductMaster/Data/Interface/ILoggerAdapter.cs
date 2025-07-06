using System;

namespace ProductMaster.Data
{

    /// <summary>
    /// Logger adapter interface
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="informationMessage"></param>
        void LogInformation(string informationMessage);

        /// <summary>
        /// Log Information with body
        /// </summary>
        /// <param name="informationMessage"></param>
        /// <param name="body"></param>
        void LogInformation(string informationMessage, string body);

        /// <summary>
        /// Log event without body
        /// </summary>
        /// <param name="informationMessage"></param>
        void LogEvent(string informationMessage);

        /// <summary>
        /// Log event with a body / parameters
        /// </summary>
        /// <param name="informationMessage"></param>
        /// <param name="body"></param>
        void LogEvent(string informationMessage, string body);

        /// <summary>
        /// Log error without body
        /// </summary>
        /// <param name="exception"></param>
        void LogError(Exception exception);

        /// <summary>
        /// Log Error with a body / parameters
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="body"></param>
        void LogError(Exception exception, string body);

        /// <summary>
        /// Log error with exception body and apiUrl
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="body"></param>
        /// <param name="apiUrl"></param>
        void LogError(Exception exception, string body, string apiUrl);
    }
}
