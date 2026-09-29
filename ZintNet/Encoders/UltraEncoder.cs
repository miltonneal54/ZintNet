/* UltraodeEncoder.cs - Handles Ultracode 2D symbol */

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
using System.Linq;
using System.Text;

namespace ZintNet.Encoders
{
    class UltraEncoder : SymbolEncoder
    {
        # region Tables and Constants
        private static string[] Fragments = {
            "http://", "https://", "http://www.", "https://www.",
            "ftp://", "www.", ".com", ".edu", ".gov", ".int", ".mil", ".net", ".org",
            ".mobi", ".coop", ".biz", ".info", "mailto:", "tel:", ".cgi", ".asp",
            ".aspx", ".php", ".htm", ".html", ".shtml", "file:" };

        private static string UltraC43Set1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 .,%";
        private static string UltraC43Set2 = "abcdefghijklmnopqrstuvwxyz:/?#[]@=_~!.,-";
        private static string UltraC43Set3 = "{}`()\"+'<>|$;&\\^*";
        private static string UltraDigitSet = "0123456789,/";
        private static string UltraColour = "WCBMRYGK";

        // According to Table 1.
        // static int[] ultra_maxsize = { 34, 78, 158, 282 };
        // Adjusted to allow 79-81 codeword range in 3-row symbols (only 1 secondary vertical clock track, not 2, so 3 extra).
        private static int[] UltraMaximumSize = { 34, 81, 158, 282 };
        // # Total Tile Columns from Table 1.
        private static int[] UltraMinimumColumns = { 5, 13, 23, 30 };
        // Value K(EC) from Table 12.
        private static int[] Kec = { 0, 1, 2, 4, 6, 8 };

        # region Old Tables - C# does not support octal literals.
        /*private static int[] dccu = {
            051363, 051563, 051653, 053153, 053163, 053513, 053563, 053613,     //  0-7
            053653, 056153, 056163, 056313, 056353, 056363, 056513, 056563,     //  8-15
            051316, 051356, 051536, 051616, 053156, 053516, 053536, 053616,     // 16-23
            053636, 053656, 056136, 056156, 056316, 056356, 056516, 056536 };   // 24-31*/

        /*private static int[] dccl = {
            061351, 061361, 061531, 061561, 061631, 061651, 063131, 063151,     //  0-7
            063161, 063531, 063561, 063631, 065131, 065161, 065351, 065631,     //  8-15
            031351, 031361, 031531, 031561, 031631, 031651, 035131, 035151,     // 16-23
            035161, 035361, 035631, 035651, 036131, 036151, 036351, 036531 };   // 24-31*/

        /* private static int[] tiles = {
             013135, 013136, 013153, 013156, 013163, 013165, 013513, 013515, 013516, 013531, //   0-9
             013535, 013536, 013561, 013563, 013565, 013613, 013615, 013616, 013631, 013635, //  10-19
             013636, 013651, 013653, 013656, 015135, 015136, 015153, 015163, 015165, 015313, //  20-29
             015315, 015316, 015351, 015353, 015356, 015361, 015363, 015365, 015613, 015615, //  30-39
             015616, 015631, 015635, 015636, 015651, 015653, 015656, 016135, 016136, 016153, //  40-49
             016156, 016165, 016313, 016315, 016316, 016351, 016353, 016356, 016361, 016363, //  50-59
             016365, 016513, 016515, 016516, 016531, 016535, 016536, 016561, 016563, 016565, //  60-69
             031315, 031316, 031351, 031356, 031361, 031365, 031513, 031515, 031516, 031531, //  70-79
             031535, 031536, 031561, 031563, 031565, 031613, 031615, 031631, 031635, 031636, //  80-89
             031651, 031653, 031656, 035131, 035135, 035136, 035151, 035153, 035156, 035161, //  90-99
             035163, 035165, 035315, 035316, 035351, 035356, 035361, 035365, 035613, 035615, // 100-109
             035616, 035631, 035635, 035636, 035651, 035653, 035656, 036131, 036135, 036136, // 110-119
             036151, 036153, 036156, 036163, 036165, 036315, 036316, 036351, 036356, 036361, // 120-129
             036365, 036513, 036515, 036516, 036531, 036535, 036536, 036561, 036563, 036565, // 130-139
             051313, 051315, 051316, 051351, 051353, 051356, 051361, 051363, 051365, 051513, // 140-149
             051516, 051531, 051536, 051561, 051563, 051613, 051615, 051616, 051631, 051635, // 150-159
             051636, 051651, 051653, 051656, 053131, 053135, 053136, 053151, 053153, 053156, // 160-169
             053161, 053163, 053165, 053513, 053516, 053531, 053536, 053561, 053563, 053613, // 170-179
             053615, 053616, 053631, 053635, 053636, 053651, 053653, 053656, 056131, 056135, // 180-189
             056136, 056151, 056153, 056156, 056161, 056163, 056165, 056313, 056315, 056316, // 190-199
             056351, 056353, 056356, 056361, 056363, 056365, 056513, 056516, 056531, 056536, // 200-209
             056561, 056563, 061313, 061315, 061316, 061351, 061353, 061356, 061361, 061363, // 210-219
             061365, 061513, 061515, 061516, 061531, 061535, 061536, 061561, 061563, 061565, // 220-229
             061615, 061631, 061635, 061651, 061653, 063131, 063135, 063136, 063151, 063153, // 230-239
             063156, 063161, 063163, 063165, 063513, 063515, 063516, 063531, 063535, 063536, // 240-249
             063561, 063563, 063565, 063613, 063615, 063631, 063635, 063651, 063653, 065131, // 250-259
             065135, 065136, 065151, 065153, 065156, 065161, 065163, 065165, 065313, 065315, // 260-269
             065316, 065351, 065353, 065356, 065361, 065363, 065365, 065613, 065615, 065631, // 270-279
             065635, 065651, 065653, 056565, 051515 };                                       // 280-284*/

        # endregion

        private static int[] Dccu = {
            21235, 21363, 21419, 22123, 22131, 22347, 22387, 22411,         //  0-7
            22443, 23659, 23667, 23755, 23787, 23795, 23883, 23923,         //  8-15
            21198, 21230, 21342, 21390, 22126, 22350, 22366, 22414,         // 16-23
            22430, 22446, 23646, 23662, 23758, 23790, 23886, 23902 };       // 24-31

        private static int[] Dccl = {
            25321, 25329, 25433, 25457, 25497, 25513, 26201, 26217,         //  0-7
            26225, 26457, 26481, 26521, 27225, 27249, 27369, 27545,         //  8-15
            13033, 13041, 13145, 13169, 13209, 13225, 14937, 14953,         // 16-23
            14961, 15089, 15257, 15273, 15449, 15465, 15593, 15705 };       // 24-31

        private static int[] Tiles = {
            5725, 5726, 5739, 5742, 5747, 5749, 5963, 5965, 5966, 5977,                 //   0-9
            5981, 5982, 6001, 6003, 6005, 6027, 6029, 6030, 6041, 6045,                 //  10-19
            6046, 6057, 6059, 6062, 6749, 6750, 6763, 6771, 6773, 6859,                 //  20-29
            6861, 6862, 6889, 6891, 6894, 6897, 6899, 6901, 7051, 7053,                 //  30-39
            7054, 7065, 7069, 7070, 7081, 7083, 7086, 7261, 7262, 7275,                 //  40-49
            7278, 7285, 7371, 7373, 7374, 7401, 7403, 7406, 7409, 7411,                 //  50-59
            7413, 7499, 7501, 7502, 7513, 7517, 7518, 7537, 7539, 7541,                 //  60-69
            13005, 13006, 13033, 13038, 13041, 13045, 13131, 13133, 13134, 13145,       //  70-79
            13149, 13150, 13169, 13171, 13173, 13195, 13197, 13209, 13213, 13214,       //  80-89
            13225, 13227, 13230, 14937, 14941, 14942, 14953, 14955, 14958, 14961,       //  90-99
            14963, 14965, 15053, 15054, 15081, 15086, 15089, 15093, 15243, 15245,       // 100-109
            15246, 15257, 15261, 15262, 15273, 15275, 15278, 15449, 15453, 15454,       // 110-119
            15465, 15467, 15470, 15475, 15477, 15565, 15566, 15593, 15598, 15601,       // 120-129
            15605, 15691, 15693, 15694, 15705, 15709, 15710, 15729, 15731, 15733,       // 130-139
            21195, 21197, 21198, 21225, 21227, 21230, 21233, 21235, 21237, 21323,       // 140-149
            21326, 21337, 21342, 21361, 21363, 21387, 21389, 21390, 21401, 21405,       // 150-159
            21406, 21417, 21419, 21422, 22105, 22109, 22110, 22121, 22123, 22126,       // 160-169
            22129, 22131, 22133, 22347, 22350, 22361, 22366, 22385, 22387, 22411,       // 170-179
            22413, 22414, 22425, 22429, 22430, 22441, 22443, 22446, 23641, 23645,       // 180-189
            23646, 23657, 23659, 23662, 23665, 23667, 23669, 23755, 23757, 23758,       // 190-199
            23785, 23787, 23790, 23793, 23795, 23797, 23883, 23886, 23897, 23902,       // 200-209
            23921, 23923, 25291, 25293, 25294, 25321, 25323, 25326, 25329, 25331,       // 210-219
            25333, 25419, 25421, 25422, 25433, 25437, 25438, 25457, 25459, 25461,       // 220-229
            25485, 25497, 25501, 25513, 25515, 26201, 26205, 26206, 26217, 26219,       // 230-239
            26222, 26225, 26227, 26229, 26443, 26445, 26446, 26457, 26461, 26462,       // 240-249
            26481, 26483, 26485, 26507, 26509, 26521, 26525, 26537, 26539, 27225,       // 250-259
            27229, 27230, 27241, 27243, 27246, 27249, 27251, 27253, 27339, 27341,       // 260-269
            27342, 27369, 27371, 27374, 27377, 27379, 27381, 27531, 27533, 27545,       // 270-279
            27549, 27561, 27563, 23925, 21325 };                                        // 280-284

        private const int EIGHTBIT_MODE = 10;
        private const int ASCII_MODE = 20;
        private const int C43_MODE = 30;
        private const int PREDICT_WINDOW = 12;
        private const int ULTRA_COMPRESSION = 128;
        # endregion

        private int optionCompression;
        private int optionEccLevel;

        public UltraEncoder(Symbology symbology, string barcodeMessage, int optionCompression, int optionEccLevel, int eci, EncodingMode mode)
        {
            this.symbolId = symbology;
            this.barcodeMessage = barcodeMessage;
            this.optionCompression = optionCompression;
            this.optionEccLevel = optionEccLevel;
            this.eci = eci;
            this.encodingMode = mode;
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

            UltraCode();
            return Symbol;
        }

        private void UltraCode()
        {
            int acc, qcc;
            int eccLevel;
            int rows, columns;
            int totalCodewords;
            int pads;
            int codewordSize;
            int position;
            int totalHeight, totalWidth;
            int tileX, tileY;
            int dataColumnCount;
            int[] dataCodewords;
            int dataCodewordCount = 0;
            char[] pattern;
            char[] tilePattern = new char[5];
            int[] codeword = new int[282 + 3]; // Allow for 3 pads in final 57th (60th incl. clock tracks) column of 5-row symbol (57 * 5 == 285)
            int inputLength = barcodeData.Length;

            codewordSize = inputLength * 2;
            if (codewordSize < 283)
                codewordSize = 283;

            if (eci > 811799)
                throw new InvalidDataException("Ultracode: Invalid ECI value.");

            dataCodewords = new int[codewordSize];
            dataCodewordCount = GenerateCodewords(inputLength, dataCodewords);
            dataCodewordCount += 2; // 2 == MCC + ACC (data codeword count includes start char).

            // Default ECC level is EC2.
            if ((optionEccLevel <= 0) || (optionEccLevel > 6))
                eccLevel = 2;

            else
                eccLevel = optionEccLevel - 1;

            // ECC calculation from section 7.7.2.
            if (eccLevel == 0)
                qcc = 3;

            else
            {
                if ((dataCodewordCount % 25) == 0)
                    qcc = (Kec[eccLevel] * (dataCodewordCount / 25)) + 3 + 2;

                else
                    qcc = (Kec[eccLevel] * ((dataCodewordCount / 25) + 1)) + 3 + 2;
            }

            acc = qcc - 3;

            // Maximum capacity is 282 codewords.
            totalCodewords = dataCodewordCount + qcc + 3; // 3 == TCC pattern + RSEC pattern + QCC pattern
            if (totalCodewords > 282)
                throw new InvalidDataLengthException("Ultracode: Data too long for selected error correction capacity.");

            rows = 5;
            for (int i = 2; i >= 0; i--)
            {
                if (totalCodewords - 6 <= UltraMaximumSize[i])  // Total codewords less 6 overhead (Start + MCC + ACC + 3 TCC/RSEC/QCC patterns)
                    rows--;
            }

            if ((totalCodewords % rows) == 0)
            {
                pads = 0;
                columns = totalCodewords / rows;
            }

            else
            {
                pads = rows - (totalCodewords % rows);
                columns = (totalCodewords / rows) + 1;
            }

            columns += columns / 15; // Secondary vertical clock tracks.

            // Insert MCC and ACC into data codewords.
            for (int i = 282; i > 2; i--)
                dataCodewords[i] = dataCodewords[i - 2];

            dataCodewords[1] = dataCodewordCount; // MCC
            dataCodewords[2] = acc; // ACC

            position = 0;
            // Calculate error correction codewords (RSEC).
            UltraGF283((short)dataCodewordCount, (short)qcc, dataCodewords);

            // Re-arrange to make final codeword sequence.
            codeword[position++] = dataCodewords[282 - (dataCodewordCount + qcc)]; // Start Character
            codeword[position++] = dataCodewordCount; // MCC
            for (int i = 0; i < qcc; i++)
                codeword[position++] = dataCodewords[(282 - qcc) + i]; // RSEC Region

            codeword[position++] = dataCodewordCount + qcc; // TCC = C + Q - section 6.11.4
            codeword[position++] = 283; // Separator
            codeword[position++] = acc; // ACC
            for (int i = 0; i < (dataCodewordCount - 3); i++)
                codeword[position++] = dataCodewords[(282 - ((dataCodewordCount - 3) + qcc)) + i]; // Data Region

            for (int i = 0; i < pads; i++)
                codeword[position++] = 284; // Pad pattern

            codeword[position++] = qcc; // QCC
            totalHeight = (rows * 6) + 1;
            totalWidth = columns + 6;

            // Build the symbol.
            pattern = new char[totalHeight * totalWidth];
            for (int i = 0; i < (totalHeight * totalWidth); i++)
                pattern[i] = 'W';

            // Border.
            for (int i = 0; i < totalWidth; i++)
            {
                pattern[i] = 'K'; // Top
                pattern[(totalHeight * totalWidth) - i - 1] = 'K'; // Bottom
            }

            for (int i = 0; i < totalHeight; i++)
            {
                pattern[totalWidth * i] = 'K'; // Left
                pattern[(totalWidth * i) + 3] = 'K';
                pattern[(totalWidth * i) + (totalWidth - 1)] = 'K'; // Right
            }

            // Clock tracks.
            for (int i = 0; i < totalHeight; i += 2)
            {
                pattern[(totalWidth * i) + 1] = 'K'; // Primary vertical clock track
                if (totalWidth > 20)
                    pattern[(totalWidth * i) + 19] = 'K'; // Secondary vertical clock track

                if (totalWidth > 36)
                    pattern[(totalWidth * i) + 35] = 'K'; // Secondary vertical clock track

                if (totalWidth > 52)
                    pattern[(totalWidth * i) + 51] = 'K'; // Secondary vertical clock track
            }

            for (int i = 6; i < totalHeight; i += 6)
            {
                for (int j = 5; j < totalWidth; j += 2)
                    pattern[(totalWidth * i) + j] = 'K'; // Horizontal clock track
            }

            // Place tiles.
            tileX = 0;
            tileY = 0;
            for (int i = 0; i < position; i++)
            {
                for (int j = 0; j < 5; j++)
                    tilePattern[4 - j] = UltraColour[(Tiles[codeword[i]] >> (3 * j)) & 0x07];

                if ((tileY + 1) >= totalHeight)
                {
                    tileY = 0;
                    tileX++;

                    if (tileX == 14)
                        tileX++;

                    if (tileX == 30)
                        tileX++;

                    if (tileX == 46)
                        tileX++;

                }

                for (int j = 0; j < 5; j++)
                    pattern[((tileY + j + 1) * totalWidth) + (tileX + 5)] = tilePattern[j];

                tileY += 6;
            }

            // Add data column count.
            dataColumnCount = columns - UltraMinimumColumns[rows - 2];
            tileX = 2;
            tileY = (totalHeight - 11) / 2;
            // DCCU.
            for (int j = 0; j < 5; j++)
                tilePattern[4 - j] = UltraColour[(Dccu[dataColumnCount] >> (3 * j)) & 0x07];

            for (int j = 0; j < 5; j++)
                pattern[((tileY + j) * totalWidth) + tileX] = tilePattern[j];

            // DCCL.
            tileY += 6;
            for (int j = 0; j < 5; j++)
                tilePattern[4 - j] = UltraColour[(Dccl[dataColumnCount] >> (3 * j)) & 0x07];

            for (int j = 0; j < 5; j++)
                pattern[((tileY + j) * totalWidth) + tileX] = tilePattern[j];

            // Build the symbol.
            byte[] rowData;
            for (int y = 0; y < totalHeight; y++)
            {
                rowData = new byte[totalWidth];
                for (int x = 0; x < totalWidth; x++)
                    rowData[x] = (byte)(pattern[(totalWidth * y) + x]);

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private int GenerateCodewords(int inputLength, int[] codewords)
        {
            int croppedLength;
            int symbolMode;
            int currentMode;
            int subset;
            float eightbitScore;
            float asciiScore;
            float c43Score;
            int endCharacter;
            int blockLength;
            int fragmentNumber;
            int codewordCount = 0;
            int position = 0;
            int fragmentLength = 0;
            int asciiEncoded = 0, c43Encoded = 0;

            char[] croppedSource = new char[inputLength];
            char[] mode = new char[inputLength + 1];
            int[] codewordFragment = new int[inputLength * 2 + 1];

            // Decide start character codeword (from Table 5)
            symbolMode = ASCII_MODE;
            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] >= 0x80)
                {
                    symbolMode = EIGHTBIT_MODE;
                    break;
                }
            }

            if (optionCompression != ULTRA_COMPRESSION && !isGS1)   // Force eight-bit mode by default as other modes are poorly documented
                symbolMode = EIGHTBIT_MODE;

            // Calculate start character codeword.
            if (symbolMode == ASCII_MODE)
            {
                if (isGS1)
                    codewords[0] = 273;

                else
                    codewords[0] = 272;
            }

            else
            {
                if ((eci >= 3) && (eci <= 18) && (eci != 14))
                {
                    // ECI indicates use of character set within ISO/IEC 8859
                    codewords[0] = 257 + (eci - 3);
                    if (codewords[0] > 267) // Avoids ECI 14 for non-existant ISO/IEC 8859-12.
                        codewords[0]--;
                }

                else if ((eci > 18) && (eci <= 898))
                {
                    // ECI indicates use of character set outside ISO/IEC 8859
                    codewords[0] = 275 + (eci / 256);
                    codewords[1] = eci % 256;
                    codewordCount++;
                }

                else if (eci == 899)    // Non-language byte data.
                    codewords[0] = 280;

                else if ((eci > 899) && (eci <= 9999))
                {
                    // ECI beyond 899 needs to use fixed length encodable ECI invocation (section 7.6.2)
                    // Encode as 3 codewords
                    codewords[0] = 257; // ISO/IEC 8859-1 used to enter 8-bit mode
                    codewords[1] = 274; // Encode ECI as 3 codewords
                    codewords[2] = (eci / 100) + 128;
                    codewords[3] = (eci % 100) + 128;
                    codewordCount += 3;
                }

                else if (eci >= 10000)
                {
                    // Encode as 4 codewords
                    codewords[0] = 257; // ISO/IEC 8859-1 used to enter 8-bit mode
                    codewords[1] = 275; // Encode ECI as 4 codewords
                    codewords[2] = (eci / 10000) + 128;
                    codewords[3] = ((eci % 10000) / 100) + 128;
                    codewords[4] = (eci % 100) + 128;
                    codewordCount += 4;
                }

                else
                    codewords[0] = 257; // Default is assumed to be ISO/IEC 8859-1 (ECI 3)
            }

            if ((codewords[0] == 257) || (codewords[0] == 272))
            {
                fragmentNumber = FindFragment(barcodeData, inputLength, 0);

                // Check for http:// at start of input.
                if ((fragmentNumber == 0) || (fragmentNumber == 2))
                {
                    codewords[0] = 281;
                    position = 7;
                    symbolMode = EIGHTBIT_MODE;
                }

                // Check for https:// at start of input.
                if ((fragmentNumber == 1) || (fragmentNumber == 3))
                {
                    codewords[0] = 282;
                    position = 8;
                    symbolMode = EIGHTBIT_MODE;
                }
            }

            codewordCount++;
            // Check for 06 Macro Sequence and crop accordingly.
            if (inputLength >= 9
                    && barcodeData[0] == '[' && barcodeData[1] == ')' && barcodeData[2] == '>' && barcodeData[3] == '\x1e'
                    && barcodeData[4] == '0' && barcodeData[5] == '6' && barcodeData[6] == '\x1d'
                    && barcodeData[inputLength - 2] == '\x1e' && barcodeData[inputLength - 1] == '\x04')
            {

                if (symbolMode == EIGHTBIT_MODE)
                    codewords[codewordCount] = 271; // 06 Macro

                else
                    codewords[codewordCount] = 273; // 06 Macro

                codewordCount++;
                for (int i = 7; i < (inputLength - 2); i++)
                    croppedSource[i - 7] = barcodeData[i];

                croppedLength = inputLength - 9;
                Array.Resize(ref croppedSource, croppedLength);
            }

            else
            {
                // Make a cropped version of input data - removes http:// and https:// if needed.
                for (int i = position; i < inputLength; i++)
                    croppedSource[i - position] = barcodeData[i];

                croppedLength = inputLength - position;
                Array.Resize(ref croppedSource, croppedLength);
            }

            // Attempt encoding in all three modes to see which offers best compaction and store results.
            if (optionCompression == ULTRA_COMPRESSION || isGS1)
            {
                currentMode = symbolMode;
                position = 0;
                do
                {
                    endCharacter = position + PREDICT_WINDOW;
                    eightbitScore = LookAheadEightbit(croppedSource, position, currentMode, endCharacter, codewordFragment, ref fragmentLength);
                    asciiScore = LookAheadAscii(croppedSource, position, currentMode, symbolMode, endCharacter, codewordFragment, ref fragmentLength, ref asciiEncoded);
                    subset = C43ShouldLatchOther(croppedSource, position, 1 /*subset*/) ? 2 : 1;
                    c43Score = LookAheadC43(croppedSource, position, currentMode, endCharacter, subset, codewordFragment, ref fragmentLength, ref c43Encoded);

                    mode[position] = 'a';
                    currentMode = ASCII_MODE;

                    if ((c43Score > asciiScore) && (c43Score > eightbitScore))
                    {
                        mode[position] = 'c';
                        currentMode = C43_MODE;
                    }

                    if ((eightbitScore > asciiScore) && (eightbitScore > c43Score))
                    {
                        mode[position] = '8';
                        currentMode = EIGHTBIT_MODE;
                    }

                    if (mode[position] == 'a')
                    {
                        for (int i = 0; i < asciiEncoded; i++)
                            mode[position + i] = 'a';

                        position += asciiEncoded;
                    }

                    else if (mode[position] == 'c')
                    {
                        for (int i = 0; i < c43Encoded; i++)
                            mode[position + i] = 'c';

                        position += c43Encoded;
                    }

                    else
                        position++;

                } while (position < croppedLength);
            }

            else
            {
                // Force eight-bit mode
                for (position = 0; position < croppedLength; position++)
                    mode[position] = '8';
            }

            // Use results from test to perform actual mode switching.
            currentMode = symbolMode;
            position = 0;
            do
            {
                blockLength = 0;
                do
                {
                    blockLength++;
                } while (mode[position + blockLength] == mode[position]);

                switch (mode[position])
                {
                    case 'a':
                        LookAheadAscii(croppedSource, position, currentMode, symbolMode, position + blockLength, codewordFragment, ref fragmentLength, ref asciiEncoded);
                        currentMode = ASCII_MODE;
                        break;

                    case 'c':
                        subset = C43ShouldLatchOther(croppedSource, position, 1 /*subset*/) ? 2 : 1;
                        LookAheadC43(croppedSource, position, currentMode, position + blockLength, subset, codewordFragment, ref fragmentLength, ref c43Encoded);

                        // Substitute temporary latch if possible.
                        if ((currentMode == EIGHTBIT_MODE) && (codewordFragment[0] == 260) && (fragmentLength >= 5) && (fragmentLength <= 11))
                            codewordFragment[0] = 256 + ((fragmentLength - 5) / 2); // Temporary latch to submode 1 from Table 11.

                        else if ((currentMode == EIGHTBIT_MODE) && (codewordFragment[0] == 266) && (fragmentLength >= 5) && (fragmentLength <= 11))
                            codewordFragment[0] = 262 + ((fragmentLength - 5) / 2); // Temporary latch to submode 2 from Table 11.

                        else if ((currentMode == ASCII_MODE) && (codewordFragment[0] == 278) && (fragmentLength >= 5) && (fragmentLength <= 11))
                            codewordFragment[0] = 274 + ((fragmentLength - 5) / 2); // Temporary latch to submode 1 from Table 9.

                        else
                            currentMode = C43_MODE;
                        break;

                    case '8':
                        LookAheadEightbit(croppedSource, position, currentMode, position + blockLength, codewordFragment, ref fragmentLength);
                        currentMode = EIGHTBIT_MODE;
                        break;
                }

                for (int i = 0; i < fragmentLength; i++)
                    codewords[codewordCount + i] = codewordFragment[i];

                codewordCount += fragmentLength;
                position += blockLength;
            } while (position < croppedLength);

            return codewordCount;
        }

        // Encode characters in 8-bit mode.
        private float LookAheadEightbit(char[] source, int position, int currentMode, int endCharacter, int[] codewords, ref int codewordLength)
        {
            int i;
            int codewordCount = 0;
            int lettersEncoded = 0;
            int inputLength = source.Length;

            if (currentMode != EIGHTBIT_MODE)
            {
                codewords[codewordCount] = 282; // Unlatch
                codewordCount += 1;
            }

            i = position;
            do
            {
                if ((source[i] == '[') && isGS1)
                    codewords[codewordCount] = 268; // FNC1

                else
                    codewords[codewordCount] = source[i];

                i++;
                codewordCount++;
            } while ((i < inputLength) && (i < endCharacter));

            lettersEncoded = i - position;
            codewordLength = codewordCount;

            if (codewordCount == 0)
                return 0.0f;

            else
                return (float)lettersEncoded / (float)codewordCount;
        }

        // Encode character in the ASCII mode/submode (including numeric compression).
        private float LookAheadAscii(char[] source, int position, int currentMode, int symbolMode, int endCharacter, int[] codewords, ref int codewordLength,
                                     ref int encoded)
        {
            int i;
            int firstDigit, secondDigit;
            bool done;
            int codewordCount = 0;
            int inputLength = source.Length;

            if (currentMode == EIGHTBIT_MODE)
            {
                codewords[codewordCount] = 267; // Latch ASCII Submode
                codewordCount++;
            }

            if (currentMode == C43_MODE)
            {
                codewords[codewordCount] = 282; // Unlatch
                codewordCount++;
                if (symbolMode == EIGHTBIT_MODE)
                {
                    codewords[codewordCount] = 267; // Latch ASCII Submode
                    codewordCount++;
                }
            }

            i = position;
            do
            {
                // Check for double digits.
                done = false;
                if (position != (inputLength - 1) && i < (inputLength - 1))
                {
                    firstDigit = UltraDigitSet.IndexOf(source[i]);
                    secondDigit = UltraDigitSet.IndexOf(source[i + 1]);
                    if ((firstDigit != -1) && (secondDigit != -1))
                    {
                        // Double digit can be encoded.
                        if ((firstDigit >= 0) && (firstDigit <= 9) && (secondDigit >= 0) && (secondDigit <= 9))
                        {
                            // Double digit numerics.
                            codewords[codewordCount] = (10 * firstDigit) + secondDigit + 128;
                            codewordCount++;
                            i += 2;
                            done = true;
                        }

                        else if ((firstDigit >= 0) && (firstDigit <= 9) && (secondDigit == 10))
                        {
                            // Single digit followed by selected decimal point character.
                            codewords[codewordCount] = firstDigit + 228;
                            codewordCount++;
                            i += 2;
                            done = true;
                        }

                        else if ((firstDigit == 10) && (secondDigit >= 0) && (secondDigit <= 9))
                        {
                            // Selected decimal point character followed by single digit.
                            codewords[codewordCount] = secondDigit + 238;
                            codewordCount++;
                            i += 2;
                            done = true;
                        }

                        else if ((firstDigit >= 0) && (firstDigit <= 9) && (secondDigit == 11))
                        {
                            // Single digit or decimal point followed by field deliminator.
                            codewords[codewordCount] = firstDigit + 248;
                            codewordCount++;
                            i += 2;
                            done = true;

                        }

                        else if ((firstDigit == 11) && (secondDigit >= 0) && (secondDigit <= 9))
                        {
                            // Field deliminator followed by single digit or decimal point.
                            codewords[codewordCount] = secondDigit + 259;
                            codewordCount++;
                            i += 2;
                            done = true;
                        }
                    }
                }

                if (!done && source[i] < 0x80)
                {
                    if ((source[i] == '[') && isGS1)
                        codewords[codewordCount] = 272; // FNC1

                    else
                        codewords[codewordCount] = source[i];

                    codewordCount++;
                    i++;
                }
            } while ((i < inputLength) && (i < endCharacter) && (source[i] < 0x80));

            encoded = i - position;
            codewordLength = codewordCount;

            if (codewordCount == 0)
                return 0.0f;

            else
                return (float)encoded / (float)codewordCount;
        }

        // Encode characters in the C43 compaction submode.
        private float LookAheadC43(char[] source, int position, int currentMode, int endCharacter, int subset, int[] codewords, ref int codewordLength, ref int encoded)
        {
            int fragmentNumber;
            int subPosition = position;
            int newSubset;
            int unshiftSet;
            int base43Value;
            int pad;
            int codewordCount = 0;
            int subCodewordCount = 0;
            int inputLength = source.Length;

            int[] subCodewords = new int[(inputLength + 3) * 2];
            if (currentMode == EIGHTBIT_MODE)
            {
                // Check for permissable URL C43 macro sequences, otherwise encode directly.
                fragmentNumber = FindFragment(source, inputLength, subPosition);
                if ((fragmentNumber == 2) || (fragmentNumber == 3))
                {
                    // http://www. -> http:
                    // https://www. -> https:
                    fragmentNumber -= 2;
                }

                switch (fragmentNumber)
                {
                    case 17: // mailto:
                        codewords[codewordCount] = 276;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    case 18: // tel:
                        codewords[codewordCount] = 277;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    case 26: // file:
                        codewords[codewordCount] = 278;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    case 0: // http://
                        codewords[codewordCount] = 279;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    case 1: // https://
                        codewords[codewordCount] = 280;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    case 4: // ftp://
                        codewords[codewordCount] = 281;
                        subPosition += Fragments[fragmentNumber].Length;
                        codewordCount++;
                        break;

                    default:
                        if (subset == 1)
                        {
                            codewords[codewordCount] = 260; // C43 compaction submode C1
                            codewordCount++;
                        }

                        if ((subset == 2) || (subset == 3))
                        {
                            codewords[codewordCount] = 266; // C43 compaction submode C2
                            codewordCount++;
                        }
                        break;
                }
            }

            if (currentMode == ASCII_MODE)
            {
                if (subset == 1)
                {
                    codewords[codewordCount] = 278; // C43 compaction submode C1
                    codewordCount++;
                }

                if ((subset == 2) || (subset == 3))
                {
                    codewords[codewordCount] = 280; // C43 compaction submode C2
                    codewordCount++;
                }
            }

            unshiftSet = subset;
            do
            {
                // Check for FNC1.
                if (isGS1 && source[subPosition] == '[')
                    break;

                newSubset = GetSubset(source, inputLength, subPosition, subset);
                if (newSubset == 0)
                    break;

                if ((newSubset != subset) && ((newSubset == 1) || (newSubset == 2)))
                {
                    if (C43ShouldLatchOther(source, subPosition, subset))
                    {
                        subCodewords[subCodewordCount] = 42; // Latch to other C43 set.
                        subCodewordCount++;
                        unshiftSet = newSubset;
                    }

                    else
                    {
                        subCodewords[subCodewordCount] = 40; // Shift to other C43 set for 1 character.
                        subCodewordCount++;
                        if (newSubset == 1)
                            subCodewords[subCodewordCount] = UltraC43Set1.IndexOf(source[subPosition]);

                        else
                            subCodewords[subCodewordCount] = UltraC43Set2.IndexOf(source[subPosition]);

                        subCodewordCount++;
                        subPosition++;
                        continue;
                    }
                }

                subset = newSubset;

                if (subset == 1)
                {
                    subCodewords[subCodewordCount] = UltraC43Set1.IndexOf(source[subPosition]);
                    subCodewordCount++;
                    subPosition++;
                }

                if (subset == 2)
                {
                    subCodewords[subCodewordCount] = UltraC43Set2.IndexOf(source[subPosition]);
                    subCodewordCount++;
                    subPosition++;
                }

                if (subset == 3)
                {
                    subCodewords[subCodewordCount] = 41; // Shift to set 3
                    subCodewordCount++;

                    fragmentNumber = FindFragment(source, inputLength, subPosition);
                    if (fragmentNumber == 26)
                        fragmentNumber = -1;

                    if ((fragmentNumber >= 0) && (fragmentNumber <= 18))
                    {
                        subCodewords[subCodewordCount] = fragmentNumber; // C43 Set 3 codewords 0 to 18
                        subCodewordCount++;
                        subPosition += Fragments[fragmentNumber].Length;
                    }

                    if ((fragmentNumber >= 19) && (fragmentNumber <= 25))
                    {
                        subCodewords[subCodewordCount] = fragmentNumber + 17; // C43 Set 3 codewords 36 to 42
                        subCodewordCount++;
                        subPosition += Fragments[fragmentNumber].Length;
                    }

                    if (fragmentNumber == -1)
                    {
                        subCodewords[subCodewordCount] = UltraC43Set3.IndexOf(source[subPosition]) + 19;    // C43 Set 3 codewords 19 to 35
                        subCodewordCount++;
                        subPosition++;
                    }

                    subset = unshiftSet;
                }

            } while ((subPosition < inputLength) && (subPosition < endCharacter));

            pad = 3 - (subCodewordCount % 3);
            if (pad == 3)
                pad = 0;

            for (int i = 0; i < pad; i++)
            {
                subCodewords[subCodewordCount] = 42; // Latch to other C43 set used as pad.
                subCodewordCount++;
            }

            encoded = subPosition - position;
            for (int i = 0; i < subCodewordCount; i += 3)
            {
                base43Value = (43 * 43 * subCodewords[i]) + (43 * subCodewords[i + 1]) + subCodewords[i + 2];
                codewords[codewordCount] = base43Value / 282;
                codewordCount++;
                codewords[codewordCount] = base43Value % 282;
                codewordCount++;
            }

            codewordLength = codewordCount;
            if (codewordCount == 0)
                return 0.0f;

            else
                return (float)encoded / (float)codewordCount;
        }

        // Returns true if should latch to subset other than given `subset`.
        private bool C43ShouldLatchOther(char[] source, int postion, int subset)
        {
            int fragmentLength, predictWindow;
            int fragmentNumber;
            int i, count, alternateCount;
            int inputLength = source.Length;

            string set = (subset == 1) ? UltraC43Set1 : UltraC43Set2;
            string alternateSet = (subset == 2) ? UltraC43Set1 : UltraC43Set2;

            if (postion + 3 > inputLength)
                return false;

            predictWindow = postion + 3;

            for (i = postion, count = 0, alternateCount = 0; i < predictWindow; i++)
            {
                if (source[i] <= 0x1F || source[i] >= 0x7F || (isGS1 && source[i] == '['))
                    break;

                fragmentNumber = FindFragment(source, inputLength, i);
                if (fragmentNumber != -1 && fragmentNumber != 26)
                {
                    fragmentLength = Fragments[fragmentNumber].Length;
                    predictWindow += fragmentLength;
                    if (predictWindow > inputLength)
                        predictWindow = inputLength;

                    i += fragmentLength - 1;
                }

                else
                {
                    if (set.IndexOf(source[i]) != -1)
                        count++;

                    if (alternateSet.IndexOf(source[i]) != -1)
                        alternateCount++;
                }
            }

            return alternateCount > count;
        }

        private int GetSubset(char[] source, int inputLength, int position, int currentSubset)
        {
            int fragmentNumber;
            int subset = 0;

            fragmentNumber = FindFragment(source, inputLength, position);
            if ((fragmentNumber != -1) && (fragmentNumber != 26))
                subset = 3;

            else if (currentSubset == 2)
            {
                if (UltraC43Set2.IndexOf(source[position]) != -1)
                    subset = 2;

                else if (UltraC43Set1.IndexOf(source[position]) != -1)
                    subset = 1;
            }

            else
            {
                if (UltraC43Set1.IndexOf(source[position]) != -1)
                    subset = 1;

                else if (UltraC43Set2.IndexOf(source[position]) != -1)
                    subset = 2;
            }

            if (subset == 0)
            {
                if (UltraC43Set3.IndexOf(source[position]) != -1)
                    subset = 3;
            }

            return subset;
        }

        private int FindFragment(char[] source, int inputLength, int position)
        {
            int j, k;
            bool latch = false;
            int retval = -1;

            for (j = 0; j < 27; j++)
            {
                latch = false;
                if ((position + Fragments[j].Length) <= inputLength)
                {
                    latch = true;
                    for (k = 0; k < Fragments[j].Length; k++)
                    {
                        if (source[position + k] != Fragments[j][k])
                        {
                            latch = false;
                            break;
                        }
                    }
                }

                if (latch)
                    retval = j;
            }

            return retval;
        }

        private void UltraGF283(short dataSize, short eccSize, int[] codewords)
        {
            /* Input is complete message codewords in array codewords[282]
             * DataSize is number of message codewords
             * EccSize is number of Reed-Solomon GF(283) check codewords to generate
             *
             * Upon exit, Message[282] contains complete 282 codeword Symbol Message
             * including leading zeroes corresponding to each truncated codeword */

            ushort[] gPoly = new ushort[283];
            ushort[] gfPwr = new ushort[(282 * 2)];
            ushort[] gfLog = new ushort[283];
            int j;
            ushort t;
            ushort result;

            // First build the log & antilog tables used in multiplication & division.
            UltraInitLogTables(gfPwr, gfLog);

            // Then generate the division polynomial of length EccSize.
            UltraGeneratePoly(eccSize, gPoly, gfPwr, gfLog);

            // Zero all EccSize codeword values.
            for (j = 281; (j > (281 - eccSize)); j--)
                codewords[j] = 0;

            // Shift message codewords to the right, leave space for ECC checkwords.
            for (int i = dataSize - 1; i >= 0; j--, i--)
                codewords[j] = codewords[i];

            // Add zeroes to pad left end Message[] for truncated codewords.
            j++;
            for (int i = 0; i < j; i++)
                codewords[i] = 0;

            // Generate (EccSize) Reed-Solomon checkwords.
            for (int n = j; n < (j + dataSize); n++)
            {
                t = (ushort)((codewords[j + dataSize] + codewords[n]) % 283);
                for (int i = 0; i < (eccSize - 1); i++)
                {
                    result = GfMultiply(t, gPoly[eccSize - 1 - i], gfPwr, gfLog);
                    codewords[j + dataSize + i] = (codewords[j + dataSize + i + 1] + 283 - result) % 283;
                }

                result = GfMultiply(t, gPoly[0], gfPwr, gfLog);
                codewords[j + dataSize + eccSize - 1] = (283 - result) % 283;
            }

            for (int i = j + dataSize; i < (j + dataSize + eccSize); i++)
                codewords[i] = (283 - codewords[i]) % 283;
        }

        // Generate the log and antilog tables for GF283() multiplication & division.
        void UltraInitLogTables(ushort[] gfPwr, ushort[] gfLog)
        {
            int i;

            for (int j = 0; j < 283; j++)
                gfLog[j] = 0;

            i = 1;
            for (int j = 0; j < 282; j++)
            {
                // j + 282 indicies save doing the modulo operation in GFMUL.
                gfPwr[j + 282] = gfPwr[j] = (ushort)i;
                gfLog[i] = (ushort)j;
                i = (i * 3) % 283;
            }
        }

        // Generate divisor polynomial gQ(x) for GF283() given the required ECC size, 3 to 101.
        void UltraGeneratePoly(short eccSize, ushort[] gfPoly, ushort[] gfPwr, ushort[] gfLog)
        {
            int i, j;
            ushort result;

            gfPoly[0] = 1;
            for (i = 1; i < (eccSize + 1); i++) gfPoly[i] = 0;

            for (i = 0; i < eccSize; i++)
            {
                for (j = i; j >= 0; j--)
                {
                    result = GfMultiply(gfPoly[j + 1], gfPwr[i + 1], gfPwr, gfLog);
                    gfPoly[j + 1] = (ushort)((gfPoly[j] + result) % 283);
                }

                result = GfMultiply(gfPoly[0], gfPwr[i + 1], gfPwr, gfLog);
                gfPoly[0] = result;
            }
            for (i = eccSize - 1; i >= 0; i -= 2)
                gfPoly[i] = (ushort)(283 - gfPoly[i]);
            // gPoly[i] is > 0 so modulo operation not needed.
        }

        private ushort GfMultiply(int i, int j, ushort[] gfPwr, ushort[] gfLog)
        {
            return (ushort)(((i == 0) || (j == 0)) ? 0 : gfPwr[gfLog[i] + gfLog[j]]);
        }
    }
}
