using System.Diagnostics;
using System.Threading;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
    /// <summary>
    /// Service for logging.
    /// </summary>
    public sealed class LogService : ILogService
    {
        private readonly BooleanSwitch boolSwitch;
        private readonly TraceSource source;
        private readonly Mutex mutex = new Mutex(false, "LogMutex");

        public LogService()
        {
            boolSwitch = new BooleanSwitch("Switch", string.Empty);
            source = new TraceSource("Source");
        }

        /// <summary>
        /// Logs according to App.config.
        /// </summary>
        /// <param name="traceEventType">
        /// Event type of the trace data.
        /// </param>
        /// <param name="message">
        /// The trace message to write.
        /// </param>
        public void Log(TraceEventType traceEventType, string message)
        {
            mutex.WaitOne();

            if (boolSwitch.Enabled)
            {
                source.TraceEvent(traceEventType, 0, message);
            }

            mutex.ReleaseMutex();
        }
    }
}
