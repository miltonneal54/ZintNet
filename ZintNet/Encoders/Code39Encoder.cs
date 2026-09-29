/* Code39Encoder.cs - Handles Code 39 Based 1D symbols */

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Code 39 based symbol encoder.
    /// </summary>
    internal class Code39Encoder : SymbolEncoder
    {
        #region Tables

        private readonly string[] Code39Table = {
            "1112212111", "2112111121", "1122111121", "2122111111", "1112211121",
            "2112211111", "1122211111", "1112112121", "2112112111", "1122112111",
            "2111121121", "1121121121", "2121121111", "1111221121", "2111221111",
            "1121221111", "1111122121", "2111122111", "1121122111", "1111222111",
            "2111111221", "1121111221", "2121111211", "1111211221", "2111211211",
            "1121211211", "1111112221", "2111112211", "1121112211", "1111212211",
            "2211111121", "1221111121", "2221111111", "1211211121", "2211211111",
            "1221211111", "1211112121", "2211112111", "1221112111", "1212121111",
            "1212111211", "1211121211", "1112121211", "1211212111"};

        private readonly string[] ExtendedC39Ctrl = {
            // Encoding the full ASCII character set in Code 39 (Table A2).
            "%U", "$A", "$B", "$C", "$D", "$E", "$F", "$G", "$H", "$I", "$J", "$K",
            "$L", "$M", "$N", "$O", "$P", "$Q", "$R", "$S", "$T", "$U", "$V", "$W", "$X", "$Y", "$Z",
            "%A", "%B", "%C", "%D", "%E", " ", "/A", "/B", "/C", "/D", "/E", "/F", "/G", "/H", "/I", "/J",
            "/K", "/L", "-", ".", "/O", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "/Z", "%F",
            "%G", "%H", "%I", "%J", "%V", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "%K", "%L", "%M", "%N", "%O",
            "%W", "+A", "+B", "+C", "+D", "+E", "+F", "+G", "+H", "+I", "+J", "+K", "+L", "+M", "+N", "+O",
            "+P", "+Q", "+R", "+S", "+T", "+U", "+V", "+W", "+X", "+Y", "+Z", "%P", "%Q", "%R", "%S", "%T" };

        #endregion

        private readonly bool optionalCheckDigit;
        private readonly bool showCheckDigit;
        private readonly bool insertVINPrefix;
        private readonly int pnzSize;

        private char checkDigit;

        // Code39, Code39 Extended, LOGMARS
        public Code39Encoder(Symbology symbolId, char[] barcodeMessage, bool optionalCheckDigit, bool showCheckDigit)
            : this(symbolId, barcodeMessage, optionalCheckDigit, showCheckDigit, false, 0, EncodingFormat.Standard)
        { }

        // HIBC39
        public Code39Encoder(Symbology symbolId, char[] barcodeMessage, EncodingFormat encodingMode)
            : this(symbolId, barcodeMessage, false, false, false, 0, encodingMode)
        { }

        // PNZ
        public Code39Encoder(Symbology symbolId, char[] barcodeMessage, PNZLength pnzSize)
            : this(symbolId, barcodeMessage, false, false, false, pnzSize, EncodingFormat.Standard)
        { }

        // Code32
        public Code39Encoder(Symbology symbolId, char[] barcodeMessage)
            : this(symbolId, barcodeMessage, false, false, false, 0, EncodingFormat.Standard)
        { }

        // Vehicle Identification Number. (VIN)
        public Code39Encoder(Symbology symbolId, char[] barcodeMessage, bool insertVINPrefix)
            : this(symbolId, barcodeMessage, false, false, insertVINPrefix, 0, EncodingFormat.Standard)
        { }
        private Code39Encoder(Symbology symbolId, char[] barcodeMessage, bool optionalCheckDigit, bool showCheckDigit, bool insertVINPrefix, PNZLength pnzSize, EncodingFormat encodingMode)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionalCheckDigit = optionalCheckDigit;
            this.showCheckDigit = showCheckDigit;
            this.insertVINPrefix = insertVINPrefix;
            this.pnzSize = (int)pnzSize;
            this.encodingMode = encodingMode;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (symbolId)
            {
                case Symbology.Code32:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    Code32();
                    break;

                case Symbology.PharmaZentralNummer:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    PNZ();
                    break;

                case Symbology.LOGMARS:
                    barcodeData = barcodeMessage;
                    Logmars();
                    break;

                case Symbology.VINCode:
                    barcodeData = barcodeMessage;
                    VINCode();
                    break;

                case Symbology.Code39Extended:
                    barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
                    Code39Extended();
                    break;

                case Symbology.Code39:
                    if (encodingMode == EncodingFormat.HIBC)
                    {
                        // HIBC length check and check digit done in parser.
                        barcodeData = MessagePreProcessor.HIBCParser(barcodeMessage);
                        Code39();
                    }

                    else
                    {
                        barcodeData = barcodeMessage;
                        Code39();
                    }

                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// Code 32 (Italian PhamaCode)
        /// </summary>
        private void Code32()
        {
            char[] sourceData;
            char[] resultData = new char[6];
            int[] codeWord = new int[6];
            int maxLength = 8;
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 32: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < 8)
            {
                string zeros = new string('0', 8 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                inputLength = barcodeData.Length;
            }

            sourceData = new char[inputLength];
            Array.Copy(barcodeData, sourceData, inputLength);

            // Calculate the check digit.
            int checkSum = 0;
            for (int i = 0; i < 4; i++)
            {
                int checkPart = (int)barcodeData[i * 2] - '0';
                checkSum += checkPart;
                checkPart = 2 * ((int)(barcodeData[(i * 2) + 1] - '0'));
                if (checkPart >= 10)
                {
                    checkSum += (checkPart - 10) + 1;
                }

                else
                {
                    checkSum += checkPart;
                }
            }

            checkDigit = (char)((checkSum % 10) + '0');
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigit);

            // Convert from decimal to base-32.
            uint pharmacode = uint.Parse(new string(barcodeData), CultureInfo.CurrentCulture);
            uint devisor = 33554432;
            uint remainder;
            for (int i = 5; i >= 0; i--)
            {
                codeWord[i] = (int)(pharmacode / devisor);
                remainder = pharmacode % devisor;
                pharmacode = remainder;
                devisor /= 32;
            }

            for (int i = 5; i >= 0; i--)
            {
                resultData[5 - i] = CharacterSets.Code32Set[codeWord[i]];
            }

            // Generate the barcode with Code 39 using the resultant data.
            Array.Copy(resultData, barcodeData, resultData.Length);
            Array.Resize(ref barcodeData, resultData.Length);
            Code39();

            // Set the human readable text.
            barcodeText = "A" + new string(sourceData) + checkDigit;
        }

        /// <summary>
        /// Pharmazentral Nummer (PZN).
        /// </summary>
        private void PNZ()
        {
            int inputLength = barcodeData.Length;
            char inputCheckDigit = '\0';
            int pnz7 = pnzSize == 8 ? 0 : 1;

            if (inputLength > pnzSize)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "PNZ: Input data too long.\nMaximum length is {0} characters.", pnzSize));
            }

            if (inputLength == 8 - pnz7)
            {
                // Extract the supplied check character.
                inputCheckDigit = barcodeData[7 - pnz7];
                barcodeData = ArrayHelper.Remove(barcodeData, inputLength - 1);
            }

            barcodeData = ArrayHelper.Insert(barcodeData, 0, '-');
            inputLength = barcodeData.Length;

            if (inputLength < pnzSize)
            {
                string zeros = new string('0', pnzSize - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 1, zeros);
                inputLength = barcodeData.Length;
            }

            int count = 0;
            for (int i = 1; i < pnzSize; i++)
            {
                count += (i + pnz7) * (int)(barcodeData[i] - '0');
            }

            int checkValue = count % 11;
            checkDigit = (char)(checkValue + '0');
            if (checkDigit == 'A')
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "PNZ: Invalid PNZ, check digit = {0}", checkValue));
            }

            if (inputCheckDigit != '\0' && checkDigit != inputCheckDigit)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "PNZ: Invalid check digit '{0}' in input data, expected '{1}'.", inputCheckDigit, checkDigit));
            }

            // Add the check character.
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigit);
            inputLength = barcodeData.Length;
            Code39();

            // Set the human readable text.
            barcodeText = "PNZ - " + new string(barcodeData, 1, inputLength - 1);
        }

        /// <summary>
        /// LOGMARS
        /// </summary>
        private void Logmars()
        {
            int maxLength = 30;
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "LOGMARS: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            Code39();

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            if (optionalCheckDigit && showCheckDigit)
            {
                checkDigitText = checkDigit.ToString();
                barcodeText += checkDigitText;
            }
        }

        /// <summary>
        /// Vehicle Identification Number (VIN).
        /// </summary>
        private void VINCode()
        {
            int fixedLength = 17;
            char inputCheckDigit;
            char outputCheckDigit;
            int[] weight = new int[] { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };
            int inputLength = barcodeData.Length;
            int prefixOffset = 0;

            // Check length.
            if (inputLength != fixedLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "VIN Code: Input data wrong length.\n{0} characters required.", fixedLength));
            }

            // Check input characters, I, O and Q are not allowed.
            barcodeData = ArrayHelper.ToUpper(barcodeData);
            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.VINSet.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "VIN Code: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            // This code verifies the check digit present in North American VIN codes.
            if (barcodeData[0] >= '1' && barcodeData[0] <= '5')
            {
                inputCheckDigit = barcodeData[8];
                int sum = 0;
                int value;
                for (int i = 0; i < 17; i++)
                {
                    if (char.IsDigit(barcodeData[i]))
                    {
                        value = barcodeData[i] - '0';
                    }

                    else if (barcodeData[i] <= 'H')
                    {
                        value = (barcodeData[i] - 'A') + 1;
                    }

                    else if (barcodeData[i] <= 'R')
                    {
                        value = (barcodeData[i] - 'J') + 1;
                    }

                    else
                    {
                        value = (barcodeData[i] - 'S') + 2;
                    }

                    sum += value * weight[i];
                }

                outputCheckDigit = (char)('0' + (sum % 11));

                if (outputCheckDigit == ':')    // Check digit was 10
                {
                    outputCheckDigit = 'X';
                }

                if (inputCheckDigit != outputCheckDigit)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "VIN Code: Invalid check digit in input data.\nExpected '{0}' at position 8.", outputCheckDigit));
                }
            }

            if (insertVINPrefix)
            {
                // Import character 'I' prefix?
                barcodeData = ArrayHelper.Insert(barcodeData, 0, 'I');
                prefixOffset = 1;
            }

            Code39();

            // Set the human readable text.
            barcodeText = new string(barcodeData, prefixOffset, 17);
        }

        /// <summary>
        /// Extended Code 39.
        /// </summary>
        private void Code39Extended()
        {
            int maxLength = 86;
            int inputLength = barcodeData.Length;
            char[] sourceData = new char[inputLength];

            // Keep a copy of the original data.
            Array.Copy(barcodeData, sourceData, inputLength);

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 39 Extended: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] > 127)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Code 39 Extended: Invalid character in input data.\nCharacter '{0}' at Position {1}.", barcodeData[i], i));
                }
            }

            List<char> extendedData = new List<char>();
            for (int i = 0; i < inputLength; i++)
            {
                string extendedString = ExtendedC39Ctrl[barcodeData[i]];
                for (int l = 0; l < extendedString.Length; l++)
                {
                    extendedData.Add(extendedString[l]);
                }
            }

            barcodeData = extendedData.ToArray();
            Code39();

            // Set the human readable text.
            for (int i = 0; i < inputLength; i++)
            {
                // Subsitute unprintable characters with a space.
                barcodeText += sourceData[i] >= ' ' && sourceData[i] != 0x7f ? sourceData[i] : ' ';
            }

            if (optionalCheckDigit && showCheckDigit)
            {
                checkDigitText = checkDigit.ToString();
                barcodeText += checkDigitText;
            }
        }

        /// <summary>
        /// Code 39.
        /// </summary>
        private void Code39()
        {
            int maxLength = 86;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 39: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.Code39Set.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Code 39: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            rowPattern.Append(Code39Table[43]);

            int index;
            for (int i = 0; i < inputLength; i++)
            {
                index = CharacterSets.Code39Set.IndexOf(barcodeData[i]);
                rowPattern.Append(Code39Table[index]);
            }

            if (optionalCheckDigit)
            {
                checkDigit = GetCheckDigit.Mod43CheckDigit(barcodeData);
                index = CharacterSets.Code39Set.IndexOf(checkDigit);
                rowPattern.Append(Code39Table[index]);
            }

            rowPattern.Append(Code39Table[43], 0, 9);
            if (symbolId == Symbology.LOGMARS || encodingMode == EncodingFormat.HIBC)
            {
                rowPattern.Replace('2', '3');
            }

            // Expand the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            if (symbolId == Symbology.Code39)
            {
                barcodeText = "*" + new string(barcodeData);
                if (optionalCheckDigit && showCheckDigit)
                {
                    // Display the check digit as an underscore for visability.
                    if (checkDigit == ' ')
                    {
                        checkDigit = '_';
                    }

                    checkDigitText = checkDigit.ToString();
                    barcodeText += checkDigitText;
                }

                barcodeText += "*";
            }
        }
    }
}