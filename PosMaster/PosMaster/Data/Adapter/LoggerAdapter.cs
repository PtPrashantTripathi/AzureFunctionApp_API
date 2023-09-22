using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PosMaster.Data.Adapter
{
    /// <summary>
    /// Telemetry logging for all activities
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LoggerAdapter : ILoggerAdapter
    {
        #region Constants

        private const string LOG_INFO_TEXT = "PosMaster";

        #endregion

        #region Properties
        /// <summary>
        /// Telemetry client
        /// </summary>
        TelemetryClient _appInsights { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="telemetryClient"></param>
        public LoggerAdapter(TelemetryClient telemetryClient)
        {
            _appInsights = telemetryClient;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Logs error
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="body"></param>
        public void LogError(Exception exception, string body)
        {
            try
            {
                var properties = new Dictionary<string, string>
                                { {"Body", body}};
                _appInsights.TrackException(exception, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Log error without body
        /// </summary>
        /// <param name="exception"></param>
        public void LogError(Exception exception)
        {
            try
            {
                _appInsights.TrackException(exception, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Log error with exception body and apiUrl
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="body"></param>
        /// <param name="apiUrl"></param>
        public void LogError(Exception exception, string body, string apiUrl)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("External API Called Failed, Url : " + apiUrl);
                sb.AppendLine();
                sb.Append(body);

                var properties = new Dictionary<string, string>
                                { {"Body", sb.ToString()}};
                _appInsights.TrackException(exception, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Log information
        /// </summary>
        /// <param name="informationMessage"></param>
        public void LogInformation(string informationMessage)
        {
            try
            {
                _appInsights.TrackTrace(informationMessage);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        /// <summary>
        /// Log Information with body
        /// </summary>
        /// <param name="informationMessage"></param>
        /// <param name="body"></param>
        public void LogInformation(string informationMessage, string body)
        {
            try
            {
                var properties = new Dictionary<string, string>
                                { {"Body", body}};
                _appInsights.TrackTrace(informationMessage, properties);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        /// <summary>
        /// Log event with a body / parameters
        /// </summary>
        /// <param name="informationMessage"></param>
        /// <param name="body"></param>
        public void LogEvent(string informationMessage, string body)
        {
            try
            {
                var properties = new Dictionary<string, string>
                                { {"Body", body}};
                _appInsights.TrackEvent(informationMessage, properties);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Log event without body
        /// </summary>
        /// <param name="informationMessage"></param>
        public void LogEvent(string informationMessage)
        {
            try
            {

                _appInsights.TrackEvent(informationMessage);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        #endregion
    }
}
