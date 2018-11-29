namespace wdt.Model
{
    public class User
    {
        public string Name { get; private set; }
        public UserType Type { get; private set; }

        public User(string name, UserType type)
        {
            Name = name;
            Type = type;
        }
    }
}