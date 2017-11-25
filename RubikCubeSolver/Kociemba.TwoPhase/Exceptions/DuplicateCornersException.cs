using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class DuplicateCornersException : InvalidRubikCubeException
    {
        public DuplicateCornersException()
            : this("Not all 8 corners exist exactly once!")
        {
        }

        public DuplicateCornersException(string message) : base(message)
        {
        }

        public DuplicateCornersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateCornersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
