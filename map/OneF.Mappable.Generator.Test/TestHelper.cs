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

using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class TestHelper
{
    public static Compilation GetCompilation(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = AssemblyLoadContext.Default.Assemblies
            .Where(x => !x.IsDynamic)
            .Select(x => x.Location)
            .Where(x => File.Exists(x))
            .Select(x => MetadataReference.CreateFromFile(x));

        var compilation = CSharpCompilation.Create(
            "dynamic_compilation",
            new[] { syntaxTree },
            references,
            new(OutputKind.DynamicallyLinkedLibrary));

        var compilationResult = compilation.Emit("./a");

        if(!compilationResult.Success)
        {
            throw new Exception(string.Join(',', compilationResult.Diagnostics.Select(x => x.GetMessage())));
        }

        return compilation;
    }
}
