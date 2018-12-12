namespace Wdt.Model
{
    public class StockRequest
    {
        public int Id { get; }
        public string Store { get; }
        public string Product { get; }
        public int Quantity { get; }
        public int StockLevel { get; }
        public bool StockAvail { get;}

        public StockRequest(int id, string store, string product, int quantity, int stockLevel, bool stockAvail)
        {
            Id = id;
            Store = store;
            Product = product;
            Quantity = quantity;
            StockLevel = stockLevel;
            StockAvail = stockAvail;
        }
    }
}