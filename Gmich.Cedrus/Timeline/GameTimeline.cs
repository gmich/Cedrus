using Microsoft.Xna.Framework;
using System;


namespace Gmich.Cedrus.Timeline
{
    internal class GameTimeline : ITimeline
    {
        internal GameTime GameTime { get; set; }

        public TimeSpan DeltaTime
        {
            get { return GameTime.ElapsedGameTime; }
        }


    }
}

