#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEditor;

namespace TNRD {

	public class ExtendedWindow {

		public ExtendedEditor Editor;

		public bool IsBlocking = true;
		public bool IsInitialized = false;

		#region GUI.Window 
		public Rect WindowRect = new Rect();
		public GUIContent WindowContent = new GUIContent();
		public GUIStyle WindowStyle;
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

		[JsonProperty]
		protected List<ExtendedControl> Controls;
		[JsonIgnore]
		protected List<ExtendedControl> ControlsToProcess;
		[JsonProperty]
		private Dictionary<Type, List<ExtendedControl>> controlsDict;

		[JsonProperty]
		protected bool updateSizeInUpdate = false;
		[JsonIgnore]
		private long lastClick = 0;
		[JsonIgnore]
		private int lastButton = -1;

		public ExtendedWindow() {
			Controls = new List<ExtendedControl>();
			ControlsToProcess = new List<ExtendedControl>();
			controlsDict = new Dictionary<Type, List<ExtendedControl>>();
		}
		public ExtendedWindow( bool isBlocking ) : this() {
			IsBlocking = isBlocking;
		}

		public virtual void OnInitialize() {
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

		public virtual void Update() {
			ControlsToProcess = new List<ExtendedControl>( Controls );

			if ( updateSizeInUpdate ) {
				var previousSize = WindowRect.size;
				var currentSize = new Vector2( Editor.position.size.x - 2, Editor.position.size.y );
				WindowRect.size = currentSize;
				if ( previousSize != currentSize ) {
					Editor.Repaint();
				}
			}

			foreach ( var item in ControlsToProcess ) {
				item.Update();
			}
		}

		public virtual void OnGUI( int id ) {
			var e = Event.current;

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
					case EventType.MouseDown:
						var click = DateTime.Now.Ticks;
						var button = e.button;

						if ( click - lastClick < 2500000 && button == lastButton ) {
							OnDoubleClick( (EMouseButton)e.button, e.mousePosition );
						}

						lastClick = click;
						lastButton = button;
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
		public virtual void OnDoubleClick( EMouseButton button, Vector2 position ) {
			for ( int i = ControlsToProcess.Count - 1; i >= 0; i-- ) {
				ControlsToProcess[i].OnDoubleClick( button, position );
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
	}
}
#endif