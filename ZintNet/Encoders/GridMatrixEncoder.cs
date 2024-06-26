﻿/* GridMatrixEncodeEncoder.cs - Handles encoding Grid Matrix 2D symbol */

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ZintNet.Encoders
{
    internal class GridMatrixEncoder : SymbolEncoder
    {
        # region Tables

        private static char[] ShiftSet = {
            // From Table 7 - Encoding of control characters.
            (char)0x00, (char)0x01, (char)0x02, (char)0x03, (char)0x04, (char)0x05, (char)0x06, (char)0x07,
            (char)0x08, (char)0x09, (char)0x0a, (char)0x0b, (char)0x0c, (char)0x0d, (char)0x0e, (char)0x0f, /* NULL -> SI */
            (char)0x10, (char)0x11, (char)0x12, (char)0x13, (char)0x14, (char)0x15, (char)0x16, (char)0x17,
            (char)0x18, (char)0x19, (char)0x1a, (char)0x1b, (char)0x1c, (char)0x1d, (char)0x1e, (char)0x1f, /* DLE -> US */
            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':',
            ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~' };

        private static int[] RecommendCodewords = {
            9, 30, 59, 114, 170, 237, 315, 405, 506, 618, 741, 875, 1021 };

        private static int[] GMMaximumCodewords = {
            11, 40, 79, 146, 218, 305, 405, 521, 650, 794, 953, 1125, 1313 };

        private static int[] DataCodewords = {
            0, 15, 13, 11, 9,
            45, 40, 35, 30, 25,
            89, 79, 69, 59, 49,
            146, 130, 114, 98, 81,
            218, 194, 170, 146, 121,
            305, 271, 237, 203, 169,
            405, 360, 315, 270, 225,
            521, 463, 405, 347, 289,
            650, 578, 506, 434, 361,
            794, 706, 618, 530, 441,
            953, 847, 741, 635, 529,
            1125, 1000, 875, 750, 625,
            1313, 1167, 1021, 875, 729 };

        private static int[] GMN1 = {
            18, 50, 98, 81, 121, 113, 113, 116, 121, 126, 118, 125, 122 };

        private static int[] GMB1 = {
            1, 1, 1, 2, 2, 2, 2, 3, 2, 7, 5, 10, 6 };

        private static int[] GMB2 = {
            0, 0, 0, 0, 0, 1, 2, 2, 4, 0, 4, 0, 6 };

        // Values from table A.1.
        private static int[] GMEBValues = {
            /* E1 B3 E2 B4 */
            0, 0, 0, 0, // version 1
            3, 1, 0, 0,
            5, 1, 0, 0,
            7, 1, 0, 0,
            9, 1, 0, 0,
            5, 1, 0, 0, // version 2
            10, 1, 0, 0,
            15, 1, 0, 0,
            20, 1, 0, 0,
            25, 1, 0, 0,
            9, 1, 0, 0, // version 3
            19, 1, 0, 0,
            29, 1, 0, 0,
            39, 1, 0, 0,
            49, 1, 0, 0,
            8, 2, 0, 0, // version 4
            16, 2, 0, 0,
            24, 2, 0, 0,
            32, 2, 0, 0,
            41, 1, 40, 1,
            12, 2, 0, 0, // version 5
            24, 2, 0, 0,
            36, 2, 0, 0,
            48, 2, 0, 0,
            61, 1, 60, 1,
            11, 3, 0, 0, // version 6
            23, 1, 22, 2,
            34, 2, 33, 1,
            45, 3, 0, 0,
            57, 1, 56, 2,
            12, 1, 11, 3, // version 7
            23, 2, 22, 2,
            34, 3, 33, 1,
            45, 4, 0, 0,
            57, 1, 56, 3,
            12, 2, 11, 3, // version 8
            23, 5, 0, 0,
            35, 3, 34, 2,
            47, 1, 46, 4,
            58, 4, 57, 1,
            12, 6, 0, 0, // version 9
            24, 6, 0, 0,
            36, 6, 0, 0,
            48, 6, 0, 0,
            61, 1, 60, 5,
            13, 4, 12, 3, // version 10
            26, 1, 25, 6,
            38, 5, 37, 2,
            51, 2, 50, 5,
            63, 7, 0, 0,
            12, 6, 11, 3, // version 11
            24, 4, 23, 5,
            36, 2, 35, 7,
            47, 9, 0, 0,
            59, 7, 58, 2,
            13, 5, 12, 5, // version 12
            25, 10, 0, 0,
            38, 5, 37, 5,
            50, 10, 0, 0,
            63, 5, 62, 5,
            13, 1, 12, 11, //version 13
            25, 3, 24, 9,
            37, 5, 36, 7,
            49, 7, 48, 5,
            61, 9, 60, 3 };

        private static int[] MacroMatrix = {
            728, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650,
            727, 624, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 651,
            726, 623, 528, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 553, 652,
            725, 622, 527, 440, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 463, 554, 653,
            724, 621, 526, 439, 360, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 381, 464, 555, 654,
            723, 620, 525, 438, 359, 288, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 307, 382, 465, 556, 655,
            722, 619, 524, 437, 358, 287, 224, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 241, 308, 383, 466, 557, 656,
            721, 618, 523, 436, 357, 286, 223, 168, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 183, 242, 309, 384, 467, 558, 657,
            720, 617, 522, 435, 356, 285, 222, 167, 120, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 133, 184, 243, 310, 385, 468, 559, 658,
            719, 616, 521, 434, 355, 284, 221, 166, 119, 80, 49, 50, 51, 52, 53, 54, 55, 56, 91, 134, 185, 244, 311, 386, 469, 560, 659,
            718, 615, 520, 433, 354, 283, 220, 165, 118, 79, 48, 25, 26, 27, 28, 29, 30, 57, 92, 135, 186, 245, 312, 387, 470, 561, 660,
            717, 614, 519, 432, 353, 282, 219, 164, 117, 78, 47, 24, 9, 10, 11, 12, 31, 58, 93, 136, 187, 246, 313, 388, 471, 562, 661,
            716, 613, 518, 431, 352, 281, 218, 163, 116, 77, 46, 23, 8, 1, 2, 13, 32, 59, 94, 137, 188, 247, 314, 389, 472, 563, 662,
            715, 612, 517, 430, 351, 280, 217, 162, 115, 76, 45, 22, 7, 0, 3, 14, 33, 60, 95, 138, 189, 248, 315, 390, 473, 564, 663,
            714, 611, 516, 429, 350, 279, 216, 161, 114, 75, 44, 21, 6, 5, 4, 15, 34, 61, 96, 139, 190, 249, 316, 391, 474, 565, 664,
            713, 610, 515, 428, 349, 278, 215, 160, 113, 74, 43, 20, 19, 18, 17, 16, 35, 62, 97, 140, 191, 250, 317, 392, 475, 566, 665,
            712, 609, 514, 427, 348, 277, 214, 159, 112, 73, 42, 41, 40, 39, 38, 37, 36, 63, 98, 141, 192, 251, 318, 393, 476, 567, 666,
            711, 608, 513, 426, 347, 276, 213, 158, 111, 72, 71, 70, 69, 68, 67, 66, 65, 64, 99, 142, 193, 252, 319, 394, 477, 568, 667,
            710, 607, 512, 425, 346, 275, 212, 157, 110, 109, 108, 107, 106, 105, 104, 103, 102, 101, 100, 143, 194, 253, 320, 395, 478, 569, 668,
            709, 606, 511, 424, 345, 274, 211, 156, 155, 154, 153, 152, 151, 150, 149, 148, 147, 146, 145, 144, 195, 254, 321, 396, 479, 570, 669,
            708, 605, 510, 423, 344, 273, 210, 209, 208, 207, 206, 205, 204, 203, 202, 201, 200, 199, 198, 197, 196, 255, 322, 397, 480, 571, 670,
            707, 604, 509, 422, 343, 272, 271, 270, 269, 268, 267, 266, 265, 264, 263, 262, 261, 260, 259, 258, 257, 256, 323, 398, 481, 572, 671,
            706, 603, 508, 421, 342, 341, 340, 339, 338, 337, 336, 335, 334, 333, 332, 331, 330, 329, 328, 327, 326, 325, 324, 399, 482, 573, 672,
            705, 602, 507, 420, 419, 418, 417, 416, 415, 414, 413, 412, 411, 410, 409, 408, 407, 406, 405, 404, 403, 402, 401, 400, 483, 574, 673,
            704, 601, 506, 505, 504, 503, 502, 501, 500, 499, 498, 497, 496, 495, 494, 493, 492, 491, 490, 489, 488, 487, 486, 485, 484, 575, 674,
            703, 600, 599, 598, 597, 596, 595, 594, 593, 592, 591, 590, 589, 588, 587, 586, 585, 584, 583, 582, 581, 580, 579, 578, 577, 576, 675,
            702, 701, 700, 699, 698, 697, 696, 695, 694, 693, 692, 691, 690, 689, 688, 687, 686, 685, 684, 683, 682, 681, 680, 679, 678, 677, 676 };



        #endregion

        #region Constants

        // Encoding modes.
        private const char GM_CHINESE = 'H';
        private const char GM_NUMBER = 'N';
        private const char GM_LOWER = 'L';
        private const char GM_UPPER = 'U';
        private const char GM_MIXED = 'M';
        private const char GM_BYTE = 'B';
        // Must be in same order as GM_H etc.
        private readonly char[] modeTypes = { GM_CHINESE, GM_NUMBER, GM_LOWER, GM_UPPER, GM_MIXED, GM_BYTE };

        // Indexes into mode_types array.
        private const int GM_H = 0; // Chinese (Hanzi)
        private const int GM_N = 1; // Numeral
        private const int GM_L = 2; // Lower case
        private const int GM_U = 3; // Upper case
        private const int GM_M = 4; // Mixed
        private const int GM_B = 5; // Byte

        // Indexes to state array.
        const int GM_N_END = 0;         // Numeric end index.
        const int GM_N_COST = 1;        // Numeric cost.
        const int GM_BYTE_CNT = 2;      // Byte count index.

        private const int GM_NUM_MODES = 6;

        // Bits multiplied by this for costs, so as to be whole integer divisible by 2 and 3.
        private const int GM_MULT = 6;
        private readonly string NumeralNonDigits = " +-.,"; // Non-digit numeral set, excluding EOL (carriage return/linefeed)

        // Initial mode costs.
        static int[] InitHeadCosts = {
                /*  H            N (+pad prefix)    L            U            M            B (+byte count) */
                    4 * GM_MULT, (4 + 2) * GM_MULT, 4 * GM_MULT, 4 * GM_MULT, 4 * GM_MULT, (4 + 9) * GM_MULT };

        #endregion

        private int optionEccLevel;
        private int optionSymbolSize;

        public GridMatrixEncoder(Symbology symbology, string barcodeMessage, int optionSymbolSize, int optionEccLevel, int eci)
        {
            this.symbolId = symbology;
            this.barcodeMessage = barcodeMessage;
            this.optionEccLevel = optionEccLevel;
            this.optionSymbolSize = optionSymbolSize;
            this.eci = eci;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
            GridMatrix();
            return Symbol;
        }

        private void GridMatrix()
        {
            int size, modules, dark;
            int autoLayers, minimumLayers, layers;
            int autoEccLevel, minimumEccLevel, eccLevel;
            byte[] symbolGrid;
            int dataMaximum;
            int dataCodewords;
            bool inputLatch = false;
            int inputLength = barcodeData.Length;
            BitVector bitStream = new BitVector();
            int[] eccData = new int[1460];
            char[] gbData = new char[barcodeData.Length + 1];

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] <= 0xff)
                    gbData[i] = barcodeData[i];

                else
                    gbData[i] = GetGB2312Character(barcodeData[i]);
            }

            if (eci > 811799)
                throw new InvalidDataException("Grid Matrix: Invalid ECI value.");

            if (!GridMatrixEncode(bitStream, gbData, inputLength))
                throw new InvalidDataLengthException("Grid Matrix: Input data too long.");

            // Determine the size of the symbol.
            dataCodewords = bitStream.SizeInBits / 7;
            autoLayers = 13;
            for (int i = 12; i > 0; i--)
            {
                if (RecommendCodewords[(i - 1)] >= dataCodewords)
                    autoLayers = i;
            }

            minimumLayers = 13;
            for (int i = 12; i > 0; i--)
            {
                if (GMMaximumCodewords[(i - 1)] >= dataCodewords)
                    minimumLayers = i;
            }

            layers = autoLayers;
            if (optionSymbolSize >= 1 && optionSymbolSize <= 13)
            {
                inputLatch = true;
                if (optionSymbolSize >= minimumLayers)
                    layers = optionSymbolSize;

                else
                    throw new InvalidDataLengthException("Grid Matrix: Input data too long for selected symbol size.");
            }

            autoEccLevel = 3;
            if (layers == 1)
                autoEccLevel = 5;

            if ((layers == 2) || (layers == 3))
                autoEccLevel = 4;

            eccLevel = autoEccLevel;
            minimumEccLevel = 1;
            if (layers == 1)
                minimumEccLevel = 4;

            if ((layers == 2))
                minimumEccLevel = 2;

            if ((optionEccLevel >= 1) && (optionEccLevel <= 5))
            {
                if (optionEccLevel >= minimumEccLevel)
                    eccLevel = optionEccLevel;

                else
                    eccLevel = minimumEccLevel;
            }

            if (dataCodewords > DataCodewords[(5 * (layers - 1)) + (eccLevel - 1)])
            {
                if (inputLatch && eccLevel > minimumEccLevel)
                {
                    // If layers user-specified, try reducing ECC level first.
                    do
                    {
                        eccLevel--;
                    } while ((dataCodewords > DataCodewords[(5 * (layers - 1)) + (eccLevel - 1)]) && (eccLevel > minimumEccLevel));
                }

                while (dataCodewords > DataCodewords[(5 * (layers - 1)) + (eccLevel - 1)] && (layers < 13))
                {
                    layers++;
                }

                while (dataCodewords > DataCodewords[(5 * (layers - 1)) + (eccLevel - 1)] && eccLevel > 1)
                {
                    /* ECC min level 1 for layers > 2 */
                    eccLevel--;
                }
            }

            dataMaximum = 1313;
            switch (eccLevel)
            {
                case 2:
                    dataMaximum = 1167;
                    break;

                case 3:
                    dataMaximum = 1021;
                    break;

                case 4:
                    dataMaximum = 875;
                    break;

                case 5: dataMaximum = 729;
                    break;
            }

            if (dataCodewords > dataMaximum)
                throw new InvalidDataLengthException("Grid Matix: Input data too long.");

            AddErrorCorrection(bitStream, dataCodewords, layers, eccLevel, eccData);
            size = 6 + (layers * 12);
            modules = 1 + (layers * 2);
            symbolGrid = new byte[size * size];
            PlaceDataInGrid(symbolGrid, eccData, modules, size);
            PlaceLayerId(symbolGrid, size, layers, modules, eccLevel);

            // Add macro module frames.
            for (int x = 0; x < modules; x++)
            {
                dark = 1 - (x & 1);
                for (int y = 0; y < modules; y++)
                {
                    if (dark == 1)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            symbolGrid[((y * 6) * size) + (x * 6) + i] = 1;
                            symbolGrid[(((y * 6) + 5) * size) + (x * 6) + i] = 1;
                            symbolGrid[(((y * 6) + i) * size) + (x * 6)] = 1;
                            symbolGrid[(((y * 6) + i) * size) + (x * 6) + 5] = 1;
                        }

                        symbolGrid[(((y * 6) + 5) * size) + (x * 6) + 5] = 1;
                        dark = 0;
                    }

                    else
                        dark = 1;
                }
            }

            // Build the symbol.
            byte[] rowData;
            for (int y = 0; y < size; y++)
            {
                rowData = new byte[size];
                for (int x = 0; x < size; x++)
                    rowData[x] = symbolGrid[(size * y) + x];

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private static void PlaceMacroModule(byte[] symbolGrid, int x, int y, int word1, int word2, int size)
        {
            int i, j;

            i = (x * 6) + 1;
            j = (y * 6) + 1;

            if ((word2 & 0x40) > 0)
                symbolGrid[(j * size) + i + 2] = 1;

            if ((word2 & 0x20) > 0)
                symbolGrid[(j * size) + i + 3] = 1;

            if ((word2 & 0x10) > 0)
                symbolGrid[((j + 1) * size) + i] = 1;

            if ((word2 & 0x08) > 0)
                symbolGrid[((j + 1) * size) + i + 1] = 1;

            if ((word2 & 0x04) > 0)
                symbolGrid[((j + 1) * size) + i + 2] = 1;

            if ((word2 & 0x02) > 0)
                symbolGrid[((j + 1) * size) + i + 3] = 1;

            if ((word2 & 0x01) > 0)
                symbolGrid[((j + 2) * size) + i] = 1;

            if ((word1 & 0x40) > 0)
                symbolGrid[((j + 2) * size) + i + 1] = 1;

            if ((word1 & 0x20) > 0)
                symbolGrid[((j + 2) * size) + i + 2] = 1;

            if ((word1 & 0x10) > 0)
                symbolGrid[((j + 2) * size) + i + 3] = 1;

            if ((word1 & 0x08) > 0)
                symbolGrid[((j + 3) * size) + i] = 1;

            if ((word1 & 0x04) > 0)
                symbolGrid[((j + 3) * size) + i + 1] = 1;

            if ((word1 & 0x02) > 0)
                symbolGrid[((j + 3) * size) + i + 2] = 1;

            if ((word1 & 0x01) > 0)
                symbolGrid[((j + 3) * size) + i + 3] = 1;
        }

        private static void PlaceDataInGrid(byte[] symbolGrid, int[] eccData, int modules, int size)
        {
            int macroModule, offset;

            offset = 13 - ((modules - 1) / 2);
            for (int y = 0; y < modules; y++)
            {
                for (int x = 0; x < modules; x++)
                {
                    macroModule = MacroMatrix[((y + offset) * 27) + (x + offset)];
                    PlaceMacroModule(symbolGrid, x, y, eccData[macroModule * 2], eccData[(macroModule * 2) + 1], size);
                }
            }
        }

        // Place the layer ID into each macromodule.
        private static void PlaceLayerId(byte[] symbolGrid, int size, int layers, int modules, int eccLevel)
        {
            int layer, start, stop;
            int[] layerid = new int[layers + 1];
            int[] id = new int[modules * modules];

            // Calculate Layer IDs.
            for (int i = 0; i <= layers; i++)
            {
                if (eccLevel == 1)
                    layerid[i] = 3 - (i % 4);

                else
                    layerid[i] = (i + 5 - eccLevel) % 4;
            }

            for (int i = 0; i < modules; i++)
            {
                for (int j = 0; j < modules; j++)
                    id[(i * modules) + j] = 0;
            }

            // Calculate which value goes in each macro module.
            start = modules / 2;
            stop = modules / 2;
            for (layer = 0; layer <= layers; layer++)
            {
                for (int i = start; i <= stop; i++)
                {
                    id[(start * modules) + i] = layerid[layer];
                    id[(i * modules) + start] = layerid[layer];
                    id[((modules - start - 1) * modules) + i] = layerid[layer];
                    id[(i * modules) + (modules - start - 1)] = layerid[layer];
                }

                start--;
                stop++;
            }

            // Place the data in the grid.
            for (int i = 0; i < modules; i++)
            {
                for (int j = 0; j < modules; j++)
                {
                    if ((id[(i * modules) + j] & 0x02) > 0)
                        symbolGrid[(((i * 6) + 1) * size) + (j * 6) + 1] = 1;

                    if ((id[(i * modules) + j] & 0x01) > 0)
                        symbolGrid[(((i * 6) + 1) * size) + (j * 6) + 2] = 1;
                }
            }
        }

        private bool GridMatrixEncode(BitVector bitStream, char[] gbData, int inputLength)
        {
            int c1, c2;
            char nextMode;
            char lastMode = '\0';
            char currentMode = '\0';
            int sourceIndex = 0;
            int glyph = 0;
            int position = 0;
            int punctuation = 0;
            int punctuationPosition;
            int numberPadPosition = 0;
            int byteCountPosition = 0;
            int byteCount = 0;
            int shift;
            bool done = false;
            int[] numericBuffer = new int[3];
            char[] mode = new char[inputLength];
            if (eci != 0)
            {
                // ECI assignment according to Table 8.
                bitStream.AppendBits(12, 4); // ECI.
                if (eci <= 1023)
                    bitStream.AppendBits(eci, 11);

                if ((eci >= 1024) && (eci <= 32767))
                {
                    bitStream.AppendBits(2, 2);
                    bitStream.AppendBits(eci, 15);
                }

                if (eci >= 32768)
                {
                    bitStream.AppendBits(3, 2);
                    bitStream.AppendBits(eci, 20);
                }
            }

            DefineModes(mode, gbData, inputLength);
            do
            {
                nextMode = mode[sourceIndex];
                if (nextMode != currentMode)
                {
                    switch (currentMode)
                    {
                        case '\0':
                            switch (nextMode)
                            {
                                case GM_CHINESE:
                                    bitStream.AppendBits(1, 4);    // 0001.
                                    break;

                                case GM_NUMBER:
                                    bitStream.AppendBits(2, 4);    // 0010.
                                    break;

                                case GM_LOWER:
                                    bitStream.AppendBits(3, 4);    //0011.
                                    break;

                                case GM_UPPER:
                                    bitStream.AppendBits(4, 4);    // 0100.
                                    break;

                                case GM_MIXED:
                                    bitStream.AppendBits(5, 4);    // 0101
                                    break;

                                case GM_BYTE:
                                    bitStream.AppendBits(6, 4);    // 0111.
                                    break;
                            }
                            break;

                        case GM_CHINESE:
                            switch (nextMode)
                            {
                                case GM_NUMBER:
                                    bitStream.AppendBits(8161, 13);    // 1111111100001.
                                    break; // 8161

                                case GM_LOWER:
                                    bitStream.AppendBits(8162, 13);    // 1111111100010.
                                    break; // 8162

                                case GM_UPPER:
                                    bitStream.AppendBits(8163, 13);    // 1111111100011.
                                    break; // 8163

                                case GM_MIXED:
                                    bitStream.AppendBits(8164, 13);    // 1111111100100.
                                    break; // 8164

                                case GM_BYTE:
                                    bitStream.AppendBits(8165, 13);    // 1111111100101.
                                    break; // 8165
                            }
                            break;

                        case GM_NUMBER:
                            // Add numeric block padding value.
                            switch (position)
                            {
                                case 1:
                                    bitStream[numberPadPosition] = 1;
                                    bitStream[numberPadPosition + 1] = 0;
                                    break; // 2 pad digits

                                case 2:
                                    bitStream[numberPadPosition] = 0;
                                    bitStream[numberPadPosition + 1] = 1;
                                    break; // 1 pad digit

                                case 3:
                                    bitStream[numberPadPosition] = 0;
                                    bitStream[numberPadPosition + 1] = 0;
                                    break; // 0 pad digits
                            }

                            switch (nextMode)
                            {
                                case GM_CHINESE:
                                    bitStream.AppendBits(1019, 10);    // 1111111011.
                                    break; // 1019

                                case GM_LOWER:
                                    bitStream.AppendBits(1020, 10);    // 1111111100.
                                    break; // 1020

                                case GM_UPPER:
                                    bitStream.AppendBits(1021, 10);    // 1111111101.
                                    break; // 1021

                                case GM_MIXED:
                                    bitStream.AppendBits(1022, 10);    // 1111111110.
                                    break; // 1022

                                case GM_BYTE:
                                    bitStream.AppendBits(1023, 10);    // 1111111111
                                    break; // 1023
                            }
                            break;

                        case GM_LOWER:
                        case GM_UPPER:
                            switch (nextMode)
                            {
                                case GM_CHINESE:
                                    bitStream.AppendBits(28, 5);   // 11100.
                                    break; // 28

                                case GM_NUMBER:
                                    bitStream.AppendBits(29, 5);   // 11101.
                                    break; // 29

                                case GM_LOWER:
                                case GM_UPPER:
                                    bitStream.AppendBits(30, 5);   // 11110.
                                    break; // 30

                                case GM_MIXED:
                                    bitStream.AppendBits(124, 7);   // 1111100.
                                    break; // 124

                                case GM_BYTE:
                                    bitStream.AppendBits(126, 7);   // 1111110
                                    break; // 126
                            }
                            break;

                        case GM_MIXED:
                            switch (nextMode)
                            {
                                case GM_CHINESE:
                                    bitStream.AppendBits(1009, 10);    // 1111110001.
                                    break; // 1009

                                case GM_NUMBER:
                                    bitStream.AppendBits(1010, 10);    // 1111110010.
                                    break; // 1010

                                case GM_LOWER:
                                    bitStream.AppendBits(1011, 10);    // 1111110011.
                                    break; // 1011

                                case GM_UPPER:
                                    bitStream.AppendBits(1012, 10);    // 1111110100.
                                    break; // 1012

                                case GM_BYTE:
                                    bitStream.AppendBits(1015, 10);    // 1111110111.
                                    break; // 1015
                            }
                            break;

                        case GM_BYTE:
                            // Add byte block length indicator.
                            AddByteCount(bitStream, byteCountPosition, byteCount);
                            byteCount = 0;
                            switch (nextMode)
                            {
                                case GM_CHINESE:
                                    bitStream.AppendBits(1, 4);    // 0001
                                    break; // 1

                                case GM_NUMBER:
                                    bitStream.AppendBits(2, 4);    //0010.
                                    break; // 2

                                case GM_LOWER:
                                    bitStream.AppendBits(3, 4);    // 0011.
                                    break; // 3

                                case GM_UPPER:
                                    bitStream.AppendBits(4, 4);    // 0100.
                                    break; // 4

                                case GM_MIXED:
                                    bitStream.AppendBits(5, 4);    // 0101.
                                    break; // 5
                            }
                            break;
                    }
                }

                lastMode = currentMode;
                currentMode = nextMode;
                switch (currentMode)
                {
                    case GM_CHINESE:
                        done = false;
                        if (gbData[sourceIndex] > 0xff)
                        {
                            // GB2312 character.
                            c1 = (gbData[sourceIndex] & 0xff00) >> 8;
                            c2 = gbData[sourceIndex] & 0xff;

                            if ((c1 >= 0xa0) && (c1 <= 0xa9))
                                glyph = (0x60 * (c1 - 0xa1)) + (c2 - 0xa0);

                            if ((c1 >= 0xb0) && (c1 <= 0xf7))
                                glyph = (0x60 * (c1 - 0xb0 + 9)) + (c2 - 0xa0);

                            done = true;
                        }

                        if (!done)
                        {
                            if (sourceIndex != (inputLength - 1))
                            {
                                if ((gbData[sourceIndex] == 0x13) && (gbData[sourceIndex + 1] == 0x10))
                                {
                                    // End of line.
                                    glyph = 7776;
                                    sourceIndex++;
                                    done = true;
                                }
                            }
                        }

                        if (!done)
                        {
                            if (sourceIndex != (inputLength - 1))
                            {
                                if (Char.IsDigit(gbData[sourceIndex]) && Char.IsDigit(gbData[sourceIndex + 1]))
                                {
                                    // Two digits.
                                    glyph = 8033 + (10 * (gbData[sourceIndex] - '0')) + (gbData[sourceIndex + 1] - '0');
                                    sourceIndex++;
                                }
                            }
                        }

                        if (!done)
                            glyph = 7777 + gbData[sourceIndex]; // Byte value.

                        bitStream.AppendBits(glyph, 13);
                        sourceIndex++;
                        break;

                    case GM_NUMBER:
                        if (lastMode != currentMode)
                        {
                            // Reserve a space for numeric digit padding value (2 bits).
                            numberPadPosition = bitStream.SizeInBits;
                            bitStream.AppendBits(0, 2); // "XX"
                        }

                        position = 0;
                        punctuationPosition = -1;

                        // Numeric compression can also include certain combinations of non-numeric character.
                        numericBuffer[0] = '0';
                        numericBuffer[1] = '0';
                        numericBuffer[2] = '0';
                        do
                        {
                            if (Char.IsDigit(gbData[sourceIndex]))
                            {
                                numericBuffer[position] = gbData[sourceIndex];
                                position++;
                            }

                            else if (NumeralNonDigits.Contains(gbData[sourceIndex]))
                            {
                                if (punctuationPosition != -1)
                                    break;

                                punctuation = gbData[sourceIndex];
                                punctuationPosition = position;
                            }

                            else if (sourceIndex < (inputLength - 1) && (gbData[sourceIndex] == 0x13) && (gbData[sourceIndex + 1] == 0x10))
                            {
                                // End of line. 
                                if (punctuationPosition != -1)
                                    break;

                                punctuation = gbData[sourceIndex];
                                sourceIndex++;
                                punctuationPosition = position;
                            }

                            else
                                break;

                            sourceIndex++;
                        } while ((position < 3) && (sourceIndex < inputLength));

                        if (punctuationPosition != -1)
                        {
                            switch (punctuation)
                            {
                                case ' ':
                                    glyph = 0;
                                    break;

                                case '+':
                                    glyph = 3;
                                    break;

                                case '-':
                                    glyph = 6;
                                    break;

                                case '.':
                                    glyph = 9;
                                    break;

                                case ',':
                                    glyph = 12;
                                    break;

                                case 0x13:
                                    glyph = 15;
                                    break;
                            }

                            glyph += punctuationPosition;
                            glyph += 1000;
                            bitStream.AppendBits(glyph, 10);
                        }

                        glyph = (100 * (numericBuffer[0] - '0')) + (10 * (numericBuffer[1] - '0')) + (numericBuffer[2] - '0');
                        bitStream.AppendBits(glyph, 10);
                        break;

                    case GM_BYTE:
                        if (lastMode != currentMode)
                        {
                            // Reserve space for byte block length indicator (9 bits).
                            byteCountPosition = bitStream.SizeInBits;
                            bitStream.AppendBits(0, 9);    // LLLLLLLLL;
                        }

                        glyph = gbData[sourceIndex];
                        if (byteCount == 512 || (glyph > 0xFF && byteCount == 511))
                        {
                            // Maximum byte block size is 512 bytes. If longer, need to start a new block.
                            if (glyph > 0xFF && byteCount == 511)
                            {
                                // Split double-byte.
                                bitStream.AppendBits(glyph >> 8, 8);
                                glyph &= 0xFF;
                                byteCount++;
                            }

                            AddByteCount(bitStream, byteCountPosition, byteCount);
                            bitStream.AppendBits(7, 4);    // 0111.
                            byteCountPosition = bitStream.SizeInBits;
                            bitStream.AppendBits(0, 9);    // LLLLLLLLL;
                            byteCount = 0;
                        }

                        bitStream.AppendBits(glyph, glyph > 0xff ? 16 : 8);
                        sourceIndex++;
                        byteCount++;
                        if (glyph > 0xff)
                            byteCount++;

                        break;

                    case GM_MIXED:
                        shift = 1;
                        if (Char.IsDigit(gbData[sourceIndex]))
                            shift = 0;

                        if (Char.IsUpper(gbData[sourceIndex]))
                            shift = 0;

                        if (Char.IsLower(gbData[sourceIndex]))
                            shift = 0;

                        if (gbData[sourceIndex] == ' ')
                            shift = 0;

                        if (shift == 0)
                        {
                            // Mixed Mode character.
                            glyph = CharacterSets.GridMatrixSet.IndexOf(gbData[sourceIndex]);
                            bitStream.AppendBits(glyph, 6);
                        }

                        else
                        {
                            // Shift Mode character.
                            bitStream.AppendBits(1014, 10);    // 1014 - shift indicator.
                            AddShiftCharacter(bitStream, gbData[sourceIndex]);
                        }

                        sourceIndex++;
                        break;

                    case GM_UPPER:
                        shift = 1;
                        if (Char.IsUpper(gbData[sourceIndex]))
                            shift = 0;

                        if (gbData[sourceIndex] == ' ')
                            shift = 0;

                        if (shift == 0)
                        {
                            // Upper Case character.
                            glyph = CharacterSets.UpperCaseSet.IndexOf(gbData[sourceIndex]);
                            bitStream.AppendBits(glyph, 5);
                        }

                        else
                        {
                            // Shift Mode character.
                            bitStream.AppendBits(125, 7);      // Shift indicator.
                            AddShiftCharacter(bitStream, gbData[sourceIndex]);
                        }

                        sourceIndex++;
                        break;

                    case GM_LOWER:
                        shift = 1;
                        if (Char.IsLower(gbData[sourceIndex]))
                            shift = 0;

                        if (gbData[sourceIndex] == ' ')
                            shift = 0;

                        if (shift == 0)
                        {
                            // Lower Case character.
                            glyph = CharacterSets.LowerCaseSet.IndexOf(gbData[sourceIndex]);
                            bitStream.AppendBits(glyph, 5);
                        }

                        else
                        {
                            // Shift Mode character.
                            bitStream.AppendBits(125, 7);      // Shift indicator.
                            AddShiftCharacter(bitStream, gbData[sourceIndex]);
                        }

                        sourceIndex++;
                        break;
                }

                if (bitStream.SizeInBits > 9191)
                    return false;

            } while (sourceIndex < inputLength);

            if (currentMode == GM_NUMBER)
            {
                // Add numeric block padding value.
                switch (position)
                {
                    case 1:
                        bitStream[numberPadPosition] = 1;
                        bitStream[numberPadPosition + 1] = 0;
                        break; // 2 pad digits

                    case 2:
                        bitStream[numberPadPosition] = 0;
                        bitStream[numberPadPosition + 1] = 1;
                        break; // 1 pad digit

                    case 3: bitStream[numberPadPosition] = 0;
                        bitStream[numberPadPosition + 1] = 0;
                        break; // 0 pad digits
                }
            }

            if (currentMode == GM_BYTE)
            {
                // Add byte block length indicator.
                AddByteCount(bitStream, byteCountPosition, byteCount);
            }

            // Add "end of data" character.
            switch (currentMode)
            {
                case GM_CHINESE:
                    bitStream.AppendBits(8160, 13);
                    break;

                case GM_NUMBER:
                    bitStream.AppendBits(1018, 10);
                    break;

                case GM_LOWER:
                case GM_UPPER:
                    bitStream.AppendBits(27, 5);
                    break;

                case GM_MIXED:
                    bitStream.AppendBits(1008, 10);
                    break;

                case GM_BYTE:
                    bitStream.AppendBits(0, 4);
                    break;
            }

            // Add padding bits if required.
            position = 7 - (bitStream.SizeInBits % 7);
            if (position == 7)
                position = 0;

            if (position > 0)
                bitStream.AppendBits(0, position);

            if (bitStream.SizeInBits > 9191)
                return false;

            return true;
        }

        private static void AddErrorCorrection(BitVector binaryStream, int dataPosition, int layers, int eccLevel, int[] eccData)
        {
            int codewords, dataIndex;
            int n1, b1, n2, b2, e1, b3, e2;
            int blockSize, dataSize, eccSize;
            byte[] dataBlock;
            byte[] eccBlock;
            byte[] data = new byte[1320];
            byte[] block = new byte[130];

            codewords = DataCodewords[((layers - 1) * 5) + (eccLevel - 1)];
            // Convert from binary stream to 7-bit codewords.
            for (int i = 0; i < dataPosition; i++)
            {
                for (int p = 0; p < 7; p++)
                {
                    if (binaryStream[(i * 7) + p] == 1)
                        data[i] += (byte)(0x40 >> p);
                }
            }

            // Add padding codewords.
            data[dataPosition] = 0x00;
            for (int i = dataPosition + 1; i < codewords; i++)
            {
                if ((i & 1) == 1)
                    data[i] = 0x7e;

                else
                    data[i] = 0x00;
            }

            // Get block sizes.
            n1 = GMN1[(layers - 1)];
            b1 = GMB1[(layers - 1)];
            n2 = n1 - 1;
            b2 = GMB2[(layers - 1)];
            e1 = GMEBValues[((layers - 1) * 20) + ((eccLevel - 1) * 4)];
            b3 = GMEBValues[((layers - 1) * 20) + ((eccLevel - 1) * 4) + 1];
            e2 = GMEBValues[((layers - 1) * 20) + ((eccLevel - 1) * 4) + 2];

            // Split the data into blocks.
            dataIndex = 0;
            for (int i = 0; i < (b1 + b2); i++)
            {
                if (i < b1)
                    blockSize = n1;

                else
                    blockSize = n2;

                if (i < b3)
                    eccSize = e1;

                else
                    eccSize = e2;

                dataSize = blockSize - eccSize;
                dataBlock = new byte[dataSize];
                eccBlock = new byte[eccSize];
                for (int j = 0; j < dataSize; j++)
                {
                    dataBlock[j] = data[dataIndex];
                    dataIndex++;
                }

                // Calculate ECC data for this block.
                ReedSolomon.RSInitialise(0x89, eccSize, 1);
                ReedSolomon.RSEncode(dataSize, dataBlock, eccBlock);

                // Correct error correction data but in reverse order.
                for (int j = 0; j < dataSize; j++)
                    block[j] = dataBlock[j];

                for (int j = 0; j < eccSize; j++)
                    block[(j + dataSize)] = eccBlock[eccSize - j - 1];

                for (int j = 0; j < n2; j++)
                    eccData[((b1 + b2) * j) + i] = block[j];

                if (blockSize == n1)
                    eccData[((b1 + b2) * (n1 - 1)) + i] = block[(n1 - 1)];
            }
        }

        // Whether in numeral or not. If in numeral, numeralEnd is set to position after numeral, and numeralCost is set to per-numeral cost.
        private bool InNumeric(char[] data, int length, int posn, ref int numericEnd, ref int numericCost)
        {
            int i;
            int digitCount;
            int nonDigitPosition;
            int nonDigit;

            if (posn < numericEnd)
                return true;

            /* Attempt to calculate the average 'cost' of using numeric mode in number of bits (times GM_MULT) */
            /* Also ensures that numeric mode is not selected when it cannot be used: for example in
               a string which has "2.2.0" (cannot have more than one non-numeric character for each
               block of three numeric characters) */
            for (i = posn, digitCount = 0, nonDigit = 0, nonDigitPosition = 0; i < length && i < posn + 4 && digitCount < 3; i++)
            {
                if (Char.IsDigit(data[i]))
                    digitCount++;

                else if (NumeralNonDigits.Contains(data[i]))
                {
                    if (nonDigit > 0)
                        break;

                    nonDigit = 1;
                    nonDigitPosition = i;
                }

                else if (i < length - 1 && data[i] == 13 && data[i + 1] == 10)
                {
                    if (nonDigit > 0)
                        break;

                    i++;
                    nonDigit = 2;
                    nonDigitPosition = i;
                }

                else
                    break;
            }

            if (digitCount == 0)
            {
                // Must have at least one digit.
                numericEnd = 0;
                return false;
            }

            if (nonDigit > 0 && nonDigitPosition == i - 1)
            {
                // Non-digit can't be at end.
                nonDigit = 0;
            }

            numericEnd = posn + digitCount + nonDigit;
            // Calculate per-numeral cost where 120 == (10 + 10) * GM_MULT, 60 == 10 * GM_MULT.
            if (digitCount == 3)
                numericCost = nonDigit == 2 ? 24 /* (120 / 5) */ : nonDigit == 1 ? 30 /* (120 / 4) */ : 20 /* (60 / 3) */;

            else if (digitCount == 2)
                numericCost = nonDigit == 2 ? 30 /* (120 / 4) */ : nonDigit == 1 ? 40 /* (120 / 3) */ : 30 /* (60 / 2) */;

            else
                numericCost = nonDigit == 2 ? 40 /* (120 / 3) */ : nonDigit == 1 ? 60 /* (120 / 2) */ : 60 /* (60 / 1) */;

            return true;
        }

        private static char GetGB2312Character(char data)
        {
            byte[] gb2312Bytes;
            char[] gb2312 = new char[1];
            char gb2312Character;

            try
            {
                gb2312[0] = data;
                gb2312Bytes = Encoding.GetEncoding("GB2312",
                    new EncoderExceptionFallback(),
                    new DecoderExceptionFallback()).GetBytes(gb2312);
            }

            catch (EncoderFallbackException e)
            {
                throw new InvalidDataException("GB2312 Mode: Invalid byte sequence.", e);
            }

            gb2312Character = (char)(gb2312Bytes[0] << 8 | gb2312Bytes[1] & 0xff);
            return gb2312Character;
        }

        // Calculate optimized encoding modes. Adapted from Project Nayuki.
        private void DefineModes(char[] mode, char[] data, int inputLength)
        {
            int[] state = { 0 /*numeral_end*/, 0 /*numeral_cost*/, 0 /*byte_count*/ };
            CharacterModes.DefineModes(mode, data, inputLength, state, modeTypes, GM_NUM_MODES, HeadCost, SwitchCost, EodCost, CurrentCost);
        }

        private static int[] HeadCost(int[] state)
        {
            return InitHeadCosts;
        }

        // Cost of switching modes from k to j - see AIMD014 Rev. 1.63 Table 9 – Type conversion codes.
        private static int SwitchCost(int[] state, int k, int j)
        {
            int[,] switchCosts = {
                /*      H             N                   L             U             M             B  */
                /*H*/ {            0, (13 + 2) * GM_MULT, 13 * GM_MULT, 13 * GM_MULT, 13 * GM_MULT, (13 + 9) * GM_MULT },
                /*N*/ { 10 * GM_MULT,                  0, 10 * GM_MULT, 10 * GM_MULT, 10 * GM_MULT, (10 + 9) * GM_MULT },
                /*L*/ {  5 * GM_MULT,  (5 + 2) * GM_MULT,            0,  5 * GM_MULT,  7 * GM_MULT,  (7 + 9) * GM_MULT },
                /*U*/ {  5 * GM_MULT,  (5 + 2) * GM_MULT,  5 * GM_MULT,            0,  7 * GM_MULT,  (7 + 9) * GM_MULT },
                /*M*/ { 10 * GM_MULT, (10 + 2) * GM_MULT, 10 * GM_MULT, 10 * GM_MULT,            0, (10 + 9) * GM_MULT },
                /*B*/ {  4 * GM_MULT,  (4 + 2) * GM_MULT,  4 * GM_MULT,  4 * GM_MULT,  4 * GM_MULT,                  0 }, };

            return switchCosts[k, j];
        }

        // Final end-of-data cost - see AIMD014 Rev. 1.63 Table 9 – Type conversion codes.
        private static int EodCost(int k)
        {
            int[] eodCosts = {
                /*  H             N             L            U            M             B  */
                13 * GM_MULT, 10 * GM_MULT, 5 * GM_MULT, 5 * GM_MULT, 10 * GM_MULT, 4 * GM_MULT };

            return eodCosts[k];
        }

        /* Calculate cost of encoding current character */
        private void CurrentCost(int[] state, char[] data, int length, int position, char[] characterModes, int[] previousCosts, int[] currentCosts)
        {

            int cm_i = position * GM_NUM_MODES;
            bool doubleByte, space, numeric, lower, upper, control, doubleDigit, eol;

            doubleByte = data[position] > 0xFF;
            space = data[position] == ' ';
            numeric = Char.IsDigit(data[position]);
            lower = Char.IsLower(data[position]);
            upper = Char.IsUpper(data[position]);
            control = !space && !numeric && !lower && !upper && data[position] < 0x7F; /* Exclude DEL */
            doubleDigit = position < length - 1 && numeric && Char.IsDigit(data[position + 1]);
            eol = position < length - 1 && data[position] == 13 && data[position + 1] == 10;

            // Hanzi mode can encode anything.
            currentCosts[GM_H] = previousCosts[GM_H] + (doubleDigit || eol ? 39 : 78); // (6.5 : 13) * GM_MULT.
            characterModes[cm_i + GM_H] = GM_CHINESE;

            // Byte mode can encode anything.
            if (state[GM_BYTE_CNT] == 512 || (doubleByte && state[GM_BYTE_CNT] == 511))
            {
                currentCosts[GM_B] = InitHeadCosts[GM_B];
                if (doubleByte && state[GM_BYTE_CNT] == 511)
                {
                    currentCosts[GM_B] += 48;   // 8 * GM_MULT.
                    doubleByte = false;         // Splitting double-byte so mark as single.
                }

                state[GM_BYTE_CNT] = 0;
            }
            currentCosts[GM_B] += previousCosts[GM_B] + (doubleByte ? 96 : 48); // (16 : 8) * GM_MULT.
            characterModes[cm_i + GM_B] = GM_BYTE;
            state[GM_BYTE_CNT] += doubleByte ? 2 : 1;

            if (InNumeric(data, length, position, ref state[GM_N_END], ref state[GM_N_COST]))
            {
                currentCosts[GM_N] = previousCosts[GM_N] + state[GM_N_COST];
                characterModes[cm_i + GM_N] = GM_NUMBER;
            }

            if (control)
            {
                currentCosts[GM_L] = previousCosts[GM_L] + 78; // (7 + 6) * GM_MULT.
                characterModes[cm_i + GM_L] = GM_LOWER;
                currentCosts[GM_U] = previousCosts[GM_U] + 78; // (7 + 6) * GM_MULT.
                characterModes[cm_i + GM_U] = GM_UPPER;
                currentCosts[GM_M] = previousCosts[GM_M] + 96; // (10 + 6) * GM_MULT.
                characterModes[cm_i + GM_M] = GM_MIXED;
            }

            else
            {
                if (lower || space)
                {
                    currentCosts[GM_L] = previousCosts[GM_L] + 30; // 5 * GM_MULT.
                    characterModes[cm_i + GM_L] = GM_LOWER;
                }

                if (upper || space)
                {
                    currentCosts[GM_U] = previousCosts[GM_U] + 30; // 5 * GM_MULT.
                    characterModes[cm_i + GM_U] = GM_UPPER;
                }

                if (numeric || lower || upper || space)
                {
                    currentCosts[GM_M] = previousCosts[GM_M] + 36; // 6 * GM_MULT.
                    characterModes[cm_i + GM_M] = GM_MIXED;
                }
            }
        }

        // Add the length indicator for byte encoded blocks.
        private static void AddByteCount(BitVector binaryStream, int position, int byteCount)
        {
            byteCount--;
            for (int p = 0; p < 9; p++)
            {
                if ((byteCount & (0x100 >> p)) > 0)
                    binaryStream[position + p] = 1;

                else
                    binaryStream[position + p] = 0;
            }
        }

        // Add a control character to the data stream
        private static void AddShiftCharacter(BitVector binaryStream, int shiftY)
        {
            int glyph = 0;

            for (int i = 0; i < 64; i++)
            {
                if (ShiftSet[i] == shiftY)
                {
                    glyph = i;
                    break;
                }
            }

            binaryStream.AppendBits(glyph, 6);
        }
    }
}
