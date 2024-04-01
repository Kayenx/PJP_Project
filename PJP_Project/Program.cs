using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
namespace PJP_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var fileName = "input.txt";
            Console.WriteLine("Parsing: " + fileName);
            var inputFile = new StreamReader(fileName);
            AntlrInputStream input = new AntlrInputStream(inputFile);
            PJP_Project_exprLexer lexer = new PJP_Project_exprLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            PJP_Project_exprParser parser = new PJP_Project_exprParser(tokens);

            parser.AddErrorListener(new VerboseListener());
            IParseTree tree = parser.program();

            if (parser.NumberOfSyntaxErrors == 0)
            {
                Console.WriteLine(tree.ToStringTree(parser));
                ParseTreeWalker walker = new ParseTreeWalker();

            }
            else
            {
                Console.WriteLine("Syntax error found");
            }


        }
    }
}
