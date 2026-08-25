using System;
using System.Collections.Generic;

namespace KIDE
{
    internal class CodeDebugger
    {
        private readonly string code;

        private readonly List<CodeError> errors =
            new List<CodeError>();

        public CodeDebugger(string code)
        {
            this.code = code ?? string.Empty;
        }

        public List<CodeError> Analyze()
        {
            errors.Clear();

            CheckSemicolons();
            CheckBrackets();
            CheckKeywordSpelling();
            CheckKeywordAsVariable();
            CheckVariableNaming();
            CheckUndefinedVariables();
            CheckStrings();
            CheckOperators();
            CheckFunctionParameters();
            CheckMultilineComments();

            return new List<CodeError>(errors);
        }

        private void AddError(int line, string message)
        {
            errors.Add(
                new CodeError(line, message));
        }

        // =========================================
        // 1. Missing Semicolon
        // =========================================

        private void CheckSemicolons()
        {
            string[] lines = code.Split('\n');

            bool insideMultilineComment = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = RemoveCommentsAndStrings(
                    lines[i],
                    ref insideMultilineComment);

                line = line.Trim();

                // خط خالی
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Preprocessor مثل #include و #define
                if (line.StartsWith("#"))
                    continue;

                // namespace declaration
                if (line.StartsWith("using namespace "))
                    continue;

                // خطوطی که ذاتاً ; نمی‌خواهند
                if (DoesNotRequireSemicolon(line))
                    continue;

                // اگر ; دارد، صحیح است
                if (line.EndsWith(";"))
                    continue;

                // شروع یا پایان block
                if (line.EndsWith("{") ||
                    line == "}")
                    continue;

                // case / default
                if (line.StartsWith("case ") ||
                    line == "default:")
                    continue;

                AddError(
                    i + 1,
                    "A semicolon ';' is missing at the end of this statement.");
            }
        }
        private bool DoesNotRequireSemicolon(string line)
        {
            string trimmed = line.Trim();

            // ساختارهای کنترلی C++
            if (trimmed.StartsWith("if ") ||
                trimmed.StartsWith("if(") ||
                trimmed.StartsWith("else") ||
                trimmed.StartsWith("for ") ||
                trimmed.StartsWith("for(") ||
                trimmed.StartsWith("while ") ||
                trimmed.StartsWith("while(") ||
                trimmed.StartsWith("switch ") ||
                trimmed.StartsWith("switch(") ||
                trimmed.StartsWith("do") ||
                trimmed.StartsWith("try") ||
                trimmed.StartsWith("catch ") ||
                trimmed.StartsWith("catch("))
            {
                return true;
            }

            // تعریف کلاس، struct و namespace
            if (trimmed.StartsWith("class ") ||
                trimmed.StartsWith("struct ") ||
                trimmed.StartsWith("namespace "))
            {
                return true;
            }

            // شروع / پایان block
            if (trimmed == "{" ||
                trimmed == "}")
            {
                return true;
            }

            // خطوطی که با { تمام می‌شوند
            if (trimmed.EndsWith("{"))
                return true;

            return false;
        }
        private string RemoveCommentsAndStrings(
    string line,
    ref bool insideMultilineComment)
        {
            string result = "";

            bool insideString = false;
            bool insideChar = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // داخل کامنت چندخطی
                if (insideMultilineComment)
                {
                    if (c == '*' &&
                        i + 1 < line.Length &&
                        line[i + 1] == '/')
                    {
                        insideMultilineComment = false;
                        i++;
                    }

                    continue;
                }

                // داخل string
                if (insideString)
                {
                    if (c == '\\' &&
                        i + 1 < line.Length)
                    {
                        i++;
                        continue;
                    }

                    if (c == '"')
                    {
                        insideString = false;
                    }

                    continue;
                }

                // داخل char
                if (insideChar)
                {
                    if (c == '\\' &&
                        i + 1 < line.Length)
                    {
                        i++;
                        continue;
                    }

                    if (c == '\'')
                    {
                        insideChar = false;
                    }

                    continue;
                }

                // شروع string
                if (c == '"')
                {
                    insideString = true;
                    continue;
                }

                // شروع char
                if (c == '\'')
                {
                    insideChar = true;
                    continue;
                }

                // شروع comment تک‌خطی
                if (c == '/' &&
                    i + 1 < line.Length &&
                    line[i + 1] == '/')
                {
                    break;
                }

                // شروع comment چندخطی
                if (c == '/' &&
                    i + 1 < line.Length &&
                    line[i + 1] == '*')
                {
                    insideMultilineComment = true;
                    i++;
                    continue;
                }

                result += c;
            }

            return result;
        }

        // =========================================
        // 2. Bracket Mismatch
        // =========================================

        private void CheckBrackets()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 3. Keyword Spelling
        // =========================================

        private void CheckKeywordSpelling()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 4. Keyword Used as Variable
        // =========================================

        private void CheckKeywordAsVariable()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 5. Invalid Variable Naming
        // =========================================

        private void CheckVariableNaming()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 6. Undefined Variable
        // =========================================

        private void CheckUndefinedVariables()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 7. String Error
        // =========================================

        private void CheckStrings()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 8. Invalid Operators
        // =========================================

        private void CheckOperators()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 9. Function Parameters
        // =========================================

        private void CheckFunctionParameters()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }

        // =========================================
        // 10. Multiline Comments
        // =========================================

        private void CheckMultilineComments()
        {
            // مرحله بعد پیاده‌سازی می‌شود
        }
    }
}