using Monkey.Object;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Monkey.Repl
{
    public class Repl
    {
        private const string Prompt = ">> ";

        private const string MonkeyFace = @"            __,__
   .--.  .-""     ""-.  .--.
  / .. \/  .-. .-.  \/ .. \
 | |  '|  /   Y   \  |'  | |
 | \   \  \ 0 | 0 /  /   / |
  \ '- ,\.-""""""""""""""-./, -' /
   ''-' /_   ^ ^   _\ '-''
       |  \._   _./  |
       \   \ '~' /   /
        '._ '-=-' _.'
           '-----'";

        public void Start(TextReader reader, TextWriter writer)
        {
            var env = new Environment();

            while (true)
            {
                writer.Write(Prompt);
                var line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    return;
                }

                var l = new Lexer.Lexer(line);
                var p = new Parser.Parser(l);

                var program = p.ParseProgram();
                if (p.Errors.Any())
                {
                    PrintParserErrors(writer, p.Errors);
                    continue;
                }

                var evaluated = new Evaluator.Evaluator().Eval(program, env);
                if (evaluated != null)
                {
                    writer.WriteLine(evaluated.Inspect());
                }
            }
        }

        private void PrintParserErrors(TextWriter writer, IEnumerable<string> errors)
        {
            writer.WriteLine(MonkeyFace);
            writer.WriteLine("Woops! We ran into some monkey business here!");
            writer.WriteLine(" parser errors:");
            foreach (var msg in errors)
            {
                writer.WriteLine("\t" + msg);
            }
        }
    }
}