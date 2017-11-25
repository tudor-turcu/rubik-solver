using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class TwistedCornerException : InvalidRubikCubeException
    {
        public TwistedCornerException()
            : this("Twist error: One corner has to be twisted!")
        {
        }

        public TwistedCornerException(string message) : base(message)
        {
        }

        public TwistedCornerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TwistedCornerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
