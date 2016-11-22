using System;

namespace Gmich.Cedrus.IOC
{
    public abstract class CleanedupEntity : IDisposable
    {
        private bool isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void OnDisposal();

        private void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                OnDisposal();
                isDisposed = true;
            }
        }
    }
}
