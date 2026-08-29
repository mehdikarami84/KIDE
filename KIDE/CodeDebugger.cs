using System;
using System.Collections.Generic;

namespace KIDE
{
    internal class CodeDebugger
    {
        private readonly string code;

        private readonly List<CodeError> errors =
            new List<CodeError>();

        // =========================================================
        // C++ Keywords
        // =========================================================

        private readonly HashSet<string> cppKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "alignas",
            "alignof",
            "and",
            "and_eq",
            "asm",
            "auto",
            "bitand",
            "bitor",
            "bool",
            "break",
            "case",
            "catch",
            "char",
            "char8_t",
            "char16_t",
            "char32_t",
            "class",
            "compl",
            "concept",
            "const",
            "consteval",
            "constexpr",
            "constinit",
            "const_cast",
            "continue",
            "co_await",
            "co_return",
            "co_yield",
            "decltype",
            "default",
            "delete",
            "do",
            "double",
            "dynamic_cast",
            "else",
            "enum",
            "explicit",
            "export",
            "extern",
            "false",
            "float",
            "for",
            "friend",
            "goto",
            "if",
            "inline",
            "int",
            "long",
            "mutable",
            "namespace",
            "new",
            "noexcept",
            "not",
            "not_eq",
            "nullptr",
            "operator",
            "or",
            "or_eq",
            "private",
            "protected",
            "public",
            "register",
            "reinterpret_cast",
            "requires",
            "return",
            "short",
            "signed",
            "sizeof",
            "static",
            "static_assert",
            "static_cast",
            "struct",
            "switch",
            "template",
            "this",
            "thread_local",
            "throw",
            "true",
            "try",
            "typedef",
            "typeid",
            "typename",
            "union",
            "unsigned",
            "using",
            "virtual",
            "void",
            "volatile",
            "wchar_t",
            "while",
            "xor",
            "xor_eq"
        };
        private readonly HashSet<string> controlKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "if",
            "else",
            "for",
            "while",
            "do",
            "switch",
            "catch"
        };

        // =========================================================
        // Known C++ names
        // =========================================================

        private readonly HashSet<string> knownTypes =
            new HashSet<string>(StringComparer.Ordinal)
        {
            "bool",
            "char",
            "char8_t",
            "char16_t",
            "char32_t",
            "double",
            "float",
            "int",
            "long",
            "short",
            "signed",
            "unsigned",
            "void",
            "wchar_t",
            "size_t",
            "string",
            "std",
            "auto"
        };

        private readonly HashSet<string> knownFunctions =
            new HashSet<string>(StringComparer.Ordinal)
        {
            "cin",
            "cout",
            "cerr",
            "clog",
            "endl",
            "printf",
            "scanf",
            "strlen",
            "strcpy",
            "strcmp",
            "sqrt",
            "pow",
            "abs",
            "sin",
            "cos",
            "tan",
            "log",
            "exp",
            "getline"
        };

        // =========================================================
        // Constructor
        // =========================================================

        public CodeDebugger(string code)
        {
            this.code = code ?? string.Empty;
        }

        // =========================================================
        // Main Analyze
        // =========================================================

        public List<CodeError> Analyze()
        {
            errors.Clear();

            List<Token> tokens = Tokenize();

            CheckBrackets(tokens);
            CheckKeywordSpelling(tokens);
            CheckVariableDeclarations(tokens);
            CheckUndefinedVariables(tokens);
            CheckFunctionParameters(tokens);
            CheckSemicolons(tokens);
            CheckOperators(tokens);

            return SortErrors();
        }

        // =========================================================
        // Add Error
        // =========================================================

        private void AddError(int line, string message)
        {
            if (line < 1)
                line = 1;

            // جلوگیری از خطاهای کاملاً تکراری
            foreach (CodeError error in errors)
            {
                // چون ساختار CodeError را نمی‌دانیم، اینجا
                // عمداً duplicate detection را بر اساس message
                // انجام نمی‌دهیم.
            }

            errors.Add(new CodeError(line, message));
        }

        // =========================================================
        // Token Types
        // =========================================================

        private enum TokenType
        {
            Identifier,
            Keyword,
            Number,
            String,
            Character,

            Operator,
            OpenBracket,
            CloseBracket,

            Semicolon,
            Comma,
            Colon,
            Dot,

            Preprocessor,

            Unknown
        }

        // =========================================================
        // Token
        // =========================================================

        private class Token
        {
            public string Text { get; }
            public TokenType Type { get; }
            public int Line { get; }
            public int Column { get; }
            public int Position { get; }

            public Token(
                string text,
                TokenType type,
                int line,
                int column,
                int position)
            {
                Text = text;
                Type = type;
                Line = line;
                Column = column;
                Position = position;
            }
        }

        // =========================================================
        // 1. Lexer / Tokenizer
        // =========================================================

        private List<Token> Tokenize()
        {
            List<Token> tokens =
                new List<Token>();

            int i = 0;
            int line = 1;
            int column = 1;

            while (i < code.Length)
            {
                char c = code[i];

                // -------------------------------------------------
                // New line
                // -------------------------------------------------

                if (c == '\n')
                {
                    i++;
                    line++;
                    column = 1;
                    continue;
                }

                // -------------------------------------------------
                // Whitespace
                // -------------------------------------------------

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Single line comment
                // -------------------------------------------------

                if (c == '/' &&
                    i + 1 < code.Length &&
                    code[i + 1] == '/')
                {
                    i += 2;
                    column += 2;

                    while (i < code.Length &&
                           code[i] != '\n')
                    {
                        i++;
                        column++;
                    }

                    continue;
                }

                // -------------------------------------------------
                // Multiline comment
                // -------------------------------------------------

                if (c == '/' &&
                    i + 1 < code.Length &&
                    code[i + 1] == '*')
                {
                    int startLine = line;

                    i += 2;
                    column += 2;

                    bool closed = false;

                    while (i < code.Length)
                    {
                        if (code[i] == '*' &&
                            i + 1 < code.Length &&
                            code[i + 1] == '/')
                        {
                            i += 2;
                            column += 2;
                            closed = true;
                            break;
                        }

                        if (code[i] == '\n')
                        {
                            i++;
                            line++;
                            column = 1;
                        }
                        else
                        {
                            i++;
                            column++;
                        }
                    }

                    if (!closed)
                    {
                        AddError(
                            startLine,
                            "The multiline comment was not closed.");
                    }

                    continue;
                }

                // -------------------------------------------------
                // Preprocessor
                // -------------------------------------------------

                if (c == '#' && column == 1)
                {
                    int start = i;
                    int startColumn = column;

                    while (i < code.Length &&
                           code[i] != '\n')
                    {
                        i++;
                        column++;
                    }

                    tokens.Add(
                        new Token(
                            code.Substring(
                                start,
                                i - start),
                            TokenType.Preprocessor,
                            line,
                            startColumn,
                            start));

                    continue;
                }

                // -------------------------------------------------
                // String
                // -------------------------------------------------

                if (c == '"')
                {
                    int start = i;
                    int startLine = line;
                    int startColumn = column;

                    i++;
                    column++;

                    bool closed = false;

                    while (i < code.Length)
                    {
                        char current = code[i];

                        if (current == '\\')
                        {
                            if (i + 1 < code.Length)
                            {
                                i += 2;
                                column += 2;
                                continue;
                            }

                            i++;
                            column++;
                            continue;
                        }

                        if (current == '"')
                        {
                            i++;
                            column++;
                            closed = true;
                            break;
                        }

                        // Normal C++ string cannot cross lines
                        if (current == '\n')
                        {
                            break;
                        }

                        i++;
                        column++;
                    }

                    if (!closed)
                    {
                        AddError(
                            startLine,
                            "The '\"' was not closed.");

                        // Do not continue interpreting the rest
                        // of this line as normal tokens.
                        while (i < code.Length &&
                               code[i] != '\n')
                        {
                            i++;
                            column++;
                        }
                    }
                    else
                    {
                        tokens.Add(
                            new Token(
                                code.Substring(
                                    start,
                                    i - start),
                                TokenType.String,
                                startLine,
                                startColumn,
                                start));
                    }

                    continue;
                }

                // -------------------------------------------------
                // Character
                // -------------------------------------------------

                if (c == '\'')
                {
                    int start = i;
                    int startLine = line;
                    int startColumn = column;

                    i++;
                    column++;

                    bool closed = false;

                    while (i < code.Length)
                    {
                        char current = code[i];

                        if (current == '\\')
                        {
                            if (i + 1 < code.Length)
                            {
                                i += 2;
                                column += 2;
                                continue;
                            }

                            i++;
                            column++;
                            continue;
                        }

                        if (current == '\'')
                        {
                            i++;
                            column++;
                            closed = true;
                            break;
                        }

                        if (current == '\n')
                        {
                            break;
                        }

                        i++;
                        column++;
                    }

                    if (!closed)
                    {
                        AddError(
                            startLine,
                            "The character literal was not closed.");

                        while (i < code.Length &&
                               code[i] != '\n')
                        {
                            i++;
                            column++;
                        }
                    }
                    else
                    {
                        tokens.Add(
                            new Token(
                                code.Substring(
                                    start,
                                    i - start),
                                TokenType.Character,
                                startLine,
                                startColumn,
                                start));
                    }

                    continue;
                }

                // -------------------------------------------------
                // Identifier / Keyword
                // -------------------------------------------------

                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    int startColumn = column;

                    i++;
                    column++;

                    while (i < code.Length &&
                           (char.IsLetterOrDigit(code[i]) ||
                            code[i] == '_'))
                    {
                        i++;
                        column++;
                    }

                    string word =
                        code.Substring(
                            start,
                            i - start);

                    TokenType type =
                        cppKeywords.Contains(word)
                            ? TokenType.Keyword
                            : TokenType.Identifier;

                    tokens.Add(
                        new Token(
                            word,
                            type,
                            line,
                            startColumn,
                            start));

                    continue;
                }

                // -------------------------------------------------
                // Number
                // -------------------------------------------------

                if (char.IsDigit(c))
                {
                    int start = i;
                    int startColumn = column;

                    i++;
                    column++;

                    while (i < code.Length &&
                           (char.IsLetterOrDigit(code[i]) ||
                            code[i] == '.' ||
                            code[i] == '_'))
                    {
                        i++;
                        column++;
                    }

                    tokens.Add(
                        new Token(
                            code.Substring(
                                start,
                                i - start),
                            TokenType.Number,
                            line,
                            startColumn,
                            start));

                    continue;
                }

                // -------------------------------------------------
                // Brackets
                // -------------------------------------------------

                if (c == '(' ||
                    c == '{' ||
                    c == '[')
                {
                    tokens.Add(
                        new Token(
                            c.ToString(),
                            TokenType.OpenBracket,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                if (c == ')' ||
                    c == '}' ||
                    c == ']')
                {
                    tokens.Add(
                        new Token(
                            c.ToString(),
                            TokenType.CloseBracket,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Semicolon
                // -------------------------------------------------

                if (c == ';')
                {
                    tokens.Add(
                        new Token(
                            ";",
                            TokenType.Semicolon,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Comma
                // -------------------------------------------------

                if (c == ',')
                {
                    tokens.Add(
                        new Token(
                            ",",
                            TokenType.Comma,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Colon
                // -------------------------------------------------

                if (c == ':')
                {
                    tokens.Add(
                        new Token(
                            ":",
                            TokenType.Colon,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Dot
                // -------------------------------------------------

                if (c == '.')
                {
                    tokens.Add(
                        new Token(
                            ".",
                            TokenType.Dot,
                            line,
                            column,
                            i));

                    i++;
                    column++;
                    continue;
                }

                // -------------------------------------------------
                // Operators
                // -------------------------------------------------

                string op =
                    GetOperatorAt(i);

                if (op != null)
                {
                    tokens.Add(
                        new Token(
                            op,
                            TokenType.Operator,
                            line,
                            column,
                            i));

                    i += op.Length;
                    column += op.Length;
                    continue;
                }

                // -------------------------------------------------
                // Unknown character
                // -------------------------------------------------

                tokens.Add(
                    new Token(
                        c.ToString(),
                        TokenType.Unknown,
                        line,
                        column,
                        i));

                i++;
                column++;
            }

            return tokens;
        }

        // =========================================================
        // Operators
        // =========================================================

        private string GetOperatorAt(int position)
        {
            string[] operators =
            {
                ">>=",
                "<<=",
                "->*",
                "...",

                "++",
                "--",
                "==",
                "!=",
                "<=",
                ">=",
                "&&",
                "||",
                "+=",
                "-=",
                "*=",
                "/=",
                "%=",
                "&=",
                "|=",
                "^=",
                "<<",
                ">>",
                "->",
                "::",
                ".*",

                "+",
                "-",
                "*",
                "/",
                "%",
                "=",
                "<",
                ">",
                "!",
                "&",
                "|",
                "^",
                "~",
                "?",
            };

            foreach (string op in operators)
            {
                if (position + op.Length <= code.Length &&
                    code.Substring(
                        position,
                        op.Length) == op)
                {
                    return op;
                }
            }

            return null;
        }

        // =========================================================
        // 2. Bracket Check
        // =========================================================

        private void CheckBrackets(List<Token> tokens)
        {
            Stack<Token> stack =
                new Stack<Token>();

            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.OpenBracket)
                {
                    stack.Push(token);
                    continue;
                }

                if (token.Type != TokenType.CloseBracket)
                    continue;

                if (stack.Count == 0)
                {
                    AddError(
                        token.Line,
                        $"The '{token.Text}' has no matching opening bracket.");

                    continue;
                }

                Token opening =
                    stack.Peek();

                if (IsMatchingBracket(
                    opening.Text[0],
                    token.Text[0]))
                {
                    stack.Pop();
                }
                else
                {
                    AddError(
                        token.Line,
                        $"The '{token.Text}' does not match the opening '{opening.Text}'.");

                    // مهم:
                    // bracket باز را اینجا Pop نمی‌کنیم.
                    // چون ممکن است bracket بعدی با آن match شود.
                }
            }

            while (stack.Count > 0)
            {
                Token opening =
                    stack.Pop();

                char expected =
                    GetClosingBracket(
                        opening.Text[0]);

                AddError(
                    opening.Line,
                    $"The '{opening.Text}' was not closed. Expected '{expected}'.");
            }
        }

        private bool IsMatchingBracket(
            char opening,
            char closing)
        {
            return
                (opening == '(' && closing == ')') ||
                (opening == '{' && closing == '}') ||
                (opening == '[' && closing == ']');
        }

        private char GetClosingBracket(char opening)
        {
            switch (opening)
            {
                case '(':
                    return ')';

                case '{':
                    return '}';

                case '[':
                    return ']';

                default:
                    return '?';
            }
        }

        // =========================================================
        // 3. Keyword Spelling
        // =========================================================

        private void CheckKeywordSpelling(List<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"TEXT={token.Text} | TYPE={token.Type} | LINE={token.Line}");
            }
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                // فقط Identifierها را بررسی می‌کنیم.
                // Keywordهای صحیح از قبل Keyword هستند.
                if (token.Type != TokenType.Identifier)
                    continue;

                string word = token.Text;

                // ---------------------------------------------------------
                // اگر نام عضو باشد:
                //
                // object.foo
                // object->foo
                // std::foo
                //
                // نباید typo keyword محسوب شود.
                // ---------------------------------------------------------

                int previousIndex =
                    GetPreviousSignificantTokenIndex(tokens, i);

                if (previousIndex >= 0)
                {
                    string previous =
                        tokens[previousIndex].Text;

                    if (previous == "." ||
                        previous == "->" ||
                        previous == "::")
                    {
                        continue;
                    }
                }

                // ---------------------------------------------------------
                // اگر نام شناخته‌شده‌ای مثل cout یا cin باشد،
                // typo keyword نیست.
                // ---------------------------------------------------------

                if (knownFunctions.Contains(word))
                    continue;

                // ---------------------------------------------------------
                // آیا بعد از identifier پرانتز آمده؟
                //
                // مثال:
                //
                // sum(...)
                // foo(...)
                // whlie(...)
                // ---------------------------------------------------------

                int nextIndex =
                    GetNextSignificantTokenIndex(tokens, i);

                bool followedByParenthesis =
                    nextIndex >= 0 &&
                    tokens[nextIndex].Text == "(";

                // ---------------------------------------------------------
                // پیدا کردن نزدیک‌ترین keyword
                // ---------------------------------------------------------

                string closestKeyword = null;
                int bestDistance = int.MaxValue;

                foreach (string keyword in cppKeywords)
                {
                    int distance =
                        DamerauLevenshteinDistance(word, keyword);

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        closestKeyword = keyword;
                    }
                }

                if (closestKeyword == null)
                    continue;

                // ---------------------------------------------------------
                // فقط typoهای خیلی نزدیک را بررسی می‌کنیم.
                // ---------------------------------------------------------

                if (bestDistance != 1)
                    continue;

                // ---------------------------------------------------------
                // اگر identifier به شکل function call آمده،
                // فقط در صورتی typo محسوبش کن که به یک control keyword
                // نزدیک باشد.
                //
                // مثال:
                //
                // whlie(...) → while
                // whlie → while فاصله 1
                //
                // ولی:
                //
                // foo(...) → for
                //
                // نباید خطا بدهد.
                // ---------------------------------------------------------

                if (followedByParenthesis)
                {
                    if (!controlKeywords.Contains(closestKeyword))
                        continue;
                }

                // ---------------------------------------------------------
                // گزارش خطا
                // ---------------------------------------------------------

                AddError(
                    token.Line,
                    $"The keyword '{word}' is misspelled. Did you mean '{closestKeyword}'?");
            }
        }

        private bool IsFunctionLikeIdentifier(List<Token> tokens, Token token)
        {
            int index =
                tokens.IndexOf(token);

            if (index < 0)
                return false;

            int next =
                GetNextSignificantTokenIndex(
                    tokens,
                    index);

            if (next < 0 ||
                tokens[next].Text != "(")
            {
                return false;
            }

            // ---------------------------------------------------------
            // If it is very close to a keyword,
            // do NOT assume it is a function.
            //
            // Example:
            //
            // whlie(...)
            //
            // should be treated as a misspelled "while".
            // ---------------------------------------------------------

            string closestKeyword = null;
            int bestDistance = int.MaxValue;

            foreach (string keyword in cppKeywords)
            {
                int distance =
                    DamerauLevenshteinDistance(token.Text, keyword);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closestKeyword = keyword;
                }
            }

            if (closestKeyword != null &&
                bestDistance <= 1)
            {
                return false;
            }

            return true;
        }

        // =========================================================
        // Levenshtein
        // =========================================================

        private int DamerauLevenshteinDistance(string a, string b)
        {
            int[,] matrix =
                new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= b.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost =
                        a[i - 1] == b[j - 1]
                            ? 0
                            : 1;

                    matrix[i, j] =
                        Math.Min(
                            Math.Min(
                                matrix[i - 1, j] + 1,
                                matrix[i, j - 1] + 1),
                            matrix[i - 1, j - 1] + cost);

                    // -------------------------------------------------
                    // جابجایی دو کاراکتر مجاور
                    //
                    // مثال:
                    // whlie
                    // while
                    //
                    // i و l جابجا شده‌اند.
                    // -------------------------------------------------

                    if (i > 1 &&
                        j > 1 &&
                        a[i - 1] == b[j - 2] &&
                        a[i - 2] == b[j - 1])
                    {
                        matrix[i, j] =
                            Math.Min(
                                matrix[i, j],
                                matrix[i - 2, j - 2] + 1);
                    }
                }
            }

            return matrix[a.Length, b.Length];
        }

        // =========================================================
        // 4. Variable Declaration
        // =========================================================

        private class VariableInfo
        {
            public string Name { get; }
            public int Line { get; }

            public VariableInfo(
                string name,
                int line)
            {
                Name = name;
                Line = line;
            }
        }

        private void CheckVariableDeclarations(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (!IsTypeToken(token))
                    continue;

                int nameIndex =
                    FindDeclarationName(
                        tokens,
                        i);

                if (nameIndex < 0)
                    continue;

                Token nameToken =
                    tokens[nameIndex];

                // ---------------------------------------------------------
                // Keyword used as variable name
                // ---------------------------------------------------------

                if (nameToken.Type == TokenType.Keyword &&
                    cppKeywords.Contains(nameToken.Text))
                {
                    AddError(
                        nameToken.Line,
                        $"Variable name '{nameToken.Text}' is a reserved keyword.");

                    continue;
                }

                // ---------------------------------------------------------
                // فقط identifier می‌تواند نام متغیر معمولی باشد.
                // ---------------------------------------------------------

                if (nameToken.Type != TokenType.Identifier)
                    continue;

                // ---------------------------------------------------------
                // اگر بعد از نام '(' باشد،
                // احتمالاً function declaration است.
                // ---------------------------------------------------------

                int afterName =
                    GetNextSignificantTokenIndex(
                        tokens,
                        nameIndex);

                if (afterName >= 0 &&
                    tokens[afterName].Text == "(")
                {
                    continue;
                }
            }
        }

        private bool IsTypeToken(Token token)
        {
            if (token.Type != TokenType.Keyword &&
                token.Type != TokenType.Identifier)
            {
                return false;
            }

            return
                token.Text == "int" ||
                token.Text == "float" ||
                token.Text == "double" ||
                token.Text == "char" ||
                token.Text == "bool" ||
                token.Text == "void" ||
                token.Text == "long" ||
                token.Text == "short" ||
                token.Text == "signed" ||
                token.Text == "unsigned" ||
                token.Text == "auto" ||
                token.Text == "string";
        }

        private int FindDeclarationName(List<Token> tokens, int typeIndex)
        {
            int i = typeIndex + 1;

            // ---------------------------------------------------------
            // Qualifiers / type modifiers
            // ---------------------------------------------------------

            while (i < tokens.Count)
            {
                string text = tokens[i].Text;

                if (text == "const" ||
                    text == "unsigned" ||
                    text == "signed" ||
                    text == "long" ||
                    text == "short")
                {
                    i++;
                    continue;
                }

                break;
            }

            if (i >= tokens.Count)
                return -1;

            // ---------------------------------------------------------
            // Pointer
            // ---------------------------------------------------------

            while (i < tokens.Count &&
                   (tokens[i].Text == "*" ||
                    tokens[i].Text == "&"))
            {
                i++;
            }

            if (i >= tokens.Count)
                return -1;

            // ---------------------------------------------------------
            // Identifier OR Keyword
            //
            // Keyword is intentionally accepted here so that:
            //
            // int if = 10;
            //
            // can be detected.
            // ---------------------------------------------------------

            if (tokens[i].Type == TokenType.Identifier ||
                tokens[i].Type == TokenType.Keyword)
            {
                return i;
            }

            return -1;
        }

        // =========================================================
        // 5. Undefined Variables
        // =========================================================

        private class Scope
        {
            public Scope Parent { get; }

            public HashSet<string> Variables { get; } =
                new HashSet<string>(StringComparer.Ordinal);

            public HashSet<string> Functions { get; } =
                new HashSet<string>(StringComparer.Ordinal);

            public Scope(Scope parent)
            {
                Parent = parent;
            }

            public bool ContainsVariable(string name)
            {
                if (Variables.Contains(name))
                    return true;

                return Parent != null &&
                       Parent.ContainsVariable(name);
            }

            public bool ContainsFunction(string name)
            {
                if (Functions.Contains(name))
                    return true;

                return Parent != null &&
                       Parent.ContainsFunction(name);
            }
        }

        private void CheckUndefinedVariables(
            List<Token> tokens)
        {
            Scope globalScope =
                new Scope(null);

            // -----------------------------------------------------
            // First collect function names
            // -----------------------------------------------------

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type != TokenType.Identifier)
                    continue;

                int next =
                    GetNextSignificantTokenIndex(
                        tokens,
                        i);

                if (next >= 0 &&
                    tokens[next].Text == "(")
                {
                    // اگر شبیه function باشد،
                    // به عنوان function شناخته می‌شود.
                    globalScope.Functions.Add(
                        tokens[i].Text);
                }
            }

            Scope currentScope =
                globalScope;

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                // -------------------------------------------------
                // Open scope
                // -------------------------------------------------

                if (token.Text == "{")
                {
                    currentScope =
                        new Scope(currentScope);

                    continue;
                }

                // -------------------------------------------------
                // Close scope
                // -------------------------------------------------

                if (token.Text == "}")
                {
                    if (currentScope.Parent != null)
                        currentScope =
                            currentScope.Parent;

                    continue;
                }

                // -------------------------------------------------
                // Ignore non-identifiers
                // -------------------------------------------------

                if (token.Type != TokenType.Identifier)
                    continue;

                // -------------------------------------------------
                // Known C++ functions / objects
                // -------------------------------------------------

                if (knownFunctions.Contains(token.Text))
                    continue;

                // -------------------------------------------------
                // Known types / std
                // -------------------------------------------------

                if (knownTypes.Contains(token.Text))
                    continue;

                // -------------------------------------------------
                // Function call / declaration
                // -------------------------------------------------

                int nextIndex =
                    GetNextSignificantTokenIndex(
                        tokens,
                        i);

                if (nextIndex >= 0 &&
                    tokens[nextIndex].Text == "(")
                {
                    continue;
                }

                // -------------------------------------------------
                // Namespace/member access
                // -------------------------------------------------

                int previousIndex =
                    GetPreviousSignificantTokenIndex(
                        tokens,
                        i);

                if (previousIndex >= 0)
                {
                    string previous =
                        tokens[previousIndex].Text;

                    if (previous == "." ||
                        previous == "->" ||
                        previous == "::")
                    {
                        continue;
                    }
                }

                // -------------------------------------------------
                // Variable declaration
                // -------------------------------------------------

                if (IsDeclarationName(
                    tokens,
                    i))
                {
                    currentScope.Variables.Add(
                        token.Text);

                    continue;
                }

                // -------------------------------------------------
                // Already defined
                // -------------------------------------------------

                if (currentScope.ContainsVariable(
                    token.Text))
                {
                    continue;
                }

                // -------------------------------------------------
                // Language identifiers
                // -------------------------------------------------

                if (token.Text == "main")
                    continue;

                // -------------------------------------------------
                // Avoid member / labels
                // -------------------------------------------------

                if (nextIndex >= 0 &&
                    tokens[nextIndex].Text == ":")
                {
                    continue;
                }

                // -------------------------------------------------
                // Potential undefined variable
                // -------------------------------------------------

                AddError(
                    token.Line,
                    $"Variable '{token.Text}' is not defined.");
            }
        }

        private bool IsDeclarationName(
            List<Token> tokens,
            int index)
        {
            if (index <= 0)
                return false;

            int previous =
                GetPreviousSignificantTokenIndex(
                    tokens,
                    index);

            if (previous < 0)
                return false;

            return IsTypeToken(
                tokens[previous]);
        }

        // =========================================================
        // 6. Function Parameters
        // =========================================================

        private class FunctionInfo
        {
            public string Name { get; }
            public int ParameterCount { get; }

            public FunctionInfo(
                string name,
                int parameterCount)
            {
                Name = name;
                ParameterCount = parameterCount;
            }
        }

        private void CheckFunctionParameters(
            List<Token> tokens)
        {
            Dictionary<string, FunctionInfo>
                functions =
                    new Dictionary<string, FunctionInfo>(
                        StringComparer.Ordinal);

            // -----------------------------------------------------
            // Find function definitions
            // -----------------------------------------------------

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type != TokenType.Identifier)
                    continue;

                int next =
                    GetNextSignificantTokenIndex(
                        tokens,
                        i);

                if (next < 0 ||
                    tokens[next].Text != "(")
                {
                    continue;
                }

                int close =
                    FindMatchingBracket(
                        tokens,
                        next);

                if (close < 0)
                    continue;

                // If followed by ; -> declaration/prototype
                // If followed by { -> definition
                int afterClose =
                    GetNextSignificantTokenIndex(
                        tokens,
                        close);

                if (afterClose < 0)
                    continue;

                if (tokens[afterClose].Text != "{")
                    continue;

                string functionName =
                    tokens[i].Text;

                int parameterCount =
                    CountTopLevelParameters(
                        tokens,
                        next,
                        close);

                if (!functions.ContainsKey(
                    functionName))
                {
                    functions.Add(
                        functionName,
                        new FunctionInfo(
                            functionName,
                            parameterCount));
                }
            }

            // -----------------------------------------------------
            // Check function calls
            // -----------------------------------------------------

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (token.Type != TokenType.Identifier)
                    continue;

                int open =
                    GetNextSignificantTokenIndex(
                        tokens,
                        i);

                if (open < 0 ||
                    tokens[open].Text != "(")
                {
                    continue;
                }

                string functionName =
                    token.Text;

                if (!functions.ContainsKey(
                    functionName))
                {
                    continue;
                }

                int close =
                    FindMatchingBracket(
                        tokens,
                        open);

                if (close < 0)
                    continue;

                int actualCount =
                    CountTopLevelParameters(
                        tokens,
                        open,
                        close);

                int expectedCount =
                    functions[
                        functionName]
                    .ParameterCount;

                // Function definition itself
                int afterClose =
                    GetNextSignificantTokenIndex(
                        tokens,
                        close);

                if (afterClose >= 0 &&
                    tokens[afterClose].Text == "{")
                {
                    continue;
                }

                if (actualCount != expectedCount)
                {
                    AddError(
                        token.Line,
                        $"Function '{functionName}' expects {expectedCount} parameter(s), but got {actualCount}.");
                }
            }
        }

        // =========================================================
        // Find matching bracket
        // =========================================================

        private int FindMatchingBracket(
            List<Token> tokens,
            int openIndex)
        {
            if (openIndex < 0 ||
                openIndex >= tokens.Count)
            {
                return -1;
            }

            string opening =
                tokens[openIndex].Text;

            string closing;

            if (opening == "(")
                closing = ")";
            else if (opening == "[")
                closing = "]";
            else if (opening == "{")
                closing = "}";
            else
                return -1;

            int depth = 0;

            for (int i = openIndex;
                 i < tokens.Count;
                 i++)
            {
                if (tokens[i].Text == opening)
                {
                    depth++;
                }
                else if (tokens[i].Text == closing)
                {
                    depth--;

                    if (depth == 0)
                        return i;
                }
            }

            return -1;
        }

        // =========================================================
        // Count function parameters
        // =========================================================

        private int CountTopLevelParameters(
            List<Token> tokens,
            int openIndex,
            int closeIndex)
        {
            if (openIndex + 1 >= closeIndex)
                return 0;

            int depthParentheses = 0;
            int depthBrackets = 0;
            int depthBraces = 0;

            int count = 1;

            for (int i = openIndex + 1;
                 i < closeIndex;
                 i++)
            {
                string text =
                    tokens[i].Text;

                if (text == "(")
                    depthParentheses++;

                else if (text == ")")
                    depthParentheses--;

                else if (text == "[")
                    depthBrackets++;

                else if (text == "]")
                    depthBrackets--;

                else if (text == "{")
                    depthBraces++;

                else if (text == "}")
                    depthBraces--;

                else if (
                    text == "," &&
                    depthParentheses == 0 &&
                    depthBrackets == 0 &&
                    depthBraces == 0)
                {
                    count++;
                }
            }

            return count;
        }

        // =========================================================
        // 7. Semicolon Check
        // =========================================================

        private void CheckSemicolons(List<Token> tokens)
        {
            int i = 0;

            while (i < tokens.Count)
            {
                Token current = tokens[i];

                // ---------------------------------------------------------
                // Preprocessor
                // ---------------------------------------------------------

                if (current.Type == TokenType.Preprocessor)
                {
                    i++;
                    continue;
                }

                // ---------------------------------------------------------
                // Closing block
                // ---------------------------------------------------------

                if (current.Text == "}")
                {
                    i++;
                    continue;
                }

                // ---------------------------------------------------------
                // Opening block
                // ---------------------------------------------------------

                if (current.Text == "{")
                {
                    i++;
                    continue;
                }

                // ---------------------------------------------------------
                // Control structures
                // ---------------------------------------------------------

                if (current.Text == "if" ||
                    current.Text == "else" ||
                    current.Text == "for" ||
                    current.Text == "while" ||
                    current.Text == "switch" ||
                    current.Text == "do" ||
                    current.Text == "try" ||
                    current.Text == "catch")
                {
                    int controlEnd =
                        FindControlStructureEnd(
                            tokens,
                            i);

                    if (controlEnd > i)
                    {
                        i = controlEnd + 1;
                    }
                    else
                    {
                        i++;
                    }

                    continue;
                }

                // ---------------------------------------------------------
                // Find end of current statement
                // ---------------------------------------------------------

                int statementEnd =
                    FindNextStatementBoundary(
                        tokens,
                        i);

                if (statementEnd < 0)
                    break;

                Token last =
                    tokens[statementEnd];

                // ---------------------------------------------------------
                // Correct semicolon
                // ---------------------------------------------------------

                if (last.Type == TokenType.Semicolon)
                {
                    i = statementEnd + 1;
                    continue;
                }

                // ---------------------------------------------------------
                // Block ending with }
                // ---------------------------------------------------------

                if (last.Text == "}")
                {
                    i = statementEnd + 1;
                    continue;
                }

                // ---------------------------------------------------------
                // Function / class / namespace definition
                // ---------------------------------------------------------

                if (IsFunctionDefinition(
                        tokens,
                        i,
                        statementEnd))
                {
                    i = statementEnd + 1;
                    continue;
                }

                if (IsTypeOrNamespaceBlock(
                        tokens,
                        i,
                        statementEnd))
                {
                    i = statementEnd + 1;
                    continue;
                }

                // ---------------------------------------------------------
                // case / default
                // ---------------------------------------------------------

                if (current.Text == "case" ||
                    current.Text == "default")
                {
                    i = statementEnd + 1;
                    continue;
                }

                // ---------------------------------------------------------
                // Determine whether this looks like a statement
                // ---------------------------------------------------------

                if (LooksLikeStatement(
                        tokens,
                        i,
                        statementEnd))
                {
                    AddError(
                        last.Line,
                        "A semicolon ';' is missing at the end of this statement.");
                }

                // ---------------------------------------------------------
                // VERY IMPORTANT:
                // move to the end of the whole statement.
                // This prevents duplicate errors.
                // ---------------------------------------------------------

                i = statementEnd + 1;
            }
        }

        private int FindNextStatementBoundary(
    List<Token> tokens,
    int start)
        {
            int parentheses = 0;
            int brackets = 0;
            int braces = 0;

            for (int i = start; i < tokens.Count; i++)
            {
                string text = tokens[i].Text;

                if (text == "(")
                {
                    parentheses++;
                    continue;
                }

                if (text == ")")
                {
                    parentheses--;
                    continue;
                }

                if (text == "[")
                {
                    brackets++;
                    continue;
                }

                if (text == "]")
                {
                    brackets--;
                    continue;
                }

                if (text == "{")
                {
                    braces++;
                    continue;
                }

                if (text == "}")
                {
                    if (parentheses == 0 &&
                        brackets == 0 &&
                        braces == 0)
                    {
                        return i;
                    }

                    braces--;
                    continue;
                }

                // ---------------------------------------------------------
                // Semicolon at top level
                // ---------------------------------------------------------

                if (text == ";" &&
                    parentheses == 0 &&
                    brackets == 0 &&
                    braces == 0)
                {
                    return i;
                }

                // ---------------------------------------------------------
                // A new statement on a later line
                //
                // We only use this as a safety boundary.
                // ---------------------------------------------------------

                if (i > start &&
                    tokens[i].Line > tokens[i - 1].Line &&
                    parentheses == 0 &&
                    brackets == 0 &&
                    braces == 0)
                {
                    if (IsLikelyNewStatement(
                            tokens,
                            i))
                    {
                        return i - 1;
                    }
                }
            }

            return tokens.Count - 1;
        }

        private bool IsLikelyNewStatement(
    List<Token> tokens,
    int index)
        {
            if (index < 0 ||
                index >= tokens.Count)
            {
                return false;
            }

            string text =
                tokens[index].Text;

            return
                text == "int" ||
                text == "float" ||
                text == "double" ||
                text == "char" ||
                text == "bool" ||
                text == "long" ||
                text == "short" ||
                text == "unsigned" ||
                text == "signed" ||
                text == "return" ||
                text == "if" ||
                text == "for" ||
                text == "while" ||
                text == "switch" ||
                text == "cout" ||
                text == "cin" ||
                text == "break" ||
                text == "continue";
        }

        private int FindControlStructureEnd(
    List<Token> tokens,
    int start)
        {
            int i = start;

            // ---------------------------------------------------------
            // Find condition parentheses
            // ---------------------------------------------------------

            while (i < tokens.Count &&
                   tokens[i].Text != "(")
            {
                i++;
            }

            if (i >= tokens.Count)
                return start;

            int close =
                FindMatchingBracket(
                    tokens,
                    i);

            if (close < 0)
                return start;

            int next =
                GetNextSignificantTokenIndex(
                    tokens,
                    close);

            if (next < 0)
                return close;

            // ---------------------------------------------------------
            // if (...) { ... }
            // for (...) { ... }
            // while (...) { ... }
            // ---------------------------------------------------------

            if (tokens[next].Text == "{")
            {
                int blockEnd =
                    FindMatchingBracket(
                        tokens,
                        next);

                if (blockEnd >= 0)
                    return blockEnd;

                return next;
            }

            // ---------------------------------------------------------
            // Control structure without braces:
            //
            // if (x)
            //     x++;
            // ---------------------------------------------------------

            return close;
        }

        private bool IsPotentialStatementStart(
            List<Token> tokens,
            int index)
        {
            Token token = tokens[index];

            if (token.Type == TokenType.Preprocessor)
                return false;

            if (token.Text == "{" ||
                token.Text == "}")
                return false;

            if (token.Type == TokenType.Semicolon)
                return false;

            return true;
        }

        private int FindStatementEnd(
            List<Token> tokens,
            int start)
        {
            int parenthesisDepth = 0;
            int bracketDepth = 0;

            for (int i = start;
                 i < tokens.Count;
                 i++)
            {
                string text =
                    tokens[i].Text;

                if (text == "(")
                    parenthesisDepth++;

                else if (text == ")")
                    parenthesisDepth--;

                else if (text == "[")
                    bracketDepth++;

                else if (text == "]")
                    bracketDepth--;

                if (parenthesisDepth == 0 &&
                    bracketDepth == 0)
                {
                    if (tokens[i].Type ==
                        TokenType.Semicolon)
                    {
                        return i;
                    }

                    if (text == "{")
                    {
                        int close =
                            FindMatchingBracket(
                                tokens,
                                i);

                        if (close >= 0)
                            return close;
                    }

                    if (text == "}")
                        return i;
                }

                // Don't let one line consume the whole file
                // if another statement begins on a later line.
                if (i > start &&
                    tokens[i].Line >
                    tokens[i - 1].Line)
                {
                    if (parenthesisDepth == 0 &&
                        bracketDepth == 0 &&
                        LooksLikeStatementBoundary(
                            tokens,
                            i))
                    {
                        return i - 1;
                    }
                }
            }

            return tokens.Count - 1;
        }

        private bool LooksLikeStatementBoundary(
            List<Token> tokens,
            int index)
        {
            if (index >= tokens.Count)
                return true;

            string text =
                tokens[index].Text;

            return
                text == "if" ||
                text == "for" ||
                text == "while" ||
                text == "return" ||
                text == "int" ||
                text == "float" ||
                text == "double" ||
                text == "char" ||
                text == "bool" ||
                text == "cout" ||
                text == "cin";
        }

        private bool LooksLikeStatement(
            List<Token> tokens,
            int start,
            int end)
        {
            if (end < start)
                return false;

            Token first =
                tokens[start];

            Token last =
                tokens[end];

            // return
            if (first.Text == "return")
                return true;

            // break / continue
            if (first.Text == "break" ||
                first.Text == "continue")
                return true;

            // Variable declaration
            if (IsTypeToken(first))
                return true;

            // Assignment / expression
            for (int i = start;
                 i <= end;
                 i++)
            {
                if (tokens[i].Type ==
                    TokenType.Operator)
                {
                    return true;
                }

                if (tokens[i].Type ==
                    TokenType.Identifier)
                {
                    int next =
                        GetNextSignificantTokenIndex(
                            tokens,
                            i);

                    if (next >= 0 &&
                        tokens[next].Text == "(")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsControlStructure(
            List<Token> tokens,
            int start)
        {
            string text =
                tokens[start].Text;

            return
                text == "if" ||
                text == "else" ||
                text == "for" ||
                text == "while" ||
                text == "switch" ||
                text == "do" ||
                text == "try" ||
                text == "catch";
        }

        private bool IsBlockStatement(
            List<Token> tokens,
            int start,
            int end)
        {
            return
                tokens[start].Text == "{" ||
                tokens[end].Text == "}";
        }

        private bool IsFunctionDefinition(
            List<Token> tokens,
            int start,
            int end)
        {
            for (int i = start;
                 i <= end;
                 i++)
            {
                if (tokens[i].Text != "(")
                    continue;

                int close =
                    FindMatchingBracket(
                        tokens,
                        i);

                if (close < 0)
                    continue;

                int next =
                    GetNextSignificantTokenIndex(
                        tokens,
                        close);

                if (next >= 0 &&
                    tokens[next].Text == "{")
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTypeOrNamespaceBlock(
            List<Token> tokens,
            int start,
            int end)
        {
            string text =
                tokens[start].Text;

            return
                text == "class" ||
                text == "struct" ||
                text == "namespace" ||
                text == "enum";
        }

        private bool IsCaseLabel(
            List<Token> tokens,
            int start)
        {
            return
                tokens[start].Text == "case" ||
                tokens[start].Text == "default";
        }

        // =========================================================
        // 8. Operator Check
        // =========================================================

        private void CheckOperators(
            List<Token> tokens)
        {
            for (int i = 0;
                 i < tokens.Count;
                 i++)
            {
                Token token =
                    tokens[i];

                if (token.Type != TokenType.Operator)
                    continue;

                // ---------------------------------------------
                // Assignment inside if
                // ---------------------------------------------

                if (token.Text == "=" &&
                    IsInsideIfCondition(
                        tokens,
                        i))
                {
                    AddError(
                        token.Line,
                        "Possible operator mistake: using '=' inside a condition. Did you mean '=='?");
                }

                // ---------------------------------------------
                // Two binary operators together
                // ---------------------------------------------

                if (IsBinaryOperator(
                        token.Text))
                {
                    int next =
                        GetNextSignificantTokenIndex(
                            tokens,
                            i);

                    if (next >= 0 &&
                        IsBinaryOperator(
                            tokens[next].Text))
                    {
                        string nextOp =
                            tokens[next].Text;

                        // Some combinations are valid.
                        if (!IsValidOperatorCombination(
                                token.Text,
                                nextOp))
                        {
                            AddError(
                                token.Line,
                                $"Possible invalid operator sequence '{token.Text}{nextOp}'.");
                        }
                    }
                }
            }
        }

        private bool IsInsideIfCondition(
            List<Token> tokens,
            int index)
        {
            int depth = 0;

            for (int i = index - 1;
                 i >= 0;
                 i--)
            {
                if (tokens[i].Text == ")")
                    depth++;

                else if (tokens[i].Text == "(")
                {
                    if (depth > 0)
                    {
                        depth--;
                        continue;
                    }

                    int before =
                        GetPreviousSignificantTokenIndex(
                            tokens,
                            i);

                    return
                        before >= 0 &&
                        tokens[before].Text == "if";
                }

                if (tokens[i].Line <
                    tokens[index].Line - 2)
                {
                    break;
                }
            }

            return false;
        }

        private bool IsBinaryOperator(
            string op)
        {
            return
                op == "+" ||
                op == "-" ||
                op == "*" ||
                op == "/" ||
                op == "%" ||
                op == "=" ||
                op == "==" ||
                op == "!=" ||
                op == "<" ||
                op == ">" ||
                op == "<=" ||
                op == ">=" ||
                op == "&&" ||
                op == "||" ||
                op == "&" ||
                op == "|" ||
                op == "^";
        }

        private bool IsValidOperatorCombination(
            string first,
            string second)
        {
            // ++ and -- are single operators and therefore
            // normally won't arrive here as two operators.

            if (first == "+" &&
                second == "+")
                return true;

            if (first == "-" &&
                second == "-")
                return true;

            if (first == "&" &&
                second == "&")
                return true;

            if (first == "|" &&
                second == "|")
                return true;

            return false;
        }

        // =========================================================
        // Utility: Previous Token
        // =========================================================

        private int GetPreviousSignificantTokenIndex(List<Token> tokens, int index)
        {
            int previous = index - 1;

            if (previous < 0)
                return -1;

            return previous;
        }

        // =========================================================
        // Utility: Next Token
        // =========================================================

        private int GetNextSignificantTokenIndex(List<Token> tokens, int index)
        {
            int next = index + 1;

            if (next >= tokens.Count)
                return -1;

            return next;
        }

        // =========================================================
        // Sort Errors
        // =========================================================

        private List<CodeError> SortErrors()
        {
            // چون CodeError فعلی را نمی‌شناسیم، مرتب‌سازی را
            // به ترتیب اضافه شدن نگه می‌داریم.
            //
            // ترتیب Analyze نیز عملاً ترتیب منطقی بررسی‌هاست.

            return new List<CodeError>(errors);
        }
    }
}