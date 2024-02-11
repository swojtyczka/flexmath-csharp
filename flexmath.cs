using System;
using System.Collections.Generic;
using System.IO;

class Program
{
	/// Pamięć programu
	static Dictionary<string, double> Memory = new Dictionary<string, double>() { { "ans", 0 } };

	/// Ewaluacja wyrażenia
	static double Eval(string expr)
	{
		Lexer L = new Lexer(expr);
		Parser P = new Parser(L.tokenize());
		Expression E = P.parse_Expression();
		return E.Evaluate(Memory);
	}

	/// Ewaluacja + wypisanie
	static void EvalPrint(string expr, string variable)
	{
		try
		{
			double res = Eval(expr);
			Memory[variable] = res;
			Console.WriteLine(res);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	/// Wyświetlanie logo
	static void Logo()
	{
		string logo = @"  _____.__                                __  .__     
_/ ____\  |   ____ ___  ___ _____ _____ _/  |_|  |__  
\   __\|  | _/ __ \\  \/  ///     \\__  \\   __\  |  \ 
 |  |  |  |_\  ___/ >    <|  Y Y  \/ __ \|  | |   Y  \
 |__|  |____/\___  >__/\_ \__|_|  (____  /__| |___|  /
                 \/      \/     \/     \/          \/";
		Console.WriteLine(logo);
		Console.WriteLine("(c) 2024 Szymon Wojtyczka. All rights reserved.");
	}

	public static void Main(string[] args)
	{
		Logo();

		if (args.GetLength(0) > 0)
		{
			Console.WriteLine("> " + string.Join(" ", args));
			EvalPrint(string.Join(" ", args), "ans");
		}

		string query = "";
		while (query != "/q")
		{
			Console.Write("> ");
			query = Console.ReadLine();
			if (query.Length == 0)
				continue;
			else if (query[0] == '/')
			{
				string[] parameters = query.Split(' ');
				switch (parameters[0])
				{
					case "/q":
					case "/quit":
						return;

					case "/help":
						Console.WriteLine("/q /quit - Quits the program");
						Console.WriteLine("/help - Shows this list");
						Console.WriteLine("/let {x} {expr} - Evaluates expr and binds it to x");
						Console.WriteLine("/cls /clear - Clears the console");
						Console.WriteLine("/memory - Shows memory");
						Console.WriteLine("/save {path} - Saves memory to path");
						Console.WriteLine("/load {path} - Loads memory from path");
						break;

					case "/let":
						string name = parameters[1];
						string val = string.Join("", parameters, 2, parameters.GetLength(0) - 2);
						Console.Write(name + " <- ");
						EvalPrint(val, name);
						break;

					case "/clear":
					case "/cls":
						Console.Clear();
						Logo();
						break;

					case "/save":
						try
						{
							File.WriteAllText(parameters[1], "");
							foreach (var x in Memory)
								File.AppendAllText(parameters[1], x.Key + "=" + x.Value + "\n");
						}
						catch (Exception e)
						{
							Console.WriteLine(e.Message);
						}
						break;

					case "/load":
						try
						{
							var lines = File.ReadLines(parameters[1]);
							foreach (var line in lines)
							{
								string[] p = line.Split('=');
								Memory[p[0]] = Double.Parse(p[1]);
							}
						}
						catch (Exception e)
						{
							Console.WriteLine(e.Message);
						}
						break;

					case "/memory":
						foreach (var x in Memory)
							Console.WriteLine(x.Key + " = " + x.Value);
						break;

					default:
						Console.WriteLine("Invalid command. See /help");
						break;
				}
			}
			else EvalPrint(query, "ans");
		}
	}
}