/* Code2of5Encoder.cs - Handles Code 2 of 5 based 1D symbols */

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
    /// Code 2 of 5 based symbol encoder.
    /// </summary>
    internal class Code2of5Encoder : SymbolEncoder
    {
        # region Tables

        // Industrial 2 of 5 bar pattens.
        static readonly string[] Standard2of5Table =    {
            "1111313111", "3111111131", "1131111131", "3131111111", "1111311131",
            "3111311111", "1131311111", "1111113131", "3111113111", "1131113111" };

        // Standard (Matrix) bar patterns.
        static readonly string[] Matrix2of5Table = {
            "113311", "311131", "131131", "331111", "113131",
            "313111", "133111", "111331", "311311", "131311" };

        # endregion

        private readonly bool optionalCheckDigit;
        private readonly bool showCheckDigit;
        char checkDigit;

        public Code2of5Encoder(Symbology symbolId, char[] barcodeMessage, bool optionalCheckDigit, bool showCheckDigit)
        {
            this.symbolId = symbolId;
            this.barcodeMessage = barcodeMessage;
            this.optionalCheckDigit = optionalCheckDigit;
            this.showCheckDigit = showCheckDigit;
        }

        public override Collection<SymbolData> EncodeData()
        {
            Symbol = new Collection<SymbolData>();
            barcodeData = MessagePreProcessor.NumericParser(barcodeMessage);

            switch (symbolId)
            {
                case Symbology.Standard2of5:
                    Standard2of5();
                    break;

                case Symbology.Industrial2of5:
                    Industrial2of5();
                    break;

                case Symbology.IATA2of5:
                    IATA2of5();
                    break;

                case Symbology.DataLogic2of5:
                    DataLogic2of5();
                    break;
            }

            return Symbol;
        }

        /// <summary>
        /// Code 2 of 5 Industrial.
        /// </summary>
        private void Industrial2of5()
        {
            int maxLength = 79;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Industrial 2 of 5: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Start character.
            rowPattern.Append("313111");
            for (int i = 0; i < inputLength; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                rowPattern.Append(Standard2of5Table[index]);
            }

            // Get the check character.
            if (optionalCheckDigit)
            {
                rowPattern.Append(Standard2of5Table[GetCheckSum()]);
            }

            // Stop character.
            rowPattern.Append("31113");

            // Expand row into the symbol data..
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            SetBarcodeText();
        }

        /// <summary>
        /// Code 2 of 5 Standard (Matrix)
        /// </summary>
        private void Standard2of5()
        {
            int maxLength = 112;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "Standard 2 of 5: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Add start character.
            rowPattern.Append("411111");
            for (int i = 0; i < inputLength; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                rowPattern.Append(Matrix2of5Table[index]);
            }

            // Get the check character.
            if (optionalCheckDigit)
            {
                rowPattern.Append(Matrix2of5Table[GetCheckSum()]);
            }

            // Add stop character.
            rowPattern.Append("41111");

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            SetBarcodeText();
        }

        /// <summary>
        /// Code 2 of 5 IATA.
        /// </summary>
        private void IATA2of5()
        {
            int maxLength = 80;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "IATA 2 of 5: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Add start character.
            rowPattern.Append("1111");
            for (int i = 0; i < inputLength; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                rowPattern.Append(Standard2of5Table[index]);
            }

            // Get the check character.
            if (optionalCheckDigit)
            {
                rowPattern.Append(Standard2of5Table[GetCheckSum()]);
            }

            // Add stop character.
            rowPattern.Append("311");

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            SetBarcodeText();
        }

        /// <summary>
        /// Code 2of5 Data Logic.
        /// </summary>
        private void DataLogic2of5()
        {
            int maxLength = 113;
            int inputLength = barcodeData.Length;
            StringBuilder rowPattern = new StringBuilder();

            if (inputLength > maxLength)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "DataLogic 2 of 5: Input data too long.\nMaximum length is {0} characters.", maxLength));
            }

            // Start character.
            rowPattern.Append("1111");
            for (int i = 0; i < inputLength; i++)
            {
                int index = CharacterSets.NumberOnlySet.IndexOf(barcodeData[i]);
                rowPattern.Append(Matrix2of5Table[index]);
            }

            // Get the check character.
            if (optionalCheckDigit)
            {
                rowPattern.Append(Standard2of5Table[GetCheckSum()]);
            }

            // Stop character.
            rowPattern.Append("311");

            // Expand row into the symbol data.
            SymbolBuilder.BuildSymbol(Symbol, rowPattern, 0.0f);

            // Set the human readable text.
            SetBarcodeText();
        }

        private int GetCheckSum()
        {
            checkDigit = GetCheckDigit.Mod10CheckDigit(barcodeData);
            return CharacterSets.NumberOnlySet.IndexOf(checkDigit);
        }

        private void SetBarcodeText()
        {
            barcodeText = new string(barcodeData);
            if (optionalCheckDigit && showCheckDigit)
            {
                checkDigitText = checkDigit.ToString();
                barcodeText += checkDigitText;
            }
        }
    }
}