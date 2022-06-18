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

public static class ValidateHelper
{
    /// <summary>
    /// 验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="validateAllProperties"><see langword="true"/>时，检查所有属性，否则，仅检查标注了<see cref="RequiredAttribute"/>的属性，默认<see langword="true"/></param>
    /// <returns></returns>
    public static ValidateResult TryValidate<T>(T instance, bool validateAllProperties = true)
        where T : class
    {
        _ = Check.NotNull(instance);

        var errors = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            instance!,
            new(instance!, null, null),
            errors,
            validateAllProperties);

        return new ValidateResult(isValid, errors);
    }

    public readonly ref struct ValidateResult
    {
        public ValidateResult(bool isValid, ICollection<ValidationResult> errors)
        {
            IsValid = isValid;
            Errors = errors;
        }

        public bool IsValid { get; }

        public ICollection<ValidationResult> Errors { get; }
    }
}
