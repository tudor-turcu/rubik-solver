using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class ParityException : InvalidRubikCubeException
    {
        public ParityException()
            : this("Parity error: Two corners or two edges have to be exchanged!")
        {
        }

        public ParityException(string message) : base(message)
        {
        }

        public ParityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
