#if UNITY_EDITOR
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {
	/// <summary>
	/// Base for controls that can be added to ExtendedWindows
	/// </summary>
	public class ExtendedControl {

		[JsonIgnore]
		public ExtendedWindow Window;

		[JsonIgnore]
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

		/// <summary>
		/// Called when the control is added to a window
		/// </summary>
		public virtual void OnInitialize() {
			IsInitialized = true;

			var t = GetType();
			controlHint = t.Name.GetHashCode();
		}

		/// <summary>
		/// Called the first time OnGUI is called on this control
		/// </summary>
		public virtual void OnInitializeGUI() {
			initializedGUI = true;
		}
		
		public virtual void OnDeserialized() {
			if ( controlHint == -1 ) {
				var t = GetType();
				controlHint = t.Name.GetHashCode();
			}
		}

		/// <summary>
		/// Called when the Window gets closed
		/// </summary>
		public virtual void OnDestroy() {
			IsInitialized = false;
		}

		/// <summary>
		/// Called 100 times per second
		/// </summary>
		public virtual void Update( bool windowHasFocus ) { }

		public virtual void OnGUI() {
			if ( !initializedGUI ) {
				OnInitializeGUI();
			}
		}

		public virtual void OnSceneGUI( SceneView view ) { }

		/// <summary>
		/// Get a unique ID for a control
		/// </summary>
		public int GetControlID( FocusType focus ) {
			return GUIUtility.GetControlID( controlHint, focus );
		}

		#region Events
		public virtual void OnContextClick( Vector2 position, ref bool used ) { }
		public virtual void OnDragExited() { }
		public virtual void OnDragPerform( string[] paths, Vector2 position ) { }
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
		#endregion
	}
}
#endif