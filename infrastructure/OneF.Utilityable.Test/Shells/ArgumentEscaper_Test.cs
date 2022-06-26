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

namespace OneF.Shells;

using Shouldly;
using Xunit;

public class ArgumentEscaper_Test
{
    [Theory]
    [InlineData(new[] { "one", "two", "three" }, "one two three")]
    [InlineData(new[] { "line1\nline2", "word1\tword2" }, "\"line1\nline2\" \"word1\tword2\"")]
    [InlineData(new[] { "with spaces" }, "\"with spaces\"")]
    [InlineData(new[] { @"with\backslash" }, @"with\backslash")]
    [InlineData(new[] { @"""quotedwith\backslash""" }, @"\""quotedwith\backslash\""")]
    [InlineData(new[] { @"C:\Users\" }, @"C:\Users\")]
    [InlineData(new[] { @"C:\Program Files\dotnet\" }, @"""C:\Program Files\dotnet\\""")]
    [InlineData(new[] { @"backslash\""preceedingquote" }, @"backslash\\\""preceedingquote")]
    [InlineData(new[] { @""" hello """ }, @"""\"" hello \""""")]
    public void EscapesArgumentsForProcessStart(string[] args, string expected)
    {
        ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args).ShouldBe(expected);
    }

    [Fact]
    public void Escapes()
    {
        var result = ArgumentEscaper.EscapeSingleArg($"tool update CliFullName -g ");
    }
}
