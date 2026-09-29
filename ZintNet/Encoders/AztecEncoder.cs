/* AztecEncoder.cs - Handles encoding of Aztec & Aztec Runes 2D symbols */

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


using System.Globalization;
using System.Collections.ObjectModel;


namespace ZintNet.Encoders
{
    internal class AztecEncoder : SymbolEncoder
    {
        #region Tables.

        // 27 x 27 data grid.
        private readonly int[] CompactAztecMap = {
            609, 608, 411, 413, 415, 417, 419, 421,  423, 425,  427,  429,  431,  433,  435,  437,  439, 441,  443, 445, 447, 449, 451, 453, 455, 457, 459, /* 0 */
            607, 606, 410, 412, 414, 416, 418, 420,  422, 424,  426,  428,  430,  432,  434,  436,  438, 440,  442, 444, 446, 448, 450, 452, 454, 456, 458, /* 1 */
            605, 604, 409, 408, 243, 245, 247, 249,  251, 253,  255,  257,  259,  261,  263,  265,  267, 269,  271, 273, 275, 277, 279, 281, 283, 460, 461, /* 2 */
            603, 602, 407, 406, 242, 244, 246, 248,  250, 252,  254,  256,  258,  260,  262,  264,  266, 268,  270, 272, 274, 276, 278, 280, 282, 462, 463, /* 3 */
            601, 600, 405, 404, 241, 240, 107, 109,  111, 113,  115,  117,  119,  121,  123,  125,  127, 129,  131, 133, 135, 137, 139, 284, 285, 464, 465, /* 4 */
            599, 598, 403, 402, 239, 238, 106, 108,  110, 112,  114,  116,  118,  120,  122,  124,  126, 128,  130, 132, 134, 136, 138, 286, 287, 466, 467, /* 5 */
            597, 596, 401, 400, 237, 236, 105, 104,    3,   5,    7,    9,   11,   13,   15,   17,   19,  21,   23,  25,  27, 140, 141, 288, 289, 468, 469, /* 6 */
            595, 594, 399, 398, 235, 234, 103, 102,    2,   4,    6,    8,   10,   12,   14,   16,   18,  20,   22,  24,  26, 142, 143, 290, 291, 470, 471, /* 7 */
            593, 592, 397, 396, 233, 232, 101, 100,    1,   1, 2000, 2001, 2002, 2003, 2004, 2005, 2006,   0,    1,  28,  29, 144, 145, 292, 293, 472, 473, /* 8 */
            591, 590, 395, 394, 231, 230,  99,  98,    1,   1,    1,    1,    1,    1,    1,    1,    1,   1,    1,  30,  31, 146, 147, 294, 295, 474, 475, /* 9 */
            589, 588, 393, 392, 229, 228,  97,  96, 2027,   1,    0,    0,    0,    0,    0,    0,    0,   1, 2007,  32,  33, 148, 149, 296, 297, 476, 477, /* 10 */
            587, 586, 391, 390, 227, 226,  95,  94, 2026,   1,    0,    1,    1,    1,    1,    1,    0,   1, 2008,  34,  35, 150, 151, 298, 299, 478, 479, /* 11 */
            585, 584, 389, 388, 225, 224,  93,  92, 2025,   1,    0,    1,    0,    0,    0,    1,    0,   1, 2009,  36,  37, 152, 153, 300, 301, 480, 481, /* 12 */
            583, 582, 387, 386, 223, 222,  91,  90, 2024,   1,    0,    1,    0,    1,    0,    1,    0,   1, 2010,  38,  39, 154, 155, 302, 303, 482, 483, /* 13 */
            581, 580, 385, 384, 221, 220,  89,  88, 2023,   1,    0,    1,    0,    0,    0,    1,    0,   1, 2011,  40,  41, 156, 157, 304, 305, 484, 485, /* 14 */
            579, 578, 383, 382, 219, 218,  87,  86, 2022,   1,    0,    1,    1,    1,    1,    1,    0,   1, 2012,  42,  43, 158, 159, 306, 307, 486, 487, /* 15 */
            577, 576, 381, 380, 217, 216,  85,  84, 2021,   1,    0,    0,    0,    0,    0,    0,    0,   1, 2013,  44,  45, 160, 161, 308, 309, 488, 489, /* 16 */
            575, 574, 379, 378, 215, 214,  83,  82,    0,   1,    1,    1,    1,    1,    1,    1,    1,   1,    1,  46,  47, 162, 163, 310, 311, 490, 491, /* 17 */
            573, 572, 377, 376, 213, 212,  81,  80,    0,   0, 2020, 2019, 2018, 2017, 2016, 2015, 2014,   0,    0,  48,  49, 164, 165, 312, 313, 492, 493, /* 18 */
            571, 570, 375, 374, 211, 210,  78,  76,   74,  72,   70,   68,   66,   64,   62,   60,   58,  56,   54,  50,  51, 166, 167, 314, 315, 494, 495, /* 19 */
            569, 568, 373, 372, 209, 208,  79,  77,   75,  73,   71,   69,   67,   65,   63,   61,   59,  57,   55,  52,  53, 168, 169, 316, 317, 496, 497, /* 20 */
            567, 566, 371, 370, 206, 204, 202, 200,  198, 196,  194,  192,  190,  188,  186,  184,  182, 180,  178, 176, 174, 170, 171, 318, 319, 498, 499, /* 21 */
            565, 564, 369, 368, 207, 205, 203, 201,  199, 197,  195,  193,  191,  189,  187,  185,  183, 181,  179, 177, 175, 172, 173, 320, 321, 500, 501, /* 22 */
            563, 562, 366, 364, 362, 360, 358, 356,  354, 352,  350,  348,  346,  344,  342,  340,  338, 336,  334, 332, 330, 328, 326, 322, 323, 502, 503, /* 23 */
            561, 560, 367, 365, 363, 361, 359, 357,  355, 353,  351,  349,  347,  345,  343,  341,  339, 337,  335, 333, 331, 329, 327, 324, 325, 504, 505, /* 24 */
            558, 556, 554, 552, 550, 548, 546, 544,  542, 540,  538,  536,  534,  532,  530,  528,  526, 524,  522, 520, 518, 516, 514, 512, 510, 506, 507, /* 25 */
            559, 557, 555, 553, 551, 549, 547, 545,  543, 541,  539,  537,  535,  533,  531,  529,  527, 525,  523, 521, 519, 517, 515, 513, 511, 508, 509, /* 26 */
            /* 0   1    2    3    4    5    6    7     8    9    10    11    12    13    14    15    16   17    18   19   20   21   22   23   24   25   26 */ };

        // Pre-calculated finder, descriptor, orientation mappings for full-range symbol.
        private readonly int[,] AztecMapCore = {
            {     1,     1, 20000, 20001, 20002, 20003, 20004,     0, 20005, 20006, 20007, 20008, 20009,     0,     1, },
            {     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1, },
            { 20039,     1,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     1, 20010, },
            { 20038,     1,     0,     1,     1,     1,     1,     1,     1,     1,     1,     1,     0,     1, 20011, },
            { 20037,     1,     0,     1,     0,     0,     0,     0,     0,     0,     0,     1,     0,     1, 20012, },
            { 20036,     1,     0,     1,     0,     1,     1,     1,     1,     1,     0,     1,     0,     1, 20013, },
            { 20035,     1,     0,     1,     0,     1,     0,     0,     0,     1,     0,     1,     0,     1, 20014, },
            {     0,     1,     0,     1,     0,     1,     0,     1,     0,     1,     0,     1,     0,     1,     0, },
            { 20034,     1,     0,     1,     0,     1,     0,     0,     0,     1,     0,     1,     0,     1, 20015, },
            { 20033,     1,     0,     1,     0,     1,     1,     1,     1,     1,     0,     1,     0,     1, 20016, },
            { 20032,     1,     0,     1,     0,     0,     0,     0,     0,     0,     0,     1,     0,     1, 20017, },
            { 20031,     1,     0,     1,     1,     1,     1,     1,     1,     1,     1,     1,     0,     1, 20018, },
            { 20030,     1,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     1, 20019, },
            {     0,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1,     1, },
            {     0,     0, 20029, 20028, 20027, 20026, 20025,     0, 20024, 20023, 20022, 20021, 20020,     0,     0, }, };

        // From Table 2.
        private readonly int[] AztecSymbolCharacter = {
             0,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13,  0, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 15, 16, 17, 18, 19,
             1,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16,  0, 18,  0, 20,
             2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 21, 22, 23, 24, 25, 26,
            20,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 27, 21, 28, 22, 23,
            24,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 29, 25, 30, 26, 27 };
        private readonly string AztecModes = "BMMMMMMMMMMMMXBBBBBBBBBBBBBMMMMMXPPPPPPPPPPPXPXPDDDDDDDDDDPPPPPPMUUUUUUUUUUUUUUUUUUUUUUUUUUPMPMMMLLLLLLLLLLLLLLLLLLLLLLLLLLPMPMM";

        // Codewords per symbol.
        private readonly int[] AztecSizes = {
                  21,  48,  60,  88, 120,  156, 196, 240,  230,  272,  316,  364,  416,  470,  528,  588,
                 652, 720, 790, 864, 940, 1020, 920, 992, 1066, 1144, 1224, 1306, 1392, 1480, 1570, 1664 };

        private readonly int[] AztecCompactSizes = { 17, 40, 51, 64 };


        private readonly short[][] AztecDataSizes = {
            new short[] {
            /* Data bits per symbol maximum with 10% error correction */
              96,  246,  408,  616,  840, 1104, 1392,  1704,  2040,  2420,  2820,  3250,  3720,  4200,  4730,  5270,
            5840, 6450, 7080, 7750, 8430, 9150, 9900, 10680, 11484, 12324, 13188, 14076, 15000, 15948, 16920, 17940 },
            new short[] {
            /* Data bits per symbol maximum with 23% error correction */
              84,  204,  352,  520,  720,  944, 1184,  1456,  1750,  2070,  2410,  2780,  3180,  3590,  4040,  4500,
            5000, 5520, 6060, 6630, 7210, 7830, 8472,  9132,  9816, 10536, 11280, 12036, 12828, 13644, 14472, 15348
             },
            new short[] {
            /* Data bits per symbol maximum with 36% error correction */
              66,  168,  288,  432,  592,  776,  984,  1208,  1450,  1720,  2000,  2300,  2640,  2980,  3350,  3740,
            4150, 4580, 5030, 5500, 5990, 6500, 7032,  7584,  8160,  8760,  9372,  9996, 10656, 11340, 12024, 12744 },
            new short[] {
            /* Data bits per symbol maximum with 50% error correction */
              48,  126,  216,  328,  456,  600,  760,   936,  1120,  1330,  1550,  1790,  2050,  2320,  2610,  2910,
            3230, 3570, 3920, 4290, 4670, 5070, 5484,  5916,  6360,  6828,  7308,  7800,  8316,  8844,  9384,  9948  } };

        private readonly short[][] AztecCompactDataSizes = {
            new short[] {
            // Data bits per symbol maximum with 10% error correction.
            78, 198, 336, 512 },
            new short[] {
            // Data bits per symbol maximum with 23% error correction.
            66, 168, 288, 440  },
            new short[] {
            // Data bits per symbol maximum with 36% error correction.
            48, 138, 232, 360 },
            new short[] {
            // Data bits per symbol maximum with 50% error correction.
            36, 102, 176, 280 } };


        // Reference grid offsets.
        private readonly int[] AztecOffset = {
            66, 64, 62, 60, 57, 55, 53, 51, 49, 47, 45, 42, 40, 38, 36, 34,
            32, 30, 28, 25, 23, 21, 19, 17, 15, 13, 10,  8,  6,  4,  2,  0 };

        private readonly int[] AztecCompactOffset = { 6, 4, 2, 0 };
        private readonly int[] AztecMap = new int[AZTEC_MAP_SIZE];

        #endregion

        #region Constants.

        private const int AZTEC_MAX_CAPACITY = 19968;
        private const int AZTEC_MAP_SIZE = 22801;
        private const int AZTEC_BIN_CAPACITY = 19932;
        //private const int CompactLoop = 4;

        #endregion

        private readonly int optionErrorCorrection;
        private readonly int optionSymbolSize;

        public AztecEncoder(Symbology symbolId, char[] barcodeMessage, int optionSymbolSize, int optionErrorCorrection, int eci, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionErrorCorrection = optionErrorCorrection;
            this.optionSymbolSize = optionSymbolSize;
            this.eci = eci;
            this.encodingMode = encodingMode;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (symbolId)
            {
                case Symbology.Aztec:
                    switch (encodingMode)
                    {
                        case EncodingFormat.Standard:
                            isGS1 = false;
                            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                            Aztec();
                            break;

                        case EncodingFormat.GS1:
                            isGS1 = true;
                            barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                            Aztec();
                            break;

                        case EncodingFormat.HIBC:
                            isGS1 = false;
                            barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                            Aztec();
                            break;
                    }
                    break;

                case Symbology.AztecRunes:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    AztecRunes();
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// Generates an Aztec (ISO 24778) symbol.
        /// </summary>
        private void Aztec()
        {
            int dataBlocks, eccBlocks;
            bool compact;
            int layers;
            int codewordSize;
            int inputLength = barcodeData.Length;
            int adjustedLength;
            int padBits;
            int remainder;
            BitVector binaryStream = new BitVector();
            BitVector bitPattern = new BitVector();
            byte[] dataDescriptor = new byte[4];
            byte[] eccDescriptor = new byte[6];
            byte[] descriptor = new byte[40];

            PopulateMap();
            if (!AztecTextProcess(barcodeData, inputLength, binaryStream))
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Aztec Code: Input too long, requires too many codewords (maximum 1661)."));
            }

            int dataLength = binaryStream.SizeInBits;
            int dataMaxSize = 0;
            int adjustmentSize = 0;
            int eccLevel = optionErrorCorrection;

             if (optionSymbolSize == 0)    // Auto resizing.
            {
                if (optionErrorCorrection == -1)
                {
                    eccLevel = 2;
                }

                do
                {
                    // Decide what size symbol to use - the smallest that fits the data.
                    compact = false;
                    layers = 0;

                    // For each level of error correction work out the smallest symbol which the data will fit in.
                    for (int i = 4; i > 0; i--)
                    {
                        if ((dataLength + adjustmentSize) <= AztecCompactDataSizes[eccLevel - 1][i - 1])
                        {
                            layers = i;
                            compact = true;
                            dataMaxSize = AztecCompactDataSizes[eccLevel - 1][i - 1];
                        }
                    }

                    if (!compact)
                    {
                        for (int i = 32; i > 0; i--)
                        {
                            if ((dataLength + adjustmentSize) <= AztecDataSizes[eccLevel - 1][i - 1])
                            {
                                layers = i;
                                compact = false;
                                dataMaxSize = AztecDataSizes[eccLevel - 1][i - 1];
                            }
                        }
                    }

                    if (layers == 0)
                    {
                        // Couldn't find a symbol which fits the data.
                        if (adjustmentSize == 0)
                        {
                            throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                                "Aztec Code: Input too long for ECC level {0}.\nRequires too many codewords, maximum available {1}.", eccLevel, AztecDataSizes[eccLevel - 1][31] / 12));
                        }

                        throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                            "Aztec Code: Input too long for ECC level {0}.\nRequires {1} codewords, maximum available {2}.",
                            eccLevel, (dataLength + adjustmentSize + 11) / 12, AztecDataSizes[eccLevel - 1][31] / 12));
                    }

                    aztecAutoSize = layers;
                    codewordSize = CodewordSize(layers);
                    adjustedLength = AdjustBinaryData(binaryStream, codewordSize, dataMaxSize);

                    if (adjustedLength == 0)
                    {
                        throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                            "Aztec Code: Input too long for ECC level {0}, requires too many codewords (maximum{1})",
                            eccLevel, (adjustmentSize > 0 ? dataMaxSize : AZTEC_BIN_CAPACITY) / codewordSize));
                    }

                    adjustmentSize = adjustedLength - dataLength;

                    // Add padding.
                    remainder = adjustedLength % codewordSize;
                    padBits = codewordSize - remainder;
                    if (padBits == codewordSize)
                    {
                        padBits = 0;
                    }

                    adjustedLength = AddPadding(binaryStream, padBits, codewordSize);
                } while (adjustedLength > dataMaxSize);
                /* Note: This loop will only repeat on the rare occasions when the rule about not having all 1s or all 0s
                means that the binary string has had to be lengthened beyond the maximum number of bits that can
                be encoded in a symbol of the selected size.*/
            }

            else
            {
                // The size of the symbol has been specified by the user.
                if (optionSymbolSize <= 4)
                {
                    compact = true;
                    layers = optionSymbolSize;
                }

                else
                {
                    compact = false;
                    layers = optionSymbolSize - 4;
                }

                codewordSize = CodewordSize(layers);

                // Check if the data actually fits into the selected symbol size.
                if (compact)
                {
                    dataMaxSize = codewordSize * (AztecCompactSizes[layers - 1] - 3);
                }

                else
                {
                    dataMaxSize = codewordSize * (AztecSizes[layers - 1] - 3);
                }

                adjustedLength = AdjustBinaryData(binaryStream, codewordSize, dataMaxSize);
                if ( adjustedLength == 0)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Aztec Code: Input too long for Version {0}.\nRequires too many codewords (maximum {1}).", optionSymbolSize, dataMaxSize / codewordSize));
                }

                // Add padding.
                remainder = adjustedLength % codewordSize;
                padBits = codewordSize - remainder;
                if (padBits == codewordSize)
                {
                    padBits = 0;
                }

                if (adjustedLength + padBits > dataMaxSize)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Aztec Code: Input too long for Version {0}.\nRequires {1} codewords (maximum {2}).",
                        optionSymbolSize, (dataLength + padBits) / codewordSize, dataMaxSize / codewordSize));
                }

                adjustedLength = AddPadding(binaryStream, padBits, codewordSize);
            }

            dataBlocks = adjustedLength / codewordSize;

            if (compact)
            {
                eccBlocks = AztecCompactSizes[layers - 1] - dataBlocks;
                if (layers == 4)
                {
                    // Can use spare blocks for ECC (76 available - 64 max data blocks).
                    eccBlocks += 12;
                }
            }

            else
            {
                eccBlocks = AztecSizes[layers - 1] - dataBlocks;
            }

            if (eccBlocks < dataBlocks / 20)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Aztec Code: Number of ECC codewords {0} less than {1} (5% of data codewords {3}).", eccBlocks, dataBlocks / 20, dataBlocks));
            }

                uint[] dataCodewords = new uint[dataBlocks];
            uint[] eccCodewords = new uint[eccBlocks];

            // Split into codewords and calculate reed-solomon error correction codes.
            for (int i = 0; i < dataBlocks; i++)
            {
                for (int p = 0; p < codewordSize; p++)
                {
                    if (binaryStream[i * codewordSize + p] == 1)
                    {
                        dataCodewords[i] += (uint)(0x01 << (codewordSize - (p + 1)));
                    }
                }
            }

            switch (codewordSize)
            {
                case 6:
                    ReedSolomon.RSInitialise(0x43, eccBlocks, 1);
                    ReedSolomon.RSEncode(dataBlocks, dataCodewords, eccCodewords);
                    for (int i = (eccBlocks - 1); i >= 0; i--)
                    {
                        binaryStream.AppendBits((int)eccCodewords[i], 6);
                    }

                    break;

                case 8:
                    ReedSolomon.RSInitialise(0x12d, eccBlocks, 1);
                    ReedSolomon.RSEncode(dataBlocks, dataCodewords, eccCodewords);
                    for (int i = (eccBlocks - 1); i >= 0; i--)
                    {
                        binaryStream.AppendBits((int)eccCodewords[i], 8);
                    }

                    break;

                case 10:
                    ReedSolomon.RSInitialise(0x409, eccBlocks, 1);
                    ReedSolomon.RSEncode(dataBlocks, dataCodewords, eccCodewords);
                    for (int i = (eccBlocks - 1); i >= 0; i--)
                    {
                        binaryStream.AppendBits((int)eccCodewords[i], 10);
                    }

                    break;

                case 12:
                    ReedSolomon.RSInitialise(0x1069, eccBlocks, 1);
                    ReedSolomon.RSEncode(dataBlocks, dataCodewords, eccCodewords);
                    for (int i = (eccBlocks - 1); i >= 0; i--)
                    {
                        binaryStream.AppendBits((int)eccCodewords[i], 12);
                    }

                    break;
            }

            int totalBits = (dataBlocks + eccBlocks) * codewordSize;

            // Check our encoding is correct.
            if (totalBits != binaryStream.SizeInBits)
            {
                 throw new DataEncodingException("Aztec Code: Bitstream size error.");
            }

            // Invert the data so that actual data is on the outside and reed-solomon on the inside.
            for (int i = 0; i < totalBits; i++)
            {
                bitPattern.AppendBit(binaryStream[totalBits - i - 1]);
            }

            // Add the symbol descriptor.
            if (compact)
            {
                // The first 2 bits represent the number of layers minus 1.
                descriptor[0] = (((layers - 1) & 0x02) != 0) ? (byte)1 : (byte)0;
                descriptor[1] = (((layers - 1) & 0x01) != 0) ? (byte)1 : (byte)0;

                // The next 6 bits represent the number of data blocks minus 1.
                for (int x = 0; x < 6; x++)
                {
                    descriptor[2 + x] = (((dataBlocks - 1) & (0x20 >> x)) != 0) ? (byte)1 : (byte)0;
                }
            }

            else
            {
                // The first 5 bits represent the number of layers minus 1.
                for (int x = 0; x < 5; x++)
                {
                    descriptor[x] = (((layers - 1) & (0x10 >> x)) != 0) ? (byte)1 : (byte)0;
                }

                // The next 11 bits represent the number of data blocks minus 1.
                for (int x = 0; x < 11; x++)
                {
                    descriptor[5 + x] = (((dataBlocks - 1) & (0x400 >> x)) != 0) ? (byte)1 : (byte)0;
                }
            }

            // Split into 4 x 4-bit codewords.
            // Add reed-solomon error correction with Galois field 
            // GF(16) and prime modulus: x^4 + x + 1 (section 7.2.3)

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (descriptor[(i * 4) + j] == 1)
                    {
                        dataDescriptor[i] += (byte)(8 >> j);
                    }
                }
            }

            if (compact)
            {
                ReedSolomon.RSInitialise(0x13, 5, 1);
                ReedSolomon.RSEncode(2, dataDescriptor, eccDescriptor);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        descriptor[(i * 4) + 8 + j] = ((eccDescriptor[4 - i] & (0x08 >> j)) != 0) ? (byte)1 : (byte)0;
                    }
                }
            }

            else
            {
                ReedSolomon.RSInitialise(0x13, 6, 1);
                ReedSolomon.RSEncode(4, dataDescriptor, eccDescriptor);
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        descriptor[(i * 4) + 16 + j] = ((eccDescriptor[5 - i] & (0x08 >> j)) != 0) ? (byte)1 : (byte)0;
                    }
                }
            }

            // Merge descriptor with the rest of the symbol.
            int descriptorOffest = compact ? 1998 : 19998;
            int offsetCount = descriptorOffest - binaryStream.SizeInBits;
            for (int i = 0; i < offsetCount; i++)
            {
                bitPattern.AppendBit(0);
            }

            for (int i = 0; i < 40; i++)
            {
                bitPattern.AppendBit((descriptor[i] == 1) ? (byte)1 : (byte)0);
            }

            // Expand the row pattern into the symbol data.
            BuildSymbolData(bitPattern, layers, compact);
        }

        /// <summary>
        /// Determine codeword bitlength - Table 3
        /// </summary>
        /// <param name="layers">Number of layers.</param>
        /// <returns>The size of the codeword in bits.</returns>
        private int CodewordSize(int layers)
        {
            int codewordSize;

            if (layers <= 2)
            {
                codewordSize = 6;
            }

            else if (layers <= 8)
            {
                codewordSize = 8;
            }

            else if (layers <= 22)
            {
                codewordSize = 10;
            }

            else
            {
                codewordSize = 12;
            }

            return codewordSize;
        }

        /// <summary>
        /// Adjusts the binary data and add padding.
        /// </summary>
        /// <param name="binaryData">Bit stream.</param>
        /// <param name="codewordSize">Number of bits in the codeword.</param>
        /// <param name="dataMaxSize"></param>
        private int AdjustBinaryData(BitVector binaryData, int codewordSize, int dataMaxSize)
        {
            int length = binaryData.SizeInBits;
            int index = 0;
            int count = 0;

            for (int i = 0; i < length; i++)
            {
                if ((index + 1) % codewordSize == 0)
                {
                    // Last bit of codeword.
                    // 7.3.1.2 "whenever the first B-1 bits ... are all “0”s, then a dummy “1” is inserted..."
                    // "Similarly a message codeword that starts with B-1 “1”s has a dummy “0” inserted..."

                    if (count == (codewordSize - 1))     // Codeword of all '1's
                    {
                        binaryData.Insert(index, 0);
                    }

                    if (count == 0)                         // Codeword of all '0's
                    {
                        binaryData.Insert(index, 1);
                    }

                    if (index > dataMaxSize)
                    {
                        return 0;
                    }

                    count = 0;
                }

                else if (binaryData[i] == 1)    // Skip B so only counting B-1.
                {
                    count++;
                }

                if (index > dataMaxSize)
                {
                    return 0;
                }

                index++;
            }

            return binaryData.SizeInBits;
        }

        /// <summary>
        /// Adds the neccesary padding to the data stream.
        /// </summary>
        /// <param name="binaryData">Data stream to append the padding to.</param>
        /// <param name="padBits">Number of padding bits to append to the data stream.</param>
        /// <param name="codewordSize">Codeword size in bits.</param>
        private int AddPadding(BitVector binaryData, int padBits, int codewordSize)
        {
            int count;
            int length = binaryData.SizeInBits;

            binaryData.AppendBits(0xffff, padBits);
            length += padBits;
            count = 0;

            for (int i = (length - codewordSize); i < length; i++)
            {
                if (binaryData[i] == 1)
                {
                    count++;
                }
            }

            if (count == codewordSize)
            {
                binaryData[length - 1] = 0;
            }

            return binaryData.SizeInBits;
        }

        private bool AztecTextProcess(char[] source, int length, BitVector binaryStream)
        {
            char[] encodeMode = new char[length];
            char[] reducedSource = new char[length];
            char[] reducedEncodeMode = new char[length];
            char currentMode;
            int count;
            int l;
            int k;
            char nextMode;
            int reducedLength;
            bool byteMode = false;

            for (int i = 0; i < length; i++)
            {
                if (source[i] >= 128)
                {
                    encodeMode[i] = 'B';
                }

                else if(isGS1 && source[i] == '\x1d')
                {
                    encodeMode[i] = 'P';
                }

                else
                {
                    encodeMode[i] = AztecModes[source[i]];
                }
            }

            // Deal first with letter combinations which can be combined to one codeword
            // Combinations are (CR LF) (. SP) (, SP) (: SP) in Punct mode
            currentMode = 'U';
            for (int i = 0; i < length - 1; i++)
            {
                // Combination (CR LF) should always be in Punct mode
                if ((source[i] == 13) && (source[i + 1] == 10))
                {
                    encodeMode[i] = 'P';
                    encodeMode[i + 1] = 'P';
                }

                // Combination (: SP) should always be in Punct mode
                else if ((source[i] == ':') && (source[i + 1] == ' '))
                {
                    encodeMode[i + 1] = 'P';
                }

                // Combinations (. SP) and (, SP) sometimes use fewer bits in Digit mode
                else if (((source[i] == '.') || (source[i] == ',')) && (source[i + 1] == ' ') && (encodeMode[i] == 'X'))
                {
                    count = CountDoubles(source, i, length);
                    nextMode = GetNextMode(encodeMode, length, i);

                    if (currentMode == 'U')
                    {
                        if ((nextMode == 'D') && (count <= 5))
                        {
                            for (int j = 0; j < (2 * count); j++)
                            {
                                encodeMode[i + j] = 'D';
                            }
                        }
                    }

                    else if (currentMode == 'L')
                    {

                        if ((nextMode == 'D') && (count <= 4))
                        {
                            for (int j = 0; j < (2 * count); j++)
                            {
                                encodeMode[i + j] = 'D';
                            }
                        }
                    }

                    else if (currentMode == 'M')
                    {
                        if ((nextMode == 'D') && (count == 1))
                        {
                            encodeMode[i] = 'D';
                            encodeMode[i + 1] = 'D';
                        }
                    }

                    else if (currentMode == 'D')
                    {
                        if ((nextMode != 'D') && (count <= 4))
                        {
                            for (int j = 0; j < (2 * count); j++)
                            {
                                encodeMode[i + j] = 'D';
                            }
                        }

                        else if ((nextMode == 'D') && (count <= 7))
                        {
                            for (int j = 0; j < (2 * count); j++)
                            {
                                encodeMode[i + j] = 'D';
                            }
                        }
                    }

                    // Default is Punct mode.
                    if (encodeMode[i] == 'X')
                    {
                        encodeMode[i] = 'P';
                        encodeMode[i + 1] = 'P';
                    }
                }

                if ((encodeMode[i] != 'X') && (encodeMode[i] != 'B'))
                {
                    currentMode = encodeMode[i];
                }
            }

            // Reduce two letter combinations to one codeword marked as [abcd] in Punct mode.
            k = 0;
            l = 0;
            while (l < length)
            {
                reducedEncodeMode[k] = encodeMode[l];
                if (l + 1 < length)
                {
                    if ((source[l] == 13) && (source[l + 1] == 10))
                    {
                        // CR LF
                        reducedSource[k] = 'a';
                        l += 2;
                    }

                    else if (((source[l] == '.') && (source[l + 1] == ' ')) && (encodeMode[l] == 'P'))
                    {
                        reducedSource[k] = 'b';
                        l += 2;
                    }

                    else if (((source[l] == ',') && (source[l + 1] == ' ')) && (encodeMode[l] == 'P'))
                    {
                        reducedSource[k] = 'c';
                        l += 2;
                    }

                    else if ((source[l] == ':') && (source[l + 1] == ' '))
                    {
                        reducedSource[k] = 'd';
                        l += 2;
                    }

                    else
                    {
                        reducedSource[k] = source[l++];
                    }
                }

                else
                {
                    reducedSource[k] = source[l++];
                }

                k++;
            }

            reducedLength = k;

            currentMode = 'U';
            for (int i = 0; i < reducedLength; i++)
            {
                // Resolve Carriage Return (CR) which can be Punct or Mixed mode
                if (reducedSource[i] == 13)
                {
                    count = CountChar(reducedSource, i, reducedLength, '\r');
                    nextMode = GetNextMode(reducedEncodeMode, reducedLength, i);

                    if ((currentMode == 'U') && ((nextMode == 'U') || (nextMode == 'B')) && (count == 1))
                    {
                        reducedEncodeMode[i] = 'P';
                    }

                    else if ((currentMode == 'L') && ((nextMode == 'L') || (nextMode == 'B')) && (count == 1))
                    {
                        reducedEncodeMode[i] = 'P';
                    }

                    else if ((currentMode == 'P') || (nextMode == 'P'))
                    {
                        reducedEncodeMode[i] = 'P';
                    }

                    if (currentMode == 'D')
                    {
                        if (((nextMode == 'E') || (nextMode == 'U') || (nextMode == 'D') || (nextMode == 'B')) && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'P';
                            }
                        }

                        else if ((nextMode == 'L') && (count == 1))
                        {
                            reducedEncodeMode[i] = 'P';
                        }
                    }

                    // Default is Mixed mode
                    if (reducedEncodeMode[i] == 'X')
                    {
                        reducedEncodeMode[i] = 'M';
                    }
                }

                // Resolve full stop and comma which can be in Punct or Digit mode
                else if ((reducedSource[i] == '.') || (reducedSource[i] == ','))
                {
                    count = CountDotComma(reducedSource, i, reducedLength);
                    nextMode = GetNextMode(reducedEncodeMode, reducedLength, i);

                    if (currentMode == 'U')
                    {
                        if (((nextMode == 'U') || (nextMode == 'L') || (nextMode == 'M') || (nextMode == 'B')) && (count == 1))
                        {
                            reducedEncodeMode[i] = 'P';
                        }
                    }

                    else if (currentMode == 'L')
                    {
                        if ((nextMode == 'L') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'P';
                            }
                        }

                        else if (((nextMode == 'M') || (nextMode == 'B')) && (count == 1))
                        {
                            reducedEncodeMode[i] = 'P';
                        }
                    }

                    else if (currentMode == 'M')
                    {
                        if (((nextMode == 'E') || (nextMode == 'U') || (nextMode == 'L') || (nextMode == 'M')) && (count <= 4))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'P';
                            }
                        }

                        else if ((nextMode == 'B') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'P';
                            }
                        }
                    }

                    else if ((currentMode == 'P') && (nextMode != 'D') && (count <= 9))
                    {
                        for (int j = 0; j < count; j++)
                        {
                            reducedEncodeMode[i + j] = 'P';
                        }
                    }

                    // Default is Digit mode
                    if (reducedEncodeMode[i] == 'X')
                    {
                        reducedEncodeMode[i] = 'D';
                    }
                }

                // Resolve Space (SP) which can be any mode except Punct.
                else if (reducedSource[i] == ' ')
                {
                    count = CountChar(reducedSource, i, reducedLength, ' ');
                    nextMode = GetNextMode(reducedEncodeMode, reducedLength, i);

                    if (currentMode == 'U')
                    {
                        if ((nextMode == 'E') && (count <= 5))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'U';
                            }
                        }

                        else if (((nextMode == 'U') || (nextMode == 'L') || (nextMode == 'M') || (nextMode == 'P') || (nextMode == 'B')) && (count <= 9))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'U';
                            }
                        }
                    }

                    if (currentMode == 'L')
                    {
                        if ((nextMode == 'E') && (count <= 5))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'L';
                            }
                        }

                        else if ((nextMode == 'U') && (count == 1))
                        {
                            reducedEncodeMode[i] = 'L';
                        }

                        else if ((nextMode == 'L') && (count <= 14))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'L';
                            }
                        }

                        else if (((nextMode == 'M') || (nextMode == 'P') || (nextMode == 'B')) && (count <= 9))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'L';
                            }
                        }
                    }

                    else if (currentMode == 'M')
                    {
                        if (((nextMode == 'E') || (nextMode == 'U')) && (count <= 9))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'M';
                            }
                        }

                        else if (((nextMode == 'L') || (nextMode == 'B')) && (count <= 14))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'M';
                            }
                        }

                        else if (((nextMode == 'M') || (nextMode == 'P')) && (count <= 19))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'M';
                            }
                        }
                    }

                    else if (currentMode == 'P')
                    {
                        if ((nextMode == 'E') && (count <= 5))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'U';
                            }
                        }

                        else if (((nextMode == 'U') || (nextMode == 'L') || (nextMode == 'M') || (nextMode == 'P') || (nextMode == 'B')) && (count <= 9))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'U';
                            }
                        }
                    }

                    // Default is Digit mode
                    if (reducedEncodeMode[i] == 'X')
                    {
                        reducedEncodeMode[i] = 'D';
                    }
                }

                if (reducedEncodeMode[i] != 'B')
                {
                    currentMode = reducedEncodeMode[i];
                }
            }

            // Decide when to use P/S instead of P/L and U/S instead of U/L
            currentMode = 'U';
            for (int i = 0; i < reducedLength; i++)
            {
                if (reducedEncodeMode[i] != currentMode)
                {
                    for (count = 0; ((i + count) < reducedLength) && (reducedEncodeMode[i + count] == reducedEncodeMode[i]); count++)
                    {
                        ;
                    }

                    nextMode = GetNextMode(reducedEncodeMode, reducedLength, i);
                    if (reducedEncodeMode[i] == 'P')
                    {
                        if ((currentMode == 'U') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'p';
                            }
                        }

                        else if ((currentMode == 'L') && (nextMode != 'U') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'p';
                            }
                        }

                        else if ((currentMode == 'L') && (nextMode == 'U') && (count == 1))
                        {
                            reducedEncodeMode[i] = 'p';
                        }

                        else if ((currentMode == 'M') && (nextMode != 'M') && (count == 1))
                        {
                            reducedEncodeMode[i] = 'p';
                        }

                        else if ((currentMode == 'M') && (nextMode == 'M') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'p';
                            }
                        }

                        else if ((currentMode == 'D') && (nextMode != 'D') && (count <= 3))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'p';
                            }
                        }

                        else if ((currentMode == 'D') && (nextMode == 'D') && (count <= 6))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'p';
                            }
                        }
                    }

                    else if (reducedEncodeMode[i] == 'U')
                    {
                        if ((currentMode == 'L') && ((nextMode == 'L') || (nextMode == 'M')) && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'u';
                            }
                        }

                        else if ((currentMode == 'L') && ((nextMode == 'E') || (nextMode == 'D') || (nextMode == 'B') || (nextMode == 'P')) && (count == 1))
                        {
                            reducedEncodeMode[i] = 'u';
                        }

                        else if ((currentMode == 'D') && (nextMode == 'D') && (count == 1))
                        {
                            reducedEncodeMode[i] = 'u';
                        }

                        else if ((currentMode == 'D') && (nextMode == 'P') && (count <= 2))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                reducedEncodeMode[i + j] = 'u';
                            }
                        }
                    }
                }

                if ((reducedEncodeMode[i] != 'p') && (reducedEncodeMode[i] != 'u') && (reducedEncodeMode[i] != 'B'))
                {
                    currentMode = reducedEncodeMode[i];
                }
            }

            if (binaryStream.SizeInBits == 0 && isGS1)
            {
                binaryStream.AppendBits(0, 5); // P/S
                binaryStream.AppendBits(0, 5); // FLG(n)
                binaryStream.AppendBits(0, 3); // FLG(0)
            }

            if (eci != 0)
            {
                binaryStream.AppendBits(0, 5); // P/S
                binaryStream.AppendBits(0, 5); // FLG(n)
                if (eci < 10)
                {
                    binaryStream.AppendBits(1, 3); // FLG(1)
                    binaryStream.AppendBits(2 + eci, 4);
                }

                if ((eci >= 10) && (eci <= 99))
                {
                    binaryStream.AppendBits(2, 3); // FLG(2)
                    binaryStream.AppendBits(2 + (eci / 10), 4);
                    binaryStream.AppendBits(2 + (eci % 10), 4);
                }

                if ((eci >= 100) && (eci <= 999))
                {
                    binaryStream.AppendBits(3, 3); // FLG(3)
                    binaryStream.AppendBits(2 + (eci / 100), 4);
                    binaryStream.AppendBits(2 + ((eci % 100) / 10), 4);
                    binaryStream.AppendBits(2 + (eci % 10), 4);
                }

                if ((eci >= 1000) && (eci <= 9999))
                {
                    binaryStream.AppendBits(4, 3); // FLG(4)
                    binaryStream.AppendBits(2 + (eci / 1000), 4);
                    binaryStream.AppendBits(2 + ((eci % 1000) / 100), 4);
                    binaryStream.AppendBits(2 + ((eci % 100) / 10), 4);
                    binaryStream.AppendBits(2 + (eci % 10), 4);
                }

                if ((eci >= 10000) && (eci <= 99999))
                {
                    binaryStream.AppendBits(5, 3); // FLG(5) 
                    binaryStream.AppendBits(2 + (eci / 10000), 4);
                    binaryStream.AppendBits(2 + ((eci % 10000) / 1000), 4);
                    binaryStream.AppendBits(2 + ((eci % 1000) / 100), 4);
                    binaryStream.AppendBits(2 + ((eci % 100) / 10), 4);
                    binaryStream.AppendBits(2 + (eci % 10), 4);
                }

                if (eci >= 100000)
                {
                    binaryStream.AppendBits(6, 3); // FLG(6)
                    binaryStream.AppendBits(2 + (eci / 100000), 4);
                    binaryStream.AppendBits(2 + ((eci % 100000) / 10000), 4);
                    binaryStream.AppendBits(2 + ((eci % 10000) / 1000), 4);
                    binaryStream.AppendBits(2 + ((eci % 1000) / 100), 4);
                    binaryStream.AppendBits(2 + ((eci % 100) / 10), 4);
                    binaryStream.AppendBits(2 + (eci % 10), 4);
                }
            }

            currentMode = 'U';
            for (int i = 0; i < reducedLength; i++)
            {
                if (reducedEncodeMode[i] != currentMode)
                {
                    // Change mode
                    if (currentMode == 'U')
                    {
                        switch (reducedEncodeMode[i])
                        {
                            case 'L':
                                binaryStream.AppendBits(28, 5); // L/L
                                break;

                            case 'M':
                                binaryStream.AppendBits(29, 5); // M/L
                                break;

                            case 'P':
                                binaryStream.AppendBits(29, 5); // M/L
                                binaryStream.AppendBits(30, 5); // P/L
                                break;

                            case 'p':
                                binaryStream.AppendBits(0, 5); // P/S
                                break;

                            case 'D':
                                binaryStream.AppendBits(30, 5); // D/L
                                break;

                            case 'B':
                                binaryStream.AppendBits(31, 5); // B/S
                                break;
                        }
                    }

                    else if (currentMode == 'L')
                    {
                        switch (reducedEncodeMode[i])
                        {
                            case 'U':
                                binaryStream.AppendBits(30, 5); // D/L
                                binaryStream.AppendBits(14, 4); // U/L
                                break;

                            case 'u':
                                binaryStream.AppendBits(28, 5); // U/S
                                break;

                            case 'M':
                                binaryStream.AppendBits(29, 5); // M/L
                                break;

                            case 'P':
                                binaryStream.AppendBits(29, 5); // M/L
                                binaryStream.AppendBits(30, 5); // P/L
                                break;

                            case 'p':
                                binaryStream.AppendBits(0, 5); // P/S
                                break;

                            case 'D':
                                binaryStream.AppendBits(30, 5); // D/L
                                break;

                            case 'B':
                                binaryStream.AppendBits(31, 5); // B/S
                                break;
                        }
                    }

                    else if (currentMode == 'M')
                    {
                        switch (reducedEncodeMode[i])
                        {
                            case 'U':
                                binaryStream.AppendBits(29, 5); // U/L
                                break;

                            case 'L':
                                binaryStream.AppendBits(28, 5); // L/L
                                break;

                            case 'P':
                                binaryStream.AppendBits(30, 5); // P/L
                                break;

                            case 'p':
                                binaryStream.AppendBits(0, 5); // P/S
                                break;

                            case 'D':
                                binaryStream.AppendBits(29, 5); // U/L
                                binaryStream.AppendBits(30, 5); // D/L
                                break;

                            case 'B':
                                binaryStream.AppendBits(31, 5); // B/S
                                break;
                        }
                    }

                    else if (currentMode == 'P')
                    {
                        switch (reducedEncodeMode[i])
                        {
                            case 'U':
                                binaryStream.AppendBits(31, 5); // U/L
                                break;

                            case 'L':
                                binaryStream.AppendBits(31, 5); // U/L
                                binaryStream.AppendBits(28, 5);
                                break;

                            case 'M':
                                binaryStream.AppendBits(31, 5); // U/L
                                binaryStream.AppendBits(29, 5); // M/L
                                break;

                            case 'D':
                                binaryStream.AppendBits(31, 5); // U/L
                                binaryStream.AppendBits(30, 5); // D/L
                                break;

                            case 'B':
                                binaryStream.AppendBits(31, 5); // U/L
                                currentMode = 'U';
                                binaryStream.AppendBits(31, 5); // B/S
                                break;
                        }
                    }

                    else if (currentMode == 'D')
                    {
                        switch (reducedEncodeMode[i])
                        {
                            case 'U':
                                binaryStream.AppendBits(14, 4); // U/L
                                break;

                            case 'u':
                                binaryStream.AppendBits(15, 4); // U/S
                                break;

                            case 'L':
                                binaryStream.AppendBits(14, 4); // U/L
                                binaryStream.AppendBits(28, 5); // L/L
                                break;

                            case 'M':
                                binaryStream.AppendBits(14, 4); // U/L
                                binaryStream.AppendBits(29, 5); // M/L
                                break;

                            case 'P':
                                binaryStream.AppendBits(14, 4); // U/L
                                binaryStream.AppendBits(29, 5); // M/L
                                binaryStream.AppendBits(30, 5); // P/L
                                break;

                            case 'p':
                                binaryStream.AppendBits(0, 4); // P/S
                                break;

                            case 'B':
                                binaryStream.AppendBits(14, 4); // U/L
                                currentMode = 'U';
                                binaryStream.AppendBits(31, 5); // B/S
                                break;
                        }
                    }

                    // Byte mode length descriptor.
                    if ((reducedEncodeMode[i] == 'B') && (!byteMode))
                    {
                        for (count = 0; ((i + count) < reducedLength) && (reducedEncodeMode[i] == 'B'); count++)
                        {
                            ;
                        }

                        if (count > 2079)
                        {
                            return false;
                        }

                        if (count > 31) // Put 00000 followed by 11-bit number of bytes less 31.
                        {
                            binaryStream.AppendBits(0, 5);
                            binaryStream.AppendBits(count - 31, 11);
                        }

                        else // Put 5-bit number of bytes.
                        {
                            binaryStream.AppendBits(count, 5);
                        }

                        byteMode = true;
                    }

                    if ((reducedEncodeMode[i] != 'B') && byteMode)
                    {
                        byteMode = false;
                    }

                    if ((reducedEncodeMode[i] != 'B') && (reducedEncodeMode[i] != 'u') && (reducedEncodeMode[i] != 'p'))
                    {
                        currentMode = reducedEncodeMode[i];
                    }
                }

                if ((reducedEncodeMode[i] == 'U') || (reducedEncodeMode[i] == 'u'))
                {
                    if (reducedSource[i] == ' ')
                    {
                        binaryStream.AppendBits(1, 5);
                    }

                    else
                    {
                        binaryStream.AppendBits(AztecSymbolCharacter[(int)reducedSource[i]], 5);
                    }
                }

                if (reducedEncodeMode[i] == 'L')
                {
                    if (reducedSource[i] == ' ')
                    {
                        binaryStream.AppendBits(1, 5); // SP
                    }

                    else
                    {
                        binaryStream.AppendBits(AztecSymbolCharacter[(int)reducedSource[i]], 5);
                    }
                }

                if (reducedEncodeMode[i] == 'M')
                {
                    if (reducedSource[i] == ' ')
                    {
                        binaryStream.AppendBits(1, 5); // SP
                    }

                    else if (reducedSource[i] == 13)
                    {
                        binaryStream.AppendBits(14, 5); // CR
                    }

                    else
                    {
                        binaryStream.AppendBits(AztecSymbolCharacter[(int)reducedSource[i]], 5);
                    }
                }

                if ((reducedEncodeMode[i] == 'P') || (reducedEncodeMode[i] == 'p'))
                {
                    if (isGS1 && (reducedSource[i] == '\x1d'))
                    {
                        binaryStream.AppendBits(0, 5);     // FLG(n)
                        binaryStream.AppendBits(0, 3);     // FLG(0) = FNC1
                    }

                    else if (reducedSource[i] == 13)
                    {
                        binaryStream.AppendBits(1, 5);     // CR
                    }

                    else if (reducedSource[i] == 'a')
                    {
                        binaryStream.AppendBits(2, 5);     // CR LF
                    }

                    else if (reducedSource[i] == 'b')
                    {
                        binaryStream.AppendBits(3, 5);     // . SP
                    }

                    else if (reducedSource[i] == 'c')
                    {
                        binaryStream.AppendBits(4, 5);     // , SP
                    }

                    else if (reducedSource[i] == 'd')
                    {
                        binaryStream.AppendBits(5, 5);     // : SP
                    }

                    else if (reducedSource[i] == ',')
                    {
                        binaryStream.AppendBits(17, 5);    // Comma
                    }

                    else if (reducedSource[i] == '.')
                    {
                        binaryStream.AppendBits(19, 5);    // Full stop
                    }

                    else
                    {
                        binaryStream.AppendBits(AztecSymbolCharacter[(int)reducedSource[i]], 5);
                    }
                }

                if (reducedEncodeMode[i] == 'D')
                {
                    if (reducedSource[i] == ' ')
                    {
                        binaryStream.AppendBits(1, 4);     // SP
                    }

                    else if (reducedSource[i] == ',')
                    {
                        binaryStream.AppendBits(12, 4);    // Comma
                    }

                    else if (reducedSource[i] == '.')
                    {
                        binaryStream.AppendBits(13, 4);    // Full stop
                    }

                    else
                    {
                        binaryStream.AppendBits(AztecSymbolCharacter[(int)reducedSource[i]], 4);
                    }
                }

                if (reducedEncodeMode[i] == 'B')
                {
                    binaryStream.AppendBits(reducedSource[i], 8);
                }
            }

            if (binaryStream.SizeInBits > AZTEC_BIN_CAPACITY)
            {
                throw new InvalidDataLengthException("Aztec Code: Input data too long.");
            }

            return true;
        }

        // Calculate the position of the bits in the grid.
        private void PopulateMap()
        {
            int layer, start, length, n, i;
            int x, y;

            for (layer = 1; layer < 33; layer++)
            {
                start = (112 * (layer - 1)) + (16 * (layer - 1) * (layer - 1)) + 2;
                length = 28 + ((layer - 1) * 4) + (layer * 4);

                // Top.
                i = 0;
                x = 64 - ((layer - 1) * 2);
                y = 63 - ((layer - 1) * 2);
                for (n = start; n < (start + length); n += 2)
                {
                    AztecMap[(AvoidReferenceGrid(y) * 151) + AvoidReferenceGrid(x + i)] = n;
                    AztecMap[(AvoidReferenceGrid(y - 1) * 151) + AvoidReferenceGrid(x + i)] = n + 1;
                    i++;
                }

                // Right.
                i = 0;
                x = 78 + ((layer - 1) * 2);
                y = 64 - ((layer - 1) * 2);
                for (n = start + length; n < (start + (length * 2)); n += 2)
                {
                    AztecMap[(AvoidReferenceGrid(y + i) * 151) + AvoidReferenceGrid(x)] = n;
                    AztecMap[(AvoidReferenceGrid(y + i) * 151) + AvoidReferenceGrid(x + 1)] = n + 1;
                    i++;
                }

                // Bottom.
                i = 0;
                x = 77 + ((layer - 1) * 2);
                y = 78 + ((layer - 1) * 2);
                for (n = start + (length * 2); n < (start + (length * 3)); n += 2)
                {
                    AztecMap[(AvoidReferenceGrid(y) * 151) + AvoidReferenceGrid(x - i)] = n;
                    AztecMap[(AvoidReferenceGrid(y + 1) * 151) + AvoidReferenceGrid(x - i)] = n + 1;
                    i++;
                }

                // Left.
                i = 0;
                x = 63 - ((layer - 1) * 2);
                y = 77 + ((layer - 1) * 2);
                for (n = start + (length * 3); n < (start + (length * 4)); n += 2)
                {
                    AztecMap[(AvoidReferenceGrid(y - i) * 151) + AvoidReferenceGrid(x)] = n;
                    AztecMap[(AvoidReferenceGrid(y - i) * 151) + AvoidReferenceGrid(x - 1)] = n + 1;
                    i++;
                }
            }

            // Central finder pattern.
            for (y = 69; y <= 81; y++)
            {
                for (x = 69; x <= 81; x++)
                {
                    AztecMap[(x * 151) + y] = 1;
                }
            }

            for (y = 70; y <= 80; y++)
            {
                for (x = 70; x <= 80; x++)
                {
                    AztecMap[(x * 151) + y] = 0;
                }
            }

            for (y = 71; y <= 79; y++)
            {
                for (x = 71; x <= 79; x++)
                {
                    AztecMap[(x * 151) + y] = 1;
                }
            }

            for (y = 72; y <= 78; y++)
            {
                for (x = 72; x <= 78; x++)
                {
                    AztecMap[(x * 151) + y] = 0;
                }
            }

            for (y = 73; y <= 77; y++)
            {
                for (x = 73; x <= 77; x++)
                {
                    AztecMap[(x * 151) + y] = 1;
                }
            }

            for (y = 74; y <= 76; y++)
            {
                for (x = 74; x <= 76; x++)
                {
                    AztecMap[(x * 151) + y] = 0;
                }
            }

            // Guide bars.
            for (y = 11; y < 151; y += 16)
            {
                for (x = 1; x < 151; x += 2)
                {
                    AztecMap[(x * 151) + y] = 1;
                    AztecMap[(y * 151) + x] = 1;
                }
            }

            // Descriptor.
            for (i = 0; i < 10; i++)
            {
                AztecMap[(AvoidReferenceGrid(64) * 151) + AvoidReferenceGrid(66 + i)] = 20000 + i;  // Top.
            }

            for (i = 0; i < 10; i++)
            {
                AztecMap[(AvoidReferenceGrid(66 + i) * 151) + AvoidReferenceGrid(77)] = 20010 + i;  // Right.
            }

            for (i = 0; i < 10; i++)
            {
                AztecMap[(AvoidReferenceGrid(77) * 151) + AvoidReferenceGrid(75 - i)] = 20020 + i;  // Bottom.
            }

            for (i = 0; i < 10; i++)
            {
                AztecMap[(AvoidReferenceGrid(75 - i) * 151) + AvoidReferenceGrid(64)] = 20030 + i;  // Left.
            }

            // Orientation.
            AztecMap[(AvoidReferenceGrid(64) * 151) + AvoidReferenceGrid(64)] = 1;
            AztecMap[(AvoidReferenceGrid(65) * 151) + AvoidReferenceGrid(64)] = 1;
            AztecMap[(AvoidReferenceGrid(64) * 151) + AvoidReferenceGrid(65)] = 1;
            AztecMap[(AvoidReferenceGrid(64) * 151) + AvoidReferenceGrid(77)] = 1;
            AztecMap[(AvoidReferenceGrid(65) * 151) + AvoidReferenceGrid(77)] = 1;
            AztecMap[(AvoidReferenceGrid(76) * 151) + AvoidReferenceGrid(77)] = 1;
        }

        // Prevent data from obscuring reference grid.
        private int AvoidReferenceGrid(int output)
        {
            if (output > 10)
            {
                output += (output - 11) / 15 + 1;
            }

            return output;
        }

        private int CountDoubles(char[] source, int position, int length)
        {
            int c = 0;

            while ((position + 1 < length) && ((source[position] == '.') || (source[position] == ',')) && (source[position + 1] == ' '))
            {
                c++;
                position += 2;
            }

            return c;
        }

        private int CountChar(char[] source, int position, int length, char chr)
        {
            int c = 0;
            while (position < length && source[position] == chr)
            {
                c++;
                position++;
            }

            return c;
        }

        private int CountDotComma(char[] source, int position, int length)
        {
            int c = 0;

            while (position < length && ((source[position] == '.') || (source[position] == ',')))
            {
                c++;
                position++;
            }

            return c;
        }

        private char GetNextMode(char[] encodeMode, int length, int position)
        {
            int currentMode = encodeMode[position];

            do
            {
                position++;
            } while ((position < length) && (encodeMode[position] == currentMode));

            if (position >= length)
            {
                return 'E';
            }

            else
            {
                return encodeMode[position];
            }
        }

        private void BuildSymbolData(BitVector binaryPattern, int layers, bool isCompactSymbol)
        {
            // Expand the row pattern into the symbol data.
            byte[] rowData;
            if (isCompactSymbol)
            {
                int offset = AztecCompactOffset[layers - 1];
                int size = 27 - (2 * offset);

                for (int y = offset; y < (27 - offset); y++)
                {
                    rowData = new byte[size];
                    for (int x = offset; x < (27 - offset); x++)
                    {
                        if (CompactAztecMap[(y * 27) + x] == 1)
                        {
                            rowData[x - offset] = 1;
                        }

                        if (CompactAztecMap[(y * 27) + x] >= 2)
                        {
                            if (binaryPattern[CompactAztecMap[(y * 27) + x] - 2] == 1)
                            {
                                rowData[x - offset] = 1;
                            }
                        }
                    }

                    SymbolData symbolData = new SymbolData(rowData, 1.0f);
                    Symbol.Insert(y - offset, symbolData);
                }
            }

            else
            {
                int offset = AztecOffset[layers - 1];
                int size = 151 - (2 * offset);

                for (int y = offset; y < (151 - offset); y++)
                {
                    rowData = new byte[size];
                    for (int x = offset; x < (151 - offset); x++)
                    {
                        if (AztecMap[(y * 151) + x] == 1)
                        {
                            rowData[x - offset] = 1;
                        }

                        if (AztecMap[(y * 151) + x] >= 2)
                        {
                            if (binaryPattern[AztecMap[(y * 151) + x] - 2] == 1)
                            {
                                rowData[x - offset] = 1;
                            }
                        }
                    }

                    SymbolData symbolData = new SymbolData(rowData, 1.0f);
                    Symbol.Insert(y - offset, symbolData);
                }
            }
        }

        /// <summary>
        /// Generate an Aztec Runes symbol.
        /// </summary>
        private void AztecRunes()
        {
            int inputValue;
            int inputLength = barcodeData.Length;
            BitVector bitStream = new BitVector();
            byte[] dataCodewords = new byte[2];
            byte[] eccCodewords = new byte[5];

            if (inputLength > 3)
            {
                throw new InvalidDataLengthException("Aztec Runes: Maximum input of 3 characters.");
            }

            inputValue = int.Parse(new string(barcodeMessage), CultureInfo.CurrentCulture);
            if (inputValue > 255)
            {
                throw new InvalidDataException("Aztec Runes: Maximum numeric value 255.");
            }

            bitStream.AppendBits(inputValue, 8);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (bitStream[(i * 4) + j] == 1)
                    {
                        dataCodewords[i] += (byte)(0x08 >> j);
                    }
                }
            }

            ReedSolomon.RSInitialise(0x13, 5, 1);
            ReedSolomon.RSEncode(2, dataCodewords, eccCodewords);
            bitStream[0] = 0;

            for (int i = 0; i < 5; i++)
            {
                bitStream.AppendBits(eccCodewords[4 - i], 4);
            }

            int length = bitStream.SizeInBits;
            for (int i = 0; i < length; i += 2)
            {
                if (bitStream[i] == 0)
                {
                    bitStream[i] = 1;
                }
                else
                {
                    bitStream[i] = 0;
                }
            }

            // Expand the row pattern into the symbol data.
            byte[] rowData;
            int size = 11;

            for (int y = 8; y < 19; y++)
            {
                rowData = new byte[size];
                for (int x = 8; x < 19; x++)
                {
                    if (CompactAztecMap[(y * 27) + x] == 1)
                    {
                        rowData[x - 8] = 1;
                    }

                    if (CompactAztecMap[(y * 27) + x] >= 2)
                    {
                        if (bitStream[CompactAztecMap[(y * 27) + x] - 2000] == 1)
                        {
                            rowData[x - 8] = 1;
                        }
                    }
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                int row = y - 8;
                Symbol.Insert(row, symbolData);
            }
        }
    }
}
