
using System;
using System.IO;

namespace GJson {



partial class Parser 
{
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _string = 2;
	public const int maxT = 12;

	const bool T = true;
	const bool x = false;
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
		PushEmpty(); 
		switch (la.kind) {
		case 3: {
			Object();
			break;
		}
		case 7: {
			Array();
			break;
		}
		case 2: {
			String();
			break;
		}
		case 1: {
			Get();
			Replace<double>( t.val ); 
			break;
		}
		case 9: {
			Get();
			Replace<bool>( t.val ); 
			break;
		}
		case 10: {
			Get();
			Replace<bool>( t.val ); 
			break;
		}
		case 11: {
			Get();
			ReplaceEmpty(); 
			break;
		}
		default: SynErr(13); break;
		}
	}

	void String() {
		Expect(2);
		Replace<string>( t.val ); 
	}

	void Object() {
		ReplaceObject(); 
		Expect(3);
		if (la.kind == 2) {
			ItemList();
		}
		Expect(4);
	}

	void ItemList() {
		Item();
		while (la.kind == 5) {
			Get();
			Item();
		}
	}

	void Item() {
		PushEmpty(); 
		String();
		Expect(6);
		Value();
		AddItemToObject(); 
	}

	void Array() {
		ReplaceArray(); 
		Expect(7);
		if (StartOf(1)) {
			ValueList();
		}
		Expect(8);
	}

	void ValueList() {
		Value();
		AddItemToArray(); 
		while (la.kind == 5) {
			Get();
			Value();
			AddItemToArray(); 
		}
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
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, x,x,x,T, x,T,T,T, x,x}

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
			case 1: s = "number expected"; break;
			case 2: s = "string expected"; break;
			case 3: s = "\"{\" expected"; break;
			case 4: s = "\"}\" expected"; break;
			case 5: s = "\",\" expected"; break;
			case 6: s = "\":\" expected"; break;
			case 7: s = "\"[\" expected"; break;
			case 8: s = "\"]\" expected"; break;
			case 9: s = "\"true\" expected"; break;
			case 10: s = "\"false\" expected"; break;
			case 11: s = "\"null\" expected"; break;
			case 12: s = "??? expected"; break;
			case 13: s = "invalid Value"; break;

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