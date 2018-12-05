using System;

namespace Wdt.Model
{
    public static class UserFactory
    {
        /// <summary>
        /// factory method return user based on input from user types
        /// </summary>
        /// <param name="selection">selection from user types</param>
        /// <param name="userName">username og user</param>
        /// <returns>instanse of a user</returns>
        /// <exception cref="IndexOutOfRangeException">is thrown when attempting to create non existent user type</exception>
        public static User MakeUserFromInt(int selection, string userName = "testName")
        {
            if (selection < 0 || selection >= Enum.GetNames(typeof(UserType)).Length)
                throw new IndexOutOfRangeException("Unexpected user type");
            var userType = (UserType) selection;
            var userTypeClassName = $"{typeof(User).Namespace}.{userType.ToString()}";
            var userTypeClassType = Type.GetType(userTypeClassName, true);
            var userInstance = Activator.CreateInstance(userTypeClassType, userName);
            return (User)userInstance;
        }

        public static User MakeUser(string userName, string userType)
        {
            var userTypeClassName = $"{typeof(User).Namespace}.{userType}";
            var userTypeClassType = Type.GetType(userTypeClassName, true);
            var userInstance = Activator.CreateInstance(userTypeClassType, userName);
            return (User)userInstance;
        }
        
    }
}