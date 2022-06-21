// Copyright 2021 Maple512 and Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace System;
using Shouldly;
using Xunit;

public class StringExtensions_Test
{
    [Fact]
    public void To_Camel_Case()
    {
        nameof(StringExtensions_Test).ToCamelCase().ShouldBeEquivalentTo("stringExtensions_Test");

        "aBCD".ToCamelCase().ShouldBeEquivalentTo("aBCD");

        "ABCD".ToCamelCase().ShouldBeEquivalentTo("abcd");
    }

    [Fact]
    public void To_Snake_Case()
    {
        nameof(StringExtensions_Test).ToSnakeCase().ShouldBeEquivalentTo("string_extensions_test");

        // 无小写字幕的，直接全部转小写
        "abcd".ToSnakeCase().ShouldBeEquivalentTo("abcd");

        "ABCD".ToSnakeCase().ShouldBeEquivalentTo("abcd");

        "aBCD".ToSnakeCase().ShouldBeEquivalentTo("a_bcd");
    }

    //// base32: https://gist.github.com/erdomke/9335c394c5cc65404c4cf9aceab04143
    //// base64: https://zhuanlan.zhihu.com/p/339477329
    //public static string ToBase64String(ReadOnlySpan<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
    //{
    //    if(options is < Base64FormattingOptions.None or > Base64FormattingOptions.InsertLineBreaks)
    //    {
    //        throw new ArgumentException("");
    //    }

    //    if(bytes.Length == 0)
    //    {
    //        return string.Empty;
    //    }

    //    var insertLineBreaks = options == Base64FormattingOptions.InsertLineBreaks;
    //    //string result = string.Create(ToBase64_CalculateAndValidateOutputLength(bytes.Length, insertLineBreaks));
    //    string? result = null;
    //    unsafe
    //    {
    //        fixed(byte* bytesPtr = &MemoryMarshal.GetReference(bytes))
    //        fixed(char* charsPtr = result)
    //        {
    //            var charsWritten = ConvertToBase64Array(charsPtr, bytesPtr, 0, bytes.Length, insertLineBreaks);
    //            Debug.Assert(result.Length == charsWritten, $"Expected {result.Length} == {charsWritten}");
    //        }
    //    }

    //    return result;
    //}

    //private const int base64LineBreakPosition = 76;

    //private static int ToBase64_CalculateAndValidateOutputLength(int inputLength, bool insertLineBreaks)
    //{
    //    // the base length - we want integer division here, at most 4 more chars for the remainder
    //    var outlen = ((uint)inputLength + 2) / 3 * 4;

    //    if(outlen == 0)
    //    {
    //        return 0;
    //    }

    //    if(insertLineBreaks)
    //    {
    //        (var newLines, var remainder) = Math.DivRem(outlen, base64LineBreakPosition);
    //        if(remainder == 0)
    //        {
    //            --newLines;
    //        }

    //        outlen += newLines * 2;              // the number of line break chars we'll add, "\r\n"
    //    }

    //    // If we overflow an int then we cannot allocate enough
    //    // memory to output the value so throw
    //    if(outlen > int.MaxValue)
    //    {
    //        throw new OutOfMemoryException();
    //    }

    //    return (int)outlen;
    //}

    //internal static readonly char[] base64Table = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
    //                                                    'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
    //                                                    'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
    //                                                    't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
    //                                                    '8', '9', '+', '/', '=' };

    //private static unsafe int ConvertToBase64Array(char* outChars, byte* inData, int offset, int length, bool insertLineBreaks)
    //{
    //    var lengthmod3 = length % 3;
    //    var calcLength = offset + (length - lengthmod3);
    //    var j = 0;
    //    var charcount = 0;
    //    // Convert three bytes at a time to base64 notation.  This will consume 4 chars.
    //    int i;

    //    // get a pointer to the base64Table to avoid unnecessary range checking
    //    fixed(char* base64 = &base64Table[0])
    //    {
    //        for(i = offset; i < calcLength; i += 3)
    //        {
    //            if(insertLineBreaks)
    //            {
    //                if(charcount == base64LineBreakPosition)
    //                {
    //                    outChars[j++] = '\r';
    //                    outChars[j++] = '\n';
    //                    charcount = 0;
    //                }

    //                charcount += 4;
    //            }

    //            outChars[j] = base64[(inData[i] & 0xfc) >> 2];
    //            outChars[j + 1] = base64[((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)];
    //            outChars[j + 2] = base64[((inData[i + 1] & 0x0f) << 2) | ((inData[i + 2] & 0xc0) >> 6)];
    //            outChars[j + 3] = base64[inData[i + 2] & 0x3f];
    //            j += 4;
    //        }

    //        // Where we left off before
    //        i = calcLength;

    //        if(insertLineBreaks && (lengthmod3 != 0) && (charcount == base64LineBreakPosition))
    //        {
    //            outChars[j++] = '\r';
    //            outChars[j++] = '\n';
    //        }

    //        switch(lengthmod3)
    //        {
    //            case 2: // One character padding needed
    //                outChars[j] = base64[(inData[i] & 0xfc) >> 2];
    //                outChars[j + 1] = base64[((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)];
    //                outChars[j + 2] = base64[(inData[i + 1] & 0x0f) << 2];
    //                outChars[j + 3] = base64[64]; // Pad
    //                j += 4;
    //                break;
    //            case 1: // Two character padding needed
    //                outChars[j] = base64[(inData[i] & 0xfc) >> 2];
    //                outChars[j + 1] = base64[(inData[i] & 0x03) << 4];
    //                outChars[j + 2] = base64[64]; // Pad
    //                outChars[j + 3] = base64[64]; // Pad
    //                j += 4;
    //                break;
    //        }
    //    }

    //    return j;
    //}
}
