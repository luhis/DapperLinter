using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace DapperAnalyser.Test
{
    public class DapperSqlStyleAnalyzerUnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [Fact]
        public void EmptyCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void InnerJoin()
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
                const string sql = ""SELECT * FROM Users INNER JOIN Departments ON Departments.DepartmentId = Users.DepartmentId"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "DapperSqlStyleAnalyser",
                Message = "'\"SELECT * FROM Users INNER JOIN Departments ON Departments.DepartmentId = Users.DepartmentId\"' contains incorrectly cased reserve words",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 15, 30)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
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
                const string sql = ""SELECT * FROM Users JOIN Departments ON Departments.DepartmentId = Users.DepartmentId"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void NotDapper()
        {
            var test = @"
namespace DapperDemo
{
    using System;
    using System.Collections.Generic;

    public class Class2
    {
        public IEnumerable<Customer> A()
        {
            using (var connection = new Object())
            {
                const string x = ""select* from Person.Person where FirstName = 'Mark'"";
            return connection.Query<Customer>(x);
        }
    }
}
}";

            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DapperSqlStyleAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperSqlStyleAnalyzer();
        }
    }
}
