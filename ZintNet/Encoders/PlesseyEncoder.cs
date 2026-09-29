/* PlesseyEncoder.cs - Handles UK Plessey and MSI Plessey 1D symbols */

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

using System.Globalization;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Plessey based symbol encoder.
    /// </summary>
    internal class PlesseyEncoder : SymbolEncoder
    {
        #region Constants

        private const int IBM = 7;
        private const int NCR = 9;

        #endregion

        #region Tables

        private static readonly string[] UKPlesseyTable = {
            "13131313", "31131313", "13311313", "31311313", "13133113", "31133113",
            "13313113", "31313113", "13131331", "31131331", "13311331", "31311331",
            "13133131", "31133131", "13313131", "31313131", "31311331", "431311313"};

        private static readonly string[] MSIPlesseyTable = {
            "12121212","12121221","12122112","12122121","12211212","12211221",
            "12212112","12212121","21121212","21121221","21","121"};

        #endregion

        private readonly bool showCheckDigit;
        private readonly MSICheckDigitType checkDigitType;

        public PlesseyEncoder(Symbology symbolId, char[] barcodeMessage, bool showCheckDigit, MSICheckDigitType checkDigitType)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.showCheckDigit = showCheckDigit;
            this.checkDigitType = checkDigitType;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (symbolId)
            {
                case Symbology.MSIPlessey:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    MSIPlessey();
                    break;

                case Symbology.UKPlessey:
                    barcodeData = barcodeMessage;
                    UKPlessey();
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// MSI Plessey
        /// </summary>
        private void MSIPlessey()
        {
            int maxLength = 92;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "MSI Plessey: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Get the checksum.
            switch (checkDigitType)
            {
                case MSICheckDigitType.None:
                    break;

                case MSICheckDigitType.Mod10:
                    Mod10Checksum();
                    inputLength = barcodeData.Length;
                    break;

                case MSICheckDigitType.Mod10Mod10:
                    Mod10Checksum();
                    Mod10Checksum();
                    inputLength = barcodeData.Length;
                    break;

                case MSICheckDigitType.Mod11IBM:
                    Mod11Checksum(IBM);
                    inputLength = barcodeData.Length;
                    break;

                case MSICheckDigitType.Mod11IBMMod10:
                    Mod11Checksum(IBM);
                    Mod10Checksum();
                    inputLength = barcodeData.Length;
                    break;

                case MSICheckDigitType.Mod11NCR:
                    Mod11Checksum(NCR);
                    inputLength = barcodeData.Length;
                    break;

                case MSICheckDigitType.Mod11NCRMod10:
                    Mod11Checksum(NCR);
                    Mod10Checksum();
                    inputLength = barcodeData.Length;
                    break;
            }

            // Add the start character.
            rowPattern.Append(MSIPlesseyTable[10]);
            for (int i = 0; i < inputLength; i++)
            {
                int value = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                rowPattern.Append(MSIPlesseyTable[value]);
            }

            // Add the stop character.
            rowPattern.Append(MSIPlesseyTable[11]);

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            if (checkDigitType != MSICheckDigitType.None && showCheckDigit)
            {
                barcodeText += checkDigitText;
            }

        }

        /// <summary>
        /// UK Plessey.
        /// </summary>
        private void UKPlessey()
        {
            int maxLength = 67;
            int intputLength = barcodeData.Length;
            byte[] checkBuffer = new byte[(intputLength * 4) + 8];
            byte[] grid = { 1, 1, 1, 1, 0, 1, 0, 0, 1 };
            StringBuilder rowPattern = new StringBuilder();

            if (intputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "UK Plessey: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < intputLength; i++)
            {
                if (CharacterSets.UKPlesseySet.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "UK Plessey: Invalid character in input data.\nCharacter '{0}' at Position {1}.", barcodeData[i], i));
                }
            }

            // Start character.
            rowPattern.Append(UKPlesseyTable[16]);

            // Data.
            for (int i = 0; i < intputLength; i++)
            {
                int value = CharacterSets.UKPlesseySet.IndexOf(barcodeData[i]);
                rowPattern.Append(UKPlesseyTable[value]);
                checkBuffer[4 * i] = (byte)(value & 1);
                checkBuffer[4 * i + 1] = (byte)((value >> 1) & 1);
                checkBuffer[4 * i + 2] = (byte)((value >> 2) & 1);
                checkBuffer[4 * i + 3] = (byte)((value >> 3) & 1);
            }

            // CRC value digit code adapted from code by Leonid A. Broukhis used in GNU Barcode.
            for (int i = 0; i < (4 * intputLength); i++)
            {
                if (checkBuffer[i] != 0)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        checkBuffer[i + j] ^= grid[j];
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                switch (checkBuffer[intputLength * 4 + i])
                {
                    case 0:
                        rowPattern.Append("13");
                        break;

                    case 1:
                        rowPattern.Append("31");
                        break;
                }
            }

            rowPattern.Append(UKPlesseyTable[17]);  // Termination + Stop character.

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
        }
        private void Mod10Checksum()
        {
            int[,] values = new int[2, 10] {
                { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 },   // Doubled and digits summed.
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } }; // Single.

            char checkDigit;
            int weight = 0;
            int inputLength = barcodeData.Length;
            int tableIndex = 0;

            for (int i = inputLength - 1; i >= 0; i--)
            {
                weight += values[tableIndex, barcodeData[i] - '0'];
                tableIndex = tableIndex == 0 ? 1 : 0;
            }

            char cc = GetCheckDigit.Mod10CheckDigit(barcodeData);

            checkDigit = (char)(((10 - (weight % 10)) % 10) + '0');
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigit);
            checkDigitText += checkDigit;
        }

        private void Mod11Checksum(int factor)
        {
            // Factor of 7 = IBM, 9 = NCR.
            int inputLength = barcodeData.Length;
            int weight = 0;
            int weightFactor = 2;

            for (int i = inputLength - 1; i >= 0; i--)
            {
                weight += (weightFactor * (barcodeData[i] - '0'));
                weightFactor++;
                if (weightFactor > factor)
                {
                    weightFactor = 2;
                }
            }

            int checkValue = (11 - (weight % 11)) % 11;
            if (checkValue == 10)
            {
                checkValue = 40; // Possibly invalid, make the check digit a 'X'
            }

            char checkDigit = (char)(checkValue + '0');
            checkDigitText += checkDigit.ToString();
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigit);
        }
    }
}