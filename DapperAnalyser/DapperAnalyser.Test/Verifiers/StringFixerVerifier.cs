namespace DapperAnalyser.Test.Verifiers
{
    using FluentAssertions;
    using Xunit;

    public class StringFixerVerifier
    {
        [Theory]
        [InlineData("SELECT * FROM Users", "SELECT * FROM Users")]
        [InlineData("select * FROM Users", "SELECT * FROM Users")]
        [InlineData("seLect * FROM Users", "SELECT * FROM Users")]
        [InlineData("SELECT * from Users", "SELECT * FROM Users")]
        [InlineData("SELECT * FROM Users where UserId = 1", "SELECT * FROM Users WHERE UserId = 1")]
        [InlineData(
            "SELECT * FROM Users inner join Departments on Departments.DepartmentId = Users.DepartmentId", 
            "SELECT * FROM Users JOIN Departments ON Departments.DepartmentId = Users.DepartmentId")]
        public void ShouldUpperCaseReserveWords(string input, string output)
        {
            SqlStringFixer.Fix(input).Should().Be(output);
        }
    }
}
