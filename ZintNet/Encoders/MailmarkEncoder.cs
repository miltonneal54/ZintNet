/* Mailmark.cs - Handles Royal Mail 4-State Barcode. */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library.
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

using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Royal Mail Mailmark 4 State CL encoder.
    /// </summary>
    internal class MailmarkEncoder : SymbolEncoder
    {
        #region Tables

        private static readonly string[] postcodeFormat = new string[] {
            "ANANLLNLS", "AANNLLNLS", "AANNNLLNL", "AANANLLNL", "ANNLLNLSS", "ANNNLLNLS" };

        // Data/Check Symbols from Table 5
        private static readonly byte[] dataSymbolOdd = new byte[] {
            0x01, 0x02, 0x04, 0x07, 0x08, 0x0B, 0x0D, 0x0E, 0x10, 0x13, 0x15, 0x16,
            0x19, 0x1A, 0x1C, 0x1F, 0x20, 0x23, 0x25, 0x26, 0x29, 0x2A, 0x2C, 0x2F,
            0x31, 0x32, 0x34, 0x37, 0x38, 0x3B, 0x3D, 0x3E };

        private static readonly byte[] dataSymbolEven = new byte[] {
            0x03, 0x05, 0x06, 0x09, 0x0A, 0x0C, 0x0F, 0x11, 0x12, 0x14, 0x17, 0x18,
            0x1B, 0x1D, 0x1E, 0x21, 0x22, 0x24, 0x27, 0x28, 0x2B, 0x2D, 0x2E, 0x30,
            0x33, 0x35, 0x36, 0x39, 0x3A, 0x3C };

        private static readonly byte[] extenderGroupC = new byte[] {
            3, 5, 7, 11, 13, 14, 16, 17, 19, 0, 1, 2, 4, 6, 8, 9, 10, 12, 15, 18, 20, 21 };

        private static readonly byte[] extenderGroupL = new byte[] {
            2, 5, 7, 8, 13, 14, 15, 16, 21, 22, 23, 0, 1, 3, 4, 6, 9, 10, 11, 12, 17, 18, 19, 20, 24, 25 };

        #endregion

        #region Constants

        // Allowed character values from Table 3
        private const string SetA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SetL = "ABDEFGHJLNPQRSTUWXYZ";
        private const string SetN = "0123456789";
        private const string SetS = " ";

        #endregion

        public MailmarkEncoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.barcodeMessage = barcodeMessage;
            this.symbolId = symbolId;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.MailmarkParser(barcodeMessage);
            Mailmark4State();
            return Symbol;
        }

        /// <summary>
        /// Mailmark 4 State CL.
        /// </summary>
        private void Mailmark4State()
        {
            const int maxLength = 26;
            const int minLength = 17;
            const int itemIdLength = 8;
            const int postcodeLength = 9;
            short[] destinationPostcode = new short[112];
            short[] aRegister = new short[112];
            short[] bRegister = new short[112];
            short[] tempRegister = new short[112];
            short[] cdvAccumulator = new short[112];
            bool result;
            byte[] data = new byte[26];
            int dataTop, dataStep;
            byte[] check = new byte[7];
            short[] extender = new short[27];
            int checkCount;
            StringBuilder barPattern = new StringBuilder();
            int inputLength = barcodeData.Length;
           

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "MailMark 4 State: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < minLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                "MailMark 4 State: Input data too short.\nMinimum length is {0} characters.", minLength));
            }

            if (inputLength < 22)
            {
                for (int i = inputLength; i < 22; i++)
                {
                    barcodeData = ArrayHelper.Insert(barcodeData, inputLength, ' ');
                }

                inputLength = barcodeData.Length;
            }

            else if (inputLength > 22 && inputLength < 26)
            {
                for (int i = inputLength; i < 26; i++)
                {
                    barcodeData = ArrayHelper.Insert(barcodeData, inputLength, ' ');
                }

                inputLength = barcodeData.Length;
            }

            char barcodeType = (inputLength > 22) ? 'L' : 'C';
            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.Mailmark4State.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "MailMark 4 State: Invalid character in input data.\nCharacter '{0}' at Position {1}.", barcodeData[i], i));
                }
            }

            // Format is in the range 0-4.
            int pos = 0;
            result = int.TryParse(new string(barcodeData, pos, 1), out int formatId);
            if (!result || formatId < 0 || formatId > 4)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "MailMark 4 State: Invalid Format ID.\nExpected range 0 - 4 at position {0}.", pos + 1));
            }

            pos++;

            // Version ID is in the range 1-4.
            result = int.TryParse(new string(barcodeData, 1, 1), out int versionId);
            versionId--;    // Internal field value of 0-3.
            if (!result || versionId < 0 || versionId > 3)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "MailMark 4 State: Invalid Version ID.\nExpected range 1 - 4 at position {0}.", pos + 1));
            }

            pos++;

            // Class is in the range 0-9 & A-E.
            result = int.TryParse(new string(barcodeData, pos, 1), NumberStyles.HexNumber, null, out int classId);
            if (!result || classId < 0 || classId > 14)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "MailMark 4 State: Invalid Class ID.\nExpected 1-9, A-E at Position {0}.", pos + 1));
            }

            pos++;

            // Supply Chain ID is 2 digits for barcode C and 6 digits for barcode L
            int supplyChainLength = (barcodeType == 'C') ? 2 : 6;
            result = int.TryParse(new string(barcodeData, pos, supplyChainLength), out int supplyChainId);
            if (!result)
            {
                for (int i = 0; i < supplyChainLength; i++)
                {
                    if(!char.IsDigit(barcodeData[i + pos]))
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "MailMark 4 State: Invalid Supply Chain ID.\nExpected numeric value at position {0}.", pos + i + 1));
                    }
                }
            }

            pos += supplyChainLength;

            // Item ID is 8 digits.
            result = int.TryParse(new string(barcodeData, pos, itemIdLength), out int itemId);
            if (!result)
            {
                for (int i = 0; i < itemIdLength; i++)
                {
                    if (!char.IsDigit(barcodeData[i + pos]))
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "MailMark 4 State: Invalid Item ID.\nExpected numeric value at position {0}.", pos + i + 1));
                    }
                }
            }

            pos += itemIdLength;
            // Extract Destination Post Code and DPS field.
            string postcode = new string(barcodeData, pos, postcodeLength);
            int postcodeType = 0;
            if(!VerifyPostcode(postcode, ref postcodeType, false))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "MailMark 4 State: Invalid Postcode '{0}.", postcode));
            }

            // Convert postcode to internal user field.
            if (postcodeType != 7)
            {
                string pattern = postcodeFormat[postcodeType - 1];
                BinaryMath.BinaryLoad(bRegister, "0");
                for (int i = 0; i < postcodeLength; i++)
                {
                    switch (pattern[i])
                    {
                        case 'A':
                            BinaryMath.BinaryMultiply(bRegister, "26");
                            BinaryMath.BinaryLoad(tempRegister, "0");
                            for (int j = 0; j < 5; j++)
                            {
                                if ((SetA.IndexOf(postcode[i]) & (0x01 << j)) > 0)
                                {
                                    tempRegister[j] = 1;
                                }
                            }

                            BinaryMath.BinaryAdd(bRegister, tempRegister);
                            break;

                        case 'L':
                            BinaryMath.BinaryMultiply(bRegister, "20");
                            BinaryMath.BinaryLoad(tempRegister, "0");
                            for (int j = 0; j < 5; j++)
                            {
                                if ((SetL.IndexOf(postcode[i]) & (0x01 << j)) > 0)
                                {
                                    tempRegister[j] = 1;
                                }
                            }

                            BinaryMath.BinaryAdd(bRegister, tempRegister);
                            break;

                        case 'N':
                            BinaryMath.BinaryMultiply(bRegister, "10");
                            BinaryMath.BinaryLoad(tempRegister, "0");
                            for (int j = 0; j < 5; j++)
                            {
                                if ((SetN.IndexOf(postcode[i]) & (0x01 << j)) > 0)
                                {
                                    tempRegister[j] = 1;
                                }
                            }

                            // case 'S' ignored as value is 0.

                            BinaryMath.BinaryAdd(bRegister, tempRegister);
                            break;
                    }
                }

                // Destination postcode = accumulatorA + accumulatorB.
                BinaryMath.BinaryLoad(destinationPostcode, "0");
                BinaryMath.BinaryAdd(destinationPostcode, bRegister);

                BinaryMath.BinaryLoad(aRegister, "1");
                if (postcodeType == 1)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }

                BinaryMath.BinaryLoad(tempRegister, "5408000000");
                BinaryMath.BinaryAdd(aRegister, tempRegister);

                if (postcodeType == 2)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }

                BinaryMath.BinaryLoad(tempRegister, "5408000000");
                BinaryMath.BinaryAdd(aRegister, tempRegister);

                if (postcodeType == 3)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }

                BinaryMath.BinaryLoad(tempRegister, "54080000000");
                BinaryMath.BinaryAdd(aRegister, tempRegister);

                if (postcodeType == 4)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }

                BinaryMath.BinaryLoad(tempRegister, "140608000000");
                BinaryMath.BinaryAdd(aRegister, tempRegister);

                if (postcodeType == 5)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }

                BinaryMath.BinaryLoad(tempRegister, "208000000");
                BinaryMath.BinaryAdd(aRegister, tempRegister);

                if (postcodeType == 6)
                {
                    BinaryMath.BinaryAdd(destinationPostcode, aRegister);
                }
            }

            // Conversion from Internal User Fields to Consolidated Data Value
            // Set CDV to 0
            BinaryMath.BinaryLoad(cdvAccumulator, "0");

            // Add Destination Post Code plus DPS
            BinaryMath.BinaryAdd(cdvAccumulator, destinationPostcode);

            // Multiply by 100,000,000
            BinaryMath.BinaryMultiply(cdvAccumulator, "100000000");

            // Add Item ID
            BinaryMath.BinaryLoad(tempRegister, "0");
            for (int i = 0; i < 26; i++)
            {
                if ((0x01 & (itemId >> i)) > 0)
                {
                    tempRegister[i] = 1;
                }
            }

            BinaryMath.BinaryAdd(cdvAccumulator, tempRegister);

            if (barcodeType == 'C')
            {
                BinaryMath.BinaryMultiply(cdvAccumulator, "100");  // Barcode C - Multiply by 100
            }

            else
            {
                BinaryMath.BinaryMultiply(cdvAccumulator, "1000000");  // Barcode L - Multiply by 1,000,000
            }

            // Add supply chain id.
            BinaryMath.BinaryLoad(tempRegister, "0");
            for (int i = 0; i < 20; i++)
            {
                if ((0x01 & (supplyChainId >> i)) > 0)
                {
                    tempRegister[i] = 1;
                }
            }

            BinaryMath.BinaryAdd(cdvAccumulator, tempRegister);

            // Multiply by 15.
            BinaryMath.BinaryMultiply(cdvAccumulator, "15");

            // Add class.
            BinaryMath.BinaryLoad(tempRegister, "0");
            for (int i = 0; i < 4; i++)
            {
                if ((0x01 & (classId >> i)) > 0)
                {
                    tempRegister[i] = 1;
                }
            }

            BinaryMath.BinaryAdd(cdvAccumulator, tempRegister);

            // Multiply by 5.
            BinaryMath.BinaryMultiply(cdvAccumulator, "5");

            // Add format.
            BinaryMath.BinaryLoad(tempRegister, "0");
            for (int i = 0; i < 4; i++)
            {
                if ((0x01 & (formatId >> i)) > 0)
                {
                    tempRegister[i] = 1;
                }
            }

            BinaryMath.BinaryAdd(cdvAccumulator, tempRegister);

            // Multiply by 4.
            BinaryMath.BinaryMultiply(cdvAccumulator, "4");

            // Add version id.
            BinaryMath.BinaryLoad(tempRegister, "0");
            for (int i = 0; i < 4; i++)
            {
                if ((0x01 & (versionId >> i)) > 0)
                {
                    tempRegister[i] = 1;
                }
            }

            BinaryMath.BinaryAdd(cdvAccumulator, tempRegister);
            if (barcodeType == 'C')
            {
                dataTop = 15;
                dataStep = 8;
                checkCount = 6;
            }

            else
            {
                dataTop = 18;
                dataStep = 10;
                checkCount = 7;
            }

            // Conversion from consolidated data value to data numbers.
            for (int i = 0; i < 112; i++)
            {
                bRegister[i] = cdvAccumulator[i];
            }

            for (int j = dataTop; j >= (dataStep + 1); j--)
            {
                for (int i = 0; i < 112; i++)
                {
                    cdvAccumulator[i] = bRegister[i];
                    bRegister[i] = 0;
                    aRegister[i] = 0;
                }

                aRegister[96] = 1;
                for (int i = 91; i >= 0; i--)
                {
                    bRegister[i] = BinaryMath.IsLarger(cdvAccumulator, aRegister);
                    if (bRegister[i] == 1)
                    {
                        BinaryMath.BinarySubtract(cdvAccumulator, aRegister);
                    }

                    BinaryMath.ShiftDown(aRegister);
                }

                data[j] = (byte)((cdvAccumulator[4] * 16) + (cdvAccumulator[3] * 8) + (cdvAccumulator[2] * 4) + (cdvAccumulator[1] * 2) + cdvAccumulator[0]);
            }

            for (int j = dataStep; j >= 0; j--)
            {
                for (int i = 0; i < 112; i++)
                {
                    cdvAccumulator[i] = bRegister[i];
                    bRegister[i] = 0;
                    aRegister[i] = 0;
                }

                aRegister[95] = 1;
                aRegister[94] = 1;
                aRegister[93] = 1;
                aRegister[92] = 1;
                for (int i = 91; i >= 0; i--)
                {
                    bRegister[i] = BinaryMath.IsLarger(cdvAccumulator, aRegister);
                    if (bRegister[i] == 1)
                    {
                        BinaryMath.BinarySubtract(cdvAccumulator, aRegister);
                    }

                    BinaryMath.ShiftDown(aRegister);
                }

                data[j] = (byte)((cdvAccumulator[4] * 16) + (cdvAccumulator[3] * 8) + (cdvAccumulator[2] * 4) + (cdvAccumulator[1] * 2) + cdvAccumulator[0]);
            }

            ReedSolomon.RSInitialise(0x25, checkCount, 1);
            ReedSolomon.RSEncode(dataTop + 1, data, check);

            // Append check digits to data in reverse order.
            for (int i = 1; i <= checkCount; i++)
            {
                data[dataTop + i] = check[checkCount - i];
            }

            // Conversion from data numbers and check numbers to data symbols and check symbols.
            for (int i = 0; i <= dataStep; i++)
            {
                data[i] = dataSymbolEven[data[i]];
            }

            for (int i = dataStep + 1; i <= (dataTop + checkCount); i++)
            {
                data[i] = dataSymbolOdd[data[i]];
            }

            // Conversion from data symbols and check symbols to extender groups.
            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeType == 'C')
                {
                    extender[extenderGroupC[i]] = data[i];
                }

                else
                {
                    extender[extenderGroupL[i]] = data[i];
                }
            }

            // Conversion from extender groups to bar identifiers.
            for (int i = 0; i < inputLength; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    switch (extender[i] & 0x24)
                    {
                        case 0x24:
                            barPattern.Append("F");
                            break;

                        case 0x20:
                            if (i % 2 > 0)
                            {
                                barPattern.Append("D");
                            }

                            else
                            {
                                barPattern.Append("A");
                            }

                            break;

                        case 0x04:
                            if (i % 2 > 0)
                            {
                                barPattern.Append("A");
                            }

                            else
                            {
                                barPattern.Append("D");
                            }

                            break;

                        default:
                            barPattern.Append("T");
                            break;
                    }

                    extender[i] = (short)(extender[i] << 1);
                }
            }

            SymbolBuilder.FourStateSymbol(Symbol, barPattern);
            barcodeText = new string(barcodeData);
        }

        public static bool VerifyPostcode(string postcode, ref int postcodeType, bool is2D)
        {
            /* postcode_type is used to select which format of postcode.
             *
             * 1 = ANANLLNLS
             * 2 = AANNLLNLS
             * 3 = AANNNLLNL
             * 4 = AANANLLNL
             * 5 = ANNLLNLSS
             * 6 = ANNNLLNLS
             * 7 = International designation
             */

            // Detect postcode type.
            int length = postcode.Length;
            if (length >= 4 && string.Compare(postcode, 0, "XY11     ", 0, length) == 0)
            {
                postcodeType = 7;
            }

            else
            {
                if (length == 2 || (length == 9 && postcode[7] == ' '))
                {
                    postcodeType = 5;
                }

                else
                {
                    if (length == 3 || (length == 9 && postcode[8] == ' '))
                    {
                        // Types 1, 2 and 6.
                        if (char.IsDigit(postcode[1]))
                        {
                            if (char.IsDigit(postcode[2]))
                            {
                                postcodeType = 6;
                            }

                            else
                            {
                                postcodeType = 1;
                            }
                        }

                        else
                        {
                            postcodeType = 2;
                        }
                    }

                    else
                    {
                        if (length >= 4)
                        {
                            // Types 3 and 4.
                            if (char.IsDigit(postcode[3]))
                            {
                                postcodeType = 3;
                            }

                            else
                            {
                                postcodeType = 4;
                            }
                        }
                    }
                }
            }

            // Verify postcode type.
            if (postcodeType != 7)
            {
                return VerifyCharacter(postcode, postcodeType, is2D);
            }

            return true;
        }

        private static bool VerifyCharacter(string postcode, int type,  bool is2D)
        {
            int value = 0;

            char[] pattern = postcodeFormat[type - 1].ToCharArray();
            for (int i = 0; i < postcode.Length; i++)
            {
                switch (pattern[i])
                {
                    case 'A':
                        value = SetA.IndexOf(postcode[i]);
                        break;

                    case 'L':
                        value = is2D ? SetA.IndexOf(postcode[i]) : SetL.IndexOf(postcode[i]);
                        break;

                    case 'N':
                        value = SetN.IndexOf(postcode[i]);
                        break;

                    case 'S':
                        value = SetS.IndexOf(postcode[i]);
                        break;
                }

                if (value == -1)
                {
                    break;
                }
            }

            return value != -1;
        }
    }
}
