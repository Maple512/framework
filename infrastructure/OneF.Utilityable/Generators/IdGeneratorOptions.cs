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

namespace OneF.Generators;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class IdGeneratorOptions : IValidatableObject
{
    /// <summary>
    /// 基础时间
    /// </summary>
    public DateTime BaseTime { get; init; } = DateTime.UtcNow.AddYears(-1);

    /// <summary>
    /// 机器码位长
    /// <para>取值范围：[1,15]</para>
    /// <para>要求：序列数位长 + 机器码位长不超过22</para>
    /// </summary>
    [Range(1, 15)]
    public byte WorkerIdBitLength { get; init; } = 6;

    /// <summary>
    /// 机器码
    /// <para>范围[0,32767]</para>
    /// </summary>
    [Range(1, short.MaxValue)]
    public ushort WorkerId { get; init; } = 1;

    /// <summary>
    /// 序列数位长
    /// <para>范围：[3,21]</para>
    /// </summary>
    [Range(3, 21)]
    public byte SequenceBitLength { get; init; } = 6;

    /// <summary>
    /// 最大序列数
    /// <para>范围：[<see cref="SequenceMinNumber"/>,2^<see cref="SequenceBitLength"/> - 1]</para>
    /// </summary>
    public int SequenceMaxNumber { get; init; } = 20;

    /// <summary>
    /// 最小序列数
    /// <para>范围：[5, <see cref="MaxLengthAttribute"/>]</para>
    /// <para>每毫秒的前5个序列数对应编号0-4是保留位，其中1-4是时间回拨相应预留位，0是手工新值预留位</para>
    /// </summary>
    public ushort SequenceMinNumber { get; init; } = 5;

    /// <summary>
    /// 最大漂移次数（含），
    /// 默认2000，推荐范围500-10000（与计算能力有关）
    /// </summary>
    public virtual int TopOverCostCount { get; init; } = 2000;

    /// <summary>
    /// 数据中心ID（默认0）
    /// </summary>
    public uint DataCenterId { get; init; }

    /// <summary>
    /// 数据中心ID长度（默认0）
    /// </summary>
    public byte DataCenterIdBitLength { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        // 不能超过当前时间
        if(BaseTime > DateTime.UtcNow)
        {
            errors.Add(new ValidationResult($"The {BaseTime} can't exceed the current time."));
        }

        // 序列数位长 + 机器码位长不超过22
        if(WorkerIdBitLength + SequenceBitLength > 22)
        {
            errors.Add(new ValidationResult($"The {WorkerIdBitLength} + {SequenceBitLength} not more than 22."));
        }

        return errors;
    }
}
