/* Code128Encoder.cs - Handles Code 128 based and GS1 1D symbols */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
 
    libzint - the open source barcode library.
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
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Code 128 based and GSI sybol encoder.
    /// </summary>
    internal class Code128Encoder : SymbolEncoder
    {
        #region Tables and Constants

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

        private const int C128_MAX = 256;
        private const int C128_SYMBOL_MAX = 102;

        // Code Set states.
        private const int C128_A0 = 1;
        private const int C128_B0 = 2;
        private const int C128_A1 = 3;
        private const int C128_B1 = 4;
        private const int C128_C0 = 5;
        private const int C128_C1 = 6;
        private const int C128_STATES = 7;

        readonly byte[][][] c128LatchSequence = new byte[C128_STATES][][] {
        // Current:                                   A0                            B0                            A1                            B1                            C0                           C1                   Prior
        new byte[][] { new byte[] { 0 } },
        new byte[][] { new byte[] { 0 }, new byte[] { 0             }, new byte[] { 100           }, new byte[] { 101, 101      }, new byte[] { 100, 100, 100 }, new byte[] { 99           }, new byte[] { 101, 101, 99 } },    // A0.
        new byte[][] { new byte[] { 0 }, new byte[] { 101           }, new byte[] { 0             }, new byte[] { 101, 101, 101 }, new byte[] { 100, 100      }, new byte[] { 99           }, new byte[] { 100, 100, 99 } },    // B0.
        new byte[][] { new byte[] { 0 }, new byte[] { 101, 101      }, new byte[] { 100, 100, 100 }, new byte[] { 0             }, new byte[] { 100           }, new byte[] { 101, 101, 99 }, new byte[] { 99           } },    // A1.
        new byte[][] { new byte[] { 0 }, new byte[] { 101, 101, 101 }, new byte[] { 100, 100      }, new byte[] { 101           }, new byte[] { 0             }, new byte[] { 100, 100, 99 }, new byte[] { 99           } },    // B1.
        new byte[][] { new byte[] { 0 }, new byte[] { 101           }, new byte[] { 100           }, new byte[] { 101, 101, 101 }, new byte[] { 100, 100, 100 }, new byte[] { 0            }, new byte[] { 0            } },    // C0.
        new byte[][] { new byte[] { 0 }, new byte[] { 101, 101, 101 }, new byte[] { 100, 100, 100 }, new byte[] { 101           }, new byte[] { 100           }, new byte[] { 0            }, new byte[] { 0            } } };  // C1.

        // Lengths of above.
        readonly byte[][] c128LatchLength = new byte[C128_STATES][] { 
        // Current:  A0             B0             A1             B1             C0             C1         Prior
        new byte[] {   0                                                                                           },
        new byte[] {   0,        0,             1,             2,             3,             1,             3      },   // A0
        new byte[] {   0,        1,             0,             3,             2,             1,             3      },   // B0
        new byte[] {   0,        2,             3,             0,             1,             3,             1      },   // A1
        new byte[] {   0,        3,             2,             1,             0,             3,             1      },   // B1
        new byte[] {   0,        1,             1,             3,             3,             0,            64      },   // C0
        new byte[] {   0,        3,             3,             1,             1,            64,             0      } }; // C1

        // Start sequences for normal, GS1_MODE and READER_INIT (mutually exclusive).
        readonly byte[][][] c128StartLatchSequence = new byte[3][][] {
        //                                            A0                      B0                      A1                                 B1                                 C0                    C1 (not used).
        new byte[][] { new byte[]  { 0 }, new byte[] { 103     }, new byte[] { 104     }, new byte[] { 103, 101, 101      }, new byte[] { 104, 100, 100      }, new byte[] { 105         } },     // Normal.
        new byte[][] { new byte[]  { 0 }, new byte[] { 103,102 }, new byte[] { 104,102 }, new byte[] { 103, 102, 101, 101 }, new byte[] { 104, 102, 100, 100 }, new byte[] { 105, 102    } },     // GS1_MODE.
        new byte[][] { new byte[]  { 0 }, new byte[] { 103, 96 }, new byte[] { 104, 96 }, new byte[] { 103, 96, 101, 101  }, new byte[] { 104, 96, 100, 100  }, new byte[] { 104, 96, 99 } } };   // READER_INIT.

        // Lengths of above
        readonly byte[][] c128StartLatchLength = new byte[3][] {
        //                   A0      B0      A1      B1      C0     C1 (not used).
        new byte[] {   0,    1,      1,      3,      3,      1,     64 },   // Normal.
        new byte[] {   0,    2,      2,      4,      4,      2,     64 },   // GS1_MODE.
        new byte[] {   0,    2,      2,      4,      4,      3,     64 },   // READER_INIT.
};
        #endregion

        private StringBuilder rowPattern;
        private readonly bool abModesOnly;

        // Code 128 standard.
        public Code128Encoder(Symbology symbolId, char[] barcodeMessage, EncodingFormat encodingMode, bool abModeOnly)
            : this(symbolId, barcodeMessage, null, CompositeMode.CCA, encodingMode, abModeOnly)
        { }

        // Code 128 HIBC.
        public Code128Encoder(Symbology symbolId, char[] barcodeMessage, EncodingFormat encodingMode)
            : this(symbolId, barcodeMessage, null, CompositeMode.CCA, encodingMode, false)
        { }

        // GS1 128.
        public Code128Encoder(Symbology symbolId, char[] barcodeMessage, char[] compositeMessage, CompositeMode compositeMode, EncodingFormat encodingMode)
            : this(symbolId, barcodeMessage, compositeMessage, compositeMode, encodingMode, false)
        { }

        // EAN14, SSCC18, DPD Code and USU S10.
        public Code128Encoder(Symbology symbolId, char[] barcodeMessage)
            : this(symbolId, barcodeMessage, null, CompositeMode.CCA, EncodingFormat.Standard, false)
        { }


        private Code128Encoder(Symbology symbolId, char[] barcodeMessage, char[] compositeMessage, CompositeMode compositeMode, EncodingFormat encodingMode, bool abModesOnly)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.compositeMessage = compositeMessage;
            this.compositeMode = compositeMode;
            this.encodingMode = encodingMode;
            this.abModesOnly = abModesOnly;
            isCompositeSymbol = compositeMessage != null;
            isGS1 = false;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (symbolId)
            {
                case Symbology.Code128:
                    switch (encodingMode)
                    {
                        case EncodingFormat.Standard:
                            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                            Code128();
                            break;

                        case EncodingFormat.GS1:
                            isGS1 = true;
                            barcodeData = MessagePreProcessor.GS1Parser(barcodeMessage);
                            GS1_128();
                            break;

                        case EncodingFormat.HIBC:
                            barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                            Code128();
                            break;
                    }

                    break;

                case Symbology.EAN14:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    EAN14();
                    break;

                case Symbology.SSCC18:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    SSCC18();
                    break;

                case Symbology.DPDCode:
                    barcodeData = barcodeMessage;
                    DPDCode();
                    break;

                case Symbology.UPUS10Code:
                    barcodeData = barcodeMessage;
                    UPUS10();
                    break;
            }

            // Add the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Build the symbol separator and add the 2D component (GS1-128)
            if (isGS1 && isCompositeSymbol)
            {
                byte[] rowData = new byte[Symbol[0].GetRowData().Length];
                for (int i = 0; i < rowData.Length; i++)
                {
                    if (Symbol[0].GetRowData()[i] == 0)
                    {
                        rowData[i] = 1;
                    }
                }

                // Insert the separator to the symbol (top down ).
                SymbolData symbolData = new SymbolData(rowData, 1.0f);
                Symbol.Insert(0, symbolData);
                CompositeEncoder.AddComposite(symbolId, compositeMessage, Symbol, compositeMode, rowData.Length);
            }

            return Symbol;
        }

        /// <summary>
        /// SSCC 18 (NVE 18)
        /// </summary>
        private void SSCC18()
        {
            int inputLength = barcodeData.Length;
            if (inputLength > 17)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "SSC18: Input data too long.\nMaximum length is {0} characters.", 17));
            }

            if (inputLength < 17)
            {
                string zeros = new string('0', 17 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                inputLength = barcodeData.Length;
            }

            checkDigitText += GetCheckDigit.Mod10CheckDigit(barcodeData);
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigitText);
            barcodeData = ArrayHelper.Insert(barcodeData, 0, "00");
            GS1_128();

            // Set the human readable text.
            barcodeText = "(00)" + barcodeMessage + checkDigitText;
        }

        /// <summary>
        /// EAN 14.
        /// </summary>
        private void EAN14()
        {
            int inputLength = barcodeData.Length;
            if (inputLength > 13)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "EAN 14: Input data too long.\nMaximum length is {0} characters.", 13));
            }

            if (inputLength < 13)
            {
                string zeros = new String('0', 13 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                inputLength = barcodeData.Length;
            }

            checkDigitText += GetCheckDigit.Mod10CheckDigit(barcodeData);
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigitText);
            barcodeData = ArrayHelper.Insert(barcodeData, 0, "01");
            GS1_128();

            // Set the human readable text.
            barcodeText = "(01)" + barcodeMessage + checkDigitText;
        }

        /// <summary>
        /// DPD Code.
        /// </summary>
        private void DPDCode()
        {
            char identifier;
            const int mod = 36;
            int inputLength = barcodeData.Length;

            if (inputLength != 28)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "DPD Code: Input data wrong length.\n{0} characters required.", 28));
            }

            identifier = barcodeData[0];
            barcodeData[0] = 'A';
            barcodeData = ArrayHelper.ToUpper(barcodeData);
            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.KRSET.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "PDP Code: Alpha-Numeric data expected.\nInvalid character '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            // Check for valid identifier.
            if ((identifier < 32) || (identifier > 127))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "PDP Code: Invalid identifier.\nInvalid character '{0}' at position 1.", barcodeData[0]));
            }

            barcodeData[0] = identifier;

            // The last 16 characters are expected to be numeric.
            // Tracking Number(last 10) + Service Code(3) + CountryCode(3)
            for (int i = 12; i < inputLength; i++)
            {
                if (!char.IsDigit(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "PDP Code: Numeric only data expected.\nLast 10 characters of the Tracking Number," +
                        " Service Code and Country Code.\nInvalid character '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            // Check for valid country code. ISO 3166 Numeric.
            int countryCode = int.Parse(new string(barcodeData, 25, 3));
            if (!ISO3166.ISO3166Numeric(countryCode))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "PDP Code: Invalid Country Code '{0}{1}{2}'.", barcodeData[25], barcodeData[26], barcodeData[27]));
            }

            
            Code128();

            // Build the barcodes hr text.
            int checkDigit = mod;
            for (int i = 1; i < inputLength; i++)
            {
                barcodeText += barcodeData[i];
                checkDigit += CharacterSets.KRSET.IndexOf(barcodeData[i]);
                if (checkDigit > mod)
                {
                    checkDigit -= mod;
                }

                checkDigit *= 2;

                if (checkDigit >= (mod + 1))
                {
                    checkDigit -= mod + 1;
                }

                // Insert some spaces in the displayed text.
                switch (i)
                {
                    case 4:
                    case 7:
                    case 11:
                    case 15:
                    case 19:
                    case 21:
                    case 24:
                    case 27:
                        barcodeText += ' ';
                        break;
                }
            }

            checkDigit = mod + 1 - checkDigit;
            if (checkDigit == mod)
            {
                checkDigit = 0;
            }

            if (checkDigit < 10)
            {
                barcodeText += (char)(checkDigit + '0');
            }

            else
            {
                barcodeText += (char)(checkDigit - 10 + 'A');
            }
        }

        /// <summary>
        /// UPU S10 Code.
        /// </summary>
        private void UPUS10()
        {
            int[] weights = new int[8] { 8, 6, 4, 2, 3, 5, 9, 7 };
            char inputCheckDigit = '\0';
            int checkDigit;
            char outputCheckDigit;
            int inputLength = barcodeData.Length;

            if (inputLength != 12 && inputLength != 13)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "UPU S10 Code: Input data wrong length.\n12 or 13 characters required."));
            }

            if (inputLength == 13)
            {
                // Includes check digit - remove it.
                inputCheckDigit = barcodeData[10];
                barcodeData = ArrayHelper.Remove(barcodeData, 10);
                inputLength--;
            }

            for (int i = 0; i < inputLength; i++)
            {
                // Make sure all characters are uppercase.
                barcodeData[i] = char.ToUpper(barcodeData[i], CultureInfo.CurrentCulture);
            }

            // 2 Alpha character Service Code. 1 ... 2
            for (int i = 0; i < 2; i++)
            {
                if (!char.IsLetter(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "UPU S10 Code: Alpha only data expected in Service Indicator.\nInvalid character '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            // 8 digit Serial Number. 3 ... 10
            string numericPart = new string(barcodeMessage, 2, 8);
            for (int i = 0; i < numericPart.Length; i++)
            {
                if (!char.IsDigit(numericPart[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "UPU S10 Code: Numeric only data expected for Serial Number. \nInvalid character '{0}' at position {1}.", numericPart[i], i + 2));
                }
            }

            // 2 apha character Country Code.  11 ... 13
            for (int i = 10; i < 2; i++)
            {
                if (!char.IsLetter(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "UPU S10 Code: Alpha only data expected in Country Code.\nInvalid character '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            checkDigit = 0;
            for (int i = 2; i < 10; i++)
            {
                // Check digit calculated on the serial number only.
                checkDigit += barcodeData[i] * weights[i - 2];
            }

            checkDigit %= 11;
            checkDigit = 11 - checkDigit;

            if (checkDigit == 10)
            {
                checkDigit = 0;
            }

            else if (checkDigit == 11)
            {
                checkDigit = 5;
            }

            outputCheckDigit = (char)(checkDigit + '0');

            if (inputCheckDigit != '\0')
            {
                if (inputCheckDigit != outputCheckDigit)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "UPU S10 Code: Invalid check digit '{0}' in input data, expected '{1}'.", inputCheckDigit, outputCheckDigit));
                }
            }

            // Add the check digit.
            barcodeData = ArrayHelper.Insert(barcodeData, 10, outputCheckDigit);

            // Check for values reserved by the specification.
            string serviceIndictor = "JKSTW";
            if(serviceIndictor.IndexOf(barcodeData[0]) >= 0)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "UPU S10 Code: Use of reserved Service Indicator '{0}{1}'.", barcodeData[0], barcodeData[1]));
            }

            // Check for values unassigned by the specification.
            serviceIndictor = "FIOXY";
            if (serviceIndictor.IndexOf(barcodeData[0]) >= 0)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                             "UPU S10 Code: Use of unassigned Service Indicator '{0}{1}'.", barcodeData[0], barcodeData[1]));
            }
 
            // Check for valid country code. ISO 3166 Alpha 2
            char[] countryCode = new char[2];
            Array.Copy(barcodeData, 11, countryCode, 0, 2);
            if (!ISO3166.ISO3166Alpha2(countryCode))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "UPU S10 Code: Invalid Country Code '{0}{1}'.", countryCode[0], countryCode[1]));
            }

            Code128();

            // Set the human readable text.
            for (int i = 0; i < barcodeData.Length; i++)
            {
                barcodeText += barcodeData[i];
                // Insert spaces at these points.
                if (i == 1 || i == 4 || i == 7 || i == 10)
                {
                    barcodeText += ' ';
                }
            }
        }

        /// <summary>
        /// Code 128.
        /// </summary>
        private void Code128()
        {
            byte[] manuals = new byte[C128_MAX]; /* Dummy */
            byte[] fncs = new byte[C128_MAX];  /* Manual FNC1 positions */
            bool haveFNC1 = false; /* Whether have at least 1 manual FNC1 */
            bool haveA = false, haveB = false, haveC = false, haveExtended = false;
            byte[] priority = new byte[C128_STATES];
            int[] values = new int[C128_MAX + 2];
            int glyphCount;
            int finalCharSet = 0;
            int startIndex = 0;

            int inputLength = barcodeData.Length;
            if (inputLength > C128_MAX)
            {
                // This only blocks ridiculously long input - the actual length of the
                // resulting barcode depends on the type of data, so this is trapped later.
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code GS1 128: Input data too long.\nMaximum length is {0} characters.", C128_MAX));
            }

            // Classify data to detect which Code Set states are needed.
            if (abModesOnly)
            {
                for (int i = 0; i < inputLength; i++)
                {
                    char ch = barcodeData[i];
                    char mask_0x60 = (char)(ch & 0x60); /* 0 for (ch & 0x7F) < 32, 0x60 for (ch & 0x7F) >= 96 */
                    haveExtended |= (ch & 0x80) > 0;
                    haveA |= mask_0x60 == 0;
                    haveB |= mask_0x60 == 0x60;
                }
            }

            else
            {
                char prev_digit;
                char digit = '0';
                for (int i = 0; i < inputLength; i++)
                {
                    char ch = barcodeData[i];
                    bool is_fnc1 = ch == '\x1D' && fncs[i] == 1;
                    if (!is_fnc1)
                    {
                        char mask_0x60 = (char)(ch & 0x60); /* 0 for (ch & 0x7F) < 32, 0x60 for (ch & 0x7F) >= 96 */
                        int manual = manuals[i];
                        haveExtended |= (ch & 0x80) > 1;
                        haveA |= mask_0x60 == 0 || manual == C128_A0;
                        haveB |= mask_0x60 == 0x60 || manual == C128_B0;
                        prev_digit = digit;
                        digit = ch;
                        haveC |= char.IsDigit(prev_digit) && char.IsDigit(digit);
                    }
                }
            }

            C128SetPriority(priority, haveA, haveB, haveC, haveExtended);
            glyphCount = C128SetValues(startIndex, priority, fncs, manuals, values, ref finalCharSet);

            // Now we know how long the barcode is - stop it from being too long.
            if (glyphCount > C128_SYMBOL_MAX)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                     "Code 128: Input data too long.\nMaximum length is {0} characters.", C128_SYMBOL_MAX));
            }

            Code128Expand(values, glyphCount);

            // Set the human readable text.
            if (symbolId == Symbology.Code128)
            {
                if (haveFNC1)
                {
                    // Remove any manual FNC1 dummies ('\x1D').
                    for (int i = 0; i < inputLength; i++)
                    {
                        if (fncs[i] == 1)
                        {
                            barcodeData = ArrayHelper.Remove(barcodeData, i);
                        }
                    }
                }

                barcodeText = new string(barcodeData);
                if(encodingMode == EncodingFormat.HIBC)
                {
                    barcodeText = "*" + barcodeText + "*";
                }
            }

            //error_number = hrt_cpy_iso8859_1(symbol, src, length);
        }

        /// <summary>
        /// GS1 128
        /// </summary>
        private void GS1_128()
        {
            byte[] manuals = new byte[C128_MAX];    // Dummy.
            byte[] fncs = new byte[C128_MAX];       // Dummy, set to all 1s.
            byte[] priority = new byte[C128_STATES];
            int[] values = new int[C128_MAX + 2];
            int glyphCount;
            int finalCharSet = 0;

            int inputLength = barcodeData.Length;
            if (inputLength > C128_MAX)
            {
                // This only blocks rediculously long input - the actual length of the
                // resulting barcode depends on the type of data, so this is trapped later.
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code GS1 128: Input data too long.\nMaximum length is {0} characters.", C128_MAX));
            }

            for (int i = 0; i < inputLength; i++)
            {
                fncs[i] = 1;
            }

            // Control and extended chars not allowed so only have B/C (+FNC1).
            C128SetPriority(priority, false, true, true, false);

            glyphCount = C128SetValues(1, priority, fncs, manuals, values, ref finalCharSet);

            if (glyphCount > C128_SYMBOL_MAX)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code GS1 128: Input data too long.\nMaximum length is {0} characters.", C128_SYMBOL_MAX));
            }

            if (isCompositeSymbol)
            {
                switch (compositeMode)
                {
                    case CompositeMode.CCA:
                    case CompositeMode.CCB:
                        // CC-A or CC-B 2D component.
                        switch (finalCharSet)
                        {
                            case C128_B0:
                                values[glyphCount++] = 99;
                                break;

                            case C128_C0:
                                values[glyphCount++] = 101;
                                break;
                        }
                        break;

                    case CompositeMode.CCC:
                        // CC-C 2D component.
                        switch (finalCharSet)
                        {
                            case C128_B0:
                                values[glyphCount++] = 101;
                                break;

                            case C128_C0:
                                values[glyphCount++] = 100;
                                break;
                        }
                        break;
                }
            }

            Code128Expand(values, glyphCount);

            // Set the human readable text.
            if (symbolId == Symbology.Code128)
            {
                barcodeText = new string(barcodeMessage);
                barcodeText = barcodeText.Replace('[', '(');
                barcodeText = barcodeText.Replace(']', ')');
            }
        }

        private void C128SetPriority(byte[] priority, bool haveA, bool haveB, bool haveC, bool haveExtended)
        {
            int i = 0;
            if (haveC)
            {
                priority[i++] = C128_C0;
            }

            if (haveB || !haveA)
            {
                priority[i++] = C128_B0;
            }

            if (haveA)
            {
                priority[i++] = C128_A0;
            }

            if (haveExtended)
            {
                if (haveC)
                {
                    priority[i++] = C128_C1;
                }

                if (haveB || !haveA)
                {
                    priority[i++] = C128_B1;
                }

                if (haveA)
                {
                    priority[i++] = C128_A1;
                }
            }

            priority[i] = 0;
        }

        private int C128SetValues(int startIndex, byte[] priority, byte[] fncs, byte[] manuals, int[] values, ref int finalCharSet)
        {
            int inputLength = barcodeData.Length;
            short[,] costs = new short[inputLength, C128_STATES];
            byte[,] modes = new byte[inputLength, C128_STATES];
            int glyphCount = 0;
            int charSet = 0;

            C128Cost(0, 0, startIndex, priority, fncs, manuals, ref costs, ref modes);
            if (costs[0, 0] > C128_SYMBOL_MAX)
            {
                // Total minimal cost (glyph count).
                return costs[0, 0];
            }

            // Output codewords into `values`.
            for (int i = 0; i < inputLength; i++)
            {
                char ch = barcodeData[i];
                bool isFNC1 = ch == (char)0x1d && (fncs[i] == 1);
                int mode = modes[i, charSet];
                int prevCharSet = charSet;

                charSet = mode & 0x0f;
                if (charSet != prevCharSet)
                {
                    if (prevCharSet == 0)
                    {
                        for (int j = 0; j < c128StartLatchLength[startIndex][charSet]; j++)
                        {
                            values[glyphCount++] = c128StartLatchSequence[startIndex][charSet][j];
                        }
                    }

                    else
                    {
                        for (int j = 0; j < c128LatchLength[prevCharSet][charSet]; j++)
                        {
                            values[glyphCount++] = c128LatchSequence[prevCharSet][charSet][j];
                        }
                    }
                }

                if (mode >= 0x30)
                {
                    // Extended Shift A/B.
                    values[glyphCount++] = 100 + ((charSet) & 1); // FNC4.
                    values[glyphCount++] = 98; // SHIFT.
                }

                else if (mode >= 0x20)
                {
                    // Extended A/B.
                    values[glyphCount++] = 100 + ((charSet) & 1); // FNC4.
                }

                else if (mode >= 0x10)
                {
                    // Shift A/B.
                    values[glyphCount++] = 98; // SHIFT.
                }

                if (isFNC1)
                {
                    values[glyphCount++] = 102; // FNC1.
                }

                else if (charSet >= C128_C0)
                {
                    values[glyphCount++] = ((ch - '0') * 10) + barcodeData[++i] - '0';
                }

                else
                {
                    // (ch & 0x7F) < 32 ? (ch & 0x7F) + 64 : (ch & 0x7F) - 32.
                    values[glyphCount++] = (ch & 0x7F) + (96 * ((ch & 0x60) > 0 ? 0 : 1)) - 32;
                }
            }

            finalCharSet = charSet;
            return glyphCount;
        }

        private int C128Cost(int index, int prevCharSet, int startIndex, byte[] priority, byte[] fncs, byte[] manuals, ref short[,] costs, ref byte[,] modes)
        {
            int inputLength = barcodeData.Length;
            byte[] latchLength = prevCharSet == 0 ? c128StartLatchLength[startIndex] : c128LatchLength[prevCharSet];
            char ch = barcodeData[index];
            bool isFNC1 = ch == 0x1d && fncs[index] == 1;
            int minCost = 999999; // Max possible cost less than 2 * 256.
            int minMode = 0;
            bool canC = isFNC1 || (char.IsDigit(ch) && index + 1 < inputLength && char.IsDigit(barcodeData[index + 1]));
            bool manualCfail = !canC && manuals[index] == C128_C0; // C is requested but not doable.

            for (int p = 0; priority[p] > 0; p++)
            {
                int charSet = priority[p];
                if (charSet >= C128_C0)
                {
                    if (canC && (manuals[index] == 0 || manuals[index] == C128_C0))
                    {
                        int increment = 2 - (isFNC1 ? 1 : 0);
                        int mode = prevCharSet;
                        int cost = 1;
                        if (prevCharSet != charSet)
                        {
                            cost += latchLength[charSet];
                            mode = charSet;
                        }

                        if (index + increment < inputLength)
                        {
                            // Check if memoized.
                            if (costs[index + increment, charSet] > 0)
                            {
                                cost += costs[index + increment, charSet];
                            }

                            else
                            {
                                cost += C128Cost(index + increment, charSet, 0, priority, fncs, manuals, ref costs, ref modes);
                            }
                        }

                        if (cost < minCost)
                        {
                            minCost = cost;
                            minMode = mode;
                        }
                    }
                }

                else
                {
                    if (manuals[index] == 0 || manuals[index] == ((charSet) >> ((charSet > C128_B0) ? 1 : 0)) || manualCfail)
                    {
                        int mode = charSet;
                        int cost = isFNC1 ? 1 : C128CostAB(charSet, ch, ref mode);
                        if (prevCharSet != charSet)
                        {
                            cost += latchLength[charSet];
                        }

                        if (index + 1 < inputLength)
                        {
                            // Check if memoized.
                            if (costs[index + 1, charSet] > 0)
                            {
                                cost += costs[index + 1, charSet];
                            }
                            else
                            {
                                cost += C128Cost(index + 1, charSet, 0, priority, fncs, manuals, ref costs, ref modes);
                            }
                        }

                        if (cost < minCost)
                        {
                            minCost = cost;
                            minMode = mode;
                        }
                    }
                }
            }

            costs[index, prevCharSet] = (short)minCost;
            modes[index, prevCharSet] = (byte)minMode;

            return minCost;
        }

        // Output cost (length) for Code Sets A/B.
        private int C128CostAB(int charSet, char ch, ref int mode)
        {
            byte mask60 = (byte)(ch & 0x60); // 0 for (ch & 0x7F) < 32, 0x60 for (ch & 0x7F) >= 96.
            int ga = (charSet) & 1;
            int cost = 1;

            // SHIFT.
            if ((ga > 0 && mask60 == 0x60) || (ga == 0 && mask60 == 0))
            {
                // A and (ch & 0x7F) >= 96, or B and (ch & 0x7F) < 32.
                cost++;
                mode |= 0x10;
            }

            // FNC4.
            if (((charSet) <= C128_B0) == !(ch < 128))
            {
                // If A0/B0 and extended ASCII, or A1/B1 and ASCII.
                cost++;
                mode |= 0x20;
            }

            return cost;
        }

        private void Code128Expand(int[] values, int count)
        {
            rowPattern = new StringBuilder();
            int totalSum = values[0];

            // Expand the value into code patterns.
            for (int i = 0; i < count; i++)
            {
                rowPattern.Append(Code128Table[values[i]]);
            }

            // Check digit calculation.
            for (int i = 1; i < count; i++)
            {
                totalSum += values[i] * i;
            }

            rowPattern.Append(Code128Table[totalSum % 103]);

            // Stop character.
            rowPattern.Append(Code128Table[106]);
        }
    }
}
