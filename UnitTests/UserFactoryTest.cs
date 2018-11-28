using System;
using Xunit;
using wdt.Model;
using wdt.utils;

namespace UnitTests
{
    public class UserFactoryTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void UserFactory_TestCorrectTypes(int value)
        {
            var user = UserFactory.MakeUserFromInt(value);
            Assert.Equal(user.Type, (UserType)value );
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void userFactory_ThrowsErrors(int value)
        {
            Assert.Throws<IndexOutOfRangeException>(() => UserFactory.MakeUserFromInt(value));
        }    
        
    }
}