/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm
 */
using System;
using RubikCubeSolver.Kociemba.TwoPhase.Exceptions;
using static RubikCubeSolver.Kociemba.TwoPhase.Corner;
using static RubikCubeSolver.Kociemba.TwoPhase.Edge;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    
    /// <summary>
    /// Cube on the cubie level
    /// </summary>
    public class CubieCube
    {

        // initialize to Id-Cube

        // corner permutation
        public Corner[] CP = { URF, UFL, ULB, UBR, DFR, DLF, DBL, DRB };

        // corner orientation
        public sbyte[] CO = { 0, 0, 0, 0, 0, 0, 0, 0 };

        // edge permutation
        public Edge[] EP = { UR, UF, UL, UB, DR, DF, DL, DB, FR, FL, BL, BR };

        // edge orientation
        public sbyte[] EO = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // ************************************** Moves on the cubie level ***************************************************

        private static readonly Corner[] cpU = { UBR, URF, UFL, ULB, DFR, DLF, DBL, DRB };
        private static readonly sbyte[] coU = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly Edge[] epU = { UB, UR, UF, UL, DR, DF, DL, DB, FR, FL, BL, BR };
        private static readonly sbyte[] eoU = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private static readonly Corner[] cpR = { DFR, UFL, ULB, URF, DRB, DLF, DBL, UBR };
        private static readonly sbyte[] coR = { 2, 0, 0, 1, 1, 0, 0, 2 };
        private static readonly Edge[] epR = { FR, UF, UL, UB, BR, DF, DL, DB, DR, FL, BL, UR };
        private static readonly sbyte[] eoR = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private static readonly Corner[] cpF = { UFL, DLF, ULB, UBR, URF, DFR, DBL, DRB };
        private static readonly sbyte[] coF = { 1, 2, 0, 0, 2, 1, 0, 0 };
        private static readonly Edge[] epF = { UR, FL, UL, UB, DR, FR, DL, DB, UF, DF, BL, BR };
        private static readonly sbyte[] eoF = { 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0 };

        private static readonly Corner[] cpD = { URF, UFL, ULB, UBR, DLF, DBL, DRB, DFR };
        private static readonly sbyte[] coD = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly Edge[] epD = { UR, UF, UL, UB, DF, DL, DB, DR, FR, FL, BL, BR };
        private static readonly sbyte[] eoD = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private static readonly Corner[] cpL = { URF, ULB, DBL, UBR, DFR, UFL, DLF, DRB };
        private static readonly sbyte[] coL = { 0, 1, 2, 0, 0, 2, 1, 0 };
        private static readonly Edge[] epL = { UR, UF, BL, UB, DR, DF, FL, DB, FR, UL, DL, BR };
        private static readonly sbyte[] eoL = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private static readonly Corner[] cpB = { URF, UFL, UBR, DRB, DFR, DLF, ULB, DBL };
        private static readonly sbyte[] coB = { 0, 0, 1, 2, 0, 0, 2, 1 };
        private static readonly Edge[] epB = { UR, UF, UL, BR, DR, DF, DL, BL, FR, FL, UB, DB };
        private static readonly sbyte[] eoB = { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1 };

        // this CubieCube array represents the 6 basic cube moves
        public static CubieCube[] MoveCube = new CubieCube[6];

        static CubieCube() {
            MoveCube[0] = new CubieCube
            {
                CP = cpU,
                CO = coU,
                EP = epU,
                EO = eoU
            };

            MoveCube[1] = new CubieCube
            {
                CP = cpR,
                CO = coR,
                EP = epR,
                EO = eoR
            };

            MoveCube[2] = new CubieCube
            {
                CP = cpF,
                CO = coF,
                EP = epF,
                EO = eoF
            };

            MoveCube[3] = new CubieCube
            {
                CP = cpD,
                CO = coD,
                EP = epD,
                EO = eoD
            };

            MoveCube[4] = new CubieCube
            {
                CP = cpL,
                CO = coL,
                EP = epL,
                EO = eoL
            };

            MoveCube[5] = new CubieCube
            {
                CP = cpB,
                CO = coB,
                EP = epB,
                EO = eoB
            };

        }


        public CubieCube()
        {
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public CubieCube(Corner[] cp, sbyte[] co, Edge[] ep, sbyte[] eo) : this()
        {
            for (int i = 0; i < 8; i++)
            {
                CP[i] = cp[i];
                CO[i] = co[i];
            }

            for (int i = 0; i < 12; i++)
            {
                EP[i] = ep[i];
                EO[i] = eo[i];
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // n choose k
        private static int Cnk(int n, int k)
        {
            int i, j, s;
            if (n < k)
                return 0;
            if (k > n / 2)
                k = n - k;
            for (s = 1, i = n, j = 1; i != n - k; i--, j++)
            {
                s *= i;
                s /= j;
            }
            return s;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private static void RotateLeft(Corner[] arr, int l, int r)
            // Left rotation of all array elements between l and r
        {
            Corner temp = arr[l];
            for (int i = l; i < r; i++)
                arr[i] = arr[i + 1];
            arr[r] = temp;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private static void RotateRight(Corner[] arr, int l, int r)
            // Right rotation of all array elements between l and r
        {
            Corner temp = arr[r];
            for (int i = r; i > l; i--)
                arr[i] = arr[i - 1];
            arr[l] = temp;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private static void RotateLeft(Edge[] arr, int l, int r)
            // Left rotation of all array elements between l and r
        {
            Edge temp = arr[l];
            for (int i = l; i < r; i++)
                arr[i] = arr[i + 1];
            arr[r] = temp;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private static void RotateRight(Edge[] arr, int l, int r)
            // Right rotation of all array elements between l and r
        {
            Edge temp = arr[r];
            for (int i = r; i > l; i--)
                arr[i] = arr[i - 1];
            arr[l] = temp;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // return cube in facelet representation
        public FaceCube ToFaceCube()
        {
            FaceCube fcRet = new FaceCube();
            foreach (Corner c in Enum.GetValues(typeof(Corner)))
            {
                int i = (int)c;
                int j = (int)CP[i];// cornercubie with index j is at
                // cornerposition with index i
                sbyte ori = CO[i];// Orientation of this cubie
                for (int n = 0; n < 3; n++)
                    fcRet.F[(int)FaceCube.CornerFacelet[i][(n + ori) % 3]] = FaceCube.CornerColor[j][n];
            }

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
            {
                int i = (int)e;
                int j = (int)EP[i];// edgecubie with index j is at edgeposition
                // with index i
                sbyte ori = EO[i];// Orientation of this cubie
                for (int n = 0; n < 2; n++)
                    fcRet.F[(int)FaceCube.EdgeFacelet[i][(n + ori) % 2]] = FaceCube.EdgeColor[j][n];
            }
            return fcRet;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Multiply this CubieCube with another cubiecube b, restricted to the corners.<br>
        // Because we also describe reflections of the whole cube by permutations, we get a complication with the corners. The
        // orientations of mirrored corners are described by the numbers 3, 4 and 5. The composition of the orientations
        // cannot
        // be computed by addition modulo three in the cyclic group C3 any more. Instead the rules below give an addition in
        // the dihedral group D3 with 6 elements.<br>
        //	 
        // NOTE: Because we do not use symmetry reductions and hence no mirrored cubes in this simple implementation of the
        // Two-Phase-Algorithm, some code is not necessary here.
        //	
        public void CornerMultiply(CubieCube b)
        {
            Corner[] cPerm = new Corner[8];
            sbyte[] cOri = new sbyte[8];
            foreach (Corner corn in Enum.GetValues(typeof(Corner)))
            {
                cPerm[(int)corn] = CP[(int)b.CP[(int)corn]];

                sbyte oriA = CO[(int)b.CP[(int)corn]];
                sbyte oriB = b.CO[(int)corn];
                sbyte ori = 0;
            
                if (oriA < 3 && oriB < 3) // if both cubes are regular cubes...
                {
                    ori = (sbyte)(oriA + oriB); // just do an addition modulo 3 here
                    if (ori >= 3)
                        ori -= 3; // the composition is a regular cube

                    // +++++++++++++++++++++not used in this implementation +++++++++++++++++++++++++++++++++++
                }
                else if (oriA < 3 && oriB >= 3) // if cube b is in a mirrored
                    // state...
                {
                    ori = (sbyte)(oriA + oriB);
                    if (ori >= 6)
                        ori -= 3; // the composition is a mirrored cube
                }
                else if (oriA >= 3 && oriB < 3) // if cube a is an a mirrored
                    // state...
                {
                    ori = (sbyte)(oriA - oriB);
                    if (ori < 3)
                        ori += 3; // the composition is a mirrored cube
                }
                else if (oriA >= 3 && oriB >= 3) // if both cubes are in mirrored
                    // states...
                {
                    ori = (sbyte)(oriA - oriB);
                    if (ori < 0)
                        ori += 3; // the composition is a regular cube
                    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                }
                cOri[(int)corn] = ori;
            }

            foreach (Corner c in Enum.GetValues(typeof(Corner)))
            {
                CP[(int)c] = cPerm[(int)c];
                CO[(int)c] = cOri[(int)c];
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Multiply this CubieCube with another cubiecube b, restricted to the edges.
        public void EdgeMultiply(CubieCube b)
        {
            Edge[] ePerm = new Edge[12];
            sbyte[] eOri = new sbyte[12];

            foreach (Edge edge in Enum.GetValues(typeof(Edge)))
            {
                ePerm[(int)edge] = EP[(int)b.EP[(int)edge]];
                eOri[(int)edge] = (sbyte)((b.EO[(int)edge] + EO[(int)b.EP[(int)edge]]) % 2);
            }

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
            {
                EP[(int)e] = ePerm[(int)e];
                EO[(int)e] = eOri[(int)e];
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Multiply this CubieCube with another CubieCube b.
        private void Multiply(CubieCube b)
        {
            CornerMultiply(b);
            // edgeMultiply(b);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Compute the inverse CubieCube
        private void InvCubieCube(CubieCube c)
        {
            foreach (Edge edge in Enum.GetValues(typeof(Edge)))
                c.EP[(int)EP[(int)edge]] = edge;

            foreach (Edge edge in Enum.GetValues(typeof(Edge)))
                c.EO[(int)edge] = EO[(int)c.EP[(int)edge]];

            foreach (Corner corn in Enum.GetValues(typeof(Corner)))
                c.CP[(int)CP[(int)corn]] = corn;

            foreach (Corner corn in Enum.GetValues(typeof(Corner)))
            {
                sbyte ori = CO[(int)c.CP[(int)corn]];
                if (ori >= 3)// Just for completeness. We do not invert mirrored
                    // cubes in the program.
                    c.CO[(int)corn] = ori;
                else
                {
                    // the standard case
                    c.CO[(int)corn] = (sbyte)-ori;
                    if (c.CO[(int)corn] < 0)
                        c.CO[(int)corn] += 3;
                }
            }
        }

        // ********************************************* Get and set coordinates *********************************************

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // return the twist of the 8 corners. 0 <= twist < 3^7
        public short GetTwist()
        {
            short ret = 0;
            for (int i = (int)URF; i < (int)DRB; i++)
                ret = (short)(3 * ret + CO[i]);
            return ret;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetTwist(short twist)
        {
            int twistParity = 0;
            for (int i = (int)DRB - 1; i >= (int)URF; i--)
            {
                twistParity += CO[i] = (sbyte)(twist % 3);
                twist /= 3;
            }
            CO[(int)DRB] = (sbyte)((3 - twistParity % 3) % 3);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // return the flip of the 12 edges. 0<= flip < 2^11
        public short GetFlip()
        {
            short ret = 0;
            for (int i =(int)UR; i < (int)BR; i++)
                ret = (short)(2 * ret + EO[i]);
            return ret;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetFlip(short flip)
        {
            int flipParity = 0;
            for (int i = (int)BR - 1; i >= (int)UR; i--)
            {
                flipParity += EO[i] = (sbyte)(flip % 2);
                flip /= 2;
            }
            EO[(int)BR] = (sbyte)((2 - flipParity % 2) % 2);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Parity of the corner permutation
        public short CornerParity()
        {
            int s = 0;
            for (int i = (int)DRB; i >= (int)URF + 1; i--)
            for (int j = i - 1; j >= (int)URF; j--)
                if ((int)CP[j] > (int)CP[i])
                    s++;
            return (short)(s % 2);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Parity of the edges permutation. Parity of corners and edges are the same if the cube is solvable.
        public short EdgeParity()
        {
            int s = 0;
            for (int i = BR.Ordinal(); i >= UR.Ordinal() + 1; i--)
            for (int j = i - 1; j >= UR.Ordinal(); j--)
                if (EP[j].Ordinal() > EP[i].Ordinal())
                    s++;

            return (short)(s % 2);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // permutation of the UD-slice edges FR,FL,BL and BR
        public short GetFRtoBr()
        {
            int a = 0, x = 0;
            Edge[] edge4 = new Edge[4];
            // compute the index a < (12 choose 4) and the permutation array perm.
            for (int j = BR.Ordinal(); j >= UR.Ordinal(); j--)
                if (FR.Ordinal() <= EP[j].Ordinal() && EP[j].Ordinal() <= BR.Ordinal())
                {
                    a += Cnk(11 - j, x + 1);
                    edge4[3 - x++] = EP[j];
                }

            int b = 0;
            for (int j = 3; j > 0; j--)// compute the index b < 4! for the
                // permutation in perm
            {
                int k = 0;
                while (edge4[j].Ordinal() != j + 8)
                {
                    RotateLeft(edge4, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return (short)(24 * a + b);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetFRtoBr(short idx)
        {
            int x;
            Edge[] sliceEdge = { FR, FL, BL, BR };
            Edge[] otherEdge = { UR, UF, UL, UB, DR, DF, DL, DB };
            int b = idx % 24; // Permutation
            int a = idx / 24; // Combination

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
                EP[e.Ordinal()] = DB;// Use UR to invalidate all edges

            for (int j = 1, k; j < 4; j++)// generate permutation from index b
            {
                k = b % (j + 1);
                b /= j + 1;
                while (k-- > 0)
                    RotateRight(sliceEdge, 0, j);
            }

            x = 3;// generate combination and set slice edges
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (a - Cnk(11 - j, x + 1) >= 0)
                {
                    EP[j] = sliceEdge[3 - x];
                    a -= Cnk(11 - j, x-- + 1);
                }

            x = 0; // set the remaining edges UR..DB
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (EP[j] == DB)
                    EP[j] = otherEdge[x++];

        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Permutation of all corners except DBL and DRB
        public short GetUrFtoDlf()
        {
            int a = 0, x = 0;
            Corner[] corner6 = new Corner[6];
            // compute the index a < (8 choose 6) and the corner permutation.
            for (int j = URF.Ordinal(); j <= DRB.Ordinal(); j++)
                if (CP[j].Ordinal() <= DLF.Ordinal())
                {
                    a += Cnk(j, x + 1);
                    corner6[x++] = CP[j];
                }

            int b = 0;
            for (int j = 5; j > 0; j--)// compute the index b < 6! for the
                // permutation in corner6
            {
                int k = 0;
                while (corner6[j].Ordinal() != j)
                {
                    RotateLeft(corner6, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return (short)(720 * a + b);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetUrFtoDlf(short idx)
        {
            int x;
            Corner[] corner6 = { URF, UFL, ULB, UBR, DFR, DLF };
            Corner[] otherCorner = { DBL, DRB };
            int b = idx % 720; // Permutation
            int a = idx / 720; // Combination

            foreach (Corner c in Enum.GetValues(typeof(Corner)))
                CP[c.Ordinal()] = DRB;// Use DRB to invalidate all corners

            for (int j = 1, k; j < 6; j++)// generate permutation from index b
            {
                k = b % (j + 1);
                b /= j + 1;
                while (k-- > 0)
                    RotateRight(corner6, 0, j);
            }

            x = 5;// generate combination and set corners
            for (int j = DRB.Ordinal(); j >= 0; j--)
                if (a - Cnk(j, x + 1) >= 0)
                {
                    CP[j] = corner6[x];
                    a -= Cnk(j, x-- + 1);
                }

            x = 0;
            for (int j = URF.Ordinal(); j <= DRB.Ordinal(); j++)
                if (CP[j] == DRB)
                    CP[j] = otherCorner[x++];
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Permutation of the six edges UR,UF,UL,UB,DR,DF.
        public int GetURtoDF()
        {
            int a = 0, x = 0;
            Edge[] edge6 = new Edge[6];
            // compute the index a < (12 choose 6) and the edge permutation.
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (EP[j].Ordinal() <= DF.Ordinal())
                {
                    a += Cnk(j, x + 1);
                    edge6[x++] = EP[j];
                }

            int b = 0;
            for (int j = 5; j > 0; j--)// compute the index b < 6! for the
                // permutation in edge6
            {
                int k = 0;
                while (edge6[j].Ordinal() != j)
                {
                    RotateLeft(edge6, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return 720 * a + b;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetURtoDF(int idx)
        {
            int x;
            Edge[] edge6 = { UR, UF, UL, UB, DR, DF };
            Edge[] otherEdge = { DL, DB, FR, FL, BL, BR };
            int b = idx % 720; // Permutation
            int a = idx / 720; // Combination

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
                EP[e.Ordinal()] = BR;// Use BR to invalidate all edges

            for (int j = 1; j < 6; j++) // generate permutation from index b
            {
                int k = b % (j + 1);
                b /= j + 1;
                while (k-- > 0)
                    RotateRight(edge6, 0, j);
            }

            x = 5;// generate combination and set edges
            for (int j = BR.Ordinal(); j >= 0; j--)
                if (a - Cnk(j, x + 1) >= 0)
                {
                    EP[j] = edge6[x];
                    a -= Cnk(j, x-- + 1);
                }

            x = 0; // set the remaining edges DL..BR
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (EP[j] == BR)
                    EP[j] = otherEdge[x++];
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Permutation of the six edges UR,UF,UL,UB,DR,DF
        public static int GetURtoDF(short idx1, short idx2)
        {
            CubieCube a = new CubieCube();
            CubieCube b = new CubieCube();
            a.SetURtoUl(idx1);
            b.SetUBtoDf(idx2);

            for (int i = 0; i < 8; i++)
            {
                if (a.EP[i] != BR)
                    if (b.EP[i] != BR) // collision
                        return -1;
                    else
                        b.EP[i] = a.EP[i];
            }

            return b.GetURtoDF();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Permutation of the three edges UR,UF,UL
        public short GetURtoUl()
        {
            int a = 0, x = 0;
            Edge[] edge3 = new Edge[3];

            // compute the index a < (12 choose 3) and the edge permutation.
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (EP[j].Ordinal() <= UL.Ordinal())
                {
                    a += Cnk(j, x + 1);
                    edge3[x++] = EP[j];
                }

            int b = 0;
            for (int j = 2; j > 0; j--) // compute the index b < 3! for the
                // permutation in edge3
            {
                int k = 0;
                while (edge3[j].Ordinal() != j)
                {
                    RotateLeft(edge3, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return (short)(6 * a + b);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetURtoUl(short idx)
        {
            int x;
            Edge[] edge3 = { UR, UF, UL };
            int b = idx % 6; // Permutation
            int a = idx / 6; // Combination

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
                EP[e.Ordinal()] = BR;// Use BR to invalidate all edges

            for (int j = 1; j < 3; j++)// generate permutation from index b
            {
                int k = b % (j + 1);
                b /= j + 1;
                while (k-- > 0)
                    RotateRight(edge3, 0, j);
            }

            x = 2;// generate combination and set edges
            for (int j = BR.Ordinal(); j >= 0; j--)
                if (a - Cnk(j, x + 1) >= 0)
                {
                    EP[j] = edge3[x];
                    a -= Cnk(j, x-- + 1);
                }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Permutation of the three edges UB,DR,DF
        public short GetUBtoDf()
        {
            int a = 0, x = 0;
            Edge[] edge3 = new Edge[3];
            // compute the index a < (12 choose 3) and the edge permutation.
            for (int j = UR.Ordinal(); j <= BR.Ordinal(); j++)
                if (UB.Ordinal() <= EP[j].Ordinal() && EP[j].Ordinal() <= DF.Ordinal())
                {
                    a += Cnk(j, x + 1);
                    edge3[x++] = EP[j];
                }

            int b = 0;
            for (int j = 2; j > 0; j--)// compute the index b < 3! for the
                // permutation in edge3
            {
                int k = 0;
                while (edge3[j].Ordinal() != UB.Ordinal() + j)
                {
                    RotateLeft(edge3, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }

            return (short)(6 * a + b);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetUBtoDf(short idx)
        {
            int x;
            Edge[] edge3 = { UB, DR, DF };
            int b = idx % 6; // Permutation
            int a = idx / 6; // Combination

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
                EP[e.Ordinal()] = BR; // Use BR to invalidate all edges

            for (int j = 1, k; j < 3; j++) // generate permutation from index b
            {
                k = b % (j + 1);
                b /= j + 1;
                while (k-- > 0)
                    RotateRight(edge3, 0, j);
            }

            x = 2; // generate combination and set edges
            for (int j = BR.Ordinal(); j >= 0; j--)
                if (a - Cnk(j, x + 1) >= 0)
                {
                    EP[j] = edge3[x];
                    a -= Cnk(j, x-- + 1);
                }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private int GetUrFtoDlb()
        {
            Corner[] perm = new Corner[8];
            int b = 0;
            for (int i = 0; i < 8; i++)
                perm[i] = CP[i];
            for (int j = 7; j > 0; j--)// compute the index b < 8! for the permutation in perm
            {
                int k = 0;
                while (perm[j].Ordinal() != j)
                {
                    RotateLeft(perm, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return b;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetUrFtoDlb(int idx)
        {
            Corner[] perm = { URF, UFL, ULB, UBR, DFR, DLF, DBL, DRB };
            int k;
            for (int j = 1; j < 8; j++)
            {
                k = idx % (j + 1);
                idx /= j + 1;
                while (k-- > 0)
                    RotateRight(perm, 0, j);
            }

            int x = 7; // set corners
            for (int j = 7; j >= 0; j--)
                CP[j] = perm[x--];
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private int GetURtoBr()
        {
            Edge[] perm = new Edge[12];
            int b = 0;
            for (int i = 0; i < 12; i++)
                perm[i] = EP[i];

            for (int j = 11; j > 0; j--) // compute the index b < 12! for the permutation in perm
            {
                int k = 0;
                while (perm[j].Ordinal() != j)
                {
                    RotateLeft(perm, 0, j);
                    k++;
                }
                b = (j + 1) * b + k;
            }
            return b;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void SetURtoBr(int idx)
        {
            Edge[] perm = { UR, UF, UL, UB, DR, DF, DL, DB, FR, FL, BL, BR };
            for (int j = 1; j < 12; j++)
            {
                int k = idx % (j + 1);
                idx /= j + 1;
                while (k-- > 0)
                    RotateRight(perm, 0, j);
            }

            int x = 11;// set edges
            for (int j = 11; j >= 0; j--)
                EP[j] = perm[x--];
        }

        /// <summary>
        /// Check a cubiecube for solvability. Throws an expection if invalid
        /// </summary>
        /// <exception cref="DuplicateEdgesException">-2: Not all 12 edges exist exactly once</exception>
        /// <exception cref="FlipErrorException">-3: Flip error: One edge has to be flipped</exception>
        /// <exception cref="DuplicateCornersException">-4: Not all corners exist exactly once</exception>
        /// <exception cref="TwistedCornerException">-5: Twist error: One corner has to be twisted</exception>
        /// <exception cref="ParityException">-6: Parity error: Two corners ore two edges have to be exchanged</exception>
        public void Validate()
        {
            int sum = 0;
            int[] edgeCount = new int[12];

            foreach (Edge e in Enum.GetValues(typeof(Edge)))
                    edgeCount[EP[e.Ordinal()].Ordinal()]++;

            for (int i = 0; i < 12; i++)
                if (edgeCount[i] != 1)
                    throw new DuplicateEdgesException();

            for (int i = 0; i < 12; i++)
                sum += EO[i];

            if (sum % 2 != 0)
                throw new FlipErrorException();

            int[] cornerCount = new int[8];
            foreach (Corner c in Enum.GetValues(typeof(Corner)))
                cornerCount[CP[c.Ordinal()].Ordinal()]++;

            for (int i = 0; i < 8; i++)
                if (cornerCount[i] != 1)
                    throw new DuplicateCornersException(); // duplicate/missing corners

            sum = 0;
            for (int i = 0; i < 8; i++)
                sum += CO[i];

            if (sum % 3 != 0)
                throw new TwistedCornerException(); // twisted corner

            if ((EdgeParity() ^ CornerParity()) != 0)
                throw new ParityException(); // parity error

            // cube ok
        }
    }

}
