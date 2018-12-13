namespace Wdt.Model
{
    public class OwnerInventory
    {
        public int ProductId { get; }
        public string Name { get; }
        public int StockLevel { get; }

        public OwnerInventory(int productId, string name, int stockLevel)
        {
            ProductId = productId;
            Name = name;
            StockLevel = stockLevel;
        }
    }
}