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

namespace OneF.Commands;

using System;
using System.Collections.Generic;
using System.Text;

// TODO: 等待建立单元测试
// source: https://github.com/dotnet/sdk/blob/937c7a68e16de27a7861f250bd572c8fd6849534/src/Cli/Microsoft.DotNet.Cli.Utils/ArgumentEscaper.cs

/// <summary>
/// <para>用于转义 <see cref="System.Diagnostics.ProcessStartInfo"/> 的参数</para>
/// </summary>
public static class ArgumentEscaper
{
    /// <summary>
    /// Undo the processing which took place to create string[] args in Main, so that the next
    /// process will receive the same string[] args
    ///
    /// See here for more info: http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string EscapeAndConcatenateArgArrayForProcessStart(IEnumerable<string> args)
    {
        var escaped = EscapeArgArray(args);

        return escaped.JoinAsString(' ');
    }

    /// <summary>
    /// Undo the processing which took place to create string[] args in Main, so that the next
    /// process will receive the same string[] args
    ///
    /// See here for more info: http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string EscapeAndConcatenateArgArrayForCmdProcessStart(IEnumerable<string> args)
    {
        var escaped = EscapeArgArrayForCmd(args);

        return escaped.JoinAsString(' ');
    }

    /// <summary>
    /// Undo the processing which took place to create string[] args in Main, so that the next
    /// process will receive the same string[] args
    ///
    /// See here for more info: http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static IEnumerable<string> EscapeArgArray(IEnumerable<string> args)
    {
        var escapedArgs = new List<string>();

        foreach(var arg in args)
        {
            escapedArgs.Add(EscapeSingleArg(arg));
        }

        return escapedArgs;
    }

    /// <summary>
    /// This prefixes every character with the '^' character to force cmd to interpret the argument
    /// string literally. An alternative option would be to do this only for cmd metacharacters.
    ///
    /// See here for more info: http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private static IEnumerable<string> EscapeArgArrayForCmd(IEnumerable<string> arguments)
    {
        var escapedArgs = new List<string>();

        foreach(var arg in arguments)
        {
            escapedArgs.Add(EscapeArgForCmd(arg));
        }

        return escapedArgs;
    }

    public static string EscapeSingleArg(string arg)
    {
        var sb = new StringBuilder();

        try
        {
            var length = arg.Length;
            var needsQuotes = length == 0 || ShouldSurroundWithQuotes(arg);
            var isQuoted = needsQuotes || IsSurroundedWithQuotes(arg);

            if(needsQuotes)
            {
                _ = sb.Append('"');
            }

            for(var i = 0; i < length; ++i)
            {
                var backslashCount = 0;

                // Consume All Backslashes
                while(i < arg.Length && arg[i] == '\\')
                {
                    backslashCount++;

                    i++;
                }

                // Escape any backslashes at the end of the arg when the argument is also quoted.
                // This ensures the outside quote is interpreted as an argument delimiter
                if(i == arg.Length && isQuoted)
                {
                    _ = sb.Append('\\', 2 * backslashCount);
                }

                // At then end of the arg, which isn't quoted, just add the backslashes, no need to escape
                else if(i == arg.Length)
                {
                    _ = sb.Append('\\', backslashCount);
                }

                // Escape any preceding backslashes and the quote
                else if(arg[i] == '"')
                {
                    _ = sb.Append('\\', (2 * backslashCount) + 1);
                    _ = sb.Append('"');
                }

                // Output any consumed backslashes and the character
                else
                {
                    _ = sb.Append('\\', backslashCount);
                    _ = sb.Append(arg[i]);
                }
            }

            if(needsQuotes)
            {
                _ = sb.Append('"');
            }

            return sb.ToString();
        }
        finally
        {
            _ = sb.Clear();
        }
    }

    /// <summary>
    /// Prepare as single argument to roundtrip properly through cmd.
    ///
    /// This prefixes every character with the '^' character to force cmd to interpret the argument
    /// string literally. An alternative option would be to do this only for cmd metacharacters.
    ///
    /// See here for more info: http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    private static string EscapeArgForCmd(string argument)
    {
        var sb = new StringBuilder();

        var quoted = ShouldSurroundWithQuotes(argument);

        if(quoted)
        {
            _ = sb.Append("^\"");
        }

        // Prepend every character with ^ This is harmless when passing through cmd and ensures cmd
        // metacharacters are not interpreted as such
        foreach(var character in argument)
        {
            _ = sb.Append('^');
            _ = sb.Append(character);
        }

        if(quoted)
        {
            _ = sb.Append("^\"");
        }

        return sb.ToString();
    }

    internal static bool ShouldSurroundWithQuotes(string argument)
    {
        // Only quote if whitespace exists in the string
        return ArgumentContainsWhitespace(argument);
    }

    internal static bool IsSurroundedWithQuotes(string argument)
    {
        return argument.StartsWith("\"", StringComparison.Ordinal)
            && argument.EndsWith("\"", StringComparison.Ordinal);
    }

    internal static bool ArgumentContainsWhitespace(string argument)
    {
        return argument.Contains(' ')
            || argument.Contains('\t')
            || argument.Contains('\n');
    }
}
