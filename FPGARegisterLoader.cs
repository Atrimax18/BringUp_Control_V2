using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BringUp_Control
{
    internal class FPGARegisterLoader
    {
        // Matches each table row and captures the FIRST td (Name) and SECOND td (Address).
        // Example row in your file:
        //   <tr ...><td align="left">dsp_cfg_ul[0].gain</td><td align="right">0x000008</td>...
        private static readonly Regex RowRegex = new Regex(
            @"<tr\b[^>]*>\s*" +
            @"<td\b[^>]*>\s*(?<name>[^<]+)\s*</td>\s*" +
            @"<td\b[^>]*>\s*(?<addr>0x[0-9a-fA-F]+)\s*</td>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Converts "dsp_cfg_ul[0].gain" -> "dsp_cfg_ul_i0_gain"
        // Converts "dsp_ul_pwr_measure_status[3].power_sat_count" -> "dsp_ul_pwr_measure_status_i3_power_sat_count"
        // Converts "activate_loopback" -> "activate_loopback"
        public static string NormalizeRegisterNameToKey(string htmlName)
        {
            if (string.IsNullOrWhiteSpace(htmlName))
                throw new ArgumentException("Register name is empty.", nameof(htmlName));

            string s = htmlName.Trim();

            // Replace array indices: [0] -> _i0, [12] -> _i12
            s = Regex.Replace(s, @"\[(\d+)\]", m => "_i" + m.Groups[1].Value);

            // Replace '.' hierarchy separators with underscores
            s = s.Replace('.', '_');

            // Optional: collapse duplicate underscores
            s = Regex.Replace(s, @"_+", "_");

            // Optional: force lower-case to match your current dictionary style
            s = s.ToLowerInvariant();

            return s;
        }

        public Dictionary<string, uint> LoadRegisterMapFromHtml(string htmlPath)
        {
            if (string.IsNullOrWhiteSpace(htmlPath))
                throw new ArgumentException("Path is empty.", nameof(htmlPath));
            if (!File.Exists(htmlPath))
                throw new FileNotFoundException("HTML file not found.", htmlPath);

            string html = File.ReadAllText(htmlPath);

            var map = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

            foreach (Match m in RowRegex.Matches(html))
            {
                string rawName = WebDecodeBasic(m.Groups["name"].Value);
                string rawAddr = m.Groups["addr"].Value.Trim();

                string key = NormalizeRegisterNameToKey(rawName);
                uint addr = ParseHexUint(rawAddr);

                // If duplicates exist, last one wins (change to throw if you prefer)
                map[key] = addr;
            }

            if (map.Count == 0)
                throw new InvalidDataException("No register rows were parsed. Check HTML format / regex.");

            return map;
        }

        public static string GenerateCSharpDictionaryInitializer(Dictionary<string, uint> map, string dictName)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (string.IsNullOrWhiteSpace(dictName)) dictName = "RegisterMap";

            var sb = new StringBuilder();
            sb.AppendLine($"public static readonly Dictionary<string, uint> {dictName} = new Dictionary<string, uint>()");
            sb.AppendLine("{");

            foreach (var kv in map)
            {
                sb.AppendLine($"    {{ \"{kv.Key}\", 0x{kv.Value:X} }},");
            }

            sb.AppendLine("};");
            return sb.ToString();
        }

        private static uint ParseHexUint(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);

            return uint.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        // Minimal decode for common HTML entities that may appear in <td>.
        // (Your names are mostly plain, but this keeps it safe.)
        private static string WebDecodeBasic(string s)
        {
            return s
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&#39;", "'");
        }

    }
}
