using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace LocalUtilities
{
    internal class CSVHelper
    {
        private List<string[]> raw = new();

        public CSVHelper(string path)
        {
            using TextFieldParser parser = new(path);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
                raw.Add(parser.ReadFields() ?? []);
            raw.RemoveAll(arr => (arr.Length == 0 || arr.All(item => string.IsNullOrWhiteSpace(item))));
        }

        public string[,] Grid
            => raw.SelectMany((fields, rowIndex)
            => fields.Select((field, colIndex)
            => new { field, rowIndex, colIndex }))
            .Aggregate(new string[raw.Count, raw[0].Length], (res, x) =>
            {
                res[x.rowIndex, x.colIndex] = x.field;
                return res;
            });

        public Dictionary<string, string[]> DataTable
            => raw[0]
            .Skip(1)
            .Select((header, colIndex)
            => new KeyValuePair<string, string[]>(
                header,
                raw.Skip(1)
                    .Select(row => row[colIndex])
                    .TakeWhile(field => !string.IsNullOrWhiteSpace(field))
                    .ToArray()))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        public Dictionary<string, string[]> HeaderTable
            => raw
            .Select(row => row.First())
            .Skip(1)
            .TakeWhile(field => !string.IsNullOrWhiteSpace(field))
            .Aggregate(
                new List<List<string>> { new() },
                (acc, item) =>
                {
                    if (Regex.Match(item, "[A-Z]{2}").Success && acc.Last().Count > 1)
                        acc.Add([item]);
                    else
                        acc.Last().Add(item);
                    return acc;
                }
                )
            .Where(sublist => sublist.Count > 0)
            .Select((item)
                => new KeyValuePair<string, string[]>(item.First(), item.Skip(1).ToArray()))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        public string ToJson()
        {
            Dictionary<string, object> dict = new(){
                {"Counties", HeaderTable},
                {"Cities", DataTable}
            };
            JsonSerializerOptions options = new()
            {
                WriteIndented = false
            };
            return JsonSerializer.Serialize(dict, options);
        }
    }
}
