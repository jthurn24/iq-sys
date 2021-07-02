using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQI.Intuition.Domain.Utilities
{
    public class UrlEncodeHelper
    {

        public static string UrlEncode(string str)
        {
            if (str == null)
                return (string)null;
            else
                return UrlEncodeHelper.UrlEncode(str, Encoding.UTF8);
        }

        private static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return (string)null;
            else
                return Encoding.ASCII.GetString(UrlEncodeHelper.UrlEncodeToBytes(str, e));
        }

        private static byte[] UrlEncodeToBytes(string str)
        {
            if (str == null)
                return (byte[])null;
            else
                return UrlEncodeHelper.UrlEncodeToBytes(str, Encoding.UTF8);
        }

        private static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return (byte[])null;
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeHelper.UrlEncode(bytes, 0, bytes.Length, false);
        }

        private static byte[] UrlEncode(byte[] bytes, int offset, int count, bool alwaysCreateNewReturnValue)
        {
            byte[] numArray = UrlEncodeHelper.UrlEncode(bytes, offset, count);
            if (!alwaysCreateNewReturnValue || numArray == null || numArray != bytes)
                return numArray;
            else
                return (byte[])numArray.Clone();
        }

        private static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (!HttpEncoderUtility.ValidateUrlEncodingParameters(bytes, offset, count))
                return (byte[])null;
            int num1 = 0;
            int num2 = 0;
            for (int index = 0; index < count; ++index)
            {
                char ch = (char)bytes[offset + index];
                if ((int)ch == 32)
                    ++num1;
                else if (!HttpEncoderUtility.IsUrlSafeChar(ch))
                    ++num2;
            }
            if (num1 == 0 && num2 == 0)
                return bytes;
            byte[] numArray1 = new byte[count + num2 * 2];
            int num3 = 0;
            for (int index1 = 0; index1 < count; ++index1)
            {
                byte num4 = bytes[offset + index1];
                char ch = (char)num4;
                if (HttpEncoderUtility.IsUrlSafeChar(ch))
                    numArray1[num3++] = num4;
                else if ((int)ch == 32)
                {
                    numArray1[num3++] = (byte)43;
                }
                else
                {
                    byte[] numArray2 = numArray1;
                    int index2 = num3;
                    int num5 = 1;
                    int num6 = index2 + num5;
                    int num7 = 37;
                    numArray2[index2] = (byte)num7;
                    byte[] numArray3 = numArray1;
                    int index3 = num6;
                    int num8 = 1;
                    int num9 = index3 + num8;
                    int num10 = (int)(byte)HttpEncoderUtility.IntToHex((int)num4 >> 4 & 15);
                    numArray3[index3] = (byte)num10;
                    byte[] numArray4 = numArray1;
                    int index4 = num9;
                    int num11 = 1;
                    num3 = index4 + num11;
                    int num12 = (int)(byte)HttpEncoderUtility.IntToHex((int)num4 & 15);
                    numArray4[index4] = (byte)num12;
                }
            }
            return numArray1;
        }

        public static string UrlDecode(string str)
        {
            if (str == null)
                return (string)null;
            else
                return UrlEncodeHelper.UrlDecode(str, Encoding.UTF8);
        }

        private static string UrlDecode(string value, Encoding encoding)
        {
            if (value == null)
                return (string)null;
            int length = value.Length;
            UrlEncodeHelper.UrlDecoder urlDecoder = new UrlEncodeHelper.UrlDecoder(length, encoding);
            for (int index = 0; index < length; ++index)
            {
                char ch1 = value[index];
                switch (ch1)
                {
                    case '+':
                        ch1 = ' ';
                        goto default;
                    case '%':
                        if (index < length - 2)
                        {
                            if ((int)value[index + 1] == 117 && index < length - 5)
                            {
                                int num1 = HttpEncoderUtility.HexToInt(value[index + 2]);
                                int num2 = HttpEncoderUtility.HexToInt(value[index + 3]);
                                int num3 = HttpEncoderUtility.HexToInt(value[index + 4]);
                                int num4 = HttpEncoderUtility.HexToInt(value[index + 5]);
                                if (num1 >= 0 && num2 >= 0 && (num3 >= 0 && num4 >= 0))
                                {
                                    char ch2 = (char)(num1 << 12 | num2 << 8 | num3 << 4 | num4);
                                    index += 5;
                                    urlDecoder.AddChar(ch2);
                                    break;
                                }
                                else
                                    goto default;
                            }
                            else
                            {
                                int num1 = HttpEncoderUtility.HexToInt(value[index + 1]);
                                int num2 = HttpEncoderUtility.HexToInt(value[index + 2]);
                                if (num1 >= 0 && num2 >= 0)
                                {
                                    byte b = (byte)(num1 << 4 | num2);
                                    index += 2;
                                    urlDecoder.AddByte(b);
                                    break;
                                }
                                else
                                    goto default;
                            }
                        }
                        else
                            goto default;
                    default:
                        if (((int)ch1 & 65408) == 0)
                        {
                            urlDecoder.AddByte((byte)ch1);
                            break;
                        }
                        else
                        {
                            urlDecoder.AddChar(ch1);
                            break;
                        }
                }
            }
            return urlDecoder.GetString();
        }

        private class UrlDecoder
        {
            private int _bufferSize;
            private int _numChars;
            private char[] _charBuffer;
            private int _numBytes;
            private byte[] _byteBuffer;
            private Encoding _encoding;

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                this._bufferSize = bufferSize;
                this._encoding = encoding;
                this._charBuffer = new char[bufferSize];
            }

            private void FlushBytes()
            {
                if (this._numBytes <= 0)
                    return;
                this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                this._numBytes = 0;
            }

            internal void AddChar(char ch)
            {
                if (this._numBytes > 0)
                    this.FlushBytes();
                this._charBuffer[this._numChars++] = ch;
            }

            internal void AddByte(byte b)
            {
                if (this._byteBuffer == null)
                    this._byteBuffer = new byte[this._bufferSize];
                this._byteBuffer[this._numBytes++] = b;
            }

            internal string GetString()
            {
                if (this._numBytes > 0)
                    this.FlushBytes();
                if (this._numChars > 0)
                    return new string(this._charBuffer, 0, this._numChars);
                else
                    return string.Empty;
            }
        }

        private static class HttpEncoderUtility
        {
            public static int HexToInt(char h)
            {
                if ((int)h >= 48 && (int)h <= 57)
                    return (int)h - 48;
                if ((int)h >= 97 && (int)h <= 102)
                    return (int)h - 97 + 10;
                if ((int)h < 65 || (int)h > 70)
                    return -1;
                else
                    return (int)h - 65 + 10;
            }

            public static char IntToHex(int n)
            {
                if (n <= 9)
                    return (char)(n + 48);
                else
                    return (char)(n - 10 + 97);
            }

            public static bool IsUrlSafeChar(char ch)
            {
                if ((int)ch >= 97 && (int)ch <= 122 || (int)ch >= 65 && (int)ch <= 90 || (int)ch >= 48 && (int)ch <= 57)
                    return true;
                switch (ch)
                {
                    case '!':
                    case '(':
                    case ')':
                    case '*':
                    case '-':
                    case '.':
                    case '_':
                        return true;
                    default:
                        return false;
                }
            }

            internal static string UrlEncodeSpaces(string str)
            {
                if (str != null && str.IndexOf(' ') >= 0)
                    str = str.Replace(" ", "%20");
                return str;
            }

            public static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
            {
                if (bytes == null && count == 0)
                    return false;
                if (bytes == null)
                    throw new ArgumentNullException("bytes");
                if (offset < 0 || offset > bytes.Length)
                    throw new ArgumentOutOfRangeException("offset");
                if (count < 0 || offset + count > bytes.Length)
                    throw new ArgumentOutOfRangeException("count");
                else
                    return true;
            }
        }
    }
}
