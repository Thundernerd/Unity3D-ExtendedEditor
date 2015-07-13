#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {
	[DocsDescription("Base class for modal windows that can be added to ExtendedWindows")]
	public class ExtendedModalWindow {

		[DocsDescription("The editor this modal window is shown in")]
		public ExtendedEditor Editor;

		[DocsDescription("The result of a closed modal window")]
		public EExtendedModalWindowResult Result { get; private set; }

		[DocsDescription("Is the modal window ready to close")]
		public bool IsDone { get; private set; }

		[DocsDescription("Can the modal window be dragged around")]
		public bool IsDraggable { get; protected set; }

		[DocsDescription("The title of the modal window")]
		public string Title = "";

		[DocsDescription("The rectangle used to draw the modal window")]
		public Rect WindowRect = new Rect();

		[DocsDescription("The input manager")]
		public ExtendedInput Input { get { return Editor.Input; } }
		
		[DocsDescription("Should the modal window be aligned to the center of the editor")]
        protected bool alignToCenter = true;
		
		[DocsDescription("Should the modal window show the OK/Accept button")]
		protected bool showOKButton = false;

		[DocsDescription("Should the modal window show the Cancel/Close button")]
		protected bool showCancelButton = false;
		
		[DocsDescription("The text to show on the OK/Accept button")]
		protected string textOKButton = "OK";
		
		[DocsDescription("The text to show on the Cancel/Close button")]
		protected string textCancelButton = "Cancel";

		private bool isInitialized = false;

		[DocsDescription("Creates an instance of ExtendedModalWindow")]
		public ExtendedModalWindow() { }
		
		[DocsDescription("Called the first time OnGUI is called on this modal window")]
		protected virtual void OnInitialize() {
			isInitialized = true;
		}
		
		[DocsDescription("Called when the modal window gets closed")]
		protected virtual void OnDestroy() {
			isInitialized = false;
		}
		
		[DocsDescription("Called 100 times per second")]
		[DocsParameter("windowHasFocus", "Does this window have focus")]
		public virtual void Update( bool windowHasFocus ) { }
		
		[DocsDescription("Write your own GUI logic here")]
		[DocsParameter("id", "-")]
		public virtual void OnGUI( int id ) {
			if ( !isInitialized ) {
				OnInitialize();

				if ( alignToCenter ) {
					AlignToCenter();
				}
			}

			if ( IsDraggable ) {
				GUI.DragWindow( new Rect( 0, 0, WindowRect.width, 17.5f ) );
			} else {
				if ( alignToCenter ) {
					AlignToCenter();
				}
			}

			if ( showOKButton ) {
				if ( Input.KeyPressed( KeyCode.KeypadEnter ) || Input.KeyPressed( KeyCode.Return ) ) {
					Event.current.Use();
					Result = EExtendedModalWindowResult.OK;
					IsDone = true;
				}
			}
			if ( showCancelButton ) {
				if ( Input.KeyPressed( KeyCode.Escape ) ) {
					Event.current.Use();
					Result = EExtendedModalWindowResult.Cancel;
					IsDone = true;
				}
			}

			if ( showOKButton && showCancelButton ) {
				if ( GUI.Button( new Rect( WindowRect.width - 180, WindowRect.height - 30, 80, 20 ), textOKButton ) ) {
					OK();
				}

				if ( GUI.Button( new Rect( WindowRect.width - 90, WindowRect.height - 30, 80, 20 ), textCancelButton ) ) {
					Cancel();
				}
			} else if ( showOKButton || showCancelButton ) {
				var rect = new Rect( WindowRect.width - 90, WindowRect.height - 30, 80, 20 );

				if ( showOKButton ) {
					if ( GUI.Button( rect, textOKButton ) ) {
						OK();
					}
				}

				if ( showCancelButton ) {
					if ( GUI.Button( rect, textCancelButton ) ) {
						Cancel();
					}
				}
			}
		}
		
		[DocsDescription("Closes the modal window with the OK result")]
		public void OK() {
			Result = EExtendedModalWindowResult.OK;
			IsDone = true;
			Event.current.Use();
		}
		
		[DocsDescription("Closes the modal window with the Cancel result")]
		public void Cancel() {
			Result = EExtendedModalWindowResult.Cancel;
			IsDone = true;
			Event.current.Use();
		}
		
		[DocsDescription("Aligns the modal window to the center of the editor")]
		public void AlignToCenter() {
			var esize = Editor.position.size / 2;
			var csize = WindowRect.size / 2;
			WindowRect.position = esize - csize;
		}

		#region Events
		[DocsDescription("Invoked when a ContextClick event occurs")]
		public virtual void OnContextClick( Vector2 position ) { }
		[DocsDescription("Invoked when a DragExited event occurs")]
		public virtual void OnDragExited() { }
		[DocsDescription("Invoked when a DragPerform event occurs")]
		public virtual void OnDragPerform( string[] paths, Vector2 position ) { }
		[DocsDescription("Invoked when a DragUpdate event occurs")]
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
		#endregion
	}
}
#endif