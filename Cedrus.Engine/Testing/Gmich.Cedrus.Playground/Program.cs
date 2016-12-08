using Gmich.Cedrus.IOC;
using System;

namespace Gmich.Cedrus.Playground
{
    public static class Program
    {

        [STAThread]
        public static void Main(IocBuilder builder)
        {
            using (var game = new CedrusGame(builder))
            {
                game.Run();
            }
        }
    }
}
