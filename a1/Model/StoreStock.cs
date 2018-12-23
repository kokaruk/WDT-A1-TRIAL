namespace Wdt.Model
{
    public class StoreStock
    {
        public int Id { get; }
        public string Name { get; }
        public int Level { get; set; }

        public StoreStock(int id, string name, int level)
        {
            Id = id;
            Name = name;
            Level = level;
        }
    }
}