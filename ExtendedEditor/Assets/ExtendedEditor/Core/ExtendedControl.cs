#if UNITY_EDITOR
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("Base class for controls that can be added to ExtendedWindows")]
	public class ExtendedControl {

		[JsonIgnore]
		[DocsDescription("The window this control is added to")]
		public ExtendedWindow Window;

		[JsonIgnore]
		[DocsDescription("The input handler belonging to this control's window")]
		public ExtendedInput Input { get { return Window.Input; } }

		[DocsDescription("Is the control initialized")]
		public bool IsInitialized;

		[DocsDescription("The position of the control in the window")]
		public Vector2 Position;

		[DocsDescription("The size of the control")]
		public Vector2 Size;

		[JsonIgnore]
		[DocsDescription("The rectangle used for drawing in OnGUI")]
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

		[DocsDescription("Creates a new instance of ExtendedControl")]
		public ExtendedControl() { }

		[DocsDescription("Called when the control is added to a window")]
		public virtual void OnInitialize() {
			IsInitialized = true;

			var t = GetType();
			controlHint = t.Name.GetHashCode();
		}

		[DocsDescription("Called the first time OnGUI is called on this control")]
		public virtual void OnInitializeGUI() {
			initializedGUI = true;
		}
		
		[DocsDescription("Called when a window gets deserialized")]
		public virtual void OnDeserialized() {
			if ( controlHint == -1 ) {
				var t = GetType();
				controlHint = t.Name.GetHashCode();
			}
		}

		[DocsDescription("Called when the Window this control is located in gets closed")]
		public virtual void OnDestroy() {
			IsInitialized = false;
		}

		[DocsDescription("Called 100 times per second")]
		[DocsParameter("windowHasFocus", "Does this window have focus")]
		public virtual void Update( bool windowHasFocus ) { }

		[DocsDescription("Implement your own GUI logic here")]
		public virtual void OnGUI() {
			if ( !initializedGUI ) {
				OnInitializeGUI();
			}
		}

		[DocsDescription("Implement your own SceneGUI logic here")]
		[DocsParameter("view", "The current SceneView")]
		public virtual void OnSceneGUI( SceneView view ) { }

		[DocsDescription("Gets a unique ID for a control")]
		public int GetControlID( FocusType focus ) {
			return GUIUtility.GetControlID( controlHint, focus );
		}

		#region Events
		[DocsDescription("Invoked when a ContextClick event occurs")]
		[DocsParameter("position", "The location of the right-mouse click")]
		[DocsParameter("used", "-")]
		public virtual void OnContextClick( Vector2 position, ref bool used ) { }

		[DocsDescription("Invoked when a DragExited event occurs")]
		public virtual void OnDragExited() { }

		[DocsDescription("Invoked when a DragPerform event occurs")]
		[DocsParameter("paths", "Path(s) of the file(s) being dragged onto the editor")]
		[DocsParameter("position", "The mouse position")]
		public virtual void OnDragPerform( string[] paths, Vector2 position ) { }

		[DocsDescription("Invoked when a DragUpdate event occurs")]
		[DocsParameter("paths", "Path(s) of the file(s) being dragged onto the editor")]
		[DocsParameter("position", "The mouse position")]
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
		#endregion
	}
}
#endif