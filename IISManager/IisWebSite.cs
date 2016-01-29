namespace IISManager
{
    public class IisWebSite
    {
        public IisWebSite(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; set; }
        public string Name { get; set; }
    }
}
