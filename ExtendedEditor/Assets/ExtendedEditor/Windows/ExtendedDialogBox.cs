#if UNITY_EDITOR
using TNRD.Editor.Core;
using UnityEngine;

namespace TNRD.Editor.Windows {

	[DocsDescription("A simple dialog box")]
	public class ExtendedDialogBox : ExtendedModalWindow {

		private string message = "";
		private Vector2 messageSize = Vector2.zero;
		private Rect messageRect = new Rect();
		private GUIStyle messageStyle;

		[DocsDescription("Creates a new instance of ExtendedDialogBox")]
		public ExtendedDialogBox() : base() { }

		[DocsDescription("Creates a new instance of ExtendedDialogBox")]
		[DocsParameter("title","The title of the window")]
		[DocsParameter("message","The message to show on the ExtendedDialogBox")]
		public ExtendedDialogBox( string title, string message ) : this() {
			this.Title = title;
			this.message = message;
		}

		[DocsDescription("Creates a new instance of ExtendedDialogBox")]
		[DocsParameter("title", "The title of the window")]
		[DocsParameter("message", "The message to show on the ExtendedDialogBox")]
		[DocsParameter("okButton", "The text for the OK Button")]
		public ExtendedDialogBox( string title, string message, string okButton ) : this( title, message ) {
			textOKButton = okButton;
		}

		[DocsDescription("Creates a new instance of ExtendedDialogBox")]
		[DocsParameter("title", "The title of the window")]
		[DocsParameter("message", "The message to show on the ExtendedDialogBox")]
		[DocsParameter("okButton", "The text for the OK Button")]
		[DocsParameter("cancelButton", "The text for the Cancel Button")]
		public ExtendedDialogBox( string title, string message, string okButton, string cancelButton ) : this( title, message, okButton ) {
			textCancelButton = cancelButton;
			showCancelButton = true;
		}

		[DocsIgnore]
        protected override void OnInitialize() {
			base.OnInitialize();

			alignToCenter = true;
			IsDraggable = true;
			showOKButton = true;

			WindowRect = new Rect( 0, 0, 350, 120 );

			messageStyle = new GUIStyle( GUI.skin.label );
			messageStyle.wordWrap = true;

			messageSize = messageStyle.CalcSize( new GUIContent( message ) );
			if ( messageSize.x > 350 ) {
				var height = messageStyle.CalcHeight( new GUIContent( message ), 330 );
				messageRect = new Rect(
					10, WindowRect.height / 2 - height / 2,
					330, height );
				messageStyle.alignment = TextAnchor.MiddleCenter;
			} else {
				messageRect = new Rect(
				WindowRect.width / 2 - messageSize.x / 2,
				WindowRect.height / 2 - messageSize.y / 2,
				messageSize.x, messageSize.y );
			}
		}

		[DocsIgnore]
		public override void OnGUI( int id ) {
			base.OnGUI( id );
			GUI.Label( messageRect, message, messageStyle );
		}
	}
}
#endif