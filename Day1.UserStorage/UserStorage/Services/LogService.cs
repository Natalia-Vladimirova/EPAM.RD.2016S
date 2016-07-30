using System.Diagnostics;
using System.Threading;
using UserStorage.Interfaces.Services;

namespace UserStorage.Services
{
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
