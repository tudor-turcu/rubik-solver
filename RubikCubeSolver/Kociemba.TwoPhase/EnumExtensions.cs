using System;
using System.Globalization;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    public static class EnumExtensions
    {
        public static int Ordinal<TE>(this TE e) where TE : IConvertible

        {
            return e.ToInt32(CultureInfo.InvariantCulture);
        }
    }
}
