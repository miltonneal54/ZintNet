
namespace ZintNet
{
    internal static class Common
    {
        public static bool IsTwoDigits(char[] source, int length, int position)
        {
            return position + 1 < length && char.IsDigit(source[position]) && char.IsDigit(source[position + 1]);
        }

        // Converts a decimal character array into it's integer value. Returns -1 if not numeric.
        public static int ToInt(char[] source, int position, int length)
        {
            if( int.TryParse(new string(source, position, length), out int value))
            {
                return value;
            }

            else
            {
                return -1;
            }
        }

        // Returns how many consecutive digits lie immediately ahead (Annex F.II.A)
        public static int NumberOfDigits(char[] source, int position, int length)
        {
            int i;
            for (i = position; (i < length) && char.IsDigit(source[i]); i++)
            {
                ;
            }

            return i - position;
        }

        public static bool IsNumeric(char[] source, int length, int position)
        {
            for (int i = position; i <length; i++)
            {
                if (!char.IsDigit(source[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /* Converts a character 0-9, A-F to its equivalent integer value */
        /*public static int CharToInt(char source)
        {
            if (char.IsDigit(source))
            {
                return (source - '0');
            }

            if ((source >= 'A') && (source <= 'F'))
            {
                return (source - 'A' + 10);
            }

            if ((source >= 'a') && (source <= 'f'))
            {
                return (source - 'a' + 10);
            }

            return -1;
        }*/

        // Returns the number of times a character occurs in "source".
        public static int CharCount(char[] source, int length, char c)
        {
            int count = 0;

            for (int i = 0; i < length; i++)
            {
                if (source[i] == c)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
