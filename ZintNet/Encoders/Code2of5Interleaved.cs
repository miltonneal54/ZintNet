/* Interleaved2of5Encoder.cs - Handles Interleaved Code 2 of 5 based 1D symbols. */

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
    /// Interleaved 2 of 5 based symbol encoder.
    /// </summary>
    internal class Interleaved2of5Encoder : SymbolEncoder
    {
        # region Tables.

        // Interleaved bar pattens.
        static readonly string[] Interleaved2of5Table = {
             "11221", "21112", "12112", "22111", "11212",
             "21211", "12211", "11122", "21121", "12121" };

        #endregion

        private readonly bool optionalCheckDigit;
        private readonly I2of5CheckDigitType checkDigitType;
        private readonly bool showCheckDigit;

        // ITF14, Dutch Post.
        public Interleaved2of5Encoder(Symbology symbology, char[] barcodeMessage)
            : this(symbology, barcodeMessage, false, false, I2of5CheckDigitType.None)
        { }

        // Interleaved 2 of 5.
        public Interleaved2of5Encoder(Symbology symbolId, char[] barcodeMessage, bool optionalCheckDigit, bool showCheckDigit, I2of5CheckDigitType checkDigitType)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionalCheckDigit = optionalCheckDigit;
            this.showCheckDigit = showCheckDigit;
            this.checkDigitType = checkDigitType;
         }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
            switch (symbolId)
            {
                // Note: ITF14 and DutchPost generate their custom check digits.
                case Symbology.Interleaved2of5:
                    Interleaved2of5();
                    break;

                case Symbology.ITF14:
                    ITF14();
                    break;

                case Symbology.DeutschePostIdentCode:
                    DeutschePost(11);
                    break;

                case Symbology.DeutschePostLeitCode:
                    DeutschePost(13);
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// 2 of 5 Interleaved.
        /// </summary>
        private void Interleaved2of5()
        {
            char checkDigit;
            int maxLength = 125;
            StringBuilder rowPattern = new StringBuilder();
            
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Interleaved 2 of 5: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (optionalCheckDigit)
            {
                if (checkDigitType == I2of5CheckDigitType.USS)
                {
                    checkDigit = GetCheckDigit.Mod10CheckDigit(barcodeData);
                }

                else
                {
                    checkDigit = GetCheckDigit.OPCCCheckDigit(barcodeData);
                }

                Array.Resize(ref barcodeData, inputLength + 1);
                barcodeData[inputLength++] = checkDigit;
            }

            // Interleaved 2 of 5 must have an even number of numeric characters.
            // Pad out with a leading "0" if not.
            if (inputLength % 2 != 0)
            {
                barcodeData = ArrayHelper.Insert(barcodeData, 0, '0');
                inputLength = barcodeData.Length;
            }

            // Add the start character.
            rowPattern.Append("1111");
            for (int i = 0; i < inputLength - 1; i += 2)
            {
                // Get each character pair at a time
                int index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                string data1 = Interleaved2of5Table[index];
                index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i + 1]);
                string data2 = Interleaved2of5Table[index];

                // Interleave the data.
                for (int x = 0; x < 5; x++)
                {
                    rowPattern.Append(data1[x]);
                    rowPattern.Append(data2[x]);
                }
            }

            // Add stop character.
            rowPattern.Append("311");

            // Use 3:1 bar ratio for ITF14.
            if (symbolId == Symbology.ITF14)
            {
                rowPattern.Replace('2', '3');
            }

            // Convert the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            if( symbolId == Symbology.Interleaved2of5)
            {
                barcodeText = new string(barcodeData, 0, inputLength);
                if (optionalCheckDigit && !showCheckDigit)
                {
                    // Remove the check digit from the text.
                    barcodeText = barcodeText.Remove(inputLength - 1, 1);
                }
            }
        }

        /// <summary>
        /// ITF 14.
        /// </summary>
        private void ITF14()
        {
            char checkDigit;
            int maxLength = 14;
            int inputLength = barcodeData.Length;

            if(inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "ITF-14: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Checkdigit not supplied.
            if (inputLength <= 13)
            {
                string zeros = new string('0', 13 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                checkDigit = GetCheckDigit.Mod10CheckDigit(barcodeData);
                barcodeData = ArrayHelper.Insert(barcodeData, barcodeData.Length, checkDigit);
                inputLength = barcodeData.Length;
            }

            // Check digit supplied. Check if it is valid.
            else if (inputLength == 14)  
            {
                char cd = barcodeData[inputLength - 1];
                char[] data = new char[13];
                Array.Copy(barcodeData, data, 13);
                checkDigit = GetCheckDigit.Mod10CheckDigit(data);
                if (checkDigit != cd)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "ITF-14: Invalid check digit in input data.\nExpected '{0}' at Position 14.", checkDigit));
                }
            }

            Interleaved2of5();

            // Set the human readable text.
            for (int i = 0; i < inputLength; i++)
            {
                barcodeText += barcodeData[i];
                // Insert spaces at these points.
                if (i == 0 || i == 2 || i == 7 || i == 12)
                {
                    barcodeText += "  ";
                }
            }
        }

        /// <summary>
        /// Dutch Post (Identcode and Leitcode)
        /// </summary>
        /// <param name="maxLength">Length to determine code type.</param>
        private void DeutschePost(int maxLength)
        {
            int count = 0;
            char checkDigit;
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Deutsche Post: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < maxLength)
            {
                string zeros = new string('0', maxLength - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                inputLength = barcodeData.Length;
            }

            for (int i = inputLength - 1; i >= 0; i--)
            {
                count += 4 * (barcodeData[i] - '0');

                if ((i & 1) > 0)
                {
                    count += 5 * (barcodeData[i] - '0');
                }
            }

            checkDigit = (char)(((10 - (count % 10)) % 10) + '0');
            barcodeData = ArrayHelper.Insert(barcodeData, inputLength, checkDigit);
            Interleaved2of5();

            // Set the human readable text.
            switch (symbolId)
            {
                case Symbology.DeutschePostLeitCode:
                    for (int i = 0; i < barcodeData.Length; i++)
                    {
                        barcodeText += barcodeData[i];
                        // Insert '.' at these points.
                        if (i == 4 || i == 7 || i == 10)
                        {
                            barcodeText += ".";
                        }

                        // Insert spaces at these points.
                        else if (i == 12)
                        {
                            barcodeText += "  ";
                        }
                    }

                    break;

                case Symbology.DeutschePostIdentCode:
                    for (int i = 0; i < barcodeData.Length; i++)
                    {
                        barcodeText += barcodeData[i];
                        // Insert '.' at these points.
                        if (i == 1 ||  i == 7)
                        {
                            barcodeText += ".";
                        }

                        // Insert spaces at these points.
                        else if (i == 4 || i == 10)
                        {
                            barcodeText += "  ";
                        }
                    }

                    break;
            }
        }
    }
}
