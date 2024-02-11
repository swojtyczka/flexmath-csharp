using System;
using System.Collections.Generic;

// Tokeny są "atomami" w wyrażeniach składni konkretnej
class Token
{
	public string type;
	public string symbol;
	public Token(string t, string s)
	{
		type = t;
		symbol = s;
	}
}

class Syntax_Error : Exception
{
	public Syntax_Error(string msg) : base(msg) { }
}

// Lexer zajmuje się analizą syntaktyczną wyrażenia i zamienia je na listę tokenów
class Lexer
{
	string input;
	int position;

	string char_to_type(char t)
	{
		switch (t)
		{
			case '+': return "PLUS";
			case '-': return "MINUS";
			case '*': return "MULT";
			case '/': return "DIV";
			case '(': return "LPAREN";
			case ')': return "RPAREN";
			case ',': return "ARGSEP";
			default: return "NONE";
		}
	}

	public Lexer(string str)
	{
		input = str;
		position = 0;
	}

	// Pomija wszystkie białe znaki w stringu wejścia
	public void skip_whitespace()
	{
		while (position < input.Length && Char.IsWhiteSpace(input[position]))
			position++;
	}

	// Zwraca pierwszy następny znak w wejściu nie będący białym znakiem
	public char look_ahead()
	{
		skip_whitespace();
		if (position < input.Length)
			return input[position];
		else return (char)0;
	}

	// Zamiana stringa na listę tokenów
	public List<Token> tokenize()
	{
		List<Token> output = new List<Token>();
		string bufor = ""; // symbol pojedynczego tokena
		char cursor = look_ahead();

		while (position < input.Length)
		{
			string type = char_to_type(cursor);
			if (type != "NONE" && type != "MINUS")
			{
				output.Add(new Token(type, ""));
				position++;
			}

			// Osobny przypadek dla minusa - może być unarny lub binarny
			else if (cursor == '-')
			{
				// Jeśli to pierwszy znak to możemy potraktować minus jako odejmowanie od 0
				if (position == 0)
				{
					output.Add(new Token("NUM", "0"));
					output.Add(new Token("MINUS", ""));
				}

				// Jeśli poprzedni znak to była liczba lub nawias zamykający
				// to znaczy, że minus jest binary i oznacza odejmowanie
				else if (Char.IsLetterOrDigit(input[position - 1]) || input[position - 1] == ')')
				{
					output.Add(new Token("MINUS", ""));
				}

				// W przeciwnym wypadku minus musi być unarny
				// Sprawdzamy, czy nie było już minusa w obecnym symbolu
				else if (bufor.IndexOf('-') == -1)
					bufor = "-";
				else
					throw new Syntax_Error("Lexer: Unexpected '-'");
				position++;
			}

			else if (Char.IsDigit(cursor))
			{
				while (Char.IsDigit(cursor) || cursor == '.')
				{
					bufor += cursor;
					position++;
					cursor = look_ahead();
				}
				output.Add(new Token("NUM", bufor));
				bufor = "";
			}

			else if (Char.IsLetter(cursor))
			{
				while (Char.IsLetterOrDigit(cursor))
				{
					bufor += cursor;
					position++;
					cursor = look_ahead();
				}
				output.Add(new Token("ID", bufor));
				bufor = "";
			}

			else
				throw new Syntax_Error("Lexer: Invalid character (" + cursor + ")");

			cursor = look_ahead();
		}

		return output;
	}
}