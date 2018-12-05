using System;
using Wdt.Model;
using Xunit;


namespace UnitTests
{
    public class UserTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void UserFactory_TestCorrectTypes(int value)
        {
            var user = UserFactory.MakeUserFromInt(value);
            Assert.Equal(user.UserType, (UserType)value );
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void UserFactory_ThrowsErrors(int value)
        {
            Assert.Throws<IndexOutOfRangeException>(() => UserFactory.MakeUserFromInt(value));
        }    
        
        [Theory]
        [InlineData(0, typeof(Owner))]
        [InlineData(1, typeof(Franchisee))]
        [InlineData(2, typeof(Customer))]
        public void User_AcceptableInstancesTest(int value, Type userClassType)
        {
            var instance = (User)Activator.CreateInstance(userClassType, "test name");
            Assert.Equal(instance.UserType, (UserType)value);
        }
        
    }
}