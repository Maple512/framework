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

namespace OneF;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ObjectLayoutInspector;

// blog: .NET性能优化-使用结构体替代类 https://mp.weixin.qq.com/s/FA8GrkKx01NM6fVcy4kV-Q

public static class StructSize
{
    public static void Run()
    {
        TypeLayout.PrintLayout<FlightPrice1>();// 114 bytes = 88 + 8(Object Header) + 8(Method Table) 

        TypeLayout.PrintLayout<FlightPrice2>();// 88 bytes

        TypeLayout.PrintLayout<FlightPrice3>();// 72 bytes

        TypeLayout.PrintLayout<FlightPrice4>();// 32 bytes
    }
}

/// <summary>
/// 类
/// </summary>
public class FlightPrice1
{
    /// <summary>
    /// 航司二字码 如 中国国际航空股份有限公司：CA
    /// </summary>
    public string Airline { get; set; }

    /// <summary>
    /// 起始机场三字码 如 上海虹桥国际机场：SHA
    /// </summary>
    public string Start { get; set; }

    /// <summary>
    /// 抵达机场三字码 如 北京首都国际机场：PEK
    /// </summary>
    public string End { get; set; }

    /// <summary>
    /// 航班号 如 CA0001
    /// </summary>
    public string FlightNo { get; set; }

    /// <summary>
    /// 舱位代码 如 Y
    /// </summary>
    public string Cabin { get; set; }

    /// <summary>
    /// 价格 单位：元
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 起飞日期 如 2017-01-01
    /// </summary>
    public DateOnly DepDate { get; set; }

    /// <summary>
    /// 起飞时间 如 08:00
    /// </summary>
    public TimeOnly DepTime { get; set; }

    /// <summary>
    /// 抵达日期 如 2017-01-01
    /// </summary>
    public DateOnly ArrDate { get; set; }

    /// <summary>
    /// 抵达时间 如 08:00
    /// </summary>
    public TimeOnly ArrTime { get; set; }
}

/// <summary>
/// 结构体
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct FlightPrice2
{
    /// <summary>
    /// 航司二字码 如 中国国际航空股份有限公司：CA
    /// </summary>
    public string Airline { get; set; }

    /// <summary>
    /// 起始机场三字码 如 上海虹桥国际机场：SHA
    /// </summary>
    public string Start { get; set; }

    /// <summary>
    /// 抵达机场三字码 如 北京首都国际机场：PEK
    /// </summary>
    public string End { get; set; }

    /// <summary>
    /// 航班号 如 CA0001
    /// </summary>
    public string FlightNo { get; set; }

    /// <summary>
    /// 舱位代码 如 Y
    /// </summary>
    public string Cabin { get; set; }

    /// <summary>
    /// 价格 单位：元
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 起飞日期 如 2017-01-01
    /// </summary>
    public DateOnly DepDate { get; set; }

    /// <summary>
    /// 起飞时间 如 08:00
    /// </summary>
    public TimeOnly DepTime { get; set; }

    /// <summary>
    /// 抵达日期 如 2017-01-01
    /// </summary>
    public DateOnly ArrDate { get; set; }

    /// <summary>
    /// 抵达时间 如 08:00
    /// </summary>
    public TimeOnly ArrTime { get; set; }
}

// 跳过本地变量初始化
[SkipLocalsInit]
// 调整布局方式 使用Explicit自定义布局
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct FlightPrice3
{
    // 需要手动指定偏移量
    [FieldOffset(0)]
    // 航司使用两个字符存储
    public unsafe fixed char Airline[2];

    // 由于航司使用了4byte 所以起始机场偏移4byte
    [FieldOffset(4)]
    public unsafe fixed char Start[3];

    // 同理起始机场使用6byte 偏移10byte
    [FieldOffset(10)]
    public unsafe fixed char End[3];

    [FieldOffset(16)]
    public unsafe fixed char FlightNo[4];

    [FieldOffset(24)]
    public unsafe fixed char Cabin[2];

    // decimal 16byte
    [FieldOffset(28)]
    public decimal Price;

    // DateOnly 4byte
    [FieldOffset(44)]
    public DateOnly DepDate;

    // TimeOnly 8byte
    [FieldOffset(48)]
    public TimeOnly DepTime;

    [FieldOffset(56)]
    public DateOnly ArrDate;

    [FieldOffset(60)]
    public TimeOnly ArrTime;
}

[SkipLocalsInit]
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct FlightPrice4
{
    // 使用byte标识航司  byte范围0~255
    [FieldOffset(0)]
    public byte Airline;

    // 使用无符号整形表示起抵机场和航班号 2^16次方
    [FieldOffset(1)]
    public ushort Start;

    [FieldOffset(3)]
    public ushort End;

    [FieldOffset(5)]
    public ushort FlightNo;

    [FieldOffset(7)]
    public byte Cabin;

    // 不使用decimal 价格精确到分存储
    [FieldOffset(8)]
    public long PriceFen;

    // 使用时间戳替代
    [FieldOffset(16)]
    public long DepTime;

    [FieldOffset(24)]
    public long ArrTime;
}
