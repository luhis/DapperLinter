using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace DapperAnalyser.Test
{
    public class DapperConstAnalyserUnitTest : CodeFixVerifier
    {
        [Fact]
        public void EmptyCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void SuggestConst()
        {
            var test = @"
namespace DapperDemo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class C1
    {
        public IEnumerable<int> A()
        {
            using (var connection = new SqlConnection(
                ""Server = tcp:mhknbn2kdz.database.windows.net; Database = AdventureWorks2012; User ID = sqlfamily; Password = sqlf@m1ly; ""))
            {
                var sql = ""select * from Person.Person where FirstName = 'Mark'"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "DapperConstAnalyser",
                Message = "'sql' is not a const.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 16, 46)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void AcceptConst()
        {
            var test = @"
namespace DapperDemo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class C1
    {
        public IEnumerable<int> A()
        {
            using (var connection = new SqlConnection(
                ""Server = tcp:mhknbn2kdz.database.windows.net; Database = AdventureWorks2012; User ID = sqlfamily; Password = sqlf@m1ly; ""))
            {
                const string sql = ""select * from Person.Person where FirstName = 'Mark'"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AcceptInline()
        {
            var test = @"
namespace DapperDemo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class C1
    {
        public IEnumerable<int> A()
        {
            using (var connection = new SqlConnection(
                ""Server = tcp:mhknbn2kdz.database.windows.net; Database = AdventureWorks2012; User ID = sqlfamily; Password = sqlf@m1ly; ""))
            {
                return connection.Query<int>(""select * from Person.Person where FirstName = 'Mark'"");
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
            //return new DapperSqlAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperConstAnalyzer();
        }
    }
}