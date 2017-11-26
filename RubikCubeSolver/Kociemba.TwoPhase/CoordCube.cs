/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */

using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    /// <summary>
    /// Representation of the cube on the coordinate level 
    /// </summary>
    public class CoordCube
    {
        // All coordinates are 0 for a solved cube except for UBtoDF, which is 114
        public short Twist;
        public short Flip;
        public short Parity;
        public short FRtoBR;
        public short URFtoDLF;
        public short URtoUL;
        public short UBtoDF;
        public int URtoDF;

        /// <summary>
        /// Generate a CoordCube from a CubieCube 
        /// </summary>
        /// <param name="c"></param>
        public CoordCube(CubieCube c)
        {
            Twist = c.GetTwist();
            Flip = c.GetFlip();
            Parity = c.CornerParity();
            FRtoBR = c.GetFRtoBr();
            URFtoDLF = c.GetUrFtoDlf();
            URtoUL = c.GetURtoUl();
            UBtoDF = c.GetUBtoDf();
            URtoDF = c.GetURtoDF();// only needed in phase2
        }

        ///// <summary>
        ///// A move on the coordinate level
        ///// </summary>
        ///// <param name="m"></param>
        //private void Move(int m)
        //{
        //    Twist = MoveTables.TwistMove[Twist, m];
        //    Flip = MoveTables.FlipMove[Flip, m];
        //    Parity = MoveTables.ParityMove[Parity, m];
        //    FRtoBR = MoveTables.FRtoBR_Move[FRtoBR, m];
        //    URFtoDLF = MoveTables.URFtoDLF_Move[URFtoDLF, m];
        //    URtoUL = MoveTables.URtoUL_Move[URtoUL, m];
        //    UBtoDF = MoveTables.UBtoDF_Move[UBtoDF, m];
        //    if (URtoUL < 336 && UBtoDF < 336)// updated only if UR,UF,UL,UB,DR,DF
        //        // are not in UD-slice
        //        URtoDF = MoveTables.MergeURtoULandUBtoDF[URtoUL, UBtoDF];
        //}
    }

}
