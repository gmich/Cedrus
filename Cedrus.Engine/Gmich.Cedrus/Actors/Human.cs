using Gmich.Cedrus.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmich.Cedrus.Actors
{
    public class Human
    {

        public Human(Texture2D placeHolder)
        {

        }


        Vector2 Center { get; }
        Body Head { get; }
        Body Torso { get; }
        Body Feet { get; }
        Joint Upper { get; }
        Joint Lower { get; }
    }
}
