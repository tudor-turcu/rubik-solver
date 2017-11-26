using RubikCubeSolver.Kociemba.TwoPhase;
using Xunit;

namespace RubikCubeSolver.Tests
{
    public class RubikCubeSolvingTests
    {        
        [Theory]
        [TestFileData("TestData\\ValidScrambledCube1.txt")]
        public void ForAValidScrambledCube1_FindsTheCorrectSolution(string textPermutation)
        {
            const int maxTime = 20;
            const int maxDepth = 25;

            string facelets = Tools.RemoveWhiteSpace(textPermutation);

            string result = Search.Solution(facelets, maxDepth, maxTime, useSeparator: true);

            Assert.Equal("R2 F' R2 L' D R' L F2 D F' B . D2 F2 D B2 D' L2 D' R2", result.Trim());
        }

        [Theory]
        [TestFileData("TestData\\ValidCubeRight90.txt")]
        public void ForValidCube_RightFaceRotated90Deg_FindsTheCorrectSolution(string textPermutation)
        {
            const int maxTime = 20;
            const int maxDepth = 25;

            string facelets = Tools.RemoveWhiteSpace(textPermutation);

            string result = Search.Solution(facelets, maxDepth, maxTime, useSeparator: true);

            Assert.Equal("R' .", result.Trim());
        }

        [Theory]
        [TestFileData("TestData\\ValidScrambledCube2.txt")]
        public void ForAValidScrambledCube2_FindsTheCorrectSolution(string textPermutation)
        {
            const int maxTime = 20;
            const int maxDepth = 25;

            string facelets = Tools.RemoveWhiteSpace(textPermutation);

            string result = Search.Solution(facelets, maxDepth, maxTime, useSeparator: true);

            Assert.Equal("F2 L D R B' R' U' R' F' R' B . D' F2 R2 U F2 D' L2 D2 F2 R2", result.Trim());
        }

    }
}
