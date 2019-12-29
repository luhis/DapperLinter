namespace DapperAnalyser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class SqlStringFixer
    {
        private static Regex ToRegex(string s) =>
            new Regex($@"\b{s}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly IReadOnlyDictionary<Regex, string> Mappings = new Dictionary<Regex, string>()
        {
            {ToRegex("select"), "SELECT"},
            {ToRegex("from"), "FROM"},
            {ToRegex("where"), "WHERE"},
            {ToRegex("on"), "ON"},
            {ToRegex("inner join"), "JOIN"},
            {ToRegex("join"), "JOIN"},
            {ToRegex("insert into"), "INSERT"},
            {ToRegex("insert"), "INSERT"},
        };

        public static string Fix(string s)
        {
            return Mappings.Aggregate(s, (a, b) => b.Key.Replace(a, b.Value));
        }
    }
}
