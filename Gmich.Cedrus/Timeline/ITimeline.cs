using System;

namespace Gmich.Cedrus.Timeline
{
    public interface ITimeline
    {
        /// <summary>
        /// The time delta in seconds
        /// </summary>
        TimeSpan DeltaTime { get; }

    }
}
