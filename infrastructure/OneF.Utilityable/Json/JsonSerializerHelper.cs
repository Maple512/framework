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

namespace OneF.Json;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using OneF.Text.Json;

public static class JsonSerializerHelper
{
    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的标准实例
    /// </summary>
    public static JsonSerializerOptions StandardInstance { get; } = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        // Mark: 解决中文乱码
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        AllowTrailingCommas = true,
        WriteIndented = true,
    };

    /// <summary>
    /// <see cref="JsonSerializerOptions"/> 的实例， <see
    /// cref="JsonSerializerOptions.PropertyNamingPolicy"/> 是 <see cref="SnakeCaseNamingPolly.SnakeCase"/>
    /// </summary>
    public static JsonSerializerOptions SnakeCaseInstance { get; } = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        AllowTrailingCommas = true,
        WriteIndented = true,
        PropertyNamingPolicy = SnakeCaseNamingPolly.SnakeCase,
    };
}
