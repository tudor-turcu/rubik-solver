using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class InvalidRubikCubeException : Exception
    {
        public InvalidRubikCubeException()
        {
        }

        public InvalidRubikCubeException(string message) : base(message)
        {
        }

        public InvalidRubikCubeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidRubikCubeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
