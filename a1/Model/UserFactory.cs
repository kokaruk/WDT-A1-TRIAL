using System;
using wdt.utils;

namespace wdt.Model
{
    public static class UserFactory
    {
        // factory method return user based on input
        public static User MakeUserFromInt(int selection, string userName = "testName")
        {
            if (selection < 0 || selection >= Enum.GetNames(typeof(UserType)).Length)
                throw new IndexOutOfRangeException("Unexpected user type");
            var userType = (UserType) selection;
            var userTypeClassName = $"wdt.Model.{userType.ToString()}";
            var userTypeClassType = Type.GetType(userTypeClassName, true);
            var userInstance = Activator.CreateInstance(userTypeClassType, userName);
            return (User)userInstance;
        }
    }
}