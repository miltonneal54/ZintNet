/* DefineModes.cs - Calculate optimized character encoding modes.

/*
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

    1. Redistributions of source code must retain the above copyright
       notice, this list of conditions and the following disclaimer.
    2. Redistributions in binary form must reproduce the above copyright
       notice, this list of conditions and the following disclaimer in the
       documentation and/or other materials provided with the distribution.
    3. Neither the name of the project nor the names of its contributors
       may be used to endorse or promote products derived from this software
       without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
    OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
    HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
    OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
 * 
 *  Adapted from Project Nayuki.
 *  Copyright (c) Project Nayuki. (MIT License)
            * https://www.nayuki.io/page/qr-code-generator-library
            *
            * Permission is hereby granted, free of charge, to any person obtaining a copy of
            * this software and associated documentation files (the "Software"), to deal in
            * the Software without restriction, including without limitation the rights to
            * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
            * the Software, and to permit persons to whom the Software is furnished to do so,
            * subject to the following conditions:
            * - The above copyright notice and this permission notice shall be included in
            *   all copies or substantial portions of the Software.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZintNet
{
    internal static class CharacterModes
    {
        public delegate int[] HeadCost(int[] state);
        public delegate int SwitchCost(int[] state, int k, int j);
        public delegate int EodCost(int k);
        public delegate void CurrentCost(int[] state, char[] data, int length, int position, char[] characterModes, int[] previousCosts, int[] currentCosts);

        public static void DefineModes(char[] mode, char[] data, int length, int[] state, char[] modeTypes, int numberOfModes,
                                       HeadCost headCost, SwitchCost switchCost, EodCost eodCost, CurrentCost currentCost)
        {
            int minimumCost;
            char currentMode;
            int[] previousCosts = new int[numberOfModes];
            int[] currentCosts = new int[numberOfModes];
            char[] characterModes = new char[length * numberOfModes];

            // characterModes[i * num_modes + j] represents the mode to encode the code point at index i such that the final
            // segment ends in mode_types[j] and the total number of bits is minimized over all possible choices.

            // At the beginning of each iteration of the loop below, prev_costs[j] is the minimum number of 1/6 (1/XX_MULT)
            // bits needed to encode the entire string prefix of length i, and end in mode_types[j]
            Array.Copy(headCost(state), previousCosts, numberOfModes);

            // Calculate costs using dynamic programming.
            for (int position = 0, cmIdx = 0; position < length; position++, cmIdx += numberOfModes)
            {
                for (int i = 0; i < numberOfModes; i++)
                    currentCosts[i] = 0;

                currentCost(state, data, length, position, characterModes, previousCosts, currentCosts);
                if (eodCost != null && position == length - 1)
                {
                    // Add end of data costs if last character.
                    for (int j = 0; j < numberOfModes; j++)
                    {
                        if (characterModes[cmIdx + j] > 0)
                            currentCosts[j] += eodCost(j);
                    }
                }
                // Start new segment at the end to switch modes.
                for (int j = 0; j < numberOfModes; j++)
                {
                    // To mode.
                    for (int k = 0; k < numberOfModes; k++)
                    {
                        // From mode.
                        if (j != k && (characterModes[cmIdx + k] > 0))
                        {
                            int newCost = currentCosts[k] + switchCost(state, k, j);
                            if (characterModes[cmIdx + j] == 0 || newCost < currentCosts[j])
                            {
                                currentCosts[j] = newCost;
                                characterModes[cmIdx + j] = modeTypes[k];
                            }
                        }
                    }
                }

                Array.Copy(currentCosts, previousCosts, numberOfModes);
            }

            // Find optimal ending mode.
            minimumCost = previousCosts[0];
            currentMode = modeTypes[0];
            for (int i = 1; i < numberOfModes; i++)
            {
                if (previousCosts[i] < minimumCost)
                {
                    minimumCost = previousCosts[i];
                    currentMode = modeTypes[i];
                }
            }

            // Get optimal mode for each code point by tracing backwards.
            int modeIndex;
            for (int i = length - 1, cmIdx = i * numberOfModes; i >= 0; i--, cmIdx -= numberOfModes)
            {
                modeIndex = Array.IndexOf(modeTypes, currentMode);
                currentMode = characterModes[cmIdx + modeIndex];
                mode[i] = currentMode;
            }
        }
    }
}
