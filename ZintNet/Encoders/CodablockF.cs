/* Codablock.c - Handles Codablock-F and Codablock-E 2D barcode */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Harald Oehlmann and other Zint Authors and Contributors.
   
    libzint - the open source barcode library
    Copyright (C) 2025 Harald Oehlmann

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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// CodablockF analysing map.
    /// </summary>
    internal sealed class CharacterSetTable
    {
        public int CharacterSet = 0;   // Still possible character sets for actual.
        public int AFollowing = 0;     // Still following Characters in Charset A.
        public int BFollowing = 0;     // Still following Characters in Charset B.
        public int CFollowing = 0;     // Still following Characters in Charset C.
    }

    /// <summary>
    /// Codablock F symbol encoder.
    /// </summary>
    internal class CodaBlockEncoder : SymbolEncoder
    {
        #region Tables.

        private static readonly string[] Code128Table = {
            "212222","222122","222221","121223","121322","131222","122213","122312","132212",
            "221213","221312","231212","112232","122132","122231","113222","123122","123221",
            "223211","221132","221231","213212","223112","312131","311222","321122","321221",
            "312212","322112","322211","212123","212321","232121","111323","131123","131321",
            "112313","132113","132311","211313","231113","231311","112133","112331","132131",
            "113123","113321","133121","313121","211331","231131","213113","213311","213131",
            "311123","311321","331121","312113","312311","332111","314111","221411","431111",
            "111224","111422","121124","121421","141122","141221","112214","112412","122114",
            "122411","142112","142211","241211","221114","413111","241112","134111","111242",
            "121142","121241","114212","124112","124211","411212","421112","421211","212141",
            "214121","412121","111143","111341","131141","114113","114311","411113","411311",
            "113141","114131","311141","411131","211412","211214","211232","2331112" };

        #endregion

        #region Constants.

        // Number of bars per character. (plus 2 for end character)
        // private const int C128Elements = 11;
        // FTab C128 flags - may be added.
        private const int CodeA = 1;
        private const int CodeB = 2;
        private const int CodeC = 4;
        private const int CEnd = 8;
        private const int CShift = 16;
        private const int CFill = 32;
        private const int CodeFNC1 = 64;
        private const int CodeFNC4 = 128;
        private const int ZTNum = (CodeA | CodeB | CodeC);
        private const int ZTFNC1 = (CodeA | CodeB | CodeC | CodeFNC1);

        // ASCII-Extension for Codablock-F.
        private const byte aFNC1 = 128;
        private const byte aFNC2 = 129;
        private const byte aFNC3 = 130;
        private const byte aFNC4 = 131;
        private const byte aCodeA = 132;
        private const byte aCodeB = 133;
        private const byte aCodeC = 134;
        private const byte aShift = 135;

        #endregion

        private readonly int codaBlockRows;
        private readonly int codaBlockColumns;

        public CodaBlockEncoder(Symbology symbolId, char[] barcodeMessage, int codaBlockRows, int codaBlockColumns, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.codaBlockRows = codaBlockRows;
            this.codaBlockColumns = codaBlockColumns;
            this.encodingMode = encodingMode;
            elementsPerCharacter = 11;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (encodingMode)
            {
                case EncodingFormat.Standard:
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    CodaBlockF();
                    break;

                case EncodingFormat.HIBC:
                    barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                    CodaBlockF();
                    break;
            }

            return Symbol;
        }

        private void CodaBlockF()
        {
            bool error;
            int usableColumns;
            int checksum1, checkSum2;
            int currentRow;
            int currentCharacter;
            int currentCharacterSet;
            int emptyColumns;
            byte[] data;
            int[] characterSet;
            byte[] symbolGrid;
            int gridPosition;
            int rows = codaBlockRows;
            int columns = codaBlockColumns;
            int fillings = 0;
            StringBuilder rowPattern;
            int inputLength = barcodeData.Length;

            List<byte> dataList = new List<byte>();
            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] > 127)
                {
                    dataList.Add(aFNC4);
                    dataList.Add((byte)(barcodeData[i] & 127));
                }

                else
                {
                    dataList.Add((byte)barcodeData[i]);
                }
            }

            int dataLength = dataList.Count;
            data = dataList.ToArray();
            characterSet = new int[dataLength];

            Collection<CharacterSetTable> characterSetTables = new Collection<CharacterSetTable>();
            for (int i = 0; i < dataLength; i++)
            {
                characterSetTables.Add(new CharacterSetTable());
            }

            CreateCharacterSetTable(characterSetTables, data, dataLength);
            // Find final row and column count.
            // Nor row nor column count given.
            if (rows <= 0 && columns <= 0)
            {
                // Use 1/1 aspect/ratio Codablock.
                columns = ((int)Math.Floor(Math.Sqrt(1.0 * dataLength)) + 5);
                if (columns > 67)
                {
                    columns = 67;
                }

                else if (columns < 9)
                {
                    columns = 9;
                }
            }

            // There are 5 Codewords for Organisation Start(2), row(1), CheckSum & Stop.
            usableColumns = columns - 5;
            if (rows > 0)  // Row count given.
            {
                error = RowsToColumns(characterSetTables, dataLength, ref rows, ref usableColumns, characterSet, ref fillings);
            }

            else  // Column count given.
            {
                error = ColumnsToRows(characterSetTables, dataLength, ref rows, ref usableColumns, characterSet, ref fillings);
            }

            if (error)
            {
                throw new InvalidDataLengthException("Codablock-F: Input too long, requires too many symbol characters (maximum 2726).");
            }

            // Checksum.
            checksum1 = checkSum2 = 0;
            for (int pos = 0; pos < dataLength; pos++)
            {
                checksum1 = (checksum1 + (((pos % 86) + 1) * data[pos])) % 86;
                checkSum2 = (checkSum2 + (pos % 86 * data[pos])) % 86;
            }

            columns = usableColumns + 5;
            symbolGrid = new byte[columns * rows];
            gridPosition = 0;
            currentCharacter = 0;

            // Loop over rows.
            for (currentRow = 0; currentRow < rows; currentRow++)
            {
                if (currentCharacter >= dataLength)
                {
                    // Empty line with StartA, aCodeB, row #, and then filler aCodeC aCodeB etc.
                    symbolGrid[gridPosition++] = 0x67;
                    symbolGrid[gridPosition++] = 0x64;
                    currentCharacterSet = CodeB;
                    SumASCII(symbolGrid, ref gridPosition, currentRow + 42, currentCharacterSet);
                    emptyColumns = usableColumns;
                    if (currentRow == rows - 1)
                    {
                        emptyColumns -= 2;
                    }

                    while (emptyColumns > 0)
                    {
                        if (currentCharacterSet == CodeC)
                        {
                            A2C128_C(symbolGrid, ref gridPosition, aCodeB, 0);
                            currentCharacterSet = CodeB;
                        }

                        else
                        {
                            A2C128_B(symbolGrid, ref gridPosition, aCodeC);
                            currentCharacterSet = CodeC;
                        }

                        emptyColumns--;
                    }
                }

                else
                {
                    // Normal Line.
                    // Startcode.
                    switch (characterSet[currentCharacter] & (CodeA | CodeB | CodeC))
                    {
                        case CodeA:
                            symbolGrid[gridPosition++] = 0x67;
                            symbolGrid[gridPosition++] = 0x62;
                            currentCharacterSet = CodeA;
                            break;

                        case CodeB:
                            symbolGrid[gridPosition++] = 0x67;
                            symbolGrid[gridPosition++] = 0x64;
                            currentCharacterSet = CodeB;
                            break;

                        case CodeC:
                        default:
                            symbolGrid[gridPosition++] = 0x67;
                            symbolGrid[gridPosition++] = 0x63;
                            currentCharacterSet = CodeC;
                            break;
                    }

                    // Set F1
                    // In first line : # of rows
                    // In Case of CodeA we shifted to CodeB
                    SumASCII(symbolGrid, ref gridPosition, (currentRow == 0) ? rows - 2 : currentRow + 42, currentCharacterSet);

                    // Data 
                    emptyColumns = usableColumns;
                    // Character loop.
                    while (emptyColumns > 0 && currentCharacter < dataLength)
                    {
                        // Change character set ?
                        if (emptyColumns < usableColumns)
                        {
                            if ((characterSet[currentCharacter] & CodeA) != 0)
                            {
                                // Change to A.
                                ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, aCodeA, 0);
                                emptyColumns--;
                                currentCharacterSet = CodeA;
                            }

                            else if ((characterSet[currentCharacter] & CodeB) != 0)
                            {
                                // Change to B.
                                ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, aCodeB, 0);
                                emptyColumns--;
                                currentCharacterSet = CodeB;
                            }

                            else if ((characterSet[currentCharacter] & CodeC) != 0)
                            {
                                // Change to C.
                                ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, aCodeC, 0);
                                emptyColumns--;
                                currentCharacterSet = CodeC;
                            }
                        }

                        if ((characterSet[currentCharacter] & CShift) != 0)
                        {
                            // Shift it and put out the shifted character.
                            ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, aShift, 0);
                            emptyColumns -= 2;
                            currentCharacterSet = (currentCharacterSet == CodeB) ? CodeA : CodeB;
                            ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, data[currentCharacter], 0);
                            currentCharacterSet = (currentCharacterSet == CodeB) ? CodeA : CodeB;
                        }

                        else
                        {
                            // Normal character.
                            if (currentCharacterSet == CodeC)
                            {
                                if (data[currentCharacter] == aFNC1)
                                {
                                    A2C128_C(symbolGrid, ref gridPosition, aFNC1, 0);
                                }

                                else
                                {
                                    A2C128_C(symbolGrid, ref gridPosition, data[currentCharacter], (byte)(currentCharacter + 1 < dataLength ? data[currentCharacter + 1] : 0));
                                    currentCharacter++;
                                }
                            }

                            else
                            {
                                ASCIIZ128(symbolGrid, ref gridPosition, currentCharacterSet, data[currentCharacter], 0);
                            }

                            emptyColumns--;
                        }

                        // End Criteria.
                        if (currentCharacter < dataLength && ((characterSet[currentCharacter] & CFill) > 0 || (characterSet[currentCharacter] & CEnd) > 0))
                        {
                            // Fill Line but leave space for checks in last line.
                            if (currentRow == rows - 1)
                            {
                                emptyColumns -= 2;
                            }

                            while (emptyColumns > 0)
                            {
                                switch (currentCharacterSet)
                                {
                                    case CodeC:
                                        A2C128_C(symbolGrid, ref gridPosition, aCodeB, 0);
                                        currentCharacterSet = CodeB;
                                        break;

                                    case CodeB:
                                        A2C128_B(symbolGrid, ref gridPosition, aCodeC);
                                        currentCharacterSet = CodeC;
                                        break;

                                    case CodeA:
                                        A2C128_A(symbolGrid, ref gridPosition, aCodeC);
                                        currentCharacterSet = CodeC;
                                        break;
                                }

                                emptyColumns--;
                            }
                        }

                       /* if ((characterSet[currentCharacter] & CEnd) != 0)
                        {
                            emptyColumns = 0;
                        }*/

                        currentCharacter++;
                    }
                }

                // Add checksum in last line.
                if (currentRow == rows - 1)
                {
                    SumASCII(symbolGrid, ref gridPosition, checksum1, currentCharacterSet);
                    SumASCII(symbolGrid, ref gridPosition, checkSum2, currentCharacterSet);
                }

                // Add Code 128 checksum.
                {
                    int code128Checksum = symbolGrid[columns * currentRow] % 103;
                    int position = 1;
                    for (; position < usableColumns + 3; position++)
                    {
                        code128Checksum = (code128Checksum + symbolGrid[columns * currentRow + position] * position) % 103;
                    }

                    symbolGrid[gridPosition++] = (byte)code128Checksum;
                }

                // Add terminaton character.
                symbolGrid[gridPosition++] = 106;
            }

            // Build the symbol.
            for (int r = 0; r < rows; r++)
            {
                rowPattern = new StringBuilder();
                for (int c = 0; c < columns; c++)
                {
                    rowPattern.Append(Code128Table[symbolGrid[(r * columns) + c]]);
                }

                // Expand row into the symbol data..
                SymbolBuilder.BuildSymbol(Symbol, rowPattern, 10.0f);
            }
        }

        /// <summary>
        /// Creates a table for each data charater.
        /// </summary>
        /// <remarks>
        /// int CharacterSet: is and or of CodeA,CodeB,CodeC,CodeFNC1, in dependency which character set is applicable. (Result of GetPossibleCharacterSet)
        /// int AFollowing, BFollowing: The number of source characters you still may encode in this character set.
        /// int CFollowing: The number of characters encodable in CodeC if we start here.
        /// </remarks>
        /// <param name="characterSetTables"></param>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        private void CreateCharacterSetTable(Collection<CharacterSetTable> characterSetTables, byte[] data, int dataLength)
        {
            int currentCharacter;
            int runCharacter;
            bool prevNotFNC4;

            // Treat the Data backwards.
            currentCharacter = dataLength - 1;
            prevNotFNC4 = currentCharacter > 0 ? data[currentCharacter - 1] != aFNC4 : true;
            characterSetTables[currentCharacter].CharacterSet = GetPossibleCharacterSet(data[currentCharacter], prevNotFNC4);
            characterSetTables[currentCharacter].AFollowing = ((characterSetTables[currentCharacter].CharacterSet & CodeA) == 0) ? 0 : 1;
            characterSetTables[currentCharacter].BFollowing = ((characterSetTables[currentCharacter].CharacterSet & CodeB) == 0) ? 0 : 1;
            characterSetTables[currentCharacter].CFollowing = 0;

            for (currentCharacter--; currentCharacter >= 0; currentCharacter--)
            {
                prevNotFNC4 = currentCharacter > 0 ? data[currentCharacter - 1] != aFNC4 : true;
                characterSetTables[currentCharacter].CharacterSet = GetPossibleCharacterSet(data[currentCharacter], prevNotFNC4);
                characterSetTables[currentCharacter].AFollowing = ((characterSetTables[currentCharacter].CharacterSet & CodeA) == 0) ? 0 : characterSetTables[currentCharacter + 1].AFollowing + 1;
                characterSetTables[currentCharacter].BFollowing = ((characterSetTables[currentCharacter].CharacterSet & CodeB) == 0) ? 0 : characterSetTables[currentCharacter + 1].BFollowing + 1;
                characterSetTables[currentCharacter].CFollowing = 0;
            }

            // Find the CodeC-chains.
            for (currentCharacter = 0; currentCharacter < dataLength; currentCharacter++)
            {
                characterSetTables[currentCharacter].CFollowing = 0;
                if ((characterSetTables[currentCharacter].CharacterSet & CodeC) != 0)
                {
                    // CodeC possible.
                    runCharacter = currentCharacter;
                    do
                    {
                        // Whether this is FNC1 whether next is numeric.
                        if (characterSetTables[runCharacter].CharacterSet == ZTFNC1) // FNC1.
                        {
                            ++characterSetTables[currentCharacter].CFollowing;
                        }

                        else
                        {
                            runCharacter++;
                            if (runCharacter >= dataLength)
                            {
                                break;
                            }

                            // Only a Number may follow.
                            if (characterSetTables[runCharacter].CharacterSet == ZTNum)
                            {
                                characterSetTables[currentCharacter].CFollowing += 2;
                            }

                            else
                            {
                                break;
                            }
                        }

                        runCharacter++;
                    } while (runCharacter < dataLength);
                }
            }
        }

        /// <summary>
        /// Find the possible Code-128 Character set for a character.
        /// </summary>
        /// <remarks>
        /// The result is an or of CodeA, CodeB, CodeC, CodeFNC1 in dependency of the
        /// possible Code 128 character sets.
        /// </remarks>
        /// <param name="currentCharacter">Current character.</param>
        /// <param name="prevNotFNC4">True if previuos character was FNC4</param>
        /// <returns>Character set</returns>

        private int GetPossibleCharacterSet(byte currentCharacter, bool prevNotFNC4)
        {
            if (currentCharacter <= 0x1f)
            {
                return CodeA;
            }

            if (char.IsDigit((char)currentCharacter) && prevNotFNC4)
            {
                return ZTNum;           // ZTNum=CodeA+CodeB+CodeC.
            }

            if (currentCharacter == aFNC1)
            {
                return ZTFNC1;          // ZTFNC1=CodeA+CodeB+CodeC+CodeFNC1.
            }

            if (currentCharacter == aFNC4)
            {
                return (CodeA | CodeB | CodeFNC4);
            }

            if (currentCharacter >= 0x60 && currentCharacter <= 0x7f)      // 60 to 127.
            {
                return CodeB;
            }

            return CodeA | CodeB;
        }

        private bool RowsToColumns(Collection<CharacterSetTable> characterSetTables, int dataLength,
                                   ref int rows, ref int useColumns, int[] characterSet, ref int fillings)
        {
            bool error;
            int rowsRequested;      // Number of requested rows.
            int columnsRequested;   // Number of requested columns (if any).
            int testColumns;        // To enter into ColumnToRows.
            int[] testList = new int[62];
            int testListSize = 0;
            int[] backupSet = new int[dataLength];

            rowsRequested = rows;
            columnsRequested = useColumns >= 4 ? useColumns : 0;
            if (columnsRequested > 0)
            {
                testColumns = columnsRequested;
            }

            else
            {
                // First guess.
                testColumns = dataLength / rowsRequested;
                if (testColumns > 62)
                {
                    testColumns = 62;
                }

                else if (testColumns < 4)
                {
                    testColumns = 4;
                }
            }

            for (; ; )
            {
                testList[testListSize] = testColumns;
                testListSize++;
                useColumns = testColumns;   // Make a copy because it may be modified.
                error = ColumnsToRows(characterSetTables, dataLength, ref rows, ref useColumns, characterSet, ref fillings);
                if (error)
                {
                    return error;
                }

                if (rows <= rowsRequested)
                {
                    // Less or exactly line number found.
                    // Check if column count below already tested or at smallest/requested.
                    bool fInTestList = (rows == 2 || testColumns == 4 || testColumns == columnsRequested);
                    int posCur;
                    for (posCur = 0; posCur < testListSize && !fInTestList; posCur++)
                    {
                        if (testList[posCur] == testColumns - 1)
                        {
                            fInTestList = true;
                        }
                    }

                    if (fInTestList)
                    {
                        // Smaller width already tested.
                        if (rows < rowsRequested)
                        {
                            fillings += useColumns * (rowsRequested - rows);
                            rows = rowsRequested;
                        }

                        // Exit with actual.
                        return false;
                    }

                    // Test more rows (shorter CDB).
                    Array.Copy(characterSet, backupSet, dataLength);
                    testColumns--;
                }

                else
                {
                    // To many rows.
                    // Test less rows(longer code).
                    Array.Copy(characterSet, backupSet, dataLength);
                    if (testColumns++ > 62)
                    {
                        return true;
                    }
                }
            }
        }

        private bool ColumnsToRows(Collection<CharacterSetTable> characterSetTables, int dataLength,
                                   ref int rows, ref int useableColumns, int[] characterSet, ref int fillings)
        {
            int currentCharacter;
            int runCharacter;
            int emptyColumns;       // Number of codes still empty in line.
            int emptyColumns2;      // Alternative emptyColumns to compare.
            int codeCPairs;         // Number of digit pairs which may fit in the line.
            int currentCharSet;     // Current character set.
            bool isFNC4;            // Set true if current character FNC4.

            // Loop until currentRows <= 44.
            do
            {
                for (int i = 0; i < characterSet.Length; i++)
                {
                    characterSet[i] = 0;
                }

                currentCharacter = 0;
                rows = 0;

                // Line loop.
                do
                {
                    // Start Character.
                    emptyColumns = useableColumns;    // Remained place in Line.

                    // Choose in Set A or B.
                    // (C is changed as an option later on)
                    characterSet[currentCharacter] = currentCharSet = (characterSetTables[currentCharacter].AFollowing > characterSetTables[currentCharacter].BFollowing) ? CodeA : CodeB;

                    // Test on Numeric Mode C.
                    codeCPairs = RemainingDigits(characterSetTables, currentCharacter, emptyColumns);
                    if (codeCPairs >= 4)
                    {
                        // 4 Digits in Numeric, compression OK.
                        /* May be an odd start find more
                           Skip leading <FNC1>'s 
                           Typical structure : <FNC1><FNC1>12... 
                           Test if numeric after one isn't better.*/
                        runCharacter = currentCharacter;
                        emptyColumns2 = emptyColumns;
                        while (characterSetTables[runCharacter].CharacterSet == ZTFNC1)
                        {
                            runCharacter++;
                            emptyColumns2--;
                        }

                        if (codeCPairs >= RemainingDigits(characterSetTables, runCharacter + 1, emptyColumns2 - 1))
                        {
                            // Start odd is not better.
                            // We start in C.
                            characterSet[currentCharacter] = currentCharSet = CodeC;

                            // Increment currentCharacter.
                            if (characterSetTables[currentCharacter].CharacterSet != ZTFNC1)
                            {
                                currentCharacter++;      // 2 Num.Digits.
                            }
                        }
                    }

                    currentCharacter++;
                    emptyColumns--;

                    // Following characters.
                    while (emptyColumns > 0 && currentCharacter < dataLength)
                    {
                        isFNC4 = (characterSetTables[currentCharacter].CharacterSet & CodeFNC4) > 0;
                        switch (currentCharSet)
                        {
                            case CodeA:
                            case CodeB:
                                // Check switching to Code C.
                                /* Switch if :
                                 *  - Character not FNC1
                                 *  - 4 real Digits will fit in line
                                 *  - an odd Start will not be better
                                 */
                                if (characterSetTables[currentCharacter].CharacterSet == ZTNum
                                    && (codeCPairs = RemainingDigits(characterSetTables, currentCharacter, emptyColumns - 1)) >= 4
                                    && codeCPairs > RemainingDigits(characterSetTables, currentCharacter + 1, emptyColumns - 2))
                                {
                                    // Change to C.
                                    characterSet[currentCharacter] = currentCharSet = CodeC;
                                    currentCharacter += 2;      // 2 digits.
                                    emptyColumns -= 2;          // <SwitchC> 12.
                                }

                                else if (currentCharSet == CodeA)
                                {
                                    if (characterSetTables[currentCharacter].AFollowing == 0 || (isFNC4 && characterSetTables[currentCharacter].AFollowing == 1))
                                    {
                                        // Must change to B.
                                        if (emptyColumns == 1 || (isFNC4 && emptyColumns == 2))
                                        {
                                            // Can't switch.
                                            characterSet[currentCharacter - 1] |= CEnd + CFill;
                                            emptyColumns = 0;
                                        }

                                        else
                                        {
                                            // <Shift> or <switchB>.
                                            if (characterSetTables[currentCharacter].BFollowing == 1 || (isFNC4 && characterSetTables[currentCharacter].BFollowing == 2))
                                            {
                                                // Note using order "FNC4 shift char" (same as CODE128) not "shift FNC4 char"
                                                // as given in Table B.1 and Table B.2 
                                                if (isFNC4)
                                                {
                                                    // So skip FNC4 and shift value instead.
                                                    emptyColumns--;
                                                    currentCharacter++;
                                                }

                                                characterSet[currentCharacter] |= CShift;
                                            }

                                            else
                                            {
                                                characterSet[currentCharacter] |= CodeB;
                                                currentCharSet = CodeB;
                                            }

                                            emptyColumns -= 2;
                                            currentCharacter++;

                                        }
                                    }

                                    else if (isFNC4 && emptyColumns == 1)
                                    {
                                        // Can't fit extended ASCII on same line.
                                        characterSet[currentCharacter - 1] |= CEnd + CFill;
                                        emptyColumns = 0;
                                    }

                                    else
                                    {
                                        emptyColumns--;
                                        currentCharacter++;
                                    }
                                }

                                else
                                {
                                    // Last possibility, CodeB.
                                    if (characterSetTables[currentCharacter].BFollowing == 0 || (isFNC4 && characterSetTables[currentCharacter].BFollowing == 1))
                                    {
                                        // Must change to A.
                                        if (emptyColumns == 1 || (isFNC4 && emptyColumns == 2))
                                        {
                                            // Can't switch.
                                            characterSet[currentCharacter - 1] |= CEnd + CFill;
                                            emptyColumns = 0;
                                        }

                                        else
                                        {
                                            // <Shift> or <switchA>.
                                            if (characterSetTables[currentCharacter].AFollowing == 1 || (isFNC4 && characterSetTables[currentCharacter].AFollowing == 2))
                                            {
                                                // Note: using order "FNC4 shift char" (same as CODE128) not "shift FNC4 char" as given in Table B.1 and Table B.2
                                                if (isFNC4)
                                                {
                                                    // So skip FNC4 and shift value instead.
                                                    emptyColumns--;
                                                    currentCharacter++;
                                                }

                                                characterSet[currentCharacter] |= CShift;
                                            }

                                            else
                                            {
                                                characterSet[currentCharacter] |= CodeA;
                                                currentCharSet = CodeA;
                                            }

                                            emptyColumns -= 2;
                                            currentCharacter++;
                                        }
                                    }

                                    else if (isFNC4 && emptyColumns == 1)
                                    {
                                        // Can't fit extended ASCII on same line.
                                        characterSet[currentCharacter - 1] |= CEnd + CFill;
                                        emptyColumns = 0;
                                    }

                                    else
                                    {
                                        emptyColumns--;
                                        currentCharacter++;
                                    }
                                }

                                break;

                            case CodeC:
                                if (characterSetTables[currentCharacter].CFollowing > 0)
                                {
                                    currentCharacter += (characterSetTables[currentCharacter].CharacterSet == ZTFNC1) ? 1 : 2;
                                    emptyColumns--;
                                }

                                else
                                {
                                    // Must change to A or B.
                                    if (emptyColumns == 1 || (isFNC4 && emptyColumns == 2))
                                    {
                                        // Can't switch.
                                        characterSet[currentCharacter - 1] |= CEnd + CFill;
                                        emptyColumns = 0;
                                    }

                                    else
                                    {
                                        // <Shift> or <switchA>.
                                        currentCharSet = characterSet[currentCharacter]
                                                       = (characterSetTables[currentCharacter].AFollowing > characterSetTables[currentCharacter].BFollowing) ? CodeA : CodeB;
                                        emptyColumns -= 2;
                                        currentCharacter++;
                                    }
                                }

                                break;
                        }
                    }

                    // End of code line.
                    characterSet[currentCharacter - 1] |= CEnd;
                    rows++;
                } while (currentCharacter < dataLength);

                // Place check characters C1, C2.
                switch (emptyColumns)
                {
                    case 1:
                        characterSet[currentCharacter - 1] |= CFill;
                        rows++;
                        fillings = useableColumns - 2 + emptyColumns;
                        /* Glide in following block without break */    // not allowed in c#
                        break;

                    case 0:
                        rows++;
                        fillings = useableColumns - 2 + emptyColumns;
                        break;

                    case 2:
                        fillings = 0;
                        break;

                    default:
                        characterSet[currentCharacter - 1] |= CFill;
                        fillings = emptyColumns - 2;
                        break;
                }

                if (rows > 44)
                {
                    useableColumns++;
                    if (useableColumns > 62)
                    {
                        throw new InvalidDataLengthException("Codablock-F: Input data too long.");
                    }
                }

                else if (rows == 1)
                {
                    rows = 2;
                    fillings += useableColumns;
                }

            } while (rows > 44);

            return false;
        }

        /* Find the number of numerical characters in pairs which will fit in
         * one bundle into the line (up to here). This is calculated online because
         * it depends on the space in the line.
         */
        private int RemainingDigits(Collection<CharacterSetTable> characterSetTables, int currentCharacter, int emptyColumns)
        {
            int digitCount;     // Numerical digits fitting in the line.
            int runCharacter = currentCharacter;
            digitCount = 0;

            while (emptyColumns > 0 && runCharacter < currentCharacter + characterSetTables[currentCharacter].CFollowing)
            {
                if (characterSetTables[runCharacter].CharacterSet != ZTFNC1)
                {
                    // Not FNC1.
                    digitCount += 2;
                    runCharacter++;
                }

                runCharacter++;
                emptyColumns--;
            }

            return digitCount;
        }

        // Output a character in Characterset.
        private void ASCIIZ128(byte[] symbolGrid, ref int gridPosition, int CharacterSet, byte c1, byte c2)
        {
            if (CharacterSet == CodeA)
            {
                A2C128_A(symbolGrid, ref gridPosition, c1);
            }

            else if (CharacterSet == CodeB)
            {
                A2C128_B(symbolGrid, ref gridPosition, c1);
            }

            else
            {
                A2C128_C(symbolGrid, ref gridPosition, c1, c2);
            }
        }

        // XLate Table A of Codablock-F Specification and call output.
        private void SumASCII(byte[] symbolGrid, ref int gridPosition, int sum, int characterSet)
        {
            switch (characterSet)
            {
                case CodeA:
                case CodeB:
                    if (sum <= 31)
                    {
                        A2C128_B(symbolGrid, ref gridPosition, (byte)(sum + 96));
                    }

                    else if (sum <= 47)
                    {
                        A2C128_B(symbolGrid, ref gridPosition, (byte)sum);
                    }

                    else
                    {
                        A2C128_B(symbolGrid, ref gridPosition, (byte)(sum + 10));
                    }

                    break;

                case CodeC:
                    A2C128_C(symbolGrid, ref gridPosition, (byte)(sum / 10 + '0'), (byte)(sum % 10 + '0'));
                    break;
            }
        }

        // Print a character in character set A.
        private void A2C128_A(byte[] symbolGrid, ref int gridPosition, byte c)
        {
            switch (c)
            {
                case aCodeB: symbolGrid[gridPosition] = 100; break;
                case aFNC4: symbolGrid[gridPosition] = 101; break;
                case aFNC1: symbolGrid[gridPosition] = 102; break;
                case aFNC2: symbolGrid[gridPosition] = 97; break;
                case aFNC3: symbolGrid[gridPosition] = 96; break;
                case aCodeC: symbolGrid[gridPosition] = 99; break;
                case aShift: symbolGrid[gridPosition] = 98; break;
                default:
                    if (c >= ' ' && c <= '_')
                    {
                        symbolGrid[gridPosition] = (byte)(c - ' ');
                    }

                    else
                    {
                        symbolGrid[gridPosition] = (byte)(c + 64);
                    }

                    break;
            }

            gridPosition++;
        }


        // Output c in Set B
        private void A2C128_B(byte[] symbolGrid, ref int gridPosition, byte c)
        {
            switch (c)
            {
                case aFNC1: symbolGrid[gridPosition] = 102; break;
                case aFNC2: symbolGrid[gridPosition] = 97; break;
                case aFNC3: symbolGrid[gridPosition] = 96; break;
                case aFNC4: symbolGrid[gridPosition] = 100; break;
                case aCodeA: symbolGrid[gridPosition] = 101; break;
                case aCodeC: symbolGrid[gridPosition] = 99; break;
                case aShift: symbolGrid[gridPosition] = 98; break;
                default: symbolGrid[gridPosition] = (byte)(c - ' '); break;
            }

            gridPosition++;
        }

        // Output c1, c2 in Set C.
        private void A2C128_C(byte[] symbolGrid, ref int gridPosition, byte c1, byte c2)
        {
            switch (c1)
            {
                case aFNC1: symbolGrid[gridPosition] = 102; break;
                case aCodeB: symbolGrid[gridPosition] = 100; break;
                case aCodeA: symbolGrid[gridPosition] = 101; break;
                default: symbolGrid[gridPosition] = (byte)(10 * (c1 - '0') + (c2 - '0')); break;
            }

            gridPosition++;
        }
    }
}
