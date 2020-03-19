﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DapperAnalyser
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperConstAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "DapperConstAnalyser";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.InvocationExpression);
        }

        private static bool IsDapperSqlMapper(INamedTypeSymbol nts)
        {
            var ntst = $"{nts.ContainingNamespace}.{nts.Name}";
            return "Dapper.SqlMapper" == ntst;
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            // invocationExpr.Expression is the expression before "(", here "string.Equals".
            // In this case it should be a MemberAccessExpressionSyntax, with a member name "Equals"
            var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpr == null)
                return;

            var targetFunctionNames = new[] { "Query", "QueryAsync", "Execute", "ExecuteAsync" };

            if (targetFunctionNames.All(a => memberAccessExpr.Name.Identifier.Text != a))
                return;

            var semanticModel = context.SemanticModel;

            // Now we need to get the semantic model of this node to get the type of the node
            // So, we can check it is of type string whatever the way you define it (string or System.String)
            var memberSymbol = semanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
            if (memberSymbol == null)
                return;

            // Check the method is a member of the class string
            if (!IsDapperSqlMapper(memberSymbol.ContainingType))
                return;

            var query = invocationExpr.ArgumentList.Arguments.First();

            if (query.Expression is LiteralExpressionSyntax lit && !SqlStringValidator.IsValid(lit.Token.ValueText))
            {
                var diagnostic = Diagnostic.Create(Rule, lit.GetLocation(), lit);
                context.ReportDiagnostic(diagnostic);
            }
            else if (query.Expression is IdentifierNameSyntax ident)
            {
                ////var dec = context.SemanticModel.GetDeclaredSymbol(ident);

                var flow = semanticModel.AnalyzeDataFlow(ident);
                var value = semanticModel.GetConstantValue(ident);
                var flowIn = flow.DataFlowsIn.Single();
                var dec = semanticModel.GetDeclaredSymbol(flowIn.DeclaringSyntaxReferences.First().GetSyntax());
                //flowIn.
                //&& !SqlStringValidator.IsValid(ident.Identifier.ValueText);
                if (value.HasValue && !SqlStringValidator.IsValid(value.Value as string))
                {
                    var diagnostic = Diagnostic.Create(Rule, flowIn.DeclaringSyntaxReferences.First().GetSyntax().GetLocation(), $"\"{value.Value as string}\"");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}