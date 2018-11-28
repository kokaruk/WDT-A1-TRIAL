using wdt.Model;
using wdt.utils;

namespace wdt.DAL
{
    public class DalFactory
    {
        public static IUserDal UserDal { get; set; }

        static DalFactory()
        {
            UserDal = new FakeUserDal();
        }

        private class FakeUserDal : IUserDal
        {
            public User GetUser(string username, string password)
            {
                return new User(username, UserType.Owner);
            }
        }
        
    }
}