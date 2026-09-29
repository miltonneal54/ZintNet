/* BC412Encoder.cs - Handles IBM BC412 (SEMI T1-95) symbol. */

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

using System.Globalization;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// IBM BC412 Encoder (SEMI T1-95).
    /// </summary>
    internal class BC412Encoder : SymbolEncoder
    {
        #region Tables
        private readonly string BC412Set = "0R9GLVHA8EZ4NTS1J2Q6C7DYKBUIX3FWP5M";

        private readonly char[,] BC412Table = {
            {'1','1','1','1','1','1','1','5'}, {'1','3','1','1','1','2','1','2'},
            {'1','1','1','3','1','1','1','3'}, {'1','2','1','1','1','2','1','3'},
            {'1','2','1','2','1','3','1','1'}, {'1','3','1','3','1','1','1','1'},
            {'1','2','1','1','1','3','1','2'}, {'1','1','1','3','1','2','1','2'},
            {'1','1','1','2','1','4','1','1'}, {'1','1','1','5','1','1','1','1'},
            {'1','5','1','1','1','1','1','1'}, {'1','1','1','1','1','5','1','1'},
            {'1','2','1','3','1','2','1','1'}, {'1','3','1','2','1','1','1','2'},
            {'1','3','1','1','1','3','1','1'}, {'1','1','1','1','1','2','1','4'},
            {'1','2','1','2','1','1','1','3'}, {'1','1','1','1','1','3','1','3'},
            {'1','3','1','1','1','1','1','3'}, {'1','1','1','2','1','2','1','3'},
            {'1','1','1','4','1','1','1','2'}, {'1','1','1','2','1','3','1','2'},
            {'1','1','1','4','1','2','1','1'}, {'1','4','1','2','1','1','1','1'},
            {'1','2','1','2','1','2','1','2'}, {'1','1','1','3','1','3','1','1'},
            {'1','3','1','2','1','2','1','1'}, {'1','2','1','1','1','4','1','1'},
            {'1','4','1','1','1','2','1','1'}, {'1','1','1','1','1','4','1','2'},
            {'1','2','1','1','1','1','1','4'}, {'1','4','1','1','1','1','1','2'},
            {'1','2','1','4','1','1','1','1'}, {'1','1','1','2','1','1','1','4'},
            {'1','2','1','3','1','1','1','2'} };

        #endregion

        public BC412Encoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = barcodeMessage;
            BC412();
            return Symbol;
        }

        /// <summary>
        /// IBM BC412 Encoder.
        /// </summary>
        private void BC412()
        {
            int maxLength = 18;
            int minLength = 6;
            int length = barcodeData.Length;
            int[] indexs = new int[35];
            int counterOdd = 0, counterEven = 0;
            StringBuilder rowPattern = new StringBuilder();

            if (length > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "BC 412: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (length < minLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "BC 412: Input data too short.\nMminimum length is {0} characters.", minLength));
            }

            ArrayHelper.ToUpper(barcodeData);
            barcodeData = ArrayHelper.Insert(barcodeData, 1, '0');
            length = barcodeData.Length;

            for (int i = 0; i < length; i++)
            {
                indexs[i] = BC412Set.IndexOf(barcodeData[i]);
                if (indexs[i] == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "BC 412: Invalid character {0} at position {1} in input.\nAlpha numeric only, excluding 'O')", barcodeData[i], i - 1));
                }
            }

            for (int i = 0; i <= length; i++)
            {
                if (i % 2 == 1)
                {
                    counterEven += indexs[i];
                }

                else
                {
                    counterOdd += indexs[i];
                }
            }

            counterOdd %= 35;
            counterEven %= 35;

            // Check digit.
            int checksum = counterOdd + (2 * counterEven);
            checksum %= 35;
            checksum *= 17;
            checksum %= 35;

            barcodeData[1] = BC412Set[checksum];
            indexs[1] = checksum;

            // Start character.
            rowPattern.Append("12");
            for (int i  = 0; i <= length; i++)
            {
                for(int p = 0; p < 8; p++)
                {
                    rowPattern.Append(BC412Table[indexs[i], p]);
                }
            }

            // Stop character.
            rowPattern.Append("111");

            // Convert the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);
            barcodeText = new string(barcodeData);
        }
    }
}
