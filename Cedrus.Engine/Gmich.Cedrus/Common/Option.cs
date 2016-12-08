using System;

namespace Gmich.Cedrus
{
    public struct Option<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        internal Option(T value)
        {
            Value = value;
            HasValue = value != null;
        }

        public static explicit operator T(Option<T> optional) => optional.Value;

        public static implicit operator Option<T>(T value) => new Option<T>(value);

        public override bool Equals(object obj)
        => (obj is Option<T>) ? Equals((Option<T>)obj) : false;

        public override int GetHashCode()
        => (HasValue) ? Value.GetHashCode() : base.GetHashCode();

        public bool Equals(Option<T> other)
        => (HasValue && other.HasValue)
            ? object.Equals(Value, other.Value) : (HasValue == other.HasValue);

        public T Match(Func<T, T> some, Func<T> none)
         => HasValue ? some(Value) : none();

        public TNext Match<TNext>(Func<T, TNext> some, Func<TNext> none)
         => HasValue ? some(Value) : none();
    }

    public static class Option
    {
        public static Option<T> AsOption<T>(this T value) => new Option<T>(value);
        public static Option<T> Some<T>(T value) => new Option<T>(value);
        public static Option<T> None<T>() => new Option<T>(default(T));
    }
}
