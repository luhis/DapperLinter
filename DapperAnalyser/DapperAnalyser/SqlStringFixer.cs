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
            {ToRegex("order by"), "ORDER BY"},
            {ToRegex("group by"), "GROUP BY"},
            {ToRegex("asc"), "ASC"},
            {ToRegex("desc"), "DESC"},
            {ToRegex("and"), "AND"},
            {ToRegex("or"), "OR"},
            {ToRegex("values"), "VALUES"},
            {ToRegex("update"), "UPDATE"},
            {ToRegex("set"), "SET"},
        };

        public static string Fix(string s)
        {
            return Mappings.Aggregate(s, (a, b) => b.Key.Replace(a, b.Value));
        }
    }
}
