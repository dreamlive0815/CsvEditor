using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvEditor
{
    class CsvTest
    {

        public static void Test(Csv csv)
        {
            var parser = CsvCommandParser.GetInstance();
            parser.ParseUpdaters("Name = 11, Value = 'dasd  dasd', 'Cnt(),' = Cnt + 1");
            
        }
        
    }

    class CsvCommandRunner
    {
        private static CsvCommandRunner instance;

        public static CsvCommandRunner GetInstance()
        {
            if (instance == null)
            {
                instance = new CsvCommandRunner();
            }
            return instance;
        }

        public event Action<CsvCommand, Exception> OnError; 

        private CsvCommandRunner() { }

        
        public CsvCommandQueryResult Run(CsvCommand cmd)
        {
            return null;
        }
    }

    enum CsvCommandType
    {
        Unknown,
        Select,
        Update,
        Delete,
    }

    abstract class CsvCommand
    {

        public static CsvCommandType GetCommandType(string cmd)
        {
            if (CsvSelectCommand.Pattern.IsMatch(cmd)) return CsvCommandType.Select;

            return CsvCommandType.Unknown;
        }

        public static CsvCommand Parse(string cmd)
        {
            var type = GetCommandType(cmd);
            switch (type)
            {
                case CsvCommandType.Select:
                    return new CsvSelectCommand(cmd);

            }
            throw new Exception("Unknown command type");
        }

        public static List<CsvVLine> GetVLinesByCondition(Csv csv, ICondition condition)
        {
            var r = new List<CsvVLine>();
            for (var i = 0; i < csv.VLineCount; i++)
            {
                var vLine = csv.GetVLine(i);
                if (condition.GetBool(vLine))
                    r.Add(vLine);
            }
            return r;
        }

        protected CsvCommand(string s)
        {
            PlainCommand = s;
        }

        public string PlainCommand { get; private set; }

        public abstract CsvCommandQueryResult Query(Csv csv);
    }

    enum CsvCommandQueryResultType
    {
        Bool,
        Int,
        Csv,
    }

    class CsvCommandQueryResult
    {

        public CsvCommandQueryResultType Type { get; set; }

        public bool Boolean { get; set; }

        public int Integer { get; set; }

        public Csv Csv { get; set; }
    }


    class CsvSelectCommand : CsvCommand
    {
        public readonly static Regex Pattern = new Regex(@"SELECT\s(.+?)\sWHERE\s(.+)", RegexOptions.IgnoreCase);

        public CsvSelectCommand(string s) : base(s)
        {
            var parser = CsvCommandParser.GetInstance();
            var match = Pattern.Match(s);
            if (!match.Success)
                throw new Exception("Cannot parse into select command");
            var headerStr = match.Groups[1].Value;
            Headers = parser.ParseHeaders(headerStr);
            AllHeaders = Headers.Count == 1 && Headers[0] == "*";
            var conditionStr = match.Groups[2].Value;
            Condition = parser.ParseCondition(conditionStr);
        }

        public ICondition Condition { get; private set; }

        public bool AllHeaders { get; private set; }

        public List<string> Headers { get; private set; }

        public Csv DoSelect(Csv csv)
        {
            var headers = AllHeaders ? csv.Headers : Headers;
            var values = new List<List<string>>();
            var vLines = GetVLinesByCondition(csv, Condition);
            for (var i = 0; i < vLines.Count; i++)
            {
                var vLine = vLines[i];
                values.Add(vLine.Values);
            }
            var r = new VirtualCsv(csv, headers, values);
            return r;
        }

        public override CsvCommandQueryResult Query(Csv csv)
        {
            return new CsvCommandQueryResult()
            {
                Type = CsvCommandQueryResultType.Csv,
                Csv = DoSelect(csv),
            };
        }
    }

    

    class CsvUpdater
    {
        public static Action<CsvVLine> GetSimpleUpdateHandler(string header, string value)
        {
            return new Action<CsvVLine>((vLine) =>
            {
                vLine.SetValue(header, value);
            });
        }

        public Action<CsvVLine> UpdateHandler { get; set; }
    }

    class CsvUpdateCommand : CsvCommand
    {
        public readonly static Regex Pattern = new Regex(@"UPDATE\s+SET\s(.+?)\sWHERE\s(.+)", RegexOptions.IgnoreCase);

        public CsvUpdateCommand(string s) : base(s)
        {
            var parser = CsvCommandParser.GetInstance();
            var match = Pattern.Match(s);
            if (!match.Success)
                throw new Exception("Cannot parse into update command");
            var updateStr = match.Groups[1].Value;
            Updaters = parser.ParseUpdaters(updateStr);
            var conditionStr = match.Groups[2].Value;
            Condition = parser.ParseCondition(conditionStr);
        }

        public List<CsvUpdater> Updaters { get; private set; }

        public ICondition Condition { get; private set; }

        public int DoUpdate(Csv csv)
        {
            var values = new List<List<string>>();
            var vLines = GetVLinesByCondition(csv, Condition);
            for (var i = 0; i < vLines.Count; i++)
            {
                var vLine = vLines[i];
                for (var j = 0; j < Updaters.Count; j++)
                {
                    var updater = Updaters[j];
                    updater.UpdateHandler(vLine);
                }
            }
            return vLines.Count;
        }

        public override CsvCommandQueryResult Query(Csv csv)
        {
            return new CsvCommandQueryResult()
            {
                Type = CsvCommandQueryResultType.Int,
                Integer = DoUpdate(csv),
            };
        }
    }

    class CsvCommandParser
    {
        private static CsvCommandParser instance;

        public static CsvCommandParser GetInstance()
        {
            if (instance == null)
            {
                instance = new CsvCommandParser();
            }
            return instance;
        }

        private CsvCommandParser() { }

        /// <summary>
        /// %sq% 单引号
        /// %dq% 双引号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Decode(string s)
        {
            s = s.Replace("%sq%", "'").Replace("%dq%", "\"");
            return s;
        }

        public List<string> GetTokens(string s)
        {
            var r = new List<string>();
            var inQuote = false;
            var token = new StringBuilder();
            var addToken = new Action(() =>
            {
                if (token.Length == 0) return;
                var rawToken = token.ToString();
                token.Clear();
                rawToken = Decode(rawToken);
                r.Add(rawToken);
            });
            for (var i = 0; i < s.Length; i++)
            {
                var ch = s[i];
                if (ch == ' ')
                {
                    if (inQuote)
                    {
                        token.Append(ch);
                    }
                    else
                    {
                        addToken();
                    }
                }
                else if (ch == '\'' || ch == '"')
                {
                    inQuote = !inQuote;
                }
                else if (ch == '(' || ch == ')' || ch == ',')
                {
                    if (!inQuote)
                    {
                        addToken();
                        token.Append(ch);
                        addToken();
                    }
                    else
                    {
                        token.Append(ch);
                    }
                }
                else
                {
                    token.Append(ch);
                }
            }
            addToken();
            return r;
        }

        private enum ConditionTokenType
        {
            Key,
            Operator,
            Value,
            ContextOperator,
        }

        private Dictionary<string, TernaryOperator> ternaryOperatorMap = new Dictionary<string, TernaryOperator>()
        {
            { "=", TernaryOperator.Equal },
            { "==", TernaryOperator.Equal },
            { "!=", TernaryOperator.NotEqual },
            { "<>", TernaryOperator.NotEqual },
            { ">", TernaryOperator.Greater },
            { "<", TernaryOperator.Less },
            { ">=", TernaryOperator.GreaterOrEqual },
            { "<=", TernaryOperator.LessOrEqual },
            { "has", TernaryOperator.Has },
            { "hasnot", TernaryOperator.Hasnot },
        };

        private Dictionary<string, ContextOperator> contextOperatorMap = new Dictionary<string, ContextOperator>()
        {
            { "and", ContextOperator.And },
            { "or", ContextOperator.Or },
            { "&&", ContextOperator.And },
            { "||", ContextOperator.Or },
        };

        public ICondition ParseCondition(string condition)
        {
            condition = condition.Trim();
            if (condition == "1" || condition.ToLower() == "true")
                return new TrueCondition();
            var tokens = GetTokens(condition);
            var stack = new Stack<ConditionCollection>();
            var root = new ConditionCollection();
            stack.Push(root);
            var curTokenType = ConditionTokenType.Key;
            var nextTokenType = ConditionTokenType.Key;
            var curContextOperator = ContextOperator.And;
            var curCondition = new TernaryCondition();
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token == "(")
                {
                    var child = new ConditionCollection();
                    child.ContextOperator = curContextOperator;
                    stack.Peek().Add(child);
                    stack.Push(child);
                    nextTokenType = ConditionTokenType.Key;
                    continue;
                }
                if (token == ")")
                {
                    if (stack.Count == 1)
                        throw new Exception("Unexpected char ')'");
                    stack.Pop();
                    nextTokenType = ConditionTokenType.ContextOperator;
                    continue;
                }
                curTokenType = nextTokenType;
                switch (curTokenType)
                {
                    case ConditionTokenType.Key:
                        curCondition.Key = token;
                        nextTokenType = ConditionTokenType.Operator;
                        break;
                    case ConditionTokenType.Operator:
                        token = token.ToLower();
                        var isOperator = false;
                        if (ternaryOperatorMap.ContainsKey(token))
                        {
                            isOperator = true;
                            curCondition.Operator = ternaryOperatorMap[token];
                            nextTokenType = ConditionTokenType.Value;
                        }
                        if (!isOperator)
                            throw new Exception(string.Format("Unsupported operator: {0}", token));
                        break;
                    case ConditionTokenType.Value:
                        curCondition.Value = token;
                        nextTokenType = ConditionTokenType.ContextOperator;
                        break;
                    case ConditionTokenType.ContextOperator:
                        token = token.ToLower();
                        if (!contextOperatorMap.ContainsKey(token))
                            throw new Exception(string.Format("Unsupported context operator: {0}", token));
                        curContextOperator = contextOperatorMap[token];
                        nextTokenType = ConditionTokenType.Key;
                        break;
                }
                if (curTokenType == ConditionTokenType.Value)
                {
                    curCondition.ContextOperator = curContextOperator;
                    stack.Peek().Add(curCondition);
                    curCondition = new TernaryCondition();
                }
            }

            if (curTokenType != ConditionTokenType.Value)
            {
                throw new Exception("Condition statement ends unexpectedly");
            }
            return root;
        }

        public List<string> ParseHeaders(string headers)
        {
            var tokens = GetTokens(headers);
            var r = new List<string>();
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token == ",") continue;
                r.Add(token);
            }
            return r;
        }

        public List<CsvUpdater> ParseUpdaters(string updaters)
        {
            var tokens = GetTokens(updaters);
            tokens.Add(",");
            var tokensList = new List<List<string>>();
            var ts = new List<string>();
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token == ",")
                {
                    if (ts.Count > 0)
                        tokensList.Add(ts);
                    ts = new List<string>();
                }
                else
                {
                    ts.Add(token);
                }
            }

            var r = new List<CsvUpdater>();
            return r;
        }
    }

    interface ICondition
    {
        ContextOperator ContextOperator { get; set; }

        bool GetBool(CsvVLine line);
    }

    class TrueCondition : ICondition
    {
        public ContextOperator ContextOperator { get; set; }

        public bool GetBool(CsvVLine line)
        {
            return true;
        }
    }

    class ConditionCollection : List<ICondition>, ICondition
    {
        public ContextOperator ContextOperator { get; set; }

        public bool GetBool(CsvVLine line)
        {
            var r = true;
            for (var i = 0; i < this.Count; i++)
            {
                var child = this[i];
                var b = child.GetBool(line);
                if (child.ContextOperator == ContextOperator.And)
                    r = r && b;
                else
                    r = r || b;
            }
            return r;
        }
    }

    class BinaryCondition : ICondition
    {
        public string Key { get; set; }

        public BinaryOperator Operator { get; set; }

        public ContextOperator ContextOperator { get; set; }

        public bool GetBool(CsvVLine line)
        {
            if (!line.HasHeader(Key))
                throw new Exception(string.Format("The key: {0} is not a legal header", Key));
            return false;
        }
    }

    class TernaryCondition : ICondition
    {
        public string Key { get; set; }

        public TernaryOperator Operator { get; set; }

        public string Value { get; set; }

        public ContextOperator ContextOperator { get; set; }

        public bool GetBool(CsvVLine line)
        {
            if (!line.HasHeader(Key))
                throw new Exception(string.Format("The key: {0} is not a legal header", Key));
            var v1 = line.GetValue(Key);
            var v2 = Value;
            switch (Operator)
            {
                case TernaryOperator.Equal:
                    return string.Compare(v1, v2) == 0;
                case TernaryOperator.NotEqual:
                    return string.Compare(v1, v2) != 0;
                case TernaryOperator.Greater:
                    return string.Compare(v1, v2) > 0;
                case TernaryOperator.Less:
                    return string.Compare(v1, v2) < 0;
                case TernaryOperator.GreaterOrEqual:
                    return string.Compare(v1, v2) >= 0;
                case TernaryOperator.LessOrEqual:
                    return string.Compare(v1, v2) <= 0;
                case TernaryOperator.Has:
                    return new Regex(v2).IsMatch(v1);
                case TernaryOperator.Hasnot:
                    return !new Regex(v2).IsMatch(v1);
                default:
                    throw new Exception("Unsupported ternary operator");
            }
        }
    }

    enum ContextOperator
    {
        And,
        Or,
    }

    enum BinaryOperator
    {
    }

    enum TernaryOperator
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
        Has,
        Hasnot,
    }
}
