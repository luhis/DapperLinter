namespace DapperAnalyser.Test
{
    using FluentAssertions;
    using Xunit;

    public class SqlStringStyleFixerUnitTests
    {
        [Theory]
        [InlineData(
            "SELECT * FROM Users inner join Departments on Departments.DepartmentId = Users.DepartmentId", 
            "SELECT * FROM Users JOIN Departments on Departments.DepartmentId = Users.DepartmentId")]
        [InlineData(
            "SELECT * FROM Users join Departments on Departments.DepartmentId = Users.DepartmentId",
            "SELECT * FROM Users join Departments on Departments.DepartmentId = Users.DepartmentId")]
        [InlineData("insert into Users (UserId) values (1)", "INSERT Users (UserId) values (1)")]
        public void ShouldUpperCaseReserveWords(string input, string output)
        {
            SqlStringStyleFixer.Fix(input).Should().Be(output);
        }

        [Fact]
        public void ShouldNotInterfereWithColNames()
        {
            SqlStringStyleFixer.Fix("SELECT MahSelect from MahTable").Should().Be("SELECT MahSelect from MahTable");
        }
    }
}
