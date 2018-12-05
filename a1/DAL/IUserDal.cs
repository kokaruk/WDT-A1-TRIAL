using Wdt.Model;

namespace Wdt.DAL
{
    public interface IUserDal
    {
        User GetUser(string userName, string password);
    }
}