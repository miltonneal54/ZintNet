/* MessageProcessors.cs - Pre processors for the barcode data to be encoded. */

/*
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
using System.Globalization;
using System.Text;

namespace ZintNet
{
    /// <summary>
    /// Performs validation checks on the barcode message data.
    /// </summary>
    internal static class MessagePreProcessor
    {
        static readonly string set82 = "!\"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
        static readonly string set39 = "#-/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Check that the message format conforms with the HIBC stardard.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <returns>Character array holding the HIBC formated message.</returns>
        public static char[] HIBCParser(char[] message)
        {
            if (message.Length > 70)
            {
                throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                    "HIBC: Input data too long.\nMaximum length is {0} characters.", 70));
            }

            message = ArrayHelper.ToUpper(message);

            // First character must be a HIBC Supplier Labeling flag.
            if (message[0] != '+')
            {
                message = ArrayHelper.Insert(message, 0, '+');
            }

            foreach (char c in message)
            {
                if (CharacterSets.Code39Set.IndexOf(c) == -1)
                {
                    throw new InvalidDataLengthException(string.Format(CultureInfo.CurrentCulture,
                        "HIBC: Invalid character '{0}' in input.", c));
                }
            }

            char checkDigit = GetCheckDigit.Mod43CheckDigit(message);
            message = ArrayHelper.Insert(message, message.Length, checkDigit);
            return message;
        }

        /// <summary>
        /// Checks for valid characters and the correct formation of the AI's.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <returns>Character array holding the message.</returns>
        public static char[] GS1Parser(char[] message)
        {
            StringBuilder bcData = new StringBuilder();
            bool invalidData;
            string aiString = string.Empty;
            int[] aiValue = new int[100];
            int[] aiLocation = new int[100];
            int[] dataLocation = new int[100];
            int[] dataLength = new int[100];
            int inputLength = message.Length;

            // Detect extended ASCII characters.
            for (int i = 0; i < inputLength; i++)
            {
                if (message[i] >= 128)
                {
                    throw new InvalidDataException("GS1: Extended ASCII characters are not supported.");
                }

                if (message[i] < 32)
                {
                    throw new InvalidDataException("GS1: Control characters are not supported.");
                }
            }

            // GS1 must start with an AI.
            if (message[0] != '[')
            {
                throw new InvalidDataFormatException("GS1: Message must start with an Application Identifier.");
            }

            int bracketLevel = 0;
            int maxBracketLevel = 0;
            int aiLength = 0;
            int maxAILength = 0;
            int minAILength = 5;
            int j = 0;
            invalidData = false;

            int[] splitSize;
            string[] elements;

            // Check the bracket formatting and inputLength of AI's.
            for (int i = 0; i < inputLength; i++)
            {
                aiLength += j;
                if (j == 1 && message[i] != ']' && !Char.IsDigit(message[i]))
                {
                    invalidData = true;
                }

                if (message[i] == '[')
                {
                    bracketLevel++;
                    j = 1;
                }

                if (message[i] == ']')
                {
                    bracketLevel--;
                    if (aiLength < minAILength)
                    {
                        minAILength = aiLength;
                    }

                    j = 0;
                    aiLength = 0;
                }

                if (bracketLevel > maxBracketLevel)
                {
                    maxBracketLevel = bracketLevel;
                }

                if (aiLength > maxAILength)
                {
                    maxAILength = aiLength;
                }
            }

            minAILength--;

            // Check for invalid data or malformed AI's.
            if (bracketLevel != 0 || maxBracketLevel > 1)   // AI brackets not formatted correctly.
            {
                throw new InvalidDataFormatException("GS1: Invalid Application Identifier formatting.");
            }

            if (maxAILength > 4 || minAILength <= 1)    // AI is too long or too short.
            {
                throw new InvalidDataFormatException("GS1: Invalid length for an Application Identifier.");
            }

            if (invalidData == true)    // Non-numeric data in AI.
            {
                throw new InvalidDataFormatException("GS1: Non-numeric data in Application Identifier.");
            }

            // Find the number of AI's in the message.
            int aiCount = 0;
            for (int i = 1; i < inputLength; i++)
            {
                if (message[i - 1] == '[')
                {
                    aiString = string.Empty;
                    aiLocation[aiCount] = i;
                    j = 0;
                    do
                    {
                        aiString += message[i + j];
                        j++;
                    }
                    while (aiString[j - 1] != ']');

                    aiString = aiString.Substring(0, j - 1);
                    aiValue[aiCount] = int.Parse(aiString, CultureInfo.CurrentCulture);
                    aiCount++;
                }
            }

            for (int i = 0; i < aiCount; i++)
            {
                dataLocation[i] = aiLocation[i] + 3;
                if (aiValue[i] >= 100)
                {
                    dataLocation[i]++;
                }

                if (aiValue[i] >= 1000)
                {
                    dataLocation[i]++;
                }

                dataLength[i] = 0;
                do
                {
                    dataLength[i]++;
                }
                while ((dataLocation[i] + dataLength[i] - 1) < inputLength && message[dataLocation[i] + dataLength[i] - 1] != '[');
                dataLength[i]--;
            }

            for (int i = 0; i < aiCount; i++)
            {
                if (dataLength[i] == 0) // No data for given AI.
                {
                    throw new InvalidDataFormatException(String.Format(CultureInfo.CurrentCulture, "GS1: No data field for AI {0}.", aiString));
                }
            }

            for (int i = 0; i < aiCount; i++)
            {
                string aiElement = new string(message, dataLocation[i], dataLength[i]);
                switch (aiValue[i])
                {
                    // Length 2 Fixed numeric with no checksum.
                    case 20:    // VARIANT
                        FixedLengthNumeric(aiElement, aiValue[i], 2);
                        break;

                    // Length 3 Fix numeric with no checksum. (ISO3166)
                    case 422:   // ORIGIN
                    case 424:   // COUNTRY – PROCESS
                    case 425:   // OUNTRY –  DISASSEMBLY
                    case 426:   // COUNTRY -  FULL PROCESS
                        ISOCountryCode(aiElement, aiValue[i], 3);
                        break;

                    // Length 4 Fixed numeric with no checksum.
                    //case 7040:      // UIC + EXT
                    case var po when po >= 3940 && po <= 3949:   // PRCNT OFF
                    case 8111:  // LOYALTY POINTS
                        FixedLengthNumeric(aiElement, aiValue[i], 4);
                        break;

                    // Length 6 Fixed numeric date.
                    case 11:    // PROD DATE
                    case 12:    // DUE DATE
                    case 13:    // PACK DATE
                    case 15:    // BEST BY
                    case 16:    // SELL BY
                    case 17:    // USE BY
                    case 7006:  // FIRST FREEZE DATE
                        FixedLengthDate(aiElement, aiValue[i], true);
                        break;

                    // Length 6 Fix numeric with no checksum.
                    case var cm when cm >= 3100 && cm <= 3169:   // CAP METRIC
                    case var ci when ci >= 3200 && ci <= 3299:   // CAP IMP
                    case var dm when dm >= 3300 && dm <= 3379:   // DIM METRIC
                    case var di when di >= 3400 && di <= 3579:   // DIM IMP
                    case var vi when vi >= 3600 && vi <= 3699:   // VOL IMP
                    case 8005:  // PRICE PER UNIT
                        FixedLengthNumeric(aiElement, aiValue[i], 6);
                        break;

                    // Length 10  6 Fixed numeric date + 4 fixed numeric time.
                    case 7003:  // EXPIRY TIME
                        splitSize = new int[] { 6, 4 };
                        elements = SplitAIElement(aiElement, aiValue[i], 10, splitSize);
                        // Check each sub element individually.
                        FixedLengthDate(elements[0], aiValue[i], false);
                        VariableLengthTime(elements[1], aiValue[i], 6);
                        break;

                    // Length 13 Fixed numeric with checksum.
                    case 410:   // SHIP TO LOC
                    case 411:   // BILL TO
                    case 412:   // PURCHASE FROM
                    case 413:   // SHIP FOR LOC
                    case 414:   // LOC NO
                    case 415:   // PAY TO
                        FixedLengthChecksum(aiElement, aiValue[i], 13);
                        break;

                    // Length 13 Fix numeric with no checksum.
                    case 7001:  // NSN
                        FixedLengthNumeric(aiElement, aiValue[i], 13);
                        break;

                    // Length 14 Fixed numeric with checksum.
                    case 01:    // GTIN
                    case 02:    // CONTENT
                        FixedLengthChecksum(aiElement, aiValue[i], 14);
                        break;

                    // Length 14 Fix numeric with no checksum.
                    case 8001:  // DIMENSIONS
                        FixedLengthNumeric(aiElement, aiValue[i], 14);
                        break;

                    // Length 17 Fixed numeric with checksum.
                    case 402:   // GSIN
                        FixedLengthChecksum(aiElement, aiValue[i], 17);
                        break;

                    // Length 18 Fixed numeric with checksum.
                    case 00:    // SSCC
                    case 8017:  // GSRN PROVIDER
                    case 8018:  // GSRN RECIPIENT
                        FixedLengthChecksum(aiElement, aiValue[i], 18);
                        break;

                    // Length 18  14 Fix numeric with checksum + 4 numeric.
                    case 8006:  // ITIP
                        splitSize = new int[] { 14, 4 };
                        elements = SplitAIElement(aiElement, aiValue[i], 18, splitSize);
                        // Check each sub element individually.
                        FixedLengthChecksum(elements[0], aiValue[i], 14);
                        FixedLengthNumeric(elements[1], aiValue[i], 4, 14);
                        break;

                    // Length 4 Variable numeric.
                    case 7004:  // ACTIVE POTENCY
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 4);
                        break;

                    // Length 6 Variable numeric.
                    case 242:   // MTO VARIANT
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 6);
                        break;

                    // Length 8 Variable numeric.
                    case 30:    // VAR COUNT
                    case 37:    // COUNT
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 8);
                        break;

                    // Length 10 Variable numeric.
                    case 8019: // SRIN
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 10);
                        break;

                    // Length 12 Variable date/time. 6 fixed date + 6 variable time.
                    case 8008: // PROD TIME

                        if (aiElement.Length > 6)
                        {
                            // Date + all or part of time.
                            splitSize = new int[] { 6, aiElement.Length - 6 };
                            elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                            FixedLengthDate(elements[0], aiValue[i], false);
                            VariableLengthTime(elements[1], aiValue[i], 6);
                        }

                        else
                        {
                            // Only date supplied.
                            FixedLengthDate(aiElement, aiValue[i], false);
                        }

                        break;

                    // Length 12 Variable numeric.
                    case 8011: // CPID SERIAL
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 12);
                        break;

                    // Length 12 Variable date.  6 start date + 6 end date.
                    case 7007: // HARVEST DATE
                        // Start date + end date
                        if (aiElement.Length > 6)
                        {
                            splitSize = new int[] { 6, aiElement.Length - 6 };
                            elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                            FixedLengthDate(elements[0], aiValue[i], false);
                            FixedLengthDate(elements[1], aiValue[i], false);
                        }

                        // Start date only.
                        else
                        {
                            FixedLengthDate(aiElement, aiValue[i], false);
                        }

                        break;

                    // Length 15 Variable numeric.
                    case var a when (a >= 3900 && a <= 3909): // AMOUNT
                    case var p when (p >= 3920 && p <= 3929): // PRICE
                        VariableLengthNumeric(aiElement, aiValue[i], 1, 15);
                        break;

                    // Length 18 Fixed/Variable numeric. 3 fixed (ISO417) + 15 variable.
                    case var ia when (ia >= 3910 && ia <= 3919): // ISO AMOUNT
                    case var ip when (ip >= 3930 && ip <= 3939): // ISO PRICE
                        splitSize = new int[] { 3, aiElement.Length - 3 };
                        elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                        ISOCurrencyCode(elements[0], aiValue[i], 3);
                        VariableLengthNumeric(elements[1], aiValue[i], 1, 15, 3);
                        break;

                    // Length 2 Variable alpha numeric.
                    case 7010:  //PROD METHOD
                        VariableLengthC82(aiElement, aiValue[i], 1, 2);
                        break;

                    // Length 3 Variable alpha numeric.
                    case 427:   // ORIGIN SUBDIVISION
                    case 7008:  // AQUATIC SPECIES
                        VariableLengthC82(aiElement, aiValue[i], 1, 3);
                        break;

                    // Length 10 Variable alpha numeric.
                    case 7009:  // FISHING GEAR TYPE
                        VariableLengthC82(aiElement, aiValue[i], 1, 10);
                        break;

                    // Length 12 Variable alpha numeric.
                    case 7005:  // CATCH AREA
                        VariableLengthC82(aiElement, aiValue[i], 1, 12);
                        break;

                    // Length 12 Fixed/Variable alpha numeric. 3 fixed (ISO 3166) + 9 variable.
                    case 421:   // SHIP TO POST
                        splitSize = new int[] { 3, aiElement.Length - 3 };
                        elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                        ISOCountryCode(elements[0], aiValue[i], 3);
                        VariableLengthC82(elements[1], aiValue[i], 1, 9, 3);
                        break;

                    // Length 15 Variable numeric. 1 to 5  ISO 3166 3 digit country codes.
                    case 423:   // COUNTRY – INITIAL PROCESS
                        MultiCountyCodes(aiElement, aiValue[i], 3, 15);
                        break;

                    // Length 20 Variable alpha numeric.
                    case 10:    // BATCH/LOT
                    case 21:    // SERIAL
                    case 243:   // PCN
                    case 254:   // GLN EXTENSION COMPONENT
                    case 420:   // SHIP TO POST
                    case 710:   // NHRN PZN
                    case 711:   // NHRN CIP
                    case 712:   // NHRN CN
                    case 713:   // NHRN DRN
                    case 8002:  // CMT NO
                    case 8012:  // VERSION
                        VariableLengthC82(aiElement, aiValue[i], 1, 20);
                        break;

                    // Length 25 Variable alpha numeric.
                    case 8020:  // REF NO
                        VariableLengthC82(aiElement, aiValue[i], 1, 25);
                        break;

                    // Length 25 Fixed/Variable numeric. 13 fixed with checksum + opt 12 variable numeric.
                    case 255:   // GCN
                        if (aiElement.Length > 13)
                        {
                            splitSize = new int[] { 13, aiElement.Length - 13 };
                            elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                            FixedLengthChecksum(elements[0], aiValue[i], 13);
                            VariableLengthNumeric(elements[1], aiValue[i], 12, 13);
                        }

                        else
                        {
                            FixedLengthChecksum(aiElement, aiValue[i], 13);
                        }

                        break;

                    // Length 30 Variable alpha numeric.
                    case 240:   // ADDITIONAL ID
                    case 241:   // CUST PART NO
                    case 250:   // SECONDARY SERIAL
                    case 251:   // REF TO SOURCE
                    case 400:   // ORDER NUMBER
                    case 401:   // GINC
                    case 403:   // ROUTE
                    case 7002:  // MEAT CUT
                    case 8004:  // GIAI
                    case var intern when intern >= 90 && intern <= 99:   // INTERNAL
                        VariableLengthC82(aiElement, aiValue[i], 1, 30);
                        break;

                    case 8010:  // CPID
                        VariableLengthC39(aiElement, aiValue[i], 4, 30);
                        break;

                    // Length 30 Fixed/Variable alpha numeric. 3 fixed (ISO 3166)+ 27 variable.
                    case var p when (p >= 7030 && p <= 7039):    // PROCESOR # s
                        splitSize = new int[] { 3, aiElement.Length - 3 };
                        elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                        ISOCountryCode(elements[0], aiValue[i], 3);
                        VariableLengthC82(elements[1], aiValue[i], 1, 27, 3);
                        break;

                    // Length 30 Fixed/Variable alpha numeric. 14 fixed with checksum + opt 16 variable.
                    case 8003:   // GRAI
                        if (aiElement.Length > 14)
                        {
                            splitSize = new int[] { 14, aiElement.Length - 14 };
                            elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                            FixedLengthChecksum(elements[0], aiValue[i], 14);
                            VariableLengthC82(elements[1], aiValue[i], 1, 16, 14);
                        }

                        else
                        {
                            FixedLengthChecksum(aiElement, aiValue[i], 14);
                        }

                        break;

                    // Length 30 Fixed/Variable alpha numeric. 13 fixed with checksum + opt 17 variable.
                    case 253:   // GDTI
                        if (aiElement.Length > 13)
                        {
                            splitSize = new int[] { 13, aiElement.Length - 13 };
                            elements = SplitAIElement(aiElement, aiValue[i], aiElement.Length, splitSize);
                            FixedLengthChecksum(elements[0], aiValue[i], 13);
                            VariableLengthC82(elements[1], aiValue[i], 17, 13);
                        }

                        else
                        {
                            FixedLengthChecksum(aiElement, aiValue[i], 13);
                        }

                        break;

                    // Length 34 Variable alpha numeric.
                    case 8007: // IBAN
                        VariableLengthC82(aiElement, aiValue[i], 1, 34);
                        break;

                    // Length 70 Variable alpha numeric.
                    case 8110: // COUPON CODE
                    case 8200: // PRODUCT URL
                        VariableLengthC82(aiElement, aiValue[i], 1, 70);
                        break;

                    default:
                        throw new InvalidDataFormatException(string.Format(CultureInfo.CurrentCulture,
                            "GS1: Invalid Application Identifier [{0:d2}]", aiString));
                }
            }

            bool aiLatch = true;

            for (int i = 0; i < inputLength; i++)
            {
                if (message[i] != '[' && message[i] != ']')
                {
                    bcData.Append(message[i]);
                }

                if (message[i] == '[')
                {
                    // Start of an AI string.
                    if (aiLatch == false)
                    {
                        bcData.Append((char)0x1d);
                    }

                    aiString = new string(message, i + 1, 2);
                    // Resolve AI data.
                    int lastAI = int.Parse(aiString, CultureInfo.CurrentCulture);
                    aiLatch = false;
                    // The following values from GS-1 General Specification version 8.0 issue 2, May 2008
                    // figure 5.4.8.2.1 - 1 Element Strings with Pre-Defined Length Using Application Identifiers.
                    if ((lastAI >= 0 && lastAI <= 4)
                        || (lastAI >= 11 && lastAI <= 20)
                        || lastAI == 23	 // Legacy support - see 5.3.8.2.2.
                        || (lastAI >= 31 && lastAI <= 36)
                        || lastAI == 41)
                    {
                        aiLatch = true;
                    }
                }
                // The '[' & ']' characters are dropped from the input.
            }

            // The character '0x1d' in the returned string refers to the FNC1 character.
            return _ = bcData.ToString().ToCharArray();
        }


        /// <summary>
        /// Parse the barcode message and process any 'tilde' values.
        /// </summary>
        /// <remarks>
        /// Takes the message string and copies it to the barcode character array, processing any 'tilde' values.
        /// 1 digit decimal values ~@ to ~' will translate into decimal values 0 - 32
        /// Or 3 digit decimal values from ~000 to ~255
        /// Suitable for symbols that accept values 0 to 255
        /// </remarks>
        /// <param name="message">Input message.</param>
        /// <returns>Character array holding the message.</returns>
        public static char[] TildeParser(char[] message)
        {
            int inputLength = message.Length;
            StringBuilder bcData = new StringBuilder();

            for (int i = 0; i < inputLength; i++)
            {
                // Has a tilde command fllowed by a single character in the range @ to _.
                // eg. ~M or ~~ for a tilde
                if ((message[i] == '~') && (i < (inputLength - 1)))
                {
                    // Look ahead one character and test it's in the range "@ to _".
                    // If it is, translate to decimal "000 to 031".
                    // If next character is a tilde, treat it as a single tilde and continue.
                    char nextChar = message[i + 1];
                    if (nextChar == '~')
                    {
                        bcData.Append('~');
                        i++;
                    }

                    else if (nextChar > '?' && nextChar < '`')
                    {
                        bcData.Append((char)(nextChar - 64));
                        i++;
                    }

                    // Maybe a 3 character tilde command.
                    else if (i < (inputLength - 3))
                    {
                        // Look ahead 3 characters, try to translate to an integer value.
                        // If sucessful add the ascii value to the message array.
                        // Else treat each character as individual values.
                        int asciiValue;
                        string asciiString = new string(message, i + 1, 3);
                        if (int.TryParse(asciiString, out asciiValue))
                        {
                            if (asciiValue < 256)
                            {
                                bcData.Append((char)asciiValue);
                                i += 3;
                            }

                            else
                            {
                                bcData.Append(message[i]);
                            }
                        }

                        else
                        {
                            bcData.Append(message[i]);
                        }
                    }
                }

                else
                {
                    bcData.Append(message[i]);
                }
            }

            return bcData.ToString().ToCharArray();
        }

        /// <summary>
        /// Returns the message as a character array without any preprocessing or checks.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <returns>Character array holding the message.</returns>
        public static char[] MailmarkParser(char[] message)
        {
            // Ensure postcode type 7 (international) ends in 5 spaces.
            string barcodeMessage = new string(message);
            barcodeMessage = barcodeMessage.ToUpper(CultureInfo.CurrentCulture);
            if (barcodeMessage.Contains("XY11"))
            {
                barcodeMessage = barcodeMessage.TrimEnd(' ');
                barcodeMessage += "     ";
            }

            return barcodeMessage.ToCharArray();
        }

        /// <summary>
        /// Checks an input message for numeric only values.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <returns>Character array holding the message.</returns>
        public static char[] NumericParser(char[] message)
        {
            for (int i = 0; i < message.Length; i++)
            {
                char value = message[i];
                if (!char.IsDigit(value))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "Numeric only data expected.\nInvalid character '{0}' at position {1}.", message[i], i + 1));
                }
            }

            return message;
        }

        /// <summary>
        /// Checks the AI element for numeric only data.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="offSet">Offset from the first element position.</param>
        private static void IsNumeric(string aiElement, int aiValue, int offSet = 0)
        {
            for (int i = 0; i < aiElement.Length; i++)
            {
                if (!char.IsDigit(aiElement[i]))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Numeric only data expected.\nInvalid character '{1}' at position {2}.", aiValue, aiElement[i], offSet + i + 1));
                }
            }
        }

        /// <summary>
        /// Validates the date part of an AI element.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="zeroDay">Decides if the day needs to be specified.</param>
        private static void FixedLengthDate(string aiElement, int aiValue, bool zeroDay)
        {
            int[] daysInMonth = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            bool result;
            string year, month, day;
            int monthDays;

            int century = DateTime.Parse(DateTime.Now.ToString()).Year / 100 * 100;
            int currentYear = DateTime.Now.Year - century;

            if (aiElement.Length != 6)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length for date format 'YYMMDD'.\nExpected 6 numeric characters.", aiValue));
            }

            IsNumeric(aiElement, aiValue);

            year = aiElement.Substring(0, 2);
            result = int.TryParse(year, out int y);
            if (!result || y > 99)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0}]: Invalid value for year in date {1}.", aiValue, aiElement));
            }

            month = aiElement.Substring(2, 2);
            result = int.TryParse(month, out int m);
            if (!result || m < 1 || m > 12)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid value for month in date {1}.", aiValue, aiElement));
            }

            day = aiElement.Substring(4, 2);
            if (day == "00" && !zeroDay)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Day part of date must be specified.", aiValue));
            }

            if (day != "00")    // Day doesn't need to be specified, but filled with '00'.
            {
                result = int.TryParse(day, out int d);
                monthDays = daysInMonth[m];
                if (m == 2)  // Adjust february days for leap years.
                {
                    monthDays = IsLeapYear(century + y) ? daysInMonth[m] + 1 : daysInMonth[m];
                }

                if (!result || d < 1 || d > monthDays)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid value for day in date {1}.", aiValue, aiElement));
                }
            }
        }

        /// <summary>
        /// Validates the time part of an AI element.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void VariableLengthTime(string aiElement, int aiValue, int offSet = 0)
        {
            int count;

            if (aiElement.Length % 2 != 0 || aiElement.Length > 6)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length for time format 'HHMMSS'.\nExpected 2, 4 or 6 numeric characters.", aiValue));
            }

            IsNumeric(aiElement, aiValue, offSet);

            count = aiElement.Length;
            if (count > 0)
            {
                int hour = int.Parse(aiElement.Substring(0, 2));
                if (hour > 23)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AI [{0:d2}]: Invalid value for hour in time {1}.", aiValue, aiElement));

                }

                count -= 2;
            }

            if (count > 0)
            {
                int minute = int.Parse(aiElement.Substring(2, 2));
                if (minute > 59)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AI [{0:d2}]: Invalid value for minutes in time {1}.", aiValue, aiElement));
                }

                count -= 2;
            }

            if (count > 0)
            {
                int second = int.Parse(aiElement.Substring(4, 2));
                if (second > 59)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                            "AI [{0:d2}]: Invalid value for seconds in time {1}.", aiValue, aiElement));
                }
            }
        }

        /// <summary>
        /// Validates a fixed length numeric AI element with checksum.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="length">Fixed length of the AI element.</param>
        private static void FixedLengthChecksum(string aiElement, int aiValue, int length)
        {
            char inChecksum;
            char outChecksum;

            if(aiValue == 8003)
            {
                IsFirstZero(aiElement, aiValue);
            }

            if (aiElement.Length != length)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length.\nExpected {1} numeric characters.", aiValue, length));
            }

            IsNumeric(aiElement, aiValue);

            inChecksum = aiElement[length - 1];
            char[] data = aiElement.ToCharArray(0, length - 1);
            outChecksum = GetCheckDigit.Mod10CheckDigit(data);
            if (outChecksum != inChecksum)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid check digit in AI element.\nExpected '{1}' at Position {2}.", aiValue, outChecksum, length));
            }
        }

        /// <summary>
        /// Validates a fixed length numeric AI element.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number</param>
        /// <param name="length">Fixed length for the element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void FixedLengthNumeric(string aiElement, int aiValue, int length, int offSet = 0)
        {
            if (aiElement.Length != length)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length.\nExpected {1} numeric characters.", aiValue, length));
            }

            IsNumeric(aiElement, aiValue, offSet);
        }

        /// <summary>
        /// Validates a variable length numeric only AI element.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number</param>
        /// <param name="minLength">Minimum expected length for the element.</param>
        /// <param name="maxLength">>Maximum expected length for the element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void VariableLengthNumeric(string aiElement, int aiValue, int minLength, int maxLength, int offSet = 0)
        {
            if (aiElement.Length > maxLength || aiElement.Length < minLength)
            {
                if (offSet > 0)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid length.\nMust be {1} numeric +  {2}...{3} numeric characters.", aiValue, offSet, minLength, maxLength));
                }

                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid length.\nMust be 1...{1} numeric characters.", aiValue, maxLength));
            }

            IsNumeric(aiElement, aiValue, offSet);
        }

        /// <summary>
        /// Checks the AI element only contains characters from the c82 table.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number</param>
        /// <param name="minLength">Minimum expected length for the element.</param>
        /// <param name="maxLength">>Maximum expected length for the element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void VariableLengthC82(string aiElement, int aiValue, int minLength, int maxLength, int offSet = 0)
        {
            if (aiValue == 401 || aiValue == 8004)
            {
                IsFourDigits(aiElement, aiValue);
            }

            if (aiElement.Length > maxLength || aiElement.Length < minLength)
            {
                if (offSet > 0)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid length.\nMust be {1} numeric + {2}...{3} alpha numeric characters.", aiValue, offSet, minLength, maxLength));
                }

                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length.\nMust be {1}...{2} alpha numeric characters.", aiValue, minLength, maxLength));
            }

            for (int i = 0; i < aiElement.Length; i++)
            {
                if (set82.IndexOf(aiElement[i]) == -1)
                {
                    throw new InvalidDataException(String.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}] Invalid character in AI element.\nCharacter '{1}' at Position {2}.", aiValue, aiElement[i], offSet + i + 1));
                }
            }
        }

        /// <summary>
        /// Checks the AI element only contains characters from the c39 table.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number</param>
        /// <param name="minLength">Minimum expected length for the element.</param>
        /// <param name="maxLength">>Maximum expected length for the element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void VariableLengthC39(string aiElement, int aiValue, int minLength, int maxLength, int offSet = 0)
        {
            if (aiValue == 8010)
            {
                IsFourDigits(aiElement, aiValue);
            }

            if (aiElement.Length > maxLength || aiElement.Length < minLength)
            {
                if (offSet > 0)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid length.\nMust be {1} numeric + {2}...{3} alpha numeric characters.", aiValue, offSet, minLength, maxLength));
                }

                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length.\nMust be {1}...{2} alpha numeric characters.", aiValue, minLength, maxLength));
            }

            for (int i = 0; i < aiElement.Length; i++)
            {
                if (set39.IndexOf(aiElement[i]) == -1)
                {
                    throw new InvalidDataException(String.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}] Invalid character in AI element.\nCharacter '{1}' at Position {2}.", aiValue, aiElement[i], offSet + i + 1));
                }
            }
        }

        /// <summary>
        /// Checks a Company Prefix for the first character being '0'.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI value.</param>
        private static void IsFirstZero(string aiElement, int aiValue)
        {
            if (aiElement[0] != '0')
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid Company Prefix.\nFirst digit must be zero.", aiValue));
            }

            IsNumeric(aiElement.Substring(0, 4), aiValue);
        }

        /// <summary>
        /// Checks a Company Prefix for a minmium of 4 numeric characters.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI value.</param>
        private static void IsFourDigits(string aiElement, int aiValue)
        {
            if (aiElement.Length < 4)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid Company Prefix.\nExpected at least 4 numeric characters.", aiValue));
            }

            IsNumeric(aiElement.Substring(0, 4), aiValue);
        }

        /// <summary>
        /// Splits an AI element into sub elements.
        /// </summary>
        /// <param name="aiElement">Element associated with the AI.</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="length">Fixed length of the AI element.</param>
        /// <param name="splitSizes">Array holding the sizes of the sub elements.</param>
        /// <returns>A string array containing the split element.</returns>
        private static string[] SplitAIElement(string aiElement, int aiValue, int length, int[] splitSizes)
        {
            int offset = 0;
            int numberOfSplits = splitSizes.Length;
            string[] elements = new string[numberOfSplits];

            if (aiElement.Length == length)
            {
                for (int i = 0; i < numberOfSplits; i++)
                {
                    elements[i] = aiElement.Substring(offset, splitSizes[i]);
                    offset += splitSizes[i];
                }

                return elements;
            }

            throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                "AI [{0:d2}]: Invalid length.\nExpected {1} numeric characters.", aiValue, length));
        }

        /// <summary>
        /// Validates a ISO4217 (Numeric) currency code.
        /// </summary>
        /// <param name="aiElement">currency code as a  3 character string</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="length">Fixed length of the AI element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void ISOCurrencyCode(string aiElement, int aiValue, int length, int offSet = 0)
        {
            // Check the length.
            FixedLengthNumeric(aiElement, aiValue, length, offSet);
            // Get the currency code.
            int code = int.Parse(aiElement);
            if(!ISO4217.ISO4217Numeric(code))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid currency code {1}.", aiValue, code));
            }
        }

        /// <summary>
        /// Validates a ISO3166 (Numeric) country code.
        /// </summary>
        /// <param name="aiElement">currency code as a  3 character string</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="length">Fixed length of the AI element.</param>
        /// <param name="offSet">Offset from the begining of the AI element string.</param>
        private static void ISOCountryCode(string aiElement, int aiValue, int length, int offSet = 0)
        {
            // Check the length.
            FixedLengthNumeric(aiElement, aiValue, length, offSet);
            // Get the currency code.
            int code = int.Parse(aiElement);
            if (!ISO3166.ISO3166Numeric(code))
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid country code {1}.", aiValue, code));
            }
        }

        /// <summary>
        /// Validates multiple country codes in an AI.
        /// </summary>
        /// <param name="aiElement">The AI element to test.</param>
        /// <param name="aiValue">AI number.</param>
        /// <param name="minLength">Minimum expected length of the AI element.</param>
        /// <param name="maxLength">Maximum expected length of the AI element.</param>
        private static void MultiCountyCodes(string aiElement, int aiValue, int minLength, int maxLength)
        {
            int aiLength = aiElement.Length;
            
            // Check the length of the country code list is a multiple of 3.
            if (aiLength % 3 != 0)
            {
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "AI [{0:d2}]: Invalid length of {1}.\nExpected mulpitles of 3 numeric characters.", aiValue, aiLength));
            } 

            VariableLengthNumeric(aiElement, aiValue, minLength, maxLength);
            int index = 0;
            int count = aiLength / 3;
            for (int i = 0; i < count; i++)
            {
                int code = int.Parse(aiElement.Substring(index, 3));
                if (!ISO3166.ISO3166Numeric(code))
                {
                    throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                        "AI [{0:d2}]: Invalid country code {1} at postion {2}.", aiValue, code, i * 3));
                }

                index += 3;
            }
        }

        /// <summary>
        /// Tests for a leap year.
        /// </summary>
        /// <param name="year">Year to test.</param>
        /// <returns>True if the year is a leap year.</returns>
        private static bool IsLeapYear(int year)
        {
            if (year > 0 && year < 9999)
            {
                if (DateTime.IsLeapYear(year))
                {
                    return true;
                }
            }

            return false;
        }
    }
}