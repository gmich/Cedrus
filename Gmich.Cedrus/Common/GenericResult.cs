using Newtonsoft.Json;

namespace Gmich.Cedrus
{
    public class Result<TValue> : Result
    {
        private readonly TValue value;

        public virtual TValue Value
        {
            get
            {
                return value;
            }
        }

        public object OnSuccess { get; set; }

        public override string ToString()
        {
            return $"Success: { Success } , State: {State} , ValueType: {typeof(TValue)} , Value: {Serialize(Value)} , Message: {ErrorMessage ?? ""}";
        }

        private string Serialize(TValue value)
        {
            if (value == null) return "null";
            return JsonConvert.SerializeObject(value, Formatting.None,
            new JsonSerializerSettings()
            {
                ContractResolver = new PrimitiveOnlyResolver(),
                DefaultValueHandling = DefaultValueHandling.Populate,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }

        protected internal Result(TValue value, State status, string error)
                : base(status, error)
        {
            this.value = value;
        }

        public static implicit operator Result<TValue>(TValue someValue)
        {
            return new Result<TValue>(someValue, State.Ok, string.Empty).FailIfNull(() => $"Value in implicit convertion was null");
        }

    }

}
