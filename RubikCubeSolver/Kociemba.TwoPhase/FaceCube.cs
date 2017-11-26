/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */
using System;
using static RubikCubeSolver.Kociemba.TwoPhase.Facelet;
using static RubikCubeSolver.Kociemba.TwoPhase.Color;
using static RubikCubeSolver.Kociemba.TwoPhase.Corner;
using static RubikCubeSolver.Kociemba.TwoPhase.Edge;

namespace RubikCubeSolver.Kociemba.TwoPhase
{    
    /// <summary>
    /// Cube on the facelet level
    /// </summary>
    public class FaceCube
    {
        public Color[] F = new Color[54];

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Map the corner positions to facelet positions. cornerFacelet[URF.ordinal()][0] e.g. gives the position of the
        // facelet in the URF corner position, which defines the orientation.<br>
        // cornerFacelet[URF.ordinal()][1] and cornerFacelet[URF.ordinal()][2] give the position of the other two facelets
        // of the URF corner (clockwise).
        public static Facelet[][] CornerFacelet = {
            new[] { U9, R1, F3 }, new[] { U7, F1, L3 }, new[] { U1, L1, B3 }, new[] { U3, B1, R3 },
            new[] { D3, F9, R7 }, new[] { D1, L9, F7 }, new[] { D7, B9, L7 }, new[] { D9, R9, B7 } };

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Map the edge positions to facelet positions. edgeFacelet[UR.ordinal()][0] e.g. gives the position of the facelet in
        // the UR edge position, which defines the orientation.<br>
        // edgeFacelet[UR.ordinal()][1] gives the position of the other facelet
        public static Facelet[][] EdgeFacelet = { new[] { U6, R2 }, new[] { U8, F2 }, new[] { U4, L2 }, new[] { U2, B2 }, new[] { D6, R8 }, new[] { D2, F8 },
            new[] { D4, L8 }, new[] { D8, B8 }, new[] { F6, R4 }, new[] { F4, L6 }, new[] { B6, L4 }, new[] { B4, R6 } };

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Map the corner positions to facelet colors.
        public static Color[][] CornerColor = { new[] { U, R, Color.F }, new[] { U, Color.F, L }, new[] { U, L, B }, new[] { U, B, R }, new[] { D, Color.F, R }, new[] { D, L, Color.F },
            new[] { D, B, L }, new[] { D, R, B } };

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Map the edge positions to facelet colors.
        public static Color[][] EdgeColor = { new[] { U, R }, new[] { U, Color.F }, new[] { U, L }, new[] { U, B }, new[] { D, R }, new[] { D, Color.F }, new[] { D, L }, new[] { D, B },
            new[] { Color.F, R }, new[] { Color.F, L }, new[] { B, L }, new[] { B, R } };

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public FaceCube()
        {
            string s = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";
            for (int i = 0; i < 54; i++)
                F[i] = Enum.Parse<Color>(s.Substring(i, 1));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Construct a facelet cube from a string
        public FaceCube(string cubeString)
        {
            for (int i = 0; i < cubeString.Length; i++)
                F[i] = Enum.Parse<Color>(cubeString.Substring(i, 1));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Gives string representation of a facelet cube
        public override String ToString()
        {
            String s = "";
            for (int i = 0; i < 54; i++)
                s += F[i].ToString();
            return s;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Gives CubieCube representation of a faceletcube
        public CubieCube ToCubieCube()
        {
            CubieCube ccRet = new CubieCube();
            for (int i = 0; i < 8; i++)
                ccRet.CP[i] = URF;// invalidate corners
            for (int i = 0; i < 12; i++)
                ccRet.EP[i] = UR;// and edges

            foreach (Corner i in Enum.GetValues(typeof(Corner)))
            {
                // get the colors of the cubie at corner i, starting with U/D
                byte ori;
                for (ori = 0; ori < 3; ori++)
                    if (F[(int)CornerFacelet[(int)i][ori]] == U || F[(int)CornerFacelet[(int)i][ori]] == D)
                        break;

                Color col1 = F[(int)CornerFacelet[(int)i][(ori + 1) % 3]];
                Color col2 = F[(int)CornerFacelet[(int)i][(ori + 2) % 3]];

                foreach (Corner j in Enum.GetValues(typeof(Corner)))
                {
                    if (col1 == CornerColor[(int)j][1] && col2 == CornerColor[(int)j][2])
                    {
                        // in cornerposition i we have cornercubie j
                        ccRet.CP[(int)i] = j;
                        ccRet.CO[(int)i] = (sbyte)(ori % 3);
                        break;
                    }
                }
            }

            foreach (Edge i in Enum.GetValues(typeof(Edge)))
                foreach (Edge j in Enum.GetValues(typeof(Edge)))
                {
                    if (F[(int)EdgeFacelet[(int)i][0]] == EdgeColor[(int)j][0]
                        && F[(int)EdgeFacelet[(int)i][1]] == EdgeColor[(int)j][1])
                    {
                        ccRet.EP[(int)i] = j;
                        ccRet.EO[(int)i] = 0;
                        break;
                    }

                    if (F[(int)EdgeFacelet[(int)i][0]] == EdgeColor[(int)j][1]
                        && F[(int)EdgeFacelet[(int)i][1]] == EdgeColor[(int)j][0])
                    {
                        ccRet.EP[(int)i] = j;
                        ccRet.EO[(int)i] = 1;
                        break;
                    }
                }

            return ccRet;
        }
    }

}
