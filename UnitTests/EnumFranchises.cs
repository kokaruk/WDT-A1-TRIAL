using Wdt.Model;
using Xunit;
using Wdt.Utils;

namespace UnitTests
{
    public class EnumFranchises
    {
        [Theory]
        [InlineData(Franchises.CBD, "Melbourne CBD")]
        [InlineData(Franchises.North, "North Melbourne")]
        public void Franchises_NamesReturnCorrectStringAttribute(Franchises franchises, string attribute)
        {
            Assert.Equal(franchises.GetStringValue(), attribute);
        }

        [Theory]
        [InlineData(0, Franchises.CBD)]
        public void Franchises_EnumsReturnCorrectInt(int value, Franchises franchise)
        {
            Assert.Equal(value, (int)franchise);
        }
        
    }
}