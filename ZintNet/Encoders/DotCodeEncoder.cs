/* DotCodeEncoder.cs - Handles DotCode 2D symbol */

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

using System;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ZintNet.Encoders
{
    internal class DotCodeEncoder : SymbolEncoder
    {
        #region Tables.

        // DotCode symbol character dot patterns, from Annex C.
        private readonly int[] DotPatterns = {
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

        #endregion

        #region Constants.

        private const int GF = 113;
        private const int PM = 3;
        private const int SCORE_UNLIT_EDGE = -99999;

        #endregion

        private readonly int optionSymbolColumns;
        private readonly int optionUserMask;

        public DotCodeEncoder(Symbology symbolId, char[] barcodeMessage, int optionSymbolColumns, int eci, int optionUserMask, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionSymbolColumns = optionSymbolColumns;
            this.optionUserMask = optionUserMask;
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
            }

            DotCode();
            return Symbol;
        }

        private void DotCode()
        {
            int dataLength, eccLength;
            int minimumDots, numberOfDots, minimumArea, paddingDots;
            int height, width;
            int highScore, bestMask;
            bool binaryFinish;
            int inputLength = barcodeData.Length;
            int[] maskScore = new int[8];
            List<byte> codewords = new List<byte>();
            byte[] maskedCodewords;
            char encodingMode = 'C';
            byte insideMacro = 0;
            ulong binaryBuffer = 0;
            int bufferSize = 0;

            DCEncode(barcodeData, inputLength, true, 4, 30, codewords, ref encodingMode, ref insideMacro, ref binaryBuffer, ref bufferSize);
            binaryFinish = encodingMode == 'X';
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
                            {
                                width += 2;
                            }
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
                            {
                                height += 2;
                            }
                        }
                    }
                }
            }

            else    // Fixed width.
            {
                width = optionSymbolColumns;
                height = (minimumArea + (width - 1)) / width;
                if (((width + height) % 2) == 0)
                {
                    height++;
                }
            }

            if ((height > 200) || (width > 200))
            {
                if ((height > 200) && (width > 200))
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Dot Code: Symbol size '{0} x {1}'(WxH) is too large.", width, height));
                }

                else
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Dot Code: Symbol {0} of {1} is too large.", width > 200 ? "width" : "height", width > 200 ? width : height));
                }
            }

            if ((height < 5) || (width < 5))
            {
                if ((height < 5) && (width < 5))
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Dot Code: Symbol size '{0} x {1}'(WxH) is too small.", width, height));
                }

                else
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Dot Code: Symbol {0} of {1} is too small.", width > 200 ? "width" : "height", width > 200 ? width : height));
                }
            }

            numberOfDots = (height * width) / 2;
            BitVector dotStream = new BitVector();
            byte[] dotArray = new byte[width * height];

            // Add pad characters.
            paddingDots = numberOfDots - minimumDots;   // Get the number of free dots available for padding.
            bool isFirst = true;
            while (paddingDots >= 9)
            {
                if (paddingDots < 18 && ((dataLength % 2) == 0))
                {
                    paddingDots -= 9;
                }

                else if (paddingDots >= 18)
                {
                    if ((dataLength % 2) == 0)
                    {
                        paddingDots -= 9;
                    }

                    else
                    {
                        paddingDots -= 18;
                    }
                }

                else
                {
                    break;  // Not enough dots left for padding.
                }

                if (isFirst && binaryFinish)
                {
                    codewords.Add(109);
                }

                else
                {
                    codewords.Add(106);
                }

                dataLength++;
                isFirst = false;
            }

            eccLength = 3 + (dataLength / 2);
            maskedCodewords = new byte[dataLength + 1 + eccLength];
            if (optionUserMask > 0)
            {
                bestMask = optionUserMask - 1;
            }

            else
            {
                // Evaluate data mask options.
                for (int i = 0; i < 4; i++)
                {
                    ApplyMask(i, dataLength, maskedCodewords, codewords, eccLength);
                    BinaryDotStream(maskedCodewords, dotStream);

                    // Add pad bits.
                    for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
                    {
                        dotStream.AppendBit(1);
                    }

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
                if (highScore <= (height * width) / 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        ApplyMask(i, dataLength, maskedCodewords, codewords, eccLength);
                        BinaryDotStream(maskedCodewords, dotStream);

                        // Add pad bits.
                        for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
                        {
                            dotStream.AppendBit(1);
                        }

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
            }

            // Apply best mask.
            ApplyMask(bestMask % 4, dataLength, maskedCodewords, codewords, eccLength);
            BinaryDotStream(maskedCodewords, dotStream);

            // Add pad bits.
            for (int s = dotStream.SizeInBits; s < numberOfDots; s++)
            {
                dotStream.AppendBit(1);
            }

            FoldDotStream(dotStream, width, height, dotArray);
            if (bestMask >= 4)
            {
                ForceCorners(width, height, dotArray);
            }

            // Build the symbol.
            byte[] rowData;
            for (int h = 0; h < height; h++)
            {
                rowData = new byte[width];
                for (int w = 0; w < width; w++)
                {
                    if (dotArray[(h * width) + w] == 1)
                    {
                        rowData[w] = 1;
                    }
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        // Analyse input data stream and encode using algorithm from Annex F.
        private void DCEncode(char[] source, int length, bool lastSeg, int lastEOT, int lastRSEOT, List<byte> codewords,
                             ref char encodingMode, ref byte insideMacro, ref ulong binaryBuffer, ref int binaryBufferSize)
        {
            string leadSpecials = "\x09\x1c\x1d\x1e"; // HT, FS, GS, RS.
            int i;
            int position = 0;
            int nx = 0;
            bool firstSeg = (codewords.Count == 0);

            if (firstSeg)
            {
                if (!isGS1 && eci == 0 && length > 2 && Common.IsTwoDigits(source, length, 0))
                {
                    codewords.Add(107); // FNC1.
                }

                else if (leadSpecials.IndexOf(source[0]) != -1)
                {
                    // Prevent encodation as a macro if a special character is in first position.
                    codewords.Add(101); // Latch A.
                    codewords.Add((byte)(source[0] + 64));
                    encodingMode = 'A';
                    position++;
                }

                else if (length > 5)
                {
                    // Note assuming macro headers don't straddle segments.
                    // Step C1.
                    if (source[0] == '[' && source[1] == ')' && source[2] == '>' && source[3] == 30 /*RS*/ && lastEOT > 1)
                    {
                        bool format050612 = (source[4] == '0' && (source[5] == '5' || source[5] == '6')) || (source[4] == '1' && source[5] == '2');
                        insideMacro = 0;
                        if (length > 6 && format050612 && source[6] == 29 /*GS*/ && lastRSEOT > 1)
                        {
                            if (source[5] == '5')
                            {
                                insideMacro = 97;
                            }

                            else if (source[5] == '6')
                            {
                                insideMacro = 98;
                            }

                            else
                            {
                                insideMacro = 99;
                            }
                        }

                        else if (!format050612 && Common.IsTwoDigits(source, length, 4))
                        {
                            insideMacro = 100; // Note: no longer using for malformed 05/06/12.
                        }

                        if (insideMacro > 0)
                        {
                            codewords.Add(106); // Latch B.
                            encodingMode = 'B';
                            codewords.Add(insideMacro); // Macro.
                            if (insideMacro == 100)
                            {
                                codewords.Add((byte)(source[4] - '0' + 16));
                                codewords.Add((byte)(source[5] - '0' + 16));
                                position += 6;
                            }

                            else
                            {
                                position += 7;
                            }
                        }
                    }
                }
            }

            if (eci > 0)
            {
                if (encodingMode == 'X')
                {
                    if (eci <= 0xff)
                    {
                        AppendToBinaryBuffer(codewords, 256, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)eci, ref binaryBuffer, ref binaryBufferSize);
                        // Following BWIPP, assuming big-endian byte order.
                    }

                    else if (eci <= 0xffff)
                    {
                        AppendToBinaryBuffer(codewords, 257, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)eci >> 8, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)eci & 0xff, ref binaryBuffer, ref binaryBufferSize);
                    }

                    else
                    {
                        AppendToBinaryBuffer(codewords, 258, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)eci >> 16, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)(eci >> 8) & 0xff, ref binaryBuffer, ref binaryBufferSize);
                        AppendToBinaryBuffer(codewords, (uint)eci & 0xff, ref binaryBuffer, ref binaryBufferSize);
                    }
                }

                else
                {
                    codewords.Add(108); // FNC2.
                    if (eci <= 39)
                    {
                        codewords.Add((byte)eci);
                    }

                    else
                    {
                        // The next three codewords valued A, B & C encode the ECI value of
                        // (A - 40) * 12769 + B * 113 + C + 40 (Section 5.2.1).
                        int a, b, c;
                        a = (eci - 40) / 12769;
                        b = ((eci - 40) - (12769 * a)) / 113;
                        c = (eci - 40) - (12769 * a) - (113 * b);
                        codewords.Add((byte)(a + 40));
                        codewords.Add((byte)b);
                        codewords.Add((byte)c);
                    }
                }
            }

            while (position < length)
            {
                // Step A.
                if (lastSeg && (position == length - 2) && (insideMacro != 0) && (insideMacro != 100))
                {
                    // Inside_macro only gets set to 97, 98 or 99 if the last two characters are RS/EOT.
                    position += 2;
                    continue;
                }

                // Step B.
                if (lastSeg && (position == length - 1) && (insideMacro == 100))
                {
                    // Inside_macro only gets set to 100 if the last character is EOT.
                    position++;
                    continue;
                }

                if (encodingMode == 'C')
                {
                    // Step C2.
                    if (SeventeenTen(source, position, length))
                    {
                        codewords.Add(100); /* (17)...(10) */
                        codewords.Add((byte)Common.ToInt(source, position + 2, 2));
                        codewords.Add((byte)Common.ToInt(source, position + 4, 2));
                        codewords.Add((byte)Common.ToInt(source, position + 6, 2));
                        position += 10;
                        continue;
                    }

                    if (DatumC(source, position, length) || (isGS1 && source[position] == '\x1d'))
                    {
                        if (source[position] == '\x1d')
                        {
                            codewords.Add(107); // FNC1.
                            position++;
                        }

                        else
                        {
                            codewords.Add((byte)Common.ToInt(source, position, 2));
                            position += 2;
                        }

                        continue;
                    }

                    // Step C3.
                    if (IsBinary(source, position, length))
                    {
                        if (position + 1 < length && char.IsDigit(source[position + 1]))
                        {
                            if ((source[position] - 128) < 32)
                            {
                                codewords.Add(110); // Upper Shift A.
                                codewords.Add((byte)(source[position] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); // Upper Shift B.
                                codewords.Add((byte)(source[position] - 128 - 32));
                            }

                            position++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch.
                            encodingMode = 'X';
                        }

                        continue;
                    }

                    // Step C4.
                    {
                        int m = AheadA(source, position, length);
                        int n = AheadB(source, position, length, ref nx);
                        if (m > n)
                        {
                            codewords.Add(101); // Latch A.
                            encodingMode = 'A';
                        }

                        else
                        {
                            if (nx >= 1 && nx <= 4)
                            {
                                codewords.Add((byte)(101 + nx)); // nx Shift B.
                                for (i = 0; i < nx; i++)
                                {
                                    if (source[position] >= 32)
                                    {
                                        codewords.Add((byte)(source[position] - 32));
                                    }

                                    else if (source[position] == 13)
                                    {
                                        // CR/LF.
                                        codewords.Add(96);
                                        position++;
                                    }

                                    else
                                    {
                                        switch (source[position])
                                        {
                                            case '\x09': codewords.Add(97); break;  // HT.
                                            case '\x28': codewords.Add(98); break;  // FS.
                                            case '\x29': codewords.Add(99); break;  // GS.
                                            case '\x30': codewords.Add(100); break; // RS.
                                        }
                                    }

                                    position++;
                                }
                            }

                            else
                            {
                                codewords.Add(106); // Latch B.
                                encodingMode = 'B';
                            }
                        }

                        continue;
                    }
                }

                if (encodingMode == 'B')
                {
                    // Step D1.
                    int n = TryC(source, position, length);
                    if (n >= 2)
                    {
                        if (n <= 4)
                        {
                            codewords.Add((byte)(103 + (n - 2))); // nx Shift C.
                            for (i = 0; i < n; i++)
                            {
                                codewords.Add((byte)Common.ToInt(source, position, 2));
                                position += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(106); // Latch C.
                            encodingMode = 'C';
                        }

                        continue;
                    }

                    // Step D2.
                    if (isGS1 && source[position] == '\x1d')
                    {
                        codewords.Add(107); // FNC1.
                        position++;
                        continue;
                    }

                    if (DatumB(source, position, length) > 0)
                    {
                        bool done = false;
                        if ((source[position] >= 32) && (source[position] <= 127))
                        {
                            codewords.Add((byte)(source[position] - 32));
                            done = true;

                        }

                        else if (source[position] == 13)
                        {
                            // CR/LF.
                            codewords.Add(96);
                            position++;
                            done = true;

                        }

                        else if (!firstSeg || position != 0)
                        {
                            // HT, FS, GS and RS in the first data position would be interpreted as a macro * (see table 2).
                            switch (source[position])
                            {
                                case '\x09': codewords.Add(97); break;  // HT.
                                case '\x28': codewords.Add(98); break;  // FS.
                                case '\x29': codewords.Add(99); break;  // GS.
                                case '\x30': codewords.Add(100); break; // RS.
                            }

                            done = true;
                        }

                        if (done == true)
                        {
                            position++;
                            continue;
                        }
                    }

                    // Step D3.
                    if (IsBinary(source, position, length))
                    {
                        if (DatumB(source, position + 1, length) > 0)
                        {
                            if ((source[position] - 128) < 32)
                            {
                                codewords.Add(110); // Binary Shift A.
                                codewords.Add((byte)(source[position] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); // Binary Shift B.
                                codewords.Add((byte)(source[position] - 128 - 32));
                            }

                            position++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch.
                            encodingMode = 'X';
                        }

                        continue;
                    }

                    // Step D4.
                    if (AheadA(source, position, length) == 1)
                    {
                        codewords.Add(101); // Shift A.
                        if (source[position] < 32)
                        {
                            codewords.Add((byte)(source[position] + 64));
                        }

                        else
                        {
                            codewords.Add((byte)(source[position] - 32));
                        }

                        position++;
                    }

                    else
                    {
                        codewords.Add(102); // Latch A.
                        encodingMode = 'A';
                    }

                    continue;
                }

                if (encodingMode == 'A')
                {
                    // Step E1.
                    int n = TryC(source, position, length);
                    if (n >= 2)
                    {
                        if (n <= 4)
                        {
                            codewords.Add((byte)(103 + (n - 2))); // nx Shift C.
                            for (i = 0; i < n; i++)
                            {
                                codewords.Add((byte)Common.ToInt(source, position, 2));
                                position += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(106); // Latch C.
                            encodingMode = 'C';
                        }

                        continue;
                    }

                    // Step E2.
                    if (isGS1 && source[position] == '\x1d')
                    {
                        // Note: this branch probably never reached as no reason to be in Code Set A for GS1 data.
                        codewords.Add(107); // FNC1.
                        position++;
                        continue;
                    }

                    if (DatumA(source, position, length))
                    {
                        if (source[position] < 32)
                        {
                            codewords.Add((byte)(source[position] + 64));
                        }

                        else
                        {
                            codewords.Add((byte)(source[position] - 32));
                        }

                        position++;
                        continue;
                    }

                    // Step E3.
                    if (IsBinary(source, position, length))
                    {
                        if (DatumA(source, position + 1, length))
                        {
                            if ((source[position] - 128) < 32)
                            {
                                codewords.Add(110); // Binary Shift A.
                                codewords.Add((byte)(source[position] - 128 + 64));
                            }

                            else
                            {
                                codewords.Add(111); /* Bin Shift B */
                                codewords.Add((byte)(source[position] - 128 - 32));
                            }

                            position++;
                        }

                        else
                        {
                            codewords.Add(112); // Binary Latch.
                            encodingMode = 'X';
                        }

                        continue;
                    }

                    // Step E4.
                    AheadB(source, position, length, ref nx);
                    if (nx >= 1 && nx <= 6)
                    {
                        codewords.Add((byte)(95 + nx)); // nx Shift B.
                        for (i = 0; i < nx; i++)
                        {
                            if (source[position] >= 32)
                            {
                                codewords.Add((byte)(source[position] - 32));
                            }

                            else if (source[position] == 13)
                            {
                                // CR/LF.
                                codewords.Add(96);
                                position++;
                            }

                            else
                            {
                                switch (source[position])
                                {
                                    case '\x09': codewords.Add(97); break;  // HT.
                                    case '\x28': codewords.Add(98); break;  // FS.
                                    case '\x29': codewords.Add(99); break;  // GS.
                                    case '\x30': codewords.Add(100); break; // RS.
                                }
                            }
                            position++;
                        }
                    }

                    else
                    {
                        codewords.Add(102); // Latch B.
                        encodingMode = 'B';
                    }

                    continue;
                }

                // Step F1.
                if (encodingMode == 'X')
                {
                    int n = TryC(source, position, length);
                    if (n >= 2)
                    {
                        FlushBinaryBuffer(codewords, ref binaryBuffer, ref binaryBufferSize);
                        if (n <= 7)
                        {
                            codewords.Add((byte)(101 + n)); // Interrupt for nx Shift C.
                            for (i = 0; i < n; i++)
                            {
                                codewords.Add((byte)Common.ToInt(source, position, 2));
                                position += 2;
                            }
                        }

                        else
                        {
                            codewords.Add(111); // Terminate with Latch to C.
                            encodingMode = 'C';
                        }

                        continue;
                    }

                    // Step F2.
                    /* Section 5.2.1.1 para D.2.i states:
                     * "Groups of six codewords, each valued between 0 and 102, are radix converted from
                     * base 103 into five base 259 values..."
                     */
                    if (IsBinary(source, position, length)
                                || IsBinary(source, position + 1, length)
                                || IsBinary(source, position + 2, length)
                                || IsBinary(source, position + 3, length))
                    {
                        AppendToBinaryBuffer(codewords, source[position], ref binaryBuffer, ref binaryBufferSize);
                        position++;

                        continue;
                    }

                    // Step F3.
                    FlushBinaryBuffer(codewords, ref binaryBuffer, ref binaryBufferSize); // Empty binary buffer.

                    int v = 0;
                    if (AheadA(source, position, length) > AheadB(source, position, length, ref v))
                    {
                        codewords.Add(109); // Terminate with Latch to A.
                        encodingMode = 'A';
                    }

                    else
                    {
                        codewords.Add(110); // Terminate with Latch to B.
                        encodingMode = 'B';
                    }
                }
            }

            if (lastSeg)
            {
                if (encodingMode == 'X' && binaryBufferSize != 0)
                {
                    // Empty the binary buffer.
                    FlushBinaryBuffer(codewords, ref binaryBuffer, ref binaryBufferSize);
                }

            }
        }

        // Convert codewords to binary data stream.
        private void BinaryDotStream(byte[] maskedArray, BitVector dotStream)
        {
            int arrayLength = maskedArray.Length;
            // Mask value is encoded as two dots.
            dotStream.Clear();
            dotStream.AppendBits(maskedArray[0], 2);

            // The rest of the data uses 9-bit dot patterns from Annex C.
            for (int i = 1; i < arrayLength; i++)
            {
                dotStream.AppendBits(DotPatterns[maskedArray[i]], 9);
            }
        }

        // Place the dots in the symbol.
        private void FoldDotStream(BitVector dotStream, int width, int height, byte[] dotArray)
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
                            {
                                dotArray[(row * width) + column] = (byte)'C';
                            }

                            else
                            {
                                dotArray[((height - row - 1) * width) + column] = dotStream[position++];
                            }
                        }

                        else
                        {
                            dotArray[((height - row - 1) * width) + column] = (byte)(' '); // Non-data position
                        }
                    }
                }

                // Corners.
                dotArray[width - 2] = dotStream[position++];
                dotArray[(height * width) - 2] = dotStream[position++];
                dotArray[(width * 2) - 1] = dotStream[position++];
                dotArray[((height - 1) * width) - 1] = dotStream[position++];
                dotArray[0] = dotStream[position++];
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
                            {
                                dotArray[(row * width) + column] = (byte)'C';
                            }

                            else
                            {
                                dotArray[(row * width) + column] = dotStream[position++];
                            }
                        }

                        else
                        {
                            dotArray[(row * width) + column] = (byte)(' '); // Non-data position
                        }
                    }
                }

                // Corners.
                dotArray[((height - 1) * width) - 1] = dotStream[position++];
                dotArray[(height - 2) * width] = dotStream[position++];
                dotArray[(height * width) - 2] = dotStream[position++];
                dotArray[((height - 1) * width) + 1] = dotStream[position++];
                dotArray[width - 1] = dotStream[position++];
                dotArray[0] = dotStream[position];
            }
        }

        // Determines if a given dot is a reserved corner dot to be used by one of the last six bits.
        private bool IsCorner(int column, int row, int width, int height)
        {
            // Top Left.
            if ((column == 0) && (row == 0))
            {
                return true;
            }

            // Top Right.
            if (height % 2 > 0)
            {
                if (((column == width - 2) && (row == 0)) || ((column == width - 1) && (row == 1)))
                {
                    return true;
                }
            }

            else
            {
                if ((column == width - 1) && (row == 0))
                {
                    return true;
                }
            }

            // Bottom Left.
            if (height % 2 > 0)
            {
                if ((column == 0) && (row == height - 1))
                {
                    return true;
                }
            }

            else
            {
                if (((column == 0) && (row == height - 2)) || ((column == 1) && (row == height - 1)))
                {
                    return true;
                }
            }

            // Bottom Right.
            if (((column == width - 2) && (row == height - 1)) || ((column == width - 1) && (row == height - 2)))
            {
                return true;
            }

            return false;
        }

        private bool GetDot(byte[] dots, int height, int width, int x, int y)
        {
            if ((x >= 0) && (x < width) && (y >= 0) && (y < height))
            {
                if (dots[(y * width) + x] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyMask(int mask, int dataLength, byte[] maskedCodewords, List<byte> codewords, int eccLength)
        {
            int weight = 0;
            int j;

            switch (mask)
            {
                case 0:
                    maskedCodewords[0] = 0;
                    for (j = 0; j < dataLength; j++)
                    {
                        maskedCodewords[j + 1] = codewords[j];
                    }

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

        private void ForceCorners(int width, int height, byte[] dotArray)
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

        private bool ClearColumn(byte[] dots, int height, int width, int x)
        {
            for (int y = x & 1; y < height; y += 2)
            {

                if (GetDot(dots, height, width, x, y))
                {
                    return false;
                }
            }

            return true;

        }

        private bool ClearRow(byte[] dots, int height, int width, int y)
        {
            for (int x = y & 1; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, y))
                {
                    return false;
                }
            }

            return true;
        }

        // Calculate penalty for empty interior columns.
        private int ColumnPenalty(byte[] dots, int height, int width)
        {
            int x, penalty = 0, penaltyLocal = 0;

            for (x = 1; x < width - 1; x++)
            {
                if (ClearColumn(dots, height, width, x))
                {
                    if (penaltyLocal == 0)
                    {
                        penaltyLocal = height;
                    }

                    else
                    {
                        penaltyLocal *= height;
                    }
                }

                else
                {
                    if (penaltyLocal > 1)
                    {
                        penalty += penaltyLocal;
                        penaltyLocal = 0;
                    }
                }
            }

            return penalty + penaltyLocal;
        }

        // Calculate penalty for empty interior rows.
        private int RowPenalty(byte[] dots, int height, int width)
        {
            int y, penalty = 0, penaltyLocal = 0;

            for (y = 1; y < height - 1; y++)
            {
                if (ClearRow(dots, height, width, y))
                {
                    if (penaltyLocal == 0)
                    {
                        penaltyLocal = width;
                    }

                    else
                    {
                        penaltyLocal *= width;
                    }
                }

                else
                {
                    if (penaltyLocal > 1)
                    {
                        penalty += penaltyLocal;
                        penaltyLocal = 0;
                    }
                }
            }

            return penalty + penaltyLocal;
        }

        // Dot pattern scoring routine from Annex A.
        private int ScoreArray(byte[] dots, int height, int width)
        {
            int x, y, worstedge, first, last, sum;

            /* First, guard against "pathelogical" gaps in the array
               subtract a penalty score for empty rows/columns from total code score for each mask,
               where the penalty is Sum(N ^ n), where N is the number of positions in a column/row,
               and n is the number of consecutive empty rows/columns. */

            int penalty = RowPenalty(dots, height, width) + ColumnPenalty(dots, height, width);
            sum = 0;
            first = -1;
            last = -1;

            // Across the top edge, count printed dots and measure their extent.
            for (x = 0; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, 0))
                {
                    if (first < 0)
                    {
                        first = x;
                    }

                    last = x;
                    sum++;
                }
            }

            if (sum == 0)
            {
                return SCORE_UNLIT_EDGE;      // Guard against empty top edge.
            }

            worstedge = sum + last - first;
            worstedge *= height;

            sum = 0;
            first = -1;
            last = -1;

            // Across the bottom edge, ditto.
            for (x = width & 1; x < width; x += 2)
            {
                if (GetDot(dots, height, width, x, height - 1))
                {
                    if (first < 0)
                    {
                        first = x;
                    }

                    last = x;
                    sum++;
                }
            }

            if (sum == 0)
            {
                return SCORE_UNLIT_EDGE;      // Guard against empty bottom edge.
            }

            sum += last - first;
            sum *= height;
            if (sum < worstedge)
            {
                worstedge = sum;
            }

            sum = 0;
            first = -1;
            last = -1;

            // Down the left edge, ditto.
            for (y = 0; y < height; y += 2)
            {
                if (GetDot(dots, height, width, 0, y))
                {
                    if (first < 0)
                    {
                        first = y;
                    }

                    last = y;
                    sum++;
                }
            }

            if (sum == 0)
            {
                return SCORE_UNLIT_EDGE;      // Guard against empty left edge.
            }

            sum += last - first;
            sum *= width;
            if (sum < worstedge)
            {
                worstedge = sum;
            }

            sum = 0;
            first = -1;
            last = -1;

            // Down the right edge, ditto.
            for (y = height & 1; y < height; y += 2)
            {
                if (GetDot(dots, height, width, width - 1, y))
                {
                    if (first < 0)
                    {
                        first = y;
                    }
                    last = y;
                    sum++;
                }
            }
            if (sum == 0)
            {
                return SCORE_UNLIT_EDGE;      // Guard against empty right edge.
            }

            sum += last - first;
            sum *= width;
            if (sum < worstedge)
            {
                worstedge = sum;
            }

            // Throughout the array, count the # of unprinted 5-somes (cross patterns)
            // plus the # of printed dots surrounded by 8 unprinted neighbors.
            sum = 0;
            for (y = 0; y < height; y++)
            {
                for (x = y & 1; x < width; x += 2)
                {
                    if (!GetDot(dots, height, width, x - 1, y - 1) && !GetDot(dots, height, width, x + 1, y - 1)
                        && !GetDot(dots, height, width, x - 1, y + 1) && !GetDot(dots, height, width, x + 1, y + 1)
                        && (!GetDot(dots, height, width, x, y)
                        || (!GetDot(dots, height, width, x - 2, y) && !GetDot(dots, height, width, x, y - 2)
                        && !GetDot(dots, height, width, x + 2, y) && !GetDot(dots, height, width, x, y + 2))))
                    {
                        sum++;
                    }
                }
            }

            return (worstedge - sum * sum - penalty);
        }

        // Check if the next character is directly encodable in code set A (Annex F.II.D)
        private bool DatumA(char[] source, int position, int length)
        {
            bool retval = false;

            if (position < length)
            {
                if (source[position] <= 95)
                {
                    retval = true;
                }
            }

            return retval;
        }

        // Check if the next character is directly encodable in code set B (Annex F.II.D)
        private int DatumB(char[] source, int position, int length)
        {
            if (position < length)
            {
                if (source[position] >= 32 && source[position] <= 127)
                {
                    return 1;
                }

                switch ((int)source[position])
                {
                    case 9:     // HT
                    case 28:    // FS
                    case 29:    // GS
                    case 30:    // RS
                        return 1;
                }

                if ((position + 1 < length) && source[position] == 13 && (source[position + 1] == 10)) // CRLF.
                {
                    return 2;
                }
            }

            return 0;
        }
        // Check if the next characters are directly encodable in code set C (Annex F.II.D)
        private bool DatumC(char[] source, int position, int length)
        {
            bool retval = false;

            if (position <= length - 2)
            {
                if ((char.IsDigit(source[position])) && (char.IsDigit(source[position + 1])))
                {
                    retval = true;
                }
            }

            return retval;
        }

        // Returns how many consecutive digits lie immediately ahead (Annex F.II.A)
        private int NumberOfDigits(char[] source, int position, int length)
        {
            int i;
            for (i = position; (i < length) && char.IsDigit(source[i]); i++)
            {
                ;
            }

            return i - position;
        }

        // Checks ahead for 10 or more digits starting "17xxxxxx10..." (Annex F.II.B)
        private bool SeventeenTen(char[] source, int position, int length)
        {
            if (NumberOfDigits(source, position, length) >= 10)
            {
                if ((source[position] == '1') && (source[position + 1] == '7')
                        && (source[position + 8] == '1') && (source[position + 9] == '0'))
                {
                    return true;
                }
            }

            return false;
        }

        // Annex F.II.G.
        private int AheadA(char[] source, int position, int length)
        {
            int count = 0;

            for (int i = position; (i < length) && DatumA(source, i, length) && (TryC(source, i, length) < 2); i++)
            {
                count++;
            }

            return count;
        }

        // Annex F.II.H Note: changed to return number of chars encodable. Number of codewords returned in nx.
        private int AheadB(char[] source, int position, int length, ref int nx)
        {
            int count = 0;
            int i, incr;

            for (i = position; (i < length) && (incr = DatumB(source, i, length)) > 0 && (TryC(source, i, length) < 2); i += incr)
            {
                count++;
            }

            nx = count;
            return i - position;
        }

        //  Checks how many characters ahead can be reached while datumC is true,
        //  returning the resulting number of codewords (Annex F.II.E).
        private int AheadC(char[] source, int position, int length)
        {
            int count = 0;

            for (int i = position; (i < length) && DatumC(source, i, length); i += 2)
            {
                count++;
            }

            return count;
        }

        // Annex F.II.F.
        private int TryC(char[] source, int position, int length)
        {
            int count = 0;

            if (NumberOfDigits(source, position, length) > 0)
            {
                if (AheadC(source, position, length) > AheadC(source, position + 1, length))
                {
                    count = AheadC(source, position, length);
                }
            }

            return count;
        }

        // Checks if the next character is in the range 128 to 255  (Annex F.II.I)
        private bool IsBinary(char[] source, int position, int length)
        {
            if (position < length && source[position] >= 128)
            {
                return true;
            }

            return false;
        }

        private void FlushBinaryBuffer(List<byte> codeWords, ref ulong binaryBuffer, ref int binaryBufferSize)
        {
            byte[] radixValues = new byte[6]; // Holds reversed radix 103 values.

            if (binaryBufferSize > 0)
            {
                for (int i = 0; i < (binaryBufferSize + 1); i++)
                {
                    radixValues[i] = (byte)(binaryBuffer % 103);
                    binaryBuffer /= 103;
                }

                for (int i = 0; i < (binaryBufferSize + 1); i++)
                {
                    codeWords.Add(radixValues[binaryBufferSize - i]);
                }
            }

            binaryBuffer = 0;
            binaryBufferSize = 0;
        }

        // Add value to binary buffer, emptying if full.
        private void AppendToBinaryBuffer(List<byte> codewords, uint value, ref ulong binaryBuffer, ref int binaryBufferSize)
        {
            binaryBuffer *= 259;
            binaryBuffer += value;
            binaryBufferSize++;

            if (binaryBufferSize == 5)
            {
                FlushBinaryBuffer(codewords, ref binaryBuffer, ref binaryBufferSize);
            }
        }

        //-------------------------------------------------------------------------
        // "RSEncode(nd,nc, wd)" adds "nc" R-S check words to "nd" data words in wd[]
        // employing Galois Field GF, where GF is prime, with a prime modulus of PM
        //-------------------------------------------------------------------------

        private void RSEncode(int nd, int nc, byte[] wd)
        {
            int i, j, k, nw, start, step;
            int ND, NW, NC;
            int[] root = new int[GF];
            int[] c = new int[GF];

            // Start by generating "nc" roots (antilogs).
            root[0] = 1;
            for (i = 1; i <= nc && (i < GF); i++)
            {
                root[i] = (PM * root[i - 1]) % GF;
            }

            // Here we compute how many interleaved R-S blocks will be needed.
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
                {
                    c[i] = 0;
                }

                c[0] = 1;

                for (i = 1; i <= NC; i++)
                {
                    for (j = NC; j >= 1; j--)
                    {
                        c[j] = (GF + c[j] - (root[i] * c[j - 1]) % GF) % GF;
                    }
                }

                // And then compute the corresponding checkword values into wd[]
                // ... (a) starting at wd[start] & (b) stepping by step
                for (i = ND; i < NW; i++)
                {
                    wd[start + i * step] = 0;
                }

                for (i = 0; i < ND; i++)
                {
                    k = (wd[start + i * step] + wd[start + ND * step]) % GF;
                    for (j = 0; j < NC - 1; j++)
                    {
                        wd[start + (ND + j) * step] = (byte)((GF - ((c[j + 1] * k) % GF) + wd[start + (ND + j + 1) * step]) % GF);
                    }

                    wd[start + (ND + NC - 1) * step] = (byte)((GF - ((c[NC] * k) % GF)) % GF);
                }

                for (i = ND; i < NW; i++)
                {
                    wd[start + i * step] = (byte)((GF - wd[start + i * step]) % GF);
                }
            }
        }
    }
}
