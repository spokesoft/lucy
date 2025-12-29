using System.Text;

namespace Lucy.Application.Common.Filters;

/// <summary>
/// Parses a string query into a FilterNode tree.
/// </summary>
public class FilterParser<TField> where TField : struct, Enum
{
    private List<Token> _tokens = [];
    private int _pos;

    /// <summary>
    /// Parses the input string into a FilterNode.
    /// </summary>
    public FilterNode Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty", nameof(input));

        _tokens = Tokenize(input);
        _pos = 0;

        var node = ParseExpression();

        if (!Check(TokenType.EOF))
            throw new ArgumentException($"Unexpected token at position {_pos}: {_tokens[_pos].Value}");

        return node;
    }

    /// <summary>
    /// Tokenizes the input string into a list of tokens.
    /// </summary>
    private static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var i = 0;
        while (i < input.Length)
        {
            char c = input[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (c == '(') { tokens.Add(new Token(TokenType.OpenParen, "(")); i++; continue; }
            if (c == ')') { tokens.Add(new Token(TokenType.CloseParen, ")")); i++; continue; }
            if (c == ',') { tokens.Add(new Token(TokenType.Comma, ",")); i++; continue; }

            if (IsOperatorStart(c))
            {
                var op = ReadOperator(input, ref i);
                tokens.Add(new Token(TokenType.Operator, op));
                continue;
            }

            if (IsStringStart(c))
            {
                var str = ReadString(input, ref i);
                tokens.Add(new Token(TokenType.String, str));
                continue;
            }

            if (IsNumberStart(input, i))
            {
                var date = ReadDate(input, ref i);
                if (date != null)
                {
                    tokens.Add(new Token(TokenType.Date, date));
                }
                else
                {
                    var number = ReadNumber(input, ref i);
                    tokens.Add(new Token(TokenType.Number, number));
                }
                continue;
            }

            if (IsWordStart(c))
            {
                var word = ReadWord(input, ref i);
                if (word.Equals("AND", StringComparison.OrdinalIgnoreCase))
                    tokens.Add(new Token(TokenType.And, "AND"));
                else if (word.Equals("OR", StringComparison.OrdinalIgnoreCase))
                    tokens.Add(new Token(TokenType.Or, "OR"));
                else if (word.Equals("IN", StringComparison.OrdinalIgnoreCase))
                    tokens.Add(new Token(TokenType.Operator, "IN"));
                else if (word.Equals("LIKE", StringComparison.OrdinalIgnoreCase))
                    tokens.Add(new Token(TokenType.Operator, "LIKE"));
                else
                    tokens.Add(new Token(TokenType.Identifier, word));
                continue;
            }

            if (IsSymbolicStart(c))
            {
                var sym = ReadSymbol(input, ref i);
                if (sym == "&&")
                    tokens.Add(new Token(TokenType.And, "&&"));
                else if (sym == "||")
                    tokens.Add(new Token(TokenType.Or, "||"));
                else
                    throw new ArgumentException($"Invalid symbolic operator: {sym}");
                continue;
            }

            throw new ArgumentException($"Unexpected character: {c} at position {i}");
        }
        tokens.Add(new Token(TokenType.EOF, string.Empty));
        return tokens;
    }

    /// <summary>
    /// Checks if the character can start an operator.
    /// </summary>
    private static bool IsOperatorStart(char c) => "=!<>~".Contains(c);

    /// <summary>
    ///  Reads an operator from the input string.
    /// </summary>
    private static string ReadOperator(string input, ref int i)
    {
        int start = i;
        char c = input[i];
        i++;
        if (i < input.Length)
        {
            char next = input[i];
            if ((c == '!' && next == '=') ||
                (c == '<' && next == '=') ||
                (c == '>' && next == '='))
            {
                i++;
            }
        }
        return input[start..i];
    }

    /// <summary>
    /// Checks if the character can start a string.
    /// </summary>
    private static bool IsStringStart(char c) => c == '\'' || c == '"';

    /// <summary>
    /// Reads a string literal from the input string.
    /// </summary>
    private static string ReadString(string input, ref int i)
    {
        char quote = input[i];
        i++; // Skip opening quote
        var builder = new StringBuilder();
        while (i < input.Length)
        {
            if (input[i] == quote)
            {
                i++; // Skip closing quote
                return builder.ToString();
            }
            builder.Append(input[i]);
            i++;
        }
        throw new ArgumentException("Unterminated string literal");
    }

    /// <summary>
    /// Checks if the character can be part of a word.
    /// </summary>
    public static bool IsWordStart(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '-';

    /// <summary>
    /// Reads a word from the input string.
    /// </summary>
    private static string ReadWord(string input, ref int i)
    {
        int start = i;
        while (i < input.Length && IsWordStart(input[i]))
        {
            i++;
        }
        return input[start..i];
    }

    /// <summary>
    /// Checks if the character can start a shorthand logical operator.
    /// </summary>
    private static bool IsSymbolicStart(char c) => c == '&' || c == '|';

    /// <summary>
    /// Reads a shorthand logical operator from the input string.
    /// </summary>
    private static string ReadSymbol(string input, ref int i)
    {
        char c = input[i];
        i++;
        if (i < input.Length && input[i] == c)
        {
            i++;
            return new string(c, 2);
        }
        throw new ArgumentException($"Invalid shorthand operator at position {i - 1}");
    }

    /// <summary>
    /// Checks if the position can start a number.
    /// </summary>
    private static bool IsNumberStart(string input, int i)
    {
        char c = input[i];
        if (char.IsDigit(c)) return true;
        if (c == '-' && i + 1 < input.Length && char.IsDigit(input[i + 1])) return true;
        return false;
    }

    /// <summary>
    /// Reads a numeric literal from the input string.
    /// </summary>
    private static string ReadNumber(string input, ref int i)
    {
        int start = i;

        if (input[i] == '-')
            i++;

        while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
            i++;

        return input[start..i];
    }

    /// <summary>
    /// Reads a date literal from the input string.
    /// </summary>
    private static string? ReadDate(string input, ref int i)
    {
        int start = i;

        while (i < input.Length && IsDateTimeChar(input[i]))
            i++;

        var value = input[start..i];

        if (DateTime.TryParse(value, out _))
            return value;

        i = start; // Reset if not a valid date
        return null;
    }

    /// <summary>
    /// Checks if the character can be part of a datetime literal.
    /// </summary>
    private static bool IsDateTimeChar(char c) =>
        char.IsDigit(c) || c == '-' || c == ':' || c == 'T' || c == '.';

    /// <summary>
    /// Reads a numeric or date literal from the input string.
    /// Tries to parse as date first (e.g., 2024-01-15 or 2024-01-15T14:30:00), otherwise as number.
    /// </summary>
    private static Token ReadNumberOrDate(string input, ref int i)
    {
        int start = i;

        // Handle optional negative sign
        if (input[i] == '-')
            i++;

        // Read digits and allowed date/number characters: digits, -, :, T, .
        while (i < input.Length && IsNumberOrDateChar(input[i]))
            i++;

        var value = input[start..i];

        // Try parsing as DateTime first
        if (DateTime.TryParse(value, out _))
            return new Token(TokenType.Date, value);

        // Otherwise treat as number
        return new Token(TokenType.Number, value);
    }

    /// <summary>
    /// Checks if the character can be part of a number or date literal.
    /// </summary>
    private static bool IsNumberOrDateChar(char c) =>
        char.IsDigit(c) || c == '-' || c == ':' || c == 'T' || c == '.';

    /// <summary>
    /// Parses an expression into a FilterNode.
    /// </summary>
    private FilterNode ParseExpression()
    {
        var left = ParseTerm();

        while (Match(TokenType.Or))
        {
            var right = ParseTerm();
            left = new FilterGroup(LogicOperator.Or, [left, right]);
        }

        return left;
    }

    /// <summary>
    /// Parses a term into a FilterNode.
    /// </summary>
    private FilterNode ParseTerm()
    {
        var left = ParseFactor();

        while (Match(TokenType.And))
        {
            var right = ParseFactor();
            left = new FilterGroup(LogicOperator.And, [left, right]);
        }

        return left;
    }

    /// <summary>
    /// Parses a factor into a FilterNode.
    /// </summary>
    private FilterNode ParseFactor()
    {
        if (Match(TokenType.OpenParen))
        {
            var node = ParseExpression();
            Consume(TokenType.CloseParen);
            return node;
        }

        return ParseCondition();
    }

    /// <summary>
    /// Parses a single filter condition into a FilterCriterion.
    /// </summary>
    private FilterCriterion<TField> ParseCondition()
    {
        var fieldToken = Consume(TokenType.Identifier);

        if (!Enum.TryParse<TField>(fieldToken.Value, true, out var field))
            throw new ArgumentException($"Unknown field: {fieldToken.Value}");

        var opToken = Consume(TokenType.Operator);
        var op = FilterParser<TField>.ParseOperator(opToken.Value);

        object? value;
        if (op == FilterOperator.In)
        {
            value = ParseList();
        }
        else
        {
            value = ParseValue();
        }

        return new FilterCriterion<TField>(field, op, value);
    }

    /// <summary>
    /// Parses a list of values for the IN operator.
    /// </summary>
    private List<object> ParseList()
    {
        Consume(TokenType.OpenParen);
        var list = new List<object>();

        if (!Check(TokenType.CloseParen))
        {
            do
            {
                list.Add(ParseValue());
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.CloseParen);
        return list;
    }

    /// <summary>
    /// Parses a single value (string, number, date, or identifier).
    /// </summary>
    private object ParseValue()
    {
        if (Match(TokenType.String, out var strToken))
            return strToken.Value;

        if (Match(TokenType.Date, out var dateToken))
        {
            if (DateTime.TryParse(dateToken.Value, out var dateVal))
                return dateVal;
            throw new ArgumentException($"Invalid date format: {dateToken.Value}");
        }

        if (Match(TokenType.Number, out var numToken))
        {
            if (int.TryParse(numToken.Value, out var intVal))
                return intVal;
            if (long.TryParse(numToken.Value, out var longVal))
                return longVal;
            if (decimal.TryParse(numToken.Value, out var decVal))
                return decVal;
            throw new ArgumentException($"Invalid number format: {numToken.Value}");
        }

        if (Match(TokenType.Identifier, out var idToken))
            return idToken.Value; // Treat unquoted identifiers as strings/enums

        throw new ArgumentException($"Expected value at token {_tokens[_pos].Type}");
    }

    /// <summary>
    /// Parses the string representation of an operator into a FilterOperator enum.
    /// </summary>
    private static FilterOperator ParseOperator(string op)
    {
        return op.ToUpperInvariant() switch
        {
            "=" or "EQ" => FilterOperator.Equals,
            "!=" or "NEQ" => FilterOperator.NotEquals,
            ">" or "GT" => FilterOperator.GreaterThan,
            ">=" or "GTE" => FilterOperator.GreaterThanOrEqual,
            "<" or "LT" => FilterOperator.LessThan,
            "<=" or "LTE" => FilterOperator.LessThanOrEqual,
            "LIKE" or "~" => FilterOperator.Contains,
            "IN" => FilterOperator.In,
            _ => throw new ArgumentException($"Unknown operator: {op}")
        };
    }

    /// <summary>
    /// Matches the current token type and advances the position if matched.
    /// </summary>
    private bool Match(TokenType type)
    {
        if (Check(type))
        {
            _pos++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Matches the current token type and advances the position if matched, returning the token.
    /// </summary>
    private bool Match(TokenType type, out Token token)
    {
        if (Check(type))
        {
            token = _tokens[_pos];
            _pos++;
            return true;
        }
        token = default!;
        return false;
    }

    /// <summary>
    /// Checks if the current token matches the given type.
    /// </summary>
    private bool Check(TokenType type)
    {
        return _pos < _tokens.Count && _tokens[_pos].Type == type;
    }

    /// <summary>
    /// Consumes the current token if it matches the given type, otherwise throws an exception.
    /// </summary>
    private Token Consume(TokenType type)
    {
        if (Check(type))
        {
            return _tokens[_pos++];
        }
        throw new ArgumentException($"Expected {type} but found {_tokens[_pos].Type}");
    }

    /// <summary>
    /// Token representation for the parser.
    /// </summary>
    private record Token(TokenType Type, string Value);

    /// <summary>
    /// Types of tokens recognized by the parser.
    /// </summary>
    private enum TokenType {
        Identifier,
        String,
        Number,
        Date,
        OpenParen,
        CloseParen,
        Comma,
        And,
        Or,
        Operator,
        EOF
    }
}
