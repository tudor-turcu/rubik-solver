﻿/*
 * Herbert Kociemba Rubik's cube algorithm: http://kociemba.org/cube.htm - C# port of the original Java code
 */
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RubikCubeSolver.Kociemba.TwoPhase.Exceptions;

namespace RubikCubeSolver.Kociemba.TwoPhase
{
    /// <summary>
    /// Class Search implements the Two-Phase-Algorithm.
    /// </summary>
    public class Search
    {
        private static readonly int[] ax = new int[31]; // The axis of the move
        private static readonly int[] po = new int[31]; // The power of the move

        private static readonly int[] flip = new int[31]; // phase1 coordinates
        private static readonly int[] twist = new int[31];
        private static readonly int[] slice = new int[31];

        private static readonly int[] parity = new int[31]; // phase2 coordinates
        private static readonly int[] URFtoDLF = new int[31];
        private static readonly int[] FRtoBR = new int[31];
        private static readonly int[] URtoUL = new int[31];
        private static readonly int[] UBtoDF = new int[31];
        private static readonly int[] URtoDF = new int[31];

        private static readonly int[] minDistPhase1 = new int[31]; // IDA* distance do goal estimations
        private static readonly int[] minDistPhase2 = new int[31];

        private static MoveTables MoveTables;
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Search>();
        
        /// <summary>
        /// generate the solution string from the array data.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static string SolutionToString(int length)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                switch (ax[i])
                {
                    case 0:
                        s += "U";
                        break;
                    case 1:
                        s += "R";
                        break;
                    case 2:
                        s += "F";
                        break;
                    case 3:
                        s += "D";
                        break;
                    case 4:
                        s += "L";
                        break;
                    case 5:
                        s += "B";
                        break;
                }

                switch (po[i])
                {
                    case 1:
                        s += " ";
                        break;
                    case 2:
                        s += "2 ";
                        break;
                    case 3:
                        s += "' ";
                        break;

                }
            }
            return s;
        }

        /// <summary>
        ///  generate the solution string from the array data including a separator between phase1 and phase2 moves.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="depthPhase1">The depth phase1.</param>
        /// <returns></returns>
        private static string SolutionToString(int length, int depthPhase1)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                switch (ax[i])
                {
                    case 0:
                        s += "U";
                        break;
                    case 1:
                        s += "R";
                        break;
                    case 2:
                        s += "F";
                        break;
                    case 3:
                        s += "D";
                        break;
                    case 4:
                        s += "L";
                        break;
                    case 5:
                        s += "B";
                        break;
                }

                switch (po[i])
                {
                    case 1:
                        s += " ";
                        break;
                    case 2:
                        s += "2 ";
                        break;
                    case 3:
                        s += "' ";
                        break;

                }

                if (i == depthPhase1 - 1)
                    s += ". ";
            }
            return s;
        }
        
        /// <summary>
        /// Computes the solver string for a given cube.
        /// </summary>
        /// <param name="facelets">is the cube definition string, see {@link Facelet} for the format..</param>
        /// <param name="maxDepth">defines the maximal allowed maneuver length. For random cubes, a maxDepth of 21 usually will return a
        ///          solution in less than 0.5 seconds.With a maxDepth of 20 it takes a few seconds on average to find a
        ///          solution, but it may take much longer for specific cubes..</param>
        /// <param name="timeOut">defines the maximum computing time of the method in seconds. If it does not return with a solution, it returns with
        ///          an error code..</param>
        /// <param name="useSeparator">determines if a " . " separates the phase1 and phase2 parts of the solver string like in F' R B R L2 F .
        ///          U2 U D for example..</param>
        /// <returns>The solution string or an error code:
        ///    Error 1: NotOneFaceletOfEachColorException: There is not exactly one facelet of each colour
        ///    
        ///    Error 7: No solution exists for the given maxDepth
        ///    Error 8: Timeout, no solution within given time</returns>
        /// <exception cref="DuplicateEdgesException">-2: Not all 12 edges exist exactly once</exception>
        /// <exception cref="FlipErrorException">-3: Flip error: One edge has to be flipped</exception>
        /// <exception cref="DuplicateCornersException">-4: Not all corners exist exactly once</exception>
        /// <exception cref="TwistedCornerException">-5: Twist error: One corner has to be twisted</exception>
        /// <exception cref="ParityException">-6: Parity error: Two corners ore two edges have to be exchanged</exception>
        public static string Solution(string facelets, int maxDepth, long timeOut, bool useSeparator)
        {
            Logger.LogDebug("Validating the cube ...");

            // +++++++++++++++++++++check for wrong input +++++++++++++++++++++++++++++
            int[] count = new int[6];
            try
            {
                for (int i = 0; i < 54; i++)
                    count[Enum.Parse<Color>(facelets.Substring(i, 1)).Ordinal()]++;
            }
            catch (Exception)
            {
                throw new NotOneFaceletOfEachColorException();
            }

            for (int i = 0; i < 6; i++)
                if (count[i] != 9)
                    throw new NotOneFaceletOfEachColorException();

            FaceCube fc = new FaceCube(facelets);
            CubieCube cc = fc.ToCubieCube();

            cc.Validate();

            // create or load the move tables
            MoveTables = LoadMoveTables();

            // +++++++++++++++++++++++ initialization +++++++++++++++++++++++++++++++++
            CoordCube c = new CoordCube(cc);

            po[0] = 0;
            ax[0] = 0;
            flip[0] = c.Flip;
            twist[0] = c.Twist;
            parity[0] = c.Parity;
            slice[0] = c.FRtoBR / 24;
            URFtoDLF[0] = c.URFtoDLF;
            FRtoBR[0] = c.FRtoBR;
            URtoUL[0] = c.URtoUL;
            UBtoDF[0] = c.UBtoDF;

            minDistPhase1[1] = 1;// else failure for depth=1, n=0
            int n = 0;
            bool busy = false;
            int depthPhase1 = 1;
            
            Stopwatch stopWatch = new Stopwatch();
            
            try
            {
                stopWatch.Start();

                // +++++++++++++++++++ Main loop ++++++++++++++++++++++++++++++++++++++++++
                do
                {
                    do
                    {
                        if ((depthPhase1 - n > minDistPhase1[n + 1]) && !busy)
                        {

                            if (ax[n] == 0 || ax[n] == 3) // Initialize next move
                                ax[++n] = 1;
                            else
                                ax[++n] = 0;
                            po[n] = 1;
                        }
                        else if (++po[n] > 3)
                        {
                            do
                            {
                                // increment axis
                                if (++ax[n] > 5)
                                {

                                    stopWatch.Stop();
                                    if (stopWatch.ElapsedMilliseconds > timeOut * 1000)
                                        return "Error 8";
                                    stopWatch.Start();

                                    if (n == 0)
                                    {
                                        if (depthPhase1 >= maxDepth)
                                            return "Error 7";

                                        depthPhase1++;
                                        ax[n] = 0;
                                        po[n] = 1;
                                        busy = false;
                                        break;
                                    }
                                    else
                                    {
                                        n--;
                                        busy = true;
                                        break;
                                    }

                                }
                                else
                                {
                                    po[n] = 1;
                                    busy = false;
                                }
                            } while (n != 0 && (ax[n - 1] == ax[n] || ax[n - 1] - 3 == ax[n]));
                        }
                        else
                            busy = false;
                    } while (busy);

                    // +++++++++++++ compute new coordinates and new minDistPhase1 ++++++++++
                    // if minDistPhase1 =0, the H subgroup is reached
                    var mv = 3 * ax[n] + po[n] - 1;
                    flip[n + 1] = MoveTables.FlipMove[flip[n], mv];
                    twist[n + 1] = MoveTables.TwistMove[twist[n], mv];
                    slice[n + 1] = MoveTables.FRtoBR_Move[slice[n] * 24, mv] / 24;
                    minDistPhase1[n + 1] = Math.Max(MoveTables.GetPruning(MoveTables.Slice_Flip_Prun,
                        MoveTables.N_SLICE1 * flip[n + 1]
                        + slice[n + 1]), MoveTables.GetPruning(MoveTables.Slice_Twist_Prun,
                        MoveTables.N_SLICE1 * twist[n + 1]
                        + slice[n + 1]));
                    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    if (minDistPhase1[n + 1] == 0 && n >= depthPhase1 - 5)
                    {
                        minDistPhase1[n + 1] = 10; // instead of 10 any value >5 is possible
                        int s;
                        if (n == depthPhase1 - 1 && (s = TotalDepth(depthPhase1, maxDepth)) >= 0)
                        {
                            if (s == depthPhase1
                                || (ax[depthPhase1 - 1] != ax[depthPhase1] &&
                                    ax[depthPhase1 - 1] != ax[depthPhase1] + 3))
                                return useSeparator ? SolutionToString(s, depthPhase1) : SolutionToString(s);
                        }

                    }
                } while (true);

            }
            finally
            {
                stopWatch.Stop();
            }
        }

        private static MoveTables LoadMoveTables()
        {
            string cacheFile = GetCacheFilePath();

            if (File.Exists(cacheFile))
            {
                return RestoreTablesFromFile(cacheFile);
            }

            MoveTables moveTables = new MoveTables();
            moveTables.Build();
            SaveTablesToFile(moveTables, cacheFile);
            return moveTables;
        }

        private static MoveTables RestoreTablesFromFile(string cacheFile)
        {
            Logger.LogDebug($"Loading the move tables from {cacheFile}  ...");

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            JsonSerializer serializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};

            using (StreamReader sw = new StreamReader(cacheFile))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                MoveTables moveTables = serializer.Deserialize<MoveTables>(reader);

                stopWatch.Stop();

                Logger.LogDebug($"Move tables loaded in {stopWatch.Elapsed.TotalSeconds}s");

                return moveTables;
            }
        }

        private static void SaveTablesToFile(MoveTables moveTables, string cacheFile)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            
            Logger.LogDebug($"Saving the move tables to {cacheFile}  ...");

            using (StreamWriter sw = new StreamWriter(cacheFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, moveTables);
            }
        }

        private static string GetCacheFilePath()
        {
            string cachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string cacheFile = Path.Combine(cachePath, "RubikMoveTablesCache.json");
            return cacheFile;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Apply phase2 of algorithm and return the combined phase1 and phase2 depth. In phase2, only the moves
        // U,D,R2,F2,L2 and B2 are allowed.
        private static int TotalDepth(int depthPhase1, int maxDepth)
        {
            int mv = 0, d1 = 0, d2 = 0;
            int maxDepthPhase2 = Math.Min(10, maxDepth - depthPhase1);// Allow only max 10 moves in phase2
            for (int i = 0; i < depthPhase1; i++)
            {
                mv = 3 * ax[i] + po[i] - 1;
                URFtoDLF[i + 1] = MoveTables.URFtoDLF_Move[URFtoDLF[i], mv];
                FRtoBR[i + 1] = MoveTables.FRtoBR_Move[FRtoBR[i], mv];
                parity[i + 1] = MoveTables.ParityMove[parity[i], mv];
            }

            if ((d1 = MoveTables.GetPruning(MoveTables.Slice_URFtoDLF_Parity_Prun,
                    (MoveTables.N_SLICE2 * URFtoDLF[depthPhase1] + FRtoBR[depthPhase1]) * 2 + parity[depthPhase1])) > maxDepthPhase2)
                return -1;

            for (int i = 0; i < depthPhase1; i++)
            {
                mv = 3 * ax[i] + po[i] - 1;
                URtoUL[i + 1] = MoveTables.URtoUL_Move[URtoUL[i], mv];
                UBtoDF[i + 1] = MoveTables.UBtoDF_Move[UBtoDF[i], mv];
            }
            URtoDF[depthPhase1] = MoveTables.MergeURtoULandUBtoDF[URtoUL[depthPhase1], UBtoDF[depthPhase1]];

            if ((d2 = MoveTables.GetPruning(MoveTables.Slice_URtoDF_Parity_Prun,
                    (MoveTables.N_SLICE2 * URtoDF[depthPhase1] + FRtoBR[depthPhase1]) * 2 + parity[depthPhase1])) > maxDepthPhase2)
                return -1;

            if ((minDistPhase2[depthPhase1] = Math.Max(d1, d2)) == 0)// already solved
                return depthPhase1;

            // now set up search

            int depthPhase2 = 1;
            int n = depthPhase1;
            bool busy = false;
            po[depthPhase1] = 0;
            ax[depthPhase1] = 0;
            minDistPhase2[n + 1] = 1;// else failure for depthPhase2=1, n=0
                                     // +++++++++++++++++++ end initialization +++++++++++++++++++++++++++++++++
            do
            {
                do
                {
                    if ((depthPhase1 + depthPhase2 - n > minDistPhase2[n + 1]) && !busy)
                    {

                        if (ax[n] == 0 || ax[n] == 3) // Initialize next move
                        {
                            ax[++n] = 1;
                            po[n] = 2;
                        }
                        else
                        {
                            ax[++n] = 0;
                            po[n] = 1;
                        }
                    }
                    else if ((ax[n] == 0 || ax[n] == 3) ? (++po[n] > 3) : ((po[n] = po[n] + 2) > 3))
                    {
                        do
                        {
                            // increment axis
                            if (++ax[n] > 5)
                            {
                                if (n == depthPhase1)
                                {
                                    if (depthPhase2 >= maxDepthPhase2)
                                        return -1;
                                    else
                                    {
                                        depthPhase2++;
                                        ax[n] = 0;
                                        po[n] = 1;
                                        busy = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    n--;
                                    busy = true;
                                    break;
                                }

                            }
                            else
                            {
                                if (ax[n] == 0 || ax[n] == 3)
                                    po[n] = 1;
                                else
                                    po[n] = 2;
                                busy = false;
                            }
                        } while (n != depthPhase1 && (ax[n - 1] == ax[n] || ax[n - 1] - 3 == ax[n]));
                    }
                    else
                        busy = false;
                } while (busy);

                // +++++++++++++ compute new coordinates and new minDist ++++++++++
                mv = 3 * ax[n] + po[n] - 1;

                URFtoDLF[n + 1] = MoveTables.URFtoDLF_Move[URFtoDLF[n], mv];
                FRtoBR[n + 1] = MoveTables.FRtoBR_Move[FRtoBR[n], mv];
                parity[n + 1] = MoveTables.ParityMove[parity[n], mv];
                URtoDF[n + 1] = MoveTables.URtoDF_Move[URtoDF[n], mv];

                minDistPhase2[n + 1] = Math.Max(MoveTables.GetPruning(MoveTables.Slice_URtoDF_Parity_Prun, (MoveTables.N_SLICE2
                                                                * URtoDF[n + 1] + FRtoBR[n + 1])
                                                                * 2 + parity[n + 1]), MoveTables.GetPruning(MoveTables.Slice_URFtoDLF_Parity_Prun, (MoveTables.N_SLICE2
                                                                        * URFtoDLF[n + 1] + FRtoBR[n + 1])
                                                                        * 2 + parity[n + 1]));
                // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            } while (minDistPhase2[n + 1] != 0);

            return depthPhase1 + depthPhase2;
        }
    }

}
