using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class RubikCubeException : Exception
    {
        public RubikCubeException()
        {
        }

        public RubikCubeException(string message) : base(message)
        {
        }

        public RubikCubeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RubikCubeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
