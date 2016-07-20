namespace Monitor
{
    using System.Threading;

    // Use Monitor (not lock) to protect this structure.
    public class MyClass
    {
        private readonly object locker = new object();
        private int _value;

        public int Counter
        {
            get
            {
                Monitor.Enter(locker);
                try
                {
                    return _value;
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
            set
            {
                Monitor.Enter(locker);
                try
                {
                    _value = value;
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
        }

        public void Increase()
        {
            Monitor.Enter(locker);
            try
            {
                _value++;
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }

        public void Decrease()
        {
            Monitor.Enter(locker);
            try
            {
                _value--;
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }
    }
}
