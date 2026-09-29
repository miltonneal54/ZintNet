/*  SymbolEncoder.cs - Base class for all symbol encoders. */

/* 
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>

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
using System.Collections.ObjectModel;

namespace ZintNet.Encoders
{
    /// <summary>
    /// Provides the base class for all symbol encoders.
    /// </summary>
    internal abstract class SymbolEncoder
    {
        protected Symbology symbolId;
        protected char[] barcodeMessage;
        protected char[] compositeMessage;
        protected char[] supplementMessage;
        protected CompositeMode compositeMode;
        protected char[] barcodeData;
        protected Collection<SymbolData> Symbol;
        protected EncodingFormat encodingMode;
        protected int eci;
        protected bool isCompositeSymbol = false;
        protected bool isGS1 = false;
        //protected GS1Mode gs1Mode = GS1Mode.GS1Mode0;

        // Strings for returning variable length barcode text elements.
        protected string barcodeText = string.Empty;
        protected string checkDigitText = string.Empty;

        // Strings for returning EAN/UPC barcode text elements.
        protected string rightHandText = string.Empty;
        protected string leftHandText = string.Empty;
        protected string rightHandCharacter = string.Empty;
        protected string leftHandCharacter = string.Empty;
        protected string supplementText = string.Empty;
        protected int supplementBars = 0;
        protected int elementsPerCharacter = 0;     // The number of bars for one character;
        public const int supplementMargin = 12;     // Suppliment gap of 12 elements.

        public int aztecAutoSize;

        // Readonly properties.
        /// <summary>
        /// Gets the barcode's human readable text.
        /// </summary>
        public string BarcodeText
        {
            get { return barcodeText; }
        }

        /// <summary>
        /// Gets the barcode's check digit(s) string.
        /// </summary>
        public string CheckDigitText
        {
            get { return checkDigitText; }
        }

        /// <summary>
        /// Gets the barcode's left hand character.
        /// </summary>
        public String LeftHandCharacter
        {
            get { return leftHandCharacter; }
        }

        /// <summary>
        /// Gets the barcode's left hand text.
        /// </summary>
        public string LeftHandText
        {
            get { return leftHandText; }
        }

        /// <summary>
        /// Returns the barcode's right hand text.
        /// </summary>
        public string RightHandText
        {
            get { return rightHandText; }
        }

        /// <summary>
        /// Gets the barcode's right hand character.
        /// </summary>
        public string RightHandCharacter
        {
            get { return rightHandCharacter; }
        }

        /// <summary>
        /// Gets the barcode's supplement text.
        /// </summary>
        public string SupplementText
        {
            get { return supplementText; }
        }

        /// <summary>
        /// Set true if it is a Composite Symbol.
        /// </summary>
        public bool IsCompositeSymbol
        {
            get { return isCompositeSymbol;  }
        }

        /// <summary>
        /// Gets the number of elements in the supplement.
        /// </summary>
        public int SupplimentBars
        {
            get { return supplementBars; }
        }

        /// <summary>
        /// Gets the number of elements that represents one character.
        /// </summary>
        public int ElementsPerCharacter
        {
            get { return elementsPerCharacter; }
        }

        public abstract Collection<SymbolData> EncodeData();
    }
}