using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvEditor
{
    abstract class Csv
    {
        public static Csv FromFile(string filePath)
        {
            return new CsvFromFile(filePath);
        }

        public static string FilterGetValue(string value)
        {
            return value;
        }

        public static string FilterSetValue(string value)
        {
            return value;
        }

        protected List<string> types;
        protected List<string> headers;
        protected List<List<string>> values;

        protected Dictionary<string, int> headerIndexes;
        protected bool changed;

        protected Csv()
        {
            changed = false;
        }

        public List<string> Headers
        {
            get { return headers; }
        }

        public int VLineCount
        {
            get { return values.Count; }
        }

        public bool Changed { get { return changed; } }

        public void SetChanged()
        {
            changed = true;
        }

        public void SaveAs(string filePath)
        {
            var sb = new StringBuilder();
            var nl = Environment.NewLine;
            var joinLine = new Action<List<string>>((lineValues) =>
            {
                var line = string.Join(",", lineValues);
                sb.Append(line);
            });
            joinLine(types);
            sb.Append(nl);
            joinLine(headers);
            sb.Append(nl);
            for (var i = 0; i < values.Count; i++)
            {
                joinLine(values[i]);
                sb.Append(nl);
            }

            File.WriteAllText(filePath, sb.ToString());
            changed = false;
        }

        public CsvVLine GetVLine(int index)
        {
            return new CsvVLine(this, values[index]);
        }

        public bool HasHeader(string header)
        {
            return headerIndexes.ContainsKey(header);
        }

        public virtual int GetHeaderIndex(string header)
        {
            return headerIndexes[header];
        }

        public string GetHeaderType(string header)
        {
            var index = GetHeaderIndex(header);
            return types[index];
        }

        public string GetValue(int lineIndex, string header)
        {
            var lineValues = values[lineIndex];
            var headerIndex = GetHeaderIndex(header);
            var value = lineValues[headerIndex];
            value = FilterGetValue(value);
            return value;
        }

        public void SetValue(int lineIndex, string header, string value)
        {
            var lineValues = values[lineIndex];
            var headerIndex = GetHeaderIndex(header);
            value = FilterSetValue(value);
            lineValues[headerIndex] = value;
            changed = true;
        }

        public abstract void Save();

        protected void InitHeaderIndexes()
        {
            headerIndexes = new Dictionary<string, int>();
            for (var i = 0; i < headers.Count; i++)
            {
                headerIndexes.Add(headers[i], i);
            }
        }
    }

    class CsvVLine
    {
        private Csv parent;
        private List<string> values;

        public CsvVLine(Csv parent, List<string> values)
        {
            this.parent = parent;
            this.values = values;
        }

        public List<string> Values
        {
            get { return values; }
        }

        public bool HasHeader(string header)
        {
            return parent.HasHeader(header);
        }

        public string GetValue(string header)
        {
            var index = parent.GetHeaderIndex(header);
            var value = values[index];
            value = Csv.FilterGetValue(value);
            return value;
        }

        public void SetValue(string header, string value)
        {
            var index = parent.GetHeaderIndex(header);
            value = Csv.FilterSetValue(value);
            values[index] = value;
            parent.SetChanged();
        }

        public override string ToString()
        {
            var nl = Environment.NewLine;
            var sp = new string(' ', 4);
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(nl);
            for (var i = 0; i < parent.Headers.Count; i++)
            {
                var header = parent.Headers[i];
                sb.Append(sp);
                sb.Append(header);
                sb.Append(" = ");
                sb.Append(GetValue(header));
                sb.Append(nl);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    class CsvFromFile : Csv
    {
        private string filePath;

        public CsvFromFile(string filePath) : base()
        {
            this.filePath = filePath;
            ParseFromFile(filePath);
            InitHeaderIndexes();
        }

        public override void Save()
        {
            SaveAs(filePath);
        }

        private List<string> SplitLine(string s)
        {
            s += ',';
            var r = new List<string>();
            var offset = 0;
            var inQuote = false;
            for (var i = 0; i < s.Length; i++)
            {
                var ch = s[i];
                if (ch == '"')
                {
                    inQuote = !inQuote;
                }
                else if (ch == ',')
                {
                    if (!inQuote)
                    {
                        var ss = s.Substring(offset, i - offset);
                        offset = i + 1;
                        r.Add(ss);
                    }
                }
            }
            return r;
        }

        private void ParseFromFile(string filePath)
        {
            values = new List<List<string>>();
            var lines = File.ReadAllLines(filePath);
            var typeF = false;
            var headerF = false;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (!typeF)
                {
                    types = SplitLine(line);
                    typeF = true;
                }
                else if (!headerF)
                {
                    headers = SplitLine(line);
                    if (types.Count != headers.Count)
                        throw new Exception("The number of types and headers is different");
                    headerF = true;
                }
                else
                {
                    var lineValues = SplitLine(line);
                    if (headers.Count != lineValues.Count)
                        throw new Exception(string.Format("Line {0}:the number of headers and lineValues is different", i + 1));
                    values.Add(lineValues);
                }
            }
        }
    }

    class VirtualCsv : Csv
    {
        private Csv parent;

        public VirtualCsv(Csv parent, List<string> headers, List<List<string>> values) : base()
        {
            this.parent = parent;
            var types = new List<string>();
            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (!parent.HasHeader(header))
                    throw new Exception(string.Format("The header: {0} does not exist in parent csv", header));
                var type = parent.GetHeaderType(header);
                types.Add(type);
            }
            this.types = types;
            this.headers = headers;
            this.values = values;
            InitHeaderIndexes();
        }

        public override int GetHeaderIndex(string header)
        {
            var index = parent.GetHeaderIndex(header);
            return index;
        }

        public override void Save()
        {
            parent.Save();
        }
    }
}
