/* ISO3166.cs Tests for ISO 3166 numeric, ISO 3166 alpha 2  */

/*
    ZintNetLib - a C# implementation of libzint library.
    Copyright (C) 2013-2025 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2009-2025 Robin Stuart <rstuart114@gmail.com>

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
namespace ZintNet
{
    internal static class ISO3166
    {
        // ISO 3166-1 numeric.
        public static bool ISO3166Numeric(int cc)
        {
            byte[] codes = new byte[112] {
                0x10, 0x15, 0x11, 0x91, 0x11, 0x11, 0x1D, 0x11,
                0x51, 0x15, 0x50, 0x14, 0x11, 0x11, 0x11, 0x11,
                0x10, 0x11, 0x11, 0x51, 0x44, 0xC4, 0x14, 0x91,
                0x11, 0x18, 0x51, 0x44, 0x84, 0xC7, 0x44, 0x45,
                0x54, 0x54, 0x18, 0x00, 0x11, 0x11, 0x11, 0x11,
                0x11, 0x51, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11,
                0x11, 0x41, 0x11, 0x45, 0x46, 0x54, 0x44, 0x45,
                0x44, 0x44, 0x44, 0x44, 0x11, 0x10, 0x1D, 0x11,
                0x11, 0x11, 0xE9, 0x10, 0x10, 0x44, 0x44, 0x44,
                0xB4, 0x87, 0x40, 0x11, 0x11, 0x11, 0x45, 0x44,
                0x4C, 0x50, 0xD8, 0x44, 0x44, 0x44, 0x45, 0xC0,
                0x47, 0x10, 0x10, 0x13, 0x10, 0x11, 0x11, 0x15,
                0x11, 0x11, 0x11, 0x59, 0x91, 0x00, 0x04, 0x84,
                0x07, 0x01, 0x44, 0x54, 0x00, 0x10, 0x84, 0x40 };

            int b = cc >> 3;

            if (b < 0 || b >= 112)
            {
                return false;
            }

            return (codes[b] & (1 << (cc & 0x7))) > 0;
        }

        // ISO 3166-1 alpha2.
        public static bool ISO3166Alpha2(char[] cc)
        {
            byte[] codes = new byte[85] {
                0x78, 0x59, 0xDF, 0xEE, 0xEF, 0xBD, 0xDD, 0xDE,
                0x27, 0x3F, 0x84, 0x15, 0x80, 0xD4, 0x00, 0x0E,
                0x00, 0x5C, 0x09, 0xB0, 0x9F, 0xFB, 0x15, 0x00,
                0x8D, 0x06, 0x18, 0x78, 0x0F, 0x40, 0x40, 0x03,
                0x00, 0x1D, 0x2B, 0xF4, 0x41, 0x81, 0x4F, 0xFD,
                0xFC, 0xFF, 0xD7, 0x25, 0x4B, 0x08, 0x00, 0x01,
                0x40, 0x3C, 0x8F, 0x53, 0x01, 0x00, 0x00, 0x40,
                0x00, 0x51, 0xF1, 0xFD, 0xE7, 0x3A, 0xBB, 0x9F,
                0x9A, 0x41, 0x10, 0x04, 0x57, 0x85, 0x40, 0x00,
                0x02, 0x40, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
                0x08, 0x04, 0x40, 0x00, 0x01 };

            int ccInt;

            if (cc[0] < 'A' || cc[0] > 'Z' || cc[1] < 'A' || cc[1] > 'Z')
            {
                return false;
            }

            ccInt = (cc[0] - 'A') * 26 + (cc[1] - 'A');
            return ((codes[ccInt >> 3]) & (1 << (ccInt & 0x7))) > 0;
        }
    }
}
