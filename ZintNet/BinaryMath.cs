/* BinaryMath.cs - Handles larger math calculations.

/* 
    ZintNetLib - a C# port of libzint.
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>
    Acknowledgments to Robin Stuart and other Zint Authors and Contributors.
  
    libzint - the open source barcode library
    Copyright (C) 2009-2020 Robin Stuart <rstuart114@gmail.com>

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

namespace ZintNet
{
    /// <summary>
    /// Class for binary math calculations on large numbers.
    /// </summary>
    internal static class BinaryMath
    {
        /// <summary>
        /// Binary addition.
        /// </summary>
        /// <param name="accumulator">result</param>
        /// <param name="register">operand</param>
        public static void BinaryAdd(short[] accumulator, short[] register)
        {
            int carry = 0;
            bool done;

            for (int i = 0; i < 112; i++)
            {
                done = false;
                if (((register[i] == 0) && (accumulator[i] == 0)) && ((carry == 0) && !done))
                {
                    accumulator[i] = 0;
                    carry = 0;
                    done = true;
                }

                if (((register[i] == 0) && (accumulator[i] == 0)) && ((carry == 1) && !done))
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }

                if (((register[i] == 0) && (accumulator[i] == 1)) && ((carry == 0) && !done))
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }

                if (((register[i] == 0) && (accumulator[i] == 1)) && ((carry == 1) && !done))
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }

                if (((register[i] == 1) && (accumulator[i] == 0)) && ((carry == 0) && !done))
                {
                    accumulator[i] = 1;
                    carry = 0;
                    done = true;
                }

                if (((register[i] == 1) && (accumulator[i] == 0)) && ((carry == 1) && !done))
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }

                if (((register[i] == 1) && (accumulator[i] == 1)) && ((carry == 0) && !done))
                {
                    accumulator[i] = 0;
                    carry = 1;
                    done = true;
                }

                if (((register[i] == 1) && (accumulator[i] == 1)) && ((carry == 1) && !done))
                {
                    accumulator[i] = 1;
                    carry = 1;
                    done = true;
                }
            }
        }

        /// <summary>
        /// Binary subtraction.
        /// </summary>
        /// <param name="accumulator">result</param>
        /// <param name="register">operand</param>
        public static void BinarySubtract(short[] accumulator, short[] register)
        {
            // 2's compliment subtraction.
            // Subtract buffer from accumulator and put answer in accumulator.
            short[] subtractBuffer = new short[112];

            for (int i = 0; i < 112; i++)
            {
                if (register[i] == 0)
                    subtractBuffer[i] = 1;

                else
                    subtractBuffer[i] = 0;
            }

            BinaryAdd(accumulator, subtractBuffer);
            subtractBuffer[0] = 1;

            for (int i = 1; i < 112; i++)
                subtractBuffer[i] = 0;

            BinaryAdd(accumulator, subtractBuffer);
        }

        /// <summary>
        /// Binary multipication.
        /// </summary>
        /// <param name="register">contents to be multiplied</param>
        /// <param name="data">number to multiply by</param>
        public static void BinaryMultiply(short[] register, string data)
        {
            short[] temporary = new short[112];
            short[] accumulator = new short[112];

            BinaryLoad(temporary, data);
            for(int i = 0; i < 102; i++)
            {
                if (temporary[i] == 1)
                    BinaryAdd(accumulator, register);

                ShiftUp(register);
            }

            Array.Copy(accumulator, register, 112);
        }

        /// <summary>
        /// Perform a binary shift left
        /// </summary>
        /// <param name="buffer">operand buffer</param>
        public static void ShiftDown(short[] buffer)
        {
            buffer[102] = 0;
            buffer[103] = 0;

            for (int i = 0; i < 102; i++)
                buffer[i] = buffer[i + 1];
        }

        /// <summary>
        /// Perform a binary shift right.
        /// </summary>
        /// <param name="buffer">operand buffer</param>
        public static void ShiftUp(short[] buffer)
        {
            for (int i = 102; i > 0; i--)
                buffer[i] = buffer[i - 1];

            buffer[0] = 0;
        }

        /// <summary>
        /// Performs a binary comparison between two values.
        /// </summary>
        /// <param name="accumulator">accumulator value for comparison</param>
        /// <param name="register">register value for comparison</param>
        /// <returns>1 if accumulator is larger than register, else 0</returns>
        public static short IsLarger(short[] accumulator, short[] register)
        {
            bool latch = false;
            int index = 103;
            short larger = 0;

            do
            {
                if ((accumulator[index] == 1) && (register[index] == 0))
                {
                    latch = true;
                    larger = 1;
                }

                if ((accumulator[index] == 0) && (register[index] == 1))
                    latch = true;

                index--;
            } while ((latch == false) && (index >= 0));

            return larger;
        }

        /// <summary>
        /// Convert a numeric string to a binary value.
        /// </summary>
        /// <param name="register">result</param>
        /// <param name="data">string to store as a binary value</param>
        public static void BinaryLoad(short[] register, string data)
        {
            int length = data.Length;
            short[] tempBuffer = new short[112];

            for(int i = 0; i < length; i++)
            {
                if(!Char.IsDigit(data[i]))
                    throw new ArgumentException("Data is not a numeric string value.");
            }

            Array.Clear(register, 0, 112);
            for (int index = 0; index < length; index++)
            {
                Array.Copy(register, tempBuffer, 112);

                for (int i = 0; i < 9; i++)
                    BinaryAdd(register, tempBuffer);

                Array.Clear(tempBuffer, 0, 112);

                for (int i = 0; i < 4; i++)
                {
                    if ((data[index] - '0' & (0x01 << i)) > 0)
                        tempBuffer[i] = 1;
                }

                BinaryAdd(register, tempBuffer);
            }
        }
    }
}
