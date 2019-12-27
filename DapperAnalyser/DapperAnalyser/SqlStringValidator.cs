namespace DapperAnalyser
{
    public static class SqlStringValidator
    {
        public static bool IsValid(string s)
        {
            return s == SqlStringFixer.Fix(s);
        }
    }
}