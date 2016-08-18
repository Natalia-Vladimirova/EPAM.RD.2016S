using System.Diagnostics;

namespace UserStorage.Interfaces.Services
{
    /// <summary>
    /// Represents service for logging.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Logs message using event type.
        /// </summary>
        /// <param name="traceEventType">
        /// Event type of the trace data.
        /// </param>
        /// <param name="message">
        /// The trace message to write.
        /// </param>
        void Log(TraceEventType traceEventType, string message);
    }
}
