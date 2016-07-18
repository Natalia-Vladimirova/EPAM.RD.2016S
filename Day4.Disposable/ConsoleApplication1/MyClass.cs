using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    // The code below contains a lot of issues. Please, fix all of them.
    // Use as a guidelines:
    // https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.110).aspx
    // https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.100%29.aspx?f=255&MSPPError=-2147217396
    // https://msdn.microsoft.com/en-us/library/fs2xkftw(v=vs.110).aspx
    public class MyClass : IDisposable
    {
        private IntPtr buffer;       // unmanaged resource
        private SafeHandle resource; // managed resource
        private bool disposed;

        public MyClass()
        {
            buffer = Helper.AllocateBuffer();
            resource = Helper.GetResource();
        }

        ~MyClass()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            Helper.DeallocateBuffer(buffer);
            if (disposing)
            {
                resource?.Dispose();
            }
            disposed = true;
        }

        public void DoSomething()
        {
            // NOTE: Manupulation with _buffer and _resource in this line.
            if (disposed)
            {
                throw new ObjectDisposedException(ToString());
            }
        }
    }
}
