# Interpreter wyrażeń arytmetycznych

## Zadania

- rozumienie wyrażeń w postaci infiksowej, naturalnej dla człowieka
- tworzenie własnych zmiennych przez użytkownika
- możliwość zapisywania i oczytywania stanu pamięci do/z pliku
- obliczanie wyrażenia podanego jako argument wywołania

## 1. Lexer

Zadaniem lexera będzie przerobić string zawierający wyrażenie na listę tokenów.
Na przykład dla napisu `"x*(3+5)/2"` lexer zwróci listę wyglądającą podobnie do:

```cs
{
    {"ID", "x"},
    {"MULT", "*"},
    {"LPAREN", "("},
    {"NUM", "3"},
    {"PLUS", "+"},
    {"NUM", "5"},
    {"RPAREN", ")"},
    {"DIV", "/"},
    {"NUM", "2"}
}
```

## 2. Parser

Parser przyjmie listę tokenów i na jej podstawie wygeneruje drzewo: składnię abstrakcyjną wyrażenia\
Dla listy tokenów z przykładu drzewo będzie miało strukturę:

```cs
        (/)
      /    \
    (*)    (2)
    /  \
  (x)  (+)
      /   \
     (3)  (5)
```

## 3. Ewaluacja

Otrzymane drzewo możemy następnie obliczyć do wartości dzięki rekurencyjnej metodzie klasy `Expression`: `Evaluate()`

## 4. Program główny

Najpierw zostaje obliczone ewentualne wyrażenie z argumentu wywołania a następnie program będzie oczekiwał na wczytanie wyrażeń i je obliczał (lub informował o błędach) do zamknięcia programu. Jeśli wejście użytkownika będzie zaczyna się od `/` to program będzie je traktował jako polecenie.
