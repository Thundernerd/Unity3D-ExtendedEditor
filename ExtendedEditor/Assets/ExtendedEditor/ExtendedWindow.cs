#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEditor;

namespace TNRD {

	public class ExtendedWindow {

		private class Notification {
			public GUIContent Text;
			public Color @Color;
			public float Duration;
			public readonly Vector2 Size;

			public Notification( string text, Color color, float duration, GUIStyle style ) {
				Text = new GUIContent( text );
				Color = color;
				Duration = duration;

				style.CalcMinMaxWidth( Text, out Size.y, out Size.x );
				Size.y = style.CalcHeight( Text, Size.x );
			}
		}

		public ExtendedEditor Editor;

		public bool IsBlocking = true;
		public bool IsInitialized = false;

		#region GUI.Window 
		public GUIContent WindowContent = new GUIContent();
		public Rect WindowRect = new Rect();
		public GUIStyle WindowStyle = null;
		#endregion

		public Vector2 Position {
			get {
				return WindowRect.position;
			}
		}
		public Vector2 Size {
			get {
				return WindowRect.size;
			}
		}

		public ExtendedInput Input { get { return Editor.Input; } }

		[JsonProperty]
		protected List<ExtendedControl> Controls = new List<ExtendedControl>();
		[JsonIgnore]
		protected List<ExtendedControl> ControlsToProcess = new List<ExtendedControl>();
		[JsonProperty]
		private Dictionary<Type, List<ExtendedControl>> controlsDict = new Dictionary<Type, List<ExtendedControl>>();

		[JsonProperty]
		protected bool fullscreen = true;
		[JsonIgnore]
		private Vector2 previousEditorSize;

		[JsonIgnore]
		private List<Notification> notifications = new List<Notification>();
		[JsonIgnore]
		private GUIStyle notificationBackgroundStyle;
		[JsonIgnore]
		private GUIStyle notificationTextStyle;
		[JsonIgnore]
		private bool initializedStyles;

		private ExtendedWindow() { }
		public ExtendedWindow( bool fullscreen, bool isBlocking ) {
			this.fullscreen = fullscreen;
			IsBlocking = isBlocking;
		}

		public virtual void OnInitialize() {
			WindowRect = new Rect( 0, 0, Editor.position.size.x, Editor.position.size.y );
			IsInitialized = true;
		}

		public virtual void OnDeserialized() {
			for ( int i = 0; i < Controls.Count; i++ ) {
				Controls[i].Window = this;
				Controls[i].OnDeserialized();
			}
		}

		public virtual void OnDestroy() {
			IsInitialized = false;
		}

		public virtual void OnFocus() { }
		public virtual void OnLostFocus() { }

		public virtual void Update( bool hasFocus ) {
			ControlsToProcess = new List<ExtendedControl>( Controls );

			if ( fullscreen ) {
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
		}

		public virtual void OnGUI( int id ) {
			if ( !initializedStyles ) {
				notificationBackgroundStyle = new GUIStyle( "NotificationBackground" );
				notificationTextStyle = new GUIStyle( "NotificationText" );
				notificationTextStyle.padding = new RectOffset( 20, 20, 20, 20 );
				notificationTextStyle.fontSize = 17;
				initializedStyles = true;
			}

			var e = Editor.CurrentEvent;

			if ( Input.IsDoubleClick ) {
				OnDoubleClick( Input.Button, e.mousePosition );
			}

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

			foreach ( var item in ControlsToProcess ) {
				item.OnGUI();
			}

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
		}

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

		public List<T> GetControls<T>() where T : ExtendedControl {
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

		public List<ExtendedControl> GetControls( Type type ) {
			if ( controlsDict.ContainsKey( type ) ) {
				return controlsDict[type];
			} else {
				return new List<ExtendedControl>();
			}
		}

		#region Events
		public virtual void OnContextClick( Vector2 position ) {
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnContextClick( position );
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
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnScrollWheel( delta );
			}
		}
		#endregion

		public void ShowNotification( string text ) {
			ShowNotification( text, Color.white, 1.25f );
		}

		public void ShowNotification( string text, Color color, float duration ) {
			if ( string.IsNullOrEmpty( text ) ) return;
			color.a = 0;
			notifications.Add( new Notification( text, color, duration, notificationTextStyle ) );
		}
	}
}
#endif