using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GJsonCorrect
{
	public partial class GJsonCorrectForm : Form
	{
		const string _kTestFile = "test.json";

		GJsonCorrectFormController _controller;

		public GJsonCorrectForm()
		{
			InitializeComponent();

			_controller = new GJsonCorrectFormController( this );

			SetStatusText( String.Empty );

			InputBox.Text = GJsonTests.Tests.ReadFile( _kTestFile );
			InputBox.SelectionStart = Math.Max( 0, InputBox.Text.Length - 1 );
			InputBox.SelectionLength = 0;
		}

		public void SetStatusText( string txt )
		{
			StatusText.Text = txt;
		}

		private void InputBox_TextChanged( object sender, EventArgs e )
		{
			_controller.OnTextChanged();
		}
	}
}
