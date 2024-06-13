/* DotCodeEncoder.cs - Handles DotCode 2D symbol */

/*
    ZintNetLib - a C# port of libzint.
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
 
    libzint - the open source barcode library
    Copyright (C) 2008-2020 Robin Stuart <rstuart114@gmail.com>

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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZintNet.Encoders
{
    internal class DotCodeEncoder : SymbolEncoder
    {
        #region Tables and Constants
        // DotCode symbol character dot patterns, from Annex C.
        private static int[] dotPatterns = {
            0x155, 0x0ab, 0x0ad, 0x0b5, 0x0d5, 0x156, 0x15a, 0x16a, 0x1aa, 0x0ae,
            0x0b6, 0x0ba, 0x0d6, 0x0da, 0x0ea, 0x12b, 0x12d, 0x135, 0x14b, 0x14d,
            0x153, 0x159, 0x165, 0x169, 0x195, 0x1a5, 0x1a9, 0x057, 0x05b, 0x05d,
            0x06b, 0x06d, 0x075, 0x097, 0x09b, 0x09d, 0x0a7, 0x0b3, 0x0b9, 0x0cb,
            0x0cd, 0x0d3, 0x0d9, 0x0e5, 0x0e9, 0x12e, 0x136, 0x13a, 0x14e, 0x15c,
            0x166, 0x16c, 0x172, 0x174, 0x196, 0x19a, 0x1a6, 0x1ac, 0x1b2, 0x1b4,
            0x1ca, 0x1d2, 0x1d4, 0x05e, 0x06e, 0x076, 0x07a, 0x09e, 0x0bc, 0x0ce,
            0x0dc, 0x0e6, 0x0ec, 0x0f2, 0x0f4, 0x117, 0x11b, 0x11d, 0x127, 0x133,
            0x139, 0x147, 0x163, 0x171, 0x18b, 0x18d, 0x193, 0x199, 0x1a3, 0x1b1,
            0x1c5, 0x1c9, 0x1d1, 0x02f, 0x037, 0x03b, 0x03d, 0x04f, 0x067, 0x073,
            0x079, 0x08f, 0x0c7, 0x0e3, 0x0f1, 0x11e, 0x13c, 0x178, 0x18e, 0x19c,
            0x1b8, 0x1c6, 0x1cc };

        private const int GF = 113;
        private const int PM = 3;
        #endregion

        private int optionSymbolColumns;

        public DotCodeEncoder(Symbology symbology, string barcodeMessage, int optionSymbolColumns, int eci, EncodingMode encodingMode)
        {
            this.symbolId = symbology;
            this.barcodeMessage = barcodeMessage;
            this.optionSymbolColumns = optionSymbolColumns;
            this.eci = eci;
            this.encodingMode = encodingMode;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (encodingMode)
            {
                case EncodingMode.Standard:
                    isGS1 = false;
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    break;

                case EncodingMode.GS1:
                    isGS1 = true;
                    barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                    break;
            }

            DotCode();
            return Symbol;
        }

        private void DotCode()
        {
            int dataLength, eccLength;
            int minimumDots, numberOfDots, minimumArea, paddingDots;
            int height, width;
            bool isFirst;
            int highScore, bestMask;
            bool binaryFinish = false;
            int inputLength = barcodeData.Length;
            int[] maskScore = new int[8];
            List<byte> codewords = new List<byte>();
            byte[] maskedCodewords;

            DotCodeEncode(codewords, inputLength, ref binaryFinish);
            dataLength = codewords.Count;
            eccLength = 3 + (dataLength / 2);
            minimumDots = 9 * (dataLength + eccLength) + 2;
            minimumArea = minimumDots * 2;

            if (optionSymbolColumns == 0)   // Automatic sizing
            {
                // Implement Rule 3 Section 5.2.2
                // Recommended W:H ratio of 3:2
                float h = (float)(Math.Sqrt(minimumArea * 0.666));
                float w = (float)(Math.Sqrt(minimumArea * 1.5));
                height = (int)h;
                width = (int)w;
                if ((width + height) % 2 == 1)
                {
                    if ((width * height) < minimumArea)
                    {
                        width++;
                        height++;
                    }
                }

                else
                {
                    if ((h * width) < (w * height))
                    {
                        width++;
                        if ((width * height) < minimumArea)
                        {
                            width--;
                            height++;
                            if ((width * height) < minimumArea)
                                width += 2;
                        }
                    }

                    else
                    {
                        height++;
                        if ((width * height) < minimumArea)
                        {
                            width++;
                            height--;
                            if ((width * height) < minimumArea)
                                height += 2;
                        }
                    }
                }
            }

            else    // Fixed width.
            {
                width = optionSymbolColumns;
                height = (minimumArea + (width - 1)) / width;
                if (((width + height) % 2) == 0)
                    height++;
            }

            if ((height > 200) || (width > 200))
                throw new InvalidSymbolSizeException("Dot Code: Specified symbol size is too large.");

            numberOfDots = (height * width) / 2;
            BitVector dotStream = new BitVector();
            byte[] dotArray = new byte[width * height];

            // Add pad characters.
            paddingDots = numberOfDots - minimumDots;   // Get the number of free dots available for padding.
            isFirst = true;
            while (paddingDots >= 9)
            {
                if (paddingDots < 18 && ((dataLength % 2) == 0))
                    paddingDots -= 9;

                else if (paddingDots >= 18)
                {
                    if ((dataLength % 2) == 0)
                        paddingDots -= 9;

                    else
                        paddingDots -= 18;
                }

                else
                    break;  // Not enough dots left for padding.

                if (isFirst && binaryFinish)
                    codewords.Add(109);

                else
                    codewords.Add(106);

                dataLength++;
                isFirst = false;
            }

            eccLength = 3 + (dataLength / 2);
            maskedCodewords = new byte[dataLength + 1 + eccLength];
            // Evaluate data mask options.
            for (int i = 0; i < 4; i++)
            {
                ApplyMask(i, dataLength, maskedCodewords, codewords, eccLength);
                BinaryDotStream(maskedCodewords, dotStream);

                // Add pad bits.
                for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
                    dotStream.AppendBit(1);

                FoldDotStream(dotStream, width, height, dotArray);
                maskScore[i] = ScoreArray(dotArray, height, width);
            }

            highScore = maskScore[0];
            bestMask = 0;

            for (int i = 1; i < 4; i++)
            {
                if (maskScore[i] > highScore)
                {
                    highScore = maskScore[i];
                    bestMask = i;
                }
            }

            // Re-evaluate using forced corners if needed.
            if (bestMask <= (height * width) / 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    ApplyMask(i, dataLength, maskedCodewords, codewords, eccLength);
                    BinaryDotStream(maskedCodewords, dotStream);

                    // Add pad bits.
                    for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
                        dotStream.AppendBit(1);

                    FoldDotStream(dotStream, width, height, dotArray);
                    ForceCorners(width, height, dotArray);
                    maskScore[i + 4] = ScoreArray(dotArray, height, width);
                }

                for (int i = 4; i < 8; i++)
                {
                    if (maskScore[i] > highScore)
                    {
                        highScore = maskScore[i];
                        bestMask = i;
                    }
                }
            }

            // Apply best mask.
            ApplyMask(bestMask % 4, dataLength, maskedCodewords, codewords, eccLength);
            BinaryDotStream(maskedCodewords, dotStream);

            // Add pad bits.
            for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
                dotStream.AppendBit(1);

            FoldDotStream(dotStream, width, height, dotArray);
            if (bestMask >= 4)
                ForceCorners(width, height, dotArray);

            // Build the symbol.
            byte[] rowData;
            for (int h = 0; h < height; h++)
            {
                rowData = new byte[width];
                for (int w = 0; w < width; w++)
                {
                    if (dotArray[(h * width) + w] == 1)
                        rowData[w] = 1;
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private void DotCodeEncode(List<byte> codewords, int length, ref bool binaryFinish)
        {
            int inputPosition = 0;
            char encodingMode = 'C';
            int insideMacro = 0;
            bool done = false;
            int binaryBufferSize = 0;
            ulong binaryBuffer = 0;

            if (!isGS1)
            {
                if(length > 2)
                {
                    if(Char.IsDigit(barcodeData[inputPosition]) && char.IsDigit(barcodeData[inputPosition + 1]))
                        codewords.Add(107); // FNC1
                }
            }

            if (eci > 0)
            {
                if (eci > 811799)
                    throw new InvalidDataException("Dot Code: Invalid ECI value.");

                codewords.Add(108); // FNC2
                if (eci <= 39)
                    codewords.Add((byte)eci);

                else
                {
                    // The next three codewords valued A, B & C encode the ECI value of
                    // (A - 40) * 12769 + B * 113 + C + 40 (Section 5.2.1)
                    int a, b, c;
                    a = (eci - 40) / 12769;
                    b = ((eci - 40) - (12769 * a)) / 113;
                    c = (eci - 40) - (12769 * a) - (113 * b);

                    codewords.Add((byte)(a + 40));
                    codewords.Add((byte)b);
                    codewords.Add((byte)c);
                }
            }

            // Prevent encodation as a macro if a special character is in first position
            if (barcodeData[inputPosition] == 9)
            {
                codewords.Add(101); // Latch A
                codewords.Add(73);  // HT
                encodingMode = 'A';
            }

            if (barcodeData[inputPosition] == 28)
            {
                codewords.Add(101); // Latch A
                codewords.Add(92);  // FS
                encodingMode = 'A';
            }

            if (barcodeData[inputPosition] == 29)
            {
                codewords.Add(101);  // Latch A
                codewords.Add(93);  // GS
                encodingMode = 'A';
            }

            if (barcodeData[inputPosition] == 30)
            {
                codewords.Add(101); // Latch A
                codewords.Add(94);  // RS
                encodingMode = 'A';
            }

            do
            {
                done = false;
                // Step A.
                if ((inputPosition == length - 2) && (insideMacro != 0) && (insideMacro != 100))
                {
                    // Inside_macro only gets set to 97, 98 or 99 if the last two characters are RS/EOT
                    inputPosition += 2;
                    done = true;
                }

                if ((inputPosition == length - 1) && (insideMacro == 100))
                {
                    // Inside_macro only gets set to 100 if the last character is EOT
                    inputPosition++;
                    done = true;
                }

                // Step B1.
                if ((!done) && (encodingMode == 'C'))
                {
                    if ((codewords.Count == 0) && (length > 9))
                    {
                        if ((barcodeData[inputPosition] == '[')
                                && (barcodeData[inputPosition + 1] == ')')
                                && (barcodeData[inputPosition + 2] == '>')
                                && (barcodeData[inputPosition + 3] == 30)   // RS
                                && (barcodeData[length - 1] == 04))         // EOT
                        {
                            if ((barcodeData[inputPosition + 6] == 29) && (barcodeData[length - 2] == 30))
                            { 
                                // GS/RS
                                if ((barcodeData[inputPosition + 4] == '0') && (barcodeData[inputPosition + 5] == '5'))
                                {
                                    codewords.Add(102); // Shift B
                                    codewords.Add(97);  // Macro
                                    inputPosition += 7;
                                    insideMacro = 97;
                                    done = true;
                                }

                                if ((barcodeData[inputPosition + 4] == '0') && (barcodeData[inputPosition + 5] == '6'))
                                {
                                    codewords.Add(102); // Shift B
                                    codewords.Add(98);  // Macro
                                    inputPosition += 7;
                                    insideMacro = 98;
                                    done = true;
                                }

                                if ((barcodeData[inputPosition + 4] == '1') && (barcodeData[inputPosition + 5] == '2'))
                                {
                                    codewords.Add(102); // Shift B
                                    codewords.Add(99);  // Macro
                                    inputPosition += 7;
                                    insideMacro = 99;
                                    done = true;
                                }
                            }

                            if (!done && Char.IsDigit(barcodeData[inputPosition]) && Char.IsDigit(barcodeData[inputPosition + 1]))
                            {
                                codewords.Add(102); // Shift B
                                codewords.Add(100); // Macro
                                inputPosition += 4;
                                insideMacro = 100;
                                done = true;
                            }
                        }
                    }
                }

                // Step B2.
                if ((!done) && (encodingMode == 'C'))
                {
                    if (SeventeenTen(inputPosition, length))
                    {
                        codewords.Add(100); // (17)...(10)
                        codewords.Add((byte)(((barcodeData[inputPosition + 2] - '0') * 10) + (barcodeData[inputPosition + 3] - '0')));
                        codewords.Add((byte)(((barcodeData[inputPosition + 4] - '0') * 10) + (barcodeData[inputPosition + 5] - '0')));
                        codewords.Add((byte)(((barcodeData[inputPosition + 6] - '0') * 10) + (barcodeData[inputPosition + 7] - '0')));
                        inputPosition += 10;
                        done = true;
                    }
                }

                if ((!done) && (encodingMode == 'C'))
                {
                    if (DatumC(inputPosition, length) || ((barcodeData[inputPosition] == '[') && isGS1))
                    {
                        if (barcodeData[inputPosition] == '[')
                        {
                            codewords.Add(107);  // FNC1
                            inputPosition++;
                        }

                        else
                        {
                            codewords.Add((byte)(((barcodeData[inputPosition] - '0') * 10) + (barcodeData[inputPosition + 1] - '0')));
                            inputPosition += 2;
                        }

                        done = true;
                    }
                }

                // Setp B3.
                if ((!done) && (encodingMode == 'C'))
                {
                    if (IsBinary(inputPosition))
                    {
                        if (NumberOfDigits(inputPosition + 1, length) > 0)
                        {
                            if ((barcodeData[inputPosition] - 128) < 32)
                            {
                                codewords.Add(110); // Binary Shift A
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); // Binary Shift B
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 - 32));
                            }

                            inputPosition++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch
                            encodingMode = 'X';
                        }

                        done = true;

                    }
                }

                // Step B4.
                if ((!done) && (encodingMode == 'C'))
                {
                    int m = AheadA(inputPosition, length);
                    int n = AheadB(inputPosition, length);
                    if (m > n)
                    {
                        codewords.Add(101); // Latch A
                        encodingMode = 'A';
                    }

                    else
                    {
                        if (n <= 4)
                        {
                            codewords.Add((byte)(101 + n)); // nx Shift B
                            for (int i = 0; i < n; i++)
                            {
                                codewords.Add((byte)(barcodeData[inputPosition] - 32));
                                inputPosition++;
                            }
                        }

                        else
                        {
                            codewords.Add(106); // Latch B
                            encodingMode = 'B';
                        }
                    }

                    done = true;

                }

                // Step C1
                if ((!done) && (encodingMode == 'B'))
                {
                    int n = TryC(inputPosition, length);
                    if (n >= 2)
                    {
                        if (n <= 4)
                        {
                            codewords.Add((byte)(103 + (n - 2))); // nx Shift C
                            for (int i = 0; i < n; i++)
                            {
                                codewords.Add((byte)(((barcodeData[inputPosition] - '0') * 10) + (barcodeData[inputPosition + 1] - '0')));
                                inputPosition += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(106); // Latch C
                            encodingMode = 'C';
                        }

                        done = true;
                    }
                }

                // Step C2.
                if ((!done) && (encodingMode == 'B'))
                {
                    if ((barcodeData[inputPosition] == '[') && isGS1)
                    {
                        codewords.Add(107); // FNC1
                        inputPosition++;
                        done = true;
                    }

                    else
                    {
                        if (DatumB(inputPosition, length))
                        {
                            if ((barcodeData[inputPosition] >= 32) && (barcodeData[inputPosition] <= 127))
                            {
                                codewords.Add((byte)(barcodeData[inputPosition] - 32));
                                done = true;
                            }

                            if ((barcodeData[inputPosition] == 13)) // CRLF
                            {
                                codewords.Add(96);
                                inputPosition++;
                                done = true;
                            }

                            else if (inputPosition != 0)
                            {
                                // HT, FS, GS and RS in the first data position would be interpreted as a macro (see table 2).
                                switch ((int)barcodeData[inputPosition])
                                {
                                    case 9: // HT
                                        codewords.Add(97);
                                        break;

                                    case 28: // FS
                                        codewords.Add(98);
                                        break;

                                    case 29: // GS
                                        codewords.Add(99);
                                        break;

                                    case 30: // RS
                                        codewords.Add(100);
                                        break;
                                }

                                done = true;
                            }

                            if (done)
                                inputPosition++;
                        }
                    }
                }

                /* Step C3 */
                if ((!done) && (encodingMode == 'B'))
                {
                    if (IsBinary(inputPosition))
                    {
                        if (DatumB(inputPosition + 1, length))
                        {
                            if ((barcodeData[inputPosition] - 128) < 32)
                            {
                                codewords.Add(110); // Binary Shift A
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); // Binary Shift B
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 - 32));
                            }

                            inputPosition++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch
                            encodingMode = 'X';
                        }

                        done = true;
                    }
                }

                // Step C4.
                if ((!done) && (encodingMode == 'B'))
                {
                    if (AheadA(inputPosition, length) == 1)
                    {
                        codewords.Add(101); // Shift A
                        if (barcodeData[inputPosition] < 32)
                            codewords.Add((byte)(barcodeData[inputPosition] + 64));

                        else
                            codewords.Add((byte)(barcodeData[inputPosition] - 32));

                        inputPosition++;
                    }

                    else
                    {
                        codewords.Add(102); // Latch A
                        encodingMode = 'A';
                    }

                    done = true;
                }

                /* Step D1 */
                if ((!done) && (encodingMode == 'A'))
                {
                    int n = TryC(inputPosition, length);
                    if (n >= 2)
                    {
                        if (n <= 4)
                        {
                            codewords.Add((byte)(103 + (n - 2))); // nx Shift C
                            for (int i = 0; i < n; i++)
                            {
                                codewords.Add((byte)(((barcodeData[inputPosition] - '0') * 10) + (barcodeData[inputPosition + 1] - '0')));
                                inputPosition += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(106); // Latch C
                            encodingMode = 'C';
                        }

                        done = true;
                    }
                }

                // Step D2.
                if ((!done) && (encodingMode == 'A'))
                {
                    if ((barcodeData[inputPosition] == '[') && isGS1)
                    {
                        codewords.Add(107); // FNC1
                        inputPosition++;
                        done = true;
                    }

                    else
                    {
                        if (DatumA(inputPosition, length))
                        {
                            if (barcodeData[inputPosition] < 32)
                                codewords.Add((byte)(barcodeData[inputPosition] + 64));

                            else
                                codewords.Add((byte)(barcodeData[inputPosition] - 32));

                            inputPosition++;
                            done = true;
                        }
                    }
                }

                // Step D3.
                if ((!done) && (encodingMode == 'A'))
                {
                    if (IsBinary(inputPosition))
                    {
                        if (DatumA(inputPosition + 1, length))
                        {
                            if ((barcodeData[inputPosition] - 128) < 32)
                            {
                                codewords.Add(110); // binary Shift A
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); // Binary Shift B
                                codewords.Add((byte)(barcodeData[inputPosition] - 128 - 32));
                            }

                            inputPosition++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch
                            encodingMode = 'X';
                        }

                        done = true;
                    }
                }

                // Step D4.
                if ((!done) && (encodingMode == 'A'))
                {
                    int n = AheadB(inputPosition, length);
                    if (n <= 6)
                    {
                        codewords.Add((byte)(95 + n)); // nx Shift B
                        for (int i = 0; i < n; i++)
                        {
                            codewords.Add((byte)(barcodeData[inputPosition] - 32));
                            inputPosition++;
                        }
                    }

                    else
                    {
                        codewords.Add(102); // Latch B
                        encodingMode = 'B';
                    }

                    done = true;
                }

                // Step E1.
                if ((!done) && (encodingMode == 'X'))
                {
                    int n = TryC(inputPosition, length);
                    if (n >= 2)
                    {
                        // Empty binary buffer.
                        FlushBinaryBuffer(codewords, binaryBuffer, binaryBufferSize);
                        binaryBuffer = 0;
                        binaryBufferSize = 0;

                        if (n <= 7)
                        {
                            codewords.Add((byte)(101 + n)); // Interrupt for nx Shift C.
                            for (int i = 0; i < n; i++)
                            {
                                codewords.Add((byte)(((barcodeData[inputPosition] - '0') * 10) + (barcodeData[inputPosition + 1] - '0')));
                                inputPosition += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(111); // Terminate with Latch to C
                            encodingMode = 'C';
                        }

                        done = true;
                    }
                }

                // Step E2.
                /* Section 5.2.1.1 para D.2.i states:
                 * "Groups of six codewords, each valued between 0 and 102, are radix converted from
                 * base 103 into five base 259 values..."
                 */
                if ((!done) && (encodingMode == 'X'))
                {
                    if (IsBinary(inputPosition)
                            || IsBinary(inputPosition + 1)
                            || IsBinary(inputPosition + 2)
                            || IsBinary(inputPosition + 3))
                    {
                        binaryBuffer *= 259;
                        binaryBuffer += barcodeData[inputPosition];
                        binaryBufferSize++;

                        if (binaryBufferSize == 5)
                        {
                            FlushBinaryBuffer(codewords, binaryBuffer, binaryBufferSize);
                            binaryBuffer = 0;
                            binaryBufferSize = 0;
                        }

                        inputPosition++;
                        done = true;
                    }
                }

                // Step E3.
                if ((!done) && (encodingMode == 'X'))
                {
                    // Empty binary buffer.
                    FlushBinaryBuffer(codewords, binaryBuffer, binaryBufferSize);
                    binaryBuffer = 0;
                    binaryBufferSize = 0;

                    if (AheadA(inputPosition, length) > AheadB(inputPosition, length))
                    {
                        codewords.Add(109); // Terminate with Latch to A
                        encodingMode = 'A';
                    }

                    else
                    {
                        codewords.Add(110); // Terminate with Latch to B
                        encodingMode = 'B';
                    }

                    done = true;
                }

            } while (inputPosition < length);

            if (encodingMode == 'X')
            {
                if (binaryBufferSize > 0)
                    FlushBinaryBuffer(codewords, binaryBuffer, binaryBufferSize);    // Empty binary buffer.

                binaryFinish = true;
            }
        }

        // Convert codewords to binary data stream.
        private static void BinaryDotStream(byte[] maskedArray, BitVector dotStream)
        {
            int arrayLength = maskedArray.Length;
            // Mask value is encoded as two dots.
            dotStream.Clear();
            dotStream.AppendBits(maskedArray[0], 2);

            // The rest of the data uses 9-bit dot patterns from Annex C.
            for (int i = 1; i < arrayLength; i++)
                dotStream.AppendBits(dotPatterns[maskedArray[i]], 9);
        }

        // Place the dots in the symbol.
        private static void FoldDotStream(BitVector dotStream, int width, int height, byte[] dotArray)
        {
            int column, row;
            int position = 0;

            if (height % 2 > 0)
            {
                // Horizontal folding.
                for (row = 0; row < height; row++)
                {
                    for (column = 0; column < width; column++)
                    {
                        if (((column + row) % 2) == 0)
                        {
                            if (IsCorner(column, row, width, height))
                                dotArray[(row * width) + column] = (byte)('C');

                            else
                            {
                                dotArray[((height - row - 1) * width) + column] = dotStream[position];
                                position++;
                            }
                        }

                        else
                            dotArray[((height - row - 1) * width) + column] = (byte)(' '); // Non-data position
                    }
                }

                // Corners.
                dotArray[width - 2] = dotStream[position];
                position++;
                dotArray[(height * width) - 2] = dotStream[position];
                position++;
                dotArray[(width * 2) - 1] = dotStream[position];
                position++;
                dotArray[((height - 1) * width) - 1] = dotStream[position];
                position++;
                dotArray[0] = dotStream[position];
                position++;
                dotArray[(height - 1) * width] = dotStream[position];
            }

            else
            {
                // Vertical folding.
                for (column = 0; column < width; column++)
                {
                    for (row = 0; row < height; row++)
                    {
                        if (((column + row) % 2) == 0)
                        {
                            if (IsCorner(column, row, width, height))
                                dotArray[(row * width) + column] = (byte)('C');

                            else
                            {
                                dotArray[(row * width) + column] = dotStream[position];
                                position++;
                            }
                        }

                        else
                            dotArray[(row * width) + column] = (byte)(' '); // Non-data position
                    }
                }

                // Corners.
                dotArray[((height - 1) * width) - 1] = dotStream[position];
                position++;
                dotArray[(height - 2) * width] = dotStream[position];
                position++;
                dotArray[(height * width) - 2] = dotStream[position];
                position++;
                dotArray[((height - 1) * width) + 1] = dotStream[position];
                position++;
                dotArray[width - 1] = dotStream[position];
                position++;
                dotArray[0] = dotStream[position];
            }
        }

        // Determines if a given dot is a reserved corner dot to be used by one of the last six bits.
        private static bool IsCorner(int column, int row, int width, int height)
        {
            bool corner = false;

            // Top Left.
            if ((column == 0) && (row == 0))
                corner = true;

            // Top Right.
            if (height % 2 > 0)
            {
                if (((column == width - 2) && (row == 0)) || ((column == width - 1) && (row == 1)))
                    corner = true;
            }

            else
            {
                if ((column == width - 1) && (row == 0))
                    corner = true;
            }

            // Bottom Left.
            if (height % 2 > 0)
            {
                if ((column == 0) && (row == height - 1))
                    corner = true;
            }

            else
            {
                if (((column == 0) && (row == height - 2)) || ((column == 1) && (row == height - 1)))
                    corner = true;
            }

            // Bottom Right.
            if (((column == width - 2) && (row == height - 1)) || ((column == width - 1) && (row == height - 2)))
                corner = true;

            return corner;
        }

        private static bool GetDot(byte[] dots, int height, int width, int x, int y)
        {
            bool retval = false;

            if ((x >= 0) && (x < width) && (y >= 0) && (y < height))
            {
                if (dots[(y * width) + x] == 1)
                    retval = true;
            }

            return retval;
        }

        private static void ApplyMask(int mask, int dataLength, byte[] maskedCodewords, List<byte> codewords, int eccLength)
        {
            int weight = 0;
            int j;

            switch (mask)
            {
                case 0:
                    maskedCodewords[0] = 0;
                    for (j = 0; j < dataLength; j++)
                        maskedCodewords[j + 1] = codewords[j];
                    break;

                case 1:
                    maskedCodewords[0] = 1;
                    for (j = 0; j < dataLength; j++)
                    {
                        maskedCodewords[j + 1] = (byte)((weight + codewords[j]) % 113);
                        weight += 3;
                    }
                    break;

                case 2:
                    maskedCodewords[0] = 2;
                    for (j = 0; j < dataLength; j++)
                    {
                        maskedCodewords[j + 1] = (byte)((weight + codewords[j]) % 113);
                        weight += 7;
                    }
                    break;

                case 3:
                    maskedCodewords[0] = 3;
                    for (j = 0; j < dataLength; j++)
                    {
                        maskedCodewords[j + 1] = (byte)((weight + codewords[j]) % 113);
                        weight += 17;
                    }
                    break;
            }

            RSEncode(dataLength + 1, eccLength, maskedCodewords);
        }


        private static void ForceCorners(int width, int height, byte[] dotArray)
        {
            if (width % 2 > 0)
            {
                // "Vertical" symbol
                dotArray[0] = 1;
                dotArray[width - 1] = 1;
                dotArray[(height - 2) * width] = 1;
                dotArray[((height - 1) * width) - 1] = 1;
                dotArray[((height - 1) * width) + 1] = 1;
                dotArray[(height * width) - 2] = 1;
            }

            else
            {
                // "Horizontal" symbol
                dotArray[0] = 1;
                dotArray[width - 2] = 1;
                dotArray[(2 * width) - 1] = 1;
                dotArray[((height - 1) * width) - 1] = 1;
                dotArray[(height - 1) * width] = 1;
                dotArray[(height * width) - 2] = 1;
            }
        }

        private static bool ClearColumn(byte[] dots, int height, int width, int x)
        {
            int y;

            for (y = x & 1; y < height; y += 2)
            {

                if (GetDot(dots, height, width, x, y))
                    return false;
            }

            return true;

        }

        private static bool ClearRow(byte[] dots, int height, int width, int y)
        {
            int x;

            for (x = y & 1; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, y))
                    return false;
            }

            return true;
        }

        // Dot pattern scoring routine from Annex A.
        private static int ScoreArray(byte[] dots, int height, int width)
        {
            int worstEdge;
            int x, y, first, last, sum;
            int penalty = 0;
            int penaltyLocal = 0;

            // First, guard against "pathelogical" gaps in the array
            if ((height & 1) > 0)
            {
                if (height < 12)
                {
                    sum = 0;
                    for (x = 1; x < width - 1; x++)
                    {
                        if (!(ClearColumn(dots, height, width, x)))
                        {
                            sum = 0;

                            if (penaltyLocal > 0)
                            {
                                penalty += penaltyLocal;
                                penaltyLocal = 0;
                            }
                        }

                        else
                        {
                            sum++;
                            if (sum == 1)
                                penaltyLocal = height;

                            else
                                penaltyLocal *= height;
                        }
                    }
                }
            }

            else
            {
                if (width < 12)
                {
                    sum = 0;
                    for (y = 1; y < height - 1; y++)
                    {
                        if (!(ClearRow(dots, height, width, y)))
                        {
                            sum = 0;
                            if (penaltyLocal > 0)
                            {
                                penalty += penaltyLocal;
                                penaltyLocal = 0;
                            }
                        }

                        else
                        {
                            sum++;
                            if (sum == 1)
                                penaltyLocal = width;

                            else
                                penaltyLocal *= width;
                        }
                    }
                }
            }

            // Across the top edge, count printed dots and measure their extent.
            sum = 0;
            first = -1;
            last = -1;

            for (x = 0; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, 0))
                {
                    if (first < 0)
                        first = x;

                    last = x;
                    sum++;
                }
            }

            worstEdge = sum + last - first;
            worstEdge *= height;
            sum = 0;
            first = -1;

            // Across the bottom edge, ditto
            for (x = width & 1; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, height - 1))
                {
                    if (first < 0)
                        first = x;

                    last = x;
                    sum++;
                }
            }

            sum += last - first;
            sum *= height;
            if (sum < worstEdge)
                worstEdge = sum;

            sum = 0;
            first = -1;

            // Down the left edge, ditto
            for (y = 0; y < height; y += 2)
            {
                if (GetDot(dots, height, width, 0, y))
                {
                    if (first < 0)
                        first = y;

                    last = y;
                    sum++;
                }
            }

            sum += last - first;
            sum *= width;
            if (sum < worstEdge)
                worstEdge = sum;

            sum = 0;
            first = -1;

            // Down the right edge, ditto
            for (y = height & 1; y < height; y += 2)
            {
                if (GetDot(dots, height, width, width - 1, y))
                {
                    if (first < 0)
                        first = y;

                    last = y;
                    sum++;
                }
            }

            sum += last - first;
            sum *= width;
            if (sum < worstEdge)
                worstEdge = sum;

            // Throughout the array, count the # of unprinted 5-somes (cross patterns)
            // plus the # of printed dots surrounded by 8 unprinted neighbors
            sum = 0;
            for (y = 0; y < height; y++)
            {
                for (x = y & 1; x < width; x += 2)
                {
                    if ((!GetDot(dots, height, width, x - 1, y - 1))
                            && (!GetDot(dots, height, width, x + 1, y - 1))
                            && (!GetDot(dots, height, width, x - 1, y + 1))
                            && (!GetDot(dots, height, width, x + 1, y + 1))
                            && ((!GetDot(dots, height, width, x, y))
                            || ((!GetDot(dots, height, width, x - 2, y))
                            && (!GetDot(dots, height, width, x, y - 2))
                            && (!GetDot(dots, height, width, x + 2, y))
                            && (!GetDot(dots, height, width, x, y + 2)))))
                    {
                        sum++;
                    }
                }
            }

            return (worstEdge - sum * sum);
        }

        // Check if the next character is directly encodable in code set A (Annex F.II.D)
        private bool DatumA(int position, int length)
        {
            bool retval = false;

            if (position < length)
            {
                if (barcodeData[position] <= 95)
                    retval = true;
            }

            return retval;
        }

        // Check if the next character is directly encodable in code set B (Annex F.II.D)
        private bool DatumB(int position, int length)
        {
            bool retval = false;

            if (position < length)
            {
                if (barcodeData[position] >= 32)
                    retval = true;

                switch ((int)barcodeData[position])
                {
                    case 9:     // HT
                    case 28:    // FS
                    case 29:    // GS
                    case 30:    // RS
                        retval = false;
                        break;
                }

                if ((barcodeData[position] == 13) && (barcodeData[position + 1] == 10)) // CRLF
                    retval = true;
            }

            return retval;
        }
        // Check if the next characters are directly encodable in code set C (Annex F.II.D)
        private bool DatumC(int position, int length)
        {
            bool retval = false;

            if (position <= length - 2)
            {
                if ((Char.IsDigit(barcodeData[position])) && (Char.IsDigit(barcodeData[position + 1])))
                    retval = true;
            }

            return retval;
        }

        // Returns how many consecutive digits lie immediately ahead (Annex F.II.A)
        private int NumberOfDigits(int position, int length)
        {
            int i;
            for (i = position; ((i < length) && Char.IsDigit(barcodeData[i])); i++) ;

            return i - position;
        }

        // Checks ahead for 10 or more digits starting "17xxxxxx10..." (Annex F.II.B)
        private bool SeventeenTen(int position, int length)
        {
            bool found = false;

            if (NumberOfDigits(position, length) >= 10)
            {
                if (((barcodeData[position] == '1') && (barcodeData[position + 1] == '7'))
                        && ((barcodeData[position + 8] == '1') && (barcodeData[position + 9] == '0')))
                    found = true;
            }

            return found;
        }

        // Annex F.II.G.
        private int AheadA(int position, int length)
        {
            int count = 0;

            for (int i = position; ((i < length) && DatumA(i, length)) && (TryC(i, length) < 2); i++)
                count++;

            return count;
        }

        // Annex F.II.H 
        int AheadB(int position, int length)
        {
            int count = 0;

            for (int i = position; ((i < length) && DatumB(i, length)) && (TryC(i, length) < 2); i++)
                count++;

            return count;
        }

        /*  Checks how many characters ahead can be reached while datumC is true,
         *  returning the resulting number of codewords (Annex F.II.E)
         */
        int AheadC(int position, int length)
        {
            int count = 0;

            for (int i = position; (i < length) && DatumC(i, length); i += 2)
                count++;

            return count;
        }

        // Annex F.II.F.
        private int TryC(int position, int length)
        {
            int count = 0;

            if (NumberOfDigits(position, length) > 0)
            {
                if (AheadC(position, length) > AheadC(position + 1, length))
                    count = AheadC(position, length);
            }

            return count;
        }

        // Checks if the next character is in the range 128 to 255  (Annex F.II.I)
        private bool IsBinary(int position)
        {
            return (barcodeData[position] > 127 && barcodeData[position] < 256);
        }

        private static void FlushBinaryBuffer(List<byte> codeWords, ulong binaryBuffer, int binaryBufferSize)
        {
            byte[] radixValues = new byte[6]; // Holds reversed radix 103 values.

            for (int i = 0; i < (binaryBufferSize + 1); i++)
            {
                radixValues[i] = (byte)(binaryBuffer % 103);
                binaryBuffer /= 103;
            }

            for (int i = 0; i < (binaryBufferSize + 1); i++)
                codeWords.Add(radixValues[binaryBufferSize - i]);
        }

        //-------------------------------------------------------------------------
        // "RSEncode(nd,nc, wd)" adds "nc" R-S check words to "nd" data words in wd[]
        // employing Galois Field GF, where GF is prime, with a prime modulus of PM
        //-------------------------------------------------------------------------

        private static void RSEncode(int nd, int nc, byte[] wd)
        {
            int i, j, k, nw, start, step;
            int ND, NW, NC;
            int[] root = new int[GF];
            int[] c = new int[GF];

            // Start by generating "nc" roots (antilogs).
            root[0] = 1;
            for (i = 1; i <= nc && (i < GF); i++)
                root[i] = (PM * root[i - 1]) % GF;

            // Here we compute how many interleaved R-S blocks will be needed
            nw = nd + nc;
            step = (nw + GF - 2) / (GF - 1);

            // ...& then for each such block:
            for (start = 0; start < step; start++)
            {
                ND = (nd - start + step - 1) / step;
                NW = (nw - start + step - 1) / step;
                NC = NW - ND;

                // First compute the generator polynomial "c" of order "NC":
                for (i = 1; i <= NC; i++)
                    c[i] = 0;

                c[0] = 1;

                for (i = 1; i <= NC; i++)
                {
                    for (j = NC; j >= 1; j--)
                        c[j] = (GF + c[j] - (root[i] * c[j - 1]) % GF) % GF;
                }

                // And then compute the corresponding checkword values into wd[]
                // ... (a) starting at wd[start] & (b) stepping by step
                for (i = ND; i < NW; i++)
                    wd[start + i * step] = 0;

                for (i = 0; i < ND; i++)
                {
                    k = (wd[start + i * step] + wd[start + ND * step]) % GF;
                    for (j = 0; j < NC - 1; j++)
                        wd[start + (ND + j) * step] = (byte)((GF - ((c[j + 1] * k) % GF) + wd[start + (ND + j + 1) * step]) % GF);

                    wd[start + (ND + NC - 1) * step] = (byte)((GF - ((c[NC] * k) % GF)) % GF);
                }

                for (i = ND; i < NW; i++)
                    wd[start + i * step] = (byte)((GF - wd[start + i * step]) % GF);
            }
        }
    }
}
