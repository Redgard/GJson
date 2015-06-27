using System;
using System.Collections.Generic;

namespace GJson
{
	public class IdentWriter : DefaultWriter
    {
        int _ident;

		public bool Ident { set; get; }
		public bool SingleLineArray { set; get; }

		public string IdentString = StringConstants.Tab;

        public IdentWriter() :
            base()
        {
			Ident = true;
        }

		void WriteNewLine()
		{
			if ( Ident )
			{
				Write( Environment.NewLine );
				WriteIdent();
			}
		}

        void WriteIdent()
        {
			for ( int i = 0; i < _ident; ++i )
			{
				Write( IdentString );
			}
        }

		void IncreaseIdent()
		{
			_ident++;
		}

		void DecreaseIdent()
		{
			_ident--;
		}

		public override void WriteObject( JsonValue json )
        {
            if ( json.Count > 0 )
            {
				Write( StringConstants.CurlyBracketOpen );

				IncreaseIdent();

				WriteNewLine();

                bool firstD = true;

                foreach ( var jsonV in json.AsObject )
                {
                    if ( !firstD )
                    {
                        Write( StringConstants.Comma );
						WriteNewLine();
                    }

					WriteObjectPair( jsonV );
					
					firstD = false;
                }

				DecreaseIdent();

				WriteNewLine();
				
				Write( StringConstants.CurlyBracketClose );
            }
            else
            {
				Write( StringConstants.CurlyBracketOpen + StringConstants.CurlyBracketClose );
            }
        }

		void WriteObjectPair( KeyValuePair<string, JsonValue> pair )
		{
			Write( StringConstants.QuotationMark );
			Write( pair.Key );
			Write( StringConstants.QuotationMark + StringConstants.Space + StringConstants.Colon );

			if ( pair.Value.Count > 0 )
            {
				WriteNewLine();
            }

			pair.Value.Write( this );
		}

		public override void WriteArray( JsonValue json )
        {
            if ( json.Count > 0 )
            {
				Write( StringConstants.SquareBracketOpen );

				IncreaseIdent();

				WriteNewLine();

                bool firstA = true;

                foreach ( var jsonV in json.AsArray )
                {
                    if ( !firstA )
                    {
                        Write( StringConstants.Comma );

                        if ( !SingleLineArray )
                        {
							WriteNewLine();
                        }
                    }

					jsonV.Write( this );

                    firstA = false;
                }

				DecreaseIdent();
								
				WriteNewLine();
				
				Write( StringConstants.SquareBracketClose );
            }
            else
            {
				Write( StringConstants.SquareBracketOpen + StringConstants.SquareBracketClose );
            }
        }
	}
}
