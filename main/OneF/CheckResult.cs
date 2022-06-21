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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CheckResult
{
#pragma warning disable CA1825 // 避免长度为零的数组分配
    private static readonly ValidationResult[] _empty = new ValidationResult[0];
#pragma warning restore CA1825 // 避免长度为零的数组分配

    public CheckResult(bool isValid, ICollection<ValidationResult>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? _empty;
    }

    /// <summary>
    /// 是否通过验证
    /// <para><see langword="true"/>: 有效，通过验证</para>
    /// </summary>
    public bool IsValid { get; }

    public ICollection<ValidationResult> Errors { get; }
}
