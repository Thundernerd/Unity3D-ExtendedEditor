#if UNITY_EDITOR
using UnityEngine;
using TNRD.Json;

namespace TNRD {
	public class ExtendedControl {

		public ExtendedWindow Window;
		public ExtendedInput Input { get { return Window.Input; } }

		public bool IsInitialized;

		public Vector2 Position;
		public Vector2 Size;
		[JsonIgnore]
		public Rect Rectangle {
			get {
				var scaledPosition = Window.ScaleMatrix.MultiplyVector( Position + (Vector2)Window.Camera );
				var scaledSize = Window.ScaleMatrix.MultiplyVector( Size );
				return new Rect( scaledPosition.x, scaledPosition.y, scaledSize.x, scaledSize.y );
			}
		}

		[JsonProperty]
		private int controlHint = -1;

		[JsonIgnore]
		private bool initializedGUI = false;

		public ExtendedControl() { }

		public virtual void OnInitialize() {
			IsInitialized = true;

			var t = GetType();
			controlHint = t.Name.GetHashCode();
		}

		public virtual void OnInitializeGUI() {
			initializedGUI = true;
		}

		public virtual void OnDeserialized() {
			if ( controlHint == -1 ) {
				var t = GetType();
				controlHint = t.Name.GetHashCode();
			}
		}

		public virtual void OnDestroy() {
			IsInitialized = false;
		}

		public virtual void Update( bool hasFocus ) { }
		public virtual void OnGUI() {
			if ( !initializedGUI ) {
				OnInitializeGUI();
			}
		}

		public int GetControlID( FocusType focus ) {
			return GUIUtility.GetControlID( controlHint, focus );
		}

		#region Events
		public virtual void OnContextClick( Vector2 position, ref bool used ) { }
		public virtual void OnDragExited() { }
		public virtual void OnDragPerform( string[] paths, Vector2 position ) { }
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
		public virtual void OnScrollWheel( Vector2 delta ) { }
		#endregion
	}
}
#endif