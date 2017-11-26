/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */
using System;
using RubikCubeSolver.Kociemba.TwoPhase.Exceptions;

namespace RubikCubeSolver.Kociemba.TwoPhase
{

    public class Tools
    {

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Check if the cube string s represents a solvable cube.
        // 0: Cube is solvable
        // -1: There is not exactly one facelet of each colour
        // -2: Not all 12 edges exist exactly once
        // -3: Flip error: One edge has to be flipped
        // -4: Not all corners exist exactly once
        // -5: Twist error: One corner has to be twisted
        // -6: Parity error: Two corners or two edges have to be exchanged
        // 
        /**
         * Check if the cube definition string s represents a solvable cube.
         * 
         * @param s is the cube definition string , see {@link Facelet}
         * @return 0: Cube is solvable<br>
         *         -1: There is not exactly one facelet of each colour<br>
         *         -2: Not all 12 edges exist exactly once<br>
         *         -3: Flip error: One edge has to be flipped<br>
         *         -4: Not all 8 corners exist exactly once<br>
         *         -5: Twist error: One corner has to be twisted<br>
         *         -6: Parity error: Two corners or two edges have to be exchanged
         */

        public static void Verify(string s)
        {
            int[] count = new int[6];
            try
            {
                for (int i = 0; i < 54; i++)
                    count[Enum.Parse<Color>(s.Substring(i, 1)).Ordinal()]++;
            }
            catch (Exception)
            {
                throw new NotOneFaceletOfEachColorException();
            }

            for (int i = 0; i < 6; i++)
                if (count[i] != 9)
                    throw new NotOneFaceletOfEachColorException();

            FaceCube fc = new FaceCube(s);
            CubieCube cc = fc.ToCubieCube();

            cc.Validate();
        }

        /**
         * Generates a random cube.
         * @return A random cube in the string representation. Each cube of the cube space has the same probability.
         */
        public static String RandomCube()
        {
            CubieCube cc = new CubieCube();
            Random gen = new Random();
            cc.SetFlip((short)gen.Next(MoveTables.N_FLIP));
            cc.SetTwist((short)gen.Next(MoveTables.N_TWIST));
            do
            {
                cc.SetUrFtoDlb(gen.Next(MoveTables.N_URFtoDLB));
                cc.SetURtoBr(gen.Next(MoveTables.N_URtoBR));
            } while ((cc.EdgeParity() ^ cc.CornerParity()) != 0);
            FaceCube fc = cc.ToFaceCube();
            return fc.ToString();
        }

        public static string RemoveWhiteSpace(string textPermutation)
        {
            return textPermutation.Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }

        public static void PrettyPrintCube(string textPermutation)
        {
            string configuration = RemoveWhiteSpace(textPermutation);
            
            Console.WriteLine("\n\r-----------------");
            ConsoleColor currentBackground = Console.BackgroundColor;
            ConsoleColor currentForeground = Console.ForegroundColor;

            for (var i = 0; i < configuration.Length; i++)
            {
                char f = configuration[i];

                if (i % 3 == 0)
                {
                    Console.WriteLine();
                }

                if (i % 9 == 0)
                {
                    Console.WriteLine();
                }

                PrintFacet(f);
            }

            Console.ResetColor();

            Console.WriteLine("\n\r-----------------");
        }

        private static void PrintFacet(char f)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            switch (f)
            {
                case 'F':
                    Console.BackgroundColor = ConsoleColor.White;                    
                    break;
                case 'U':
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 'R':
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 'L':
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case 'D':
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case 'B':
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.Write(f);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

}
