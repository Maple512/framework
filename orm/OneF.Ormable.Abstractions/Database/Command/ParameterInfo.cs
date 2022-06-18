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

namespace OneF.Ormable.Database.Command;

using System.Data;
using OneF;

/// <summary>
/// 命令参数
/// </summary>
public class ParameterInfo
{
    public ParameterInfo(string name, object? value)
        : this(name, value, null, null, null, null)
    {
    }

    public ParameterInfo(
        string name,
        object? value = null,
        byte? precision = null,
        byte? scale = null,
        DbType? dbType = null,
        int? size = null)
    {
        Name = Check.NotNullOrWhiteSpace(name);
        Value = value;
        Precision = precision;
        Scale = scale;
        DbType = dbType;
        Size = size;
    }

    /// <summary>
    /// 参数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 参数值
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// 参数类型为<see langword="decimal"/>时的精度
    /// </summary>
    public byte? Precision { get; init; }

    /// <summary>
    /// 参数类型为<see langword="decimal"/>时的小数位数
    /// </summary>
    public byte? Scale { get; init; }

    /// <summary>
    /// 参数的类型
    /// </summary>
    public DbType? DbType { get; init; }

    /// <summary>
    /// 参数类型为<see langword="string"/>时的长度
    /// </summary>
    public int? Size { get; init; }
}
