using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace DapperAnalyser.Test
{
    public class DapperSqlAnalyserUnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [Fact]
        public void EmptyCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void LowerCaseSelect()
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
            var expected = new DiagnosticResult
            {
                Id = "DapperAnalyser",
                Message = "'\"select * from Person.Person where FirstName = 'Mark'\"' contains incorrectly cased reserve words",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 15, 46)
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
                return connection.Query<int>(""SELECT * FROM Person.Person WHERE FirstName = 'Mark'"");
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void LowerCaseSelectSqlConst()
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
            var expected = new DiagnosticResult
            {
                Id = "DapperAnalyser",
                Message = "'\"select * from Person.Person where FirstName = 'Mark'\"' contains incorrectly cased reserve words",
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
                const string sql = ""SELECT * FROM Person.Person WHERE FirstName = 'Mark'"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact(Skip = "todo")]
        public void LowerCaseSelectSqlVariable()
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
                Id = "DapperAnalyser",
                Message = "'\"select * from Person.Person where FirstName = 'Mark'\"' contains incorrectly cased reserve words",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 15, 46)
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
                    var sql = ""SELECT * from Person.Person where FirstName = 'Mark'"";
                return connection.Query<int>(sql);
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void LowerCaseSelectAsync()
        {
            var test = @"
namespace DapperDemo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class C1
    {
        public async Task<IEnumerable<int>> A()
        {
            using (var connection = new SqlConnection(
                ""Server = tcp:mhknbn2kdz.database.windows.net; Database = AdventureWorks2012; User ID = sqlfamily; Password = sqlf@m1ly; ""))
            {
                return await connection.QueryAsync<int>(""select * from Person.Person where FirstName = 'Mark'"");
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "DapperAnalyser",
                Message = "'\"select * from Person.Person where FirstName = 'Mark'\"' contains incorrectly cased reserve words",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 16, 57)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace DapperDemo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class C1
    {
        public async Task<IEnumerable<int>> A()
        {
            using (var connection = new SqlConnection(
                ""Server = tcp:mhknbn2kdz.database.windows.net; Database = AdventureWorks2012; User ID = sqlfamily; Password = sqlf@m1ly; ""))
            {
                return await connection.QueryAsync<int>(""SELECT * FROM Person.Person WHERE FirstName = 'Mark'"");
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void UpperCaseSelect()
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
                return connection.Query<int>(""SELECT * FROM Person.Person WHERE FirstName = 'Mark'"");
            }
        }
    }
}";

            VerifyCSharpDiagnostic(test);
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
            return new DapperSqlAnalyserCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperSqlAnalyzer();
        }
    }
}
