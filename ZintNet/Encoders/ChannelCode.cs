/* ChannelCodeEncoder.cs - Handles Channel Code 1D symbols */

/*
    ZintNetLib - a C# port of libzint.
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>
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
    internal class ChannelCodeEncoder : SymbolEncoder
    {
        private int channels = 0;

        // Global variables for Channel Code.
        private StringBuilder rowPattern;
        private int[] S;
        private int[] B;
        private long index;
        private long targetValue;

        public ChannelCodeEncoder(Symbology symbolId, char[] barcodeMessage, int channels)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.channels = channels;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
            ChannelCode();
            return Symbol;
        }

        private void ChannelCode()
        {
            int[] maxRanges = { -1, -1, -1, 26, 292, 3493, 44072, 576688, 7742862 };
            int maxLength = 7;
            index = 0;
            targetValue = 0;
            S = new int[11];
            B = new int[11];
            rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Channel Code: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            targetValue = int.Parse(new string(barcodeMessage));

            if (channels == 0)
            {
                channels = inputLength + 1;
                if (targetValue > 576688 && channels < 8)
                {
                    channels = 8;
                }

                else if (targetValue > 44072 && channels < 7)
                {
                    channels = 7;
                }

                else if (targetValue > 3493 && channels < 6)
                {
                    channels = 6;
                }

                else if (targetValue > 292 && channels < 5)
                {
                    channels = 5;
                }

                else if (targetValue > 26 && channels < 4)
                {
                    channels = 4;
                }
            }

            if (targetValue > maxRanges[channels])
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                     "Channel Code: Target value {0} is out of range for {1} channels.", targetValue, channels));
            }


            B[0] = S[1] = B[1] = S[2] = B[2] = 1;

            NextS(channels, 3, channels, channels);
            if (channels - 1 - inputLength > 0)
            {
                string zeros = new string('0', channels - 1 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
            }

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
        }

        /* NextS() and NextB() are from ANSI/AIM BC12-1998 and are Copyright (c) AIM 1997.
           They are used here on the understanding that they form part of the specification
           for Channel Code and therefore their use is permitted under the following terms
           set out in that document:

           "It is the intent and understanding of AIM, that the symbology presented in this
           specification is entirely in the public domain and free of all use restrictions,
           licenses and fees. AIM USA, its member companies, or individual officers
           assume no liability for the use of this document." */

        private void NextS(int channel, int i, int maxS, int maxB)
        {
            for (int s = (i < channel + 2) ? 1 : maxS; s <= maxS; s++)
            {
                S[i] = s;
                NextB(channel, i, maxB, maxS + 1 - s);
             }
        }

        private void NextB(int channel, int i, int maxB, int maxS)
        {
            int b = (S[i] + B[i - 1] + S[i - 1] + B[i - 2] > 4) ? 1 : 2;

            if (i < channel + 2)
            {
                for (; b <= maxB; b++)
                {
                    B[i] = b;
                    NextS(channel, i + 1, maxS, maxB + 1 - b);
                }
            }

            else if (b <= maxB)
            {
                B[i] = maxB;
                if (index == targetValue)
                {
                    // Target reached - save the generated pattern.
                    rowPattern.Append("11110");
                    for (int p = 0; p < 11; p++)
                    {
                        rowPattern.Append((char)(S[p] + '0'));
                        rowPattern.Append((char)(B[p] + '0'));
                    }
                }

                index++;
            }
        }
    }
}
