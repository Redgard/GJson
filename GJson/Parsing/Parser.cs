
using System;
using System.Collections.Generic;

namespace GJson
{


public partial class Parser
{

	public enum ETerminal
	{
		EOF = 0,
		TNumber = 1,
		TString = 2,
		TNull = 3,
		TTrue = 4,
		TFalse = 5,
		QuotationMark = 6,
		CurlyBracketOpen = 7,
		CurlyBracketClose = 8,
		SquareBracketOpen = 9,
		SquareBracketClose = 10,
		Colon = 11,
		Comma = 12,
		Tab = 13
	}

	public const int MaxT = 14;

	public enum ENonTerminal
	{
		Json,
		Value,
		String,
		Number,
		True,
		False,
		Null,
		Object,
		ObjectList,
		ObjectItem,
		Array,
		ArrayList,
		ArrayItem
	}


	const bool T = true;
	const bool F = false;
	const int minErrDist = 2;
	
	Scanner _scanner;

	Token t;    // last recognized token
	Token la;   // lookahead token
	int errDist;
	
    partial void ProductionBegin( ENonTerminal production );
    partial void ProductionEnd( ENonTerminal production );
	
	public ParserErrors Errors { get; set; }
	
    public string CurrentToken { get { return t.val; } }
	
	int GetNextTokenKind() { return _scanner.Peek().kind; }
	


    public Parser()
    {
        Errors = new ParserErrors();
    }

	void SyntaxError(int n)
	{
		if (errDist >= minErrDist) Errors.SyntaxError(la.line, la.col, n);
		errDist = 0;
	}

	void SemanticError(string msg)
	{
		if (errDist >= minErrDist) Errors.SemanticError(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get()
	{
		while (true)
		{
			t = la;
			la = _scanner.Scan();
			if (la.kind <= MaxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect(ETerminal n)
	{
		if (la.kind==(int)n) Get(); else { SyntaxError((int)n); }
	}
	
	bool StartOf(int s)
	{
		return set[s, la.kind];
	}
	
	void ExpectWeak(ETerminal n, int follow)
	{
		if (la.kind == (int)n) Get();
		else
		{
			SyntaxError((int)n);
			while (!StartOf(follow)) Get();
		}
	}

	bool WeakSeparator(int n, int syFol, int repFol)
	{
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else 
		{
			SyntaxError(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) 
			{
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}
	
	void Json() {
		ProductionBegin( ENonTerminal.Json );
		Value();
		ProductionEnd( ENonTerminal.Json );
	}

	void Value() {
		ProductionBegin( ENonTerminal.Value );
		switch ((ETerminal)la.kind) {
		case ETerminal.CurlyBracketOpen: {
			Object();
			break;
		}
		case ETerminal.SquareBracketOpen: {
			Array();
			break;
		}
		case ETerminal.TString: {
			String();
			break;
		}
		case ETerminal.TNumber: {
			Number();
			break;
		}
		case ETerminal.TTrue: {
			True();
			break;
		}
		case ETerminal.TFalse: {
			False();
			break;
		}
		case ETerminal.TNull: {
			Null();
			break;
		}
		default: SyntaxError(15); break;
		}
		ProductionEnd( ENonTerminal.Value );
	}

	void String() {
		ProductionBegin( ENonTerminal.String );
		Expect(ETerminal.TString);
		ProductionEnd( ENonTerminal.String );
	}

	void Number() {
		ProductionBegin( ENonTerminal.Number );
		Expect(ETerminal.TNumber);
		ProductionEnd( ENonTerminal.Number );
	}

	void True() {
		ProductionBegin( ENonTerminal.True );
		Expect(ETerminal.TTrue);
		ProductionEnd( ENonTerminal.True );
	}

	void False() {
		ProductionBegin( ENonTerminal.False );
		Expect(ETerminal.TFalse);
		ProductionEnd( ENonTerminal.False );
	}

	void Null() {
		ProductionBegin( ENonTerminal.Null );
		Expect(ETerminal.TNull);
		ProductionEnd( ENonTerminal.Null );
	}

	void Object() {
		ProductionBegin( ENonTerminal.Object );
		Expect(ETerminal.CurlyBracketOpen);
		if (la.kind == (int)ETerminal.TString) {
			ObjectList();
		}
		Expect(ETerminal.CurlyBracketClose);
		ProductionEnd( ENonTerminal.Object );
	}

	void ObjectList() {
		ProductionBegin( ENonTerminal.ObjectList );
		ObjectItem();
		while (la.kind == (int)ETerminal.Comma) {
			Get();
			ObjectItem();
		}
		ProductionEnd( ENonTerminal.ObjectList );
	}

	void ObjectItem() {
		ProductionBegin( ENonTerminal.ObjectItem );
		String();
		Expect(ETerminal.Colon);
		Value();
		ProductionEnd( ENonTerminal.ObjectItem );
	}

	void Array() {
		ProductionBegin( ENonTerminal.Array );
		Expect(ETerminal.SquareBracketOpen);
		if (StartOf(1)) {
			ArrayList();
		}
		Expect(ETerminal.SquareBracketClose);
		ProductionEnd( ENonTerminal.Array );
	}

	void ArrayList() {
		ProductionBegin( ENonTerminal.ArrayList );
		ArrayItem();
		while (la.kind == (int)ETerminal.Comma) {
			Get();
			ArrayItem();
		}
		ProductionEnd( ENonTerminal.ArrayList );
	}

	void ArrayItem() {
		ProductionBegin( ENonTerminal.ArrayItem );
		Value();
		ProductionEnd( ENonTerminal.ArrayItem );
	}



    public void Parse( Scanner s )
    {
        _scanner = s;
		errDist = minErrDist;
        Errors.Clear();

        Parse();
    }
	
	public void Parse() 
	{
		la = new Token();
		la.val = "";		
		Get();
		Json();
		Expect(ETerminal.EOF);

	}
	
	static readonly bool[,] set =
	{
		{T,F,F,F, F,F,F,F, F,F,F,F, F,F,F,F},
		{F,T,T,T, T,T,F,T, F,T,F,F, F,F,F,F}

	};
}

public sealed class ParserErrors
{
    public int TotalErrorsAmount { get; set; }
    public int TotalWarningsAmount { get; set; }

    public enum EType 
    {
        Error,
        Warning
    }

    public struct Data
    {
        public int Line;
        public int Column;

        public EType Type;

        public string Text;
    }

    public delegate void MessageDelegate( Data data );

    public event MessageDelegate Message;

    public ParserErrors()
    {
        Clear();
    }

    public void Clear()
    {
        TotalErrorsAmount = 0;
        TotalWarningsAmount = 0;
    }
	
    public void SyntaxError( int line, int col, int n )
    {
		string s;

		switch ( n )
		{
			case 0: s = "EOF expected"; break;
			case 1: s = "TNumber expected"; break;
			case 2: s = "TString expected"; break;
			case 3: s = "TNull expected"; break;
			case 4: s = "TTrue expected"; break;
			case 5: s = "TFalse expected"; break;
			case 6: s = "QuotationMark expected"; break;
			case 7: s = "CurlyBracketOpen expected"; break;
			case 8: s = "CurlyBracketClose expected"; break;
			case 9: s = "SquareBracketOpen expected"; break;
			case 10: s = "SquareBracketClose expected"; break;
			case 11: s = "Colon expected"; break;
			case 12: s = "Comma expected"; break;
			case 13: s = "Tab expected"; break;
			case 14: s = "??? expected"; break;
			case 15: s = "invalid Value"; break;

			default: s = "error " + n; break;
		}

		TotalErrorsAmount++;
        if ( Message != null )
        {
            Message( new Data { Line = line, Column = col, Type = EType.Error, Text = s } );
        }
	}

    public void SemanticError( int line, int col, string s )
    {
        TotalErrorsAmount++;
        if ( Message != null )
        {
            Message( new Data { Line = line, Column = col, Type = EType.Error, Text = s } );
        }
    }

    public void SemanticError( string s )
    {
        TotalErrorsAmount++;
        if ( Message != null )
        {
            Message( new Data { Type = EType.Error, Text = s } );
        }
    }

    public void Warning( int line, int col, string s )
    {
        TotalWarningsAmount++;
        if ( Message != null )
        {
            Message( new Data { Line = line, Column = col, Type = EType.Warning, Text = s } );
        }
    }

    public void Warning( string s )
    {
        TotalWarningsAmount++;
        if ( Message != null )
        {
            Message( new Data { Type = EType.Warning, Text = s } );
        }
    }
}

public class FatalError : Exception
{
	public FatalError( string m ):
		base( m )
	{
	}
}
}