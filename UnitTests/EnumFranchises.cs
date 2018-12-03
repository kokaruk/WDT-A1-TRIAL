using System;
using Xunit;
using wdt.Model;
using wdt.utils;

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
    }
}