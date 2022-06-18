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

namespace System.Security.Cryptography;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using OneF;
using Shouldly;
using Xunit;

/// <summary>
/// The aes helper_ test.
/// </summary>

public class AesHelper_Test
{
    /// <summary>
    /// Use_bytes the.
    /// </summary>
    /// <param name="cipherText">The cipher text.</param>
    [Theory]
    [InlineData("0123456789")]
    [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("👍🎂🌹😉🎶😢👀😫🤗🤩🥼🕉💢")]
    [InlineData("你好，我是Maple512")]
    public void Use_bytes(string cipherText)
    {
        var data = Encoding.UTF8.GetBytes(cipherText);

        var key = RandomHelper.GetRandomBytes(16).ToArray();

        var iv = RandomHelper.GetRandomBytes(16).ToArray();

        var encrypted = AesHelper.Encrypt(data, key, iv);

        var decrypted = AesHelper.Decrypt(encrypted, key, iv);

        decrypted.ShouldBe(data);

        Encoding.UTF8.GetString(decrypted).ShouldBe(cipherText);
    }

    /// <summary>
    /// Use_strings the.
    /// </summary>
    /// <param name="cipherText">The cipher text.</param>
    [Theory]
    [InlineData("0123456789")]
    [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("👍🎂🌹😉🎶😢👀😫🤗🤩🥼🕉💢")]
    [InlineData("你好，我是Maple512")]
    public void Use_string(string cipherText)
    {
        var key = "0123456789102154";

        var iv = "0123456789102154";

        var encrypted = AesHelper.Encrypt(cipherText, key, iv);

        var decrypted = AesHelper.Decrypt(encrypted, key, iv);

        decrypted.ShouldBe(cipherText);
    }

    /// <summary>
    /// Use_files the.
    /// </summary>
    /// <returns>A Task.</returns>
    [Fact]
    public async Task Use_file()
    {
        var cipherText = await File.ReadAllTextAsync("./fakes/cipher.txt");

        var key = "0123456789102154";

        var iv = "0123456789102154";

        var encrypted = AesHelper.Encrypt(cipherText, key, iv);

        var decrypted = AesHelper.Decrypt(encrypted, key, iv);

        decrypted.ShouldBe(cipherText);
    }

    /// <summary>
    /// Use_external_encryptions the.
    /// </summary>
    [Fact]
    public void Use_external_encryption()
    {
        var key = "0123456789102154";

        var iv = "0123456789102154";

        var cipherText = "你好，我是Maple512";

        // http://tool.chacuo.net/cryptaes
        var encrypted = "jZJXXVBA9ypcsui5gCv7ZrsJ4Ya4zajnIp6irIVxKOI=";

        var decrypted = AesHelper.Decrypt(encrypted, key, iv);

        decrypted.ShouldBe(cipherText);
    }
}
