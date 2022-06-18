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

namespace OneF.IO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

public class File_Test
{
    private static readonly FileOptions _options = FileOptions.None;

    private const int DefaultFileRows = 1000;

    [Theory]
    [MemberData(nameof(GeneratorRandomFile))]
    public void Get_file_length_by_random_access(string file)
    {
        using var handle = File.OpenHandle(file);

        var length = RandomAccess.GetLength(handle);

        Debug.Print($"File length: {length}");

        length.ShouldBeGreaterThan(0);
    }

    [Theory]
    [MemberData(nameof(GeneratorRandomFile))]
    public async Task Write_using_buffer(string filename)
    {
        using var handle = File.OpenHandle(filename, FileMode.Create, FileAccess.Write, options: _options);

        handle.IsAsync.ShouldBeFalse();

        await RandomAccess.WriteAsync(handle, new ReadOnlyMemory<byte>[] { Array.Empty<byte>() }, fileOffset: 0);
    }

    [Fact]
    public async Task Write_beyond_end_of_file_extends_the_file()
    {
        var file = CreateRandomFileName();

        using var handle = File.OpenHandle(file, FileMode.CreateNew, FileAccess.Write, options: _options);

        RandomAccess.GetLength(handle).ShouldBe(0);

        await RandomAccess.WriteAsync(handle, new ReadOnlyMemory<byte>[] { new byte[1] { 1 } }, fileOffset: 1);

        RandomAccess.GetLength(handle).ShouldBe(2);
    }

    [Fact]
    public async Task Write_bytes_from_given_buffer_to_given_file_at_given_offset()
    {
        var file = CreateRandomFileName();

        var source = @"
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
6563441546asdfapoi3j2o4jop213i4234234426534v4623464 426344 623446 2434624364 426323542364 23464 243644236 446234 2344233 544632434 6523425234
";

        var size = source.Length;
        var content = Encoding.UTF8.GetBytes(source);

        using(var handle = File.OpenHandle(file, FileMode.CreateNew, FileAccess.Write, options: _options))
        {
            var total = 0L;

            while(total != size)
            {
                var buffer = (int)Math.Min(content.Length - total, size / 4);

                var buffer1 = content.AsMemory((int)total, buffer);
                var buffer2 = content.AsMemory((int)total + buffer);

                await RandomAccess.WriteAsync(handle,
                    new ReadOnlyMemory<byte>[]
                    {
                     buffer1,
                     Array.Empty<byte>(),
                     buffer2
                    },
                    fileOffset: total);

                total += buffer1.Length + buffer2.Length;
            }
        }

        var bytes = File.ReadAllBytes(file);

        bytes.ShouldBeEquivalentTo(content);

        var text = Encoding.UTF8.GetString(bytes);

        text.ShouldBeEquivalentTo(source);
    }

    private static IEnumerable<object> GeneratorRandomFile() => GeneratorFile();

    private static IEnumerable<object> GeneratorFile(int? rows = null)
    {
        var file = CreateRandomFileName();

        rows ??= RandomNumberGenerator.GetInt32(1, DefaultFileRows);

        GenerateTextFile(file, rows.Value);

        yield return new object[] { file };
    }

    private static string CreateRandomFileName()
    {
        var file = Path.Combine(TestAssetFolder(), Path.GetRandomFileName());

        Debug.Print($"The file name of random: {file}");

        return file;
    }

    // TODO: async after the https://github.com/xunit/xunit/issues/1698, https://github.com/xunit/xunit/issues/2133 is closed
    private static void GenerateTextFile(string file, int rows)
    {
        if(File.Exists(file))
        {
            File.Delete(file);
        }

        Debug.Print($"The file name of generated text: {file}");

        var text = "M7dG0vibh9bOA";

        File.AppendAllLines(file, Enumerable.Range(0, rows).Select(x => $"【{x + 1}】{text}"));
    }

    private static string TestAssetFolder()
    {
        var folder = Path.GetRelativePath(".", "test_asset");

        if(!Directory.Exists(folder))
        {
            _ = Directory.CreateDirectory(folder);
        }

        return folder;
    }
}
