#if UNITY_EDITOR
using UnityEngine;
namespace TNRD {
	public class ExtendedInputDialog : ExtendedModalWindow {

		public string InputText = "";

		private string message = "";

		public ExtendedInputDialog() : base() { }

		public ExtendedInputDialog( string title, string message ) : this() {
			this.Title = title;
			this.message = message;
		}

		public ExtendedInputDialog( string title, string message, string input ) : this( title, message ) {
			this.InputText = input;
		}
		
		protected override void Initialize() {
			base.Initialize();

			showOKButton = true;
			showCancelButton = true;

			IsDraggable = true;
			alignToCenter = true;

			WindowRect = new Rect( 0, 0, 350, 120 );
		}

		public override void OnGUI( int id ) {
			base.OnGUI( id );

			GUI.Label( new Rect( 20, 35, WindowRect.width - 40, 20 ), message );

			var rect = new Rect( 20, 55, WindowRect.width - 40, 20 );
			InputText = GUI.TextField( rect, InputText );
		}
	}
}
#endif