using FluentAssertions;
using Xunit;

namespace DapperAnalyser.Test
{
    public class SqlReservedWordsCaserUnitTests
    {
        [Theory]
        [InlineData("SELECT * FROM Users", "SELECT * FROM Users")]
        [InlineData("select * FROM Users", "SELECT * FROM Users")]
        [InlineData("seLect * FROM Users", "SELECT * FROM Users")]
        [InlineData("SELECT * from Users", "SELECT * FROM Users")]
        [InlineData("SELECT * FROM Users where UserId = 1 and Name = 'Dave'", "SELECT * FROM Users WHERE UserId = 1 AND Name = 'Dave'")]
        [InlineData(
            "SELECT * FROM Users inner join Departments on Departments.DepartmentId = Users.DepartmentId",
            "SELECT * FROM Users INNER JOIN Departments ON Departments.DepartmentId = Users.DepartmentId")]
        [InlineData(
            "SELECT * FROM Users join Departments on Departments.DepartmentId = Users.DepartmentId",
            "SELECT * FROM Users JOIN Departments ON Departments.DepartmentId = Users.DepartmentId")]
        [InlineData("insert into Users (UserId) values (1)", "INSERT INTO Users (UserId) VALUES (1)")]
        [InlineData("update Users set name = 1 where id = 2", "UPDATE Users SET name = 1 WHERE id = 2")]
        [InlineData("SELECT * into newtable FROM oldtable", "SELECT * INTO newtable FROM oldtable")]
        public void ShouldUpperCaseReserveWords(string input, string output)
        {
            SqlReservedWordsCaserFixer.Fix(input).Should().Be(output);
        }

        [Fact]
        public void ShouldNotInterfereWithColNames()
        {
            SqlReservedWordsCaserFixer.Fix("SELECT MahSelect from MahTable").Should().Be("SELECT MahSelect FROM MahTable");
        }
    }
}