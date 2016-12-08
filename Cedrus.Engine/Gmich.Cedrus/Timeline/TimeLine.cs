using System;

namespace Gmich.Cedrus
{
    public class Timeline : ITimeline
    {
        private readonly Func<double> timeProvider;
        private readonly Func<double> total;

        public Timeline(Func<double> timeProvider, Func<double> total)
        {
            this.timeProvider = timeProvider;
            this.total = total;
        }

        public TimeSpan DeltaTime => TimeSpan.FromSeconds(timeProvider());

        public TimeSpan TotalTime => TimeSpan.FromSeconds(total());
    }


}

