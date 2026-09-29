/* SymbolBuilder.cs - Builds the barcode sysmbol from the row data supplied. */

/*
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>

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
    ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
    OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
    HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
    OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet
{
    /// <summary>
    /// Build the symbols row data.
    /// </summary>
    internal static class SymbolBuilder
    {
        public static void BuildSymbol(Collection<SymbolData> Symbol, StringBuilder symbolPattern, float height)
        {
            List<byte> rowData = new List<byte>();
            bool isBar = true;	    // Start with a bar.
            for (int i = 0; i < symbolPattern.Length; i++)
            {
                int value = symbolPattern[i] - '0';
                for (int j = 0; j < value; j++)
                {
                    if (isBar)
                    {
                        rowData.Add(1);
                    }

                    else
                    {
                        rowData.Add(0);
                    }
                }

                isBar = !isBar;
            }

            SymbolData symbolData = new SymbolData(rowData.ToArray(), height);
            Symbol.Add(symbolData);
        }

        /// <summary>
        /// // Translate 4-state data pattern to symbol.
        /// </summary>
        /// <param name="Symbol">barcode</param>
        /// <param name="symbolPattern">pattern to translate</param>
        public static void FourStateSymbol(Collection<SymbolData> Symbol, StringBuilder symbolPattern)
        {
            // Turn the symbol into a bar pattern.
            int index = 0;
            int patternLength = symbolPattern.Length;
            byte[] rowData1 = new byte[patternLength * 2];
            byte[] rowData2 = new byte[patternLength * 2];
            byte[] rowData3 = new byte[patternLength * 2];
            SymbolData symbolData;

            for (int i = 0; i < patternLength; i++)
            {
                // Full or Ascender.
                if ((symbolPattern[i] == 'F') || (symbolPattern[i] == 'A') || (symbolPattern[i] == '0') || (symbolPattern[i] == '1'))
                {
                    rowData1[index] = 1;
                }

                // Tracker.
                rowData2[index] = 1;

                // Full or Decender.
                if ((symbolPattern[i] == 'F') || (symbolPattern[i] == 'D') || (symbolPattern[i] == '0') || (symbolPattern[i] == '2'))
                {
                    rowData3[index] = 1;
                }

                index += 2;
            }

            symbolData = new SymbolData(rowData1, 4.0f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData2, 2.5f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData3, 4.0f);
            Symbol.Add(symbolData);
        }

        public static void USPostSymbol(Collection<SymbolData> Symbol, StringBuilder symbolPattern)
        {
            SymbolData symbolData;
            byte[] rowData1;
            byte[] rowData2;

            int patternLength = symbolPattern.Length;
            rowData1 = new byte[patternLength * 3];
            rowData2 = new byte[patternLength * 3];

            int index = 0;
            for (int p = 0; p < patternLength; p++)
            {
                if (symbolPattern[p] == 'L')
                {
                    rowData1[index] = 1;
                }

                rowData2[index] = 1;
                index += 3;
            }

            symbolData = new SymbolData(rowData1, 6.0f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData2, 4.0f);
            Symbol.Add(symbolData);
        }
    }
}
