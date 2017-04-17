
using System;
using System.Collections;

namespace GJson {

public class Token
{
	public int kind;    // token kind
	public int pos;     // token position in bytes in the source text (starting at 0)
	public int charPos;  // token position in characters in the source text (starting at 0)
	public int col;     // token column (starting at 1)
	public int line;    // token line (starting at 1)
	public string val;  // token value
	public Token next;  // ML 2005-03-11 Tokens are kept in linked list
}

public class Buffer
{
	public const int EOF = char.MaxValue + 1;
    readonly string _s;
    int _bufPos;

	public Buffer(string s)
	{
		_s = s;
		_bufPos = 0;
	}

	public int Read()
	{
		return (_bufPos < _s.Length) ? _s[_bufPos++] : EOF;
	}

	public int Peek()
	{
		int curPos = Pos;
		int ch = Read();
		Pos = curPos;
		return ch;
	}

	public int Pos
	{
		get { return _bufPos; }
		set
		{
			if (value < 0 || value >= _s.Length)
				throw new FatalError("buffer out of bounds access, position: " + value);

			_bufPos = value;
		}
	}
}
	
public class Scanner
{
	const char EOL = '\n';
	const int eofSym = 0; /* pdt */
	const int maxT = 14;
	const int noSym = 14;


	public Buffer buffer; // scanner buffer
	
	Token t;          // current token
	int ch;           // current input character
	int pos;          // byte position of current character
	int charPos;      // position by unicode characters starting with 0
	int col;          // column number of current character
	int line;         // line number of current character
	int oldEols;      // EOLs that appeared in a comment;
	static readonly Hashtable start; // maps first token character to start state

	Token tokens;     // list of tokens already peeked (first token is a dummy)
	Token pt;         // current peek token
	
	char[] tval = new char[128]; // text of current token
	int tlen;         // length of current token
	
	static Scanner() 
	{
		start = new Hashtable(128);
		for (int i = 49; i <= 57; ++i) start[i] = 7;
		start[48] = 2; 
		start[45] = 1; 
		start[34] = 34; 
		start[110] = 14; 
		start[116] = 18; 
		start[102] = 22; 
		start[123] = 27; 
		start[125] = 28; 
		start[91] = 29; 
		start[93] = 30; 
		start[58] = 31; 
		start[44] = 32; 
		start[9] = 33; 
		start[Buffer.EOF] = -1;

	}
	
	public Scanner(string inputString)
	{
		buffer = new Buffer(inputString);
		Init();
	}

    void Init()
	{
		pos = -1;
		line = 1;
		col = 0;
		charPos = -1;
		oldEols = 0;
		NextCh();
		pt = tokens = new Token(); // first token is a dummy
	}
	
	void NextCh()
	{
		if (oldEols > 0)
		{
			ch = EOL; oldEols--; 
		} 
		else
		{
			pos = buffer.Pos;
			ch = buffer.Read(); col++; charPos++;
			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; col = 0; }
		}

	}

	void AddCh()
	{
		if (tlen >= tval.Length)
		{
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != Buffer.EOF)
		{
			tval[tlen++] = (char) ch;
			NextCh();
		}
	}



	bool Comment0() {
		int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
		NextCh();
		if (ch == '/') {
			NextCh();
			for(;;) {
				if (ch == 10) {
					level--;
					if (level == 0) { oldEols = line - line0; NextCh(); return true; }
					NextCh();
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			buffer.Pos = pos0; NextCh(); line = line0; col = col0; charPos = charPos0;
		}
		return false;
	}

	bool Comment1() {
		int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
		NextCh();
		if (ch == '*') {
			NextCh();
			for(;;) {
				if (ch == '*') {
					NextCh();
					if (ch == '/') {
						level--;
						if (level == 0) { oldEols = line - line0; NextCh(); return true; }
						NextCh();
					}
				} else if (ch == '/') {
					NextCh();
					if (ch == '*') {
						level++; NextCh();
					}
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			buffer.Pos = pos0; NextCh(); line = line0; col = col0; charPos = charPos0;
		}
		return false;
	}


	void CheckLiteral()
	{
		switch (t.val) {
			default: break;
		}
	}

	Token NextToken() 
	{
		while (ch == ' ' ||
			ch >= 9 && ch <= 10 || ch == 13
		) NextCh();
		if (ch == '/' && Comment0() ||ch == '/' && Comment1()) return NextToken();
		int recKind = noSym;
		int recEnd = pos;
		t = new Token();
		t.pos = pos; t.col = col; t.line = line; t.charPos = charPos;
		int state;
		if (start.ContainsKey(ch)) { state = (int) start[ch]; }
		else { state = 0; }
		tlen = 0; AddCh();
		
		switch (state)
		{
			case -1: { t.kind = eofSym; break; } 
			case 0: 
			{
				if (recKind != noSym)
				{
					tlen = recEnd - t.pos;
					SetScannerBehindT();
				}
				t.kind = recKind; break;
			}
			case 1:
				if (ch >= '1' && ch <= '9') {AddCh(); goto case 7;}
				else if (ch == '0') {AddCh(); goto case 2;}
				else {goto case 0;}
			case 2:
				recEnd = pos; recKind = 1;
				if (ch == 'E' || ch == 'e') {AddCh(); goto case 3;}
				else if (ch == '.') {AddCh(); goto case 6;}
				else {t.kind = 1; break;}
			case 3:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 5;}
				else if (ch == '+' || ch == '-') {AddCh(); goto case 4;}
				else {goto case 0;}
			case 4:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 5;}
				else {goto case 0;}
			case 5:
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 5;}
				else {t.kind = 1; break;}
			case 6:
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 6;}
				else if (ch == 'E' || ch == 'e') {AddCh(); goto case 3;}
				else {t.kind = 1; break;}
			case 7:
				recEnd = pos; recKind = 1;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 7;}
				else if (ch == 'E' || ch == 'e') {AddCh(); goto case 3;}
				else if (ch == '.') {AddCh(); goto case 6;}
				else {t.kind = 1; break;}
			case 8:
				if (ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 65535) {AddCh(); goto case 8;}
				else if (ch == '"') {AddCh(); goto case 13;}
				else if (ch == 92) {AddCh(); goto case 35;}
				else {goto case 0;}
			case 9:
				if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 11;}
				else {goto case 0;}
			case 11:
				if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 12;}
				else {goto case 0;}
			case 12:
				if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 8;}
				else {goto case 0;}
			case 13:
				{t.kind = 2; break;}
			case 14:
				if (ch == 'u') {AddCh(); goto case 15;}
				else {goto case 0;}
			case 15:
				if (ch == 'l') {AddCh(); goto case 16;}
				else {goto case 0;}
			case 16:
				if (ch == 'l') {AddCh(); goto case 17;}
				else {goto case 0;}
			case 17:
				{t.kind = 3; break;}
			case 18:
				if (ch == 'r') {AddCh(); goto case 19;}
				else {goto case 0;}
			case 19:
				if (ch == 'u') {AddCh(); goto case 20;}
				else {goto case 0;}
			case 20:
				if (ch == 'e') {AddCh(); goto case 21;}
				else {goto case 0;}
			case 21:
				{t.kind = 4; break;}
			case 22:
				if (ch == 'a') {AddCh(); goto case 23;}
				else {goto case 0;}
			case 23:
				if (ch == 'l') {AddCh(); goto case 24;}
				else {goto case 0;}
			case 24:
				if (ch == 's') {AddCh(); goto case 25;}
				else {goto case 0;}
			case 25:
				if (ch == 'e') {AddCh(); goto case 26;}
				else {goto case 0;}
			case 26:
				{t.kind = 5; break;}
			case 27:
				{t.kind = 7; break;}
			case 28:
				{t.kind = 8; break;}
			case 29:
				{t.kind = 9; break;}
			case 30:
				{t.kind = 10; break;}
			case 31:
				{t.kind = 11; break;}
			case 32:
				{t.kind = 12; break;}
			case 33:
				{t.kind = 13; break;}
			case 34:
				recEnd = pos; recKind = 6;
				if (ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 65535) {AddCh(); goto case 8;}
				else if (ch == '"') {AddCh(); goto case 13;}
				else if (ch == 92) {AddCh(); goto case 35;}
				else {t.kind = 6; break;}
			case 35:
				if (ch == '"' || ch == '/' || ch == 92 || ch == 'b' || ch == 'f' || ch == 'n' || ch == 'r' || ch == 't') {AddCh(); goto case 8;}
				else if (ch == 'u') {AddCh(); goto case 9;}
				else {goto case 0;}

		}
		t.val = new string(tval, 0, tlen);
		return t;
	}

    void SetScannerBehindT() 
	{
		buffer.Pos = t.pos;
		NextCh();
		line = t.line; col = t.col; charPos = t.charPos;
		for (int i = 0; i < tlen; i++) NextCh();
	}
	
	public Token Scan () 
	{
		if (tokens.next == null)
		{
			return NextToken();
		}
		else 
		{
			pt = tokens = tokens.next;
			return tokens;
		}
	}

	public Token Peek () 
	{
		do 
		{
			if (pt.next == null) 
			{
				pt.next = NextToken();
			}
			pt = pt.next;
		}
		while (pt.kind > maxT);
	
		return pt;
	}

	public void ResetPeek () { pt = tokens; }
} 
}