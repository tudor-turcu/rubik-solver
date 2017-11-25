using System;
using RubikCubeSolver.Kociemba.TwoPhase;
using RubikCubeSolver.Kociemba.TwoPhase.Exceptions;
using Xunit;

namespace RubikCubeSolver.Tests
{
    public class RubikCubeValidationTests
    {
        private const int MaxTime = 20;
        private const int MaxDepth = 25;

        [Theory]
        [TestFileData("TestData\\ValidSolvedCube.txt")]
        public void PermutationString_Having_ValidSolvedCube_IsOk(string textPermutation)
        {
            string facelets = RemoveWhiteSpace(textPermutation);

            string result = Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true);

            Assert.DoesNotContain("Error", result, StringComparison.OrdinalIgnoreCase);
        }
        
        [Theory]
        [TestFileData("TestData\\InvalidCubeWrongColorNumbers.txt")]
        public void PermutationString_Having_WrongColourNumbers_IsNotOk(string textPermutation)
        {            
            string facelets = RemoveWhiteSpace(textPermutation);
            
            Assert.Throws<NotOneFaceletOfEachColorException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        [Theory]
        [TestFileData("TestData\\InvalidCubeDuplicateEdge.txt")]
        public void PermutationString_Having_DuplicateEdge_IsNotOk(string textPermutation)
        {            
            string facelets = RemoveWhiteSpace(textPermutation);

            Assert.Throws<DuplicateEdgesException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        [Theory]
        [TestFileData("TestData\\InvalidCubeFlipError.txt")]
        public void PermutationString_Having_FlippedEdges_IsNotOk(string textPermutation)
        {
            string facelets = RemoveWhiteSpace(textPermutation);

            Assert.Throws<FlipErrorException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        [Theory]
        [TestFileData("TestData\\InvalidCubeDuplicateCorners.txt")]
        public void PermutationString_Having_DuplicateCorners_IsNotOk(string textPermutation)
        {
            string facelets = RemoveWhiteSpace(textPermutation);

            Assert.Throws<DuplicateCornersException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        [Theory]
        [TestFileData("TestData\\InvalidCubeTwistedCorner.txt")]
        public void PermutationString_Having_TwistedCorners_IsNotOk(string textPermutation)
        {
            string facelets = RemoveWhiteSpace(textPermutation);

            Assert.Throws<TwistedCornerException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        [Theory]
        [TestFileData("TestData\\InvalidCubeParityError.txt")]
        public void PermutationString_Having_ParityError_IsNotOk(string textPermutation)
        {
            string facelets = RemoveWhiteSpace(textPermutation);

            Assert.Throws<ParityException>(() => Search.Solution(facelets, MaxDepth, MaxTime, useSeparator: true));
        }

        private static string RemoveWhiteSpace(string textPermutation)
        {
            return textPermutation.Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }
    }
}
