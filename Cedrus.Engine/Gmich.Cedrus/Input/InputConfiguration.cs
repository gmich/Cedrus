using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.Input
{
    public class InputConfiguration
    {
        private readonly List<Tuple<Predicate<InputHandler>, Action>> config = new List<Tuple<Predicate<InputHandler>, Action>>();

        public IDisposable Add(Predicate<InputHandler> input, Action action)
        {
            var tuple = Tuple.Create(input, action);
            config.Add(tuple);
            return Disposable.ForList(config, tuple);
        }        

        public void Check(InputHandler inputManager)
        {
            foreach(var entry in config)
            {
                if(entry.Item1(inputManager))
                {
                    entry.Item2.Invoke();
                }
            }
        }

    }
}
