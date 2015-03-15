#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;

namespace TNRD {
	public class ExtendedEditor : EditorWindow {

		[JsonProperty]
		protected bool RepaintOnUpdate = false;

		[JsonProperty]
		protected Dictionary<string, ExtendedSharedObject> SharedObjects = new Dictionary<string, ExtendedSharedObject>();

		[JsonProperty]
		protected List<ExtendedWindow> Windows = new List<ExtendedWindow>();
		[JsonIgnore]
		protected List<ExtendedWindow> WindowsToProcess = new List<ExtendedWindow>();
		[JsonProperty]
		private Dictionary<Type, List<ExtendedWindow>> windowsDict = new Dictionary<Type, List<ExtendedWindow>>();
		[JsonIgnore]
		private ExtendedModalWindow modalWindow;
		[JsonIgnore]
		private Action<ExtendedModalWindowEventArgs> modalWindowCallback;
		[JsonIgnore]
		private long lastClick = 0;
		[JsonIgnore]
		private int lastButton = -1;

		/// <summary>
		/// Dirtiest variable; it is 50 percent of a recompilation check
		/// </summary>
		private object initializedCheck;

		protected virtual void OnInitialize() {
			initializedCheck = new object();

			Windows = new List<ExtendedWindow>();
			WindowsToProcess = new List<ExtendedWindow>();
			modalWindow = null;
			modalWindowCallback = null;

			wantsMouseMove = true;
		}

		protected virtual void Update() {
			// The other 50 percent
			if ( initializedCheck == null ) {
				ExtendedInput.Initialize();
				// Horribleeeee!
				OnInitialize();
			}

			if ( modalWindow == null ) {
				for ( int i = Windows.Count - 1; i >= 0; i-- ) {
					Windows[i].Update();

					if ( Windows[i].IsBlocking ) {
						break;
					}
				}
			}

			foreach ( var item in SharedObjects ) {
				item.Value.Update();
			}

			if ( RepaintOnUpdate ) {
				Repaint();
			}
		}

		protected virtual void OnGUI() {
			var e = Event.current;
			if ( e != null ) {
				switch ( e.type ) {
					case EventType.ContextClick:
						OnContextClick( e.mousePosition );
						break;
					case EventType.DragExited:
						OnDragExited();
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
					case EventType.ScrollWheel:
						OnScrollWheel( e.delta );
						break;
				}
			}

			ExtendedInput.OnGUI();

			WindowsToProcess = new List<ExtendedWindow>( Windows );

			BeginWindows();
			for ( int i = WindowsToProcess.Count - 1; i >= 0; i-- ) {
				var w = WindowsToProcess[i];
				if ( w.WindowStyle == null ) {
					GUI.Window( i, w.WindowRect, w.OnGUI, w.WindowContent );
				} else {
					GUI.Window( i, w.WindowRect, w.OnGUI, w.WindowContent, w.WindowStyle );
				}

				if ( w.IsBlocking ) {
					break;
				}
			}

			if ( modalWindow != null ) {
				GUI.BringWindowToFront( WindowsToProcess.Count );
				var p1 = modalWindow.WindowRect;
				modalWindow.WindowRect = GUI.Window( WindowsToProcess.Count, modalWindow.WindowRect, modalWindow.OnGUI, modalWindow.Title );
				if ( p1 != modalWindow.WindowRect ) {
					Event.current.Use();
				}

				if ( modalWindow.IsDone ) {
					var windowCopy = modalWindow;
					var callbackCopy = modalWindowCallback;

					modalWindow = null;
					modalWindowCallback = null;

					if ( callbackCopy != null ) {
						callbackCopy.Invoke( new ExtendedModalWindowEventArgs( windowCopy, windowCopy.Result ) );
					}
				}
			}
			EndWindows();
		}

		#region Window
		public virtual void AddWindow( ExtendedWindow window ) {
			if ( Windows.Contains( window ) ) return;

			window.Editor = this;

			if ( !window.IsInitialized ) {
				window.OnInitialize();
			}

			var type = window.GetType();
			if ( !windowsDict.ContainsKey( type ) ) {
				windowsDict.Add( type, new List<ExtendedWindow>() );
			}

			windowsDict[type].Add( window );
			Windows.Add( window );
		}

		public virtual void RemoveWindow( ExtendedWindow window ) {
			if ( window.IsInitialized ) {
				window.OnDestroy();
			}

			windowsDict[window.GetType()].Remove( window );
			Windows.Remove( window );
		}

		public List<T> GetWindows<T>() where T : ExtendedWindow {
			var type = typeof(T);
			if ( windowsDict.ContainsKey( type ) ) {
				var items = new List<T>();
				foreach ( var item in windowsDict[type] ) {
					items.Add( item as T );
				}
				return items;
			} else {
				return new List<T>();
			}
		}

		public List<ExtendedWindow> GetWindows( Type type ) {
			if ( windowsDict.ContainsKey( type ) ) {
				return windowsDict[type];
			} else {
				return new List<ExtendedWindow>();
			}
		}
		#endregion

		#region Modal Window
		public void ShowModalWindow( ExtendedModalWindow window, Action<ExtendedModalWindowEventArgs> callback ) {
			modalWindow = window;
			modalWindow.Editor = this;

			modalWindowCallback = callback;
		}

		public void ShowModalWindow( ExtendedModalWindow window ) {
			ShowModalWindow( window, null );
		}
		#endregion

		#region Shared Object
		public virtual void AddSharedObject( string key, ExtendedSharedObject value ) {
			AddSharedObject( key, value, true );
		}

		public virtual void AddSharedObject( string key, ExtendedSharedObject value, bool overwrite ) {
			if ( SharedObjects.ContainsKey( key ) && !overwrite ) return;

			if ( SharedObjects.ContainsKey( key ) ) {
				SharedObjects[key] = value;
			} else {
				SharedObjects.Add( key, value );
			}
		}

		public ExtendedSharedObject GetSharedObject( string key ) {
			if ( SharedObjects.ContainsKey( key ) ) {
				return SharedObjects[key];
			} else {
				return null;
			}
		}

		public T GetSharedObject<T>( string key ) where T : ExtendedSharedObject {
			if ( SharedObjects.ContainsKey( key ) ) {
				return SharedObjects[key] as T;
			} else {
				return default(T);
			}
		}

		public virtual void RemoveSharedObject( string key ) {
			if ( !SharedObjects.ContainsKey( key ) ) return;
			SharedObjects.Remove( key );
		}
		#endregion

		#region Events
		protected virtual void OnContextClick( Vector2 position ) {
			if ( modalWindow != null ) {
				modalWindow.OnContextClick( position );
			}
		}
		protected virtual void OnDoubleClick( EMouseButton button, Vector2 position ) {
			if ( modalWindow != null ) {
				modalWindow.OnDoubleClick( button, position );
			}
		}
		protected virtual void OnDragExited() {
			if ( modalWindow != null ) {
				modalWindow.OnDragExited();
			}
		}
		protected virtual void OnDragPerform( string[] paths, Vector2 position ) {
			if ( modalWindow != null ) {
				modalWindow.OnDragPerform( paths, position );
			}
		}
		protected virtual void OnDragUpdate( string[] paths, Vector2 position ) {
			if ( modalWindow != null ) {
				modalWindow.OnDragUpdate( paths, position );
			}
		}
		protected virtual void OnScrollWheel( Vector2 delta ) {
			if ( modalWindow != null ) {
				modalWindow.OnScrollWheel( delta );
			}
		}
		#endregion

		#region Serialization/Deserialization
		public string Serialize() {
			var settings = new JsonSerializerSettings();
			settings.PreserveReferencesHandling = PreserveReferencesHandling.All;
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			settings.TypeNameHandling = TypeNameHandling.Auto;

			try {
				var serialized = JsonConvert.SerializeObject( this, Formatting.None, settings );
				return serialized;
			} catch ( JsonSerializationException ex ) {
				Debug.LogErrorFormat( "Error serializing: {0}", ex.Message );
				return "";
			}
		}

		public bool SaveToPreferences( string key ) {
			try {
				var serialized = Serialize();
				if ( string.IsNullOrEmpty( key ) ) {
					Debug.LogError( "Unable to save to preferences, key cannot be empty." );
					return false;
				} else if ( string.IsNullOrEmpty( serialized ) ) {
					Debug.LogError( "Unable to save to preferences, error while serializing." );
					return false;
				}

				PlayerPrefs.SetString( key, serialized );
				PlayerPrefs.Save();
				return true;
			} catch ( PlayerPrefsException) {
				Debug.LogError( "Unabled to save to preferences, exceeding maximum size." );
				return false;
			}
		}

		public bool SaveToFile( string path ) {
			if ( string.IsNullOrEmpty( path ) ) return false;

			try {
				var serialized = Serialize();
				if ( string.IsNullOrEmpty( path ) ) {
					Debug.LogError( "Unable to save to file, path cannot be empty." );
					return false;
				} else if ( string.IsNullOrEmpty( serialized ) ) {
					Debug.LogError( "Unable to save to file, error while serializing." );
					return false;
				}

				File.WriteAllText( path, serialized );
				return true;
			} catch ( PathTooLongException) {
				Debug.LogError( "Unable to save to file, path is too long." );
				return false;
			} catch ( NotSupportedException) {
				Debug.LogError( "Unable to save to file, check your path format." );
				return false;
			} catch ( System.Security.SecurityException) {
				Debug.LogError( "Unable to save to file, lacking permission to write the file." );
				return false;
			} catch ( UnauthorizedAccessException) {
				Debug.LogError( "Unable to save to file, check your permissions, if the file is writable, and if you're on the right platform." );
				return false;
			} catch ( DirectoryNotFoundException) {
				Debug.LogError( "Unable to save to file, no such directory." );
				return false;
			} catch ( IOException) {
				Debug.LogError( "Unable to save to file, IO exception." );
				return false;
			}
		}

		public void Deserialize<T>( string value ) where T : ExtendedEditor {
			var settings = new JsonSerializerSettings();
			settings.Converters.Add( new ExtebdedEditorConverter() );
			settings.TypeNameHandling = TypeNameHandling.Auto;

			ExtendedEditor deserialized = null;

			try {
				deserialized = JsonConvert.DeserializeObject<T>( value, settings );
			} catch ( JsonReaderException ex ) {
				deserialized = null;
				Debug.LogErrorFormat( "Error deserializing: {0}", ex.Message );
			}

			if ( deserialized != null ) {
				RepaintOnUpdate = deserialized.RepaintOnUpdate;
				SharedObjects = deserialized.SharedObjects;
				Windows = deserialized.Windows;
				windowsDict = deserialized.windowsDict;

				// Backward-compat
				if ( windowsDict == null ) {
					windowsDict = new Dictionary<Type, List<ExtendedWindow>>();
				}

				for ( int i = 0; i < Windows.Count; i++ ) {
					var w = Windows[i];

					w.Editor = this;
					w.OnDeserialized();

					var t = w.GetType();

					if ( !windowsDict.ContainsKey( t ) ) {
						windowsDict.Add( t, new List<ExtendedWindow>() );
					}

					if ( !windowsDict[t].Contains( w ) ) {
						windowsDict[t].Add( w );
					}
				}
			}
		}

		public bool LoadFromPreferences<T>( string key ) where T : ExtendedEditor {
			if ( PlayerPrefs.HasKey( key ) ) {
				try {
					Deserialize<T>( PlayerPrefs.GetString( key ) );
					return true;
				} catch ( Exception) {
					Debug.LogError( "Unabled to deserialize content." );
					return false;
				}
			} else {
				Debug.LogError( "Unabled to deserialize content, key does not exist" );
				return false;
			}
		}

		public bool LoadFromFile<T>( string path ) where T : ExtendedEditor {
			if ( string.IsNullOrEmpty( path ) ) {
				Debug.LogError( "Path is empty, cancelling LoadFromFile." );
				return false;
			}

			try {
				var content = File.ReadAllText( path );
				Deserialize<T>( content );
				return true;
			} catch ( FileNotFoundException) {
				Debug.LogError( "Unable to deserialize content, no such file." );
				return false;
			} catch ( NotSupportedException) {
				Debug.LogError( "Unable to deserialize content, check your path format." );
				return false;
			} catch ( System.Security.SecurityException) {
				Debug.LogError( "Unable to deserialize content, lacking permission to read the file." );
				return false;
			} catch ( DirectoryNotFoundException) {
				Debug.LogError( "Unable to deserialize content, no such directory." );
				return false;
			} catch ( IOException) {
				Debug.LogError( "Unable to deserialize content, IO exception." );
				return false;
			} catch ( Exception ex ) {
				Debug.LogError( ex );
				Debug.LogError( "Unabled to deserialize content." );
				return false;
			}
		}
		#endregion

		public void DestroyAsset( UnityEngine.Object obj ) {
			DestroyImmediate( obj );
		}

		public void DestroyAsset( UnityEngine.Object obj, bool allowDestroyingAssets ) {
			DestroyImmediate( obj, allowDestroyingAssets );
		}
	}
}
#endif