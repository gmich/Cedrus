using System;

namespace Gmich.Cedrus
{
    public class GameTimeline : ITimeline
    {
        public TimeSpan DeltaTime { get; internal set; }
        public TimeSpan TotalTime { get; internal set; }
    }
}