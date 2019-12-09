namespace DapperAnalyser
{
    public static class SqlStringFixer
    {
        public static string Fix(string s) => s.Replace("select", "SELECT");
    }
}