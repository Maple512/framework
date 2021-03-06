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

namespace OneF.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IndentedStringBuilder
{
    private const byte IndentSize = 4;
    private byte _indent;
    private bool _indentPending = true;

    private readonly StringBuilder _stringBuilder = new();

    public virtual int Length => _stringBuilder.Length;

    /// <summary>
    /// 将给定字符串分隔为多行，然后将每行（以当前缩进为前缀，后跟新行）追加到正在生成的字符串中。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="skipFinalNewline"></param>
    /// <returns></returns>
    public virtual IndentedStringBuilder AppendLines(string value, bool skipFinalNewline = false)
    {
        using(var reader = new StringReader(value))
        {
            var first = true;
            string? line;
            while((line = reader.ReadLine()) != null)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    _ = AppendLine();
                }

                if(line.Length != 0)
                {
                    _ = Append(line);
                }
            }
        }

        if(!skipFinalNewline)
        {
            _ = AppendLine();
        }

        return this;
    }

    public virtual IndentedStringBuilder AppendLine()
    {
        _ = AppendLine(string.Empty);

        return this;
    }

    public virtual IndentedStringBuilder AppendLine(string value)
    {
        if(value.Length != 0)
        {
            DoIndent();
        }

        _ = _stringBuilder.AppendLine(value);

        _indentPending = true;

        return this;
    }

    public virtual IndentedStringBuilder Append(char value)
    {
        DoIndent();

        _ = _stringBuilder.Append(value);

        return this;
    }

    public virtual IndentedStringBuilder Append(string value)
    {
        DoIndent();

        _ = _stringBuilder.Append(value);

        return this;
    }

    public virtual IndentedStringBuilder Append(IEnumerable<string> values)
    {
        DoIndent();

        // AppendJoin也是使用的foreach
        foreach(var value in values)
        {
            _ = _stringBuilder.Append(value);
        }

        return this;
    }

    public virtual IndentedStringBuilder Append(IEnumerable<char> value)
    {
        DoIndent();

        foreach(var chr in value)
        {
            _ = _stringBuilder.Append(chr);
        }

        return this;
    }

    public override string ToString() => _stringBuilder.ToString();

    public virtual IndentedStringBuilder IncrementIndent()
    {
        _indent++;

        return this;
    }

    public virtual IndentedStringBuilder DecrementIndent()
    {
        if(_indent > 0)
        {
            _indent--;
        }

        return this;
    }

    public virtual IndentedStringBuilder Clear()
    {
        _ = _stringBuilder.Clear();
        _indent = 0;

        return this;
    }

    public virtual IDisposable Indent()
        => new Indenter(this);

    public virtual IDisposable SuspendIndent()
        => new IndentSuspender(this);

    private sealed class Indenter : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;

        public Indenter(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;

            _ = _stringBuilder.IncrementIndent();
        }

        public void Dispose()
            => _stringBuilder.DecrementIndent();
    }

    private sealed class IndentSuspender : IDisposable
    {
        private readonly IndentedStringBuilder _stringBuilder;
        private readonly byte _indent;

        public IndentSuspender(IndentedStringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
            _indent = _stringBuilder._indent;
            _stringBuilder._indent = 0;
        }

        public void Dispose()
            => _stringBuilder._indent = _indent;
    }

    private void DoIndent()
    {
        if(_indentPending && _indent > 0)
        {
            _ = _stringBuilder.Append(' ', _indent * IndentSize);
        }

        _indentPending = false;
    }
}
