/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */
// ReSharper disable InconsistentNaming
namespace RubikCubeSolver.Kociemba.TwoPhase
{    
    /// <summary>
    /// The names of the corner positions of the cube. Corner URF e.g., has an U(p), a R(ight) and a F(ront) facelet
    /// </summary>
    public enum Corner
    {
        URF, UFL, ULB, UBR, DFR, DLF, DBL, DRB
    }

}
