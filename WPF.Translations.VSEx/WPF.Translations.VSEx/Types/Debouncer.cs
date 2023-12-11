using System.Timers;

namespace WPF.Translations.VSEx.Types
{
    /// <summary>Ensures a method is only fired one time in a given time span (interval).</summary>
    public class Debouncer : IDisposable
    {
        #region Fields

        private Action action;
        private Timer timer;
        private bool disposedValue;

        #endregion

        #region Constructors

        public Debouncer(double interval, Action action)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));

            this.action = action;

            timer = new Timer(interval);
            timer.Elapsed += Timer_Elapsed;
        }

        #endregion

        #region Methods

        public void Debounce()
        {
            // if the timer is running, stop it and start it again
            // this makes the interval reset
            if (timer.Enabled) timer.Stop();

            timer.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Translations.VSEx.Types.Debouncer");

            if (!disposedValue)
            {
                if (disposing)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Translations.VSEx.Types.Debouncer");

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            action();
        }

        #endregion
    }
}
