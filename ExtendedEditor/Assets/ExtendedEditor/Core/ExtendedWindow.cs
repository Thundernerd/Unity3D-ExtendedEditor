#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

	public class ExtendedWindow {

		[JsonProperty]
		public ExtendedAssets Assets;
		[JsonIgnore]
		public ExtendedEditor Editor;
		[JsonProperty]
		public ExtendedWindowSettings Settings;

		public bool IsInitialized = false;

		#region GUI.Window 
		public GUIContent WindowContent = new GUIContent();
		public Rect WindowRect = new Rect();
		public GUIStyle WindowStyle = null;
		#endregion

		[JsonIgnore]
		public Vector2 Position {
			get {
				return WindowRect.position;
			}
		}
		[JsonIgnore]
		public Vector2 Size {
			get {
				return WindowRect.size;
			}
		}
		[JsonIgnore]
		public Vector3 Camera = new Vector3( 0, 0, 1 );
		[JsonIgnore]
		public Matrix4x4 ScaleMatrix = Matrix4x4.identity;
		[JsonIgnore]
		public ExtendedInput Input { get { return Editor.Input; } }

		[JsonProperty]
		protected List<ExtendedControl> Controls = new List<ExtendedControl>();
		[JsonIgnore]
		protected List<ExtendedControl> ControlsToProcess = new List<ExtendedControl>();
		[JsonProperty]
		private Dictionary<Type, List<ExtendedControl>> controlsDict = new Dictionary<Type, List<ExtendedControl>>();

		[JsonIgnore]
		private Vector2 previousEditorSize;

		[JsonIgnore]
		private bool initializedGUI = false;

		private const int cameraSpeed = 500;

		public ExtendedWindow() : this( new ExtendedWindowSettings() ) { }
		public ExtendedWindow( ExtendedWindowSettings settings ) {
			Settings = settings;
		}

		#region Initialization
		public virtual void OnInitialize() {
			Assets = new ExtendedAssets( Settings.AssetPath, this );

			if ( Settings.UseOnSceneGUI ) {
				SceneView.onSceneGUIDelegate += InternalSceneGUI;
			}

			WindowRect = new Rect( 0, 0, Editor.position.size.x, Editor.position.size.y );
			IsInitialized = true;
		}

		protected virtual void OnInitializeGUI() {
			notificationBackgroundStyle = new GUIStyle( "NotificationBackground" );
			notificationTextStyle = new GUIStyle( "NotificationText" );
			notificationTextStyle.padding = new RectOffset( 20, 20, 20, 20 );
			notificationTextStyle.fontSize = 17;
			initializedGUI = true;
		}

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

		public virtual void OnFocus() { }
		public virtual void OnLostFocus() { }

		public virtual void Update( bool hasFocus ) {
			ControlsToProcess = new List<ExtendedControl>( Controls );

			if ( Settings.IsFullscreen ) {
				var currentEditorSize = Editor.position.size;
				if ( currentEditorSize != previousEditorSize ) {
					WindowRect.size = currentEditorSize;
				}

				previousEditorSize = currentEditorSize;
			}

			foreach ( var item in ControlsToProcess ) {
				item.Update( hasFocus );
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

			if ( hasFocus ) {
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
		}

		#region SceneGUI
		public void InternalSceneGUI( SceneView view ) {
			Handles.BeginGUI();
			OnSceneGUI( view );
			Handles.EndGUI();
		}
		public virtual void OnSceneGUI( SceneView view ) {
			foreach ( var item in Controls ) {
				item.OnSceneGUI( view );
			}
		}
		#endregion

		#region GUI
		public void InternalGUI( int id ) {
			if ( !initializedGUI ) {
				OnInitializeGUI();
			}

			var e = Editor.CurrentEvent;

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
				case EventType.ScrollWheel:
					OnScrollWheel( e.delta );
					break;
			}

			BeginGUI();
			OnGUI();
			EndGUI();
		}
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
			if ( Settings.DrawToolbar ) {
				area.y += 17.5f;
				area.height -= 17.5f;

				// Feels a bit weird, but it has to be I guess
				var pos = Input.MousePosition;
				pos.y -= 17.5f;
				Input.MousePosition = pos;
			}

			GUILayout.BeginArea( area );
			ExtendedGUI.BeginArea( new ExtendedGUIOption() { Type = ExtendedGUIOption.EType.WindowSize, Value = area.size } );

			foreach ( var item in ControlsToProcess ) {
				item.OnGUI();
			}
		}
		public virtual void OnToolbarGUI() { }
		public virtual void OnGUI() { }
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
		}
		#endregion

		#region Controls
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
		public virtual void RemoveControl( ExtendedControl control ) {
			if ( control.IsInitialized ) {
				control.OnDestroy();
			}

			controlsDict[control.GetType()].Remove( control );
			Controls.Remove( control );
		}

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
		public List<ExtendedControl> GetControlsByType( Type type ) {
			if ( controlsDict.ContainsKey( type ) ) {
				return controlsDict[type];
			} else {
				return new List<ExtendedControl>();
			}
		}

		public List<T> GetControlsByBaseType<T>() where T : ExtendedControl {
			var type = typeof(T);
			var list = new List<T>();

			foreach ( var item in Controls ) {
				var baseType = item.GetType().BaseType;
				while ( baseType != null ) {
					if ( baseType == type ) {
						list.Add( item as T );
						break;
					}
					baseType = baseType.BaseType;
				}
			}

			return list;
		}
		public List<ExtendedControl> GetControlsByBaseType( Type type ) {
			var list = new List<ExtendedControl>();

			foreach ( var item in Controls ) {
				var baseType = item.GetType().BaseType;
				while ( baseType != null ) {
					if ( baseType == type ) {
						list.Add( item );
						break;
					}
					baseType = baseType.BaseType;
				}
			}

			return list;
		}
		#endregion

		#region Events
		public void OnContextClick( Vector2 position ) {
			bool used = false;
			OnContextClick( position, ref used );
		}
		public virtual void OnContextClick( Vector2 position, ref bool used ) {
			if ( Settings.DrawToolbar ) {
				position.y -= 17.5f;
			}

			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnContextClick( position, ref used );
			}
		}
		public virtual void OnDragExited() {
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnDragExited();
			}
		}
		public virtual void OnDragPerform( string[] paths, Vector2 position ) {
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnDragPerform( paths, position );
			}
		}
		public virtual void OnDragUpdate( string[] paths, Vector2 position ) {
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnDragUpdate( paths, position );
			}
		}
		public virtual void OnScrollWheel( Vector2 delta ) {
			if ( Settings.UseCamera ) {
				var mDelta = Input.MouseDelta.y;
				if ( mDelta > 0 ) {
					Camera.z *= 0.9f;
				} else if ( mDelta < 0 ) {
					Camera.z *= 1.1f;
				}
			}
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnScrollWheel( delta );
			}
		}
		#endregion

		#region Notifications
		[JsonIgnore]
		private List<ExtendedNotification> notifications = new List<ExtendedNotification>();
		[JsonIgnore]
		private GUIStyle notificationBackgroundStyle;
		[JsonIgnore]
		private GUIStyle notificationTextStyle;

		public void ShowNotification( string text ) {
			ShowNotification( text, Color.white, 1.25f );
		}
		public void ShowNotification( string text, Color color, float duration ) {
			if ( string.IsNullOrEmpty( text ) ) return;
			color.a = 0;
			notifications.Add( new ExtendedNotification( text, color, duration, notificationTextStyle ) );
		}
		#endregion
	}
}
#endif