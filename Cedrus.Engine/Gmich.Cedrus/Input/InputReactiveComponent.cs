using System;

namespace Gmich.Cedrus.Input
{
    public class InputReactiveComponent
    {
        Action<InputManager> Update { get; }
    }
}
