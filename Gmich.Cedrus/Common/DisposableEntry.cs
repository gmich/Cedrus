using System;
using System.Collections.Generic;

namespace Gmich.Cedrus
{
    public sealed class DisposableEntry<TEntry> : IDisposable
    {
        private readonly IList<TEntry> registered;
        private readonly TEntry current;

        internal DisposableEntry(IList<TEntry> registered, TEntry current)
        {
            this.registered = registered;
            this.current = current;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool isDisposed = false;
        private void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                if (registered.Contains(current))
                    registered.Remove(current);
                isDisposed = true;
            }
        }
    }

    public static class Disposable 
    {
        public static DisposableEntry<TEntry> For<TEntry>(IList<TEntry> registered, TEntry current)
        {
            return new DisposableEntry<TEntry>(registered, current);
        }

        public static IDisposable ThatDoesNothing => new DoNothingDisposable();
    }

    public class DoNothingDisposable : IDisposable
    {
        public void Dispose()
        {
            return;
        }
    }
}
