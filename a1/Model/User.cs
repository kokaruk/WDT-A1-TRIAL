namespace Wdt.Model
{
    public class User
    {
        private string Name { get; }
        public UserType UserType { get; set; }
        protected User(string name)
        {
            Name = name;
        }
    }

    public class Customer : User
    {
        public Customer(string name) : base(name)
        {
            UserType = UserType.Customer;
        }
    }

    public class Franchisee : User
    {
        public Franchises Location { get; set; }
        public Franchisee(string name) : base(name)
        {
            UserType = UserType.Franchisee;
        }
    }

    public class Owner : User
    {
        public Owner(string name) : base(name)
        {
            UserType = UserType.Owner;
        }
    }
}