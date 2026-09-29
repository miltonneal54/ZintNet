/* DataMatrixEncoder.cs Handles Data Matrix ECC 200 2D symbol */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2009-2025 Robin Stuart <rstuart114@gmail.com>

    developed from and including some functions from:
        IEC16022 bar code generation
        Adrian Kennard, Andrews & Arnold Ltd
        with help from Cliff Hones on the RS coding

        (c) 2004 Adrian Kennard, Andrews & Arnold Ltd
        (c) 2006 Stefan Schmidt <stefan@datenfreihafen.org>

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
    /// <summary>
    /// Builds a DataMatrix Symbol
    /// </summary>
    internal class DataMatrixEncoder : SymbolEncoder
    {
        #region Tables.

        private readonly int[] C40Shift = {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
            2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        private readonly int[] C40Values = {
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

        private readonly int[] TextValues = {
             0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
             3,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
             4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20,
            21,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 22, 23, 24, 25, 26,
             0, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
            29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 27, 28, 29, 30, 31 };

        private readonly int[] MatrixBytes = {
	        /* 0*/   3 /*10x10*/,      5 /*12x12*/,      5 /*8x18*/,       8 /*14x14*/,
            /* 4*/  10 /*8x32*/,      12 /*16x16*/,     16 /*12x26*/,     18 /*18x18*/,
            /* 8*/  18 /*8x48*/,      22 /*20x20*/,     22 /*12x36*/,     24 /*8x64*/,
            /*12*/  30 /*22x22*/,     32 /*16x36*/,     32 /*8x80*/,      36 /*24x24*/,
            /*16*/  38 /*8x96*/,      43 /*12x64*/,     44 /*26x26*/,     44 /*20x36*/,
            /*20*/  49 /*16x48*/,     49 /*8x120*/,     56 /*20x44*/,     62 /*32x32*/,
            /*24*/  62 /*16x64*/,     63 /*8x144*/,     64 /*12x88*/,     70 /*26x40*/,
            /*28*/  72 /*22x48*/,     80 /*24x48*/,     84 /*20x64*/,     86 /*36x36*/,
            /*32*/  90 /*26x48*/,    108 /*24x64*/,    114 /*40x40*/,    118 /*26x64*/,
            /*36*/ 144 /*44x44*/,    174 /*48x48*/,    204 /*52x52*/,    280 /*64x64*/,
            /*40*/ 368 /*72x72*/,    456 /*80x80*/,    576 /*88x88*/,    696 /*96x96*/,
            /*44*/ 816 /*104x104*/, 1050 /*120x120*/, 1304 /*132x132*/, 1558 /*144x144*/ };

        private readonly int[] IntSymbols = {
            /* Standard DM square */
            /*  1-4*/  0 /*10x10 (3)*/,      1 /*12x12 (5)*/,       3 /*14x14 (8)*/,       5 /*16x16 (12)*/,
            /*  5-8*/  7 /*18x18 (18)*/,     9 /*20x20 (22)*/,     12 /*22x22 (30)*/,     15 /*24x24 (36)*/,
            /* 9-12*/ 18 /*26x26 (44)*/,    23 /*32x32 (62)*/,     31 /*36x36 (86)*/,     34 /*40x40 (114)*/,
            /*13-16*/ 36 /*44x44 (144)*/,   37 /*48x48 (174)*/,    38 /*52x52 (204)*/,    39 /*64x64 (280)*/,
            /*17-20*/ 40 /*72x72 (368)*/,   41 /*80x80 (456)*/,    42 /*88x88 (576)*/,    43 /*96x96 (696)*/,
            /*21-24*/ 44 /*104x104 (816)*/, 45 /*120x120 (1050)*/, 46 /*132x132 (1304)*/, 47 /*144x144 (1558)*/,

            /* Standard DM rectangular */
            /*25-28*/  2 /*8x18 (5)*/,       4 /*8x32 (10)*/,       6 /*12x26 (16)*/,     10 /*12x36 (22)*/,
            /*29-30*/ 13 /*16x36 (32)*/,    20 /*16x48 (49)*/,

            /* DMRE */
            /*31-34*/  8 /*8x48 (18)*/,     11 /*8x64 (24)*/,      14 /*8x80 (32)*/,      16 /*8x96 (38)*/,
            /*35-38*/ 21 /*8x120 (49)*/,    25 /*8x144 (63)*/,     17 /*12x64 (43)*/,     26 /*12x88 (64)*/,
            /*39-42*/ 24 /*16x64 (62)*/,    19 /*20x36 (44)*/,     22 /*20x44 (56)*/,     30 /*20x64 (84)*/,
            /*43-46*/ 28 /*22x48 (72)*/,    29 /*24x48 (80)*/,     33 /*24x64 (108)*/,    27 /*26x40 (70)*/,
            /*47-48*/ 32 /*26x48 (90)*/,    35 /*26x64 (118)*/ };

        private readonly bool[] IsDMRE = {
            /* 0*/ false /*10x10 (3)*/,     false /*12x12 (5)*/,      false /*8x18 (5)*/,       false /*14x14 (8)*/,
            /* 4*/ false /*8x32 (10)*/,     false /*16x16 (12)*/,     false /*12x26 (16)*/,     false /*18x18 (18)*/,
            /* 8*/ true  /*8x48 (18)*/,     false /*20x20 (22)*/,     false /*12x36 (22)*/,     true  /*8x64 (24)*/,
            /*12*/ false /*22x22 (30)*/,    false /*16x36 (32)*/,     true  /*8x80 (32)*/,      false /*24x24 (36)*/,
            /*16*/ true  /*8x96 (38)*/,     true  /*12x64 (43)*/,     false /*26x26 (44)*/,     true  /*20x36 (44)*/,
            /*20*/ false /*16x48 (49)*/,    true  /*8x120 (49)*/,     true  /*20x44 (56)*/,     false /*32x32 (62)*/,
            /*24*/ true  /*16x64 (62)*/,    true  /*8x144 (63)*/,     true  /*12x88 (64)*/,     true  /*26x40 (70)*/,
            /*28*/ true  /*22x48 (72)*/,    true  /*24x48 (80)*/,     true  /*20x64 (84)*/,     false /*36x36 (86)*/,
            /*32*/ true  /*26x48 (90)*/,    true  /*24x64 (108)*/,    false /*40x40 (114)*/,    true  /*26x64 (118)*/,
            /*36*/ false /*44x44 (144)*/,   false /*48x48 (174)*/,    false /*52x52 (204)*/,    false /*64x64 (280)*/,
            /*40*/ false /*72x72 (368)*/,   false /*80x80 (456)*/,    false /*88x88 (576)*/,    false /*96x96 (696)*/,
            /*44*/ false /*104x104 (816)*/, false /*120x120 (1050)*/, false /*132x132 (1304)*/, false /*144x144 (1558)*/ };


        private readonly int[] MatrixHeights = {
	        /* 0*/  10 /*10x10*/,    12 /*12x12 */,    8 /*8x18*/,     14 /*14x14*/,
            /* 4*/   8 /*8x32*/,     16 /*16x16*/,    12 /*12x26*/,    18 /*18x18*/,
            /* 8*/   8 /*8x48*/,     20 /*20x20*/,    12 /*12x36*/,     8 /*8x64*/,
            /*12*/  22 /*22x22*/,    16 /*16x36*/,     8 /*8x80*/,     24 /*24x24*/,
            /*16*/   8 /*8x96*/,     12 /*12x64*/,    26 /*26x26*/,    20 /*20x36*/,
            /*20*/  16 /*16x48*/,     8 /*8x120*/,    20 /*20x44*/,    32 /*32x32*/,
            /*24*/  16 /*16x64*/,     8 /*8x144*/,    12 /*12x88*/,    26 /*26x40*/,
            /*28*/  22 /*22x48*/,    24 /*24x48*/,    20 /*20x64*/,    36 /*36x36*/,
            /*32*/  26 /*26x48*/,    24 /*24x64*/,    40 /*40x40*/,    26 /*26x64*/,
            /*36*/  44 /*44x44*/,    48 /*48x48*/,    52 /*52x52*/,    64 /*64x64*/,
            /*40*/  72 /*72x72*/,    80 /*80x80*/,    88 /*88x88*/,    96 /*96x96*/,
            /*44*/ 104 /*104x104*/, 120 /*120x120*/, 132 /*132x132*/, 144 /*144x144*/ };


        private readonly int[] MatrixWidths = {
	        /* 0*/  10 /*10x10*/,    12 /*12x12*/,    18 /*8x18*/,     14 /*14x14*/,
            /* 4*/  32 /*8x32*/,     16 /*16x16*/,    26 /*12x26*/,    18 /*18x18*/,
            /* 8*/  48 /*8x48*/,     20 /*20x20*/,    36 /*12x36*/,    64 /*8x64*/,
            /*12*/  22 /*22x22*/,    36 /*16x36*/,    80 /*8x80*/,     24 /*24x24*/,
            /*16*/  96 /*8x96*/,     64 /*12x64*/,    26 /*26x26*/,    36 /*20x36*/,
            /*20*/  48 /*16x48*/,   120 /*8x120*/,    44 /*20x44*/,    32 /*32x32*/,
            /*24*/  64 /*16x64*/,   144 /*8x144*/,    88 /*12x88*/,    40 /*26x40*/,
            /*28*/  48 /*22x48*/,    48 /*24x48*/,    64 /*20x64*/,    36 /*36x36*/,
            /*32*/  48 /*26x48*/,    64 /*24x64*/,    40 /*40x40*/,    64 /*26x64*/,
            /*36*/  44 /*44x44*/,    48 /*48x48*/,    52 /*52x52*/,    64 /*64x64*/,
            /*40*/  72 /*72x72*/,    80 /*80x80*/,    88 /*88x88*/,    96 /*96x96*/,
            /*44*/ 104 /*104x104*/, 120 /*120x120*/, 132 /*132x132*/, 144 /*144x144*/ };

        // Horizontal submodule size (including subfinder)
        private readonly int[] MatrixFH = {
	        /* 0*/ 10 /*10x10*/,   12 /*12x12*/,    8 /*8x18*/,    14 /*14x14*/,
            /* 4*/  8 /*8x32*/,    16 /*16x16*/,   12 /*12x26*/,   18 /*18x18*/,
            /* 8*/  8 /*8x48*/,    20 /*20x20*/,   12 /*12x36*/,    8 /*8x64*/,
            /*12*/ 22 /*22x22*/,   16 /*16x36*/,    8 /*8x80*/,    24 /*24x24*/,
            /*16*/  8 /*8x96*/,    12 /*12x64*/,   26 /*26x26*/,   20 /*20x36*/,
            /*20*/ 16 /*16x48*/,    8 /*8x120*/,   20 /*20x44*/,   16 /*32x32*/,
            /*24*/ 16 /*16x64*/,    8 /*8x144*/,   12 /*12x88*/,   26 /*26x40*/,
            /*28*/ 22 /*22x48*/,   24 /*24x48*/,   20 /*20x64*/,   18 /*36x36*/,
            /*32*/ 26 /*26x48*/,   24 /*24x64*/,   20 /*40x40*/,   26 /*26x64*/,
            /*36*/ 22 /*44x44*/,   24 /*48x48*/,   26 /*52x52*/,   16 /*64x64*/,
            /*40*/ 18 /*72x72*/,   20 /*80x80*/,   22 /*88x88*/,   24 /*96x96*/,
            /*44*/ 26 /*104x104*/, 20 /*120x120*/, 22 /*132x132*/, 24 /*144x144*/ };

        // Vertical submodule size (including subfinder)
        private readonly int[] MatrixFW = {
	        /* 0*/ 10 /*10x10*/,   12 /*12x12*/,   18 /*8x18*/,    14 /*14x14*/,
            /* 4*/ 16 /*8x32*/,    16 /*16x16*/,   26 /*12x26*/,   18 /*18x18*/,
            /* 8*/ 24 /*8x48*/,    20 /*20x20*/,   18 /*12x36*/,   16 /*8x64*/,
            /*12*/ 22 /*22x22*/,   18 /*16x36*/,   20 /*8x80*/,    24 /*24x24*/,
            /*16*/ 24 /*8x96*/,    16 /*12x64*/,   26 /*26x26*/,   18 /*20x36*/,
            /*20*/ 24 /*16x48*/,   20 /*8x120*/,   22 /*20x44*/,   16 /*32x32*/,
            /*24*/ 16 /*16x64*/,   24 /*8x144*/,   22 /*12x88*/,   20 /*26x40*/,
            /*28*/ 24 /*22x48*/,   24 /*24x48*/,   16 /*20x64*/,   18 /*36x36*/,
            /*32*/ 24 /*26x48*/,   16 /*24x64*/,   20 /*40x40*/,   16 /*26x64*/,
            /*36*/ 22 /*44x44*/,   24 /*48x48*/,   26 /*52x52*/,   16 /*64x64*/,
            /*40*/ 18 /*72x72*/,   20 /*80x80*/,   22 /*88x88*/,   24 /*96x96*/,
            /*44*/ 26 /*104x104*/, 20 /*120x120*/, 22 /*132x132*/, 24 /*144x144*/ };

        // Data Codewords per RS-Block.
        private readonly int[] MatrixDataBlocks = {
	        /* 0*/   3 /*10x10*/,     5 /*12x12*/,     5 /*8x18*/,      8 /*14x14*/,
            /* 4*/  10 /*8x32*/,     12 /*16x16*/,    16 /*12x26*/,    18 /*18x18*/,
            /* 8*/  18 /*8x48*/,     22 /*20x20*/,    22 /*12x36*/,    24 /*8x64*/,
            /*12*/  30 /*22x22*/,    32 /*16x36*/,    32 /*8x80*/,     36 /*24x24*/,
            /*16*/  38 /*8x96*/,     43 /*12x64*/,    44 /*26x26*/,    44 /*20x36*/,
            /*20*/  49 /*16x48*/,    49 /*8x120*/,    56 /*20x44*/,    62 /*32x32*/,
            /*24*/  62 /*16x64*/,    63 /*8x144*/,    64 /*12x88*/,    70 /*26x40*/,
            /*28*/  72 /*22x48*/,    80 /*24x48*/,    84 /*20x64*/,    86 /*36x36*/,
            /*32*/  90 /*26x48*/,   108 /*24x64*/,   114 /*40x40*/,   118 /*26x64*/,
            /*36*/ 144 /*44x44*/,   174 /*48x48*/,   102 /*52x52*/,   140 /*64x64*/,
            /*40*/  92 /*72x72*/,   114 /*80x80*/,   144 /*88x88*/,   174 /*96x96*/,
            /*44*/ 136 /*104x104*/, 175 /*120x120*/, 163 /*132x132*/, 156 /*144x144*/ };

        // ECC Codewords per RS-Block.
        private readonly int[] MatrixEccBlocks = {
	        /* 0*/  5 /*10x10*/,    7 /*12x12*/,    7 /*8x18*/,    10 /*14x14*/,
            /* 4*/ 11 /*8x32*/,    12 /*16x16*/,   14 /*12x26*/,   14 /*18x18*/,
            /* 8*/ 15 /*8x48*/,    18 /*20x20*/,   18 /*12x36*/,   18 /*8x64*/,
            /*12*/ 20 /*22x22*/,   24 /*16x36*/,   22 /*8x80*/,    24 /*24x24*/,
            /*16*/ 28 /*8x96*/,    27 /*12x64*/,   28 /*26x26*/,   28 /*20x36*/,
            /*20*/ 28 /*16x48*/,   32 /*8x120*/,   34 /*20x44*/,   36 /*32x32*/,
            /*24*/ 36 /*16x64*/,   36 /*8x144*/,   36 /*12x88*/,   38 /*26x40*/,
            /*28*/ 38 /*22x48*/,   41 /*24x48*/,   42 /*20x64*/,   42 /*36x36*/,
            /*32*/ 42 /*26x48*/,   46 /*24x64*/,   48 /*40x40*/,   50 /*26x64*/,
            /*36*/ 56 /*44x44*/,   68 /*48x48*/,   42 /*52x52*/,   56 /*64x64*/,
            /*40*/ 36 /*72x72*/,   48 /*80x80*/,   56 /*88x88*/,   68 /*96x96*/,
            /*44*/ 56 /*104x104*/, 68 /*120x120*/, 62 /*132x132*/, 62 /*144x144*/ };

        #endregion

        #region Constants.

        // Number of DM Sizes
        private const int DMSIZESCOUNT = 48;    // Number of DM Sizes.
        private const int SYMBOL144 = 47;       // Number of 144x144 for special interlace.

        private const int ASCII = 1;
        private const int C40 = 2;
        private const int TEXT = 3;
        private const int X12 = 4;
        private const int EDIFACT = 5;
        private const int BASE256 = 6;
        private const int NUM_MODES = 6;

        #endregion

        private int optionSymbolSize;
        private readonly bool optionSquareOnly;         // True if force a square symbol in autosize.
        private readonly bool optionDMRE;               // True if using rectangular extensions.
        private readonly GS1MODE optionGS1Mode;         // GS1 separator character.
        private readonly SymbolEncodeMode encodeMode;   // Symbol encoding mode.
        // private bool optionReader;                   // True if reader initialization mode. (not supported)

        public DataMatrixEncoder(Symbology symbolId, char[] barcodeMessage, Mailmark2DFormat formatSize)
            : this(symbolId, barcodeMessage, (DataMatrixSize)formatSize, false, false, 0, SymbolEncodeMode.FastMode, 0, EncodingFormat.Standard)
        { }

        public DataMatrixEncoder(Symbology symbolId, char[] barcodeMessage, DataMatrixSize optionSymbolSize, bool optionSquareOnly,
                                 bool optionDMRE, GS1MODE optionGS1Mode,  SymbolEncodeMode encodeMode, int eci, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionSymbolSize = (int)optionSymbolSize;
            this.optionSquareOnly = optionSquareOnly;
            this.optionDMRE = optionDMRE;
            this.optionGS1Mode = optionGS1Mode;
            this.encodeMode = encodeMode;
            this.eci = eci;
            this.encodingMode = encodingMode;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();

            switch (symbolId)
            {
                case Symbology.DataMatrix:
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

                        case EncodingFormat.HIBC:
                            isGS1 = false;
                            barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                            break;
                    }

                    DataMatrix();
                    break;

                case Symbology.Mailmark2D:
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    Mailmark2D(barcodeData);
                    break;
            }

            return Symbol;
        }

        private void DataMatrix()
        {
            byte[] symbolGrid;
            byte[] target = new byte[2200];
            int tp = 0; // Size of target in bytes;
            bool skew = false;
            int symbolSize;
            int dataBlocks, eccBlocks;

            EncodeSegments(barcodeData, target, ref tp);
            symbolSize = GetSymbolSize(tp);
            if (tp > MatrixBytes[symbolSize])
            {
                if (optionSymbolSize >= 1 && optionSymbolSize <= DMSIZESCOUNT)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Data Matrix: Input too long for Version {0}.\nRequires {1} codewords, maximum available {2}.", optionSymbolSize, tp, MatrixBytes[symbolSize]));
                }
            }

            int matrixHeight = MatrixHeights[symbolSize];
            int matrixWidth = MatrixWidths[symbolSize];
            int fHeight = MatrixFH[symbolSize];
            int fWidth = MatrixFW[symbolSize];
            dataBlocks = MatrixDataBlocks[symbolSize];
            eccBlocks = MatrixEccBlocks[symbolSize];

            int bytes = MatrixBytes[symbolSize];
            int padLength = bytes - tp;
            if (padLength > 0)
            {
                AddPadding(target, padLength, ref tp);
            }

            // ECC code.
            if (symbolSize == SYMBOL144/* && !(symbol->option_3 & DM_ISO_144)*/)
            {
                skew = true;
            }

            AddErrorCorrection(target, bytes, dataBlocks, eccBlocks, skew);

            // Placement.
            int numberOfColumns = matrixWidth - 2 * (matrixWidth / fWidth);
            int numberOfRows = matrixHeight - 2 * (matrixHeight / fHeight);
            int[] matrixLocations = new int[numberOfColumns * numberOfRows];
            Placement(matrixLocations, numberOfRows, numberOfColumns);

            symbolGrid = new byte[matrixWidth * matrixHeight];
            for (int y = 0; y < matrixHeight; y += fHeight)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    symbolGrid[y * matrixWidth + x] = 1;
                }

                for (int x = 0; x < matrixWidth; x += 2)
                {
                    symbolGrid[(y + fHeight - 1) * matrixWidth + x] = 1;
                }
            }

            for (int x = 0; x < matrixWidth; x += fWidth)
            {
                for (int y = 0; y < matrixHeight; y++)
                {
                    symbolGrid[y * matrixWidth + x] = 1;
                }

                for (int y = 0; y < matrixHeight; y += 2)
                {
                    symbolGrid[y * matrixWidth + x + fWidth - 1] = 1;
                }
            }

            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    int binaryValue = 0;
                    int locationValue = matrixLocations[(numberOfRows - y - 1) * numberOfColumns + x];

                    if (locationValue == 1 || locationValue > 7)
                    {
                        if (locationValue > 7)
                        {
                            binaryValue = target[(locationValue >> 3) - 1] & (1 << (locationValue & 7));
                        }

                        if (locationValue == 1 || binaryValue != 0)
                        {
                            int position = (1 + y + 2 * (y / (fHeight - 2))) * matrixWidth + 1 + x + 2 * (x / (fWidth - 2));
                            symbolGrid[position] = 1;
                        }
                    }
                }
            }

            byte[] rowData;
            for (int y = matrixHeight - 1; y >= 0; y--)
            {
                rowData = new byte[matrixWidth];
                for (int x = 0; x < matrixWidth; x++)
                {
                    rowData[x] = symbolGrid[(matrixWidth * y) + x];
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private void EncodeSegments(char[] source, byte[] target, ref int tp)
        {
            bool lastSeg = true;
            int gs1 = 0;
            int length = source.Length;

            // gs1 flag values: 0: no gs1, 1: gs1 with FNC1 serparator, 2: gs1 with GS separator.
            if (encodingMode == EncodingFormat.GS1)
            {
                if (optionGS1Mode == GS1MODE.GS1ModeGS)
                {
                    gs1 = 2;
                }

                else
                {
                    gs1 = 1;
                }

                target[tp++] = 232;    // FNC1.
            }

            // Add support for Macro05 & Macro06
            // "[)>[RS]05[GS]...[RS][EOT]" -> CW 236
            // "[)>[RS]06[GS]...[RS][EOT]" -> CW 237

            if (tp == 0 && length >= 9
                 && source[0] == '[' && source[1] == ')' && source[2] == '>'
                 && source[3] == '\x1e' && source[4] == '0'
                 && (source[5] == '5' || source[5] == '6')
                 && source[6] == '\x1d'
                 && source[length - 2] == '\x1e' && source[length - 1] == '\x04')
            {
                // Output macro Codeword.
                if (source[5] == '5')
                {
                    target[tp++] = 236;
                }

                else
                {
                    target[tp++] = 237;
                }

                // Remove 7 macro characters from the front, RS and EOT from the end of the input string.
                for(int i = 0; i < 2; i++)
                {
                    source = ArrayHelper.Remove(source, length - 1);
                }

                Array.Copy(source, 7, source, 0, length - 9);
            }

            DMEncode(source, length, eci, lastSeg, target, ref tp, gs1);
        }

        // Encodes data using ASCII, C40, Text, X12, EDIFACT or Base 256 modes as appropriate.
        // Supports encoding FNC1 in supporting systems.
        private void DMEncode(char[] source, int length, int eci, bool lastSeg, byte[] target, ref int tp, int gs1)
        {
            int[] processBuffer = new int[8];      // Holds remaining data to be finalised.
            int processIndex = 0;                  // Number of characters left to finalise.
            int sourceIndex = 0;
            int currentMode = ASCII;
            int b256Start = 0;
            int symbolsLeft;

            if (eci > 0)
            {
                // Encode ECI numbers according to Table 6.
                target[tp++] = 241; // ECI Character.

                if (eci <= 126)
                {
                    target[tp++] = (byte)(eci + 1);
                }

                else if (eci <= 16382)
                {
                    target[tp++] = (byte)((eci - 127) / 254 + 128);
                    target[tp++] = (byte)((eci - 127) % 254 + 1);
                }

                else
                {
                    target[tp++] = (byte)((eci - 16383) / 64516 + 192);
                    target[tp++] = (byte)(((eci - 16383) / 254 % 254) + 1);
                    target[tp++] = (byte)((eci - 16383) % 254 + 1);
                }
            }

            // If FAST_MODE or MAILMARK_2D, do Annex J-based encodation.
            if (encodeMode == SymbolEncodeMode.FastMode || symbolId == Symbology.Mailmark2D)
            {
                ISOEncode(source, length, ref sourceIndex, target, ref tp, processBuffer, ref processIndex, ref b256Start, ref currentMode, gs1);
            }

            else
            {
                // Do default minimal encodation.
                MinimalEncode(source, length, lastSeg, ref sourceIndex, target, ref tp, processBuffer, ref processIndex, ref b256Start, ref currentMode, gs1);
            }

            symbolsLeft = lastSeg ? CodewordsRemaining(tp, processIndex) : 3;

            if (currentMode == C40 || currentMode == TEXT)
            {
                // NOTE: changed to follow spec exactly here, only using Shift 1 padded triplets when 2 symbol chars remain.
                // This matches the behaviour of BWIPP but not TEC-IT, nor figures 4.15.1-1 and 4.15-1-2 in GS1 General
                // Specifications 21.0.1.

                if (processIndex == 0)
                {
                    if (symbolsLeft > 0)
                    {
                        target[tp++] = 254;    // Unlatch.
                        tp++;
                    }
                }

                else
                {
                    if (processIndex == 2 && symbolsLeft == 2)
                    {
                        // 5.2.5.2 (b).
                        processBuffer[processIndex++] = 0;     // Shift 1.
                        _ = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);

                    }

                    else if (processIndex == 1 && symbolsLeft <= 2 && IsC40Text(currentMode, source[length - 1]))
                    {
                        // 5.2.5.2 (c)/(d).
                        if (symbolsLeft > 1)
                        {
                            // 5.2.5.2 (c).
                            target[tp++] = 254;    // Unlatch and encode remaining data in ASCII.
                        }

                        target[tp++] = (byte)(source[length - 1] + 1);
                    }

                    else
                    {
                        int count, totalCount = 0;

                        // Backtrack to last complete triplet (same technique as BWIPP).
                        while (sourceIndex > 0 && (processIndex % 3) > 0)
                        {
                            sourceIndex--;
                            count = C40texCount(currentMode, source[sourceIndex]);
                            totalCount += count;
                            processIndex -= count;
                        }

                        tp -= totalCount / 3 * 2;
                        target[tp++] = 254;    // Unlatch.

                        for (; sourceIndex < length; sourceIndex++)
                        {
                            if (Common.IsTwoDigits(source, length, sourceIndex))
                            {
                                target[tp++] = (byte)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                                sourceIndex++;
                            }

                            else if ((source[sourceIndex] & 0x80) > 1)
                            {
                                target[tp++] = 235;    // FNC4.
                                target[tp++] = (byte)(source[sourceIndex] - 128 + 1);
                            }

                            else if (gs1 > 0 && source[sourceIndex] == '\x1d')
                            {
                                if (gs1 == 2)
                                {
                                    target[tp++] = 29 + 1;     // GS.
                                }

                                else
                                {
                                    target[tp++] = 232;        // FNC1.
                                }
                            }

                            else
                            {
                                target[tp++] = (byte)(source[sourceIndex] + 1);
                            }
                        }
                    }
                }

            }

            else if (currentMode == X12)
            {
                if ((symbolsLeft == 1) && (processIndex == 1))
                {
                    // Unlatch not required!
                    target[tp++] = (byte)(source[length - 1] + 1);
                }

                else
                {
                    if (symbolsLeft > 0)
                    {
                        target[tp++] = 254;    // Unlatch.
                    }

                    if (processIndex == 1)
                    {
                        target[tp++] = (byte)(source[length - 1] + 1);
                    }

                    else if (processIndex == 2)
                    {
                        target[tp++] = (byte)(source[length - 2] + 1);
                        target[tp++] = (byte)(source[length - 1] + 1);
                    }
                }
            }

            else if (currentMode == EDIFACT)
            {
                if (symbolsLeft <= 2 && processIndex <= symbolsLeft)
                {
                    // Unlatch not required!
                    if (processIndex == 1)
                    {
                        target[tp++] = (byte)(source[length - 1] + 1);
                    }

                    else if (processIndex == 2)
                    {
                        target[tp++] = (byte)(source[length - 2] + 1);
                        target[tp++] = (byte)(source[length - 1] + 1);
                    }
                }

                else
                {
                    // Append EDIFACT unlatch value (31) and empty buffer.
                    if (processIndex <= 3)
                    {
                        processBuffer[processIndex++] = 31;
                    }

                    _ = EDIBufferTransfer(processBuffer, processIndex, target, ref tp, true);
                }

            }

            else if (currentMode == BASE256)
            {
                if (symbolsLeft > 0)
                {
                    tp = UpdateB256Length(target, tp, b256Start);
                }

                // B.2.1 255-state randomising algorithm.
                for (int i = b256Start; i < tp; i++)
                {
                    int prn = (149 * (i + 1) % 255) + 1;
                    target[i] = (byte)((target[i] + prn) & 0xff);
                }

            }
        }

        // Annex F placement alorithm low level.
        private void PlacementBit(int[] matrixLocations, int numberOfRows, int numberOfColumns, int row, int column, int position, byte offset)
        {
            if (row < 0)
            {
                row += numberOfRows;
                column += 4 - ((numberOfRows + 4) % 8);
            }

            if (column < 0)
            {
                column += numberOfColumns;
                row += 4 - ((numberOfColumns + 4) % 8);
            }

            // Necessary for DMRE (ISO/IEC 21471:2020 Annex E).
            if (row >= numberOfRows)
            {
                row -= numberOfRows;
            }

            matrixLocations[row * numberOfColumns + column] = (position << 3) + offset;
        }

        private void PlacementBlock(int[] matrixLocations, int numberOfRows, int numberOfColumns, int row, int column, int position)
        {
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 2, column - 2, position, 7);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 2, column - 1, position, 6);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 1, column - 2, position, 5);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 1, column - 1, position, 4);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 1, column - 0, position, 3);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 0, column - 2, position, 2);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 0, column - 1, position, 1);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, row - 0, column - 0, position, 0);
        }

        private void PlacementCornerA(int[] matrixLocations, int numberOfRows, int numberOfColumns, int position)
        {
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 0, position, 7);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 1, position, 6);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 2, position, 5);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 2, position, 4);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 1, position, 3);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 1, position, 2);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 2, numberOfColumns - 1, position, 1);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 3, numberOfColumns - 1, position, 0);
        }

        private void PlacementCornerB(int[] matrixLocations, int numberOfRows, int numberOfColumns, int position)
        {
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 3, 0, position, 7);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 2, 0, position, 6);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 0, position, 5);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 4, position, 4);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 3, position, 3);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 2, position, 2);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 1, position, 1);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 1, position, 0);
        }

        private void PlacementCornerC(int[] matrixLocations, int numberOfRows, int numberOfColumns, int position)
        {
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 3, 0, position, 7);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 2, 0, position, 6);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 0, position, 5);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 2, position, 4);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 1, position, 3);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 1, position, 2);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 2, numberOfColumns - 1, position, 1);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 3, numberOfColumns - 1, position, 0);
        }

        private void PlacementCornerD(int[] matrixLocations, int numberOfRows, int numberOfColumns, int position)
        {
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, 0, position, 7);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, numberOfRows - 1, numberOfColumns - 1, position, 6);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 3, position, 5);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 2, position, 4);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 0, numberOfColumns - 1, position, 3);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 3, position, 2);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 2, position, 1);
            PlacementBit(matrixLocations, numberOfRows, numberOfColumns, 1, numberOfColumns - 1, position, 0);
        }

        // Annex F placement alorithm main function.
        private void Placement(int[] matrixLocations, int numberOfRows, int numberOfColumns)
        {
            // Start.
            int position = 1;
            int row = 4;
            int column = 0;

            do
            {
                // Check corner.
                if (row == numberOfRows && column == 0)
                {
                    PlacementCornerA(matrixLocations, numberOfRows, numberOfColumns, position++);
                }

                if (row == numberOfRows - 2 && column == 0 && numberOfColumns % 4 != 0)
                {
                    PlacementCornerB(matrixLocations, numberOfRows, numberOfColumns, position++);
                }

                if (row == numberOfRows - 2 && column == 0 && (numberOfColumns % 8) == 4)
                {
                    PlacementCornerC(matrixLocations, numberOfRows, numberOfColumns, position++);
                }

                if (row == numberOfRows + 4 && column == 2 && (numberOfColumns % 8) == 0)
                {
                    PlacementCornerD(matrixLocations, numberOfRows, numberOfColumns, position++);
                }

                // Up/Right.
                do
                {
                    if (row < numberOfRows && column >= 0 && matrixLocations[row * numberOfColumns + column] == 0)
                    {
                        PlacementBlock(matrixLocations, numberOfRows, numberOfColumns, row, column, position++);
                    }

                    row -= 2;
                    column += 2;
                } while (row >= 0 && column < numberOfColumns);

                row++;
                column += 3;

                // Down/Left.
                do
                {
                    if (row >= 0 && column < numberOfColumns && matrixLocations[row * numberOfColumns + column] == 0)
                    {
                        PlacementBlock(matrixLocations, numberOfRows, numberOfColumns, row, column, position++);
                    }

                    row += 2;
                    column -= 2;
                } while (row < numberOfRows && column >= 0);

                row += 3;
                column++;
            } while (row < numberOfRows || column < numberOfColumns);

            // Unfilled corner.
            if (matrixLocations[numberOfRows * numberOfColumns - 1] == 0)
            {
                matrixLocations[numberOfRows * numberOfColumns - 1] = matrixLocations[numberOfRows * numberOfColumns - numberOfColumns - 2] = 1;
            }
        }

        // Calculate and append error correction code, and if necessary interleave.
        private void AddErrorCorrection(byte[] target, int bytes, int dataBlocks, int eccBlocks, bool skew)
        {
            byte[] dataCodewords;
            byte[] eccCodewords;
            int length;
            int blocks = (bytes + 2) / dataBlocks;

            // Allocate space for error correction bytes.
            int totalEccBlocks = blocks * eccBlocks;
            ReedSolomon.RSInitialise(0x12d, eccBlocks, 1);

            for (int b = 0; b < blocks; b++)
            {
                length = 0;
                dataCodewords = new byte[dataBlocks];
                eccCodewords = new byte[eccBlocks];

                for (int n = b; n < bytes; n += blocks)
                {
                    dataCodewords[length++] = target[n];
                }

                ReedSolomon.RSEncode(length, dataCodewords, eccCodewords);
                length = eccBlocks - 1;	// Comes back reversed

                for (int n = b; n < totalEccBlocks; n += blocks)
                {
                    if (skew)
                    {
                        // Rotate ecc data to make 144x144 size symbols acceptable.
                        // See http://groups.google.com/group/postscriptbarcode/msg/5ae8fda7757477da
                        if (b < 8)
                        {
                            target[bytes + n + 2] = eccCodewords[length--];
                        }

                        else
                        {
                            target[bytes + n - 8] = eccCodewords[length--];
                        }
                    }

                    else
                    {
                        target[bytes + n] = eccCodewords[length--];
                    }
                }
            }
        }

        // Is basic (non-shifted) C40?
        private bool IsC40(char value)
        {
            return char.IsDigit(value) || char.IsUpper(value) || value == ' ';
        }

        // Is basic (non-shifted) TEXT?
        private bool IsText(char value)
        {
            return char.IsDigit(value) || char.IsLower(value) || value == ' ';
        }

        // Is basic (non-shifted) C40 or TEXT?
        private bool IsC40Text(int currentMode, char value)
        {
            return currentMode == C40 ? IsC40(value) : IsText(value);
        }

        // Return true if a character is valid in X12 set.
        private bool IsX12(char value)
        {
            return IsC40(value) || value == 13 || value == '*' || value == '>';
        }

        // Return true if a character is valid in EDIFACT set.
        private bool IsEdifact(char value)
        {
            return value >= ' ' && value <= '^';
        }

        // Does Annex J section (r)(6)(ii)(I) apply?
        private bool SpecialX12(char[] source, int length, int sp)
        {
            // Annex J section (r)(6)(ii)(I)
            // "If one of the three X12 terminator/separator characters first
            // occurs in the yet to be processed data before a non-X12 character..."

            for (int i = sp; i < length && IsX12(source[i]); i++)
            {
                if (source[i] == 13 || source[i] == '*' || source[i] == '>')
                {
                    return true;
                }
            }

            return false;
        }

        // Count number of TEXT characters around `sp` between `position` and `length`
        // - helper to avoid exiting from Base 256 too early if have series of TEXT characters.
        private int TextSPCount(char[] source, int position, int length, int sp)
        {
            int i;
            int count = 0;

            // Count from `sp` forward.
            for (i = sp; i < length && IsText(source[i]); i++, count++) ;

            // Count backwards from `sp`.
            for (i = sp - 1; i >= position && IsText(source[i]); i--, count++) ;

            return count;
        }

        #region Look Ahead constants.

        // Character counts are multiplied by this, so as to be whole integer divisible by 2, 3 and 4.
        const int DM_MULT = 12;
        const int DM_MULT_1_DIV_2 = 6;
        const int DM_MULT_2_DIV_3 = 8;
        const int DM_MULT_3_DIV_4 = 9;
        const int DM_MULT_1 = 12;
        const int DM_MULT_4_DIV_3 = 16;
        const int DM_MULT_2 = 24;
        const int DM_MULT_8_DIV_3 = 32;
        const int DM_MULT_3 = 26;
        const int DM_MULT_13_DIV_4 = 39;
        const int DM_MULT_10_DIV_3 = 40;
        const int DM_MULT_4 = 48;
        const int DM_MULT_17_DIV_4 = 51;
        const int DM_MULT_13_DIV_3 = 52;
        const int DM_MULT_MINUS_1 = 11;
        private int DM_MULT_CEIL(int n) { return (n + DM_MULT_MINUS_1) / DM_MULT * DM_MULT; }

        #endregion

        // Look ahead test from Annex J.
        private int LookAheadTest(char[] source, int length, int position, int currentMode, int modeArg, int gs1)
        {
            int asciiCount, c40Count, textCount, x12Count, edfCount, b256Count;
            int asciiRounded, c40Rounded, textRounded, x12Rounded, edfRounded, b256Rounded;
            int count1;
            int sp;

            // Step (j).
            if (currentMode == ASCII || currentMode == BASE256)
            {
                // Adjusted to use for BASE256 also.
                asciiCount = 0;
                c40Count = DM_MULT_1;
                textCount = DM_MULT_1;
                x12Count = DM_MULT_1;
                edfCount = DM_MULT_1;
                b256Count = DM_MULT_2; // Adjusted from DM_MULT_5_DIV_4 (1.25).
            }

            else
            {
                asciiCount = DM_MULT_1;
                c40Count = DM_MULT_2;
                textCount = DM_MULT_2;
                x12Count = DM_MULT_2;
                edfCount = DM_MULT_2;
                b256Count = DM_MULT_3; // Adjusted from DM_MULT_9_DIV_4 (2.25).
            }

            switch (currentMode)
            {
                case C40:
                    c40Count = 0;
                    break;

                case TEXT:
                    textCount = 0;
                    break;

                case X12:
                    x12Count = 0;
                    break;

                case EDIFACT:
                    edfCount = 0;
                    break;

                case BASE256:
                    b256Count = modeArg == 249 ? DM_MULT_1 : 0; // Adjusted to use no. of bytes written.
                    break;
            }

            for (sp = position; sp < length; sp++)
            {
                char c = source[sp];
                bool isExtended = (c & 0x80) > 0;

                // ASCII ... Step (l).
                if (char.IsDigit(c))
                {
                    asciiCount += DM_MULT_1_DIV_2; // (l)(1).
                }

                else
                {
                    if (isExtended)
                    {
                        asciiCount = DM_MULT_CEIL(asciiCount) + DM_MULT_2; // (l)(2).
                    }

                    else
                    {
                        asciiCount = DM_MULT_CEIL(asciiCount) + DM_MULT_1; // (l)(3).
                    }
                }

                // C40 ... Step (m).
                if (IsC40(c))
                {
                    c40Count += DM_MULT_2_DIV_3; // (m)(1).
                }
                else
                {
                    if (isExtended)
                    {
                        c40Count += DM_MULT_8_DIV_3; // (m)(2).
                    }

                    else
                    {
                        c40Count += DM_MULT_4_DIV_3; // (m)(3).
                    }
                }

                // TEXT ... Step (n).
                if (IsText(c))
                {
                    textCount += DM_MULT_2_DIV_3; // (n)(1).
                }

                else
                {
                    if (isExtended)
                    {
                        textCount += DM_MULT_8_DIV_3; // (n)(2).
                    }

                    else
                    {
                        textCount += DM_MULT_4_DIV_3; // (n)(3).
                    }
                }

                // X12 ... Step (o).
                if (IsX12(c))
                {
                    x12Count += DM_MULT_2_DIV_3; // (o)(1).
                }

                else
                {
                    if (isExtended)
                    {
                        x12Count += DM_MULT_13_DIV_3; // (o)(2).
                    }

                    else
                    {
                        x12Count += DM_MULT_10_DIV_3; // (o)(3).
                    }
                }

                // EDIFACT ... Step (p).
                if (IsEdifact(c))
                {
                    edfCount += DM_MULT_3_DIV_4; // (p)(1).
                }

                else
                {
                    if (isExtended)
                    {
                        edfCount += DM_MULT_17_DIV_4; // (p)(2).
                    }

                    else
                    {
                        edfCount += DM_MULT_13_DIV_4; // (p)(3).
                    }
                }

                // Base 256 ... Step (q).
                if (gs1 == 1 && c == '\x1d')
                {
                    // FNC1 separator.
                    b256Count += DM_MULT_4; // (q)(1).
                }

                else
                {
                    b256Count += DM_MULT_1; // (q)(2).
                }

                if (sp >= position + 3)
                {
                    // At least 4 data characters processed ... step (r).
                    // NOTE: previous behaviour was at least 5 (same as BWIPP).
                    count1 = asciiCount + DM_MULT_1;

                    // Adjusted from <= b256_count.
                    if (count1 < b256Count && count1 <= edfCount && count1 <= textCount && count1 <= x12Count
                            && count1 <= c40Count)
                    {
                        return ASCII; // Step (r)(1).
                    }

                    count1 = b256Count + DM_MULT_1;
                    if (count1 <= asciiCount || (count1 < edfCount && count1 < textCount && count1 < x12Count && count1 < c40Count))
                    {
                        return BASE256; // Step (r)(2).
                    }

                    count1 = edfCount + DM_MULT_1;
                    if (count1 < asciiCount && count1 < b256Count && count1 < textCount && count1 < x12Count && count1 < c40Count)
                    {
                        return EDIFACT; // Step (r)(3).
                    }

                    count1 = textCount + DM_MULT_1;
                    if (count1 < asciiCount && count1 < b256Count && count1 < edfCount && count1 < x12Count && count1 < c40Count)
                    {
                        // Adjusted to avoid early exit from Base 256 if have less than break-even sequence of TEXT chars.
                        if (currentMode == BASE256 && position + 6 < length)
                        {
                            if (TextSPCount(source, position, length, sp) >= 12)
                            {
                                return TEXT; // Step (r)(4).
                            }
                        }

                        else
                        {
                            return TEXT; // Step (r)(4).
                        }
                    }

                    count1 = x12Count + DM_MULT_1;
                    if (count1 < asciiCount && count1 < b256Count && count1 < edfCount && count1 < textCount && count1 < c40Count)
                    {
                        return X12; // Step (r)(5).
                    }

                    count1 = c40Count + DM_MULT_1;
                    if (count1 < asciiCount && count1 < b256Count && count1 < edfCount && count1 < textCount)
                    {
                        if (c40Count < x12Count)
                        {
                            return C40; // Step (r)(6)(i).
                        }

                        if (c40Count == x12Count)
                        {
                            if (SpecialX12(source, length, sp))
                            {
                                return X12; // Step (r)(6)(ii)(I).
                            }

                            return C40; // Step (r)(6)(ii)(II).
                        }
                    }
                }
            }

            // At the end of data ... Step (k).
            // Step (k)(1).
            asciiRounded = DM_MULT_CEIL(asciiCount);
            b256Rounded = DM_MULT_CEIL(b256Count);
            edfRounded = DM_MULT_CEIL(edfCount);
            textRounded = DM_MULT_CEIL(textCount);
            x12Rounded = DM_MULT_CEIL(x12Count);
            c40Rounded = DM_MULT_CEIL(c40Count);

            if (asciiRounded <= b256Rounded && asciiRounded <= edfRounded && asciiRounded <= textRounded && asciiRounded <= x12Rounded && asciiRounded <= c40Rounded)
            {
                return ASCII; // Step (k)(2).
            }

            if (b256Rounded < asciiRounded && b256Rounded < edfRounded && b256Rounded < textRounded && b256Rounded < x12Rounded && b256Rounded < c40Rounded)
            {
                return BASE256; // Step (k)(3).
            }

            // Adjusted from < x12_rnded.
            if (edfRounded < asciiRounded && edfRounded < b256Rounded && edfRounded < textRounded && edfRounded <= x12Rounded && edfRounded < c40Rounded)
            {
                return EDIFACT; // Step (k)(4).
            }

            if (textRounded < asciiRounded && textRounded < b256Rounded && textRounded < edfRounded && textRounded < x12Rounded && textRounded < c40Rounded)
            {
                return TEXT; // Step (k)(5).
            }

            // Adjusted from < edf_rnded.
            if (x12Rounded < asciiRounded && x12Rounded < b256Rounded && x12Rounded <= edfRounded && x12Rounded < textRounded && x12Rounded < c40Rounded)
            {
                return X12; // Step (k)(6).
            }

            return C40; // Step (k)(7).
        }

        /// <summary>
        /// Copy C40/TEXT/X12 triplets from buffer to target.
        /// </summary>
        /// <param name="processBuffer">Process buffer.</param>
        /// <param name="processIndex">Number of elements left in the buffer(index).</param>
        /// <param name="target">Target codewords.</param>
        /// <param name="tp">Index to the current target length.</param>
        /// <returns>Elements left in buffer less than 3.</returns>
        private int CTXBufferTransfer(int[] processBuffer, int processIndex, byte[] target, ref int tp)
        {
            int processEnd = processIndex / 3 * 3;

            for (int i = 0; i < processEnd; i += 3)
            {
                int iv = (1600 * processBuffer[i]) + (40 * processBuffer[i + 1]) + processBuffer[i + 2] + 1;
                target[tp++] = (byte)(iv >> 8);
                target[tp++] = (byte)(iv & 0xff);
            }

            processIndex -= processEnd;

            if (processIndex > 0)
            {
                Array.Copy(processBuffer, processEnd, processBuffer, 0, processIndex);
            }

            return processIndex;
        }

        /// <summary>
        /// Copy EDIFACT quadruplets from buffer to target.
        /// </summary>
        /// <param name="processBuffer">Process buffer.</param>
        /// <param name="processIndex">Number of elements left in the buffer(index).</param>
        /// <param name="target">Target codewords.</param>
        /// <param name="tp">Index to the current target length.</param>
        /// <param name="emptyBuffer">Set true in the buffer is to be emptied.</param>
        /// <returns>Elements left in buffer(less than 4).</returns>
        private int EDIBufferTransfer(int[] processBuffer, int processIndex, byte[] target, ref int tp, bool emptyBuffer)
        {
            int i;
            int processEnd = processIndex / 4 * 4;

            for (i = 0; i < processEnd; i += 4)
            {
                target[tp++] = (byte)(processBuffer[i] << 2 | (processBuffer[i + 1] & 0x30) >> 4);
                target[tp++] = (byte)((processBuffer[i + 1] & 0x0f) << 4 | (processBuffer[i + 2] & 0x3c) >> 2);
                target[tp++] = (byte)((processBuffer[i + 2] & 0x03) << 6 | processBuffer[i + 3]);
            }

            processIndex -= processEnd;

            if (processIndex > 0)
            {
                Array.Copy(processBuffer, 0, processBuffer, processEnd, processIndex);
                if (emptyBuffer)
                {
                    if (processIndex == 3)
                    {
                        target[tp++] = (byte)(processBuffer[i] << 2 | (processBuffer[i + 1] & 0x30) >> 4);
                        target[tp++] = (byte)((processBuffer[i + 1] & 0x0f) << 4 | (processBuffer[i + 2] & 0x3c) >> 2);
                        target[tp++] = (byte)((processBuffer[i + 2] & 0x03) << 6);
                    }

                    else if (processIndex == 2)
                    {
                        target[tp++] = (byte)(processBuffer[i] << 2 | (processBuffer[i + 1] & 0x30) >> 4);
                        target[tp++] = (byte)((processBuffer[i + 1] & 0x0f) << 4);
                    }

                    else
                    {
                        target[tp++] = (byte)(processBuffer[i] << 2);
                    }

                    processIndex = 0;
                }
            }

            return processIndex;
        }

        // Get index of symbol size in codewords array `MatrixBytes`, as specified or
        // else smallest containing `minimum` codewords.
        private int GetSymbolSize(int minimum)
        {
            int i;

            if ((optionSymbolSize >= 1) && (optionSymbolSize <= DMSIZESCOUNT))
            {
                return IntSymbols[optionSymbolSize - 1];
            }

            if (minimum > 1304)
            {
                return minimum <= 1558 ? DMSIZESCOUNT - 1 : 0;
            }

            for (i = minimum >= 62 ? 23 : 0; minimum > MatrixBytes[i]; i++)
            {
                ;
            }

            if (optionDMRE)
            {
                return i;
            }

            if (optionSquareOnly)
            {
                // Skip rectangular symbols in square only mode.
                for (; MatrixHeights[i] != MatrixWidths[i]; i++)
                {
                    ;
                }

                return i;
            }

            // Skip DMRE symbols in no DMRE mode.
            for (; IsDMRE[i]; i++)
            {
                ;
            }

            return i;
        }

        // Number of codewords remaining in a particular version (may be negative).
        private int CodewordsRemaining(int tp, int process_p)
        {
            int symbolsize = GetSymbolSize(tp + process_p); // Allow for the remaining data characters.
            return MatrixBytes[symbolsize] - tp;
        }

        // Number of C40/TEXT elements needed to encode `value`.
        private int C40texCount(int currentMode, char value)
        {
            if (optionGS1Mode > 0 && value == '\x1d')
            {
                return 2;
            }

            int count = 1;

            if ((value & 0x80) > 0)
            {
                count += 2;
                value = (char)(value - 128);
            }

            if ((currentMode == C40 && C40Shift[value] > 0) || (currentMode == TEXT && TextShift[value] > 0))
            {
                count += 1;
            }

            return count;
        }

        // Update Base 256 field length.
        private int UpdateB256Length(byte[] target, int tp, int b256Start)
        {
            int b256Count = tp - (b256Start + 1);
            if (b256Count <= 249)
            {
                target[b256Start] = (byte)b256Count;
            }

            else
            {
                // Insert extra codeword.
                Array.Copy(target, b256Start + 1, target, b256Start + 2, b256Count);
                target[b256Start] = (byte)(249 + (b256Count / 250));
                target[b256Start + 1] = (byte)(b256Count % 250);
                tp++;
            }

            return tp;
        }

        // Switch from ASCII or Base 256 to another mode.
        private int SwitchMode(int nextMode, byte[] target, int tp, ref int b256Start)
        {
            switch (nextMode)
            {
                case ASCII:
                    break;

                case C40:
                    target[tp++] = 230;
                    break;

                case TEXT:
                    target[tp++] = 239;
                    break;

                case X12:
                    target[tp++] = 238;
                    break;

                case EDIFACT:
                    target[tp++] = 240;
                    break;

                case BASE256:
                    target[tp++] = 231;
                    b256Start = tp;
                    target[tp++] = 0; // Byte count holder (may be expanded to 2 codewords).
                    tp++;
                    break;
            }

            return tp;
        }

        // Determine if next 1 to 4 chars are at EOD and can be encoded as 1 or 2 ASCII codewords.
        private int LastASCII(char[] source, int length, int from)
        {
            if (length - from > 4 || from >= length)
            {
                return 0;
            }

            if (length - from == 1)
            {
                if ((source[from] & 0x80) > 0)
                {
                    return 0;
                }

                return 1;
            }

            if (length - from == 2)
            {
                if ((source[from] & 0x80) > 0 || (source[from + 1] & 0x80) > 0)
                {
                    return 0;
                }

                if (char.IsDigit(source[from]) && char.IsDigit(source[from + 1]))
                {
                    return 1;
                }

                return 2;
            }

            if (length - from == 3)
            {
                if (char.IsDigit(source[from]) && char.IsDigit(source[from + 1]) && !((source[from + 2] & 0x80) > 0))
                {
                    return 2;
                }

                if (char.IsDigit(source[from + 1]) && char.IsDigit(source[from + 2]) && !((source[from] & 0x80) > 0))
                {
                    return 2;
                }

                return 0;
            }

            if (char.IsDigit(source[from]) && char.IsDigit(source[from + 1]) && char.IsDigit(source[from + 2]) && char.IsDigit(source[from + 3]))
            {
                return 2;
            }

            return 0;
        }

        // Treat EDIFACT edges specially, returning DM_ASCII mode if not full (i.e. encoding < 4 chars), or if
        // full and at EOD where 1 or 2 ASCII chars can be encoded */
        private int GetEndMode(char[] source, int length, bool lastSeg, int mode, int from, int len, int size)
        {
            if (mode == EDIFACT)
            {
                if (len < 4)
                {
                    return ASCII;
                }

                if (lastSeg)
                {
                    int lastASCII = LastASCII(source, length, from + len);
                    if (lastASCII > 0)
                    {
                        // At EOD with remaining chars ASCII-encodable in 1 or 2 codewords.
                        int symbols_left = CodewordsRemaining(size + lastASCII, 0);

                        // If no codewords left and 1 or 2 ASCII-encodables or 1 codeword left and 1 ASCII-encodable.
                        if (symbols_left <= 2 - lastASCII)
                        {
                            return ASCII;
                        }
                    }
                }
            }

            return mode;
        }

        // Return number of C40/TEXT codewords needed to encode characters in full batches of 3 (or less if EOD).
        // The number of characters encoded is returned in `len`.
        private int GetNumberOfC40Words(char[] source, int length, int from, int mode, ref int len)
        {
            int thirdsCount = 0;
            int i;

            for (i = from; i < length; i++)
            {
                char ci = source[i];
                int remainder;

                if (IsC40Text(mode, ci))
                {
                    thirdsCount++; // Native.
                }

                else if (!((ci & 0x80) > 0))
                {
                    thirdsCount += 2; // Shift.
                }

                else if (IsC40Text(mode, (char)(ci & 0x7F)))
                {
                    thirdsCount += 3; // Shift, Upper shift.
                }


                else
                {
                    thirdsCount += 4; // Shift, Upper shift, shift.
                }

                remainder = thirdsCount % 3;
                if (remainder == 0 || (remainder == 2 && i + 1 == length))
                {
                    len = i - from + 1;
                    return (thirdsCount + 2) / 3 * 2;
                }
            }

            len = 0;
            return 0;
        }

        #region Edge Class.

        // The size of this class could be significantly reduced using techniques pointed out by Alex Geller,
        // but not done currently to avoid the processing overhead.
        class Edge
        {
            public int pos;         // Position of the edge within the edges array.
            public int mode;
            public int endMode;     // Mode returned by `dm_getEndMode()`.
            public int from;        // Position in input data, 0-based.
            public int len;
            public int size;        // Cumulative number of codewords.
            public int bytes;       // DM_BASE256 byte count, kept to avoid runtime calc.
            public int previous;    // Index into edges array.
        };

        #endregion

        // Initialize a new edge. Returns endMode.
        private int NewEdge(char[] source, int length, bool lastSeg, Edge[] edges, int mode, int from, int len, Edge previous, Edge edge, int cwds)
        {
            int previousMode;
            int size;

            edge.pos = 0;
            edge.mode = mode;
            edge.endMode = mode;
            edge.from = from;
            edge.len = len;
            edge.bytes = 0;
            if (previous != null)
            {
                previousMode = previous.endMode;
                edge.previous = previous.pos;
                size = previous.size;
            }

            else
            {
                previousMode = ASCII;
                edge.previous = 0;
                size = 0;
            }

            switch (mode)
            {
                case ASCII:
                    size++;
                    if ((source[from] & 0x80) > 0)
                    {
                        size++;
                    }

                    if (previousMode != ASCII && previousMode != BASE256)
                    {
                        size++; // Unlatch to ASCII.
                    }

                    break;

                case BASE256:
                    size++;
                    if (previousMode != BASE256)
                    {
                        size += 2; // Byte count + latch to BASE256.
                        if (previousMode != ASCII)
                        {
                            size++; // Unlatch to ASCII.
                        }

                        edge.bytes = 1;
                    }

                    else
                    {
                        edge.bytes = 1 + previous.bytes;
                        if (edge.bytes == 250)
                        {
                            size++; // Extra byte count.
                        }
                    }

                    break;

                case C40:
                case TEXT:
                    size += cwds;
                    if (previousMode != mode)
                    {
                        size++;     // Latch to this mode.
                        if (previousMode != ASCII && previousMode != BASE256)
                        {
                            size++; // Unlatch to ASCII.
                        }
                    }

                    if (lastSeg && from + len + 2 >= length)
                    {
                        // If less than batch of 3 away from EOD.
                        int last_ascii = LastASCII(source, length, from + len);
                        int symbols_left = CodewordsRemaining(size + last_ascii, 0);
                        if (symbols_left > 0)
                        {
                            size++; // We need an extra unlatch at the end.
                        }
                    }

                    break;

                case X12:
                    size += 2;
                    if (previousMode != X12)
                    {
                        size++; // Latch to this mode.
                        if (previousMode != ASCII && previousMode != BASE256)
                        {
                            size++; // Unlatch to ASCII.
                        }
                    }

                    if (lastSeg && from + len + 2 >= length)
                    {
                        // If less than batch of 3 away from EOD.
                        int last_ascii = LastASCII(source, length, from + len);
                        if (last_ascii == 2)
                        {
                            // Only 1 ASCII-encodable allowed at EOD for X12, unlike C40/TEXT.
                            size++; // We need an extra unlatch at the end.
                        }

                        else
                        {
                            int symbols_left = CodewordsRemaining(size + last_ascii, 0);
                            if (symbols_left > 0)
                            {
                                size++; // We need an extra unlatch at the end.
                            }
                        }
                    }

                    break;

                case EDIFACT:
                    size += 3;
                    if (previousMode != EDIFACT)
                    {
                        size++; // Latch to this mode.
                        if (previousMode != ASCII && previousMode != BASE256)
                        {
                            size++; // Unlatch to ASCII.
                        }
                    }

                    edge.endMode = GetEndMode(source, length, lastSeg, mode, from, len, size);
                    break;
            }

            edge.size = size;
            return edge.endMode;
        }

        // Add an edge for a mode at a vertex if no existing edge or if more optimal than existing edge.
        private void AddEdge(char[] source, int length, bool lastSeg, Edge[] edges, int mode, int from, int len, Edge previous, int cwds)
        {
            Edge edge = new Edge();
            int endMode = NewEdge(source, length, lastSeg, edges, mode, from, len, previous, edge, cwds);
            int vertexIndex = from + len;
            int v_ij = vertexIndex * NUM_MODES + endMode - 1;

            if (edges[v_ij].mode == 0 || edges[v_ij].size > edge.size)
            {
                edge.pos = v_ij;
                edges[v_ij] = edge;
            }

            else
            {
                ;
            }
        }

        // Add edges for the various modes at a vertex.
        private void AddEdges(char[] source, int length, bool lastSeg, Edge[] edges, int from, Edge previous, int gs1)
        {
            int[] c40text_modes = { C40, TEXT };
            int i, pos;

            // Not possible to unlatch a full EDF edge to something else.
            if (previous == null || previous.endMode != EDIFACT)

            {
                if (char.IsDigit(source[from]) && from + 1 < length && char.IsDigit(source[from + 1]))
                {
                    AddEdge(source, length, lastSeg, edges, ASCII, from, 2, previous, 0);
                    // If ASCII vertex, don't bother adding other edges as this will be optimal; suggested by Alex Geller.
                    if (previous != null && previous.mode == ASCII)
                    {
                        return;
                    }
                }

                else
                {
                    AddEdge(source, length, lastSeg, edges, ASCII, from, 1, previous, 0);
                }

                for (i = 0; i < c40text_modes.Length; i++)
                {
                    int len = 0;
                    int cwds = GetNumberOfC40Words(source, length, from, c40text_modes[i], ref len);
                    if (cwds > 0)
                    {
                        AddEdge(source, length, lastSeg, edges, c40text_modes[i], from, len, previous, cwds);
                    }
                }

                if (from + 2 < length && IsX12(source[from]) && IsX12(source[from + 1]) && IsX12(source[from + 2]))
                {
                    AddEdge(source, length, lastSeg, edges, X12, from, 3, previous, 0);
                }

                if (gs1 != 1 || source[from] != '\x1d')
                {
                    AddEdge(source, length, lastSeg, edges, BASE256, from, 1, previous, 0);
                }
            }

            if (IsEdifact(source[from]))
            {
                /* We create 3 EDF edges, 2, 3 or 4 characters length. The 4-char normally doesn't have a latch to ASCII
                   unless it is 2 characters away from the end of the input. */
                for (i = 1, pos = from + i; i < 4 && pos < length && IsEdifact(source[pos]); i++, pos++)
                {
                    AddEdge(source, length, lastSeg, edges, EDIFACT, from, i + 1, previous, 0);
                }
            }
        }

        // Calculate optimized encoding modes.
        private void DefineMode(char[] source, int[] modes, int length, bool lastSeg, int gs1)
        {
            int v_i;
            int minimalJ, minimalSize;
            int currentMode;
            int modeEnd, modeLength;
            Edge edge;
            Edge[] edges = new Edge[(length + 1) * NUM_MODES];

            // Initialise the edges array.
            for (int e = 0; e < edges.Length; e++)
            {
                edges[e] = new Edge();
            }

            edges[0].previous = -1;

            AddEdges(source, length, lastSeg, edges, 0, null, gs1);
            for (int i = 1; i < length; i++)
            {
                v_i = i * NUM_MODES;
                for (int j = 0; j < NUM_MODES; j++)
                {
                    if (edges[v_i + j].mode > 0)
                    {
                        AddEdges(source, length, lastSeg, edges, i, edges[v_i + j], gs1);
                    }
                }
            }

            v_i = length * NUM_MODES;
            minimalJ = -1;
            minimalSize = int.MaxValue;     // INT_MAX;
            for (int j = 0; j < NUM_MODES; j++)
            {
                edge = edges[v_i + j];
                if (edge.mode > 0)
                {
                    if (edge.size < minimalSize)
                    {
                        minimalSize = edge.size;
                        minimalJ = j;
                    }
                }

                else
                {
                    ;
                }
            }

            edge = edges[v_i + minimalJ];
            modeLength = 0;
            modeEnd = length;

            while (edge != null)
            {
                currentMode = edge.mode;
                modeLength += edge.len;
                edge = edge.previous > 0 ? edges[edge.previous] : null;
                if (edge == null || edge.mode != currentMode)
                {
                    for (int i = modeEnd - modeLength; i < modeEnd; i++)
                    {
                        modes[i] = currentMode;
                    }

                    modeEnd -= modeLength;
                    modeLength = 0;
                }
            }
        }

        // Do default minimal encodation.
        private void MinimalEncode(char[] source, int length, bool lastSeg, ref int sourceIndex, byte[] target, ref int tp,
                                  int[] processBuffer, ref int processIndex, ref int b256Start, ref int currentMode, int gs1)
        {
            int[] modes = new int[length];

            DefineMode(source, modes, length, lastSeg, gs1);
            while (sourceIndex < length)
            {
                if (modes[sourceIndex] != currentMode)
                {
                    switch (currentMode)
                    {
                        case C40:
                        case TEXT:
                        case X12:
                            processIndex = 0;      // Throw away buffer if any.
                            target[tp++] = 254;    // Unlatch.
                            break;

                        case EDIFACT:
                            if (lastSeg)
                            {
                                int lastAscii = LastASCII(source, length, sourceIndex);
                                if (lastAscii == 0)
                                {
                                    processBuffer[processIndex++] = 31; // Unlatch.
                                }

                                else
                                {
                                    int symbolsLeft = CodewordsRemaining(tp + lastAscii, processIndex);

                                    if (symbolsLeft > 2 - lastAscii)
                                    {
                                        processBuffer[processIndex++] = 31; /* Unlatch */
                                    }
                                }
                            }

                            processIndex = EDIBufferTransfer(processBuffer, processIndex, target, ref tp, true);
                            break;

                        case BASE256:
                            tp = UpdateB256Length(target, tp, b256Start);

                            // B.2.1 255-state randomising algorithm.
                            for (int i = b256Start; i < tp; i++)
                            {
                                int prn = (149 * (i + 1) % 255) + 1;
                                target[i] = (byte)((target[i] + prn) & 0xff);
                            }

                            break;
                    }

                    tp = SwitchMode(modes[sourceIndex], target, tp, ref b256Start);
                }

                currentMode = modes[sourceIndex];
                if (currentMode == ASCII)
                {
                    if (Common.IsTwoDigits(source, length, sourceIndex))
                    {
                        target[tp++] = (byte)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                        sourceIndex += 2;
                    }

                    else
                    {
                        if ((source[sourceIndex] & 0x80) > 1)
                        {
                            target[tp++] = 235;    // FNC4.
                            target[tp++] = (byte)(source[sourceIndex] - 128 + 1);
                        }

                        else
                        {
                            if (gs1 > 0 && source[sourceIndex] == '\x1d')
                            {
                                if (gs1 == 2)
                                {
                                    target[tp++] = 29 + 1;     // GS.
                                }

                                else
                                {
                                    target[tp++] = 232;        // FNC1.
                                }
                            }

                            else
                            {
                                target[tp++] = (byte)(source[sourceIndex] + 1);
                            }
                        }

                        sourceIndex++;
                    }

                }

                else if (currentMode == C40 || currentMode == TEXT)
                {

                    int shiftSet, value;
                    int[] ctShift, ctValue;

                    if (currentMode == C40)
                    {
                        ctShift = C40Shift;
                        ctValue = C40Values;
                    }
                    else
                    {
                        ctShift = TextShift;
                        ctValue = TextValues;
                    }

                    if ((source[sourceIndex] & 0x80) > 0)
                    {
                        processBuffer[processIndex++] = 1;
                        processBuffer[processIndex++] = 30;    // Upper Shift.
                        shiftSet = ctShift[source[sourceIndex] - 128];
                        value = ctValue[source[sourceIndex] - 128];
                    }

                    else
                    {
                        if (gs1 > 0 && source[sourceIndex] == '\x1d')
                        {
                            if (gs1 == 2)
                            {
                                shiftSet = ctShift[29];
                                value = ctValue[29];        // GS.
                            }

                            else
                            {
                                shiftSet = 2;
                                value = 27;                 // FNC1.
                            }
                        }

                        else
                        {
                            shiftSet = ctShift[source[sourceIndex]];
                            value = ctValue[source[sourceIndex]];
                        }
                    }

                    if (shiftSet != 0)
                    {
                        processBuffer[processIndex++] = shiftSet - 1;
                    }

                    processBuffer[processIndex++] = value;

                    if (processIndex >= 3)
                    {
                        processIndex = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);
                    }

                    sourceIndex++;

                }

                else if (currentMode == X12)
                {

                    string x12_nonalphanum_chars = "\015*> ";
                    int value = 0;

                    if (char.IsDigit(source[sourceIndex]))
                    {
                        value = source[sourceIndex] - '0' + 4;
                    }

                    else if (char.IsUpper(source[sourceIndex]))
                    {
                        value = source[sourceIndex] - 'A' + 14;
                    }

                    else
                    {
                        value = x12_nonalphanum_chars.IndexOf(source[sourceIndex]);
                    }

                    processBuffer[processIndex++] = value;

                    if (processIndex >= 3)
                    {
                        processIndex = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);
                    }
                    sourceIndex++;

                }

                else if (currentMode == EDIFACT)
                {

                    int value = source[sourceIndex];

                    if (value >= 64)
                    {
                        // '@'.
                        value -= 64;
                    }

                    processBuffer[processIndex++] = value;
                    sourceIndex++;

                    if (processIndex >= 4)
                    {
                        processIndex = EDIBufferTransfer(processBuffer, processIndex, target, ref tp, false);
                    }

                }

                else if (currentMode == BASE256)
                {

                    target[tp++] = (byte)source[sourceIndex++];
                }

                if (tp > 1558)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Data Matrix: Input too long, requires {0} codewords (maximum 1558).", tp));
                }

            } // end while.
        }

        // Encode using algorithm based on ISO/IEC 21471:2020 Annex J (was ISO/IEC 21471:2006 Annex P).
        private void ISOEncode(char[] source, int length, ref int sourceIndex, byte[] target, ref int tp, int[] processBuffer,
                               ref int processIndex, ref int b256Start, ref int currentMode, int gs1)
        {
            bool mailmark = symbolId == Symbology.Mailmark2D;
            bool notFirst = false;

            // Step (a).
            int nextMode = ASCII;

            if (mailmark)
            {
                // First 45 characters C40.
                nextMode = C40;
                tp = SwitchMode(nextMode, target, tp, ref b256Start);

                while (sourceIndex < 45)
                {
                    processBuffer[processIndex++] = C40Values[source[sourceIndex]];

                    if (processIndex >= 3)
                    {
                        processIndex = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);
                    }

                    sourceIndex++;
                }

                currentMode = nextMode;
                notFirst = true;
            }

            while (sourceIndex < length)
            {

                currentMode = nextMode;

                // Step (b) - ASCII encodation.
                if (currentMode == ASCII)
                {
                    nextMode = ASCII;

                    if (Common.IsTwoDigits(source, length, sourceIndex))
                    {
                        target[tp++] = (byte)((10 * (source[sourceIndex] - '0')) + (source[sourceIndex + 1] - '0') + 130);
                        sourceIndex += 2;
                    }
                    else
                    {
                        nextMode = LookAheadTest(source, length, sourceIndex, currentMode, 0, gs1);

                        if (nextMode != ASCII)
                        {
                            tp = SwitchMode(nextMode, target, tp, ref b256Start);
                            notFirst = false;
                        }

                        else
                        {
                            if ((source[sourceIndex] & 0x80) > 1)
                            {
                                target[tp++] = 235;    // FNC4.
                                target[tp++] = (byte)(source[sourceIndex] - 128 + 1);
                                tp += 2;
                            }

                            else
                            {
                                if (gs1 > 0 && source[sourceIndex] == '\x1d')
                                {
                                    if (gs1 == 2)
                                    {
                                        target[tp++] = 29 + 1; // GS.
                                    }

                                    else
                                    {
                                        target[tp++] = 232;    // FNC1.
                                    }
                                }

                                else
                                {
                                    target[tp++] = (byte)(source[sourceIndex] + 1);
                                }
                            }

                            sourceIndex++;
                        }
                    }

                    // Step (c)/(d) C40/TEXT encodation.
                }

                else if (currentMode == C40 || currentMode == TEXT)
                {

                    nextMode = currentMode;
                    if (processIndex == 0 && notFirst)
                    {
                        nextMode = LookAheadTest(source, length, sourceIndex, currentMode, processIndex, gs1);
                    }

                    if (nextMode != currentMode)
                    {
                        target[tp++] = 254;    // Unlatch.
                        nextMode = ASCII;
                    }

                    else
                    {
                        int shiftSet, value;
                        int[] ctShift, ctValue;

                        if (currentMode == C40)
                        {
                            ctShift = C40Shift;
                            ctValue = C40Values;
                        }

                        else
                        {
                            ctShift = TextShift;
                            ctValue = TextValues;
                        }

                        if ((source[sourceIndex] & 0x80) > 1)
                        {
                            processBuffer[processIndex++] = 1;
                            processBuffer[processIndex++] = 30;    // Upper Shift.
                            shiftSet = ctShift[source[sourceIndex] - 128];
                            value = ctValue[source[sourceIndex] - 128];
                        }

                        else
                        {
                            if (gs1 > 0 && source[sourceIndex] == '\x1d')
                            {
                                if (gs1 == 2)
                                {
                                    shiftSet = ctShift[29];
                                    value = ctValue[29];        // GS.
                                }

                                else
                                {
                                    shiftSet = 2;
                                    value = 27;                 // FNC1.
                                }
                            }

                            else
                            {
                                shiftSet = ctShift[source[sourceIndex]];
                                value = ctValue[source[sourceIndex]];
                            }
                        }

                        if (shiftSet != 0)
                        {
                            processBuffer[processIndex++] = shiftSet - 1;
                        }

                        processBuffer[processIndex++] = value;

                        if (processIndex >= 3)
                        {
                            processIndex = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);
                        }

                        sourceIndex++;
                        notFirst = true;
                    }
                }

                // Step (e) X12 encodation.
                else if (currentMode == X12)
                {

                    if (!IsX12(source[sourceIndex]))
                    {
                        nextMode = ASCII;
                    }

                    else
                    {
                        nextMode = X12;
                        if (processIndex == 0 && notFirst)
                        {
                            nextMode = LookAheadTest(source, length, sourceIndex, currentMode, processIndex, gs1);
                        }
                    }

                    if (nextMode != X12)
                    {
                        sourceIndex -= processIndex;    // About to throw away buffer, need to re-process input, cf Okapi commit [fb7981e].
                        processIndex = 0;               // Throw away buffer if any.
                        target[tp++] = 254;             // Unlatch.
                        nextMode = ASCII;
                    }

                    else
                    {
                        string x12_nonalphanum_chars = "\015*> ";
                        int value;

                        if (char.IsDigit(source[sourceIndex]))
                        {
                            value = source[sourceIndex] - '0' + 4;
                        }

                        else if (char.IsUpper(source[sourceIndex]))
                        {
                            value = source[sourceIndex] - 'A' + 14;
                        }

                        else
                        {
                            value = x12_nonalphanum_chars.IndexOf(source[sourceIndex]);
                        }

                        processBuffer[processIndex++] = value;

                        if (processIndex >= 3)
                        {
                            processIndex = CTXBufferTransfer(processBuffer, processIndex, target, ref tp);
                        }

                        sourceIndex++;
                        notFirst = true;
                    }
                }

                // Step (f) EDIFACT encodation.
                else if (currentMode == EDIFACT)
                {

                    if (!IsEdifact(source[sourceIndex]))
                    {
                        nextMode = ASCII;
                    }

                    else
                    {
                        nextMode = EDIFACT;
                        if (processIndex == 3)
                        {
                            // Note different than spec Step (f)(2), which suggests checking when 0, 
                            // but this seems towork better in many cases as the switch to ASCII is "free".
                            nextMode = LookAheadTest(source, length, sourceIndex, currentMode, processIndex, gs1);
                        }
                    }

                    if (nextMode != EDIFACT)
                    {
                        processBuffer[processIndex++] = 31;
                        processIndex = EDIBufferTransfer(processBuffer, processIndex, target, ref tp, true);
                        nextMode = ASCII;
                    }

                    else
                    {
                        int value = source[sourceIndex];

                        if (value >= 64)
                        {
                            // '@'.
                            value -= 64;
                        }

                        processBuffer[processIndex++] = value;
                        sourceIndex++;
                        notFirst = true;

                        if (processIndex >= 4)
                        {
                            processIndex = EDIBufferTransfer(processBuffer, processIndex, target, ref tp, false);
                        }
                    }
                }

                // Step (g) Base 256 encodation.
                else if (currentMode == BASE256)
                {

                    if (gs1 == 1 && source[sourceIndex] == '\x1d')
                    {
                        nextMode = ASCII;
                    }

                    else
                    {
                        nextMode = BASE256;
                        if (notFirst)
                        {
                            tp -= b256Start + 1;
                            nextMode = LookAheadTest(source, length, sourceIndex, currentMode, tp, gs1);
                        }
                    }

                    if (nextMode != BASE256)
                    {
                        tp = UpdateB256Length(target, tp, b256Start);

                        // B.2.1 255-state randomising algorithm.
                        for (int i = b256Start; i < tp; i++)
                        {
                            int prn = (149 * (i + 1) % 255) + 1;
                            target[i] = (byte)((target[i] + prn) & 0xff);
                        }

                        // Switch directly here to avoid flipping back to Base 256 due to `TextSPCount()`.
                        tp = SwitchMode(nextMode, target, tp, ref b256Start);
                        notFirst = false;
                    }

                    else
                    {
                        if (gs1 == 2 && source[sourceIndex] == '\x1d')
                        {
                            target[tp++] = 29;     // GS.
                        }

                        else
                        {
                            target[tp++] = (byte)source[sourceIndex];
                        }

                        sourceIndex++;
                        notFirst = true;
                    }
                }

                if (tp > 1558)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "Data Matrix: Input too long, requires too many codewords (maximum 1558)."));
                }

            } // end while.
        }

        private void AddPadding(byte[] target, int padLength, ref int tp)
        {
            int prn;

            for (int i = padLength; i > 0; i--)
            {
                if (i == padLength)
                {
                    target[tp++] = 129;
                }

                else
                {
                    prn = (149 * (tp + 1) % 253) + 130;
                    target[tp++] = (prn <= 254) ? (byte)prn : (byte)(prn - 254);
                }
            }
        }

        #region Mailmark 2D

        private void Mailmark2D(char[] barcodeData)
        {
            int maxLength = 90;
            int minLength = 32;
            int inputLength = barcodeData.Length;
            string prefix = "JGB ";

            barcodeData = ArrayHelper.ToUpper(barcodeData);

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < minLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Input data too short.\nMinimum length is {0} characters.", minLength));
            }

            if (inputLength < 39)
            {
                // Add space padding to RTS postcode field.
                string spaces = new string(' ', 39 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, barcodeData.Length, spaces);
                inputLength += 39 - inputLength;
            }

            if (inputLength < 45)
            {
                // Add space padding to Reserved field.
                string spaces = new string(' ', 45 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, barcodeData.Length, spaces);
                inputLength += 45 - inputLength;
            }

            if (optionSymbolSize != 8 && optionSymbolSize != 10 && optionSymbolSize != 30)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Invalid 2D Version '{0)' Versions 7, 9 or 29 expected.", optionSymbolSize - 1));
            }

            else
            {
                // Autosize.
                if (inputLength <= 51)
                {
                    optionSymbolSize = 8;
                }

                else if (inputLength <= 70)
                {
                    optionSymbolSize = 30;
                }

                else
                {
                    optionSymbolSize = 10;
                }
            }

            for (int i = 0; i < 45; i++)
            {
                if (CharacterSets.Mailmark4State.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Mailmark 2D: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            // Check prefix format.
            int pos = 0;
            if (new string(barcodeData, 0, 4) != prefix)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Invalid prefix. Expecting '{0}' at position {1}.", prefix, 1));
            }

            pos += 4;

            // Information Type ID.
            if (barcodeData[pos] != '0')
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Invalid Information Type ID '{0}' at position {1}.", barcodeData[4], 5));
            }

            pos++;

            // Version ID.
            if (barcodeData[pos] != '1')
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Mailmark 2D: Invalid Version ID ('1' only) '{0}' at position {1}.", barcodeData[5], 6));
            }

            pos++;

            // Class.
            bool result = int.TryParse(new string(barcodeData, pos, 1), NumberStyles.HexNumber, null, out int classId);
            if (!result || classId < 0 || classId > 14)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Mailmark 2D: Invalid Class ID.\nExpected 1-9, A-E at Position {0}.", 8));
            }

            pos++;

            // Supply Chain ID.
            int supplyChainLength = 7;
            for (int i = pos; i < pos + supplyChainLength; i++)
            {
                if (!char.IsDigit(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Mailmark 2D: Invalid Supply Chain ID.\nExpected numeric value at position {0}.", pos + 1));
                }
            }

            pos += supplyChainLength;

            // Item ID.
            int itemIdLength = 8;
            for (int i = pos; i < pos + itemIdLength; i++)
            {
                if (!char.IsDigit(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "MailMark 4 State: Invalid Item ID.\nExpected numeric value at position {0}.", pos + 1));
                }
            }

            pos += itemIdLength;

            // Destination Post Code plus DP field.
            int dpcLength = 9;
            string postcode = new string(barcodeData, pos, dpcLength);
            int type = 0;
            if (postcode != "         ")
            {
                int count = postcode.Length - postcode.TrimEnd(' ').Length;
                if (count > 2)
                {
                    // Trim off trailing spaces if there are more than 2.
                    postcode = postcode.TrimEnd(' ');
                }

                if (!MailmarkEncoder.VerifyPostcode(postcode, ref type, true))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                           "MailMark 2D: Invalid Destination Postcode and DPS."));
                }
            }

            pos += dpcLength;

            // Service Type.
            if (barcodeData[pos] < '0' || barcodeData[pos] > '6')
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "MailMark 2D: Invalid Service Type. Expected a value between 0 and 6."));
            }

            pos++;

            // Return to Sender Post Code.
            int rpcLength = 7;
            postcode = new string(barcodeData, pos, rpcLength);
            if (postcode != "       ")
            {
                // If not blank (allowed).
                if (!VerifyRTSPostcode(postcode, ref type))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                           "MailMark 2D: Invalid RTS Postcode."));
                }
            }

            pos += rpcLength;

            // Reserved space.
            int rsLength = 6;
            if (new string(barcodeData, pos, rsLength) != "      ")
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "MailMark 2D: Invalid Reserved field, must only contain spaces."));
            }

            pos += rsLength;
            int userDataLength = inputLength - pos;

            DataMatrix();
        }

        private bool VerifyRTSPostcode(string postcode, ref int postcodeType)
        {
            // Detect postcode type.
            if (postcode[5] == ' ')
            {
                postcodeType = 1;
            }

            else
            {
                if (postcode[6] == ' ')
                {
                    // Types 3 and 5.
                    if (char.IsDigit(postcode[1]))
                    {
                        if (char.IsDigit(postcode[2]))
                        {
                            postcodeType = 3;
                        }

                        else
                        {
                            postcodeType = 5;
                        }
                    }

                    else
                    {
                        postcodeType = 2;
                    }
                }

                else
                {
                    // Types 4 and 6.
                    if (char.IsDigit(postcode[3]))
                    {
                        postcodeType = 4;
                    }

                    else
                    {
                        postcodeType = 6;
                    }
                }
            }

            if (!VerifyCharacter(postcode, postcodeType))
            {
                return false;
            }

            return true;
        }

        private bool VerifyCharacter(string postcode, int type)
        {
            string[] postcodeFormat = new string[] {
            "ANNAASS", "AANNAAS", "ANNNAAS", "AANNNAA", "ANANAAS", "AANANAA" };

            string SetA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string SetN = "0123456789";
            string SetS = " ";
            int value = -1;

            char[] pattern = postcodeFormat[type - 1].ToCharArray();
            for (int i = 0; i < postcode.Length; i++)
            {
                switch (pattern[i])
                {
                    case 'A':
                        value = SetA.IndexOf(postcode[i]);
                        break;

                    case 'N':
                        value = SetN.IndexOf(postcode[i]);
                        break;

                    case 'S':
                        value = SetS.IndexOf(postcode[i]);
                        break;
                }

                // Break when encounter an invalid character.
                if (value == -1)
                {
                    break;
                }
            }

            return value != -1;
        }
    }

    #endregion
}

