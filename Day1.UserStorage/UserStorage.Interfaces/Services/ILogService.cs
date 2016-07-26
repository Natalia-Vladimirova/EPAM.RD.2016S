using System.Diagnostics;

namespace UserStorage.Interfaces.Services
{
    public interface ILogService
    {
        void Log(TraceEventType traceEventType, string message);
    }
}
