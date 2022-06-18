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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

[StackTraceHidden]
[DebuggerStepThrough]

public static class OneFStringExtensions
{
    #region Check

    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    #endregion

    #region char case

    /// <summary>
    /// 转驼峰命名（首字符转小写，其余不变）
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>一般情况，首字符转小写，其余不变：<c>HelloWorld</c> -&gt; <c>helloWorld</c></item>
    /// <item>全是大写时，所有字幕均转小写：<c>HELLOWORLD</c> -&gt; <c>helloworld</c></item>
    /// <item>首字符为小写时，全部字符不变：<c>hELLOWORLD</c> -&gt; <c>hELLOWORLD</c></item>
    /// </list>
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("s")]
    public static string? ToCamelCase(this string? s)
    {
        if(s.IsNullOrWhiteSpace() || !char.IsUpper(s![0]))
        {
            return s;
        }

        var array = s.ToCharArray();

        for(var i = 0; i < array.Length && (i != 1 || char.IsUpper(array[i])); i++)
        {
            var flag = i + 1 < array.Length;

            if(i > 0 && flag && !char.IsUpper(array[i + 1]))
            {
                break;
            }

            array[i] = char.ToLowerInvariant(array[i]);
        }

        return new string(array);
    }

    /// <summary>
    /// 转蛇形命名（大写字符转小写，同时在小写字符和大写字符中间加下划线） 
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>一般情况，大写转小写，同时在小写字符和大写字符中间加下划线：<c>HelloWorld</c> -&gt; <c>hello_world</c></item>
    /// <item>全是大写时，所有字符均转小写：<c>HELLOWORLD</c> -&gt; <c>helloworld</c></item>
    /// </list>
    /// </remarks>
    /// <param name="s"></param>
    /// <param name="separator">分隔符，默认下滑线</param>
    /// <returns></returns>
    [return: NotNullIfNotNull("s")]
    public static string? ToSnakeCase(this string? s, char separator = '_')
    {
        if(s.IsNullOrWhiteSpace())
        {
            return s;
        }

        // * 2：冗余空间
        var stringBuilder = new StringBuilder(s!.Length * 2);

        var snakeCaseState = SnakeCaseState.Start;

        for(var i = 0; i < s.Length; i++)
        {
            if(s[i] == ' ')
            {
                if(snakeCaseState != 0)
                {
                    snakeCaseState = SnakeCaseState.NewWord;
                }
            }
            else if(char.IsUpper(s[i]))
            {
                switch(snakeCaseState)
                {
                    case SnakeCaseState.Upper:
                    {
                        var flag = i + 1 < s.Length;
                        if(i > 0 && flag)
                        {
                            var c = s[i + 1];
                            if(!char.IsUpper(c) && c != separator)
                            {
                                _ = stringBuilder.Append(separator);
                            }
                        }

                        break;
                    }
                    case SnakeCaseState.Lower:
                    case SnakeCaseState.NewWord:
                        _ = stringBuilder.Append(separator);
                        break;
                }

                var value = char.ToLowerInvariant(s[i]);
                _ = stringBuilder.Append(value);
                snakeCaseState = SnakeCaseState.Upper;
            }
            else if(s[i] == separator)
            {
                _ = stringBuilder.Append(separator);
                snakeCaseState = SnakeCaseState.Start;
            }
            else
            {
                if(snakeCaseState == SnakeCaseState.NewWord)
                {
                    _ = stringBuilder.Append(separator);
                }

                _ = stringBuilder.Append(s[i]);
                snakeCaseState = SnakeCaseState.Lower;
            }
        }

        return stringBuilder.ToString();
    }

    #endregion char case

    private enum SnakeCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord,
    }
}
