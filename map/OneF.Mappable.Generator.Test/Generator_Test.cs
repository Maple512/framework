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

namespace OneF.Mappable;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

public class Generator_Test
{
    [Fact]
    public void Test1()
    {
        _ = Verify(@"namespace EnumGenerator;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using OneF.Mappable.DataAnnotations;

[AutoMapTo(typeof(Model3))]
public class Model1
{
    [NotMapped]
    public int Id { get; set; }

    public string Name { get; set; }
}

[AutoMapTo(typeof(Model3))]
public class Model2
{
    public string Name { get; set; }
}

public class Model3
{
    public int Id { get; set; }

    public string Name { get; set; }
}");
    }

    public static Task Verify(string source)
    {
        var compilation = TestHelper.GetCompilation(source);

        var generator = new MapGenerator();

        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);

        return Verifier.Verify(driver)
            .UseDirectory("Snapshots");
    }
}
