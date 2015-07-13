#if UNITY_EDITOR
using TNRD.Editor.Core;
using UnityEngine;

namespace TNRD.Editor.Windows {

	[DocsDescription("A simple input dialog")]
	public class ExtendedInputDialog : ExtendedModalWindow {

		[DocsDescription("The input text in this dialog")]
		public string InputText = "";

		private string message = "";
		
		[DocsDescription("Creates a new instance of ExtendedInputDialog")]
		public ExtendedInputDialog() : base() { }

		[DocsDescription("Creates a new instance of ExtendedInputDialog")]
		[DocsParameter("title", "The title of the window")]
		[DocsParameter("message", "The message to show on the ExtendedDialogBox")]
		public ExtendedInputDialog( string title, string message ) : this() {
			this.Title = title;
			this.message = message;
		}

		[DocsDescription("Creates a new instance of ExtendedInputDialog")]
		[DocsParameter("title", "The title of the window")]
		[DocsParameter("message", "The message to show on the ExtendedDialogBox")]
		[DocsParameter("input", "The input to already show in the inputbox")]
		public ExtendedInputDialog( string title, string message, string input ) : this( title, message ) {
			this.InputText = input;
		}

		[DocsIgnore]
		protected override void OnInitialize() {
			base.OnInitialize();

			showOKButton = true;
			showCancelButton = true;

			IsDraggable = true;
			alignToCenter = true;

			WindowRect = new Rect( 0, 0, 350, 120 );
		}

		[DocsIgnore]
		public override void OnGUI( int id ) {
			base.OnGUI( id );

			GUI.Label( new Rect( 20, 35, WindowRect.width - 40, 20 ), message );

			var rect = new Rect( 20, 55, WindowRect.width - 40, 20 );
			InputText = GUI.TextField( rect, InputText );
		}
	}
}
#endif