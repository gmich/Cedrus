namespace Gmich.Cedrus
{

    public class Identity 
    {
        public Identity(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public override bool Equals(object obj)
        {
            var item = obj as Identity;

            if (item == null)
            {
                return false;
            }

            return Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
