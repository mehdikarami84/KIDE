using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIDE
{
    internal class CodeError
    {
        public int Line { get; }
        public string Message { get; }

        public CodeError(int line, string message)
        {
            Line = line;
            Message = message;
        }

        public override string ToString()
        {
            return $"Line {Line}: {Message}";
        }
    }
}
