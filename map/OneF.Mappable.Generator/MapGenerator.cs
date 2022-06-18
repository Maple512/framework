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

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OneF.Mappable.Internal;

[Generator]
public class MapGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 查找所有符合条件的class
        var classDeclaration = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, token) => HasExtensionsClass(node, token),
            static (context, token) => GetClassDeclaration(context, token))
            .Where(x => x is not null);

        var provider = context.CompilationProvider.Combine(classDeclaration.Collect());

        // 注册代码生成器
        context.RegisterSourceOutput(provider,
            static (context, content) => GenerateClassFile(context, content.Left, content.Right));
    }

    private static bool HasExtensionsClass(SyntaxNode node, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        if(node is not ClassDeclarationSyntax c)
        {
            return false;
        }

        return c.AttributeLists.Count > 0
          && c.AttributeLists.SelectMany(x => x.DescendantNodes().OfType<AttributeSyntax>()
                                                                      .Select(a => a.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault())
                                                                      .Select(a => a.ToString()))
          .IncludeAny(Constants.AttributeNames);
    }

    private static ClassDeclarationSyntax? GetClassDeclaration(GeneratorSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach(var attributeListSyntax in classDeclaration.AttributeLists)
        {
            foreach(var attributeSyntax in attributeListSyntax.Attributes)
            {
                if(context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var fullName = attributeSymbol.ContainingType.ToDisplayString();

                if(Constants.AttributeFullNames.Contains(fullName))
                {
                    return classDeclaration;
                }
            }
        }

        return null;
    }

    private static void GenerateClassFile(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> data)
    {
        if(data.IsDefaultOrEmpty)
        {
            return;
        }

        var mapContext = new TypeMapContext(compilation, data);

        var content = mapContext.Builder(context.CancellationToken);

        _ = content.ToString();

        context.AddSource(Constants.ExtensionsClassFileName, SourceText.From(content.ToString(), Encoding.UTF8));
    }
}
