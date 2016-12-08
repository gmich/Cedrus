using System.Collections.Generic;

namespace Gmich.Cedrus.Primitives.Providers
{
    public interface IShapeProvider
    {
        IList<IShape> Load();
        bool Save(IList<IShape> shapes);
    }
}