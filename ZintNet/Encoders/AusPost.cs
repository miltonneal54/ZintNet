/* AusPost.cs - Handles Australia Post 4-State Barcode */

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
using System.Text;

namespace ZintNet.Encoders
{
    internal class AusPostEncoder : SymbolEncoder
    {
        #region Tables

        private static readonly string[] AusNTable = { "00", "01", "02", "10", "11", "12", "20", "21", "22", "30" };

        private static readonly string[] AusCTable = {
            "222", "300", "301", "302", "310", "311", "312", "320", "321", "322", "000", "001", "002",
            "010", "011", "012", "020", "021", "022", "100", "101", "102", "110", "111", "112", "120",
            "121", "122", "200", "201", "202", "210", "211", "212", "220", "221", "023", "030", "031",
            "032", "033", "103", "113", "123", "130", "131", "132", "133", "203", "213", "223", "230",
            "231", "232", "233", "303", "313", "323", "330", "331", "332", "333", "003", "013" };

        private static readonly string[] AusBarTable = {
            "000", "001", "002", "003", "010", "011", "012", "013", "020", "021", "022", "023", "030",
            "031", "032", "033", "100", "101", "102", "103", "110", "111", "112", "113", "120", "121",
            "122", "123", "130", "131", "132", "133", "200", "201", "202", "203", "210", "211", "212",
            "213", "220", "221", "222", "223", "230", "231", "232", "233", "300", "301", "302", "303",
            "310", "311", "312", "313", "320", "321", "322", "323", "330", "331", "332", "333"};

        private const char fillCharacter = '3';

        #endregion

        public AusPostEncoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            AusPost();
            return Symbol;
        }

        /// <summary>
        /// Australian Post encoding modes.
        /// </summary>
        internal enum AusPostEncoding
        {
            /// <summary>
            /// Numeric mode.
            /// </summary>
            Numeric = 0,

            /// <summary>
            /// Alpha numeric mode.
            /// </summary>
            AlphaNumeric,
        }

        private void AusPost()
        {
            /* Handles Australia Posts's 4 State Codes
	           The contents of data pattern conform to the following standard:
	           0 = Tracker, Ascender and Descender
	           1 = Tracker and Ascender
	           2 = Tracker and Descender
	           3 = Tracker only */

            int inputLength = barcodeMessage.Length;
            int minLength = 10;
            StringBuilder barPattern = new StringBuilder();
            AusPostEncoding encodingMode = AusPostEncoding.Numeric;
            string cif = string.Empty;          // Customer Information Field.
            int cifLength = 0;
            string fcc;
            string dpid;
            // Do all of the length and integrity checking first.
            if (inputLength >= minLength)
            {
                // Check valid FCC and DPID.
                for (int i = 0; i < 10; i++)
                {
                    if (CharacterSets.NumberOnlySet.IndexOf(barcodeMessage[i]) == -1)
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AusPost: FCC and DPID must be numeric data.\nInvalid character '{0}' at position {1}.", barcodeMessage[i], i + 1));
                    }
                }

                fcc = new string(barcodeMessage, 0, 2);    // 2 digits.
                dpid = new string(barcodeMessage, 2, 8);   // 8 digits.

                // Check for CIF.
                if (inputLength > 10)
                {
                    cif = new string(barcodeMessage, 10, inputLength - 10);
                    cifLength = cif.Length;
                }
            }

            else
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AusPost: Input data wrong length.\nMinimum length is {0} characters.", minLength));
            }

            if (symbolId == Symbology.AusPostStandard)
            {
                if (fcc != "11" && fcc != "59" && fcc != "62")
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AusPost Standard: Invalid Format Control Code '{0}'.", fcc));
                }

                if (fcc == "11" && inputLength != 10)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AusPost Standard: Input data wrong length for Format Control Code 11.\n{0} numeric characters required.", 10));
                }

                if (fcc == "59")
                {
                    // CFI length of 5-8 characters require numeric encoding.
                    if (cifLength > 5 && cifLength < 9)
                    {
                        for (int i = 0; i < cifLength; i++)
                        {
                            if (CharacterSets.NumberOnlySet.IndexOf(cif[i]) == -1)
                            {
                                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                    "AusPost Standard: Using Numeric encoding.\nInvalid character {0} at position{1}.", barcodeMessage[i + 10], i + 10 + 1));
                            }
                        }

                        encodingMode = AusPostEncoding.Numeric;
                    }

                    // CFI length of < 5 characters use alpha numeric encoding.
                    else if (cifLength > 0 && cifLength < 6)
                    {
                        for (int i = 0; i < cifLength; i++)
                        {
                            if (CharacterSets.AusPostSet.IndexOf(cif[i]) == -1)
                            {
                                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                    "AusPost Standard: Using Character encoding.\nInvalid character {0} at position{1}.", barcodeMessage[i + 10], i + 10 + 1));
                            }
                        }

                        encodingMode = AusPostEncoding.AlphaNumeric;
                    }

                    else
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AusPost Standard: Wrong length for Customer Information Field for FCC {0}.\nExpected between 1 and 8 characters.", fcc));
                    }
                }

                if (fcc == "62")
                {
                    if (cifLength > 10 && cifLength < 16)
                    {
                        for (int i = 0; i < cifLength; i++)
                        {
                            if (CharacterSets.NumberOnlySet.IndexOf(cif[i]) == -1)
                            {
                                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                    "AusPost Standard: Using Numeric encoding.\nInvalid character {0} at position{1}.", barcodeMessage[i + 10], i + 10 + 1));
                            }
                        }

                        encodingMode = AusPostEncoding.Numeric;
                    }

                    else if (cifLength > 0 && cifLength < 11)
                    {
                        for (int i = 0; i < cifLength; i++)
                        {
                            if (CharacterSets.AusPostSet.IndexOf(cif[i]) == -1)
                            {
                                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                   "AusPost Standard: Using Character encoding.\nInvalid character {0} at position{1}.", barcodeMessage[i + 10], i + 10 + 1));
                            }
                        }

                        encodingMode = AusPostEncoding.AlphaNumeric;
                    }

                    else
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AusPost Standard: Wrong length for Customer Information Field for FCC {0}.\nExpected between 1 and 15 characters.", fcc));
                    }
                }
            }

            else
            {
                if (inputLength != 10)
                {
                    throw new InvalidDataLengthException("AusPost: Input is wrong length for selected AusPost symbol.");
                }

                switch (symbolId)
                {
                    case Symbology.AusPostReplyPaid:
                        if (fcc != "45")
                        {
                            throw new InvalidDataException("AusPost Reply Paid: Invalid Format Control Code.");
                        }

                        break;

                    case Symbology.AusPostRouting:
                        if (fcc != "87")
                        {
                            throw new InvalidDataException("AusPost Routing: Invalid Format Control Code.");
                        }

                        break;

                    case Symbology.AusPostRedirect:
                        if (fcc != "92")
                        {
                            throw new InvalidDataException("AusPost Redirect: Invalid Format Control Code.");
                        }

                        break;
                }
            }

            // Encode the Format Control Code.
            for (int i = 0; i < 2; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(fcc[i]);
                barPattern.Append(AusNTable[index]);
            }

            // Delivery Point Identifier.
            for (int i = 0; i < 8; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(dpid[i]);
                barPattern.Append(AusNTable[index]);
            }

            if (cifLength > 0)
            {

                if (encodingMode == AusPostEncoding.AlphaNumeric)
                {
                    for (int i = 0; i < cifLength; i++)
                    {
                        int index = CharacterSets.AusPostSet.IndexOf(cif[i]);
                        barPattern.Append(AusCTable[index]);
                    }
                }

                else
                {
                    for (int i = 0; i < cifLength; i++)
                    {
                        int index = CharacterSets.NumberOnlySet.IndexOf(cif[i]);
                        barPattern.Append(AusNTable[index]);
                    }
                }

            }

            // Filler bars.
            int patternLength = barPattern.Length;
            if (fcc == "59" && patternLength < 36)
            {
                barPattern.Append(fillCharacter, 36 - patternLength);
            }

            else if (fcc == "62" && patternLength < 51)
            {
                barPattern.Append(fillCharacter, 51 - patternLength);
            }

            else
            {
                barPattern.Append(fillCharacter);
            }

            AddErrorCorrection(barPattern);

            // Add start & stop characters.
            barPattern.Insert(0, "13");
            barPattern.Append("13");

            SymbolBuilder.FourStateSymbol(Symbol, barPattern);
            barcodeText = new string(barcodeMessage);
            if (cifLength > 0)
            {
                barcodeText = barcodeText.Insert(10, " ");
            }

            barcodeText = barcodeText.Insert(2, " ");
        }

        private void AddErrorCorrection(StringBuilder barPattern)
        {
            // Adds Reed-Solomon error correction.
            int dataCodewords = 0;
            byte[] eccBlocks = new byte[4];
            byte[] dataBlocks = new byte[31];

            // Build the triples data blocks;
            for (int i = 0; i < barPattern.Length; i += 3, dataCodewords++)
            {
                dataBlocks[dataCodewords] = (byte)(ConvertPattern(barPattern[i], 4)
                    + ConvertPattern(barPattern[i + 1], 2)
                    + ConvertPattern(barPattern[i + 2], 0));
            }

            ReedSolomon.RSInitialise(0x43, 4, 1);
            ReedSolomon.RSEncode(dataCodewords, dataBlocks, eccBlocks);

            // Append error correction in reverse order.
            for (int i = 4; i > 0; i--)
            {
                barPattern.Append(AusBarTable[eccBlocks[i - 1]]);
            }
        }

        private byte ConvertPattern(char data, int shift)
        {
            return (byte)((data - '0') << shift);
        }
    }
}

