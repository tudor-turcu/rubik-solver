using System;
using System.Runtime.Serialization;

namespace RubikCubeSolver.Kociemba.TwoPhase.Exceptions
{
    public class NotOneFaceletOfEachColorException : InvalidRubikCubeException
    {
        public NotOneFaceletOfEachColorException()
            : this("There are not exactly nine facelets of each color!")
        {
        }

        public NotOneFaceletOfEachColorException(string message) : base(message)
        {
        }

        public NotOneFaceletOfEachColorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotOneFaceletOfEachColorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
