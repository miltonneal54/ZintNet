/* PharmacodeEncoder.cs - Handles Pharamacode and Pharmacode 2 Track symbols */

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Pharmacode symbol encoder.
    /// </summary>
    internal class PharmacodeEncoder : SymbolEncoder
    {
        public PharmacodeEncoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
            switch (symbolId)
            {
                case Symbology.Pharmacode:
                    PharmaOne();
                    break;

                case Symbology.Pharmacode2Track:
                    PharmaTwo();
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// PharmaCode 1 Track.
        /// </summary>
        private void PharmaOne()
        {
            int maxLength = 6;
            List<char> intermediate = new List<char>();     // 131070 -> 17 bits.
            StringBuilder rowPattern = new StringBuilder(); // 17 * 2 + 1.
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Pharmacode: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            uint inputValue = uint.Parse(new string(barcodeMessage), CultureInfo.CurrentCulture);
            if ((inputValue < 3) || (inputValue > 131070))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Pharmacode: Input value of {0} is out of range, [3 to 131070] required.", inputValue));
            }

            do {
                if ((inputValue & 1) == 0)
                {
                    intermediate.Add('W');
                    inputValue = (inputValue - 2) / 2;
                }

                else
                {
                    intermediate.Add('N');
                    inputValue = (inputValue - 1) / 2;
                }

            } while (inputValue != 0);

            int iLength = intermediate.Count;
            for (int c = iLength - 1; c >= 0; c--)
            {
                if (intermediate[c] == 'W')
                {
                    rowPattern.Append("32");    // 3 bars and 2 spaces.
                }

                else
                {
                    rowPattern.Append("12");    // 1 bar and 2 spaces.
                }
            }

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);
        }

        /// <summary>
        /// PharmaCode 2 Track.
        /// </summary>
        private void PharmaTwo()
        {
            int maxLength = 8;
            SymbolData symbolData;
            byte[] rowData1;
            byte[] rowData2;
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Pharmacode 2-Track: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            char[] symbolPattern = PharmaTwoCalculator();
            int patternLength = symbolPattern.Length;
            rowData1 = new byte[patternLength * 2];
            rowData2 = new byte[patternLength * 2];
            int index = 0;

            for (int l = 0; l < patternLength; l++)
            {
                if ((symbolPattern[l] == '2') || (symbolPattern[l] == '3'))
                {
                    rowData1[index] = 1;
                }

                if ((symbolPattern[l] == '1') || (symbolPattern[l] == '3'))
                {
                    rowData2[index] = 1;
                }

                index += 2;
            }

            symbolData = new SymbolData(rowData1);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData2);
            Symbol.Add(symbolData);
        }

        private char[] PharmaTwoCalculator()
        {
            char[] pattern;
            List<char> intermediate = new List<char>();

            /* This code uses the Two Track Pharmacode defined in the document at
               http://www.laetus.com/laetus.php?request=file&id=69 and using a modified
               algorithm from the One Track system. This standard accepts integer values
               from 4 to 64570080. */

            uint inputValue = uint.Parse(new string(barcodeMessage), CultureInfo.CurrentCulture);
            if ((inputValue < 4) || (inputValue > 64570080))
            {
                throw new InvalidDataException(String.Format(CultureInfo.CurrentCulture,
                    "Pharmacode 2-Track: Input value of {0} is out of range, [4 to 64570080] required.", inputValue));
            }

            do {
                switch (inputValue % 3)
                {
                    case 0:
                        intermediate.Add('3');
                        inputValue = (inputValue - 3) / 3;
                        break;

                    case 1:
                        intermediate.Add('1');
                        inputValue = (inputValue - 1) / 3;
                        break;

                    case 2:
                        intermediate.Add('2');
                        inputValue = (inputValue - 2) / 3;
                        break;
                }

            } while (inputValue != 0);

            int count = intermediate.Count;
            pattern = new char[count];

            for (int c = count - 1; c >= 0; c--)
            {
                pattern[count - c - 1] = intermediate[c];
            }

            return pattern;
        }
    }
}
