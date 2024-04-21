using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_Project
{
    public class VirtualMachine
    {
        Stack<(Type type, object value)> stack = new();
        private List<string> code = new List<string>();
        Dictionary<string, (Type type, object value)> memory = new();
        private Dictionary<int, int> labels = new();

        public VirtualMachine(string Text)
        {
            string[] lines = Text.Split('\n');
            int i = 0;
            foreach (var line in lines)
            {
                code.Add(line);
                if (line.StartsWith("label"))
                {
                    var index = int.Parse(line.Split(' ')[1]);
                    labels.Add(index, i);
                }
                i++;
            }
        }
        public void Run()
        {
            int current = 0;
            while (current < code.Count)
            {
                var command = code[current].Split(" ");
                var commandName = command[0];
                var commandArgs = command.Skip(1).ToArray();

                switch(commandName)
                {
                    case "jmp":
                        current = labels[int.Parse(commandArgs[0])];
                        break;
                    case "fjmp":
                        current = (bool)stack.Pop().value ? current : labels[int.Parse(commandArgs[0])];
                        break;
                    case "label":
                        break;
                    case "load":
                        stack.Push(memory[commandArgs[0]]);
                        break;
                    case "save":
                        var value = stack.Pop();
                        memory[commandArgs[0]] = value;
                        break;
                    case "print":
                        var n = int.Parse(commandArgs[0]);
                        var items = new object[n];
                        for (int i = n -1  ; i >= 0; i--)
                        {
                            items[i] = stack.Pop().value;
                        }
                        Console.WriteLine(string.Join("", items));
                        break;
                    case "read":
                        var input = Console.ReadLine();
                        object parsedValue = null;
                        Type type = null;
                        switch (commandArgs[0])
                        {
                            case "I":
                                parsedValue = int.Parse(input);
                                type = typeof(int);
                                break;
                            case "F":
                                parsedValue = float.Parse(input);
                                type = typeof(float);
                                break;
                            case "B":
                                parsedValue = bool.Parse(input);
                                type = typeof(bool);
                                break;
                            case "S":
                                parsedValue = input.Replace("\"", String.Empty);
                                type = typeof(string);
                                break;
                        }
                        stack.Push((type, parsedValue));
                        break;
                    case "pop":
                        stack.Pop();
                        break;
                    case "push":
                        var p_type = commandArgs[0];
                        var val = commandArgs[1];
                        if (p_type == "S")
                            for (int i = 2; i < commandArgs.Length; i++)
                                val += " " + commandArgs[i];
                         parsedValue = null;
                         type = null;
                        switch (commandArgs[0])
                        {
                            case "I":
                                parsedValue = int.Parse(val);
                                type = typeof(int);
                                break;
                            case "F":
                                parsedValue = float.Parse(val);
                                type = typeof(float);
                                break;
                            case "B":
                                parsedValue = bool.Parse(val);
                                type = typeof(bool);
                                break;
                            case "S":
                                parsedValue = val.Replace("\"", String.Empty);
                                type = typeof(string);
                                break;
                        }
                        stack.Push((type, parsedValue));
                        break;
                    case "itof":
                        value = stack.Pop();
                        stack.Push((typeof(float), (float)Convert.ToSingle(value.value)));
                        break;
                    case "not":
                        value = stack.Pop();
                        stack.Push((typeof(bool), !(bool)value.value));
                        break;
                    case "eq":
                        var rightValue = stack.Pop();
                        var leftValue = stack.Pop();
                        bool result = false;
                        type = leftValue.type;
                        if (type == typeof(int))
                            result = (int)Convert.ToInt32(leftValue.value) == (int)Convert.ToInt32(rightValue.value);
                        if(type == typeof(float))
                            result = (float)leftValue.value == (float)rightValue.value;
                        if (type == typeof(string))
                            result = (string)leftValue.value == (string)rightValue.value;
                        stack.Push((typeof(bool), result));
                        break;
                    case "lt":
                         rightValue = stack.Pop();
                         leftValue = stack.Pop();
                         result = false;
                        type = leftValue.type;
                        if (type == typeof(int))
                            result = (int)Convert.ToInt32(leftValue.value) < (int)Convert.ToInt32(rightValue.value);
                        if (type == typeof(float))
                            result = (float)leftValue.value < (float)rightValue.value;
                        stack.Push((typeof(bool), result));
                        break;
                    case "gt":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        result = false;
                        type = leftValue.type;
                        if (type == typeof(int))
                            result = (int)Convert.ToInt32(leftValue.value) > (int)Convert.ToInt32(rightValue.value);
                        if (type == typeof(float))
                            result = (float)leftValue.value > (float)rightValue.value;
                        stack.Push((typeof(bool), result));
                        break;
                    case "or":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        result = (bool)leftValue.value || (bool)rightValue.value;
                        stack.Push((typeof(bool), result));
                        break;
                    case "and":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        result = (bool)leftValue.value && (bool)rightValue.value;
                        stack.Push((typeof(bool), result));
                        break;
                    case "concat":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        stack.Push((typeof(string), $"{(string)leftValue.value}{(string)rightValue.value}"));
                        break;
                    case "uminus":
                        value = stack.Pop();
                        type = value.type;
                        if (type == typeof(int))
                            stack.Push((type, -(int)Convert.ToInt32(value.value)));
                        if (type == typeof(float))
                            stack.Push((type, -(float)value.value));
                        break;
                    case "mod":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        stack.Push((typeof(int), (int)Convert.ToInt32(leftValue.value) % (int)Convert.ToInt32(rightValue.value)));
                        break;
                    case "div":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        type = leftValue.type;
                        if (type == typeof(int))
                            stack.Push((typeof(int), (int)Convert.ToInt32(leftValue.value) / (int)Convert.ToInt32(rightValue.value)));
                        if (type == typeof(float))
                            stack.Push((typeof(float), (float)leftValue.value / (float)rightValue.value));
                        break;
                    case "mul":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        type = leftValue.type;
                        if (type == typeof(int))
                            stack.Push((typeof(int), (int)Convert.ToInt32(leftValue.value) * (int)Convert.ToInt32(rightValue.value)));
                        if (type == typeof(float))
                            stack.Push((typeof(float), (float)Convert.ToSingle(leftValue.value) * (float)Convert.ToSingle(rightValue.value)));
                        break;
                    case "add":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        type = leftValue.type;
                        if (type == typeof(int))
                            stack.Push((typeof(int), (int)Convert.ToInt32(leftValue.value) + (int)Convert.ToInt32(rightValue.value)));
                        if (type == typeof(float))
                            stack.Push((typeof(float), (float)leftValue.value + (float)rightValue.value));
                        break;
                    case "sub":
                        rightValue = stack.Pop();
                        leftValue = stack.Pop();
                        type = leftValue.type;
                        if (type == typeof(int))
                            stack.Push((typeof(int), (int)Convert.ToInt32(leftValue.value) - (int)Convert.ToInt32(rightValue.value)));
                        if (type == typeof(float))
                            stack.Push((typeof(float), (float)leftValue.value - (float)rightValue.value));
                        break;
                }
                current++;
            }

        }
    }
}
