namespace wdt.Model
{
    public class User
    {
        public string Name { get; }
        public UserType UserType { get; set; }
        protected User(string name)
        {
            Name = name;
        }
    }

    class Customer : User
    {
        public Customer(string name) : base(name)
        {
            UserType = UserType.Customer;
        }
    }

    class Franchisee : User
    {
        public Franchises Location { get; set; }
        public Franchisee(string name) : base(name)
        {
            UserType = UserType.Franchisee;
        }
    }

    class Owner : User
    {
        public Owner(string name) : base(name)
        {
            UserType = UserType.Owner;
        }
    }
}