
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
		Json = 0,
		Value = 1,
		String = 2,
		Number = 3,
		True = 4,
		False = 5,
		Null = 6,
		Object = 7,
		ObjectList = 8,
		ObjectItem = 9,
		Key = 10,
		Array = 11,
		ArrayList = 12,
		ArrayItem = 13
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
		Key();
		Expect(ETerminal.Colon);
		Value();
		ProductionEnd( ENonTerminal.ObjectItem );
	}

	void Key() {
		ProductionBegin( ENonTerminal.Key );
		String();
		ProductionEnd( ENonTerminal.Key );
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
        SyntaxError,
		SemanticError,
        Warning
    }
	
	public enum ESyntaxErrorType 
    {
		Unknown,
        TokenExpected,
		UnknownTokenExpected,
        InvalidToken
    }

    public struct Data
    {
        public int Line;
        public int Column;

        public EType Type;

        public string Text;
		
		public ESyntaxErrorType SyntaxErrorType;
		public Parser.ETerminal? SyntaxErrorTerminal;
		public Parser.ENonTerminal? SyntaxErrorNonTerminal;
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
		//string s = "";
		
		ESyntaxErrorType syntaxErrorType = ESyntaxErrorType.Unknown;
		Parser.ETerminal? syntaxErrorTerminal = null;
		Parser.ENonTerminal? syntaxErrorNonTerminal = null;
		
		switch(n)
		{
			case 0:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.EOF;break;
			case 1:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.TNumber;break;
			case 2:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.TString;break;
			case 3:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.TNull;break;
			case 4:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.TTrue;break;
			case 5:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.TFalse;break;
			case 6:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.QuotationMark;break;
			case 7:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.CurlyBracketOpen;break;
			case 8:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.CurlyBracketClose;break;
			case 9:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.SquareBracketOpen;break;
			case 10:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.SquareBracketClose;break;
			case 11:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.Colon;break;
			case 12:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.Comma;break;
			case 13:syntaxErrorType = ESyntaxErrorType.TokenExpected;syntaxErrorTerminal = Parser.ETerminal.Tab;break;
			case 14:syntaxErrorType = ESyntaxErrorType.UnknownTokenExpected;break;
			case 15:syntaxErrorType = ESyntaxErrorType.InvalidToken;syntaxErrorNonTerminal = Parser.ENonTerminal.Value;break;

		}
		
		TotalErrorsAmount++;

        if ( Message != null )
        {
            Message( new Data 
			{
				Line = line, 
				Column = col, 
				Type = EType.SyntaxError, 
				SyntaxErrorType = syntaxErrorType,
				SyntaxErrorTerminal = syntaxErrorTerminal,
				SyntaxErrorNonTerminal = syntaxErrorNonTerminal 
			} );
        }
	}

    public void SemanticError( int line, int col, string s )
    {
        TotalErrorsAmount++;

        if ( Message != null )
        {
            Message( new Data { Line = line, Column = col, Type = EType.SemanticError, Text = s } );
        }
    }

    public void SemanticError( string s )
    {
        TotalErrorsAmount++;

        if ( Message != null )
        {
            Message( new Data { Type = EType.SemanticError, Text = s } );
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