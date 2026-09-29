/* ArrayHelper.cs - System.Array helper methods. */

/*
    Copyright (C) 2013-2020 Milton Neal <milton200954@gmail.com>

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
    static internal class ArrayHelper
    {
        /// <summary>
        /// Converts the specified character array to upper case.
        /// </summary>
        /// <param name="data">input array</param>
        /// <returns>converted array</returns>
        public static char[] ToUpper(char[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] > 96) && (data[i] < 123))
                {
                    data[i] -= (char)32;
                }
            }

            return data;
        }

        /// <summary>
        /// Converts the specified character array to lower case.
        /// </summary>
        /// <param name="data">input array</param>
        /// <returns>converted array</returns>
        public static char[] ToLower(char[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] > 64) && (data[i] < 91))
                {
                    data[i] += (char)32;
                }
            }

            return data;
        }
        /// <summary>
        /// Inserts a character into the existing array at a specified index.
        /// </summary>
        /// <param name="data">array to insert the data</param>
        /// <param name="index">index to insert</param>
        /// <param name="value">character to insert</param>
        /// <returns>the modified array</returns>
        public static char[] Insert(char[] data, int index, char value)
        {
            int length = data.Length + 1;

            if (index > length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Array.Resize(ref data, length);
            for (int i = length - 1; i > index; i--)
            {
                data[i] = data[i - 1];
            }

            data[index] = value;
            return data;
        }

        /// <summary>
        /// Inserts a character array into the exiting array at a specified index.
        /// </summary>
        /// <param name="data">array to insert the data</param>
        /// <param name="index">index to insert</param>
        /// <param name="value">array to insert</param>
        /// <returns>the modified array</returns>
        public static char[] Insert(char[] data, int index, char[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                data = Insert(data, index, value[i]);
                index = data.Length;
            }

            return data;
        }

        /// <summary>
        /// Inserts a string into the existing character array at a specified index.
        /// </summary>
        /// <param name="data">array to insert the data</param>
        /// <param name="index">index to insert the byte</param>
        /// <param name="value">string to insert</param>
        /// <returns>the modified array</returns>
        public static char[] Insert(char[] data, int index, string value)
        {
            int valueLength = value.Length;
            int dataLength = data.Length;
            if (dataLength == 0)
            {
                index = 0;
            }

            if (value.Length != 0)
            {
                Array.Resize(ref data, data.Length + value.Length);

                for (int i = dataLength; i > index; i--)
                {
                    data[i + valueLength - 1] = data[i - 1];
                }

                for (int i = 0; i < value.Length; i++)
                {
                    data[index + i] = value[i];
                }
            }

            return data;
        }

        /// <summary>
        /// Inserts a character into the existing array at a specified index.
        /// </summary>
        /// <param name="data">array to insert the data</param>
        /// <param name="index">index to insert</param>
        /// <param name="value">character to insert</param>
        /// <returns>the modified array</returns>
        public static int[] Insert(int[] data, int index, int value)
        {
            int length = data.Length + 1;

            if (index > length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Array.Resize(ref data, length);
            for (int i = length - 1; i > index; i--)
            {
                data[i] = data[i - 1];
            }

            data[index] = value;
            return data;
        }


        /// <summary>
        /// Removes the character at the specified index from the array.
        /// </summary>
        /// <param name="data">array to remove the character from.</param>
        /// <param name="index">the index that pointers to the character to be removed.</param>
        /// <returns>the modified array</returns>

        // Removes the character at index from the array.
        public static char[] Remove(char[] data, int index)
        {
            int length = data.Length;

            if (index > length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            for (int i = index; i < length - 1; i++)
            {
                data[i] = data[i + 1];
            }

            Array.Resize(ref data, length - 1);
            return data;
        }

        /// <summary>
        /// Reports a zero based index of the first occurrance of the specified character.
        /// </summary>
        /// <param name="data">Character array to search.</param>
        /// <param name="value">Character to look for.</param>
        /// <returns>Zero based index of the character or -1 if not found.</returns>
        public static int IndexOf(char[] data, char value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
