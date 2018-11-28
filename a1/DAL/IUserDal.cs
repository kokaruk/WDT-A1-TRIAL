using wdt.Model;

namespace wdt.DAL
{
    public interface IUserDal
    {
        User GetUser(string username, string password);
    }
}