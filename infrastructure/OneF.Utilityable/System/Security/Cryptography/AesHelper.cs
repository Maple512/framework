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

using System.Diagnostics;
using System.Linq;
using System.Text;
using OneF;

/// <summary>
/// AES对称密码
/// <list type="bullet">
/// <item>keySize: 128, 192, 256 bit</item>
/// <item>key: 16, 24, 32 byte = keySize / 8</item>
/// <item>blockSize: 128 bit</item>
/// <item>iv: 16 byte = blockSize / 8</item>
/// </list>
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
public static class AesHelper
{
    // source: https://github.com/dotnet/runtime/blob/899bf9704693661d6fe53fdb7f737f76447efa14/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/Aes.cs#L36
    private static readonly int[] _keySizes = { 16, 24, 32 };

    /// <summary>
    /// Encrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key, the length must be between 16, 24, 32 bytes.</param>
    /// <param name="iv">The iv, the length must be between 16 bytes.</param>
    /// <param name="keySize">The key size, the value must be between 128, 192, 256, the unit is bit, if not specified, it is calculated from the key.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>A base64 string.</returns>
    public static string Encrypt(
        string data,
        string key,
        string iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        _ = Check.NotNullOrWhiteSpace(data);

        var result = Encrypt(
            Encoding.UTF8.GetBytes(data),
            Encoding.UTF8.GetBytes(key),
            Encoding.UTF8.GetBytes(iv),
            keySize,
            mode,
            padding);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Encrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key, the length must be between 16, 24, 32 bytes.</param>
    /// <param name="iv">The iv, the length must be between 16 bytes.</param>
    /// <param name="keySize">The key size, the value must be between 128, 192, 256 bit, the unit is bit, if not specified, it is calculated from the key.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>An array of byte.</returns>
    public static byte[] Encrypt(
        byte[] data,
        byte[] key,
        byte[] iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        _ = Check.NotNullOrEmpty(data);

        using var aes = Aes.Create();

        aes.Mode = mode;
        aes.KeySize = keySize ?? KeySizeRollback(key);
        aes.Padding = padding;

        var encryptor = aes.CreateEncryptor(key, iv);

        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// Decrypts the.
    /// </summary>
    /// <param name="data">The data is a base64 string.</param>
    /// <param name="key">The key, the length must be between 16, 24, 32 bytes.</param>
    /// <param name="iv">The iv, the length must be between 16 bytes.</param>
    /// <param name="keySize">The key size, the value must be between 128, 192, 256 bit, the unit is bit, if not specified, it is calculated from the key.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>A decrypted string.</returns>
    public static string Decrypt(
        string data,
        string key,
        string iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        _ = Check.NotNullOrWhiteSpace(data);

        var result = Decrypt(
            Convert.FromBase64String(data),
            Encoding.UTF8.GetBytes(key),
            Encoding.UTF8.GetBytes(iv),
            keySize,
            mode,
            padding);

        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// Decrypts the.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="key">The key, the length must be between 16, 24, 32 bytes.</param>
    /// <param name="iv">The iv, the length must be between 16 bytes.</param>
    /// <param name="keySize">The key size, the value must be between 128, 192, 256 bit, the unit is bit, if not specified, it is calculated from the key.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="padding">The padding.</param>
    /// <returns>An array of byte.</returns>
    public static byte[] Decrypt(
        byte[] data,
        byte[] key,
        byte[] iv,
        int? keySize = null,
        CipherMode mode = CipherMode.CBC,
        PaddingMode padding = PaddingMode.PKCS7)
    {
        _ = Check.NotNullOrEmpty(data);

        using var aes = Aes.Create();

        aes.Mode = mode;
        aes.KeySize = keySize ?? KeySizeRollback(key);
        aes.Padding = padding;

        var transform = aes.CreateDecryptor(key, iv);

        return transform.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// 通过指定的key计算KeySize
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static int KeySizeRollback(byte[] key)
    {
        if(_keySizes.Contains(key.Length))
        {
            return key.Length * 8;
        }

        throw new ArgumentOutOfRangeException(nameof(key), "Specified key is not a valid size for this algorithm, the size must be between 16, 24, 32 bytes.");
    }
}
