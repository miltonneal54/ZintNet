/* PostalEncoder.cs - Handles PostNet, PLANET, Korean, Japan Post, FIM, RM4SCC and Flattermarken */

/*  ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2008-2025 Robin Stuart <rstuart114@gmail.com>
    Including bug fixes by Bryan Hatton

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
    /// Postal encoders.
    /// </summary>
    internal class PostalEncoder : SymbolEncoder
    {
        #region Tables

        private static readonly string[] PostNetTable = {
            "LLSSS", "SSSLL", "SSLSL", "SSLLS", "SLSSL", "SLSLS", "SLLSS", "LSSSL",
            "LSSLS", "LSLSS" };

        private static readonly string[] PlanetTable = {
            "SSLLL", "LLLSS", "LLSLS", "LLSSL", "LSLLS", "LSLSL", "LSSLL", "SLLLS",
            "SLLSL", "SLSLL" };

        private static readonly string[] KoreanTable = {
            "1313150613", "0713131313", "0417131313", "1506131313",
            "0413171313", "17171313", "1315061313", "0413131713", "17131713", "13171713" };

        private static readonly string[] JapanTable = {
            "114", "132", "312", "123", "141", "321", "213", "231", "411", "144",
            "414", "324", "342", "234", "432", "243", "423", "441", "111" };

        private static readonly string[] RoyalValues = {
            "11", "12", "13", "14", "15", "10", "21", "22", "23", "24", "25",
            "20", "31", "32", "33", "34", "35", "30", "41", "42", "43", "44", "45", "40", "51", "52",
            "53", "54", "55", "50", "01", "02", "03", "04", "05", "00" };

        /* 0 = Full, 1 = Ascender, 2 = Descender, 3 = Tracker */
        private static readonly string[] RoyalTable = {
            "3300", "3210", "3201", "2310", "2301", "2211", "3120", "3030", "3021",
            "2130", "2121", "2031", "3102", "3012", "3003", "2112", "2103", "2013", "1320", "1230",
            "1221", "0330", "0321", "0231", "1302", "1212", "1203", "0312", "0303", "0213", "1122",
            "1032", "1023", "0132", "0123", "0033" };

        private static string[] FlatTable = { "0504", "18", "0117", "0216", "0315", "0414", "0513", "0612", "0711", "0810" };
        #endregion

        public PostalEncoder(Symbology symbolId, char[] barcodeMessage)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            switch (symbolId)
            {
                case Symbology.POSTNET:
                case Symbology.CEPNET:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    PostNet();
                    break;

                case Symbology.PLANET:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    Planet();
                    break;

                case Symbology.KoreaPost:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    KoreaPost();
                    break;

                case Symbology.FIM:
                    barcodeData = barcodeMessage;
                    FIM();
                    break;

                case Symbology.RoyalMail4SCC:
                    barcodeData = barcodeMessage;
                    RoyalMail();
                    break;

                case Symbology.KixCode:
                    barcodeData = barcodeMessage;
                    KixCode();
                    break;

                case Symbology.DaftCode:
                    barcodeData = barcodeMessage;
                    DaftCode();
                    break;

                case Symbology.Flattermarken:
                    barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);
                    Flattermarken();
                    break;

                case Symbology.JapanPost:
                    barcodeData = barcodeMessage;
                    JapanPost();
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// POSTNET Zip Codes. (US)
        /// </summary>
        private void PostNet()
        {
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (symbolId == Symbology.CEPNET)
            {
                if (inputLength != 8)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Brazilian Post: Input data wrong length.\nLength must be 8 numeric characters."));
                }
            }

            else
            {
                if (inputLength != 5 && inputLength != 9 && inputLength != 11)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "POSTNET: Input data wrong length.\nLength must be 5, 9 or 11 numeric characters."));
                }
            }

            symbolPattern.Append('L');
            int sum = 0;
            for (int i = 0; i < inputLength; i++)
            {
                int value = barcodeData[i] - '0';
                symbolPattern.Append(PostNetTable[value]);
                sum += value;
            }

            int checkValue = (10 - (sum % 10)) % 10;
            symbolPattern.Append(PostNetTable[checkValue]);
            symbolPattern.Append('L');
            SymbolBuilder.USPostSymbol(Symbol, symbolPattern);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            checkDigitText = new string((char)(checkValue + '0'), 1);
        }

        /// <summary>
        /// PLANET Tracking System. (US)
        /// </summary>
        private void Planet()
        {
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength != 11 && inputLength != 13)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "PLANET: Input data wrong length.\nLength must be 11 or 13 numeric characters."));
            }

            symbolPattern.Append('L');
            int sum = 0;
            for (int i = 0; i < inputLength; i++)
            {
                int value = barcodeData[i] - '0';
                symbolPattern.Append(PlanetTable[value]);
                sum += value;
            }

            int checkValue = (10 - (sum % 10)) % 10;
            symbolPattern.Append(PlanetTable[checkValue]);
            symbolPattern.Append('L');
            SymbolBuilder.USPostSymbol(Symbol, symbolPattern);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            checkDigitText = new string((char)(checkValue + '0'), 1);
        }

        /// <summary>
        /// Korean Post.
        /// </summary>
        private void KoreaPost()
        {
            int maxLength = 6;
            StringBuilder rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Korea Post: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            if (inputLength < maxLength)
            {
                string zeros = new string('0', 6 - inputLength);
                barcodeData = ArrayHelper.Insert(barcodeData, 0, zeros);
                inputLength = barcodeData.Length;
            }

            int sum = 0;
            for (int l = 0; l < inputLength; l++)
            {
                sum += barcodeData[l] - '0';
            }

            int checkValue = (10 - (sum % 10)) % 10;
            barcodeData = ArrayHelper.Insert(barcodeData, barcodeData.Length, (char)(checkValue + '0'));
            int value;
            for (int l = 5; l >= 0; l--)
            {
                value = barcodeData[l] - '0';
                rowPattern.Append(KoreanTable[value]);
            }

            value = barcodeData[6] - '0';
            rowPattern.Append(KoreanTable[value]);

            // Expand the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            checkDigitText += barcodeData[6];
        }

        /// <summary>
        /// FIM.
        /// </summary>
        private void FIM()
        {
            int maxLength = 1;
            StringBuilder rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "FIM: Input data too long.\nMaximum length is {0} character.", maxLength));
            }

            switch (barcodeData[0])
            {
                case 'a':
                case 'A':
                    rowPattern.Append("111515111");
                    break;

                case 'b':
                case 'B':
                    rowPattern.Append("13111311131");
                    break;

                case 'c':
                case 'C':
                    rowPattern.Append("11131313111");
                    break;

                case 'd':
                case 'D':
                    rowPattern.Append("1111131311111");
                    break;

                default:
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "FIM: Codabar: Invalid character in input data.\nCharacters A, B, C, D or E only allowed."));
            }

            // Expand the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);
        }

        /// <summary>
        /// Royal Mail 4-State Customer Code.
        /// </summary>
        private void RoyalMail()
        {
            int maxLength = 50;
            int top = 0;
            int bottom = 0;
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Royal Mail: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < inputLength; i++)
            {
                barcodeData[i] = char.ToUpper(barcodeData[i], CultureInfo.CurrentCulture);
                if (CharacterSets.KRSET.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Royal Mail: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            symbolPattern.Append('1');  // Start character.
            for (int i = 0; i < inputLength; i++)
            {
                int position = CharacterSets.KRSET.IndexOf(barcodeData[i]);
                symbolPattern.Append(RoyalTable[position]);
                string values = RoyalValues[position];
                top += values[0] - '0';
                bottom += values[1] - '0';
            }

            // Calculate the check digit.
            int row = (top % 6) - 1;
            int column = (bottom % 6) - 1;
            if (row == -1)
            {
                row = 5;
            }

            if (column == -1)
            {
                column = 5;
            }

            int checkValue = (6 * row) + column;
            symbolPattern.Append(RoyalTable[checkValue]);
            symbolPattern.Append('0');  // Stop character.
            SymbolBuilder.FourStateSymbol(Symbol, symbolPattern);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
            checkDigitText += CharacterSets.KRSET[checkValue];
        }

        /// <summary>
        /// Dutch Post KIX.
        /// </summary>
        private void KixCode()
        {
            int maxLength = 18;
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Dutch Post KIX: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < inputLength; i++)
            {
                barcodeData[i] = char.ToUpper(barcodeData[i], CultureInfo.CurrentCulture);
                if (CharacterSets.KRSET.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Dutch Post KIX: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            for (int i = 0; i < inputLength; i++)
            {
                int position = CharacterSets.KRSET.IndexOf(barcodeData[i]);
                symbolPattern.Append(RoyalTable[position]);
            }

            SymbolBuilder.FourStateSymbol(Symbol, symbolPattern);

            // Set the human readable text.
            barcodeText = new string(barcodeData);
        }

        /// <summary>
        /// DAFT Code.
        /// </summary>
        private void DaftCode()
        {
            int maxLength = 576;
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "DAFT Code: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int i = 0; i < inputLength; i++)
            {
                barcodeData[i] = char.ToUpper(barcodeData[i], CultureInfo.CurrentCulture);
            }

            for (int i = 0; i < inputLength; i++)
            {
                switch (barcodeData[i])
                {
                    case 'D':
                        symbolPattern.Append("2");
                        break;

                    case 'A':
                        symbolPattern.Append("1");
                        break;

                    case 'F':
                        symbolPattern.Append("0");
                        break;

                    case 'T':
                        symbolPattern.Append("3");
                        break;

                    default:
                        throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "DAFT Code: Invalid character in input data.\nCharacter '{0}' at position {1}.", barcodeData[i], i));
                }
            }

            SymbolBuilder.FourStateSymbol(Symbol, symbolPattern);
        }

        /// <summary>
        /// Flattermarken
        /// </summary>
        private void Flattermarken()
        {
            int maxLength = 128;
            StringBuilder rowPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Flattermarken: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            for (int l = 0; l < inputLength; l++)
            {
                int value = barcodeData[l] - '0';
                rowPattern.Append(FlatTable[value]);
            }

            // Expand the row pattern into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);
        }

        /// <summary>
        /// Japanese Postal.
        /// </summary>
        private void JapanPost()
        {
            int maxLength = 20;
            int sourceIndex = 0;
            int intermediateIndex = 0;
            SymbolData symbolData;
            StringBuilder intermediate = new StringBuilder();
            StringBuilder symbolPattern = new StringBuilder();
            int inputLength = barcodeData.Length;

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Japan Post: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            barcodeData = ArrayHelper.ToUpper(barcodeData);
            for (int i = 0; i < inputLength; i++)
            {
                if (CharacterSets.SHKASUTSET.IndexOf(barcodeData[i]) == -1)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Japan Post: Invalid character in input data.\nCharacter '{0}' at Position {1}.", barcodeData[i], i));
                }
            }

            intermediate.Append('d', 20);  // Pad character CC4.

            do
            {
                if (char.IsDigit(barcodeData[sourceIndex]) || (barcodeData[sourceIndex] == '-'))
                {
                    intermediate[intermediateIndex] = barcodeData[sourceIndex];
                    intermediateIndex++;
                }

                else
                {
                    if (barcodeData[sourceIndex] <= 'J')
                    {
                        intermediate[intermediateIndex] = 'a';
                        intermediate[intermediateIndex + 1] = (char)(barcodeData[sourceIndex] - 'A' + '0');
                        intermediateIndex += 2;
                    }

                    else if (barcodeData[sourceIndex] <= 'T')
                    {
                        intermediate[intermediateIndex] = 'b';
                        intermediate[intermediateIndex + 1] = (char)(barcodeData[sourceIndex] - 'K' + '0');
                        intermediateIndex += 2;
                    }

                    else
                    {
                        intermediate[intermediateIndex] = 'c';
                        intermediate[intermediateIndex + 1] = (char)(barcodeData[sourceIndex] - 'U' + '0');
                        intermediateIndex += 2;
                    }
                }

                sourceIndex++;
            } while ((sourceIndex < inputLength) && (intermediateIndex < 20));

            symbolPattern.Append("13"); // Start.

            int sum = 0;
            for (int i = 0; i < 20; i++)
            {
                int position = CharacterSets.KASUTSET.IndexOf(intermediate[i]);
                symbolPattern.Append(JapanTable[position]);
                sum += CharacterSets.CHKASUTSET.IndexOf(intermediate[i]);
            }

            // Calculate check digit.
            int checkValue = 19 - (sum % 19);
            if (checkValue == 19)
            {
                checkValue = 0;
            }

            char checkDigit = '\0';
            if (checkValue <= 9)
            {
                checkDigit = (char)(checkValue + '0');
            }

            if (checkValue == 10)
            {
                checkDigit = '-';
            }

            if (checkValue >= 11)
            {
                checkDigit = (char)((checkValue - 11) + 'a');
            }

            symbolPattern.Append(JapanTable[CharacterSets.KASUTSET.IndexOf(checkDigit)]);
            symbolPattern.Append("31"); // Stop.

            int patternLength = symbolPattern.Length;
            byte[] rowData1 = new byte[patternLength * 2];
            byte[] rowData2 = new byte[patternLength * 2];
            byte[] rowData3 = new byte[patternLength * 2];

            int rowIndex = 0;
            for (int l = 0; l < patternLength; l++)
            {
                if ((symbolPattern[l] == '2') || (symbolPattern[l] == '1'))
                {
                    rowData1[rowIndex] = 1;
                }

                rowData2[rowIndex] = 1;
                if ((symbolPattern[l] == '3') || (symbolPattern[l] == '1'))
                {
                    rowData3[rowIndex] = 1;
                }

                rowIndex += 2;
            }

            symbolData = new SymbolData(rowData1, 3.0f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData2, 2.0f);
            Symbol.Add(symbolData);
            symbolData = new SymbolData(rowData3, 3.0f);
            Symbol.Add(symbolData);
        }
    }
}
