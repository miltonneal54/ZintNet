/* Code49Encoder.cs - Handles DX Film Edge symbol */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2024-2025 Antoine Merino <antoine.merino.dev@gmail.com>

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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ZintNet.Encoders
{
    /// <summary>
    /// DX film edge encoder.
    /// </summary>
    internal class DXFilmEdgeEncoder : SymbolEncoder
    {

        #region Constants

        private const int MAX_DX_INFO_LENGTH = 6;
        private const int MAX_FRAME_INFO_LENGTH = 3;

        #endregion

        public DXFilmEdgeEncoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = barcodeMessage;
            DXFilmEdge();
            return Symbol;
        }

        private void DXFilmEdge()
        {
            int maxLength = 10;
            BitVector binaryStream = new BitVector();
            bool hasFrameInfo = false;
            int inputLength = barcodeData.Length;
            string longClockPattern = "1111101010101010101010101010111";
            string shortClockPattern = "11111010101010101010111";
            string clockPattern;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "DX Film Edge: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            DXEncode(barcodeData, inputLength, binaryStream, ref hasFrameInfo);

            // Clock signal is longer if the frame number is provided.
            if (hasFrameInfo)
            {
                clockPattern = longClockPattern;
            }

            else
            {
                clockPattern = shortClockPattern;
            }

            List<byte> rowData1 = new List<byte>();
            List<byte> rowData2 = new List<byte>();
            for (int p = 0; p < binaryStream.SizeInBits; p++)
            {
                if (clockPattern[p] == '1')
                {
                    rowData1.Add(1);
                }

                else if (clockPattern[p] == '0')
                {
                    rowData1.Add(0);
                }

                if (binaryStream[p] == 1)
                {
                    rowData2.Add(1);
                }

                else if (binaryStream[p] == 0)
                {
                    rowData2.Add(0);
                }
            }

            SymbolData symbolData = new SymbolData(rowData1.ToArray(), 3.0f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData2.ToArray(), 3.0f);
            Symbol.Add(symbolData);

        }

        /// <summary>
        /// Validate the input data and encodes it into a binary stream.
        /// </summary>
        /// <param name="source">Input data.</param>
        /// <param name="length">Length of input data.</param>
        /// <param name="binaryStream">Bit vector to output the binary stream.</param>
        /// <param name="hasFrameInfo">True is has has a half frame flag.</param>
        private void DXEncode(char[] source, int length, BitVector binaryStream, ref bool hasFrameInfo)
        {
            byte parityBit = 0;
            int dxCode1 = -1, dxCode2 = -1, frameNumber = -1;
            char halfFrameFlag = '\0';
            string dxInfo = string.Empty;
            string frameInfo = string.Empty;
            int dxLength;

            hasFrameInfo = false;
            if (!char.IsDigit(source[0]))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "DX Film Edge: Invalid first character '{0}', DX code should start with a number.", source[0]));
            }

            // Split the DX information from the frame number.
            string[] dxData = new string(source).Split('/');

            if (dxData.Length > 1)
            {
                dxInfo = dxData[0];
                frameInfo = dxData[1];
                dxLength = dxInfo.Length;
                int frameInfoLength = frameInfo.Length;

                if (dxLength > MAX_DX_INFO_LENGTH)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX information length {0} too long. (maximum {1})", dxLength, MAX_DX_INFO_LENGTH));
                }

                if (frameInfoLength > MAX_FRAME_INFO_LENGTH)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Frame number part length {0} too long. (maximum {1})", frameInfoLength, MAX_FRAME_INFO_LENGTH));
                }

                hasFrameInfo = true;
                frameInfo = frameInfo.ToUpper(CultureInfo.CurrentCulture);
                if (!IsValidFrameInfo(frameInfo))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Frame number '{0}' is invalid.\nExpected digits, optionally followed by a single 'A'.)", frameInfo));
                }
            }

            else
            {
                // No '/' separator found, store the entire input in dxInfo.
                dxLength = length;
                if (dxLength > MAX_DX_INFO_LENGTH)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX information length {0} too long. (maximum {1})", dxLength, MAX_FRAME_INFO_LENGTH));
                }

                dxInfo = dxData[0];
            }

            for (int i = 0; i < dxLength; i++)
            {
                if (!char.IsDigit(dxInfo[i]) && dxInfo[i] != '-')
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Invalid character at position {0} in DX info.\nDigits and '-' character only.", i + 1));
                }
            }

            if (dxInfo.Contains("-"))
            {
                // DX code parts 1 and 2 are given directly, separated by a '-'. Eg: "79-7".
                string[] dxInfoParts = dxInfo.Split('-');
                if (dxInfoParts.Length > 2)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: The '-' is used to separate DX parts 1 and 2, and should be used no more than once."));
                }

                if (!int.TryParse(dxInfoParts[0], out dxCode1) || !int.TryParse(dxInfoParts[1], out dxCode2))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Wrong format for DX parts 1 and 2 (expected format: NNN-NN, digits)."));
                }

                if (dxCode1 <= 0 || dxCode1 > 127)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX part 1 '{0} out of range. (1 to 127)", dxCode1));
                }

                if (dxCode2 < 0 || dxCode2 > 15)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX part 2 '{0} out of range (0 to 15)", dxCode1));
                }
            }

            else
            {
                int dxExtract;
                // DX format is either 4 digits (DX Extract, eg: 1271) or 6 digits (DX Full, eg: 012710).
                if (dxLength == 5)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX number '{0} is incorrect.\nExpected 4 digits (DX extract) or 6 digits (DX full)", dxInfo));
                }

                if (dxLength == 6)
                {
                    // Convert DX Full to DX Extract (remove first and last character).
                    dxInfo = dxInfo.Remove(5, 1);    // Remove last;
                    dxInfo = dxInfo.Remove(0, 1);    // Remove first.
                    dxLength = dxInfo.Length;
                }

                // Compute the DX parts 1 and 2 from the DX extract.
                dxExtract = Common.ToInt(dxInfo.ToCharArray(), 0, dxLength);
                if (dxExtract < 16 || dxExtract > 2047)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: DX extract '{0}' out of range. (16 to 2047)", dxInfo));
                }

                dxCode1 = dxExtract >> 4;
                dxCode2 = dxExtract & 0xf;
            }

            if (hasFrameInfo)
            {
                if (frameInfo.Length < 1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Frame number indicator '/' at position {0}, but frame number is empty.", dxLength + 1));
                }

                // Some frame numbers are special values, convert them their equivalent number.
                if (frameInfo == "S" || frameInfo == "X")
                {
                    frameInfo = "62";
                }

                else if (frameInfo == "SA" || frameInfo == "XA")
                {
                    frameInfo = "62A";
                }

                else if (frameInfo == "K" || frameInfo == "00")
                {
                    frameInfo = "63";
                }

                else if (frameInfo == "KA" || frameInfo == "00A")
                {
                    frameInfo = "63A";
                }

                else if (frameInfo == "F")
                {
                    frameInfo = "0";
                }

                else if (frameInfo == "FA")
                {
                    frameInfo = "0A";
                }

                SplitFrameInfo(frameInfo, ref frameNumber, ref halfFrameFlag);

                if (frameNumber < 0 || frameNumber > 63)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DX Film Edge: Frame number '{0}' out of range. (0 to 63)", frameNumber));
                }
            }

            // Build the binary output.
            binaryStream.AppendBits(42, 6);            // Start pattern "101010".
            binaryStream.AppendBits(dxCode1, 7);
            binaryStream.AppendBit(0);                 // Separator between DX part 1 and DX part 2.
            binaryStream.AppendBits(dxCode2, 4);
            if (hasFrameInfo)
            {
                binaryStream.AppendBits(frameNumber, 6);
                if (halfFrameFlag == 'A')
                {
                    binaryStream.AppendBit(1);
                }

                else
                {
                    binaryStream.AppendBit(0);
                }

                binaryStream.AppendBit(0);
            }

            // Parity bit.
            for (int i = 6; i < binaryStream.SizeInBits; i++)
            {
                if (binaryStream[i] == 1)
                {
                    parityBit ^= 1;
                }
            }

            binaryStream.AppendBit(parityBit);
            binaryStream.AppendBits(5, 4);     // Stop pattern "0101"
            barcodeText = string.Format("{0,0:D4}{1}", (dxCode1 << 4) | dxCode2, frameInfo);
        }

        /// <summary>
        /// Check the frame info field is in the correct format.
        /// </summary>
        /// <param name="frameInfo">Frame information string.</param>
        /// <returns>True if correct, otherwise false.</returns>
        private bool IsValidFrameInfo(string frameInfo)
        {
            string frameChars = "1234567890SXKF";
            int length = frameInfo.Length;

            // All digits.
            if (Common.NumberOfDigits(frameInfo.ToCharArray(), 0, length) == length)
            {
                return true;
            }

            // One digit or special frame character.
            if (length == 1 && frameChars.IndexOf(frameInfo[0]) != -1)
            {
                return true;
            }

            // One digit or special frame character, followed by 'A'.
            if (length == 2 && frameChars.IndexOf(frameInfo[0]) != -1 && frameInfo.EndsWith("A"))
            {
                return true;
            }

            // Two digits followed by 'A'.

            if (length == 3 && char.IsDigit(frameInfo[0]) && char.IsDigit(frameInfo[1]) && frameInfo.EndsWith("A"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Splits thefFrame info int digit part and if present, character part.
        /// </summary>
        /// <param name="frameInfo">Frame information string.</param>
        /// <param name="frameNumber">Holds the frame number</param>
        /// <param name="halfFrameFlag">Hold the half frame flag.</param>
        private void SplitFrameInfo(string frameInfo, ref int frameNumber, ref char halfFrameFlag)
        {
            int length = frameInfo.Length;

            int count = Common.NumberOfDigits(frameInfo.ToCharArray(), 0, length);
            frameNumber = Common.ToInt(frameInfo.ToCharArray(), 0, count);
            halfFrameFlag = frameInfo.EndsWith("A") ? 'A' : '\x0';
        }
    }
}
