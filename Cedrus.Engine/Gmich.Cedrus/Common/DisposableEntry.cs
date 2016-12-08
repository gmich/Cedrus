using System;
using System.Collections.Generic;

namespace Gmich.Cedrus
{
    public sealed class DisposableEntry : IDisposable
    {
        private readonly Action disposal;
        private bool isDisposed = false;

        internal DisposableEntry(Action disposal)
        {
            this.disposal = disposal;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                disposal();
                isDisposed = true;
            }
        }
    }

    public static class Disposable
    {
        public static IDisposable For(Action disposal) => new DisposableEntry(disposal);

        public static IDisposable ForList<TEntry>(IList<TEntry> list, TEntry item)
         => new DisposableEntry(() =>
         {
             if (list.Contains(item))
             {
                 list.Remove(item);
             }
         });

        public static IDisposable ForDictionary<TId, TValue>(IDictionary<TId, TValue> dictionary, TId id)
        => new DisposableEntry(() =>
        {
            if (dictionary.ContainsKey(id))
            {
                dictionary.Remove(id);
            }
        });

        public static IDisposable ThatDoesNothing => new DisposableEntry(() => { });
    }

}
