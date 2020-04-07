using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DapperAnalyser
{
    public static class SqlReservedWordsCaserFixer
    {
        private static IEnumerable<string> ReadEmbeddedResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("DapperAnalyser.ReservedWords.txt"));
            return  textStreamReader.ReadToEnd()
                 .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        private static Regex ToRegex(string s) =>
            new Regex($@"\b{s}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static string ToUpper(string s) => s.ToUpper();

        private static readonly IEnumerable<(Regex r, string s)> ReservedWords = 
            new HashSet<string>(ReadEmbeddedResource().Select(ToUpper)).AsEnumerable().Select(a => (ToRegex(a), a));

        public static string Fix(string s)
        {
            return ReservedWords.Aggregate(s, (a, b) => b.r.Replace(a, b.s));
        }
    }
}