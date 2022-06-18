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
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class Stream_Test
{
    #region Pipe And Stream

    // post: https://zhuanlan.zhihu.com/p/39223648

    private static readonly string _chars = "Hello world.";

    [Fact]
    public void Write_to_stream()
    {
        using var ms = new MemoryStream();

        Write(ms);

        ms.Position = 0;

        Read(ms);
    }

    [Fact]
    public async ValueTask Write_to_pipe()
    {
        var pipe = new Pipe();

        await Write(pipe.Writer);

        await Read(pipe.Reader);
    }

    private void Write(Stream stream)
    {
        var bytes = Encoding.UTF8.GetBytes(_chars);

        stream.Write(bytes);

        stream.Flush();
    }

    private void Read(Stream stream)
    {
        int bytesReaded;

        var buffer = new byte[1024];

        do
        {
            bytesReaded = stream.Read(buffer, 0, buffer.Length);

            if(bytesReaded > 0)
            {
                var s = Encoding.UTF8.GetString(buffer, 0, bytesReaded);

                Debug.WriteLine(s);
            }
        } while(bytesReaded > 0);
    }

    private async ValueTask Write(PipeWriter writer)
    {
        // 申请一个空间（不是固定的大小，只是申请一个空间）
        var workspace = writer.GetMemory(20);

        // 向空间写入数据
        var length = Encoding.UTF8.GetBytes(_chars, workspace.Span);

        // 通知writer到底写入了多少数据
        writer.Advance(length);

        // 使已写入的数据可以被Read（调用PipeRead.ReadAsync()）（如果之后不调用Complete，那么Read时会卡住）
        _ = await writer.FlushAsync();

        // 标记为写入完成，表示不在写入数据（必须的）
        await writer.CompleteAsync();
    }

    private async ValueTask Read(PipeReader reader)
    {
        do
        {
            var read = await reader.ReadAsync();

            var buffer = read.Buffer;

            if(buffer.IsEmpty && read.IsCompleted)
            {
                break;
            }

            foreach(var segment in buffer)
            {
                var s = Encoding.UTF8.GetString(segment.Span);

                Debug.WriteLine(s);
            }

            reader.AdvanceTo(buffer.End);

            //if (buffer.IsSingleSegment)
            //{
            //    break;
            //}
        } while(true);

        //  标记为已完成，不再读取任何数据
        await reader.CompleteAsync();
    }

    #endregion Pipe And Stream
}
