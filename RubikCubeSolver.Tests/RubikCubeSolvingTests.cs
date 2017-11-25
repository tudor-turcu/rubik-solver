using RubikCubeSolver.Kociemba.TwoPhase;
using Xunit;

namespace RubikCubeSolver.Tests
{
    public class RubikCubeSolvingTests
    {        
        [Theory]
        [TestFileData("TestData\\ValidScrambledCube1.txt")]
        public void ForAValidScrambledCube_FindTheCorrectSolution(string textPermutation)
        {
            const int maxTime = 20;
            const int maxDepth = 25;

            string facelets = Tools.RemoveWhiteSpace(textPermutation);

            string result = Search.Solution(facelets, maxDepth, maxTime, useSeparator: true);

            Assert.Equal("R2 F' R2 L' D R' L F2 D F' B . D2 F2 D B2 D' L2 D' R2", result.Trim());
        }
        
    }
}
