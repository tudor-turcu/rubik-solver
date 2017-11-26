/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */
using System.Diagnostics.CodeAnalysis;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Edge
    {
        UR, UF, UL, UB, DR, DF, DL, DB, FR, FL, BL, BR
    }    
}
