using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class DuplicateEdgesException : InvalidRubikCubeException
    {
        public DuplicateEdgesException()
            : this("Not all 12 edges exist exactly once!")
        {
        }

        public DuplicateEdgesException(string message) : base(message)
        {
        }

        public DuplicateEdgesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateEdgesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
