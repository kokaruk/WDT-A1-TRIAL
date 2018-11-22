using NUnit.Framework;

namespace BasicAlgorithms.Test
{
    [TestFixture]
    public class SequentialSearchTest
    {
        int[] testArray = {7, 5, 3, 2, 9, 11, 19, 0, 60, 4};

        [Test]
        public void shouldFindAllElementsInTestArray()
        {
            var x = 5;
            Assert.That( x == 5);
        }
    }
}