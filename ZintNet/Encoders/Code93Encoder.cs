/* Code93Encoder.cs - Handles Code 93 1D symbol */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
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

using System.Globalization;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    // Code 93 symbol encoder.
    internal class Code93Encoder : SymbolEncoder
    {
        #region Tables

        private static string[] Code93Table = {
            "131112","111213","111312","111411","121113","121212",
            "121311","111114","131211","141111","211113","211212",
            "211311","221112","221211","231111","112113","112212",
            "112311","122112","132111","111123","111222","111321",
            "121122","131121","212112","212211","211122","211221",
            "221121","222111","112122","112221","122121","123111",
            "121131","311112","311211","321111","112131","113121",
            "211131","121221","312111","311121","122211","111141" };

        private static string[] C93Expanded = {
            "bU", "aA", "aB", "aC", "aD", "aE", "aF", "aG", "aH", "aI", "aJ", "aK",
            "aL", "aM", "aN", "aO", "aP", "aQ", "aR", "aS", "aT", "aU", "aV", "aW", "aX", "aY", "aZ",
            "bA", "bB", "bC", "bD", "bE", " ", "cA", "cB", "cC", "$", "%", "cF", "cG", "cH", "cI", "cJ",
            "+", "cL", "-", ".", "/", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "cZ", "bF",
            "bG", "bH", "bI", "bJ", "bV", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "bK", "bL", "bM", "bN", "bO",
            "bW", "dA", "dB", "dC", "dD", "dE", "dF", "dG", "dH", "dI", "dJ", "dK", "dL", "dM", "dN", "dO",
            "dP", "dQ", "dR", "dS", "dT", "dU", "dV", "dW", "dX", "dY", "dZ", "bP", "bQ", "bR", "bS", "bT" };

        #endregion

        private readonly bool showCheckDigit;

        public Code93Encoder(Symbology symbolId, char[] barcodeMessage, bool showCheckDigit)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.showCheckDigit = showCheckDigit;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.TildeParser(barcodeMessage);
            Code93();
            return Symbol;
        }


        /// <summary>
        ///  Code 93 (Extended)
        /// </summary>
        private void Code93()
        {
            int cWeight = 1;
            int cCount = 0;
            int kWeight = 2;
            int kCount = 0;
            StringBuilder inputBuffer = new StringBuilder();
            StringBuilder rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;
            int bufferLength;

            // Check for valid characters and expand the input data.
            for (int i = 0; i < inputLength; i++)
            {
                if (barcodeData[i] > 127)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Code 93: Invalid character in input data.\nCharacter '{0}' at Position {1}.", barcodeData[i], i));
                }

                inputBuffer.Append(C93Expanded[barcodeData[i]]);
            }

            bufferLength = inputBuffer.Length;
            if (bufferLength > 123)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Code 93: Input data too long.\nMaximum length is {0} characters.", 123));
            }

            int[] values = new int[bufferLength];
            for (int i = 0; i < bufferLength; i++)
            {
                values[i] = CharacterSets.Code93Set.IndexOf(inputBuffer[i]);
            }

            // Calculate the check characters starting from the right most position.
            for (int i = values.Length - 1; i >= 0; i--)
            {
                cCount += values[i] * cWeight;
                cWeight++;
                if (cWeight == 21)
                {
                    cWeight = 1;
                }

                kCount += values[i] * kWeight;
                kWeight++;
                if (kWeight == 16)
                {
                    kWeight = 1;
                }
            }

            int checkDigitC = cCount % 47;
            kCount += checkDigitC;
            int checkDigitK = kCount % 47;

            // Add the start character.
            rowPattern.Append(Code93Table[47]);	
            for (int i = 0; i < bufferLength; i++)
            {
                rowPattern.Append(Code93Table[values[i]]);
            }

            // Add the "C" and "K" check digits, stop character and termination bar.
            rowPattern.Append(Code93Table[checkDigitC]);
            rowPattern.Append(Code93Table[checkDigitK]);
            rowPattern.Append(Code93Table[47] + "1");

            // Expand the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            inputLength = barcodeData.Length;
            for (int i = 0; i < inputLength; i++)
            {
                barcodeText += barcodeData[i] >= ' ' && barcodeData[i] != 0x7F ? barcodeData[i] : (char)149;
            }

            checkDigitText += CharacterSets.Code93Set[checkDigitC];
            checkDigitText += CharacterSets.Code93Set[checkDigitK];
            if (showCheckDigit)
            {
                barcodeText += checkDigitText;
            }
        }
    }
}