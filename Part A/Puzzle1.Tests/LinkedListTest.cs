using System;
using System.Linq;
using Xunit;

namespace Puzzle1.Tests
{
    public class LinkedListTest
    {
        [Theory]
        [InlineData(4, 7, false)]
        [InlineData(0, 11, false)]
        [InlineData(9, 2, false)]
        [InlineData(0, 2, true)]
        [InlineData(9, 11, true)]
        //Bear in mind that index starts from 0
        public void Given_SequenceOfInteger_When_GetValueByIndex_Then_ShouldReturnTheCorrectValue(
            uint index, int expectedValue, bool startFromHead)
        {
            //Arrange
            var linkedList = new LinkedList<int>();
            Enumerable.Range(2, 10).ToList().ForEach(num => linkedList.Add(num));

            //Act
            var actual = linkedList.GetByIndex(index, startFromHead);

            //Assert
            Assert.Equal(expectedValue, actual.Value);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(10, false)]
        //Bear in mind that index starts from 0
        public void Given_SequenceOfInteger_When_GetValueByInvalidIndex_Then_ArgumentOutOfRangeExceptionIsRaised(
            uint index, bool startFromHead)
        {
            //Arrange
            var linkedList = new LinkedList<int>();
            Enumerable.Range(2, 10).ToList().ForEach(num => linkedList.Add(num));

            //Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => linkedList.GetByIndex(index, startFromHead));
        }
    }
}
