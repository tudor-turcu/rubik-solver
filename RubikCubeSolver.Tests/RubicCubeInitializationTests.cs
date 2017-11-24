using System;
using System.Linq;
using Xunit;

namespace RubikCubeSolver.Tests
{
    public class RubicCubeInitializationTests
    {
        [Theory]
        [TestFileData("TestData\\ValidCube1.txt")]
        public void PermutationString_Having_54_NonWhiteSpaceChars_IsOk(string textPermutation)
        {
            RubikCube cube = new RubikCube(textPermutation);
        }
    }
}
