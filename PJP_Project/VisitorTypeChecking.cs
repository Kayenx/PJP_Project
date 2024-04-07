using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_Project
{
    public class VisitorTypeChecking : PJP_Project_exprBaseVisitor<String>
    {
        Dictionary<String, String> Types { get; set; } = new Dictionary<string, String>();

        public List<String> Errors { get; set; } = new List<string>();
        public override String VisitDeclarationOfVariables([NotNull] PJP_Project_exprParser.DeclarationOfVariablesContext context)
        {
            String type = context.primitiveType().GetText();
            foreach (var variable in context.VARIABLE())
            {
                if (Types.ContainsKey(variable.GetText()))
                {
                    Errors.Add("Promenna deklarovana dvakrat - " + context.Start.Line + "\n");
                }
                else
                {
                    Types.Add(variable.GetText(), type);
                }
            }
            return "int";
        }
        public override String VisitAssignment([NotNull] PJP_Project_exprParser.AssignmentContext context)
        {
            String variable = context.VARIABLE().GetText();
            String result = this.Visit(context.expression());
            if (!Types.ContainsKey(variable))
            {
                Errors.Add("Promena nedeklarovana - " + context.Start.Line + "\n");
            }
            else if (Types[variable] != result)
            {
                if(Types[variable] == "float" && result == "int")
                    { }
                else
                    Errors.Add("Spatny datovy typ - " + context.Start.Line  + "\n");
            }
            return "int";
        }
        public override string VisitUnaryMinus([NotNull] PJP_Project_exprParser.UnaryMinusContext context)
        {
            String type = this.Visit(context.expression());
            if (type != "int" && type != "float")
            {
                Errors.Add("Spatny datovy typ - " + context.Start.Line + "\n");
            }

            return type;
        }
        public override string VisitLogicNot([NotNull] PJP_Project_exprParser.LogicNotContext context)
        {
            String type = this.Visit(context.expression());
            if (type != "bool")
            {
                Errors.Add("Spatny datovy typ - " + context.Start.Line + "\n");
            }

            return type;
        }
        public override string VisitBoolean([NotNull] PJP_Project_exprParser.BooleanContext context)
        {
            return "bool";
        }
        public override string VisitInteger([NotNull] PJP_Project_exprParser.IntegerContext context)
        {
            return "int";
        }
        public override string VisitFloat([NotNull] PJP_Project_exprParser.FloatContext context)
        {
            return "float";
        }
        public override string VisitString([NotNull] PJP_Project_exprParser.StringContext context)
        {
            return "string";
        }
        public override string VisitVariable([NotNull] PJP_Project_exprParser.VariableContext context)
        {
            if(!Types.ContainsKey(context.GetText()))
            {
                Errors.Add("Promenna neexistuje " + context.GetText() + context.Start.Line + "\n");
                return "int";
            }
            return Types[context.GetText()];
        }
        public override string VisitMultiplyDivideModulo([NotNull] PJP_Project_exprParser.MultiplyDivideModuloContext context)
        {
            if (context.op == context.MULT()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if ((type1 != "int" && type1 != "float") || (type2 != "int" && type2 != "float"))
                {
                    Errors.Add("Nelze provest nasobeni mezi jinymi typy nezli int a float - " + context.Start.Line + "\n");
                }
                if (type1 == "float" || type2 == "float")
                {
                    return "float";
                }
                else
                {
                    return "int";
                }
            }
            if (context.op == context.DIV()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if ((type1 != "int" && type1 != "float") || (type2 != "int" && type2 != "float"))
                {
                    Errors.Add("Nelze provest deleni mezi jinymi typy nezli int a float - " + context.Start.Line + "\n");
                }
                if (type1 == "float" || type2 == "float")
                {
                    return "float";
                }
                else
                {
                    return "int";
                }
            }
            if (context.op == context.MOD()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);

                if (type1 != "int"  || type2 != "int")
                {
                    Errors.Add("Nelze provest modulo mezi jinym typem nezli int  - " + context.Start.Line + "\n");
                }
                return "int";
            }
            return "int";

        }
        public override string VisitPlusMinusConcat([NotNull] PJP_Project_exprParser.PlusMinusConcatContext context)
        {
            if (context.op == context.PLUS()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if ((type1 != "int" && type1 != "float") || (type2 != "int" && type2 != "float"))
                {
                    Errors.Add("Nelze provest scitani mezi jinymi typy nezli int a float - " + context.Start.Line + "\n");
                }
                if (type1 == "float" || type2 == "float")
                {
                    return "float";
                }
                else
                {
                    return "int";
                }
            }
            if (context.op == context.MINUS()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if ((type1 != "int" && type1 != "float") || (type2 != "int" && type2 != "float"))
                {
                    Errors.Add("Nelze provest nasobeni mezi jinymi typy nezli int a float - " + context.Start.Line + "\n");
                }
                if (type1 == "float" || type2 == "float")
                {
                    return "float";
                }
                else
                {
                    return "int";
                }
            }
            if (context.op == context.CONCAT()?.Symbol)
            {
                String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if (type1 != "string" || type2 != "string" )
                {
                    Errors.Add("Nelze provest concat mezi jinymi typy nezli string - " + context.Start.Line + "\n");
                }
                return "string";
            }
            return "int";
        }
        public override string VisitRelation([NotNull] PJP_Project_exprParser.RelationContext context)
        {
            String type1 = this.Visit(context.children[0]);
                String type2 = this.Visit(context.children[2]);
                if ((type1 != "int" && type1 != "float") || (type2 != "int" && type2 != "float"))
                {
                    Errors.Add("Nelze provest porovnavani mezi jinymi typy nezli int a float - " + context.Start.Line + "\n");
                }
            return "bool";
            
        }
        public override string VisitComparison([NotNull] PJP_Project_exprParser.ComparisonContext context)
        {
            String type1 = this.Visit(context.children[0]);
            String type2 = this.Visit(context.children[2]);
            if (type1 == "bool" || type2 == "bool")
            {
                Errors.Add("Nelze provest porovnavani mezi booly - " + context.Start.Line + "\n");
            }
            return "bool";
        }
        public override string VisitLogicAnd([NotNull] PJP_Project_exprParser.LogicAndContext context)
        {
            String type1 = this.Visit(context.children[0]);
            String type2 = this.Visit(context.children[2]);
            if (type1 != "bool" || type2 != "bool")
            {
                Errors.Add("Lze pouzit and pouze mezi booly - " + context.Start.Line + "\n");
            }
            return "bool";
        }
        public override string VisitLogicOR([NotNull] PJP_Project_exprParser.LogicORContext context)
        {
            String type1 = this.Visit(context.children[0]);
            String type2 = this.Visit(context.children[2]);
            if (type1 != "bool" || type2 != "bool")
            {
                Errors.Add("Lze pouzit and pouze mezi booly - " + context.Start.Line + "\n");
            }
            return "bool";
        }
        public override string VisitConditionalStatement([NotNull] PJP_Project_exprParser.ConditionalStatementContext context)
        {
            String type = this.Visit(context.children[2]);
            if (type != "bool")
                Errors.Add("Condition neni bool");
            return "int";
        }
        public override string VisitCycle([NotNull] PJP_Project_exprParser.CycleContext context)
        {
            String type = this.Visit(context.children[2]);
            if (type != "bool")
                Errors.Add("Condition neni bool");
            return "int";
        }
        public override string VisitParentheses([NotNull] PJP_Project_exprParser.ParenthesesContext context)
        {
            return this.Visit(context.expression());
        }
    }
}
