using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus
{
    internal static class ReflectionHelper
    {
        public static Result<IEnumerable<Tuple<Type, Attribute>>> GetTypessWithAttribute<Attribute>()
        => Result.Try(() =>
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attributes = t.GetCustomAttributes(typeof(Attribute), true)
            where attributes != null && attributes.Length == 1
            select Tuple.Create(t, (Attribute)attributes.FirstOrDefault()),
        () => $"Failed to get all types with attribute {typeof(Attribute)}.");


    }
}