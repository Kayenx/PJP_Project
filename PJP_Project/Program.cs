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
            }
            else
            {
                Console.WriteLine("Syntax error found");
                return;
            }
            VisitorTypeChecking visitorTypeChecking = new VisitorTypeChecking();
            String type = visitorTypeChecking.Visit(tree);
            if (visitorTypeChecking.Errors.Count > 0)
            {
                foreach (var error in visitorTypeChecking.Errors)
                {
                    Console.Write(error);
                }
                return;
            }
            VisitorTarget visitorTarget = new VisitorTarget();
            String target_code = visitorTarget.Visit(tree);
            try
            {
                using(StreamWriter sw = new StreamWriter("output.txt"))
                {
                    sw.Write(target_code);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            var target_input = File.ReadAllText("output.txt");
            VirtualMachine vm = new VirtualMachine(target_input);
            vm.Run();
        }
    }
}
