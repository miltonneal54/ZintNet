/* CodeOneEncoder.cs - Handles encoding of Code One 2D symbol */

/* 
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2009-2025 Robin Stuart <rstuart114@gmail.com>

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
using System.Globalization;
using System.Collections.ObjectModel;


namespace ZintNet.Encoders
{
    /// <summary>
    /// Builds a Code1 Symbol
    /// </summary>
    internal class CodeOneEncoder : SymbolEncoder
    {
        # region Tables.

        private readonly int[] C40Shift = {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
                2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        private readonly int[] C40Value = {
                 0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                 3,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
                 4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20,
                21, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 22, 23, 24, 25, 26,
                 0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

        private readonly int[] TextShift = {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
            2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2,
            3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3 };

        private readonly int[] TextValue = {
                 0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                 3,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
                 4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20,
                21,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 22, 23, 24, 25, 26,
                 0, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 27, 28, 29, 30, 31 };

        private readonly int[] Code1Height = {
            16, 22, 28, 40, 52, 70, 104, 148 };

        private readonly int[] Code1Width = {
            18, 22, 32, 42, 54, 76, 98, 134 };

        private readonly int[] Code1DataLength = {
            10, 19, 44, 91, 182, 370, 732, 1480 };

        private readonly int[] Code1EccLength = {
            10, 16, 26, 44, 70, 140, 280, 560 };

        private int[] Code1Blocks = {
            1, 1, 1, 1, 1, 2, 4, 8};

        private readonly int[] Code1DataBlocks = {
            10, 19, 44, 91, 182, 185, 183, 185 };

        private readonly int[] Code1EccBlocks = {
            10, 16, 26, 44, 70, 70, 70, 70 };

        private readonly int[] Code1GridWidth = {
            4, 5, 7, 9, 12, 17, 22, 30};

        private readonly int[] Code1GridHeight = {
            5, 7, 10, 15, 21, 30, 46, 68 };

        #endregion

        #region Constants.

        private const int C1_MAX_CWS = 1480;        /* Max data codewords for Version H */
        // private const int C1_MAX_CWS_S = "1480";    /* String version of above */
        private const int C1_MAX_ECCS = 560;        /* Max ECC codewords for Version H */

        private const int ASCII = 1;
        private const int C40 = 2;
        private const int DECIMAL = 3;
        private const int TEXT = 4;
        private const int EDIFACT = 5;
        private const int BYTE = 6;

        # endregion

        private readonly int optionSymbolSize;

        public CodeOneEncoder(Symbology symbolId, char[] barcodeMessage, int optionSymbolSize, int eci, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionSymbolSize = optionSymbolSize;
            this.eci = eci;
            this.encodingMode = encodingMode;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (encodingMode)
            {
                case EncodingFormat.Standard:
                    isGS1 = false;
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    break;

                case EncodingFormat.GS1:
                    isGS1 = true;
                    barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                    break;

                default:
                    return null;
            }

            CodeOne();
            return Symbol;
        }

        private void CodeOne()
        {
            int inputLength = barcodeData.Length;
            int codeOneSize = 1;
            int symbolRows;
            int symbolWidth;
            int subVersion = 0;
            int i, j;
            int tp = 0;
            byte[] symbolGrid;
            char[,] dataGrid = new char[136, 120];
            uint[] ecc;
            uint[] target;
            int row, column;

            if (optionSymbolSize == 9)
            {
                // Version S.
                int codewords;
                int[] binaryInput;
                ulong inputValue;
                int blockWidth;

                ecc = new uint[15];
                target = new uint[30];

                if (inputLength > 18)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Code One: Input data too long for Version 'S'.\nMaximum length is {0} characters.", 18));
                }

                for (i = 0; i < inputLength; i++)
                {
                    if (!char.IsDigit(barcodeData[i]))
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "Numeric only data expected for Version 'S'.\nInvalid character '{0}' at position {1}.", barcodeData[i], i + 1));
                    }
                }

                codeOneSize = 9;
                if (inputLength <= 6)
                {
                    // Version S-10.
                    subVersion = 1;
                    codewords = 4;
                    blockWidth = 2;
                }

                else if (inputLength <= 12)
                {
                    // Version S-20.
                    subVersion = 2;
                    codewords = 8;
                    blockWidth = 4;
                }

                else
                {
                    subVersion = 3;
                    codewords = 12;
                    blockWidth = 6;
                }

                inputValue = ulong.Parse(new string(barcodeMessage), CultureInfo.CurrentCulture) + 1;
                binaryInput = new int[codewords * 5];
                for (i = 0; i < binaryInput.Length; i++)
                {
                    binaryInput[i] = (int)((inputValue >> i) & 0x1);
                }

                for (i = 0; i < codewords; i++)
                {
                    target[codewords - i - 1] += (uint)(1 * binaryInput[i * 5]);
                    target[codewords - i - 1] += (uint)(2 * binaryInput[(i * 5) + 1]);
                    target[codewords - i - 1] += (uint)(4 * binaryInput[(i * 5) + 2]);
                    target[codewords - i - 1] += (uint)(8 * binaryInput[(i * 5) + 3]);
                    target[codewords - i - 1] += (uint)(16 * binaryInput[(i * 5) + 4]);
                }

                ReedSolomon.RSInitialise(0x25, codewords, 0);
                ReedSolomon.RSEncode(codewords, target, ecc);
                for (i = 0; i < codewords; i++)
                {
                    target[i + codewords] = ecc[codewords - i - 1];
                }

                i = 0;
                for (row = 0; row < 2; row++)
                {
                    for (column = 0; column < blockWidth; column++)
                    {
                        dataGrid[row * 2, column * 5] = (char)((target[i] & 0x10) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 5) + 1] = (char)((target[i] & 0x08) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 5) + 2] = (char)((target[i] & 0x04) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, column * 5] = (char)((target[i] & 0x02) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 5) + 1] = (char)((target[i] & 0x01) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 5) + 3] = (char)((target[i + 1] & 0x10) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 5) + 4] = (char)((target[i + 1] & 0x08) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 5) + 2] = (char)((target[i + 1] & 0x04) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 5) + 3] = (char)((target[i + 1] & 0x02) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 5) + 4] = (char)((target[i + 1] & 0x01) > 0 ? '1' : '0');
                        i += 2;
                    }
                }

                symbolRows = 8;
                symbolWidth = 10 * subVersion + 1;
            }

            else if (optionSymbolSize == 10)
            {
                // Version T.
                target = new uint[C1_MAX_CWS + C1_MAX_ECCS];
                ecc = new uint[22];
                int dataLength;
                int dataCodeword, eccCodeword, blockWidth;
                int lastMode = 0;

                dataLength = C1Encode(barcodeData, inputLength, target, ref tp, ref lastMode);
                if (dataLength > 38)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Code One: Input data too long for Version 'T'.\nRequires {0} codewords. (maximum available 38)", dataLength));
                }

                codeOneSize = 10;
                if (dataLength <= 10)
                {
                    subVersion = 1;
                    dataCodeword = 10;
                    eccCodeword = 10;
                    blockWidth = 4;
                }

                else if (dataLength <= 24)
                {
                    subVersion = 2;
                    dataCodeword = 24;
                    eccCodeword = 16;
                    blockWidth = 8;
                }

                else
                {
                    subVersion = 3;
                    dataCodeword = 38;
                    eccCodeword = 22;
                    blockWidth = 12;
                }

                // If require padding.
                if (dataCodeword > dataLength)
                {
                    /* If did not finish in ASCII or BYTE mode, switch to ASCII */
                    if (lastMode != ASCII && lastMode != BYTE)
                    {
                        target[dataLength++] = 255; // Unlatch.
                    }

                    for (i = dataLength; i < dataCodeword; i++)
                    {
                        target[i] = 129; // Pad.
                    }
                }

                // Calculate error correction data.
                ReedSolomon.RSInitialise(0x12d, eccCodeword, 0);
                ReedSolomon.RSEncode(dataCodeword, target, ecc);
                // ECC blocks come back reversed.
                for (i = 0; i < eccCodeword; i++)
                {
                    target[dataCodeword + i] = ecc[eccCodeword - i - 1];
                }

                i = 0;
                for (row = 0; row < 5; row++)
                {
                    for (column = 0; column < blockWidth; column++)
                    {
                        dataGrid[row * 2, column * 4] = (char)((target[i] & 0x80) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 1] = (char)((target[i] & 0x40) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 2] = (char)((target[i] & 0x20) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 3] = (char)((target[i] & 0x10) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, column * 4] = (char)((target[i] & 0x08) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 1] = (char)((target[i] & 0x04) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 2] = (char)((target[i] & 0x02) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 3] = (char)((target[i] & 0x01) > 0 ? '1' : '0');
                        i++;
                    }
                }

                symbolRows = 16;
                symbolWidth = (subVersion * 16) + 1;
            }

            else 
            {
                // Version A to H.
                uint[] subData = new uint[185];
                uint[] subEcc = new uint[70];
                target = new uint[C1_MAX_CWS + C1_MAX_ECCS];
                int dataLength;
                int dataCW;
                int blocks, data_blocks, ecc_blocks, ecc_length;
                int lastMode = 0;

                dataLength = C1Encode(barcodeData, inputLength, target, ref tp, ref lastMode);
                for (i = 7; i >= 0; i--)
                {
                    if (Code1DataLength[i] >= dataLength)
                    {
                        codeOneSize = i + 1;
                    }
                }

                if (optionSymbolSize > codeOneSize)
                {
                    codeOneSize = optionSymbolSize;
                }

                if ((optionSymbolSize != 0) && (optionSymbolSize < codeOneSize))
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Code One: Input data too long for Version '{0}'.\nRequires {1} codewords. (maximum available {2})",
                        (char)(optionSymbolSize + 64), dataLength, Code1DataLength[optionSymbolSize - 1]));
                }

                dataCW = Code1DataLength[codeOneSize - 1];

                // If requires padding.
                if (dataCW > dataLength)
                {
                    // If did not finish in ASCII or BYTE mode, switch to ASCII.
                    if (lastMode != ASCII && lastMode != BYTE)
                    {
                        target[dataLength++] = 255; // Unlatch.
                    }


                    for (i = dataLength; i < dataCW; i++)
                    {
                        target[i] = 129; /* Pad */
                    }
                }

                // Calculate error correction data.
                blocks = Code1Blocks[codeOneSize - 1];
                data_blocks = Code1DataBlocks[codeOneSize - 1];
                ecc_blocks = Code1EccBlocks[codeOneSize - 1];
                ecc_length = Code1EccLength[codeOneSize - 1];

                ReedSolomon.RSInitialise(0x12d, Code1EccBlocks[codeOneSize - 1], 0);
                for (i = 0; i < blocks; i++)
                {
                    for (j = 0; j < data_blocks; j++)
                    {
                        subData[j] = target[j * blocks + i];
                    }

                    ReedSolomon.RSEncode(Code1DataBlocks[codeOneSize - 1], subData, subEcc);
                    // ECC blocks come back reversed.
                    for (j = 0; j < ecc_blocks; j++)
                    {
                        target[dataCW + j * blocks + i] = subEcc[ecc_length - j - 1];
                    }
                }

                i = 0;
                for (row = 0; row < Code1GridHeight[codeOneSize - 1]; row++)
                {
                    for (column = 0; column < Code1GridWidth[codeOneSize - 1]; column++)
                    {
                        dataGrid[row * 2, column * 4] = (char)((target[i] & 0x80) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 1] = (char)((target[i] & 0x40) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 2] = (char)((target[i] & 0x20) > 0 ? '1' : '0');
                        dataGrid[row * 2, (column * 4) + 3] = (char)((target[i] & 0x10) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, column * 4] = (char)((target[i] & 0x08) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 1] = (char)((target[i] & 0x04) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 2] = (char)((target[i] & 0x02) > 0 ? '1' : '0');
                        dataGrid[(row * 2) + 1, (column * 4) + 3] = (char)((target[i] & 0x01) > 0 ? '1' : '0');
                        i++;
                    }
                }

                symbolRows = Code1Height[codeOneSize - 1];
                symbolWidth = Code1Width[codeOneSize - 1];
            }

            symbolGrid = new byte[symbolRows * symbolWidth];
            switch (codeOneSize)
            {
                case 1: // Version A.
                    CentralFinder(symbolGrid, symbolWidth, 6, 3, 1);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 6, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 12, 5, false);
                    SetModule(symbolGrid, symbolWidth, 5, 12, 1);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 15);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 5, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 5, 12, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 5, 0, 5, 12, 6, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 5, 12, 5, 4, 6, 2);
                    break;

                case 2: // Version B.
                    CentralFinder(symbolGrid, symbolWidth, 8, 4, 1);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 8, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 16, 7, false);
                    SetModule(symbolGrid, symbolWidth, 7, 16, 1);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 21);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 7, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 7, 16, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 7, 0, 7, 16, 8, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 7, 16, 7, 4, 8, 2);
                    break;

                case 3: // Version C.
                    CentralFinder(symbolGrid, symbolWidth, 11, 4, 2);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 11, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 13, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 10, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 10, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 27);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 10, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 10, 20, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 24, 10, 4, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 10, 0, 10, 4, 8, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 10, 4, 10, 20, 8, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 10, 24, 10, 4, 8, 4);
                    break;

                case 4: // Version D.
                    CentralFinder(symbolGrid, symbolWidth, 16, 5, 1);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 16, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 20, 16, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 36, 16, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 15, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 20, 15, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 36, 15, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 12);
                    Spigot(symbolGrid, symbolWidth, 27);
                    Spigot(symbolGrid, symbolWidth, 39);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 15, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 15, 14, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 18, 15, 14, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 32, 15, 4, 0, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 15, 0, 15, 4, 10, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 15, 4, 15, 14, 10, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 15, 18, 15, 14, 10, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 15, 32, 15, 4, 10, 6);
                    break;

                case 5: // Version E.
                    CentralFinder(symbolGrid, symbolWidth, 22, 5, 2);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 22, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 24, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 22, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 21, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 21, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 21, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 12);
                    Spigot(symbolGrid, symbolWidth, 39);
                    Spigot(symbolGrid, symbolWidth, 51);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 21, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 21, 20, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 24, 21, 20, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 44, 21, 4, 0, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 21, 0, 21, 4, 10, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 21, 4, 21, 20, 10, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 21, 24, 21, 20, 10, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 21, 44, 21, 4, 10, 6);
                    break;

                case 6: // Version F.
                    CentralFinder(symbolGrid, symbolWidth, 31, 5, 3);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 31, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 35, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 31, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 70, 35, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 4, 30, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 30, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 30, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 70, 30, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 12);
                    Spigot(symbolGrid, symbolWidth, 24);
                    Spigot(symbolGrid, symbolWidth, 45);
                    Spigot(symbolGrid, symbolWidth, 57);
                    Spigot(symbolGrid, symbolWidth, 69);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 30, 4, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 4, 30, 20, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 24, 30, 20, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 44, 30, 20, 0, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 64, 30, 4, 0, 8);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 30, 0, 30, 4, 10, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 30, 4, 30, 20, 10, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 30, 24, 30, 20, 10, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 30, 44, 30, 20, 10, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 30, 64, 30, 4, 10, 8);
                    break;

                case 7: // Version G.
                    CentralFinder(symbolGrid, symbolWidth, 47, 6, 2);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 6, 47, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 27, 49, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 47, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 69, 49, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 90, 47, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 6, 46, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 27, 46, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 48, 46, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 69, 46, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 90, 46, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 12);
                    Spigot(symbolGrid, symbolWidth, 24);
                    Spigot(symbolGrid, symbolWidth, 36);
                    Spigot(symbolGrid, symbolWidth, 67);
                    Spigot(symbolGrid, symbolWidth, 79);
                    Spigot(symbolGrid, symbolWidth, 91);
                    Spigot(symbolGrid, symbolWidth, 103);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 46, 6, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 6, 46, 19, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 25, 46, 19, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 44, 46, 19, 0, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 63, 46, 19, 0, 8);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 82, 46, 6, 0, 10);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 0, 46, 6, 12, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 6, 46, 19, 12, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 25, 46, 19, 12, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 44, 46, 19, 12, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 63, 46, 19, 12, 8);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 46, 82, 46, 6, 12, 10);
                    break;

                case 8: // Version H.
                    CentralFinder(symbolGrid, symbolWidth, 69, 6, 3);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 6, 69, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 73, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 46, 69, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 66, 73, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 86, 69, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 106, 73, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 126, 69, true);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 6, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 26, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 46, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 66, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 86, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 106, 68, false);
                    Verticle(symbolGrid, symbolRows, symbolWidth, 126, 68, false);
                    Spigot(symbolGrid, symbolWidth, 0);
                    Spigot(symbolGrid, symbolWidth, 12);
                    Spigot(symbolGrid, symbolWidth, 24);
                    Spigot(symbolGrid, symbolWidth, 36);
                    Spigot(symbolGrid, symbolWidth, 48);
                    Spigot(symbolGrid, symbolWidth, 60);
                    Spigot(symbolGrid, symbolWidth, 87);
                    Spigot(symbolGrid, symbolWidth, 99);
                    Spigot(symbolGrid, symbolWidth, 111);
                    Spigot(symbolGrid, symbolWidth, 123);
                    Spigot(symbolGrid, symbolWidth, 135);
                    Spigot(symbolGrid, symbolWidth, 147);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 68, 6, 0, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 6, 68, 18, 0, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 24, 68, 18, 0, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 42, 68, 18, 0, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 60, 68, 18, 0, 8);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 78, 68, 18, 0, 10);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 96, 68, 18, 0, 12);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 114, 68, 6, 0, 14);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 0, 68, 6, 12, 0);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 6, 68, 18, 12, 2);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 24, 68, 18, 12, 4);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 42, 68, 18, 12, 6);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 60, 68, 18, 12, 8);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 78, 68, 18, 12, 10);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 96, 68, 18, 12, 12);
                    BlockCopy(symbolGrid, symbolWidth, dataGrid, 68, 114, 68, 6, 12, 14);
                    break;

                case 9: // Version S.
                    Horizontal(symbolGrid, symbolWidth, 5, true);
                    Horizontal(symbolGrid, symbolWidth, 7, true);
                    SetModule(symbolGrid, symbolWidth, 6, 0, 1);
                    SetModule(symbolGrid, symbolWidth, 6, symbolWidth - 1, 1);
                    SetModule(symbolGrid, symbolWidth, 7, 1, 0);
                    SetModule(symbolGrid, symbolWidth, 7, symbolWidth - 2, 0);
                    switch (subVersion)
                    {
                        case 1: // Version S-10.
                            SetModule(symbolGrid, symbolWidth, 0, 5, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 4, 5, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 5, 4, 5, 0, 1);
                            break;

                        case 2: // Version S-20.
                            SetModule(symbolGrid, symbolWidth, 0, 10, 1);
                            SetModule(symbolGrid, symbolWidth, 4, 10, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 4, 10, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 10, 4, 10, 0, 1);
                            break;

                        case 3: // Version S-30.
                            SetModule(symbolGrid, symbolWidth, 0, 15, 1);
                            SetModule(symbolGrid, symbolWidth, 4, 15, 1);
                            SetModule(symbolGrid, symbolWidth, 6, 15, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 4, 15, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 15, 4, 15, 0, 1);
                            break;
                    }
                    break;

                case 10: // Version T.
                    Horizontal(symbolGrid, symbolWidth, 11, true);
                    Horizontal(symbolGrid, symbolWidth, 13, true);
                    Horizontal(symbolGrid, symbolWidth, 15, true);
                    SetModule(symbolGrid, symbolWidth, 12, 0, 1);
                    SetModule(symbolGrid, symbolWidth, 12, symbolWidth - 1, 1);
                    SetModule(symbolGrid, symbolWidth, 14, 0, 1);
                    SetModule(symbolGrid, symbolWidth, 14, symbolWidth - 1, 1);
                    SetModule(symbolGrid, symbolWidth, 13, 1, 0);
                    SetModule(symbolGrid, symbolWidth, 13, symbolWidth - 2, 0);
                    SetModule(symbolGrid, symbolWidth, 15, 1, 0);
                    SetModule(symbolGrid, symbolWidth, 15, symbolWidth - 2, 0);
                    switch (subVersion)
                    {
                        case 1: // Version T-16.
                            SetModule(symbolGrid, symbolWidth, 0, 8, 1);
                            SetModule(symbolGrid, symbolWidth, 10, 8, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 10, 8, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 8, 10, 8, 0, 1);
                            break;

                        case 2: // Version T-32.
                            SetModule(symbolGrid, symbolWidth, 0, 16, 1);
                            SetModule(symbolGrid, symbolWidth, 10, 16, 1);
                            SetModule(symbolGrid, symbolWidth, 12, 16, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 10, 16, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 16, 10, 16, 0, 1);
                            break;

                        case 3: // Verion T-48.
                            SetModule(symbolGrid, symbolWidth, 0, 24, 1);
                            SetModule(symbolGrid, symbolWidth, 10, 24, 1);
                            SetModule(symbolGrid, symbolWidth, 12, 24, 1);
                            SetModule(symbolGrid, symbolWidth, 14, 24, 1);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 0, 10, 24, 0, 0);
                            BlockCopy(symbolGrid, symbolWidth, dataGrid, 0, 24, 10, 24, 0, 1);
                            break;
                    }
                    break;
            }

            // Encode the data.
            byte[] rowData;
            for (int y = 0; y < symbolRows; y++)
            {
                rowData = new byte[symbolWidth];
                for (int x = 0; x < symbolWidth; x++)
                {
                    rowData[x] = symbolGrid[(y * symbolWidth) + x];
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private int C1Encode(char[] source, int length, uint[] target, ref int tp, ref int lastMode)
        {
            int currentMode, nextMode;
            int sourceIndex = 0;
            int[] cteBuffer = new int[6];   // Buffer for holding C40 / TEXT / EDI triplets.
            int cteIndex = 0;               // Characters in the CTE buffer.
            BitVector decimalBinary = new BitVector();
            int dbSize = 0;                 // Bits in the decimalBinary vector.
            int byteStart = 0;
            int eciLength = 7;

            length = eci > 0 && !isGS1 ? length + eciLength : length;
            int[] numberDigits = new int[length];

            // Step A.
            currentMode = ASCII;
            nextMode = ASCII;

            if (isGS1 && tp == 0)
            {
                SetNumberDigits(source, length, numberDigits);
                if (length >= 15 && numberDigits[0] >= 15)
                {
                    target[tp++] = 236; // FNC1 and change to Decimal.
                    nextMode = DECIMAL;
                }

                else if (length >= 7 && numberDigits[0] == length)
                {
                    target[tp++] = 236; // FNC1 and change to Decimal.
                    nextMode = DECIMAL;
                }

                else
                {
                    target[tp++] = 232; //FNC1.
                }
            }

            else
            {
                if (eci > 0)
                {
                    if (tp == 0)
                    {
                        target[tp++] = 129;         // Pad.
                        target[tp++] = '\\' + 1;    // Escape char.
                    }

                    if (eci > 0)
                    {
                        string eciEsc = string.Format("\\{0:000000}", eci);
                        source = ArrayHelper.Insert(source, 0, eciEsc);
                        length = source.Length;
                    }
                }

                SetNumberDigits(source, length, numberDigits);
            }
 
            do
            {
                lastMode = currentMode;
                if (currentMode != nextMode)
                {
                    // Change mode.
                    switch (nextMode)
                    {
                        case C40:
                            target[tp++] = 230;
                            break;

                        case TEXT:
                            target[tp++] = 239;
                            break;

                        case EDIFACT:
                            target[tp++] = 238;
                            break;

                        case BYTE:
                            target[tp++] = 231;
                            byteStart = tp;
                            target[tp++] = 0;   // Byte count holder (may be expanded to 2 codewords).
                            break;
                    }

                    currentMode = nextMode;
                }

                if (currentMode == ASCII)
                {
                    // Step B - ASCII encodation.
                    nextMode = ASCII;
                    if ((length - sourceIndex) >= 21 && numberDigits[sourceIndex] >= 21)
                    {
                        // Step B1.
                        nextMode = DECIMAL;
                        decimalBinary.AppendBits(15, 4); // "1111"
                        dbSize = decimalBinary.SizeInBits;
                    }

                    else if (length - sourceIndex >= 13 && numberDigits[sourceIndex] == (length - sourceIndex))
                    {
                        // Step B2.
                        nextMode = DECIMAL;
                        decimalBinary.AppendBits(15, 4); // "1111"
                        dbSize = decimalBinary.SizeInBits;
                    }

                    if (nextMode == ASCII)
                    {
                        // Step B3.
                        if (Common.IsTwoDigits(source, length, sourceIndex))
                        {
                            target[tp++] = (uint)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                            sourceIndex += 2;
                        }

                        else
                        {
                            if (isGS1 && (source[sourceIndex] == '\x1d'))
                            {
                                if ((length - (sourceIndex + 1)) >= 15 && numberDigits[sourceIndex + 1] >= 15)
                                {
                                    // Step B4.
                                    target[tp++] = 236; // FNC1 and change to Decimal Mode.
                                    sourceIndex++;
                                    nextMode = DECIMAL;
                                }

                                else if (length - (sourceIndex + 1) >= 7 && numberDigits[sourceIndex + 1] == length - (sourceIndex + 1))
                                {
                                    // Step B5.
                                    target[tp++] = 236; // FNC1 and change to Decimal Mode.
                                    sourceIndex++;
                                    nextMode = DECIMAL;
                                }
                            }

                            if (nextMode == ASCII)
                            {
                                // Step B6.
                                nextMode = LookAheadTest(source, length, sourceIndex, currentMode);
                                if (nextMode == lastMode)
                                {
                                    nextMode = ASCII;
                                }

                                if (nextMode == ASCII)
                                {
                                    if ((source[sourceIndex] & 0x80) > 1)
                                    {
                                        // Step B7.
                                        target[tp++] = 235; // FNC4.
                                        target[tp++] = (uint)(source[sourceIndex] - 128) + 1;
                                    }

                                    else if (isGS1 && (source[sourceIndex] == '\x1d'))
                                    {
                                        // Step B8.
                                        target[tp++] = 232; // FNC1.
                                    }

                                    else
                                    {
                                        target[tp++] = (uint)source[sourceIndex] + 1;
                                    }

                                    sourceIndex++;
                                }
                            }
                        }
                    }
                }

                else if (currentMode == C40 || currentMode == TEXT)
                {
                    // Step C/D - C40/TEXT encodation.
                    nextMode = currentMode;
                    if (cteIndex == 0)
                    {
                        // Step C/D1.
                        if ((length - sourceIndex) >= 12 && numberDigits[sourceIndex] >= 12)
                        {
                            // Step C/D1a.
                            nextMode = ASCII;
                        }

                        else if ((length - sourceIndex) >= 8 && numberDigits[sourceIndex] == (length - sourceIndex))
                        {
                            // Step C/D1b
                            nextMode = ASCII;
                        }

                        else
                        {
                            nextMode = LookAheadTest(source, length, sourceIndex, currentMode);
                        }
                    }

                    if (nextMode != currentMode)
                    {
                        // Step C/D1c.
                        target[tp++] = 255; // Unlatch.
                    }

                    else
                    {
                        // Step C/D2.
                        int[] ctShift, ctValue;

                        if (currentMode == C40)
                        {
                            ctShift = C40Shift;
                            ctValue = C40Value;
                        }

                        else
                        {
                            ctShift = TextShift;
                            ctValue = TextValue;
                        }

                        if ((source[sourceIndex] & 0x80) > 1)
                        {
                            cteBuffer[cteIndex++] = 1;    // Shift 2.
                            cteBuffer[cteIndex++] = 30;   // FNC4 (Upper Shift).
                            if (ctShift[source[sourceIndex] - 128] > 0)
                            {
                                cteBuffer[cteIndex++] = ctShift[source[sourceIndex] - 128] - 1;
                            }

                            cteBuffer[cteIndex++] = ctValue[source[sourceIndex] - 128];
                        }

                        else if (isGS1 && source[sourceIndex] == '\x1d')
                        {
                            cteBuffer[cteIndex++] = 1;  // Shift 2.
                            cteBuffer[cteIndex++] = 27; // FNC1.
                        }

                        else
                        {
                            if (ctShift[source[sourceIndex]] > 0)
                            {
                                cteBuffer[cteIndex++] = ctShift[source[sourceIndex]] - 1;
                            }

                            cteBuffer[cteIndex++] = ctValue[source[sourceIndex]];
                        }

                        if (cteIndex >= 3)
                        {
                            cteIndex = CTEBufferTransfer(cteBuffer, cteIndex, target, ref tp);
                        }

                        sourceIndex++;
                    }

                }

                else if (currentMode == EDIFACT)
                {
                    // Step E - EDI Encodation.
                    nextMode = EDIFACT;
                    if (cteIndex == 0)
                    {
                        // Step E1.
                        if ((length - sourceIndex) >= 12 && numberDigits[sourceIndex] >= 12)
                        {
                            // Step E1a.
                            nextMode = ASCII;
                        }

                        else if ((length - sourceIndex) >= 8 && numberDigits[sourceIndex] == (length - sourceIndex))
                        {
                            // Step E1b.
                            nextMode = ASCII;
                        }

                        else if ((length - sourceIndex) < 3 || !IsEDI(source[sourceIndex]) || !IsEDI(source[sourceIndex + 1])
                              || !IsEDI(source[sourceIndex + 2]))
                        {
                            // Step E1c.
                            // This ensures ASCII switch if don't have EDI triplet, so cteIndex will be zero on loop exit.
                            nextMode = ASCII;
                        }
                    }

                    if (nextMode != EDIFACT)
                    {
                        if (IsLastSingleASCII(source, length, sourceIndex) && CodewordsRemaining(tp) == 1)
                        {
                            // No unlatch needed if data fits as ASCII in last data codeword.
                        }

                        else
                        {
                            target[tp++] = 255; // Unlatch.
                        }
                    }

                    else
                    {
                        // Step E2.
                        string ediNonAlphanumChars = "\r*> ";

                        if (char.IsDigit(source[sourceIndex]))
                        {
                            cteBuffer[cteIndex++] = source[sourceIndex] - '0' + 4;
                        }

                        else if (char.IsUpper(source[sourceIndex]))
                        {
                            cteBuffer[cteIndex++] = source[sourceIndex] - 'A' + 14;
                        }

                        else
                        {
                            cteBuffer[cteIndex++] = ediNonAlphanumChars.IndexOf(source[sourceIndex]);
                        }

                        if (cteIndex >= 3)
                        {
                            cteIndex = CTEBufferTransfer(cteBuffer, cteIndex, target, ref tp);
                        }

                        sourceIndex++;
                    }
                }

                else if (currentMode == DECIMAL)
                {
                    // Step F - Decimal encodation.
                    nextMode = DECIMAL;

                    if (length - sourceIndex < 3)
                    {
                        // Step F1.
                        int bitsLeft = 8 - dbSize;
                        bool can_ascii = bitsLeft == 8 && IsLastSingleASCII(source, length, sourceIndex);

                        if (CodewordsRemaining(tp) == 1 && (can_ascii || (numberDigits[sourceIndex] == 1 && bitsLeft >= 4)))
                        {
                            if (can_ascii)
                            {
                                // Encode last character or last 2 digits as ASCII.
                                if (Common.IsTwoDigits(source, length, sourceIndex))
                                {
                                    target[tp++] = (uint)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                                    sourceIndex += 2;
                                }

                                else
                                {
                                    target[tp++] = (uint)source[sourceIndex] + 1;
                                    sourceIndex++;
                                }
                            }

                            else
                            {
                                // Encode last digit in 4 bits.
                                decimalBinary.AppendBits((int)source[sourceIndex] + 1, 4);
                                dbSize = decimalBinary.SizeInBits;
                                sourceIndex++;
                                if (bitsLeft == 6)
                                {
                                    decimalBinary.AppendBits(1, 2);
                                    dbSize = decimalBinary.SizeInBits;
                                }

                                dbSize = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
                            }
                        }

                        else
                        {
                            dbSize = DecimalUnlatch(decimalBinary, dbSize, target, ref tp, numberDigits[sourceIndex], source, ref sourceIndex);
                            currentMode = ASCII; // Note need to set current_mode also in case exit loop.
                        }

                        nextMode = ASCII;

                    }

                    else
                    {
                        if (numberDigits[sourceIndex] < 3)
                        {
                            // Step F2.
                            dbSize = DecimalUnlatch(decimalBinary, dbSize, target, ref tp, numberDigits[sourceIndex], source, ref sourceIndex);
                            currentMode = nextMode = ASCII; /* Note need to set current_mode also in case exit loop */
                        }

                        else
                        {
                            // Step F3.
                            // There are three digits - convert the value to binary.
                            int value = (100 * (int)(source[sourceIndex] - '0')) + (10 * (int)(source[sourceIndex + 1] - '0')) + (int)(source[sourceIndex + 2] - '0') + 1;
                            decimalBinary.AppendBits(value, 10);
                            dbSize = decimalBinary.SizeInBits;
                            if (dbSize >= 8)
                            {
                                dbSize = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
                            }

                            sourceIndex += 3;
                        }
                    }

                }

                else if (currentMode == BYTE)
                {
                    nextMode = BYTE;

                    if (isGS1 && source[sourceIndex] == '\x1d')
                    {
                        nextMode = ASCII;
                    }

                    else
                    {
                        if (source[sourceIndex] <= 127)
                        {
                            nextMode = LookAheadTest(source, length, sourceIndex, currentMode);
                        }
                    }

                    if (nextMode != BYTE)
                    {
                        // Update byte field length.
                        int byte_count = tp - (byteStart + 1);
                        if (byte_count <= 249)
                        {
                            target[byteStart] = (uint)byte_count;
                        }

                        else
                        {
                            // Insert extra codeword.
                            Array.Copy(target, byteStart + 1, target, byteStart + 2, byte_count);
                            target[byteStart] = (uint)(249 + (byte_count / 250));
                            target[byteStart + 1] = (uint)(byte_count % 250);
                            tp++;
                        }
                    }

                    else
                    {
                        target[tp++] = source[sourceIndex];
                        sourceIndex++;
                    }
                }

                if (tp > C1_MAX_CWS)
                {
                    throw new InvalidDataLengthException("Code One: Input data to long for symbol.");
                }

            } while (sourceIndex < length);

            // Empty buffers (note cteBuffer will be empty if currentMode EDIFACT).
            if (currentMode == C40 || currentMode == TEXT)
            {
                if (cteIndex >= 1)
                {
                    int cwdsRemaining = CodewordsRemaining(tp);

                    // Note doing strict interpretation of spec here (same as BWIPP), as now also done in Data Matrix case */
                    if (cwdsRemaining == 1 && cteIndex == 1 && IsC40Text(currentMode, source[sourceIndex - 1]))
                    {
                        // 2.2.2.2 "...except when a single symbol character is left at the end before the first
                        // error correction character. This single character is encoded in the ASCII code set."
                        target[tp++] = (uint)source[sourceIndex - 1] + 1; // As ASCII.
                        cteIndex = 0;
                    }

                    else if (cwdsRemaining == 2 && cteIndex == 2)
                    {
                        // 2.2.2.2 "Two characters may be encoded in C40 mode in the last two data symbol characters of the
                        //  symbol as two C40 values followed by one of the C40 shift characters."
                        cteBuffer[cteIndex++] = 0; // Shift 0.
                        cteIndex = CTEBufferTransfer(cteBuffer, cteIndex, target, ref tp);
                    }

                    if (cteIndex >= 1)
                    {
                        int count, totalCount = 0;
                        // Backtrack to last complete triplet (same technique as BWIPP).
                        while (sourceIndex > 0 && (cteIndex % 3) > 0)
                        {
                            sourceIndex--;
                            count = C40TextCount(currentMode, source[sourceIndex]);
                            totalCount += count;
                            cteIndex -= count;
                        }

                        tp -= (totalCount / 3) * 2;

                        target[tp++] = 255; // Unlatch.
                        for (; sourceIndex < length; sourceIndex++)
                        {
                            if (Common.IsTwoDigits(source, length, sourceIndex))
                            {
                                target[tp++] = (uint)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                                sourceIndex++;
                            }

                            else if ((source[sourceIndex] & 0x80) > 1)
                            {
                                target[tp++] = 235; // FNC4 (Upper Shift).
                                target[tp++] = (uint)source[sourceIndex] - 128 + 1;
                            }

                            else if (isGS1 && source[sourceIndex] == '\x1d')
                            {
                                target[tp++] = 232; // FNC1.
                            }

                            else
                            {
                                target[tp++] = (uint)source[sourceIndex] + 1;
                            }
                        }

                        currentMode = ASCII;
                    }
                }
            }

            else if (currentMode == DECIMAL)
            {
                int bitsLeft;

                // Finish Decimal mode and go back to ASCII unless only one codeword remaining.
                if (CodewordsRemaining(tp) > 1)
                {
                    decimalBinary.AppendBits(63, 6); // Unlatch.
                    dbSize = decimalBinary.SizeInBits;
                }

                if (dbSize >= 8)
                {
                    dbSize = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
                }

                bitsLeft = (8 - dbSize) & 0x07;
                if (bitsLeft > 0)
                {
                    if ((bitsLeft == 4) || (bitsLeft == 6))
                    {
                        decimalBinary.AppendBits(15, 4);
                        dbSize = decimalBinary.SizeInBits;
                    }

                    if (bitsLeft == 2 || bitsLeft == 6)
                    {
                        decimalBinary.AppendBits(1, 2);
                        dbSize = decimalBinary.SizeInBits;
                    }

                    _ = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
                }

                currentMode = ASCII;
            }

            else if (currentMode == BYTE)
            {
                // Update byte field length unless no codewords remaining.
                if (CodewordsRemaining(tp) > 0)
                {
                    int byte_count = tp - (byteStart + 1);
                    if (byte_count <= 249)
                    {
                        target[byteStart] = (uint)byte_count;
                    }
                    else
                    {
                        // Insert extra codeword.
                        Array.Copy(target, byteStart + 1, target, byteStart + 2, byte_count);
                        target[byteStart] = (uint)(249 + (byte_count / 250));
                        target[byteStart + 1] = (uint)(byte_count % 250);
                        tp++;
                    }
                }
            }

            // Re-check input length of data.
            if (tp > C1_MAX_CWS)
            {
                throw new InvalidDataLengthException("Code One: Input data to long for symbol.");
            }

            lastMode = currentMode;
            return tp;
        }

        // Test for special EDIFACT characters.
        // Whether Step Q4bi applies, i.e. if one of the 3 EDI terminator/separator chars appears before a non-EDI char.
        private bool Q4bi(char[] source, int length, int position)
        {
            for (int i = position; i < length && IsEDI(source[i]); i++)
            {
                if (source[i] == 13 || source[i] == '*' || source[i] == '>')
                {
                    return true;
                }
            }

            return false;
        }

        // Whether can fit last character or characters in a single ASCII codeword.
        private bool IsLastSingleASCII(char[] source, int length, int sourceIndex)
        {
            if (length - sourceIndex == 1 && source[sourceIndex] <= 127)
            {
                return true;
            }

            if (length - sourceIndex == 2 && Common.IsTwoDigits(source, length, sourceIndex))
            {
                return true;
            }

            return false;
        }

        // Initialize number of digits array (taken from BWIPP).
        private void SetNumberDigits(char[] source, int length, int[] numberDigits)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                if (char.IsDigit(source[i]))
                {
                    numberDigits[i] = numberDigits[i + 1] + 1;
                }
            }
        }

        // Copy C40/TEXT/EDI triplets from buffer to `target`. Returns elements left in buffer(< 3).
        private int CTEBufferTransfer(int[] cteBuffer, int cteIndex, uint[] target, ref int tp)
        {
            int cte_i, cte_e;

            cte_e = (cteIndex / 3) * 3;

            for (cte_i = 0; cte_i < cte_e; cte_i += 3)
            {
                int iv = (1600 * cteBuffer[cte_i]) + (40 * cteBuffer[cte_i + 1]) + (cteBuffer[cte_i + 2]) + 1;
                target[tp++] = (uint)(iv >> 8);
                target[tp++] = (uint)(iv & 0xff);
            }

            cteIndex -= cte_e;

            if (cteIndex > 0)
            {
                Array.Copy(cteBuffer, cte_e, cteBuffer, 0, cteIndex);
            }

            return cteIndex;
        }

        // Copy DECIMAL bytes to "target". Returns bits left in buffer (< 8).
        private int DecimalBinaryTransfer(BitVector decimalBinary, int dbSize, uint[] target, ref int tp)
        {
            int bIndex, bEnd;

            // Transfer full bytes to target.
            bEnd = dbSize & 0xf8;

            for (bIndex = 0; bIndex < bEnd; bIndex += 8)
            {
                uint value = 0;
                for (int p = 0; p < 8; p++)
                {
                    value <<= 1;
                    if (decimalBinary[bIndex + p] == 1)
                    {
                        value += 1;
                    }
                }

                target[tp++] = value;
            }

            dbSize &= 0x07; // Bits remaining.
            decimalBinary.RemoveBits(0, bEnd);
            return dbSize;
        }

        // Unlatch to ASCII from DECIMAL mode using 6 ones flag. DECIMAL binary buffer will be empty.
        private int DecimalUnlatch(BitVector decimalBinary, int dbSize, uint[] target, ref int tp, int decimal_count, char[] source, ref int sp)
        {
            int bitsLeft;

            decimalBinary.AppendBits(63, 6);    // Unlatch.
            dbSize = decimalBinary.SizeInBits;
            if (dbSize >= 8)
            {
                dbSize = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
            }

            bitsLeft = (8 - dbSize) & 0x07;
            if (decimal_count >= 1 && bitsLeft >= 4)
            {
                decimalBinary.AppendBits((int)(source[sp] + 1), 4);
                dbSize = decimalBinary.SizeInBits;
                sp++;
                if (bitsLeft == 6)
                {
                    decimalBinary.AppendBits(1, 2);
                    dbSize = decimalBinary.SizeInBits;
                }

                _ = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
            }

            else if (bitsLeft > 0)
            {
                if (bitsLeft >= 4)
                {
                    decimalBinary.AppendBits(15, 4);
                    dbSize = decimalBinary.SizeInBits;
                }

                if (bitsLeft == 2 || bitsLeft == 6)
                {
                    decimalBinary.AppendBits(1, 2);
                    dbSize = decimalBinary.SizeInBits;
                }

                _ = DecimalBinaryTransfer(decimalBinary, dbSize, target, ref tp);
            }

            return 0;
        }


        // Number of codewords remaining in a particular version (may be negative).
        private int CodewordsRemaining(int tp)
        {
            if (optionSymbolSize == 10)
            {
                // Version T.
                if (tp > 24)
                {
                    return 38 - tp;
                }

                if (tp > 10)
                {
                    return 24 - tp;
                }

                return 10 - tp;
            }

            // Versions A to H.
            for (int i = 6; i >= 0; i--)
            {
                if (tp > Code1DataLength[i])
                {
                    return Code1DataLength[i + 1] - tp;
                }
            }

            return Code1DataLength[0] - tp;
        }

        // Number of C40/TEXT elements needed to encode "input".
        private int C40TextCount(int currentMode, char input)
        {

            if (isGS1 && input == '\x1d')
            {
                return 2;
            }

            int count = 1;
            if ((input & 0x80) > 1)
            {
                count += 2;
                input -= (char)128;
            }

            if ((currentMode == C40 && C40Shift[input] > 1) || (currentMode == TEXT && TextShift[input] > 1))
            {
                count += 1;
            }

            return count;
        }

        #region Look Ahead constants.

        // Character counts are multiplied by this, so as to be whole integer divisible by 2 and 3.
        const int C1_MULT = 6;
        const int C1_MULT_1_DIV_2 = 3;
        const int C1_MULT_2_DIV_3 = 4;
        const int C1_MULT_1 = 6;
        const int C1_MULT_4_DIV_3 = 8;
        const int C1_MULT_2 = 12;
        const int C1_MULT_8_DIV_3 = 16;
        const int C1_MULT_3 = 18;
        const int C1_MULT_10_DIV_3 = 20;
        const int C1_MULT_13_DIV_3 = 26;
        const int C1_MULT_MINUS_1 = 5;
        private int C1_MULT_CEIL(int n) { return (n + C1_MULT_MINUS_1) / C1_MULT * C1_MULT; }

        #endregion

        // AIM USS Code One Annex D Steps J-R.
        private int LookAheadTest(char[] source, int length, int position, int currentMode)
        {
            int asciiCount, c40Count, textCount, ediCount, byteCount;
            int asciiRounded, c40Rounded, textRounded, ediRounded, byteRounded;

            // Step J1.
            if (currentMode == ASCII)
            {
                asciiCount = 0;
                c40Count = C1_MULT_1;
                textCount = C1_MULT_1;
                ediCount = C1_MULT_1;
                byteCount = C1_MULT_2;
            }

            else
            {
                asciiCount = C1_MULT_1;
                c40Count = C1_MULT_2;
                textCount = C1_MULT_2;
                ediCount = C1_MULT_2;
                byteCount = C1_MULT_3;
            }

            switch (currentMode)
            {
                case C40:
                    c40Count = 0; // Step J2.
                    break;

                case TEXT:
                    textCount = 0; // Step J3.
                    break;

                case EDIFACT:
                    ediCount = 0;
                    break;

                case BYTE:
                    byteCount = 0; // Step J4.
                    break;
            }

            for (int sp = position; sp < length; sp++)
            {
                char c = source[sp];
                bool isExtended = (c & 0x80) > 1;

                // Step L.
                if (char.IsDigit(c))
                {
                    asciiCount += C1_MULT_1_DIV_2; // Step L1.
                }

                else
                {
                    if (isExtended)
                    {
                        asciiCount = (int)Math.Ceiling((decimal)asciiCount) + C1_MULT_2; // Step L2.
                    }

                    else
                    {
                        asciiCount = (int)Math.Ceiling((decimal)asciiCount) + C1_MULT_1; // Step L3.
                    }
                }

                // Step M.
                if (IsC40(c))
                {
                    c40Count += C1_MULT_2_DIV_3; // Step M1.
                }

                else if (isExtended)
                {
                    c40Count += C1_MULT_8_DIV_3; // Step M2.
                }

                else
                {
                    c40Count += C1_MULT_4_DIV_3; // Step M3.
                }

                // Step N.
                if (IsText(c))
                {
                    textCount += C1_MULT_2_DIV_3; // Step N1.
                }

                else if (isExtended)
                {
                    textCount += C1_MULT_8_DIV_3; // Step N2.
                }

                else
                {
                    textCount += C1_MULT_4_DIV_3; // Step N3.
                }

                // Step O.
                if (IsEDI(c))
                {
                    ediCount += C1_MULT_2_DIV_3; // Step O1.
                }

                else if (isExtended)
                {
                    ediCount += C1_MULT_13_DIV_3; // Step O2.
                }

                else
                {
                    ediCount += C1_MULT_10_DIV_3; // Step O3.
                }

                // Step P.
                if (isGS1 && c == '\x1d')
                {
                    byteCount += C1_MULT_3; // Step P1.
                }

                else
                {
                    byteCount += C1_MULT_1; // Step P2.
                }

                // If at least 4 characters processed.
                // NOTE: different than spec, where it's at least 3, but that ends up suppressing C40/TEXT/EDI.
                // BWIPP also uses 4 (cf very similar Data Matrix ISO/IEC 16022:2006 Annex P algorithm).
                if (sp >= position + 3)
                {
                    //Step Q.
                    asciiRounded = C1_MULT_CEIL(asciiCount);
                    c40Rounded = C1_MULT_CEIL(c40Count);
                    textRounded = C1_MULT_CEIL(textCount);
                    ediRounded = C1_MULT_CEIL(ediCount);
                    byteRounded = C1_MULT_CEIL(byteCount);

                    int count1 = byteCount + C1_MULT_1;
                    if (count1 <= asciiRounded && count1 <= c40Rounded && count1 <= textRounded && count1 <= ediRounded)
                    {
                        return BYTE; // Step Q1.
                    }

                    count1 = asciiCount + C1_MULT_1;
                    if (count1 <= c40Rounded && count1 <= textRounded && count1 <= ediRounded && count1 <= byteRounded)
                    {
                        return ASCII; // Step Q2.
                    }

                    count1 = textRounded + C1_MULT_1;
                    if (count1 <= asciiRounded && count1 <= c40Rounded && count1 <= ediRounded && count1 <= byteRounded)
                    {
                        return TEXT; // Step Q3.
                    }

                    count1 = c40Rounded + C1_MULT_1;
                    if (count1 <= asciiRounded && count1 <= textRounded)
                    {
                        // Step Q4.
                        if (c40Rounded < ediRounded)
                        {
                            return C40; /* Step Q4a */
                        }

                        if (c40Rounded == ediRounded)
                        {
                            // Step Q4b.
                            if (Q4bi(source, length, sp + 1))
                            {
                                return EDIFACT; // Step Q4bi.
                            }

                            return C40; // Step Q4bii.
                        }
                    }

                    count1 = ediRounded + C1_MULT_1;
                    if (count1 <= asciiRounded && count1 <= c40Rounded && count1 <= textRounded && count1 <= byteRounded)
                    {
                        return EDIFACT; // Step Q5.
                    }
                }
            }

            // Step K.
            asciiRounded = C1_MULT_CEIL(asciiCount);
            c40Rounded = C1_MULT_CEIL(c40Count);
            textRounded = C1_MULT_CEIL(textCount);
            ediRounded = C1_MULT_CEIL(ediCount);
            byteRounded = C1_MULT_CEIL(byteCount);

            if (byteCount <= asciiRounded && byteCount <= c40Rounded && byteCount <= textRounded && byteCount <= ediRounded)
            {
                return BYTE; // Step K1.
            }

            if (asciiCount <= c40Rounded && asciiCount <= textRounded && asciiCount <= ediRounded
                    && asciiCount <= byteRounded)
            {
                return ASCII; // Step K2.
            }

            if (c40Rounded <= textRounded && c40Rounded <= ediRounded)
            {
                return C40; //Step K3.
            }

            if (textRounded <= ediRounded)
            {
                return TEXT; // Step K4.
            }

            return EDIFACT; // Step K5.
        }


        // Set symbol from datagrid.
        private void BlockCopy(byte[] symbolGrid, int symbolWidth, char[,] grid, int startRow, int startColumn, int height, int width, int rowOffset, int columnOffset)
        {
            for (int i = startRow; i < (startRow + height); i++)
            {
                for (int j = startColumn; j < (startColumn + width); j++)
                {
                    if (grid[i, j] == '1')
                    {
                        SetModule(symbolGrid, symbolWidth, i + rowOffset, j + columnOffset, 1);
                    }
                }
            }
        }

        private void Spigot(byte[] symbolGrid, int symbolWidth, int rowNumber)
        {
            for (int i = symbolWidth - 1; i > 0; i--)
            {
                if (IsModuleSet(symbolGrid, symbolWidth, rowNumber, i - 1))
                {
                    SetModule(symbolGrid, symbolWidth, rowNumber, i, 1);
                }
            }
        }

        private void CentralFinder(byte[] symbolGrid, int symbolWidth, int startRow, int rowCount, int fullRows)
        {
            for (int i = 0; i < rowCount; i++)
            {
                if (i < fullRows)
                {
                    Horizontal(symbolGrid, symbolWidth, startRow + (i * 2), true);
                }

                else
                {
                    Horizontal(symbolGrid, symbolWidth, startRow + (i * 2), false);
                    if (i != rowCount - 1)
                    {
                        SetModule(symbolGrid, symbolWidth, startRow + (i * 2) + 1, 1, 1);
                        SetModule(symbolGrid, symbolWidth, startRow + (i * 2) + 1, symbolWidth - 2, 1);
                    }
                }
            }
        }

        private void Horizontal(byte[] symbolGrid, int symbolWidth, int rowNumber, bool full)
        {
            if (full)
            {
                for (int i = 0; i < symbolWidth; i++)
                {
                    SetModule(symbolGrid, symbolWidth, rowNumber, i, 1);
                }
            }

            else
            {
                for (int i = 1; i < symbolWidth - 1; i++)
                {
                    SetModule(symbolGrid, symbolWidth, rowNumber, i, 1);
                }
            }
        }

        private void Verticle(byte[] symbolGrid, int symbolRows, int symbolWidth, int column, int height, bool isTop)
        {

            if (isTop)
            {
                for (int i = 0; i < height; i++)
                {
                    SetModule(symbolGrid, symbolWidth, i, column, 1);
                }
            }

            else
            {
                for (int i = 0; i < height; i++)
                {
                    SetModule(symbolGrid, symbolWidth, symbolRows - i - 1, column, 1);
                }
            }
        }

        // Is basic (non-shifted) C40?
        private bool IsC40(char input)
        {
            return char.IsDigit(input) || char.IsUpper(input) || input == ' ';
        }

        // Is basic (non-shifted) TEXT?
        private bool IsText(char input)
        {
            return char.IsDigit(input) || char.IsLower(input) || input == ' ';
        }

        // Is basic (non-shifted) C40/TEXT?
        private bool IsC40Text(int currentMode, char input)
        {
            return currentMode == C40 ? IsC40(input) : IsText(input);
        }

        // EDI characters are uppercase alphanumerics plus space plus EDI terminator (CR) plus 2 EDI separator chars.
        private bool IsEDI(char input)
        {

            if (IsC40(input))
            {
                return true;
            }

            if (input == 13 || input == '*' || input == '>')
            {
                return true;
            }

            return false;
        }

        private void SetModule(byte[] symbolGrid, int symbolWidth, int row, int column, byte value)
        {
            symbolGrid[(row * symbolWidth) + column] = value;
        }

        // Test if the specified grid module is set.
        private bool IsModuleSet(byte[] symbolGrid, int symbolWidth, int row, int column)
        {
            if (symbolGrid[(row * symbolWidth) + column] == 1)
            {
                return true;
            }

            return false;
        }
    }
}
