using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_Project
{
    class VisitorTarget : PJP_Project_exprBaseVisitor<String>
    {
        Dictionary<String, String> Types { get; set; } = new Dictionary<String, String>();
        int labelcounter = 0;
        VisitorTypeChecking VisitorType = new VisitorTypeChecking();
        public override string VisitProgram([NotNull] PJP_Project_exprParser.ProgramContext context)
        {
            String returnstring = "";
            foreach (var statement in context.statement())
            {
                string code = this.Visit(statement);
                returnstring = returnstring + code;
            }
            return returnstring;
        }
        public override string VisitDeclarationOfVariables([NotNull] PJP_Project_exprParser.DeclarationOfVariablesContext context)
        {
            String type = context.primitiveType().GetText();
            String returnstring = "";
            foreach (var variable in context.VARIABLE())
            {
                if (type == "string")
                {
                    returnstring = returnstring + "push S \"\" \n";
                    returnstring = returnstring + "save " + variable.GetText() + "\n";
                }
                if (type == "float")
                {
                    returnstring = returnstring + "push F 0.0 \n";
                    returnstring = returnstring + "save " + variable.GetText() + "\n";
                }
                if (type == "int")
                {
                    returnstring = returnstring + "push I 0 \n";
                    returnstring = returnstring + "save " + variable.GetText() + "\n";
                }
                if (type == "bool")
                {
                    returnstring = returnstring + "push B false \n";
                    returnstring = returnstring + "save " + variable.GetText() + "\n";
                }
                Types.Add(variable.GetText(), type);    
            }
            return returnstring;
            
        }
        public override string VisitAssignment([NotNull] PJP_Project_exprParser.AssignmentContext context)
        {
            String returnstring = "";
            String type = Types[context.VARIABLE().GetText()];
            String variable = context.VARIABLE().GetText();
            String expression = this.Visit(context.expression());
            returnstring = returnstring + expression + "\n";
            string other_type = VisitorType.Visit(context.expression());
            if(type == "float" && other_type == "int")
                returnstring = returnstring + "itof" + "\n";
            returnstring = returnstring + "save " + variable + "\n";
            returnstring = returnstring + "load " + variable + "\n";
            if(!(context.parent is PJP_Project_exprParser.AssignmentContext))
                returnstring = returnstring + "pop\n";
            return returnstring;
        }
        public override string VisitMultiplyDivideModulo([NotNull] PJP_Project_exprParser.MultiplyDivideModuloContext context)
        {
            String returnstring = "";
            String type = VisitorType.Visit(context.expression()[0]);
            String other_type = VisitorType.Visit(context.expression()[1]);
            returnstring = returnstring + this.Visit(context.expression()[0]);
            if (type == "float" && other_type == "int")
                returnstring = returnstring + "itof" + "\n";
            returnstring = returnstring +  this.Visit(context.expression()[1]);
            if (context.op == context.MULT()?.Symbol)
                returnstring = returnstring + "mul\n";
            if (context.op == context.DIV()?.Symbol)
                returnstring = returnstring + "div\n";
            if (context.op == context.MOD()?.Symbol)
                returnstring = returnstring + "mod\n";
            return returnstring;
        }
        public override string VisitPlusMinusConcat([NotNull] PJP_Project_exprParser.PlusMinusConcatContext context)
        {
            String returnstring = "";
            String type = VisitorType.Visit(context.expression()[0]);
            String other_type = VisitorType.Visit(context.expression()[1]);
            returnstring = returnstring + this.Visit(context.expression()[0]);
            if (type == "float" && other_type == "int")
                returnstring = returnstring + "itof" + "\n";
            returnstring = returnstring + this.Visit(context.expression()[1]);
            if (context.op == context.PLUS()?.Symbol)
                returnstring = returnstring + "add\n";
            if (context.op == context.MINUS()?.Symbol)
                returnstring = returnstring + "sub\n";
            if (context.op == context.CONCAT()?.Symbol)
                returnstring = returnstring + "concat\n";
            return returnstring;
        }
        public override string VisitInteger([NotNull] PJP_Project_exprParser.IntegerContext context)
        {
            return "push I " + context.GetText() + "\n";
        }
        public override string VisitBoolean([NotNull] PJP_Project_exprParser.BooleanContext context)
        {
            return "push B " + context.GetText() + "\n";
        }
        public override string VisitString([NotNull] PJP_Project_exprParser.StringContext context)
        {
            return "push S " + context.GetText() + "\n";
        }
        public override string VisitFloat([NotNull] PJP_Project_exprParser.FloatContext context)
        {
            return "push F " + context.GetText() + "\n";
        }
        public override string VisitVariable([NotNull] PJP_Project_exprParser.VariableContext context)
        {
            return "load " + context.GetText() + "\n";
        }
        public override string VisitUnaryMinus([NotNull] PJP_Project_exprParser.UnaryMinusContext context)
        {
            String returnstring = "";
            returnstring = returnstring + this.Visit(context.expression());
            returnstring = returnstring + "uminus\n";
            return returnstring;
        }
        public override string VisitLogicNot([NotNull] PJP_Project_exprParser.LogicNotContext context)
        {
            String returnstring = "";
            returnstring = returnstring + this.Visit(context.expression());
            returnstring = returnstring + "not\n";
            return returnstring;
        }
        public override string VisitParentheses([NotNull] PJP_Project_exprParser.ParenthesesContext context)
        {
            return this.Visit(context.expression());
        }
        public override string VisitEmptyCommand([NotNull] PJP_Project_exprParser.EmptyCommandContext context)
        {
            return "";
        }
        public override string VisitExpressionEvaluation([NotNull] PJP_Project_exprParser.ExpressionEvaluationContext context)
        {
            return this.Visit(context.expression());
        }
        public override string VisitBlockOfStatements([NotNull] PJP_Project_exprParser.BlockOfStatementsContext context)
        {
            String returnstring = "";
            foreach (var statement in context.statement())
            {
                returnstring = returnstring + Visit(statement);
            }
            return returnstring;
        }
        public override string VisitWritetoOutput([NotNull] PJP_Project_exprParser.WritetoOutputContext context)
        {
            int i = 0;
            String returnstring = "";
            foreach (var expression in context.expression())
            {
                returnstring = returnstring + this.Visit(expression);
                i++;
            }
            returnstring = returnstring + "print " + i + "\n";
            return returnstring;
        }
        public override string VisitReadFromInput([NotNull] PJP_Project_exprParser.ReadFromInputContext context)
        {
            String returnstring = "";
            foreach (var variable in context.VARIABLE())
            {
                String type = Types[variable.GetText()];
                if (type == "string")
                    type = "S";
                if (type == "int")
                    type = "I";
                if (type == "float")
                    type = "F";
                if (type == "bool")
                    type = "B";
                returnstring = returnstring + "read " + type + "\nsave " + variable.GetText() + "\n";

            }
            return returnstring;
        }
        public override string VisitConditionalStatement([NotNull] PJP_Project_exprParser.ConditionalStatementContext context)
        {
            String returnstring = "";
            returnstring = returnstring + this.Visit(context.expression());
            int labelOne = this.labelcounter++;
            int labelTwo = this.labelcounter++;
            returnstring = returnstring + "fjmp " + labelOne + "\n";
            returnstring = returnstring + this.Visit(context.statement()[0]);
            returnstring = returnstring + "jmp " + labelTwo + "\n";
            returnstring = returnstring + "label " + labelOne + "\n";
            if (context.ELSE() != null)
                returnstring = returnstring + Visit(context.statement()[1]);
            returnstring = returnstring + "label " + labelTwo + "\n";
            return returnstring;
        }
        public override string VisitCycle([NotNull] PJP_Project_exprParser.CycleContext context)
        {
            String returnstring = "";
            int labelOne = this.labelcounter++;
            int labelTwo = this.labelcounter++;
            returnstring = returnstring + "label " + labelOne + "\n";
            returnstring = returnstring + this.Visit(context.expression());
            returnstring = returnstring + "fjmp " + labelTwo + "\n";
            returnstring = returnstring + this.Visit(context.statement());
            returnstring = returnstring + "jmp " + labelOne + "\n";
            returnstring = returnstring + "label " + labelTwo + "\n";
            return returnstring;
        }
        public override string VisitRelation([NotNull] PJP_Project_exprParser.RelationContext context)
        {
            String returnstring = "";
            String type = VisitorType.Visit(context.expression()[0]);
            String other_type = VisitorType.Visit(context.expression()[1]);
            returnstring = returnstring + this.Visit(context.expression()[0]);
            if (type != other_type)
                returnstring = returnstring + "itof" + "\n";
            returnstring = returnstring + this.Visit(context.expression()[1]);
            if (context.op == context.GREATER()?.Symbol)
                returnstring = returnstring + "gt\n";
            if (context.op == context.LESSER()?.Symbol)
                returnstring = returnstring + "lt\n";
            return returnstring;
        }
        public override string VisitComparison([NotNull] PJP_Project_exprParser.ComparisonContext context)
        {
            String returnstring = "";
            String type = VisitorType.Visit(context.expression()[0]);
            String other_type = VisitorType.Visit(context.expression()[1]);
            returnstring = returnstring + this.Visit(context.expression()[0]);
            if (type == "float" && other_type == "int")
                returnstring = returnstring + "itof" + "\n";
            returnstring = returnstring + this.Visit(context.expression()[1]);
            returnstring = returnstring + "eq\n";
            if (context.op == context.NEQ()?.Symbol)
                returnstring = returnstring + "not\n";
            return returnstring;
        }
        public override string VisitLogicAnd([NotNull] PJP_Project_exprParser.LogicAndContext context)
        {
            String returnstring = "";
            returnstring = returnstring + this.Visit(context.expression()[0]);
            returnstring = returnstring + this.Visit(context.expression()[1]);
            returnstring = returnstring + "and\n";
            return returnstring;
        }
        public override string VisitLogicOR([NotNull] PJP_Project_exprParser.LogicORContext context)
        {
            String returnstring = "";
            returnstring = returnstring + this.Visit(context.expression()[0]);
            returnstring = returnstring + this.Visit(context.expression()[1]);
            returnstring = returnstring + "or\n";
            return returnstring;
        }
    }
}
