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

namespace OneF.Mechineable;

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

/// <summary>
/// 操作系统信息
/// </summary>
[Serializable]
public class OSPlateformInfomation
{
    public OSPlateformInfomation()
    {
        RegistryHelper.TryGetValue(
           RegistryHive.LocalMachine,
           "Software\\Microsoft\\Windows NT\\CurrentVersion",
           "ProductId",
           out var productId);

        ProductId = productId;
    }

    /// <summary>
    /// 启动时间
    /// </summary>
    public TimeSpan StartedTime { get; } = TimeSpan.FromMilliseconds(Environment.TickCount);

    /// <summary>
    /// 计算机名称
    /// </summary>
    public string MachineName { get; } = Environment.MachineName;

    /// <summary>
    /// 操作系统及版本
    /// </summary>
    public string OSVersion { get; } = Environment.OSVersion.ToString();

    /// <summary>
    /// 运行时身份
    /// </summary>
    public string RuntimeIdentifier { get; } = RuntimeInformation.RuntimeIdentifier;

    /// <summary>
    /// 系统架构
    /// </summary>
    public string OSArchitecture { get; } = RuntimeInformation.OSArchitecture.ToString();

    /// <summary>
    /// NET框架
    /// </summary>
    public string FrameworkDescription { get; } = RuntimeInformation.FrameworkDescription;

    /// <summary>
    /// 产品ID
    /// </summary>
    public string? ProductId { get; }
}
