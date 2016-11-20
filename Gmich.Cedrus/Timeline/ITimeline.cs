using System;

namespace Gmich.Cedrus
{
    public interface ITimeline
    {
        TimeSpan DeltaTime { get; }
        TimeSpan TotalTime { get; }
    }
}