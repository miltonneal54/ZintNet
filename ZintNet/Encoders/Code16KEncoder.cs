/* Code16Encoder.cs - Handles Code16K 2D symbol */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
 
    libzint - the open source barcode library
    Copyright (C) 2008-2025 Robin Stuart <rstuart114@gmail.com>

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

using System.Globalization;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Code 16K encoder.
    /// </summary>
    internal class Code16KEncoder : SymbolEncoder
    {
        #region Tables.

        private readonly string[] Code16KTable = {
            "212222", "222122", "222221", "121223", "121322", "131222", "122213",
            "122312", "132212", "221213", "221312", "231212", "112232", "122132", "122231", "113222",
            "123122", "123221", "223211", "221132", "221231", "213212", "223112", "312131", "311222",
            "321122", "321221", "312212", "322112", "322211", "212123", "212321", "232121", "111323",
            "131123", "131321", "112313", "132113", "132311", "211313", "231113", "231311", "112133",
            "112331", "132131", "113123", "113321", "133121", "313121", "211331", "231131", "213113",
            "213311", "213131", "311123", "311321", "331121", "312113", "312311", "332111", "314111",
            "221411", "431111", "111224", "111422", "121124", "121421", "141122", "141221", "112214",
            "112412", "122114", "122411", "142112", "142211", "241211", "221114", "413111", "241112",
            "134111", "111242", "121142", "121241", "114212", "124112", "124211", "411212", "421112",
            "421211", "212141", "214121", "412121", "111143", "111341", "131141", "114113", "114311",
            "411113", "411311", "113141", "114131", "311141", "411131", "211412", "211214", "211232",
            "211133" };

        // EN 12323 Table 3 and Table 4 - Start patterns and stop patterns.
        private readonly string[] C16KStartStop = { "3211", "2221", "2122", "1411", "1132", "1231", "1114", "3112" };


        // EN 12323 Table 5 - Start and stop values defining row numbers.
        private readonly int[] C16KStartValues = { 0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7 };
        private readonly int[] C16KStopValues = { 0, 1, 2, 3, 4, 5, 6, 7, 4, 5, 6, 7, 0, 1, 2, 3 };

        #endregion

        #region Constants.

        private const char SHIFTA = 'a';
        private const char LATCHA = 'A';
        private const char SHIFTB = 'b';
        private const char LATCHB = 'B';
        private const char LATCHC = 'C';
        private const char AORB = 'Z';
        private const char ABORC = '9';
        #endregion

        private int optionMinimumRows;

        public Code16KEncoder(Symbology symbolId, char[] barcodeMessage, int optionMinimumRows, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.encodingMode = encodingMode;
            this.optionMinimumRows = optionMinimumRows;
            elementsPerCharacter = 11;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (encodingMode)
            {
                case EncodingFormat.Standard:
                    isGS1 = false;
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    Code16K();
                    break;

                case EncodingFormat.GS1:
                    isGS1 = true;
                    barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                    Code16K();
                    break;
            }

            return Symbol;
        }

        private void Code16K()
        {
            int maxLength = 256;
            int rowsRequired;
            int padding;
            int extraPadding = 0;
            int mode;
            char currentSet;
            int barCharacters;
            int[,] encodingList = new int[2, maxLength];
            char[] set = new char[maxLength];
            char[] fset = new char[maxLength];
            int[] values = new int[maxLength];
            int m, position;
            int checkValue1, checkValue2;
            int checkSum1, checkSum2;
            int inputLength;
            StringBuilder rowPattern;
            inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 16K: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Detect extended ASCII characters.
            for (int i = 0; i < inputLength; i++)
            {
                fset[i] = barcodeData[i] >= 128 ? 'f' : ' ';
            }

            // Note: to be safe not using extended ASCII latch as not mentioned in BS EN 12323:2005.

            // Detect mode A, B and C characters.
            int listIndex = 0;
            int sourceIndex = 0;
            mode = GetMode(barcodeData[sourceIndex], isGS1);
            do
            {
                encodingList[1, listIndex] = mode;
                while ((encodingList[1, listIndex] == mode) && (sourceIndex < inputLength))
                {
                    encodingList[0, listIndex]++;
                    sourceIndex++;
                    if (sourceIndex == inputLength)
                    {
                        break;
                    }

                    mode = GetMode(barcodeData[sourceIndex], isGS1);
                }

                listIndex++;
            } while (sourceIndex < inputLength);

            DxSmooth(encodingList, ref listIndex);

            // Put set data into set[].
            PutInSet(encodingList, listIndex, set, barcodeData);

            // Start with the mode character - Table 2.
            m = 0;
            switch (set[0])
            {
                case 'A':
                    m = 0;
                    break;

                case 'B':
                    m = 1;
                    break;

                case 'C':
                    m = 2;
                    break;
            }

            /*if (symbol->output_options & READER_INIT)
            {
                if (m == 2)
                    m = 5;

                if (isGS1)
                {
                    strcpy(symbol->errtxt, "Cannot use both GS1 mode and Reader Initialisation (D22)");
                    return ZINT_ERROR_INVALID_OPTION;
                }
        
                else
                {
                    if ((set[0] == 'B') && (set[1] == 'C'))
                        m = 6;
                }

                values[bar_characters] = (7 * (rowsRequired - 2)) + m; // See 4.3.4.2
                values[bar_characters + 1] = 96; // FNC3.
                bar_characters += 2;
                }
    
            else
            {*/
            if (isGS1)
            {
                // Integrate FNC1.
                switch (set[0])
                {
                    case 'B':
                        m = 3;
                        break;

                    case 'C':
                        m = 4;
                        break;
                }
            }

            else
            {
                if ((set[0] == 'B') && (set[1] == 'C'))
                {
                    m = fset[0] == 'f' ? 6 : 5;
                }

                else if (((set[0] == 'B') && (set[1] == 'B')) && (set[2] == 'C') && fset[0] != 'f' && fset[1] != 'f')
                {
                    m = 6;
                }
            }

            barCharacters = 1;
            currentSet = set[0];
            position = 0;

            // Encode the data.
            do
            {
                if ((position != 0) && (set[position] != currentSet))
                {
                    // Latch different code set.
                    switch (set[position])
                    {
                        case 'A':
                            values[barCharacters++] = 101;
                            currentSet = 'A';
                            break;

                        case 'B':
                            values[barCharacters++] = 100;
                            currentSet = 'B';
                            break;

                        case 'C':
                            if (!(position == 1 && m >= 5) && !(position == 2 && m == 6))
                            {
                                values[barCharacters++] = 99;
                            }

                            currentSet = 'C';

                            break;
                    }
                }

                if (fset[position] == 'f')
                {
                    // Shift extended mode.
                    switch (currentSet)
                    {
                        case 'A':
                            values[barCharacters++] = 101;  //FNC4.
                            break;

                        case 'B':
                            values[barCharacters++] = 100;  // FNC4.
                            break;
                    }
                }


                if ((set[position] == 'a') || (set[position] == 'b'))
                {
                    // Insert shift character.
                    values[barCharacters++] = 98;
                }

                if (!isGS1 || barcodeData[position] != '\x1d')
                {
                    switch (set[position])
                    {
                        // Encode data characters.
                        case 'A':
                        case 'a':
                            Code16KSetA(barcodeData[position], values, ref barCharacters);
                            position++;
                            break;

                        case 'B':
                        case 'b':
                            _ = Code16KSetB(barcodeData[position], values, ref barCharacters);
                            position++;
                            break;

                        case 'C':
                            Code16KSetC(barcodeData[position], barcodeData[position + 1], values, ref barCharacters);
                            position += 2;
                            break;
                    }
                }

                else
                {
                    values[barCharacters++] = 102;
                    position++;
                }

                if (barCharacters > 80 - 2)
                {
                    // Max rows 16 * 5 - 2 check chars.
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Code 16K: Input too long, requires {0} symbol characters(maximum 78)", barCharacters));
                }

                } while (position < inputLength);

            padding = 5 - ((barCharacters + 2) % 5);
            if (padding == 5)
            {
                padding = 0;
            }

            if ((barCharacters + padding) < 8)
            {
                padding += 8 - (barCharacters + padding);
            }

            rowsRequired = (barCharacters + padding + 4) / 5;
            // User defined rows.
            if (optionMinimumRows > rowsRequired)
            {
                extraPadding = (optionMinimumRows - rowsRequired) * 5;
                rowsRequired = optionMinimumRows;
            }

            for (int i = 0; i < padding + extraPadding; i++)
            {
                values[barCharacters++] = 103;
            }

            values[0] = (7 * (rowsRequired - 2)) + m; // See 4.3.4.2

            // Calculate check digits.
            checkSum1 = 0;
            checkSum2 = 0;
            for (int i = 0; i < barCharacters; i++)
            {
                checkSum1 += (i + 2) * values[i];
                checkSum2 += (i + 1) * values[i];
            }

            checkValue1 = checkSum1 % 107;
            checkSum2 += checkValue1 * (barCharacters + 1);
            checkValue2 = checkSum2 % 107;
            values[barCharacters] = checkValue1;
            values[barCharacters + 1] = checkValue2;

            for (int row = 0; row < rowsRequired; row++)
            {
                rowPattern = new StringBuilder();
                rowPattern.Append(C16KStartStop[C16KStartValues[row]]);
                rowPattern.Append("1");
                for (int i = 0; i < 5; i++)
                {
                    rowPattern.Append(Code16KTable[values[(row * 5) + i]]);
                }

                rowPattern.Append(C16KStartStop[C16KStopValues[row]]);

                // Expand row into the symbol data.
                SymbolBuilder.BuildSymbol(Symbol, rowPattern, 10.0f);
            }
        }

        private void PutInSet(int[,] encodingList, int listIndex, char[] set, char[] source)
        {
            int position = 0;
            int i, j;
            int cCount = 0;
            bool haveNonC = false;

            for (i = 0; i < listIndex; i++)
            {
                for (j = 0; j < encodingList[0, i]; j++)
                {
                    set[position++] = (char)(encodingList[1, i]);
                }
            }

            // Watch out for odd-length Mode C blocks.
            for (i = 0; i < position; i++)
            {
                if (set[i] == 'C')
                {
                    if (source[i] == '\x1d')
                    {
                        if ((cCount & 1) > 0)
                        {
                            haveNonC = true;
                            if (i > cCount)
                            {
                                set[i - cCount] = 'B';
                            }

                            else
                            {
                                set[i - 1] = 'B';
                            }
                        }

                        cCount = 0;
                    }

                    else
                    {
                        cCount++;
                    }
                }

                else
                {
                    haveNonC = true;
                    if ((cCount & 1) > 0)
                    {
                        if (i > cCount)
                        {
                            set[i - cCount] = 'B';
                        }

                        else
                        {
                            set[i - 1] = 'B';
                        }
                    }
                    cCount = 0;
                }
            }

            if ((cCount & 1) > 0)
            {
                if (i > cCount && haveNonC)
                {
                    set[i - cCount] = 'B';
                    if (cCount < 4)
                    {
                        // Rule 1b.
                        for (j = i - cCount + 1; j < i; j++)
                        {
                            set[j] = 'B';
                        }
                    }
                }
                else
                {
                    set[i - 1] = 'B';
                }
            }
            for (i = 1; i < position - 1; i++)
            {
                if (set[i] == 'C' && set[i - 1] != 'C' && set[i + 1] != 'C')
                {
                    set[i] = set[i + 1];
                }
            }
            if (position > 1 && set[position - 1] == 'C' && set[position - 2] != 'C')
            {
                set[position - 1] = set[position - 2];
            }
        }

        // Determine appropriate mode for a given character.
        private int GetMode(char value, bool checkFNC1)
        {
            int mode;

            if (value <= 31)
            {
                mode = checkFNC1 && value == '\x1D' ? ABORC : SHIFTA;
            }

            else if ((value >= 48) && (value <= 57))
            {
                mode = ABORC;
            }

            else if (value <= 95)
            {
                mode = AORB;
            }

            else if (value <= 127)
            {
                mode = SHIFTB;
            }

            else if (value <= 159)
            {
                mode = SHIFTA;
            }

            else if (value <= 223)
            {
                mode = AORB;
            }

            else
            {
                mode = SHIFTB;
            }

            return mode;
        }

        // Implements rules from ISO 15417 Annex E
        private void DxSmooth(int[,] encodingList, ref int listIndex)
        {
            int nextShift = 0;
            int nextShiftI = 0;

            for (int i = 0; i < listIndex; i++)
            {
                int current = encodingList[1, i];   // Either ABORC, AORB, SHIFTA or SHIFTB.
                int length = encodingList[0, i];

                if (i == nextShiftI)
                {
                    nextShift = 0;
                    // Set next shift to aid deciding between latching to A or B - taken from Okapi, props Daniel Gredler.
                    for (int j = i + 1; j < listIndex; j++)
                    {
                        if (encodingList[1, j] == SHIFTA || encodingList[1, j] == SHIFTB)
                        {
                            nextShift = encodingList[1, j];
                            nextShiftI = j;
                            break;
                        }
                    }
                }

                if (i == 0) // First block.
                {
                    if (current == ABORC)
                    {
                        // Rule 1a.
                        if ((listIndex == 1) && (length == 2))
                        {
                            encodingList[1, i] = LATCHC;
                            current = LATCHC;
                        }

                        // Rule 1b.
                        else if (length >= 4)
                        {
                            encodingList[1, i] = LATCHC;
                            current = LATCHC;
                        }

                        else
                        {
                            current = AORB; // Determined below.
                        }
                    }

                    if (current == AORB)
                    {
                        if (nextShift == SHIFTA)
                        {
                            // Rule 1c.
                            encodingList[1, i] = LATCHA;
                        }

                        else
                        {
                            // Rule 1d.
                            encodingList[1, i] = LATCHB;
                        }
                    }
                }

                else
                {
                    int last = encodingList[1, i - 1];
                    if (current == ABORC)
                    {
                        if (length >= 4)
                        {
                            // Rule 3 - note Rule 3b (odd C blocks) dealt with later.
                            encodingList[1, i] = LATCHC;
                            current = LATCHC;
                        }

                        else
                        {
                            current = AORB; // Determine below.
                        }
                    }

                    if (current == AORB)
                    {
                        if (last == LATCHA || last == SHIFTB)
                        {

                            encodingList[1, i] = LATCHA;
                        }

                        else if (last == LATCHB || last == SHIFTA)
                        {
                            // Maintain state.
                            encodingList[1, i] = LATCHB;
                        }

                        else if (nextShift == SHIFTA)
                        {
                            encodingList[1, i] = LATCHA;
                        }

                        else
                        {
                            encodingList[1, i] = LATCHB;
                        }
                    }

                    else if (current == SHIFTA)
                    {
                        if (length > 1)
                        {
                            // Rule 4.
                            encodingList[1, i] = LATCHA;
                        }

                        else if (last == LATCHA || last == SHIFTB)
                        {
                            // Maintain state.
                            encodingList[1, i] = LATCHA;
                        }

                        else if (last == LATCHC)
                        {
                            encodingList[1, i] = LATCHA;
                        }
                    }

                    else if (current == SHIFTB)
                    {
                        // Unless LATCHX set above, can only be C16K_SHIFTB.
                        if (length > 1)
                        {
                            // Rule 5.
                            encodingList[1, i] = LATCHB;
                        }

                        else if (last == LATCHB || last == SHIFTA)
                        {
                            // Maintain state.
                            encodingList[1, i] = LATCHB;
                        }

                        else if (last == LATCHC)
                        {
                            encodingList[1, i] = LATCHB;
                        }
                    }
                } // Rule 2 is implemented elsewhere, Rule 6 is implied.
            }

            GroupBlocks(encodingList, ref listIndex);
        }

        /// <summary>
        /// Bring together same type blocks.
        /// </summary>
        /// <param name="encodingList"></param>
        /// <param name="listIndex"></param>
        private void GroupBlocks(int[,] encodingList, ref int listIndex)
        {
            int i, j;

            // Bring together same type blocks.
            if (listIndex > 1)
            {
                i = 1;
                while (i < listIndex)
                {
                    if (encodingList[1, i - 1] == encodingList[1, i])
                    {
                        // Bring together.
                        encodingList[0, i - 1] = encodingList[0, i - 1] + encodingList[0, i];
                        j = i + 1;

                        // Decreace the list.
                        while (j < listIndex)
                        {
                            encodingList[0, j - 1] = encodingList[0, j];
                            encodingList[1, j - 1] = encodingList[1, j];
                            j++;
                        }

                        listIndex -= 1;
                        i--;
                    }

                    i++;
                }
            }
        }

        private void Code16KSetA(char source, int[] values, ref int barCharacters)
        {
            if (source >= 128)
            {
                if (source < 160)
                {
                    values[barCharacters] = (source - 128) + 64;
                }

                else
                {
                    values[barCharacters] = (source - 128) - 32;
                }
            }

            else
            {
                if (source < 32)
                {
                    values[barCharacters] = source + 64;
                }

                else
                {
                    values[barCharacters] = source - 32;
                }
            }

            barCharacters++;
        }

        /* Translate Code 128 Set B characters into barcodes.
         * This set handles all characters which are not part of long numbers and not
         * control characters.
         */
        private bool Code16KSetB(char source, int[] values, ref int barCharacters)
        {
            if (source >= 128 + 32)
            {
                values[barCharacters] = source - 32 - 128;
            }

            else if (source >= 128)
            {
                // Should never happen.
                return false;
            }

            else if( source >= 32)
            {
                values[barCharacters] = source - 32;
            }

            else
            {
                // Should never happen.
                return false;
            }

            barCharacters++;
            return true;
        }

        /* Translate Code 128 Set C characters into barcodes
         * This set handles numbers in a compressed form
         */
        private void Code16KSetC(char sourceA, char sourceB, int[] values, ref int barCharacters)
        {
            values[barCharacters] = (10 * (sourceA - '0')) + (sourceB - '0');
            barCharacters++;
        }
    }
}
