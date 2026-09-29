/* ISO4217.cs Tests for ISO 4217-1 numeric. */

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
    internal static class ISO4217
    {
        public static bool ISO4217Numeric(int cc)
        {
            byte[] codes = new byte[125] {
                0x00, 0x11, 0x00, 0x00, 0x11, 0x10, 0x1D, 0x10,
                0x11, 0x01, 0x10, 0x04, 0x01, 0x11, 0x10, 0x10,
                0x10, 0x01, 0x01, 0x11, 0x00, 0x44, 0x00, 0x10,
                0x01, 0x08, 0x41, 0x40, 0x40, 0x41, 0x04, 0x00,
                0x40, 0x40, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
                0x11, 0x11, 0x10, 0x11, 0x11, 0x11, 0x01, 0x01,
                0x10, 0x41, 0x11, 0x45, 0x46, 0x44, 0x04, 0x40,
                0x40, 0x44, 0x00, 0x00, 0x11, 0x00, 0x05, 0x01,
                0x11, 0x10, 0x30, 0x00, 0x10, 0x44, 0x40, 0x00,
                0x04, 0x44, 0x40, 0x11, 0x01, 0x00, 0x00, 0x04,
                0x48, 0x40, 0x00, 0x00, 0x00, 0x04, 0x04, 0x40,
                0x45, 0x00, 0x00, 0x01, 0x00, 0x10, 0x11, 0x11,
                0x00, 0x11, 0x11, 0x00, 0x81, 0x00, 0x04, 0x04,
                0x04, 0x01, 0x00, 0x14, 0x00, 0x00, 0x44, 0x00,
                0x20, 0x00, 0x00, 0xF0, 0x67, 0xB5, 0xFD, 0xFB,
                0xBF, 0xBF, 0x3F, 0x47, 0xA4 };

            int b = cc >> 3;

            if (b < 0 || b >= 125)
            {
                return false;
            }

            return (codes[b] & (1 << (cc & 0x7))) > 0;
        }
    }
}
