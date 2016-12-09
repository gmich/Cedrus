using Gmich.Cedrus.IOC;
using System;
using System.Reflection;

namespace Gmich.Cedrus.TandrixClone
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var builder = new IocBuilder();
            builder.RegisterModules(Assembly.GetExecutingAssembly());

            using (var game = new CedrusGame(builder))
            {
                game.Run();
            }
        }
    }
}
