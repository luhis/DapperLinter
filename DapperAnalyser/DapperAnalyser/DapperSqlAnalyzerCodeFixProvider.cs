using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DapperAnalyser
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DapperSqlAnalyzerCodeFixProvider)), Shared]
    public class DapperSqlAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Correct SQL";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(DapperSqlAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var ancestors = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf();
            var declaration = ancestors.OfType<LiteralExpressionSyntax>().FirstOrDefault() ?? ancestors.OfType<LocalDeclarationStatementSyntax>().First().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
          

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => this.CorrectSql(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> CorrectSql(Document document, LiteralExpressionSyntax typeDecl,
            CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            var newName = SqlStringFixer.Fix(typeDecl.Token.ValueText);
            var newToken = LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(newName));

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(typeDecl, newToken);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
