/* Code11Encoder.cs - Handles Code 11 1D symbol */

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
    /// Code 11 symbol encoder.
    /// </summary>
    internal class Code11Encoder : SymbolEncoder
    {
        #region Tables

        private static string[] Code11Table = {
            "111131", "311131", "131131", "331111", "113131", "313111",
            "133111", "111331", "311311", "311111", "113111", "113311" };

        #endregion

        private readonly Code11CheckDigits numberOfCheckDigits;

        public Code11Encoder(Symbology symbolId, char[] barcodeMessage, Code11CheckDigits numberOfCheckDigits)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.numberOfCheckDigits = numberOfCheckDigits;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = barcodeMessage;
            Code11();
            return Symbol;
        }

        private void Code11()
        {
            int maxLength = 140;
            StringBuilder rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 11: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Catch any invalid characters.
            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.Code11Set.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Code 11: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i + 1));
                }
            }

            // Add the start character.
            rowPattern.Append(Code11Table[11]);
            int index;
            for (int i = 0; i < inputLength; i++)
            {
                index = CharacterSets.Code11Set.IndexOf(barcodeData[i]);
                rowPattern.Append(Code11Table[index]);
            }

            // Calculate the checksums and add the check digits.
            if (numberOfCheckDigits != Code11CheckDigits.None)
            {
                // Calculate the "C" & "K" checksums.
                int cCount = 0;
                int cWeight = 1;
                int kCount = 0;
                int kWeight = 1;

                for (int i = inputLength - 1; i >= 0; i--)
                {
                    index = CharacterSets.Code11Set.IndexOf(barcodeData[i]);
                    cCount += index * cWeight;
                    cWeight++;
                    if (cWeight > 10)
                    {
                        cWeight = 1;
                    }
                }

                int checkDigitC = cCount % 11;
                checkDigitText += CharacterSets.Code11Set[checkDigitC];
                barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigitText[0]);
                rowPattern.Append(Code11Table[checkDigitC]);
                inputLength++;

                if (numberOfCheckDigits == Code11CheckDigits.Two)
                {
                    for (int i = inputLength - 1; i >= 0; i--)
                    {
                        index = CharacterSets.Code11Set.IndexOf(barcodeData[i]);
                        kCount += index * kWeight;
                        kWeight++;
                        if (kWeight > 9)
                        {
                            kWeight = 1;
                        }
                    }

                    int checkDigitK = kCount % 11;
                    checkDigitText += CharacterSets.Code11Set[checkDigitK];
                    barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigitText[1]);
                    rowPattern.Append(Code11Table[checkDigitK]);
                }
            }

            // Add stop character.
            rowPattern.Append(Code11Table[11]);

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeMessage);
            barcodeText += checkDigitText;
        }
    }
}