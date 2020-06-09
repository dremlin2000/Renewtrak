using Xunit;

namespace Puzzle2.Tests
{
    public class StringExtensionTest
    {
        [Theory]
        [InlineData("Cat and dog", "taC dna god")]
        [InlineData(">^..^< & ˁ˚ᴥ˚ˀ", "<^..^> & ˀ˚ᴥ˚ˁ")]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit,", "meroL muspi rolod tis ,tema rutetcesnoc gnicsipida ,tile")]
        [InlineData(" ", " ")]
        [InlineData("", "")]
        public void test(string sentence, string expectedSentence)
        {
            //Act
            var actual = sentence.ReverseWords();

            //Assert
            Assert.Equal(expectedSentence, actual);
        }
    }
}
