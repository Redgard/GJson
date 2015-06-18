
using System;
using System.IO;

namespace GJson {



partial class Parser 
{
	public const int _EOF = 0;
	public const int _Number = 1;
	public const int _CString = 2;
	public const int _Null = 3;
	public const int _True = 4;
	public const int _False = 5;
	public const int _QuotationMark = 6;
	public const int _CurlyBracketOpen = 7;
	public const int _CurlyBracketClose = 8;
	public const int _SquareBracketOpen = 9;
	public const int _SquareBracketClose = 10;
	public const int _Colon = 11;
	public const int _Comma = 12;
	public const int _Tab = 13;
	public const int maxT = 14;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner)
	{
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n)
	{
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) 
	{
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get ()
	{
		for (;;)
		{
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n)
	{
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s)
	{
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) 
	{
		if (la.kind == n) Get();
		else 
		{
			SynErr(n);
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
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}
	
	void Json() {
		Value();
	}

	void Value() {
		switch (la.kind) {
		case 7: {
			Object();
			break;
		}
		case 9: {
			Array();
			break;
		}
		case 2: {
			String();
			break;
		}
		case 1: {
			Get();
			Push<Double>( t.val ); 
			break;
		}
		case 4: {
			Get();
			Push<Boolean>( t.val ); 
			break;
		}
		case 5: {
			Get();
			Push<Boolean>( t.val ); 
			break;
		}
		case 3: {
			Get();
			PushEmpty(); 
			break;
		}
		default: SynErr(15); break;
		}
	}

	void String() {
		Expect(2);
		Push<String>( t.val ); 
	}

	void Object() {
		PushEmpty(); 
		Expect(7);
		if (la.kind == 2) {
			ObjectList();
		}
		Expect(8);
	}

	void ObjectList() {
		ObjectItem();
		while (la.kind == 12) {
			Get();
			ObjectItem();
		}
	}

	void ObjectItem() {
		String();
		Expect(11);
		Value();
		AddItemToObject(); 
	}

	void Array() {
		PushEmpty(); 
		Expect(9);
		if (StartOf(1)) {
			ArrayList();
		}
		Expect(10);
	}

	void ArrayList() {
		ArrayItem();
		while (la.kind == 12) {
			Get();
			ArrayItem();
		}
	}

	void ArrayItem() {
		Value();
		AddItemToArray(); 
	}



	public void Parse()
	{
		la = new Token();
		la.val = "";		
		Get();
		Json();
		Expect(0);

	}
	
	static readonly bool[,] set = 
	{
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x}

	};
}

class Errors
{
    const string _kEerrMsgFormat = "-- line {0} col {1}: {2}";

    public int Count { get; private set; }
    public TextWriter ErrorStream { get; private set; }

    public Errors()
    {
        ErrorStream = new StringWriter();
        Count = 0;
    }

	public virtual void SynErr (int line, int col, int n)
	{
		string s;
		switch ( n )
		{
			case 0: s = "EOF expected"; break;
			case 1: s = "Number expected"; break;
			case 2: s = "CString expected"; break;
			case 3: s = "Null expected"; break;
			case 4: s = "True expected"; break;
			case 5: s = "False expected"; break;
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
		
        ErrorStream.WriteLine( _kEerrMsgFormat, line, col, s );
        Count++;
	}

    public virtual void SemErr( int line, int col, string s )
    {
        ErrorStream.WriteLine( _kEerrMsgFormat, line, col, s );
        Count++;
    }

    public virtual void SemErr( string s )
    {
        ErrorStream.WriteLine( s );
        Count++;
    }

    public virtual void Warning( int line, int col, string s )
    {
        ErrorStream.WriteLine( _kEerrMsgFormat, line, col, s );
    }

    public virtual void Warning( string s )
    {
        ErrorStream.WriteLine( s );
    }
}

class FatalError : Exception
{
	public FatalError( string m ):
		base( m )
	{
	}
}
}