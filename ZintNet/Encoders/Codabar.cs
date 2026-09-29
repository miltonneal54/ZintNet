/* CodabarEncoder.cs Handles Codabar 1D symbol */

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
    /// <summary>
    /// Codabar symbol encoder.
    /// </summary>
    internal class CodabarEncoder : SymbolEncoder
    {
        #region Tables.

        private static readonly string[] CodabarTable = {
                "11111221", "11112211", "11121121", "22111111", "11211211", "21111211",
                "12111121", "12112111", "12211111", "21121111", "11122111", "11221111",
                "21112121", "21211121", "21212111", "11212121", "11221211", "12121121",
                "11121221", "11122211"};

        #endregion

        private readonly bool optionalCheckDigit;
        private readonly bool showCheckDigit;

        public CodabarEncoder(Symbology symbolId, char[] barcodeMessage, bool optionalCheckDigit, bool showCheckDigit)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionalCheckDigit = optionalCheckDigit;
            this.showCheckDigit = showCheckDigit;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = barcodeMessage;
            Codabar();
            return Symbol;
        }

        /// <summary>
        /// Codabar.
        /// </summary>
        private void Codabar()
        {
            StringBuilder rowPattern = new StringBuilder();
            int maxLength = 103;
            int minLength = 3;
            int count = 0;
            int checkSum = 0;
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Codabar: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < minLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Codabar: Input data too short.\nMinimum length is {0} characters.", minLength));
            }

            barcodeData = ArrayHelper.ToUpper(barcodeData);

            // Codabar must begin and end with the characters A, B, C or D
            if ((barcodeData[0] != 'A') && (barcodeData[0] != 'B') && (barcodeData[0] != 'C') && (barcodeData[0] != 'D'))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Codabar: Invalid start character.\nMust start with A, B, C or D."));
            }

            int last = inputLength - 1;
            if ((barcodeData[last] != 'A') && (barcodeData[last] != 'B') && (barcodeData[last] != 'C') && (barcodeData[last] != 'D'))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Codabar: Invalid stop character.\nMust end with A, B, C or D."));
            }

            for (int i = 1; i < inputLength - 1; i++)
            {
                if (CharacterSets.CodaBarSet.IndexOf(barcodeData[i]) == -1 || char.IsLetter(barcodeData[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Codabar: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            int idx;
            for (int i = 0; i < inputLength; i++)
            {
                if (optionalCheckDigit)
                {
                    // BS EN 798:1995 A.3 suggests using ISO 7064 algorithm but leaves it application defined.
                    // Following BWIPP and TEC-IT, use this simple mod-16 algorithm (not in ISO 7064)

                    count += CharacterSets.CodaBarSet.IndexOf(barcodeData[i]);
                    if (i + 1 == inputLength)
                    {
                        checkSum = count % 16;
                        if (checkSum > 0)
                        {
                            checkSum = 16 - checkSum;
                        }

                        char checkDigit = CharacterSets.CodaBarSet[checkSum];
                        checkDigitText += checkDigit;
                        barcodeData = ArrayHelper.Insert(barcodeData, inputLength - 1, checkDigit);
                        idx = CharacterSets.CodaBarSet.IndexOf(checkDigit);
                        rowPattern.Append(CodabarTable[idx]);
                        i++;
                    }
                }

                idx = CharacterSets.CodaBarSet.IndexOf(barcodeData[i]);
                rowPattern.Append(CodabarTable[idx]);
            }

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            if (optionalCheckDigit && !showCheckDigit)
            {
                // Hide the check digit.
                barcodeText = barcodeText.Remove(inputLength - 1, 1);
            }
        }
    }
}