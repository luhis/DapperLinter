namespace DapperAnalyser
{
    public static class SqlStringStyleValidator
    {
        public static bool IsValid(string s) => s == SqlStringStyleFixer.Fix(s);
    }
}