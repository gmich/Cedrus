using Gmich.Cedrus.IOC;
using System;

namespace Gmich.Cedrus.TandrixClone
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var builder = new IocBuilder();
            using (var game = new CedrusGame(builder))
            {
                game.Run();
            }
        }
    }
}
