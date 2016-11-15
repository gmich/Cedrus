namespace Gmich.Cedrus.Rendering
{
    /// <summary>
    /// TODO: override equality, hashcode and string casting
    /// </summary>
    public class Identity
    {
        public Identity(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
