using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class FlipErrorException : InvalidRubikCubeException
    {
        public FlipErrorException()
            : this("Flip error: One edge has to be flipped!")
        {
        }

        public FlipErrorException(string message) : base(message)
        {
        }

        public FlipErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FlipErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
