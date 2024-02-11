using System;
using System.Collections.Generic;

using Memory = System.Collections.Generic.Dictionary<string, double>;

class Variable_not_found : Exception
{
	public Variable_not_found(string n) : base("Eval: Variable " + n + " not found") { }
}
class Function_not_found : Exception
{
	public Function_not_found(string n) : base("Eval: Function " + n + " not found") { }
}

class Operator_not_found : Exception
{
	public Operator_not_found(string op) : base("Eval: Operator " + op + " not found") { }
}

class Arity_mismatch : Exception
{
	public Arity_mismatch(int ex, int gv) : base("Eval: Arity mismatch! Expected: " + ex + " Given: " + gv) { }
}

/// Abstrakcyjna klasa wyrażenia
abstract class Expression
{
	public abstract double Evaluate(Memory mem);
}

/// Stała liczbowa
class Constant : Expression
{
	double value;
	public Constant(double v) { value = v; }

	public override double Evaluate(Memory mem)
	{
		return value;
	}
}

/// Zmienna
class Variable : Expression
{
	string name;

	public Variable(string s) { name = s; }

	public override double Evaluate(Memory mem)
	{
		if (mem.ContainsKey(name))
			return mem[name];
		else throw new Variable_not_found(name);
	}
}

/// Funkcja
class Function : Expression
{
	string name;
	List<Expression> args;

	public static readonly Dictionary<string, int> Arity = new Dictionary<string, int>
	{
		{"pow", 2},
		{"mod", 2},
		{"log", 2},
		{"log2", 1},
		{"log10", 1},
		{"ln", 1},
		{"exp", 1},
		{"sqrt", 1},
		{"sin", 1},
		{"cos", 1},
		{"tg", 1},
		{"ctg", 1},
		{"abs", 1}
	};

	public Function(string s, List<Expression> a)
	{
		name = s;
		args = a;
	}

	public override double Evaluate(Memory mem)
	{
		if (Arity.ContainsKey(name))
		{
			if (Arity[name] != args.Count)
				throw new Arity_mismatch(Arity[name], args.Count);

			List<double> args_values = new List<double>();
			foreach (Expression e in args)
				args_values.Add(e.Evaluate(mem));

			switch (name)
			{
				case "pow": return Math.Pow(args_values[0], args_values[1]);
				case "mod": return (int)(args_values[0]) % (int)(args_values[1]);
				case "log": return Math.Log(args_values[0]) / Math.Log(args_values[1]);
				case "log2": return Math.Log(args_values[0]) / Math.Log(2);
				case "log10": return Math.Log10(args_values[0]);
				case "ln": return Math.Log(args_values[0]);
				case "exp": return Math.Exp(args_values[0]);
				case "sqrt": return Math.Sqrt(args_values[0]);
				case "sin": return Math.Sin(args_values[0]);
				case "cos": return Math.Cos(args_values[0]);
				case "tg": return Math.Tan(args_values[0]);
				case "ctg": return 1.0 / Math.Tan(args_values[0]);
				case "abs": return Math.Abs(args_values[0]);
				default: throw new Function_not_found(name);
			}
		}
		else throw new Function_not_found(name);
	}

}

/// Operator
class Operator : Expression
{
	string symbol;
	Expression left;
	Expression right;

	public Operator(string s, Expression l, Expression r)
	{
		symbol = s;
		left = l;
		right = r;
	}

	public override double Evaluate(Memory mem)
	{
		double l = left.Evaluate(mem);
		double r = right.Evaluate(mem);
		switch (symbol)
		{
			case "PLUS": return l + r;
			case "MINUS": return l - r;
			case "MULT": return l * r;
			case "DIV": return l / r;
			default:
				throw new Operator_not_found(symbol);
		}
	}
}