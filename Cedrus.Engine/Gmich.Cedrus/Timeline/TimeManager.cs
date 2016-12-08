using System;
using System.Collections.Generic;

namespace Gmich.Cedrus
{
    public class TimeManager
    {
        public Func<Timeline> TimeFactory { get; set; }
    }

    public class Updater
    {
        private readonly IList<Action<Timeline>> updates = new List<Action<Timeline>>();
        private readonly TimeManager timeManager;

        public Updater(TimeManager timeManager)
        {
            this.timeManager = timeManager;
        }

        public IDisposable Subscribe(Action<Timeline> update)
        {
            updates.Add(update);
            return Disposable.ForList(updates, update);
        }
    }
}
