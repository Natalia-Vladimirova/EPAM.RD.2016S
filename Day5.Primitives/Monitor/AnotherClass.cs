using System.Threading;

namespace Monitor
{
    // Use SpinLock to protect this structure.
    public class AnotherClass
    {
        private static object locker = new object();
        static SpinLock spinlock = new SpinLock();
        private int _value;

        public int Counter
        {
            get
            {
                bool lockTaken = false;
                try
                {
                    spinlock.Enter(ref lockTaken);
                    return _value;
                }
                finally
                {
                    if (lockTaken) spinlock.Exit(false);
                }
            }
            set
            {
                bool lockTaken = false;
                try
                {
                    spinlock.Enter(ref lockTaken);
                    _value = value;
                }
                finally
                {
                    if (lockTaken) spinlock.Exit(false);
                }
            }
        }

        public void Increase()
        {
            bool lockTaken = false;
            try
            {
                spinlock.Enter(ref lockTaken);
                _value++;
            }
            finally
            {
                if (lockTaken) spinlock.Exit(false);
            }
        }

        public void Decrease()
        {
            bool lockTaken = false;
            try
            {
                spinlock.Enter(ref lockTaken);
                _value--;
            }
            finally
            {
                if (lockTaken) spinlock.Exit(false);
            }
        }
    }
}
