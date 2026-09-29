/* QRCodeEncoder.cs Handles QR and Micro QR 2D symbols */

/*
    ZintNetLib - a C# port of libzint.
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2009-2020 Robin Stuart <rstuart114@gmail.com>

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
using System.Data;
using System.Text;

using ArrayExt;

namespace ZintNet.Encoders
{
    internal class QRCodeEncoder : SymbolEncoder
    {
        #region Tables

        // From ISO/IEC 18004:2006 Table 7.
        private static int[] QRDataCodeWordsLow = {
	        19, 34, 55, 80, 108, 136, 156, 194, 232, 274, 324, 370, 428, 461, 523, 589, 647,
	        721, 795, 861, 932, 1006, 1094, 1174, 1276, 1370, 1468, 1531, 1631,
	        1735, 1843, 1955, 2071, 2191, 2306, 2434, 2566, 2702, 2812, 2956};

        private static int[] QRDataCodeWordsMedium = {
	        16, 28, 44, 64, 86, 108, 124, 154, 182, 216, 254, 290, 334, 365, 415, 453, 507,
	        563, 627, 669, 714, 782, 860, 914, 1000, 1062, 1128, 1193, 1267,
	        1373, 1455, 1541, 1631, 1725, 1812, 1914, 1992, 2102, 2216, 2334};

        private static int[] QRDataCodeWordsQuartile = {
	        13, 22, 34, 48, 62, 76, 88, 110, 132, 154, 180, 206, 244, 261, 295, 325, 367,
	        397, 445, 485, 512, 568, 614, 664, 718, 754, 808, 871, 911,
	        985, 1033, 1115, 1171, 1231, 1286, 1354, 1426, 1502, 1582, 1666};

        private static int[] QRDataCodeWordsHigh = {
	        9, 16, 26, 36, 46, 60, 66, 86, 100, 122, 140, 158, 180, 197, 223, 253, 283,
	        313, 341, 385, 406, 442, 464, 514, 538, 596, 628, 661, 701,
	        745, 793, 845, 901, 961, 986, 1054, 1096, 1142, 1222, 1276};

        private static int[] QRTotalCodeWords = {
            // Total code words Data + Ecc.
	        26, 44, 70, 100, 134, 172, 196, 242, 292, 346, 404, 466, 532, 581, 655, 733, 815,
	        901, 991, 1085, 1156, 1258, 1364, 1474, 1588, 1706, 1828, 1921, 2051,
	        2185, 2323, 2465, 2611, 2761, 2876, 3034, 3196, 3362, 3532, 3706};

        private static int[] QRBlocksLow = {
	        1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 7, 8, 8, 9, 9, 10, 12, 12,
	        12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25};

        private static int[] QRBlocksMedium = {
	        1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20,
	        21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49};

        private static int[] QRBlocksQuartile = {
	        1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25,
	        27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68};

        private static int[] QRBlocksHigh = {
	        1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30,
	        32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81};

        private static int[] QRSizes = {
	        21, 25, 29, 33, 37, 41, 45, 49, 53, 57, 61, 65, 69, 73, 77, 81, 85, 89, 93, 97,
	        101, 105, 109, 113, 117, 121, 125, 129, 133, 137, 141, 145, 149, 153, 157, 161, 165, 169, 173, 177};

        private static int[] QRAlignmentLoopSizes = {
	        0, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7};

        private static int[] QRTableE1 = {
	        6, 18, 0, 0, 0, 0, 0, 6, 22, 0, 0, 0, 0, 0, 6, 26, 0, 0, 0, 0, 0, 6, 30, 0, 0, 0, 0, 0,
	        6, 34, 0, 0, 0, 0, 0, 6, 22, 38, 0, 0, 0, 0, 6, 24, 42, 0, 0, 0, 0, 6, 26, 46, 0, 0, 0, 0,
	        6, 28, 50, 0, 0, 0, 0, 6, 30, 54, 0, 0, 0, 0, 6, 32, 58, 0, 0, 0, 0, 6, 34, 62, 0, 0, 0, 0,
	        6, 26, 46, 66, 0, 0, 0, 6, 26, 48, 70, 0, 0, 0, 6, 26, 50, 74, 0, 0, 0, 6, 30, 54, 78, 0, 0, 0,
	        6, 30, 56, 82, 0, 0, 0, 6, 30, 58, 86, 0, 0, 0, 6, 34, 62, 90, 0, 0, 0, 6, 28, 50, 72, 94, 0, 0,
	        6, 26, 50, 74, 98, 0, 0, 6, 30, 54, 78, 102, 0, 0, 6, 28, 54, 80, 106, 0, 0, 6, 32, 58, 84, 110, 0, 0,
	        6, 30, 58, 86, 114, 0, 0, 6, 34, 62, 90, 118, 0, 0, 6, 26, 50, 74, 98, 122, 0, 6, 30, 54, 78, 102, 126, 0,
	        6, 26, 52, 78, 104, 130, 0, 6, 30, 56, 82, 108, 134, 0, 6, 34, 60, 86, 112, 138, 0, 6, 30, 58, 86, 114, 142, 0,
	        6, 34, 62, 90, 118, 146, 0, 6, 30, 54, 78, 102, 126, 150, 6, 24, 50, 76, 102, 128, 154, 6, 28, 54, 80, 106, 132, 158,
	        6, 32, 58, 84, 110, 136, 162, 6, 26, 54, 82, 110, 138, 166, 6, 30, 58, 86, 114, 142, 170};

        private static UInt32[] QRFormatSeqence = {
	        // Format information bit sequences.
	        0x5412, 0x5125, 0x5e7c, 0x5b4b, 0x45f9, 0x40ce, 0x4f97, 0x4aa0, 0x77c4, 0x72f3, 0x7daa, 0x789d,
	        0x662f, 0x6318, 0x6c41, 0x6976, 0x1689, 0x13be, 0x1ce7, 0x19d0, 0x0762, 0x0255, 0x0d0c, 0x083b,
	        0x355f, 0x3068, 0x3f31, 0x3a06, 0x24b4, 0x2183, 0x2eda, 0x2bed};

        private static int[] QRVersionSeqence = {
	        // Version information bit sequences.
	        0x07c94, 0x085bc, 0x09a99, 0x0a4d3, 0x0bbf6, 0x0c762, 0x0d847, 0x0e60d, 0x0f928, 0x10b78,
	        0x1145d, 0x12a17, 0x13532, 0x149a6, 0x15683, 0x168c9, 0x177ec, 0x18ec4, 0x191e1, 0x1afab,
	        0x1b08e, 0x1cc1a, 0x1d33f, 0x1ed75, 0x1f250, 0x209d5, 0x216f0, 0x228ba, 0x2379f, 0x24b0b,
	        0x2542e, 0x26a64, 0x27541, 0x28c69};

        private static int[] MQRFormatSequence = {
	        // Micro QR Code format information.
	        0x4445, 0x4172, 0x4e2b, 0x4b1c, 0x55ae, 0x5099, 0x5fc0, 0x5af7, 0x6793, 0x62a4, 0x6dfd, 0x68ca, 0x7678, 0x734f,
	        0x7c16, 0x7921, 0x06de, 0x03e9, 0x0cb0, 0x0987, 0x1735, 0x1202, 0x1d5b, 0x186c, 0x2508, 0x203f, 0x2f66, 0x2a51, 0x34e3,
	        0x31d4, 0x3e8d, 0x3bba
        };

        private static int[] MQRSizes = { 11, 13, 15, 17 };

        // Rectangular Micro QR.
        private static short[] RMQRHeight = {
		    7, 7, 7, 7, 7,
		    9, 9, 9, 9, 9,
		    11, 11, 11, 11, 11, 11,
		    13, 13, 13, 13, 13, 13,
		    15, 15, 15, 15, 15,
		    17, 17, 17, 17, 17 };

        private static short[] RMQRWidth = {
		    43, 59, 77, 99, 139,
		    43, 59, 77, 99, 139,
		    27, 43, 59, 77, 99, 139,
		    27, 43, 59, 77, 99, 139,
		    43, 59, 77, 99, 139,
		    43, 59, 77, 99, 139	};

        private static short[] RMQRDataCodewordsM = {
		    6, 12, 20, 28, 44,          // R7x
		    12, 21, 31, 42, 63,         // R9x
		    7, 19, 31, 43, 57, 84,      // R11x
		    12, 27, 38, 53, 73, 106,    // R13x
		    33, 48, 67, 88, 127,        // R15x
		    39, 56, 78, 100, 152 };     // R17x

        private static short[] RMQRDataCodewordsH = {
		    3, 7, 10, 14, 24,           // R7x
		    7, 11, 17, 22, 33,          // R9x
		    5, 11, 15, 23, 29, 42,      // R11x
		    7, 13, 20, 29, 35, 54,      // R13x
		    15, 26, 31, 48, 69,         // R15x
		    21, 28, 38, 56, 76 };       // R17x

        private static short[] RMQRFixedHeightUpper = {
		    -1, 4, 9, 15, 21, 26, 31 };

        private static short[] RMQRTotalCodewords = {
		    13, 21, 32, 44, 68,         // R7x
		    21, 33, 49, 66, 99,         // R9x
		    15, 31, 47, 67, 89, 132,    // R11x
		    21, 41, 60, 85, 113, 166,   // R13x
		    51, 74, 103, 136, 199,      // R15x
		    61, 88, 122, 160, 232 };    // R17x

        private static short[] RMQRNumericCCI = {
		    4, 5, 6, 7, 7,
		    5, 6, 7, 7, 8,
		    5, 6, 7, 7, 8, 8,
		    5, 7, 7, 8, 8, 8,
		    7, 7, 8, 8, 9,
		    7, 8, 8, 8, 9 };

        private static short[] RMQRAlphanumericCCI = {
		    4, 5, 5, 6, 6,
		    5, 5, 6, 6, 7,
		    4, 5, 6, 6, 7, 7,
		    5, 6, 6, 7, 7, 8,
		    6, 7, 7, 7, 8,
		    6, 7, 7, 8, 8 };

        private static short[] RMQRByteCCI = {
		    3, 4, 5, 5, 6,
		    4, 5, 5, 6, 6,
		    3, 5, 5, 6, 6, 7,
		    4, 5, 6, 6, 7, 7,
		    6, 6, 7, 7, 7,
		    6, 6, 7, 7, 8 };

        private static short[] RMQRKanjiCCI = {
		    2, 3, 4, 5, 5,
		    3, 4, 5, 5, 6,
		    2, 4, 5, 5, 6, 6,
		    3, 5, 5, 6, 6, 7,
		    5, 5, 6, 6, 7,
		    5, 6, 6, 6, 7 };

        private static byte[] RMQRBlocksM = {
		    1, 1, 1, 1, 1,      // R7x
		    1, 1, 1, 1, 2,      // R9x
		    1, 1, 1, 1, 2, 2,   // R11x
		    1, 1, 1, 2, 2, 3,   // R13x
		    1, 1, 2, 2, 3,      // R15x
		    1, 2, 2, 3, 4 };    // R17x

        private static byte[] RMQRBlocksH = {
		    1, 1, 1, 1, 2,      // R7x
		    1, 1, 2, 2, 3,      // R9x
		    1, 1, 2, 2, 2, 3,   // R11x
		    1, 1, 2, 2, 3, 4,   // R13x
		    2, 2, 3, 4, 5,      // R15x
		    2, 2, 3, 4, 6 };    // R17x

        // Table D1 - Column coordinates of centre module of alignment patterns
        private static short[] RMQRTableD1 = {
		    21, 0, 0, 0,
		    19, 39, 0, 0,
		    25, 51, 0, 0,
		    23, 49, 75, 0,
		    27, 55, 83, 111 };

        private static uint[] RMQRFormatInfoLeft = {
		    // rMQR format information for finder pattern side.
		    0x1FAB2, 0x1E597, 0x1DBDD, 0x1C4F8, 0x1B86C, 0x1A749, 0x19903, 0x18626, 0x17F0E, 0x1602B,
		    0x15E61, 0x14144, 0x13DD0, 0x122F5, 0x11CBF, 0x1039A, 0x0F1CA, 0x0EEEF, 0x0D0A5, 0x0CF80,
		    0x0B314, 0x0AC31, 0x0927B, 0x08D5E, 0x07476, 0x06B53, 0x05519, 0x04A3C, 0x036A8, 0x0298D,
		    0x017C7, 0x008E2, 0x3F367, 0x3EC42, 0x3D208, 0x3CD2D, 0x3B1B9, 0x3AE9C, 0x390D6, 0x38FF3,
		    0x376DB, 0x369FE, 0x357B4, 0x34891, 0x33405, 0x32B20, 0x3156A, 0x30A4F, 0x2F81F, 0x2E73A,
		    0x2D970, 0x2C655, 0x2BAC1, 0x2A5E4, 0x29BAE, 0x2848B, 0x27DA3, 0x26286, 0x25CCC, 0x243E9,
		    0x23F7D, 0x22058, 0x21E12, 0x20137	};

        private static uint[] RMQRFormatInfoRight = {
		    // rMQR format information for subfinder pattern side.
		    0x20A7B, 0x2155E, 0x22B14, 0x23431, 0x248A5, 0x25780, 0x269CA, 0x276EF, 0x28FC7, 0x290E2,
		    0x2AEA8, 0x2B18D, 0x2CD19, 0x2D23C, 0x2EC76, 0x2F353, 0x30103, 0x31E26, 0x3206C, 0x33F49,
		    0x343DD, 0x35CF8, 0x362B2, 0x37D97, 0x384BF, 0x39B9A, 0x3A5D0, 0x3BAF5, 0x3C661, 0x3D944,
		    0x3E70E, 0x3F82B, 0x003AE, 0x01C8B, 0x022C1, 0x03DE4, 0x04170, 0x05E55, 0x0601F, 0x07F3A,
		    0x08612, 0x09937, 0x0A77D, 0x0B858, 0x0C4CC, 0x0DBE9, 0x0E5A3, 0x0FA86, 0x108D6, 0x117F3,
		    0x129B9, 0x1369C, 0x14A08, 0x1552D, 0x16B67, 0x17442, 0x18D6A, 0x1924F, 0x1AC05, 0x1B320,
		    0x1CFB4, 0x1D091, 0x1EEDB, 0x1F1FE };

        #endregion

        #region Constants

        const int QR_NUM_MODES = 4;
        const int QR_MULT = 6;
        const int RMQR_VERSION = 100;
        const int MICROQR_VERSION = 200;

        // Indexes into mode_types array (and state array).
        const int QR_N = 0;     // Numeric
        const int QR_A = 1;     // Alphanumeric
        const int QR_B = 2;     // Byte
        const int QR_K = 3;     // Kanji
        readonly char[] modeTypes = { 'N', 'A', 'B', 'K' }; // Must be in same order as QR_N etc.

        // Indexes to state array (0..3 = head costs).
        const int QR_VER = 4;       // Version.
        const int QR_GS1 = 5;       // GS1 mode (boolean).
        const int QR_N_END = 6;     // Numeric end index.
        const int QR_N_COST = 7;    // Numeric cost.
        const int QR_A_END = 8;     // Alpha end index.
        const int QR_A_COST = 9;    // Alpha cost.
        const int QR_A_PCENT = 10;  // Alpha 2nd char percent (GS1-specific).

        // Costs set to this for invalid MICROQR modes for versions M1 and M2.
        // 128 is the max number of data bits for M4-L (ISO/IEC 18004:2015 Table 7)
        const int QR_MICROQR_MAX = 774; // (128 + 1) * QR_MULT

        #endregion

        private QRCodeEccLevel optionErrorCorrection;
        private int optionSymbolVersion;

        public QRCodeEncoder(Symbology symbology, string barcodeMessage, int optionSymbolVersion, QRCodeEccLevel optionErrorCorrection, int eci, EncodingMode mode)
        {
            this.symbolId = symbology;
            this.barcodeMessage = barcodeMessage;
            this.optionErrorCorrection = optionErrorCorrection;
            this.optionSymbolVersion = optionSymbolVersion;
            this.eci = eci;
            this.encodingMode = mode;
            this.Symbol = new Collection<SymbolData>();
        }

        public override Collection<SymbolData> EncodeData()
        {
            switch (symbolId)
            {
                case Symbology.QRCode:
                    switch (encodingMode)
                    {
                        case EncodingMode.Standard:
                            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                            break;

                        case EncodingMode.GS1:
                            isGS1 = true;
                            barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                            break;

                        case EncodingMode.HIBC:
                            barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                            break;
                    }

                    QRCode();
                    break;

                case Symbology.MicroQRCode:
                    eci = 0;
                    barcodeData = MessagePreProcessor.MessageParser(barcodeMessage);
                    MicroQRCode();
                    break;

                case Symbology.UPNQR:
                    isGS1 = false;  // UPNQR doesn't support GS1.
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    UPNQR();
                    break;

                case Symbology.RectangularMicroQRCode:
                    eci = 0;
                    switch (encodingMode)
                    {
                        case EncodingMode.Standard:
                            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                            break;

                        case EncodingMode.GS1:
                            isGS1 = true;
                            barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                            break;
                    }

                    RMicroQRCode();
                    break;
            }

            return Symbol;
        }

        private void QRCode()
        {
            int autoSize;
            int version;
            int symbolSize;
            int estimatedBinaryLength;
            int lastBinaryLength;
            BitVector bitStream;
            bool canShrink;
            int targetCodewords;
            int blocks;
            QRCodeEccLevel eccLevel;
            int maxCodewords = 2956;
            int inputLength = barcodeData.Length;
            char[] jisData = new char[inputLength];
            char[] mode = new char[inputLength];

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] <= 0xff)
                    jisData[i] = barcodeData[i];

                else
                    jisData[i] = GetShiftJISCharacter(barcodeData[i]);
            }

            estimatedBinaryLength = GetBinaryLength(40, mode, jisData, inputLength);
            eccLevel = QRCodeEccLevel.Low;
            maxCodewords = 2956;
            switch (optionErrorCorrection)
            {
                case QRCodeEccLevel.Low:
                    break;

                case QRCodeEccLevel.Medium:
                    eccLevel = QRCodeEccLevel.Medium;
                    maxCodewords = 2334;
                    break;

                case QRCodeEccLevel.Quartile:
                    eccLevel = QRCodeEccLevel.Quartile;
                    maxCodewords = 1666;
                    break;

                case QRCodeEccLevel.High:
                    eccLevel = QRCodeEccLevel.High;
                    maxCodewords = 1276;
                    break;
            }

            if (estimatedBinaryLength > (8 * maxCodewords))
                throw new InvalidDataLengthException("QR Code: Input data too long for selected error correction level.");

            autoSize = 40;
            for (int i = 39; i >= 0; i--)
            {
                switch (eccLevel)
                {
                    case QRCodeEccLevel.Low:
                        if (8 * QRDataCodeWordsLow[i] >= estimatedBinaryLength)
                            autoSize = i + 1;
                        break;

                    case QRCodeEccLevel.Medium:
                        if (8 * QRDataCodeWordsMedium[i] >= estimatedBinaryLength)
                            autoSize = i + 1;
                        break;

                    case QRCodeEccLevel.Quartile:
                        if (8 * QRDataCodeWordsQuartile[i] >= estimatedBinaryLength)
                            autoSize = i + 1;
                        break;

                    case QRCodeEccLevel.High:
                        if (8 * QRDataCodeWordsHigh[i] >= estimatedBinaryLength)
                            autoSize = i + 1;
                        break;
                }
            }

            if (autoSize != 40)
                estimatedBinaryLength = GetBinaryLength(autoSize, mode, jisData, inputLength);

            // Now see if the optimised binary will fit in a smaller symbol.
            lastBinaryLength = estimatedBinaryLength;
            canShrink = true;
            do
            {
                if (autoSize == 1)
                    canShrink = false;

                else
                {
                    estimatedBinaryLength = GetBinaryLength(autoSize - 1, mode, jisData, inputLength);
                    switch (eccLevel)
                    {
                        case QRCodeEccLevel.Low:
                            if (8 * QRDataCodeWordsLow[autoSize - 2] < estimatedBinaryLength)
                                canShrink = false;
                            break;

                        case QRCodeEccLevel.Medium:
                            if (8 * QRDataCodeWordsMedium[autoSize - 2] < estimatedBinaryLength)
                                canShrink = false;
                            break;

                        case QRCodeEccLevel.Quartile:
                            if (8 * QRDataCodeWordsQuartile[autoSize - 2] < estimatedBinaryLength)
                                canShrink = false;
                            break;

                        case QRCodeEccLevel.High:
                            if (8 * QRDataCodeWordsHigh[autoSize - 2] < estimatedBinaryLength)
                                canShrink = false;
                            break;
                    }

                    if (canShrink)  // Optimisation worked - data will fit in a smaller symbol.
                        autoSize--;

                    else   // Data did not fit in the smaller symbol, revert to original size
                        estimatedBinaryLength = lastBinaryLength;
                }
            } while (canShrink);

            version = autoSize;
            if ((optionSymbolVersion >= 1) && (optionSymbolVersion <= 40))
            {
                /* If the user has selected a larger symbol than the smallest available,
                 then use the size the user has selected, and re-optimise for this
                 symbol size. */
                if (optionSymbolVersion > version)
                {
                    version = optionSymbolVersion;
                    estimatedBinaryLength = GetBinaryLength(optionSymbolVersion, mode, jisData, inputLength);
                }

                if (optionSymbolVersion < version)
                    throw new InvalidDataLengthException("QR Code: Input too long for selected symbol size.");
            }

            // Ensure maximum error correction capacity.
            if ((int)optionErrorCorrection == -1 || optionErrorCorrection != eccLevel)
            {
                if (estimatedBinaryLength <= QRDataCodeWordsMedium[version - 1] * 8)
                    eccLevel = QRCodeEccLevel.Medium;

                if (estimatedBinaryLength <= QRDataCodeWordsQuartile[version - 1] * 8)
                    eccLevel = QRCodeEccLevel.Quartile;

                if (estimatedBinaryLength <= QRDataCodeWordsHigh[version - 1] * 8)
                    eccLevel = QRCodeEccLevel.High;
            }

            targetCodewords = QRDataCodeWordsLow[version - 1];
            blocks = QRBlocksLow[version - 1];
            switch (eccLevel)
            {
                case QRCodeEccLevel.Medium:
                    targetCodewords = QRDataCodeWordsMedium[version - 1];
                    blocks = QRBlocksMedium[version - 1];
                    break;

                case QRCodeEccLevel.Quartile:
                    targetCodewords = QRDataCodeWordsQuartile[version - 1];
                    blocks = QRBlocksQuartile[version - 1];
                    break;

                case QRCodeEccLevel.High:
                    targetCodewords = QRDataCodeWordsHigh[version - 1];
                    blocks = QRBlocksHigh[version - 1];
                    break;
            }

            bitStream = new BitVector(QRTotalCodeWords[version - 1]);
            QRBinary(bitStream, version, targetCodewords, mode, jisData);
            AddErrorCorrection(bitStream, version, targetCodewords, blocks);

            byte[] data = bitStream.ToByteArray();
            byte b = data[0];

            symbolSize = QRSizes[version - 1];
            byte[] symbolGrid = new byte[symbolSize * symbolSize];

            SetupGrid(symbolGrid, symbolSize, version);
            PopulateGrid(symbolGrid, symbolSize, symbolSize, bitStream);

            if (version >= 7)
                AddVersionInformation(symbolGrid, symbolSize, version);

            int bitMask = ApplyBitmask(symbolGrid, symbolSize, eccLevel);
            AddFormatInformationGrid(symbolGrid, symbolSize, eccLevel, bitMask);
            BuildSymbol(symbolGrid, symbolSize);
        }

        // Calculate optimized encoding modes. Adapted from Project Nayuki.
        private void DefineModes(char[] mode, char[] data, int inputLength, int version)
        {
            int[] state = {
                0 /*N*/, 0 /*A*/, 0 /*B*/, 0 /*K*/, 0 /*version*/, 0 /*gs1*/,
                0 /*numeric_end*/, 0 /*numeric_cost*/, 0 /*alpha_end*/, 0 /*alpha_cost*/, 0 /*alpha_pcent*/  };

            state[QR_VER] = version;
            state[QR_GS1] = ((isGS1) ? 1 : 0);

            CharacterModes.DefineModes(mode, data, inputLength, state, modeTypes, QR_NUM_MODES, HeadCost, SwitchCost, null, CurrentCost);
        }


        // Initial mode costs.
        private static int[] HeadCost(int[] state)
        {
            int[,] headCosts = {
			/* N                    A                   B                   K */
			{ (10 + 4) * QR_MULT, (9 + 4) * QR_MULT, (8 + 4) * QR_MULT, (8 + 4) * QR_MULT, }, /* QR */
			{ (12 + 4) * QR_MULT, (11 + 4) * QR_MULT, (16 + 4) * QR_MULT, (10 + 4) * QR_MULT, },
			{ (14 + 4) * QR_MULT, (13 + 4) * QR_MULT, (16 + 4) * QR_MULT, (12 + 4) * QR_MULT, },
			{ 3 * QR_MULT, QR_MICROQR_MAX, QR_MICROQR_MAX, QR_MICROQR_MAX, }, /* MICROQR */
			{ (4 + 1) * QR_MULT, (3 + 1) * QR_MULT, QR_MICROQR_MAX, QR_MICROQR_MAX, },
			{ (5 + 2) * QR_MULT, (4 + 2) * QR_MULT, (4 + 2) * QR_MULT, (3 + 2) * QR_MULT, },
			{ (6 + 3) * QR_MULT, (5 + 3) * QR_MULT, (5 + 3) * QR_MULT, (4 + 3) * QR_MULT, }	};

            int version;

            // Head costs kept in states 0..3
            if (state[QR_N] != 0)
            {
                // Numeric non-zero in all configs.
                return state; // Already set.
            }

            version = (int)state[QR_VER];
            if (version < RMQR_VERSION)
            {
                // QRCODE.
                if (version < 10)
                {
                    for (int i = 0; i < QR_NUM_MODES; i++)
                        state[i] = headCosts[0, i];
                }

                else if (version < 27)
                    for (int i = 0; i < QR_NUM_MODES; i++)
                        state[i] = headCosts[1, i];

                else
                    for (int i = 0; i < QR_NUM_MODES; i++)
                        state[i] = headCosts[2, i];
            }

            else if (version < MICROQR_VERSION)
            {
                // RMQR.
                version -= RMQR_VERSION;
                state[QR_N] = (RMQRNumericCCI[version] + 3) * QR_MULT;
                state[QR_A] = (RMQRAlphanumericCCI[version] + 3) * QR_MULT;
                state[QR_B] = (RMQRByteCCI[version] + 3) * QR_MULT;
                state[QR_K] = (RMQRKanjiCCI[version] + 3) * QR_MULT;
            }

            else
            {
                // MICROQR.
                for (int i = 0; i < QR_NUM_MODES; i++)
                    state[i] = headCosts[3 + (version - MICROQR_VERSION), i];
            }

            return state;
        }

        // Costs of switching modes from k to j.
        private static int SwitchCost(int[] state, int k, int j)
        {
            return state[j]; // Same as head cost.
        }

        //private static void CurrentCost(uint[] state, char[] gbdata, int length, int position, char[] characterModes, uint[] previousCosts, uint[] currentCosts)
        // Calculate cost of encoding a character.
        private void CurrentCost(int[] state, char[] data, int length, int position, char[] characterModes, int[] previousCosts, int[] currentCosts)
        {
            bool m1, m2;
            int cmIndex = position * QR_NUM_MODES;

            m1 = state[QR_VER] == MICROQR_VERSION;
            m2 = state[QR_VER] == MICROQR_VERSION + 1;
            if (data[position] > 0xFF)
            {
                currentCosts[QR_B] = previousCosts[QR_B] + ((m1 || m2) ? QR_MICROQR_MAX : 96); // 16 * QR_MULT.
                characterModes[cmIndex + QR_B] = 'B';
                currentCosts[QR_K] = previousCosts[QR_K] + ((m1 || m2) ? QR_MICROQR_MAX : 78); // 13 * QR_MULT.
                characterModes[cmIndex + QR_K] = 'K';
            }

            else
            {
                if (InNumeric(data, length, position, ref state[QR_N_END], ref state[QR_N_COST]))
                {
                    currentCosts[QR_N] = previousCosts[QR_N] + state[QR_N_COST];
                    characterModes[cmIndex + QR_N] = 'N';
                }

                if (InAlpha(data, length, position, ref state[QR_A_END], ref state[QR_A_COST], ref state[QR_A_PCENT]))
                {
                    currentCosts[QR_A] = previousCosts[QR_A] + (m1 ? QR_MICROQR_MAX : state[QR_A_COST]);
                    characterModes[cmIndex + QR_A] = 'A';
                }

                currentCosts[QR_B] = previousCosts[QR_B] + ((m1 || m2) ? QR_MICROQR_MAX : 48); // 8 * QR_MULT.
                characterModes[cmIndex + QR_B] = 'B';
            }
        }

        // Returns true if input glyph is in the Alphanumeric set.
        bool IsAlpha(char glyph)
        {
            bool retval = false;

            if (Char.IsDigit(glyph))
                retval = true;

            else if (Char.IsUpper(glyph))
                retval = true;

            else if (isGS1 && glyph == '[')
                retval = true;

            else
            {
                switch (glyph)
                {
                    case ' ':
                    case '$':
                    case '%':
                    case '*':
                    case '+':
                    case '-':
                    case '.':
                    case '/':
                    case ':':
                        retval = true;
                        break;
                }
            }

            return retval;
        }

        // Whether in numeric or not.
        // If in numeric, pEnd is set to position after numeric, and pCost is set to per-numeric cost.
        private static bool InNumeric(char[] jisData, int length, int position, ref int pEnd, ref int pCost)
        {
            int i;
            int digitCount;

            if (position < pEnd)
                return true;

            // Attempt to calculate the average 'cost' of using numeric mode in number of bits (times QR_MULT).
            for (i = position; i < length && i < position + 4 && jisData[i] >= '0' && jisData[i] <= '9'; i++) ;

            digitCount = i - position;
            if (digitCount == 0)
            {
                pEnd = 0;
                return false;
            }

            pEnd = i;
            pCost = digitCount == 1 ? 24 /* 4 * QR_MULT */ : digitCount == 2 ? 21 /* (7 / 2) * QR_MULT */ : 20 /* (10 / 3) * QR_MULT) */;
            return true;
        }

        // Whether in alpha or not.
        // If in alpha, pEnd is set to position after alpha, and pCost is set to per-alpha cost. For GS1, pPercent set if 2nd char percent.
        bool InAlpha(char[] jisData, int length, int position, ref int pEnd, ref int pCost, ref int pPercent)
        {
            bool twoAlphas;

            if (position < pEnd)
            {
                if (isGS1 && (pPercent > 0))
                {
                    // Previous 2nd char was a percent, so allow for second half of doubled-up percent here.
                    twoAlphas = position < length - 1 && IsAlpha(jisData[position + 1]);
                    pCost = twoAlphas ? 33 /* (11 / 2) * QR_MULT */ : 36 /* 6 * QR_MULT */;
                    pPercent = 0;
                }

                return true;
            }

            // Attempt to calculate the average 'cost' of using alphanumeric mode in number of bits (times QR_MULT).
            if (!IsAlpha(jisData[position]))
            {
                pEnd = 0;
                pPercent = 0;
                return false;
            }

            if (isGS1 && jisData[position] == '%')
            {
                // Must double-up so counts as 2 chars.
                pEnd = position + 1;
                pCost = 66; // 11 * QR_MULT
                pPercent = 0;
                return true;
            }

            twoAlphas = position < length - 1 && IsAlpha(jisData[position + 1]);

            pEnd = twoAlphas ? position + 2 : position + 1;
            pCost = twoAlphas ? 33 /* (11 / 2) * QR_MULT */ : 36 /* 6 * QR_MULT */;
            pPercent = twoAlphas && (isGS1 && jisData[position + 1] == '%') ? 1 : 0;
            return true;
        }

        // Calculate the actual bit length of the proposed binary string.
        private int GetBinaryLength(int version, char[] inputMode, char[] inputData, int inputLength)
        {
            char currentMode;
            int count = 0;
            int alphalength;
            int blocklength;

            DefineModes(inputMode, inputData, inputLength, version);
            currentMode = ' ';

            if (isGS1)
            {
                // Not applicable to MICROQR.
                if (version < RMQR_VERSION)
                    count += 4;

                else
                    count += 3;
            }

            if (eci != 0)
            {
                // RMQR and MICROQR do not support ECI.
                count += 4;
                if (eci <= 127)
                    count += 8;

                else if (eci <= 16383)
                    count += 16;

                else
                    count += 24;
            }

            for (int i = 0; i < (int)inputLength; i++)
            {
                if (inputMode[i] != currentMode)
                {
                    count += ModeBits(version) + CharacterCountBits(version, inputMode[i]);
                    blocklength = BlockLength(i, inputMode, inputLength);
                    switch (inputMode[i])
                    {
                        case 'K':
                            count += (blocklength * 13);
                            break;

                        case 'B':
                            for (int j = i; j < (i + blocklength); j++)
                            {
                                if (inputData[j] > 0xff)
                                    count += 16;

                                else
                                    count += 8;
                            }
                            break;

                        case 'A':
                            alphalength = blocklength;
                            if (isGS1)
                            {
                                // In alphanumeric mode % becomes %%
                                for (int j = i; j < (i + blocklength); j++)
                                {
                                    if (inputData[j] == '%')
                                        alphalength++;
                                }
                            }

                            switch (alphalength % 2)
                            {
                                case 0:
                                    count += (alphalength / 2) * 11;
                                    break;

                                case 1:
                                    count += ((alphalength - 1) / 2) * 11;
                                    count += 6;
                                    break;
                            }
                            break;

                        case 'N':
                            switch (blocklength % 3)
                            {
                                case 0:
                                    count += (blocklength / 3) * 10;
                                    break;

                                case 1:
                                    count += ((blocklength - 1) / 3) * 10;
                                    count += 4;
                                    break;

                                case 2:
                                    count += ((blocklength - 2) / 3) * 10;
                                    count += 7;
                                    break;
                            }
                            break;
                    }

                    currentMode = inputMode[i];
                }
            }

            return count;
        }

        private void QRBinary(BitVector bitStream, int version, int targetCodewords, char[] mode, char[] jisData)
        {
            int position = 0;
            int terminationBits, padBits;
            int currentBinaryLength;
            int currentBytes;
            bool isPercent;
            int percentCount;
            char dataBlock;
            int shortDataBlockLength;
            int doubleByte;
            int modeValue = 0;
            int modeLength = 0;
            int index;
            int length = jisData.Length;

            if (isGS1)
            {
                // Not applicable to MICROQR.
                if (version < RMQR_VERSION)
                    bitStream.AppendBits(0x05, 4);   // FNC1 "0101"

                else
                    bitStream.AppendBits(0x05, 3);
            }

            if (eci != 0 && version < RMQR_VERSION)
            {
                // Not applicable to RMQR or MICROQR.
                bitStream.AppendBits(7, 4);                     // Mode indicator ECI (Table 4)
                if (eci <= 127)
                    bitStream.AppendBits(eci, 8);               // 000000 to 000127.

                else if (eci <= 16383)
                    bitStream.AppendBits(0x8000 + eci, 16);     // 000000 to 016383.

                else
                    bitStream.AppendBits(0xC00000 + eci, 24);   // 000000 to 999999.
            }

            isPercent = false;
            do
            {
                dataBlock = mode[position];
                shortDataBlockLength = 0;
                doubleByte = 0;
                do
                {
                    if (dataBlock == 'B' && jisData[position + shortDataBlockLength] > 0xFF)
                        doubleByte++;

                    shortDataBlockLength++;
                } while (((shortDataBlockLength + position) < length) && (mode[position + shortDataBlockLength] == dataBlock));

                switch (dataBlock)
                {
                    case 'K':
                        // Kanji mode.
                        // Mode indicator.
                        ModeIndicator(version, dataBlock, ref modeValue, ref modeLength);
                        bitStream.AppendBits(modeValue, modeLength);

                        // Character count indicator.
                        bitStream.AppendBits(shortDataBlockLength, CharacterCountBits(version, dataBlock));

                        // Character representation.
                        for (int i = 0; i < shortDataBlockLength; i++)
                        {
                            int jisValue = (int)(jisData[position + i]);
                            int product;

                            if (jisValue >= 0x8140 && jisValue <= 0x9ffc)
                                jisValue -= 0x8140;

                            else if (jisValue >= 0xe040 && jisValue <= 0xebbf)
                                jisValue -= 0xc140;

                            product = ((jisValue >> 8) * 0xc0) + (jisValue & 0xff);
                            bitStream.AppendBits(product, 13);
                        }
                        break;

                    case 'B':
                        // Byte mode.
                        // Mode indicator.
                        ModeIndicator(version, dataBlock, ref modeValue, ref modeLength);
                        bitStream.AppendBits(modeValue, modeLength);

                        // Character count indicator.
                        bitStream.AppendBits(shortDataBlockLength + doubleByte, CharacterCountBits(version, dataBlock));

                        // Character representation.
                        for (int i = 0; i < shortDataBlockLength; i++)
                        {
                            int value = (int)(jisData[position + i]);

                            if (isGS1 && (value == '[')) // FNC1.
                                value = 0x1d;

                            bitStream.AppendBits(value, 0x08);
                        }
                        break;

                    case 'A':
                        // Alphanumeric mode.
                        // Mode indicator.
                        ModeIndicator(version, dataBlock, ref modeValue, ref modeLength);
                        bitStream.AppendBits(modeValue, modeLength);

                        percentCount = 0;
                        if (isGS1)
                        {
                            for (int i = 0; i < shortDataBlockLength; i++)
                            {
                                if (jisData[position + i] == '%')
                                    percentCount++;
                            }
                        }

                        // Character count indicator.
                        bitStream.AppendBits(shortDataBlockLength + percentCount, CharacterCountBits(version, dataBlock));

                        // Character representation.
                        index = 0;
                        while (index < shortDataBlockLength)
                        {
                            int count = 0;
                            int first = 0, second = 0, product = 0;
                            if (!isPercent)
                            {
                                if (isGS1 && jisData[position + index] == '%')
                                {
                                    first = CharacterSets.AlphaNumericSet.IndexOf('%');
                                    second = CharacterSets.AlphaNumericSet.IndexOf('%');
                                    count = 2;
                                    product = (first * 45) + second;
                                    index++;
                                }

                                else
                                {
                                    if (isGS1 && jisData[position + index] == '[')  // FNC1.
                                        first = CharacterSets.AlphaNumericSet.IndexOf('%');

                                    else
                                        first = CharacterSets.AlphaNumericSet.IndexOf(jisData[position + index]);

                                    count = 1;
                                    index++;
                                    product = first;

                                    if (index < shortDataBlockLength && mode[position + index] == 'A')
                                    {
                                        if (isGS1 && (jisData[position + index]) == '%')
                                        {
                                            second = CharacterSets.AlphaNumericSet.IndexOf('%');
                                            count = 2;
                                            product = (first * 45) + second;
                                            isPercent = true;
                                        }

                                        else
                                        {
                                            if (isGS1 && (jisData[position + index]) == '[')
                                                second = CharacterSets.AlphaNumericSet.IndexOf('%');    // FNC1.

                                            else
                                                second = CharacterSets.AlphaNumericSet.IndexOf(jisData[position + index]);

                                            count = 2;
                                            index++;
                                            product = (first * 45) + second;
                                        }
                                    }
                                }
                            }

                            else
                            {
                                first = CharacterSets.AlphaNumericSet.IndexOf('%');
                                count = 1;
                                index++;
                                product = first;
                                isPercent = false;
                                if (index < shortDataBlockLength && mode[position + index] == 'A')
                                {
                                    if (isGS1 && (jisData[position + index]) == '%')
                                    {
                                        second = CharacterSets.AlphaNumericSet.IndexOf('%');
                                        count = 2;
                                        product = (first * 45) + second;
                                        isPercent = true;
                                    }

                                    else
                                    {
                                        if (isGS1 && (jisData[position + index]) == '[')
                                            second = CharacterSets.AlphaNumericSet.IndexOf('%');    // FNC1.

                                        else
                                            second = CharacterSets.AlphaNumericSet.IndexOf(jisData[position + index]);

                                        count = 2;
                                        index++;
                                        product = (first * 45) + second;
                                    }
                                }
                            }

                            bitStream.AppendBits(product, 1 + (5 * count));
                        }
                        break;

                    case 'N':
                        // Numeric mode.
                        // Mode indicator.
                        ModeIndicator(version, dataBlock, ref modeValue, ref modeLength);
                        bitStream.AppendBits(modeValue, modeLength);

                        // Character count indicator.
                        bitStream.AppendBits(shortDataBlockLength + doubleByte, CharacterCountBits(version, dataBlock));

                        // Character representation.
                        index = 0;
                        while (index < shortDataBlockLength)
                        {
                            int count = 0;
                            int first = 0, second = 0, third = 0, product = 0;

                            first = CharacterSets.NumberOnlySet.IndexOf(jisData[position + index]);
                            count = 1;
                            product = first;

                            if (index + 1 < shortDataBlockLength && mode[position + index + 1] == 'N')
                            {
                                second = CharacterSets.NumberOnlySet.IndexOf(jisData[position + index + 1]);
                                count = 2;
                                product = (product * 10) + second;

                                if (index + 2 < shortDataBlockLength && mode[position + index + 2] == 'N')
                                {
                                    third = CharacterSets.NumberOnlySet.IndexOf(jisData[position + index + 2]);
                                    count = 3;
                                    product = (product * 10) + third;
                                }
                            }

                            bitStream.AppendBits(product, 1 + (3 * count));
                            index += count;
                        }

                        break;
                }

                position += shortDataBlockLength;
            } while (position < (int)length);

            if (version >= MICROQR_VERSION && version < MICROQR_VERSION + 4)
                return; // MICROQR does its own terminating/padding.

            // Terminator.
            currentBinaryLength = bitStream.SizeInBits;
            terminationBits = 8 - currentBinaryLength % 8;
            if (terminationBits == 8)
                terminationBits = 0;

            currentBytes = (currentBinaryLength + terminationBits) / 8;
            if (terminationBits > 0 || currentBytes < targetCodewords)
            {
                int maxTermBits = TerminatorBits(version);
                terminationBits = (terminationBits < maxTermBits && currentBytes == targetCodewords) ? terminationBits : maxTermBits;
                bitStream.AppendBits(0, terminationBits);
                currentBinaryLength += terminationBits;
            }

            // Padding bits.
            padBits = 8 - currentBinaryLength % 8;
            if (padBits == 8)
                padBits = 0;

            if (padBits > 0)
            {
                currentBytes = (currentBinaryLength + padBits) / 8;
                bitStream.AppendBits(0, padBits);
            }

            // Add pad codewords.
            bool toggle = false;
            for (int i = currentBytes; i < targetCodewords; i++)
            {
                if (!toggle)
                    bitStream.AppendBits(0xec, 8);

                else
                    bitStream.AppendBits(0x11, 8);

                toggle = !toggle;
            }
        }

        // Adds error correction and interleaves the data.
        private static void AddErrorCorrection(BitVector bitStream, int version, int dataCodewords, int blocks)
        {
            // Split data into blocks, add error correction and then interleave the blocks and error correction data.
            int eccCodewords;
            int shortDataBlockSize;
            int shortBlocks;
            int eccBlockSize;
            int dataBlockSize;
            byte[] dataBlocks;
            byte[] eccBlocks;
            byte[] interleavedData;
            byte[] interleavedEcc;
            byte[] bitStreamData;

            if (version < RMQR_VERSION)
                eccCodewords = QRTotalCodeWords[version - 1] - dataCodewords;

            else
                eccCodewords = RMQRTotalCodewords[version - RMQR_VERSION] - dataCodewords;

            shortDataBlockSize = dataCodewords / blocks;
            shortBlocks = blocks - (dataCodewords % blocks);
            eccBlockSize = eccCodewords / blocks;
            interleavedData = new byte[dataCodewords];
            interleavedEcc = new byte[eccCodewords];
            int position = 0;

            bitStreamData = bitStream.ToByteArray();
            for (int i = 0; i < blocks; i++)
            {
                if (i < shortBlocks)
                    dataBlockSize = shortDataBlockSize;

                else
                    dataBlockSize = shortDataBlockSize + 1;

                dataBlocks = new byte[dataBlockSize];
                eccBlocks = new byte[eccBlockSize];
                for (int j = 0; j < dataBlockSize; j++)
                    dataBlocks[j] = bitStreamData[position + j];

                ReedSolomon.RSInitialise(0x11d, eccBlockSize, 0);
                ReedSolomon.RSEncode(dataBlockSize, dataBlocks, eccBlocks);

                for (int j = 0; j < shortDataBlockSize; j++)
                    interleavedData[(j * blocks) + i] = dataBlocks[j];

                if (i >= shortBlocks)
                    interleavedData[(shortDataBlockSize * blocks) + (i - shortBlocks)] = dataBlocks[shortDataBlockSize];

                for (int j = 0; j < eccBlockSize; j++)
                    interleavedEcc[(j * blocks) + i] = eccBlocks[eccBlockSize - j - 1];

                position += dataBlockSize;
            }

            bitStream.Clear();
            for (int j = 0; j < dataCodewords; j++)
                bitStream.AppendBits(interleavedData[j], 8);

            for (int j = 0; j < eccCodewords; j++)
                bitStream.AppendBits(interleavedEcc[j], 8);
        }

        // Return mode indicator bits based on version.
        private static int ModeBits(int version)
        {
            if (version < RMQR_VERSION)
                return 4; // QRCODE.

            if (version < MICROQR_VERSION)
                return 3; // RMQR.

            return version - MICROQR_VERSION; // MICROQR.
        }

        // Return character count indicator bits based on version and mode.
        private int CharacterCountBits(int version, char mode)
        {
            int modeIndex = 0;
            int[,] cciBits = {
			/* N   A   B   K */
			{ 10, 9, 8, 8 },    // QRCODE.
			{ 12, 11, 16, 10 },
			{ 14, 13, 16, 12 },
			{ 3, 0, 0, 0 },     // MICROQR.
			{ 4, 3, 0, 0 },
			{ 5, 4, 4, 3 },
			{ 6, 5, 5, 4 } };

            short[][] rmqrCCIBits = new short[][]  {
			RMQRNumericCCI, RMQRAlphanumericCCI, RMQRByteCCI, RMQRKanjiCCI };

            modeIndex = Array.IndexOf(modeTypes, mode);
            if (version < RMQR_VERSION)
            {
                // QRCODE.
                if (version < 10)
                    return cciBits[0, modeIndex];

                if (version < 27)
                    return cciBits[1, modeIndex];

                return cciBits[2, modeIndex];
            }

            if (version < MICROQR_VERSION)
            {
                /* RMQR */
                return rmqrCCIBits[modeIndex][version - RMQR_VERSION];
            }

            return cciBits[3 + (version - MICROQR_VERSION), modeIndex]; /* MICROQR */
        }

        // Returns mode indicator based on version and mode.
        private void ModeIndicator(int version, char mode, ref int value, ref int size)
        {
            int modeIndex;
            string modeString = String.Empty;
            string[,] modeIndicators = {
			/*    N       A       B       K */
			{ "0001", "0010", "0100", "1000" }, /* QRCODE */
			{ "001", "010", "011", "100" }, /* RMQR */
			{ "", "", "", "" }, /* MICROQR */
			{ "0", "1", "", "" },
			{ "00", "01", "10", "11" },
			{ "000", "001", "010", "011" } };

            value = 0;
            size = 0;
            modeIndex = Array.IndexOf(modeTypes, mode);
            if (version < RMQR_VERSION)
            {
                modeString = modeIndicators[0, modeIndex]; // QRCODE.
                if (!String.IsNullOrEmpty(modeString))
                {
                    value = Convert.ToInt32(modeString, 2);
                    size = modeString.Length;
                }
            }

            else if (version < MICROQR_VERSION)
            {
                modeString = modeIndicators[1, modeIndex]; // RMQR.
                if (!String.IsNullOrEmpty(modeString))
                {
                    value = Convert.ToInt32(modeString, 2);
                    size = modeString.Length;
                }
            }

            else
            {
                modeString = modeIndicators[2 + version - MICROQR_VERSION, modeIndex]; // MICROQR
                if (!String.IsNullOrEmpty(modeString))
                {
                    value = Convert.ToInt32(modeString, 2);
                    size = modeString.Length;
                }
            }

            return;
        }

        // Returns terminator bits based on version.
        private static int TerminatorBits(int version)
        {
            if (version < RMQR_VERSION)
                return 4; // QRCODE.

            if (version < MICROQR_VERSION)
                return 3; // RMQR.

            return 3 + (version - MICROQR_VERSION) * 2; // MICROQR (Note not actually using this at the moment).
        }

        private static void SetupGrid(byte[] symbolGrid, int symbolSize, int version)
        {
            bool latch = true;

            // Add timing patterns.
            for (int i = 0; i < symbolSize; i++)
            {
                if (latch)
                {
                    symbolGrid[(6 * symbolSize) + i] = 0x21;
                    symbolGrid[(i * symbolSize) + 6] = 0x21;
                }

                else
                {
                    symbolGrid[(6 * symbolSize) + i] = 0x20;
                    symbolGrid[(i * symbolSize) + 6] = 0x20;
                }

                latch = !latch;
            }

            // Add finder patterns.
            PlaceFinderPatterns(symbolGrid, symbolSize, 0, 0);
            PlaceFinderPatterns(symbolGrid, symbolSize, 0, symbolSize - 7);
            PlaceFinderPatterns(symbolGrid, symbolSize, symbolSize - 7, 0);

            // Add separators.
            for (int i = 0; i < 7; i++)
            {
                symbolGrid[(7 * symbolSize) + i] = 0x10;
                symbolGrid[(i * symbolSize) + 7] = 0x10;
                symbolGrid[(7 * symbolSize) + (symbolSize - 1 - i)] = 0x10;
                symbolGrid[(i * symbolSize) + (symbolSize - 8)] = 0x10;
                symbolGrid[((symbolSize - 8) * symbolSize) + i] = 0x10;
                symbolGrid[((symbolSize - 1 - i) * symbolSize) + 7] = 0x10;
            }

            symbolGrid[(7 * symbolSize) + 7] = 0x10;
            symbolGrid[(7 * symbolSize) + (symbolSize - 8)] = 0x10;
            symbolGrid[((symbolSize - 8) * symbolSize) + 7] = 0x10;

            // Add alignment patterns
            // Version 1 does not have alignment patterns.
            if (version != 1)
            {
                int loopSize = QRAlignmentLoopSizes[version - 1];
                for (int x = 0; x < loopSize; x++)
                {
                    for (int y = 0; y < loopSize; y++)
                    {
                        int xcoord = QRTableE1[((version - 2) * 7) + x];
                        int ycoord = QRTableE1[((version - 2) * 7) + y];

                        if ((symbolGrid[(ycoord * symbolSize) + xcoord] & 0x10) == 0)
                            PlaceAlignmentPatterns(symbolGrid, symbolSize, xcoord, ycoord);
                    }
                }
            }

            // Reserve space for format information.
            for (int i = 0; i < 8; i++)
            {
                symbolGrid[(8 * symbolSize) + i] += 0x20;
                symbolGrid[(i * symbolSize) + 8] += 0x20;
                symbolGrid[(8 * symbolSize) + (symbolSize - 1 - i)] = 0x20;
                symbolGrid[((symbolSize - 1 - i) * symbolSize) + 8] = 0x20;
            }

            symbolGrid[(8 * symbolSize) + 8] += 0x20;
            symbolGrid[((symbolSize - 1 - 7) * symbolSize) + 8] = 0x21; // Dark Module from Figure 25.

            // Reserve space for version information.
            if (version >= 7)
            {
                for (int i = 0; i < 6; i++)
                {
                    symbolGrid[((symbolSize - 9) * symbolSize) + i] = 0x20;
                    symbolGrid[((symbolSize - 10) * symbolSize) + i] = 0x20;
                    symbolGrid[((symbolSize - 11) * symbolSize) + i] = 0x20;
                    symbolGrid[(i * symbolSize) + (symbolSize - 9)] = 0x20;
                    symbolGrid[(i * symbolSize) + (symbolSize - 10)] = 0x20;
                    symbolGrid[(i * symbolSize) + (symbolSize - 11)] = 0x20;
                }
            }
        }

        private static void PlaceAlignmentPatterns(byte[] symbolGrid, int symbolSize, int x, int y)
        {
            int[] alignment = { 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1 };

            x -= 2;
            y -= 2; // Input values represent centre of pattern.

            for (int xp = 0; xp < 5; xp++)
            {
                for (int yp = 0; yp < 5; yp++)
                {
                    if (alignment[xp + (5 * yp)] == 1)
                        symbolGrid[((yp + y) * symbolSize) + (xp + x)] = 0x11;

                    else
                        symbolGrid[((yp + y) * symbolSize) + (xp + x)] = 0x10;
                }
            }
        }

        private static void PlaceFinderPatterns(byte[] symbolGrid, int symbolSize, int x, int y)
        {
            int[] finder =
            {
                1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1,
		        1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1,
		        1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1,
		        1, 1, 1, 1, 1, 1, 1
            };

            for (int xp = 0; xp < 7; xp++)
            {
                for (int yp = 0; yp < 7; yp++)
                {
                    if (finder[xp + (7 * yp)] == 1)
                        symbolGrid[((yp + y) * symbolSize) + (xp + x)] = 0x11;

                    else
                        symbolGrid[((yp + y) * symbolSize) + (xp + x)] = 0x10;
                }
            }
        }

        private static void PopulateGrid(byte[] symbolGrid, int hSize, int vSize, BitVector bitStream)
        {
            int direction = 1;      // Up.
            int row = 0;            // Right hand side.
            int n = bitStream.SizeInBits;
            int y = vSize - 1;
            int i = 0;
            int x;

            do
            {
                x = (hSize - 2) - (row * 2);
                if ((x < 6) && (vSize == hSize))
                    x--;    // Skip over vertical timing pattern.

                if ((symbolGrid[(y * hSize) + (x + 1)] & 0xf0) == 0)
                {
                    symbolGrid[(y * hSize) + (x + 1)] = bitStream[i];
                    i++;
                }

                if (i < n)
                {
                    if ((symbolGrid[(y * hSize) + x] & 0xf0) == 0)
                    {
                        if ((symbolGrid[(y * hSize) + x] & 0xf0) == 0)
                        {
                            symbolGrid[(y * hSize) + x] = bitStream[i];
                            i++;
                        }
                    }
                }

                if (direction == 1)
                    y--;

                else
                    y++;

                if (y == -1)
                {
                    // Reached the top.
                    row++;
                    y = 0;
                    direction = 0;
                }

                if (y == vSize)
                {
                    // Reached the bottom.
                    row++;
                    y = vSize - 1;
                    direction = 1;
                }

            }
            while (i < n);
        }

        private static int Evaluate(byte[] evaluationData, int symbolSize, int pattern)
        {
            int block;
            int result = 0;
            byte state;
            int p = 0;
            int beforeCount, afterCount;
            int darkModules;
            int k, percentage;

            byte[] local = new byte[symbolSize * symbolSize];

            // All eight bitmask variants have been encoded in the 8 bits of the bytes that make
            // up the grid array. Select them for evaluation according to the desired pattern.
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    byte value = (byte)(0x01 << pattern);
                    local[(y * symbolSize) + x] = (byte)((evaluationData[(y * symbolSize) + x] & value) != 0 ? 1 : 0);
                }
            }

            // Test 1: Adjacent modules in row/column are same colour.
            // Vertical.
            for (int x = 0; x < symbolSize; x++)
            {
                state = local[x];
                block = 0;
                for (int y = 0; y < symbolSize; y++)
                {
                    if (local[(y * symbolSize) + x] == state)
                        block++;

                    else
                    {
                        if (block > 5)
                            result += (3 + block);

                        block = 0;
                        state = local[(y * symbolSize) + x];
                    }
                }

                if (block > 5)
                    result += (3 + block);
            }

            // Horizontal.
            for (int y = 0; y < symbolSize; y++)
            {
                state = local[y * symbolSize];
                block = 0;
                for (int x = 0; x < symbolSize; x++)
                {
                    if (local[(y * symbolSize) + x] == state)
                        block++;

                    else
                    {
                        if (block > 5)
                            result += (3 + block);

                        block = 0;
                        state = local[(y * symbolSize) + x];
                    }
                }

                if (block > 5)
                    result += (3 + block);
            }

            // Test 2: Block of modules in same color.
            for (int x = 0; x < symbolSize - 1; x++)
            {
                for (int y = 0; y < symbolSize - 1; y++)
                {
                    if ((local[(y * symbolSize) + x] == local[((y + 1) * symbolSize) + x]) &&
                        (local[(y * symbolSize) + x] == local[(y * symbolSize) + (x + 1)]) &&
                        (local[(y * symbolSize) + x] == local[((y + 1) * symbolSize) + (x + 1)]))
                        result += 3;
                }
            }

            // Test 3: 1:1:3:1:1 ratio pattern in row/column.
            // Vertical
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < (symbolSize - 7); y++)
                {
                    p = 0;
                    for (int weight = 0; weight < 7; weight++)
                    {
                        if (local[((y + weight) * symbolSize) + x] == 1)
                            p += (0x40 >> weight);

                    }

                    if (p == 0x5d)
                    {
                        // Pattern found, check before and after.
                        beforeCount = 0;
                        for (int before = (y - 4); before < y; before++)
                        {
                            if (before < 0)
                                beforeCount++;

                            else
                            {
                                if (local[(before * symbolSize) + x] == 0)
                                    beforeCount++;

                                else
                                    beforeCount = 0;
                            }
                        }

                        afterCount = 0;
                        for (int after = (y + 7); after <= (y + 10); after++)
                        {
                            if (after >= symbolSize)
                                afterCount++;

                            else
                            {
                                if (local[(after * symbolSize) + x] == 0)
                                    afterCount++;

                                else
                                    afterCount = 0;
                            }
                        }

                        if ((beforeCount == 4) || (afterCount == 4))    // Pattern is preceeded or followed by light area 4 modules wide.
                            result += 40;
                    }
                }
            }

            // Horizontal
            for (int y = 0; y < symbolSize; y++)
            {
                for (int x = 0; x < (symbolSize - 7); x++)
                {
                    p = 0;
                    for (int weight = 0; weight < 7; weight++)
                    {
                        if (local[(y * symbolSize) + x + weight] == 1)
                            p += (0x40 >> weight);
                    }

                    if (p == 0x5d)
                    {
                        // Pattern found, check before and after.
                        beforeCount = 0;
                        for (int before = (x - 4); before < x; before++)
                        {
                            if (before < 0)
                                beforeCount++;

                            else
                            {
                                if (local[(y * symbolSize) + before] == 0)
                                    beforeCount++;

                                else
                                    beforeCount = 0;
                            }
                        }

                        afterCount = 0;
                        for (int after = (x + 7); after <= (x + 10); after++)
                        {
                            if (after >= symbolSize)
                                afterCount++;

                            else
                            {
                                if (local[(y * symbolSize) + after] == 0)
                                    afterCount++;

                                else
                                    afterCount = 0;
                            }
                        }

                        if ((beforeCount == 4) || (afterCount == 4))    // Pattern is preceeded or followed by light area 4 modules wide.
                            result += 40;
                    }
                }
            }

            // Test 4: Proportion of dark modules in entire symbol.
            darkModules = 0;
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    if (local[(y * symbolSize) + x] == 1)
                        darkModules++;
                }
            }

            percentage = 100 * (darkModules / (symbolSize * symbolSize));
            if (percentage <= 50)
                k = ((100 - percentage) - 50) / 5;

            else
                k = (percentage - 50) / 5;

            result += 10 * k;
            return result;
        }

        private static void AddFormatInformationEval(byte[] evaluationData, int symbolSize, QRCodeEccLevel eccLevel, int pattern)
        {
            // Add format information to the evaluation data.
            int format = pattern;
            uint sequence;

            switch (eccLevel)
            {
                case QRCodeEccLevel.Low:
                    format += 0x08;
                    break;

                case QRCodeEccLevel.Quartile:
                    format += 0x18;
                    break;

                case QRCodeEccLevel.High:
                    format += 0x10;
                    break;
            }

            sequence = QRFormatSeqence[format];

            for (int i = 0; i < 6; i++)
                evaluationData[(i * symbolSize) + 8] = (((sequence >> i) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;

            for (int i = 0; i < 8; i++)
                evaluationData[(8 * symbolSize) + (symbolSize - i - 1)] = (((sequence >> i) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;

            for (int i = 0; i < 6; i++)
                evaluationData[(8 * symbolSize) + (5 - i)] = (((sequence >> (i + 9)) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;

            for (int i = 0; i < 7; i++)
                evaluationData[(((symbolSize - 7) + i) * symbolSize) + 8] = (((sequence >> (i + 8)) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;

            evaluationData[(7 * symbolSize) + 8] = (((sequence >> 6) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;
            evaluationData[(8 * symbolSize) + 8] = (((sequence >> 7) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;
            evaluationData[(8 * symbolSize) + 7] = (((sequence >> 8) & 0x01) != 0) ? (byte)(0x01 >> pattern) : (byte)0x00;
        }

        private static void AddFormatInformationGrid(byte[] symbolGrid, int symbolSize, QRCodeEccLevel eccLevel, int bitMask)
        {
            // Add format information to symbol grid.
            int formatInfo = bitMask;

            switch (eccLevel)
            {
                case QRCodeEccLevel.Low:
                    formatInfo += 0x08;
                    break;

                case QRCodeEccLevel.Quartile:
                    formatInfo += 0x18;
                    break;

                case QRCodeEccLevel.High:
                    formatInfo += 0x10;
                    break;
            }

            uint seqence = QRFormatSeqence[formatInfo];

            for (int i = 0; i < 6; i++)
                symbolGrid[(i * symbolSize) + 8] += (byte)((seqence >> i) & 0x01);

            for (int i = 0; i < 8; i++)
                symbolGrid[(8 * symbolSize) + (symbolSize - i - 1)] += (byte)((seqence >> i) & 0x01);

            for (int i = 0; i < 6; i++)
                symbolGrid[(8 * symbolSize) + (5 - i)] += (byte)((seqence >> (i + 9)) & 0x01);

            for (int i = 0; i < 7; i++)
                symbolGrid[(((symbolSize - 7) + i) * symbolSize) + 8] += (byte)((seqence >> (i + 8)) & 0x01);

            symbolGrid[(7 * symbolSize) + 8] += (byte)((seqence >> 6) & 0x01);
            symbolGrid[(8 * symbolSize) + 8] += (byte)((seqence >> 7) & 0x01);
            symbolGrid[(8 * symbolSize) + 7] += (byte)((seqence >> 8) & 0x01);
        }

        private static int ApplyBitmask(byte[] symbolGrid, int symbolSize, QRCodeEccLevel eccLevel)
        {
            byte p;
            int[] penalty = new int[8];
            int bestPattern;
            int bestValue;

            byte[] mask = new byte[symbolSize * symbolSize];
            byte[] evaluation = new byte[symbolSize * symbolSize];

            // Perform data masking.
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    mask[(y * symbolSize) + x] = 0x00;

                    if ((symbolGrid[(y * symbolSize) + x] & 0xf0) == 0) // Exclude areas not to be masked.
                    {
                        if (((y + x) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x01;

                        if ((y & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x02;

                        if ((x % 3) == 0)
                            mask[(y * symbolSize) + x] += 0x04;

                        if (((y + x) % 3) == 0)
                            mask[(y * symbolSize) + x] += 0x08;

                        if ((((y / 2) + (x / 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x10;

                        if ((((y * x) & 1) + ((y * x) % 3)) == 0)
                            mask[(y * symbolSize) + x] += 0x20;

                        if (((((y * x) & 1) + ((y * x) % 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x40;

                        if (((((y + x) & 1) + ((y * x) % 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x80;

                    }
                }
            }

            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    if ((symbolGrid[(y * symbolSize) + x] & 0x01) > 0)
                        p = 0xff;

                    else
                        p = 0x00;

                    evaluation[(y * symbolSize) + x] = (byte)(mask[(y * symbolSize) + x] ^ p);
                }
            }

            // Evaluate result.
            for (int pattern = 0; pattern < 8; pattern++)
            {
                AddFormatInformationEval(evaluation, symbolSize, eccLevel, pattern);
                penalty[pattern] = Evaluate(evaluation, symbolSize, pattern);
            }

            bestPattern = 0;
            bestValue = penalty[0];
            for (int pattern = 1; pattern < 8; pattern++)
            {
                if (penalty[pattern] < bestValue)
                {
                    bestPattern = pattern;
                    bestValue = penalty[pattern];
                }
            }

            // Apply mask.
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    if ((mask[(y * symbolSize) + x] & (0x01 << bestPattern)) != 0)
                    {
                        {
                            if ((symbolGrid[(y * symbolSize) + x] & 0x01) != 0)
                                symbolGrid[(y * symbolSize) + x] = 0x00;

                            else
                                symbolGrid[(y * symbolSize) + x] = 0x01;
                        }
                    }
                }
            }

            return bestPattern;
        }

        private static void AddVersionInformation(byte[] symbolGrid, int symbolSize, int version)
        {
            // Add version information.
            int versionData = QRVersionSeqence[version - 7];
            for (int i = 0; i < 6; i++)
            {
                symbolGrid[((symbolSize - 11) * symbolSize) + i] += (byte)((versionData >> (i * 3)) & 0x41);
                symbolGrid[((symbolSize - 10) * symbolSize) + i] += (byte)((versionData >> ((i * 3) + 1)) & 0x41);
                symbolGrid[((symbolSize - 9) * symbolSize) + i] += (byte)((versionData >> ((i * 3) + 2)) & 0x41);
                symbolGrid[(i * symbolSize) + (symbolSize - 11)] += (byte)((versionData >> (i * 3)) & 0x01);
                symbolGrid[(i * symbolSize) + (symbolSize - 10)] += (byte)((versionData >> ((i * 3) + 1)) & 0x41);
                symbolGrid[(i * symbolSize) + (symbolSize - 9)] += (byte)((versionData >> ((i * 3) + 2)) & 0x41);
            }
        }

        private static char GetShiftJISCharacter(char data)
        {
            byte[] shiftJISBytes;
            char[] shiftJIS = new char[1];
            char shiftJISCharacter;

            try
            {
                shiftJIS[0] = data;
                shiftJISBytes = Encoding.GetEncoding("Shift_JIS",
                    new EncoderExceptionFallback(),
                    new DecoderExceptionFallback()).GetBytes(shiftJIS);
            }

            catch (EncoderFallbackException e)
            {
                throw new InvalidDataException("Shift_JIS: Invalid byte sequence.", e);
            }

            shiftJISCharacter = (char)(shiftJISBytes[0] << 8 | shiftJISBytes[1] & 0xff);
            return shiftJISCharacter;
        }

        private static int BlockLength(int start, char[] inputMode, int inputLength)
        {
            // Find the length of the block starting from 'start'.
            int i, count;
            char mode = inputMode[start];

            count = 0;
            i = start;

            do
            {
                count++;
            } while (((i + count) < inputLength) && (inputMode[i + count] == mode));

            return count;
        }

        private void BuildSymbol(byte[] symbolGrid, int symbolSize)
        {
            // Build the symbol.
            byte[] rowData;
            for (int i = 0; i < symbolSize; i++)
            {
                rowData = new byte[symbolSize];
                for (int j = 0; j < symbolSize; j++)
                {
                    if ((symbolGrid[(i * symbolSize) + j] & 0x01) > 0)
                        rowData[j] = 1;
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Insert(i, symbolData);
            }
        }

        /// <summary>
        /// MicroQR Encoder.
        /// </summary>
        private void MicroQRCode()
        {
            QRCodeEccLevel eccLevel;
            int symbolSize;
            int version;
            int inputLength = barcodeData.Length;
            bool byteOrKanjiUsed = false;
            bool alphaUsed = false;
            int targetCodewords = 0;
            int autoVersion;
            int[] versionValid = new int[4];
            int[] binaryCount = new int[4];
            BitVector bitStream = new BitVector();
            char[] mode = new char[inputLength];
            char[] jisData = new char[inputLength];

            if (inputLength > 35)
                throw new InvalidDataLengthException("MicoQR Code: Maximum input length is 35 characters.");

            eccLevel = QRCodeEccLevel.Low;
            // Check ecc option in combination with version option.
            if (optionErrorCorrection >= QRCodeEccLevel.Low && optionErrorCorrection <= QRCodeEccLevel.High)
            {
                if (optionErrorCorrection == QRCodeEccLevel.High)
                    throw new ErrorCorrectionLevelException("MicoQR Code: Error correction level H not available.");

                if (optionSymbolVersion >= 1 && optionSymbolVersion <= 4)
                {
                    if (optionSymbolVersion == 1 && optionErrorCorrection != QRCodeEccLevel.Low)
                        throw new ErrorCorrectionLevelException("MicoQR Code: Version M1 supports error correction level L only.");

                    if (optionSymbolVersion != 4 && optionErrorCorrection == QRCodeEccLevel.High)
                        throw new ErrorCorrectionLevelException("MicoQR Code: Error correction level Q requires Version M4.");
                }

                eccLevel = optionErrorCorrection;
            }

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] <= 0xff)
                    jisData[i] = barcodeData[i];

                else
                    jisData[i] = GetShiftJISCharacter(barcodeData[i]);
            }

            // Determine if alpha (excluding numerics), byte or kanji used.
            for (int i = 0; i < inputLength && (!alphaUsed || !byteOrKanjiUsed); i++)
            {
                if (!Char.IsDigit(jisData[i]))
                {
                    if (IsAlpha(jisData[i]))
                        alphaUsed = true;

                    else
                        byteOrKanjiUsed = true;
                }
            }

            for (int i = 0; i < 4; i++)
                versionValid[i] = 1;

            // Eliminate possible versions depending on type of content.
            if (byteOrKanjiUsed)
            {
                versionValid[0] = 0;
                versionValid[1] = 0;
            }

            else if (alphaUsed)
                versionValid[0] = 0;

            // Eliminate possible versions depending on error correction level specified.
            if (eccLevel == QRCodeEccLevel.Quartile)
            {
                versionValid[0] = 0;
                versionValid[1] = 0;
                versionValid[2] = 0;
            }

            else if (eccLevel == QRCodeEccLevel.Medium)
                versionValid[0] = 0;

            // Determine length of binary data.
            for (int i = 0; i < 4; i++)
            {
                if (versionValid[i] > 0)
                    binaryCount[i] = GetBinaryLength(MICROQR_VERSION + i, mode, jisData, inputLength);

                else
                    binaryCount[i] = 128 + 1;
            }

            // Eliminate possible versions depending on length of binary data.
            if (binaryCount[0] > 20)
                versionValid[0] = 0;

            if (binaryCount[1] > 40)
                versionValid[1] = 0;

            if (binaryCount[2] > 84)
                versionValid[2] = 0;

            if (binaryCount[3] > 128)
                throw new InvalidDataLengthException("MicoQR Code: Input data too long.");

            // Eliminate possible versions depending on binary length and error correction level specified.
            if (eccLevel == QRCodeEccLevel.Quartile)
            {
                if (binaryCount[3] > 80)
                    throw new InvalidDataLengthException("MicoQR Code: Input data too long for ECC level Q.");
            }

            else if (eccLevel == QRCodeEccLevel.Medium)
            {
                if (binaryCount[1] > 32)
                    versionValid[1] = 0;

                if (binaryCount[2] > 68)
                    versionValid[2] = 0;

                if (binaryCount[3] > 112)
                    throw new InvalidDataLengthException("MicoQR Code: Input data too long.");
            }

            autoVersion = 3;
            if (versionValid[2] == 1)
                autoVersion = 2;

            if (versionValid[1] == 1)
                autoVersion = 1;

            if (versionValid[0] == 1)
                autoVersion = 0;

            version = autoVersion;
            // Get version from user.
            if (optionSymbolVersion >= 1 && optionSymbolVersion <= 4)
            {
                if ((optionSymbolVersion - 1) >= autoVersion)
                    version = optionSymbolVersion - 1;

                else
                    throw new InvalidDataLengthException("MicoQR Code: Input data too long for selected symbol size.");
            }

            // If there is enough unused space then increase the error correction level.
            if ((int)optionErrorCorrection == -1 || optionErrorCorrection != eccLevel)
            {
                if (version == 3)
                {
                    if (binaryCount[3] <= 112)
                        eccLevel = QRCodeEccLevel.Medium;

                    if (binaryCount[3] <= 80)
                        eccLevel = QRCodeEccLevel.Quartile;
                }

                else if (version == 2)
                {
                    if (binaryCount[2] <= 68)
                        eccLevel = QRCodeEccLevel.Medium;
                }

                else if (version == 1)
                {
                    if (binaryCount[1] <= 32)
                        eccLevel = QRCodeEccLevel.Medium;
                }
            }

            DefineModes(mode, jisData, inputLength, MICROQR_VERSION + version);
            QRBinary(bitStream, MICROQR_VERSION + version, targetCodewords, mode, jisData);
            switch (version)
            {
                case 0:
                    MicroQRM1(bitStream);
                    break;

                case 1:
                    MicroQRM2(bitStream, eccLevel);
                    break;

                case 2:
                    MicroQRM3(bitStream, eccLevel);
                    break;

                case 3:
                    MicroQRM4(bitStream, eccLevel);
                    break;
            }

            symbolSize = MQRSizes[version];
            byte[] symbolGrid = new byte[symbolSize * symbolSize];

            MicroSetupGrid(symbolGrid, symbolSize);
            MicroPopulateGrid(symbolGrid, symbolSize, bitStream);
            int bitmask = MicroApplyBitmask(symbolGrid, symbolSize);

            // Add format data.
            int format = 0;
            switch (version)
            {
                case 1:
                    switch (eccLevel)
                    {
                        case QRCodeEccLevel.Low:
                            format = 1;
                            break;

                        case QRCodeEccLevel.Medium:
                            format = 2;
                            break;
                    }
                    break;

                case 2:
                    switch (eccLevel)
                    {
                        case QRCodeEccLevel.Low:
                            format = 3;
                            break;

                        case QRCodeEccLevel.Medium:
                            format = 4;
                            break;
                    }
                    break;

                case 3: switch (eccLevel)
                    {
                        case QRCodeEccLevel.Low:
                            format = 5;
                            break;

                        case QRCodeEccLevel.Medium:
                            format = 6;
                            break;

                        case QRCodeEccLevel.Quartile:
                            format = 7;
                            break;
                    }
                    break;
            }

            int formatSequence = MQRFormatSequence[(format << 2) + bitmask];
            int value = 0x4000;
            int count = 15;
            for (int x = 1; x < 16; x++)
            {
                if ((formatSequence & value) != 0)
                {
                    if (x < 9)
                        symbolGrid[(8 * symbolSize) + x] += 1;

                    else
                        symbolGrid[(count * symbolSize) + 8] += 1;
                }

                value /= 2;
                count--;
            }

            BuildSymbol(symbolGrid, symbolSize);
        }

        private static void MicroQRM1(BitVector bitStream)
        {
            int remainder;
            int dataCodewords;
            int eccCodewords;
            byte[] dataBlocks;
            byte[] eccBlocks;

            int bitsTotal = 20;
            bool done = false;

            // Add terminator.
            int bitsLeft = bitsTotal - bitStream.SizeInBits;
            if (bitsLeft <= 3)
            {
                bitStream.AppendBits(0, bitsLeft);
                done = true;
            }

            else
                bitStream.AppendBits(0, 3);

            if (!done)
            {
                // Manage last (4-bit) block.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                if (bitsLeft <= 4)
                {
                    bitStream.AppendBits(0, bitsLeft);
                    done = true;
                }
            }

            if (!done)
            {
                // Complete current byte.
                remainder = 8 - (bitStream.SizeInBits % 8);
                if (remainder == 8)
                    remainder = 0;

                bitStream.AppendBits(0, remainder);

                // Add padding.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                if (bitsLeft > 4)
                {
                    remainder = (bitsLeft - 4) / 8;
                    for (int i = 0; i < remainder; i++)
                    {
                        if ((i & 1) != 0)
                            bitStream.AppendBits(0x11, 8);

                        else
                            bitStream.AppendBits(0xec, 8);
                    }
                }

                bitStream.AppendBits(0, 4);
            }

            dataCodewords = 3;
            eccCodewords = 2;
            dataBlocks = bitStream.ToByteArray();
            eccBlocks = new byte[eccCodewords];

            // Calculate Reed-Solomon error correction codewords.
            ReedSolomon.RSInitialise(0x11d, eccCodewords, 0);
            ReedSolomon.RSEncode(dataCodewords, dataBlocks, eccBlocks);

            // Add Reed-Solomon codewords to binary data.
            for (int i = 0; i < eccCodewords; i++)
                bitStream.AppendBits(eccBlocks[eccCodewords - i - 1], 8);
        }

        private static void MicroQRM2(BitVector bitStream, QRCodeEccLevel eccLevel)
        {
            int bitsLeft, remainder;
            int dataCodewords = 0;
            int eccCodewords = 0;
            byte[] dataBlocks;
            byte[] eccBlocks;
            bool done = false;
            int bitsTotal = 0;

            if (eccLevel == QRCodeEccLevel.Low)
                bitsTotal = 40;

            if (eccLevel == QRCodeEccLevel.Medium)
                bitsTotal = 32;

            // Add terminator.
            bitsLeft = bitsTotal - bitStream.SizeInBits;
            if (bitsLeft <= 5)
            {
                bitStream.AppendBits(0, bitsLeft);
                done = true;
            }

            else
                bitStream.AppendBits(0, 5);


            if (!done)
            {
                // Complete current byte.
                remainder = 8 - (bitStream.SizeInBits % 8);
                if (remainder == 8)
                    remainder = 0;

                bitStream.AppendBits(0, remainder);

                // Add padding.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                remainder = bitsLeft / 8;
                for (int i = 0; i < remainder; i++)
                {
                    if ((i & 1) != 0)
                        bitStream.AppendBits(0x11, 8);

                    else
                        bitStream.AppendBits(0xec, 8);
                }
            }

            if (eccLevel == QRCodeEccLevel.Low)
            {
                dataCodewords = 5;
                eccCodewords = 5;
            }

            if (eccLevel == QRCodeEccLevel.Medium)
            {
                dataCodewords = 4;
                eccCodewords = 6;
            }

            dataBlocks = bitStream.ToByteArray();
            eccBlocks = new byte[eccCodewords];

            // Calculate Reed-Solomon error codewords.
            ReedSolomon.RSInitialise(0x11d, eccCodewords, 0);
            ReedSolomon.RSEncode(dataCodewords, dataBlocks, eccBlocks);

            // Add Reed-Solomon codewords to binary data.
            for (int i = 0; i < eccCodewords; i++)
                bitStream.AppendBits(eccBlocks[eccCodewords - i - 1], 8);
        }

        private static void MicroQRM3(BitVector bitStream, QRCodeEccLevel eccLevel)
        {
            int bitsLeft;
            int remainder;
            byte[] dataBlocks;
            byte[] eccBlocks;

            int bitsTotal = 0;
            int dataCodewords = 0;
            int eccCodewords = 0;
            bool done = false;

            if (eccLevel == QRCodeEccLevel.Low)
                bitsTotal = 84;

            if (eccLevel == QRCodeEccLevel.Medium)
                bitsTotal = 68;

            // Add terminator.
            bitsLeft = bitsTotal - bitStream.SizeInBits;
            if (bitsLeft <= 7)
            {
                bitStream.AppendBits(0, bitsLeft);
                done = true;
            }

            else
                bitStream.AppendBits(0, 7);

            if (!done)
            {
                // Manage last (4-bit) block.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                if (bitsLeft <= 4)
                {
                    bitStream.AppendBits(0, bitsLeft);
                    done = true;
                }
            }

            if (!done)
            {
                // Complete current byte.
                remainder = 8 - (bitStream.SizeInBits % 8);
                if (remainder == 8)
                    remainder = 0;

                bitStream.AppendBits(0, remainder);

                // Add padding.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                if (bitsLeft > 4)
                {
                    remainder = (bitsLeft - 4) / 8;
                    for (int i = 0; i < remainder; i++)
                    {
                        if ((i & 1) != 0)
                            bitStream.AppendBits(0x11, 8);

                        else
                            bitStream.AppendBits(0xec, 8);
                    }
                }

                bitStream.AppendBits(0, 4);
            }

            if (eccLevel == QRCodeEccLevel.Low)
            {
                dataCodewords = 11;
                eccCodewords = 6;
            }

            if (eccLevel == QRCodeEccLevel.Medium)
            {
                dataCodewords = 9;
                eccCodewords = 8;
            }

            dataBlocks = bitStream.ToByteArray();
            eccBlocks = new byte[eccCodewords];

            if (eccLevel == QRCodeEccLevel.Low)
                dataBlocks[10] = (byte)(dataBlocks[10] >> 4);

            if (eccLevel == QRCodeEccLevel.Medium)
                dataBlocks[8] = (byte)(dataBlocks[8] >> 4);

            // Calculate Reed-Solomon error codewords.
            ReedSolomon.RSInitialise(0x11d, eccCodewords, 0);
            ReedSolomon.RSEncode(dataCodewords, dataBlocks, eccBlocks);

            // Add Reed-Solomon codewords to binary data.
            for (int i = 0; i < eccCodewords; i++)
                bitStream.AppendBits(eccBlocks[eccCodewords - i - 1], 8);
        }

        private static void MicroQRM4(BitVector bitStream, QRCodeEccLevel eccLevel)
        {
            int bitsLeft;
            int remainder;
            int bitsTotal = 0;
            int dataCodewords = 0;
            int eccCodewords = 0;
            byte[] dataBlocks;
            byte[] eccBlocks;
            bool done = false;

            if (eccLevel == QRCodeEccLevel.Low)
                bitsTotal = 128;

            if (eccLevel == QRCodeEccLevel.Medium)
                bitsTotal = 112;

            if (eccLevel == QRCodeEccLevel.Quartile)
                bitsTotal = 80;

            // Add terminator.
            bitsLeft = bitsTotal - bitStream.SizeInBits;
            if (bitsLeft <= 9)
            {
                bitStream.AppendBits(0, bitsLeft);
                done = true;
            }

            else
                bitStream.AppendBits(0, 9);

            if (!done)
            {
                // Complete current byte.
                remainder = 8 - (bitStream.SizeInBits % 8);
                if (remainder == 8)
                    remainder = 0;

                bitStream.AppendBits(0, remainder);

                // Add padding.
                bitsLeft = bitsTotal - bitStream.SizeInBits;
                remainder = bitsLeft / 8;
                for (int i = 0; i < remainder; i++)
                {
                    if ((i & 1) != 0)
                        bitStream.AppendBits(0x11, 8);

                    else
                        bitStream.AppendBits(0xec, 8);
                }
            }

            if (eccLevel == QRCodeEccLevel.Low)
            {
                dataCodewords = 16;
                eccCodewords = 8;
            }

            if (eccLevel == QRCodeEccLevel.Medium)
            {
                dataCodewords = 14;
                eccCodewords = 10;
            }

            if (eccLevel == QRCodeEccLevel.Quartile)
            {
                dataCodewords = 10;
                eccCodewords = 14;
            }

            dataBlocks = bitStream.ToByteArray();
            eccBlocks = new byte[eccCodewords];

            // Calculate Reed-Solomon error codewords.
            ReedSolomon.RSInitialise(0x11d, eccCodewords, 0);
            ReedSolomon.RSEncode(dataCodewords, dataBlocks, eccBlocks);

            // Add Reed-Solomon codewords to binary data.
            for (int i = 0; i < eccCodewords; i++)
                bitStream.AppendBits(eccBlocks[eccCodewords - i - 1], 8);
        }

        private static void MicroSetupGrid(byte[] symbolGrid, int symbolSize)
        {
            bool latch = true; ;

            // Add timing patterns.
            for (int i = 0; i < symbolSize; i++)
            {
                if (latch)
                {
                    symbolGrid[i] = 0x21;
                    symbolGrid[(i * symbolSize)] = 0x21;
                    latch = false;
                }

                else
                {
                    symbolGrid[i] = 0x20;
                    symbolGrid[(i * symbolSize)] = 0x20;
                    latch = true;
                }
            }

            // Add finder patterns.
            PlaceFinderPatterns(symbolGrid, symbolSize, 0, 0);

            // Add separators.
            for (int i = 0; i < 7; i++)
            {
                symbolGrid[(7 * symbolSize) + i] = 0x10;
                symbolGrid[(i * symbolSize) + 7] = 0x10;
            }

            symbolGrid[(7 * symbolSize) + 7] = 0x10;


            // Reserve space for format information.
            for (int i = 0; i < 8; i++)
            {
                symbolGrid[(8 * symbolSize) + i] += 0x20;
                symbolGrid[(i * symbolSize) + 8] += 0x20;
            }

            symbolGrid[(8 * symbolSize) + 8] += 20;
        }

        private static void MicroPopulateGrid(byte[] symbolGrid, int symbolSize, BitVector bitStream)
        {
            int direction = 1;  // Up.
            int row = 0;        // Right hand side.

            int n = bitStream.SizeInBits;
            int y = symbolSize - 1;
            int i = 0;
            int x;
            do
            {
                x = (symbolSize - 2) - (row * 2);

                if ((symbolGrid[(y * symbolSize) + (x + 1)] & 0xf0) == 0)
                {
                    if (bitStream[i] == 1)
                        symbolGrid[(y * symbolSize) + (x + 1)] = 0x01;

                    else
                        symbolGrid[(y * symbolSize) + (x + 1)] = 0x00;

                    i++;
                }

                if (i < n)
                {
                    if ((symbolGrid[(y * symbolSize) + x] & 0xf0) == 0)
                    {
                        if (bitStream[i] == 1)
                            symbolGrid[(y * symbolSize) + x] = 0x01;

                        else
                            symbolGrid[(y * symbolSize) + x] = 0x00;

                        i++;
                    }
                }

                if (direction == 1)
                    y--;

                else
                    y++;

                if (y == 0)
                {
                    // Reached the top.
                    row++;
                    y = 1;
                    direction = 0;
                }

                if (y == symbolSize)
                {
                    // Reached the bottom.
                    row++;
                    y = symbolSize - 1;
                    direction = 1;
                }
            }
            while (i < n);
        }

        private static int MicroEvaluate(byte[] data, int symbolSize, int pattern)
        {
            byte filter = 0;
            int result = 0;

            switch (pattern)
            {
                case 0:
                    filter = 0x01;
                    break;

                case 1:
                    filter = 0x02;
                    break;

                case 2:
                    filter = 0x04;
                    break;

                case 3:
                    filter = 0x08;
                    break;
            }

            int sum1 = 0;
            int sum2 = 0;
            for (int i = 1; i < symbolSize; i++)
            {
                if ((data[(i * symbolSize) + symbolSize - 1] & filter) != 0)
                    sum1++;

                if ((data[((symbolSize - 1) * symbolSize) + i] & filter) != 0)
                    sum2++;
            }

            if (sum1 <= sum2)
                result = (sum1 * 16) + sum2;

            else
                result = (sum2 * 16) + sum1;

            return result;
        }

        private static int MicroApplyBitmask(byte[] symbolGrid, int symbolSize)
        {
            byte p;
            int pattern;
            int[] value = new int[8];
            int bestValue, bestPattern;
            int bit;

            byte[] mask = new byte[symbolSize * symbolSize];
            byte[] Evaluate = new byte[symbolSize * symbolSize];

            // Perform data masking.
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    mask[(y * symbolSize) + x] = 0x00;

                    if ((symbolGrid[(y * symbolSize) + x] & 0xf0) == 0)
                    {
                        if ((y & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x01;

                        if ((((y / 2) + (x / 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x02;

                        if (((((y * x) & 1) + ((y * x) % 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x04;

                        if (((((y + x) & 1) + ((y * x) % 3)) & 1) == 0)
                            mask[(y * symbolSize) + x] += 0x08;
                    }
                }
            }

            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    if ((symbolGrid[(y * symbolSize) + x] & 0x01) != 0)
                        p = 0xff;

                    else
                        p = 0x00;

                    Evaluate[(y * symbolSize) + x] = (byte)(mask[(y * symbolSize) + x] ^ p);
                }
            }

            // Evaluate result.
            for (pattern = 0; pattern < 8; pattern++)
                value[pattern] = MicroEvaluate(Evaluate, symbolSize, pattern);

            bestPattern = 0;
            bestValue = value[0];
            for (pattern = 1; pattern < 4; pattern++)
            {
                if (value[pattern] > bestValue)
                {
                    bestPattern = pattern;
                    bestValue = value[pattern];
                }
            }

            // Apply mask.
            for (int x = 0; x < symbolSize; x++)
            {
                for (int y = 0; y < symbolSize; y++)
                {
                    bit = 0;
                    switch (bestPattern)
                    {
                        case 0:
                            if ((mask[(y * symbolSize) + x] & 0x01) != 0)
                                bit = 1;

                            break;
                        case 1:
                            if ((mask[(y * symbolSize) + x] & 0x02) != 0)
                                bit = 1;

                            break;
                        case 2:
                            if ((mask[(y * symbolSize) + x] & 0x04) != 0)
                                bit = 1;

                            break;
                        case 3:
                            if ((mask[(y * symbolSize) + x] & 0x08) != 0)
                                bit = 1;

                            break;
                    }

                    if (bit == 1)
                    {
                        if ((symbolGrid[(y * symbolSize) + x] & 0x01) != 0)
                            symbolGrid[(y * symbolSize) + x] = 0;

                        else
                            symbolGrid[(y * symbolSize) + x] = 1;
                    }
                }
            }

            return bestPattern;
        }

        private void UPNQR()
        {
            /* Characteristics of the printed QR code on the UPN QR form 
             a.  Version QR: Version 15 (77x77 modules).  Version 15 is required, regardless of the amount of data entered. 
             b.  Data type QR: Byte data (Binary, binary). 
             c.  Data Correction Rate QR: ECC_M (Error Correction Level M). 
             d.  Character set QR: ISO 8859-2.  It is mandatory to use Extended Channel Interpretation (ECI value 000004). 
             e.  Module size QR: 0.42333 mm x 0.42333 mm. 
             f.  Size of QR code with mandatory white margin: 32.59676 x 32.59667 mm 
             g.  Space occupied by QR with mandatory white edge: 35.98333 x 35.98333 mm. 
             h.  Minimum resolution of QR output: 600 x 600 DPI. 

             If we fulfill the QR code, the limits for the length of the individual fields are in accordance with the QR structure. 
             The content of the QR record must be identical to the record on the form. */
            int version;
            QRCodeEccLevel eccLevel;
            int symbolSize;
            int estimatedBinaryLength;
            BitVector bitStream;
            int inputLength = barcodeData.Length;
            char[] upnData = new char[inputLength];
            char[] mode = new char[inputLength];

            eci = 4;
            version = 15;
            eccLevel = QRCodeEccLevel.Medium;

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] <= 0xff)
                {
                    //upnData[i] = GetISO8859Character(barcodeData[i]);
                    upnData[i] = barcodeData[i];
                    mode[i] = 'B';
                }

                else
                    throw new InvalidDataException("UPN QR Code: Invalid data in input.");
            }

            estimatedBinaryLength = GetBinaryLength(version, mode, upnData, inputLength);
            if (estimatedBinaryLength > 3320)
                throw new InvalidDataLengthException("UPN QR Code: Input data too long.");

            int targetCodewords = QRDataCodeWordsMedium[version - 1];
            int blocks = QRBlocksMedium[version - 1];
            bitStream = new BitVector(QRTotalCodeWords[version - 1]);
            QRBinary(bitStream, version, targetCodewords, mode, upnData);
            AddErrorCorrection(bitStream, version, targetCodewords, blocks);
            symbolSize = QRSizes[version - 1];
            byte[] symbolGrid = new byte[symbolSize * symbolSize];
            SetupGrid(symbolGrid, symbolSize, version);
            PopulateGrid(symbolGrid, symbolSize, symbolSize, bitStream);
            AddVersionInformation(symbolGrid, symbolSize, version);
            int bitMask = ApplyBitmask(symbolGrid, symbolSize, eccLevel);
            AddFormatInformationGrid(symbolGrid, symbolSize, eccLevel, bitMask);
            BuildSymbol(symbolGrid, symbolSize);
        }

        // Retangular micro QR code according to 2018 draft standard.
        private void RMicroQRCode()
        {
            QRCodeEccLevel eccLevel;
            int inputLength = barcodeData.Length;
            int estimatedBinaryLength;
            int autosize, version, maxCodewords, targetCodewords, blocks, hSize, vSize;
            int footprint, bestFootprint, formatData;
            uint leftFormatInfo, rightFormatInfo;

            BitVector bitStream;
            char[] mode = new char[inputLength];
            char[] jisData = new char[inputLength];

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] <= 0xff)
                    jisData[i] = barcodeData[i];

                else
                    jisData[i] = GetShiftJISCharacter(barcodeData[i]);
            }

            estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + 31, mode, jisData, inputLength);
            eccLevel = QRCodeEccLevel.Medium;
            maxCodewords = 152;
            if (optionErrorCorrection == QRCodeEccLevel.Low)
                throw new ErrorCorrectionLevelException("RMicoQR Code: Error correction level L not available.");

            if (optionErrorCorrection == QRCodeEccLevel.Quartile)
                throw new ErrorCorrectionLevelException("RMicoQR Code: Error correction level Q not available.");

            if (optionErrorCorrection == QRCodeEccLevel.High)
            {
                eccLevel = QRCodeEccLevel.High;
                maxCodewords = 76;
            }

            if (estimatedBinaryLength > (8 * maxCodewords))
                throw new InvalidDataException("RMicoQR Code: Input too long for selected error correction level.");

            version = 31; // Set default to keep compiler happy
            if (optionSymbolVersion == 0)
            {
                // Automatic symbol size
                autosize = 31;
                bestFootprint = RMQRHeight[31] * RMQRWidth[31];
                for (version = 30; version >= 0; version--)
                {
                    estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + version, mode, jisData, inputLength);
                    footprint = RMQRHeight[version] * RMQRWidth[version];
                    if (eccLevel == QRCodeEccLevel.Medium)
                    {
                        if (8 * RMQRDataCodewordsM[version] >= estimatedBinaryLength)
                        {
                            if (footprint < bestFootprint)
                            {
                                autosize = version;
                                bestFootprint = footprint;
                            }
                        }
                    }

                    else
                    {
                        if (8 * RMQRDataCodewordsH[version] >= estimatedBinaryLength)
                        {
                            if (footprint < bestFootprint)
                            {
                                autosize = version;
                                bestFootprint = footprint;
                            }
                        }
                    }
                }

                version = autosize;
                estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + version, mode, jisData, inputLength);
            }

            // User specified symbol size.
            if ((optionSymbolVersion >= 1) && (optionSymbolVersion <= 32))
            {
                version = optionSymbolVersion - 1;
                estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + version, mode, jisData, inputLength);
            }

            if (optionSymbolVersion >= 33)
            {
                // User has specified symbol height only
                version = RMQRFixedHeightUpper[optionSymbolVersion - 32];
                for (int i = version - 1; i > RMQRFixedHeightUpper[optionSymbolVersion - 33]; i--)
                {
                    estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + i, mode, jisData, inputLength);
                    if (eccLevel == QRCodeEccLevel.Medium)
                    {
                        if (8 * RMQRDataCodewordsM[i] >= estimatedBinaryLength)
                            version = i;
                    }

                    else
                    {
                        if (8 * RMQRDataCodewordsH[i] >= estimatedBinaryLength)
                            version = i;
                    }
                }

                estimatedBinaryLength = GetBinaryLength(RMQR_VERSION + version, mode, jisData, inputLength);
            }

            if ((int)optionErrorCorrection == -1)
            {
                // Detect if there is enough free space to increase ECC level
                if (estimatedBinaryLength < (RMQRDataCodewordsH[version] * 8))
                    eccLevel = QRCodeEccLevel.High;
            }

            if (eccLevel == QRCodeEccLevel.Medium)
            {
                targetCodewords = RMQRDataCodewordsM[version];
                blocks = RMQRBlocksM[version];
            }

            else
            {
                targetCodewords = RMQRDataCodewordsH[version];
                blocks = RMQRBlocksH[version];
            }

            if (estimatedBinaryLength > (targetCodewords * 8))
            {
                // User has selected a symbol size too small for the data.
                throw new InvalidDataException("RMicoQR Code: Input too long for selected symbol size.");
            }

            bitStream = new BitVector(RMQRTotalCodewords[version]);
            QRBinary(bitStream, RMQR_VERSION + version, targetCodewords, mode, jisData);
            AddErrorCorrection(bitStream, RMQR_VERSION + version, targetCodewords, blocks);

            hSize = RMQRWidth[version];
            vSize = RMQRHeight[version];
            byte[] symbolGrid = new byte[hSize * vSize];
            SetupRMQRGrid(symbolGrid, hSize, vSize);
            PopulateGrid(symbolGrid, hSize, vSize, bitStream);

            // Apply bitmask.
            for (int i = 0; i < vSize; i++)
            {
                for (int j = 0; j < hSize; j++)
                {
                    if ((symbolGrid[(i * hSize) + j] & 0xf0) == 0)
                    {
                        // This is a data module
                        if (((i / 2) + (j / 3)) % 2 == 0)
                        {
                            // This is the data mask from section 7.8.2
                            // This module needs to be changed
                            if (symbolGrid[(i * hSize) + j] == 0x01)
                                symbolGrid[(i * hSize) + j] = 0x00;

                            else
                                symbolGrid[(i * hSize) + j] = 0x01;
                        }
                    }
                }
            }

            // Add format information.
            formatData = version;
            if (eccLevel == QRCodeEccLevel.High)
                formatData += 32;

            leftFormatInfo = RMQRFormatInfoLeft[formatData];
            rightFormatInfo = RMQRFormatInfoRight[formatData];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    symbolGrid[(hSize * (i + 1)) + j + 8] = (byte)((leftFormatInfo >> ((j * 5) + i)) & 0x01);
                    symbolGrid[(hSize * (vSize - 6)) + (hSize * i) + j + (hSize - 8)] = (byte)((rightFormatInfo >> ((j * 5) + i)) & 0x01);
                }
            }

            symbolGrid[(hSize * 1) + 11] = (byte)((leftFormatInfo >> 15) & 0x01);
            symbolGrid[(hSize * 2) + 11] = (byte)((leftFormatInfo >> 16) & 0x01);
            symbolGrid[(hSize * 3) + 11] = (byte)((leftFormatInfo >> 17) & 0x01);
            symbolGrid[(hSize * (vSize - 6)) + (hSize - 5)] = (byte)((rightFormatInfo >> 15) & 0x01);
            symbolGrid[(hSize * (vSize - 6)) + (hSize - 4)] = (byte)((rightFormatInfo >> 16) & 0x01);
            symbolGrid[(hSize * (vSize - 6)) + (hSize - 3)] = (byte)((rightFormatInfo >> 17) & 0x01);

            byte[] rowData;
            for (int y = 0; y < vSize; y++)
            {
                rowData = new byte[hSize];
                for (int x = 0; x < hSize; x++)
                {
                    if ((symbolGrid[(hSize * y) + x] & 0x01) > 0)
                        rowData[x] = 1;
                }

                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Add(symbolData);
            }
        }

        private static void SetupRMQRGrid(byte[] grid, int hSize, int vSize)
        {
            byte[] alignment = { 0x1F, 0x11, 0x15, 0x11, 0x1F };
            int hVersion = 0;
            int finderPosition;

            // Add timing patterns - top and bottom.
            for (int i = 0; i < hSize; i++)
            {
                if (i % 2 > 0)
                {
                    grid[i] = 0x20;
                    grid[((vSize - 1) * hSize) + i] = 0x20;
                }

                else
                {
                    grid[i] = 0x21;
                    grid[((vSize - 1) * hSize) + i] = 0x21;
                }
            }

            // Add timing patterns - left and right.
            for (int i = 0; i < vSize; i++)
            {
                if (i % 2 > 0)
                {
                    grid[i * hSize] = 0x20;
                    grid[(i * hSize) + (hSize - 1)] = 0x20;
                }

                else
                {
                    grid[i * hSize] = 0x21;
                    grid[(i * hSize) + (hSize - 1)] = 0x21;
                }
            }

            // Add finder pattern.
            PlaceFinderPatterns(grid, hSize, 0, 0); // This works because finder is always top left

            // Add finder sub-pattern to bottom right.
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((alignment[j] & 0x10 >> i) > 0)
                        grid[((vSize - 5) * hSize) + (hSize * i) + (hSize - 5) + j] = 0x11;

                    else
                        grid[((vSize - 5) * hSize) + (hSize * i) + (hSize - 5) + j] = 0x10;
                }
            }

            // Add corner finder pattern - bottom left.
            grid[(vSize - 2) * hSize] = 0x11;
            grid[((vSize - 2) * hSize) + 1] = 0x10;
            grid[((vSize - 1) * hSize) + 1] = 0x11;

            // Add corner finder pattern - top right.
            grid[hSize - 2] = 0x11;
            grid[(hSize * 2) - 2] = 0x10;
            grid[(hSize * 2) - 1] = 0x11;

            // Add seperator.
            for (int i = 0; i < 7; i++)
                grid[(i * hSize) + 7] = 0x20;

            if (vSize > 7)
            {
                // Note for vSize = 9 this overrides the bottom right corner finder pattern
                for (int i = 0; i < 8; i++)
                    grid[(7 * hSize) + i] = 0x20;
            }

            // Add alignment patterns.
            if (hSize > 27)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (hSize == RMQRWidth[i])
                        hVersion = i;
                }

                for (int i = 0; i < 4; i++)
                {
                    finderPosition = RMQRTableD1[(hVersion * 4) + i];
                    if (finderPosition != 0)
                    {
                        for (int j = 0; j < vSize; j++)
                        {
                            if (j % 2 > 0)
                                grid[(j * hSize) + finderPosition] = 0x10;

                            else
                                grid[(j * hSize) + finderPosition] = 0x11;
                        }

                        // Top square
                        grid[hSize + finderPosition - 1] = 0x11;
                        grid[(hSize * 2) + finderPosition - 1] = 0x11;
                        grid[hSize + finderPosition + 1] = 0x11;
                        grid[(hSize * 2) + finderPosition + 1] = 0x11;

                        // Bottom square
                        grid[(hSize * (vSize - 3)) + finderPosition - 1] = 0x11;
                        grid[(hSize * (vSize - 2)) + finderPosition - 1] = 0x11;
                        grid[(hSize * (vSize - 3)) + finderPosition + 1] = 0x11;
                        grid[(hSize * (vSize - 2)) + finderPosition + 1] = 0x11;
                    }
                }
            }

            // Reserve space for format information.
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    grid[(hSize * (i + 1)) + j + 8] = 0x20;
                    grid[(hSize * (vSize - 6)) + (hSize * i) + j + (hSize - 8)] = 0x20;
                }
            }

            grid[(hSize * 1) + 11] = 0x20;
            grid[(hSize * 2) + 11] = 0x20;
            grid[(hSize * 3) + 11] = 0x20;
            grid[(hSize * (vSize - 6)) + (hSize - 5)] = 0x20;
            grid[(hSize * (vSize - 6)) + (hSize - 4)] = 0x20;
            grid[(hSize * (vSize - 6)) + (hSize - 3)] = 0x20;
        }
    }
}