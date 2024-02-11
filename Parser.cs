using System;
using System.Collections.Generic;
using System.Globalization;

class Not_parsed : Exception
{
	public Not_parsed(string msg) : base(msg) { }
}

/// Parser zamienia składnię konkretną na abstrakcyjną
class Parser
{
	List<Token> tokenList;
	int position;

	public Parser(List<Token> input)
	{
		tokenList = input;
		position = 0;
	}

	/// Zwraca token na aktualnej pozycji
	public Token GetToken()
	{
		if (position < tokenList.Count)
			return tokenList[position];
		else return new Token("EOT", "");
	}

	/// Parsuje wyrażenie
	public Expression parse_Expression()
	{
		Expression e = parse_Sum();
		if (position == tokenList.Count)
			return e;
		else throw new Not_parsed("Parse: Not parsed");
	}

	/// Parsuje wyrażenie jako sumę
	public Expression parse_Sum()
	{
		Expression e = parse_Mult();
		Token t = GetToken();

		while (t.type == "PLUS" || t.type == "MINUS")
		{
			position++;
			e = new Operator(t.type, e, parse_Mult());
			t = GetToken();
		}
		return e;
	}

	/// Parsuje wyrażenie jako iloczyn
	public Expression parse_Mult()
	{
		Expression e = parse_Term();
		Token t = GetToken();

		while (t.type == "MULT" || t.type == "DIV")
		{
			position++;
			e = new Operator(t.type, e, parse_Term());
			t = GetToken();
		}
		return e;
	}

	/// Parsuje termy
	public Expression parse_Term()
	{
		Token t = GetToken();
		if (t.type == "NUM")
			return parse_Constant();
		else if (t.type == "ID")
		{
			if (Function.Arity.ContainsKey(t.symbol))
				return parse_Function();
			else return parse_Variable();
		}
		else if (t.type == "LPAREN")
			return parse_Paren();
		else throw new Not_parsed("Parse: Invalid token");
	}

	/// Parsuje stałe liczbowe
	public Expression parse_Constant()
	{
		Token t = GetToken();
		double n = Double.Parse(t.symbol, new CultureInfo("en-US"));
		position++;
		return new Constant(n);
	}

	/// Parsuje zmienne
	public Expression parse_Variable()
	{
		Token t = GetToken();
		position++;
		return new Variable(t.symbol);
	}

	/// Parsuje funkcje
	public Expression parse_Function()
	{
		string name = GetToken().symbol;
		position++;
		if (GetToken().type == "LPAREN")
		{
			List<Expression> args = new List<Expression>();
			do
			{
				position++;
				Expression e = parse_Sum();
				args.Add(e);
			} while (GetToken().type == "ARGSEP");

			if (GetToken().type == "RPAREN")
			{
				position++;
				return new Function(name, args);
			}
			else throw new Not_parsed("Parse: Expected ')'");
		}
		else throw new Not_parsed("Parse: Function call without parenthesis");
	}

	/// Parsuje wyrażenie w nawiasach
	public Expression parse_Paren()
	{
		position++;
		Expression e = parse_Sum();
		Token t = GetToken();
		if (t.type == "RPAREN")
		{
			position++;
			return e;
		}
		else throw new Not_parsed("Parse: Expected ')'");
	}
}
