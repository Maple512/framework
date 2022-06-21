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
using System.Linq;
using Shouldly;
using Xunit;

public class Check_Test
{
    [Fact]
    public void Validate_model()
    {
        var model = new Model();

        // 验证所有属性
        var result = ValidateHelper.TryValidate(model);

        result.IsValid.ShouldBeFalse();

        result.Errors.Count.ShouldBe(1);

        model.Description = "012345";

        result = ValidateHelper.TryValidate(model);

        result.Errors.Count.ShouldBe(2);

        // 验证必填属性
        result = ValidateHelper.TryValidate(model, false);

        result.Errors.Count.ShouldBe(1);

        // 验证 IValidatableObject （在前面所有验证项都验证后，执行自定义验证）
        model.Name = "1234";
        model.Description = "12456";
        result = ValidateHelper.TryValidate(model);

        result.IsValid.ShouldBeFalse();

        result.Errors.Count.ShouldBe(1);

        result.Errors.Any(x => x.ErrorMessage == _errror_message).ShouldBeTrue();
    }

    private const string _errror_message = "Custom validation message";

    private class Model : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [MaxLength(5)]
        public string Description { get; set; }

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>
            {
                 new ValidationResult(_errror_message)
            };
        }
    }
}
