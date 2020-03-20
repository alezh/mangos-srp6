using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SRP
{
    public static class SrpHash
    {    
        public static byte[] HashEncode(this byte[] key) {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] sha = sha1.ComputeHash(key);
            sha1.Dispose();
            return sha;
        }

        private static byte MaskBits(byte b, int bitCount)
        {
            if (bitCount == 7)
                return (byte)(b & 1);
            if (bitCount == 6)
                return (byte)(b & 3);
            if (bitCount == 5)
                return (byte)(b & 7);
            if (bitCount == 4)
                return (byte)(b & 15);
            if (bitCount == 3)
                return (byte)(b & 31);
            if (bitCount == 2)
                return (byte)(b & 63);
            if (bitCount == 1)
                return (byte)(b & 127);
            throw new ArgumentException("bitCount is not from 1-7");
        }

        public static BigInteger CreateBigInteger(this string text, int convertBase)
        {
            if (convertBase == 16)
            {
                var isEvenLength = (text.Length % 2 == 0);
                var length = (text.Length / 2) + (isEvenLength ? 0 : 1);
                var result = new byte[length + 1];
                for (var i = 0; i < length; i++)
                {
                    var j = text.Length - (i * 2) - 1;
                    var ch1 = '0';
                    if (j > 0)
                        ch1 = text[j - 1];
                    var b = GetHexadecimalByte(ch1, text[j]);
                    result[i] = b;
                }
                result[length] = 0;
                return new BigInteger(result);
            }
            if (convertBase == 10)
                return BigInteger.Parse(text);
            throw new Exception("Unsupported conversion base");
        }

        public static BigInteger CreateBigInteger(int numberOfBits, Random random)
        {
            var numberOfFullBytes = numberOfBits / 8;
            var numberOfRemainingBits = numberOfBits % 8;
            var numberOfBytes = numberOfFullBytes + (numberOfRemainingBits > 0 ? 1 : 0);

            // This retry loop was added to handle weird exceptions from Mono's version of the BigInteger class
            // ToDo: Verify exactly what is causing this problem
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    var randomBytes = new byte[numberOfBytes];
                    random.NextBytes(randomBytes);

                    // Mask off unused bits
                    if (numberOfRemainingBits > 0)
                        randomBytes[randomBytes.Length - 1] =
                            MaskBits(randomBytes[randomBytes.Length - 1], 8 - numberOfRemainingBits);

                    // Make unsigned and return result
                    return new BigInteger(randomBytes).MakePositive();
                }
                catch (Exception)
                {
                    // If it's the last retry iteration, just throw the exception
                    if (i == 2)
                        throw;
                }
            }

            // This should be unreachable code
            throw new Exception("Should not happen");
        }

        private static BigInteger MakePositive(this BigInteger inValue)
        {
            if (inValue < 0)
            {
                byte[] oldBytes = inValue.ToByteArray();
                var newBytes = new byte[oldBytes.Length + 1];
                for (int i = 0; i < oldBytes.Length; i++)
                    newBytes[i] = oldBytes[i];
                newBytes[oldBytes.Length] = 0;
                return new BigInteger(newBytes);
            }
            return inValue;
        }

        private static byte GetHexadecimalByte(char c1, char c2)
        {
            byte upperByte = HexadecimalCharToByte(c1);
            byte lowByte = HexadecimalCharToByte(c2);
            upperByte = (byte)(upperByte << 4);
            return (byte)(upperByte | lowByte);
        }

        /// <summary>
        /// Returns the hexadecimal byte value that the specified character represents.
        /// </summary>
        private static byte HexadecimalCharToByte(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 12;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
            }
            throw new FormatException("Invalid hexadecimal character");
        }


        public static string ToHexString(this BigInteger bigInt)
        {
            return bigInt.ToUnsignedByteArray().ToHexString();
        }

        /// <summary>
        /// Returns a byte array for what the unsigned value of bigInt would be.
        /// </summary>
        public static byte[] ToUnsignedByteArray(this BigInteger bigInt)
        {
            byte[] bigIntByteArray = bigInt.ToByteArray();
            if (bigIntByteArray[bigIntByteArray.Length - 1] == 0)
            {
                var shortenedByteArray = new byte[bigIntByteArray.Length - 1];
                for (int i = 0; i < shortenedByteArray.Length; i++)
                {
                    shortenedByteArray[i] = bigIntByteArray[i];
                }
                bigIntByteArray = shortenedByteArray;
            }
            return bigIntByteArray;
        }

        public static string ToHexString(this byte[] s)
        {
            StringBuilder sub = new StringBuilder();
            foreach (var x in s)
            {
                sub.Append(x.ToString("X2"));
            }
            return sub.ToString();
        }

        public static byte[] Concatenate(params byte[][] arrays)
        {
            var length = arrays.Where(array => array != null).Sum(array => array.Length);
            var result = new byte[length];
            length = 0;
            foreach (var array in arrays.Where(array => array != null))
            {
                Array.Copy(array, 0, result, length, array.Length);
                length += array.Length;
            }
            return result;
        }
        
        public static BigInteger ModPow(this BigInteger bigInt, BigInteger exponent,
            BigInteger modulus)
        {
            return BigInteger.ModPow(bigInt, exponent, modulus);
        }
    }
}
