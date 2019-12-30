using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DapperAnalyser.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void EmptyCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [Ignore]
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

        [TestMethod]
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
        [TestMethod]
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

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DapperAnalyserCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperAnalyserAnalyzer();
        }
    }
}
