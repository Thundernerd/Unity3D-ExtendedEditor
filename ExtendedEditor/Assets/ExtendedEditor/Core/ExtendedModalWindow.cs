#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {
	public class ExtendedModalWindow {

		public ExtendedEditor Editor;

		public EExtendedModalWindowResult Result { get; private set; }
		public bool IsDone { get; private set; }
		public bool IsDraggable { get; protected set; }

		public string Title = "";
		public Rect WindowRect = new Rect();

		public ExtendedInput Input { get { return Editor.Input; } }

		protected bool alignToCenter = true;
		protected bool showOKButton = false;
		protected bool showCancelButton = false;
		protected string textOKButton = "OK";
		protected string textCancelButton = "Cancel";

		private bool isInitialized = false;

		public ExtendedModalWindow() { }

		protected virtual void Initialize() {
			isInitialized = true;
		}

		protected virtual void Destroy() {
			isInitialized = false;
		}

		public virtual void Update( bool hasFocus ) { }

		public virtual void OnGUI( int id ) {
			if ( !isInitialized ) {
				Initialize();

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

		public void OK() {
			Result = EExtendedModalWindowResult.OK;
			IsDone = true;
			Event.current.Use();
		}

		public void Cancel() {
			Result = EExtendedModalWindowResult.Cancel;
			IsDone = true;
			Event.current.Use();
		}

		private void AlignToCenter() {
			var esize = Editor.position.size / 2;
			var csize = WindowRect.size / 2;
			WindowRect.position = esize - csize;
		}

		#region Events
		public virtual void OnContextClick( Vector2 position ) { }
		public virtual void OnDoubleClick( EMouseButton button, Vector2 position ) { }
		public virtual void OnDragExited() { }
		public virtual void OnDragPerform( string[] paths, Vector2 position ) { }
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
		public virtual void OnScrollWheel( Vector2 delta ) { }
		#endregion
	}
}
#endif