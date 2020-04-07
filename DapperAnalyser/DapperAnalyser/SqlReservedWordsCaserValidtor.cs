namespace DapperAnalyser
{
    public static class SqlReservedWordsCaserValidtor
    {
        public static bool IsValid(string s) => s == SqlReservedWordsCaserFixer.Fix(s);
    }
}