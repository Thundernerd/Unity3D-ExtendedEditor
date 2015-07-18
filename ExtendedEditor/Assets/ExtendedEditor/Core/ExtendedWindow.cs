#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("Base class for windows that can be added to ExtendedEditor")]
	public class ExtendedWindow {

		[JsonProperty]
		[DocsDescription("The asset manager for this window")]
		public ExtendedAssets Assets;

		[JsonIgnore]
		[DocsDescription("The editor this window is added to")]
		public ExtendedEditor Editor;

		[JsonProperty]
		[DocsDescription("The settings that apply to this window")]
		public ExtendedWindowSettings Settings;

		[JsonProperty]
		[DocsDescription("Is the window initialized")]
		public bool IsInitialized = false;

		[JsonProperty]
		[DocsDescription("The content to draw on the top of the window")]
		public GUIContent WindowContent = new GUIContent();

		[JsonProperty]
		[DocsDescription("The window ID")]
		public int WindowID = -1;

		[JsonProperty]
		[DocsDescription("The rectangle used for drawing the window")]
		public Rect WindowRect = new Rect();

		[JsonProperty]
		[DocsDescription("The GUIStyle for the window")]
		public GUIStyle WindowStyle = null;

		[JsonIgnore]
		[DocsDescription("The position of the window inside the editor")]
		public Vector2 Position {
			get {
				return WindowRect.position;
			}
			set {
				Settings.IsFullscreen = false;
				WindowRect.position = value;
			}
		}

		[JsonIgnore]
		[DocsDescription("The size of the window")]
		public Vector2 Size {
			get {
				return WindowRect.size;
			}
			set {
				Settings.IsFullscreen = false;
				WindowRect.size = value;
			}
		}

		[JsonIgnore]
		[DocsDescription("The camera that is used for panning in the window")]
		public Vector3 Camera = new Vector3( 0, 0, 1 );

		[JsonIgnore]
		[DocsDescription("The matrix that is used for scaling the contents of this window")]
		public Matrix4x4 ScaleMatrix = Matrix4x4.identity;

		[JsonIgnore]
		[DocsDescription("The input manager for this window")]
		public ExtendedInput Input = new ExtendedInput();

		[JsonProperty]
		[DocsDescription("The active controls in this window")]
		protected List<ExtendedControl> Controls = new List<ExtendedControl>();

		private List<ExtendedControl> controlsToProcess = new List<ExtendedControl>();

		private List<ExtendedControl> controlsToRemove = new List<ExtendedControl>();

		private Dictionary<Type, List<ExtendedControl>> controlsDict = new Dictionary<Type, List<ExtendedControl>>();

		private Rect nonFullScreenRect;

		private bool initializedGUI = false;

		private const int cameraSpeed = 500;

		private List<ExtendedNotification> notifications = new List<ExtendedNotification>();

		private GUIStyle notificationBackgroundStyle;

		private GUIStyle notificationTextStyle;

		private GUIStyle closeButtonStyle;

		private GUIStyle maximizeButtonStyle;

		[DocsDescription("...")]
		public ExtendedWindow() : this( new ExtendedWindowSettings() ) { }

		[DocsDescription("...")]
		public ExtendedWindow( ExtendedWindowSettings settings ) {
			Settings = settings;
		}

		#region Initialization

		[DocsDescription("Called when the window is added to an editor")]
		public virtual void OnInitialize() {
			Assets = new ExtendedAssets( Settings.AssetPath, this );

			if ( Settings.UseOnSceneGUI ) {
				SceneView.onSceneGUIDelegate += InternalSceneGUI;
			}

			WindowRect = new Rect( 0, 0, Editor.position.size.x, Editor.position.size.y );
			IsInitialized = true;
		}

		[DocsDescription("Called the first time OnGUI is called on this window")]
		protected virtual void OnInitializeGUI() {
			notificationBackgroundStyle = new GUIStyle( "NotificationBackground" );
			notificationTextStyle = new GUIStyle( "NotificationText" );
			notificationTextStyle.padding = new RectOffset( 20, 20, 20, 20 );
			notificationTextStyle.fontSize = 17;

			if ( WindowStyle == null ) {
				WindowStyle = new GUIStyle( GUI.skin.window );
				WindowStyle.normal.background = Editor.SharedAssets["BackgroundNormal"];
				WindowStyle.onNormal.background = Editor.SharedAssets["BackgroundActive"];
			}

			closeButtonStyle = new GUIStyle();
			closeButtonStyle.normal.background = Editor.SharedAssets["CloseNormal"];
			closeButtonStyle.hover.background = Editor.SharedAssets["CloseActive"];

			maximizeButtonStyle = new GUIStyle();
			maximizeButtonStyle.normal.background = Editor.SharedAssets["MaximizeNormal"];
			maximizeButtonStyle.hover.background = Editor.SharedAssets["MaximizeActive"];

			initializedGUI = true;
		}

		[DocsDescription("Called when this window gets deserialized")]
		public virtual void OnDeserialized() {
			Assets = new ExtendedAssets( Settings.AssetPath, this );

			for ( int i = 0; i < Controls.Count; i++ ) {
				if ( Controls[i] != null ) {
					Controls[i].Window = this;
					Controls[i].OnDeserialized();
				}
			}

			RemoveBrokenControls();
		}

		private void RemoveBrokenControls() {
			int removed = 0;
			for ( int i = Controls.Count - 1; i >= 0; i-- ) {
				if ( Controls[i] == null ) {
					Controls.RemoveAt( i );
					removed++;
				}
			}

			foreach ( var item in controlsDict ) {
				for ( int i = item.Value.Count - 1; i >= 0; i-- ) {
					if ( item.Value[i] == null ) {
						item.Value.RemoveAt( i );
					}
				}
			}

			if ( removed > 0 ) {
				Debug.LogErrorFormat( "Removed {0} \"NULL\" control(s); Check your editor!", removed );
			}
		}

		[DocsDescription("Called when this window or the editor gets closed")]
		public virtual void OnDestroy() {
			for ( int i = Controls.Count - 1; i >= 0; i-- ) {
				Controls[i].OnDestroy();
			}

			if ( Settings.UseOnSceneGUI ) {
				SceneView.onSceneGUIDelegate -= InternalSceneGUI;
			}

			Assets.Destroy( this );



			IsInitialized = false;
		}
		#endregion

		[DocsDescription("Called when this window gets focus")]
		public virtual void OnFocus() { }

		[DocsDescription("Called when this window loses focus")]
		public virtual void OnLostFocus() { }

		[DocsDescription("Called 100 times per second")]
		[DocsParameter("windowHasFocus", "Does this window have focus")]
		public virtual void Update( bool windowHasFocus ) {
			controlsToProcess = new List<ExtendedControl>( Controls );

			if ( Settings.IsFullscreen ) {
				var currentEditorSize = Editor.position.size;
				if ( WindowRect.size != currentEditorSize ) {
					WindowRect.size = currentEditorSize;
				}

				if ( WindowRect.position.x != 0 && WindowRect.position.y != 0 ) {
					WindowRect.position = new Vector2( 0, 0 );
				}
			} else {
				nonFullScreenRect = WindowRect;
			}

			foreach ( var item in controlsToProcess ) {
				item.Update( windowHasFocus );
			}

			for ( int i = notifications.Count - 1; i >= 0; i-- ) {
				var item = notifications[i];
				if ( item.Duration > 0 && item.Color.a < 1 ) {
					item.Color.a += Editor.DeltaTime * 5;
				} else if ( item.Duration > 0 && item.Color.a >= 1 ) {
					item.Duration -= Editor.DeltaTime;
				} else if ( item.Duration <= 0 && item.Color.a > 0 ) {
					item.Color.a -= Editor.DeltaTime * 5;
				} else if ( item.Duration <= 0 && item.Color.a <= 0 ) {
					notifications.RemoveAt( i );
				}
			}

			if ( windowHasFocus ) {
				if ( Settings.UseCamera ) {
					if ( Input.KeyDown( KeyCode.LeftAlt ) || Input.KeyDown( KeyCode.RightAlt ) ) {
						if ( Input.ButtonDown( EMouseButton.Left ) ) {
							Camera += ScaleMatrix.inverse.MultiplyVector( Input.MouseDelta );
						} else if ( Input.ButtonDown( EMouseButton.Right ) ) {
							var delta = Input.MouseDelta / 1000f;
							Camera.z += delta.x;
							Camera.z -= delta.y;

							if ( Camera.z < 0.1f ) {
								Camera.z = 0.1f;
							}
						}
					}

					if ( Input.Type == EventType.MouseDrag && Input.ButtonDown( EMouseButton.Middle ) ) {
						Camera += ScaleMatrix.inverse.MultiplyVector( Input.MouseDelta );
					}

					if ( Input.KeyDown( KeyCode.LeftArrow ) ) {
						Camera.x += ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
					}
					if ( Input.KeyDown( KeyCode.RightArrow ) ) {
						Camera.x -= ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
					}
					if ( Input.KeyDown( KeyCode.UpArrow ) ) {
						Camera.y += ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
					}
					if ( Input.KeyDown( KeyCode.DownArrow ) ) {
						Camera.y -= ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
					}
				}
			}

			if ( controlsToRemove.Count > 0 ) {
				foreach ( var control in controlsToRemove ) {
					if ( control.IsInitialized ) {
						control.OnDestroy();
					}

					controlsDict[control.GetType()].Remove( control );
					Controls.Remove( control );
				}
			}

			Input.Update();
		}

		#region SceneGUI
		[DocsIgnore]
		public void InternalSceneGUI( SceneView view ) {
			Handles.BeginGUI();
			OnSceneGUI( view );
			Handles.EndGUI();
		}

		/// <summary>
		/// See <see cref="ExtendedWindowSettings.UseOnSceneGUI"/> to enable this feature
		/// </summary>
		/// <param name="view"></param>
		/// 
		[DocsDescription("Write your own SceneGUI logic here")]
		[DocsParameter("view", "The current SceneView")]
		public virtual void OnSceneGUI( SceneView view ) {
			foreach ( var item in Controls ) {
				item.OnSceneGUI( view );
			}
		}
		#endregion

		#region GUI
		[DocsIgnore]
		public void InternalGUI( int id ) {
			if ( !initializedGUI ) {
				OnInitializeGUI();
			}

			var e = Editor.CurrentEvent;
			Input.OnGUI( e );

			if ( WindowRect.Contains( e.mousePosition ) ) {
				switch ( e.type ) {
					case EventType.ContextClick:
						OnContextClick( e.mousePosition );
						break;
					case EventType.DragPerform:
						OnDragPerform( DragAndDrop.paths, e.mousePosition );
						break;
					case EventType.DragUpdated:
						OnDragUpdate( DragAndDrop.paths, e.mousePosition );
						break;
				}
			}

			switch ( e.type ) {
				case EventType.DragExited:
					OnDragExited();
					break;
			}

			BeginGUI();
			OnGUI();
			EndGUI();
		}

		[DocsIgnore]
		public void BeginGUI() {
			if ( Settings.UseCamera ) {
				var translation = Matrix4x4.TRS( WindowRect.TopLeft(), Quaternion.identity, Vector3.one );
				var scale = Matrix4x4.Scale( new Vector3( Camera.z, Camera.z, 1f ) );
				ScaleMatrix = translation * scale * translation.inverse * GUI.matrix;

				if ( Input.KeyDown( KeyCode.LeftAlt ) || Input.KeyDown( KeyCode.RightAlt ) ) {
					if ( Input.ButtonDown( EMouseButton.Right ) ) {
						EditorGUIUtility.AddCursorRect( WindowRect, MouseCursor.Zoom );
					} else {
						EditorGUIUtility.AddCursorRect( WindowRect, MouseCursor.Pan );
					}
				} else if ( Input.ButtonDown( EMouseButton.Middle ) ) {
					EditorGUIUtility.AddCursorRect( WindowRect, MouseCursor.Pan );
				} else {
					EditorGUIUtility.AddCursorRect( WindowRect, MouseCursor.Arrow );
				}
			}

			Rect area = WindowRect;
			area.position = new Vector2( 0, 0 );

			var mousePosition = Input.MousePosition;
			if ( WindowStyle != null && WindowStyle.name == "window" ) {
				area.y += 17.5f;
				area.height -= 17.5f;
				mousePosition.y -= 17.5f;
			}
			if ( Settings.DrawToolbar ) {
				area.y += 17.5f;
				area.height -= 17.5f;
				mousePosition.y -= 17.5f;
			}
			Input.MousePosition = mousePosition;

			GUILayout.BeginArea( area );
			ExtendedGUI.BeginArea( new ExtendedGUIOption() { Type = ExtendedGUIOption.EType.WindowSize, Value = area.size } );

			foreach ( var item in controlsToProcess ) {
				item.OnGUI();
			}
		}

		[DocsDescription("Write your own toolbar logic here")]
		public virtual void OnToolbarGUI() { }

		[DocsDescription("Write your own GUI logic here")]
		public virtual void OnGUI() { }

		[DocsIgnore]
		public void EndGUI() {
			if ( Settings.DrawToolbar ) {
				var pos = Input.MousePosition;
				pos.y += 17.5f;
				Input.MousePosition = pos;
			}

			ExtendedGUI.EndArea();
			GUILayout.EndArea();

			var backgroundColor = GUI.backgroundColor;
			var color = GUI.color;
			for ( int i = notifications.Count - 1; i >= 0; i-- ) {
				var item = notifications[i];

				var xp = Size.x - item.Size.x - 20;
				var yp = Size.y - item.Size.y - 20 - ( i * ( item.Size.y + 5 ) );

				GUI.backgroundColor = GUI.color = item.Color;
				GUI.Box( new Rect( xp, yp, item.Size.x, item.Size.y ), "", notificationBackgroundStyle );
				GUI.Label( new Rect( xp, yp, item.Size.x, item.Size.y ), item.Text, notificationTextStyle );
			}
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;

			if ( Settings.DrawToolbar ) {
				ExtendedGUI.BeginToolbar();
				OnToolbarGUI();
				ExtendedGUI.EndToolbar();
			}

			if ( Input.Type == EventType.ScrollWheel && Settings.UseCamera ) {
				var delta = Input.ScrollDelta.y;
				if ( delta > 0 ) {
					Camera.z *= 0.9f;
				} else if ( delta < 0 ) {
					Camera.z *= 1.1f;
				}
			}

			if ( Settings.DrawTitleBarButtons ) {
				var rect = new Rect( new Vector2( Size.x - 13, 1 ), new Vector2( 13, 13 ) );
				if ( GUI.Button( rect, "", closeButtonStyle ) ) {
					Editor.RemoveWindow( this );
				}

				rect.x -= 13;
				if ( GUI.Button( rect, "", maximizeButtonStyle ) ) {
					Settings.IsFullscreen = !Settings.IsFullscreen;
					if ( !Settings.IsFullscreen ) {
						WindowRect = nonFullScreenRect;
					}
				}
			}
		}
		#endregion

		#region Controls
		[DocsDescription("Adds a control to the window")]
		[DocsParameter("control", "The control to add")]
		public virtual void AddControl( ExtendedControl control ) {
			if ( Controls.Contains( control ) ) return;

			control.Window = this;

			if ( !control.IsInitialized ) {
				control.OnInitialize();
			}

			var type = control.GetType();
			if ( !controlsDict.ContainsKey( type ) ) {
				controlsDict.Add( type, new List<ExtendedControl>() );
			}

			controlsDict[type].Add( control );
			Controls.Add( control );
		}
		
		[DocsDescription("Removes a control from the window")]
		[DocsParameter("control", "The control to remove")]
		public virtual void RemoveControl( ExtendedControl control ) {
			controlsToRemove.Add( control );
		}

		[DocsDescription("Removes all controls from the window")]
		public virtual void ClearControls() {
			controlsToRemove.AddRange( Controls );
		}
		
		[DocsDescription("Returns a list of controls of the given type")]
		public List<T> GetControlsByType<T>() where T : ExtendedControl {
			var type = typeof(T);
			if ( controlsDict.ContainsKey( type ) ) {
				var items = new List<T>();
				foreach ( var item in controlsDict[type] ) {
					items.Add( item as T );
				}
				return items;
			} else {
				return new List<T>();
			}
		}
		
		[DocsDescription("Returns a list of controls of the given type")]
		[DocsParameter("type", "The type of the control to return")]
		public List<ExtendedControl> GetControlsByType( Type type ) {
			if ( controlsDict.ContainsKey( type ) ) {
				return controlsDict[type];
			} else {
				return new List<ExtendedControl>();
			}
		}

		[DocsDescription("Returns a list of controls of the given type, including controls that inherit from this type")]
		public List<T> GetControlsByBaseType<T>() where T : ExtendedControl {
			var type = typeof(T);
			var list = new List<T>();

			foreach ( var item in Controls ) {
				if ( item.GetType() == type ) {
					list.Add( item as T );
				} else {
					var baseType = item.GetType().BaseType;
					while ( baseType != null ) {
						if ( baseType == type ) {
							list.Add( item as T );
							break;
						}
						baseType = baseType.BaseType;
					}
				}
			}

			return list;
		}
		
		[DocsDescription("Returns a list of controls of the given type, including controls that inherit from this type")]
		[DocsParameter("type", "The type of the control to return")]
		public List<ExtendedControl> GetControlsByBaseType( Type type ) {
			var list = new List<ExtendedControl>();

			foreach ( var item in Controls ) {
				if ( item.GetType() == type ) {
					list.Add( item );
				} else {
					var baseType = item.GetType().BaseType;
					while ( baseType != null ) {
						if ( baseType == type ) {
							list.Add( item );
							break;
						}
						baseType = baseType.BaseType;
					}
				}
			}

			return list;
		}
		#endregion

		#region Events
		[DocsDescription("Invoked when a ContextClick event occurs")]
		[DocsParameter("position", "The location of the right-mouse click")]
		public void OnContextClick( Vector2 position ) {
			bool used = false;
			OnContextClick( position, ref used );
		}

		[DocsDescription("Invoked when a ContextClick event occurs")]
		[DocsParameter("position", "The location of the right-mouse click")]
		[DocsParameter("used", "-")]
		public virtual void OnContextClick( Vector2 position, ref bool used ) {
			if ( Settings.DrawToolbar ) {
				position.y -= 17.5f;
			}

			for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
				controlsToProcess[i].OnContextClick( position, ref used );
			}
		}

		[DocsDescription("Invoked when a DragExited event occurs")]
		public virtual void OnDragExited() {
			for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
				controlsToProcess[i].OnDragExited();
			}
		}

		[DocsDescription("Invoked when a DragPerform event occurs")]
		[DocsParameter("paths", "Path(s) of the file(s) being dragged onto the editor")]
		[DocsParameter("position", "The mouse position")]
		public virtual void OnDragPerform( string[] paths, Vector2 position ) {
			for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
				controlsToProcess[i].OnDragPerform( paths, position );
			}
		}

		[DocsDescription("Invoked when a DragUpdate event occurs")]
		[DocsParameter("paths", "Path(s) of the file(s) being dragged onto the editor")]
		[DocsParameter("position", "The mouse position")]
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) {
			for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
				controlsToProcess[i].OnDragUpdate( paths, position );
			}
		}
		#endregion

		#region Notifications
		[DocsDescription("Shows a notification at the bottom-right corner of the window")]
		[DocsParameter("text", "The text to display on the notification")]
		public void ShowNotification( string text ) {
			ShowNotification( text, Color.white, 1.25f );
		}
		
		[DocsDescription("Shows a notification at the bottom-right corner of the window")]
		[DocsParameter("text", "The text to display on the notification")]
		[DocsParameter("color", "The color of the notification")]
		[DocsParameter("duration", "The duration of the notification")]
		[DocsParameter("style", "The style of the notification")]
		public void ShowNotification( string text, Color color, float duration ) {
			if ( string.IsNullOrEmpty( text ) ) return;
			color.a = 0;
			notifications.Add( new ExtendedNotification( text, color, duration, notificationTextStyle ) );
		}
		#endregion
	}
}
#endif