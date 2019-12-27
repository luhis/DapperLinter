namespace DapperAnalyser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class SqlStringFixer
    {
        private static Regex ToRegex(string s) =>
            new Regex($@"{s}\s?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<Regex, string> Mappings = new Dictionary<Regex, string>()
        {
            {ToRegex("select"), "SELECT "},
            {ToRegex("from"), "FROM "},
            {ToRegex("where"), "WHERE "},
            {ToRegex("on"), "ON "},
            {ToRegex("inner join"), "JOIN "},
            {ToRegex("insert into"), "INSERT"},
        };

        public static string Fix(string s)
        {
            return Mappings.Aggregate(s, (a, b) => { return b.Key.Replace(a, b.Value); });
        }
    }
}
