#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {
	/// <summary>
	/// The base for an editor
	/// </summary>
	public class ExtendedEditor : EditorWindow {

		/// <summary>
		/// If true the editor will call Repaint every update
		/// </summary>
		[JsonProperty]
		protected bool RepaintOnUpdate = false;

		/// <summary>
		/// The active shared objects in this editor
		/// </summary>
		[JsonProperty]
		protected Dictionary<string, ExtendedSharedObject> SharedObjects = new Dictionary<string, ExtendedSharedObject>();

		/// <summary>
		/// The active windows in this editor
		/// </summary>
		[JsonProperty]
		protected List<ExtendedWindow> Windows = new List<ExtendedWindow>();

		[JsonIgnore]
		private List<ExtendedWindow> windowsToProcess = new List<ExtendedWindow>();

		private List<ExtendedWindow> windowsToRemove = new List<ExtendedWindow>();

		private Dictionary<Type, List<ExtendedWindow>> windowsDict = new Dictionary<Type, List<ExtendedWindow>>();

		private ExtendedModalWindow modalWindow;

		private Action<ExtendedModalWindowEventArgs> modalWindowCallback;

		private double previousTime = 0;

		private ExtendedWindow windowToResize = null;

		private ExtendedWindow windowToDrag = null;

		[JsonIgnore]
		public float DeltaTime = 0;

		[JsonIgnore]
		public ExtendedInput Input { get; private set; }

		/// <summary>
		/// The current event being processed by the Input manager
		/// </summary>
		[JsonIgnore]
		public Event CurrentEvent { get; private set; }

		private object initializer;

		protected virtual void OnInitialize() {
			initializer = new object();

			Input = new ExtendedInput();

			Windows = new List<ExtendedWindow>();
			windowsToProcess = new List<ExtendedWindow>();
			modalWindow = null;
			modalWindowCallback = null;
		}

		protected virtual void OnDestroy() {
			for ( int i = Windows.Count - 1; i >= 0; i-- ) {
				Windows[i].OnDestroy();
			}
		}

		protected virtual void OnFocus() {
			for ( int i = Windows.Count - 1; i >= 0; i-- ) {
				Windows[i].OnFocus();
				if ( Windows[i].Settings.IsBlocking ) return;
			}
		}

		protected virtual void OnLostFocus() {
			for ( int i = Windows.Count - 1; i >= 0; i-- ) {
				Windows[i].OnLostFocus();
				if ( Windows[i].Settings.IsBlocking ) return;
			}
		}

		protected virtual void Update() {
			if ( initializer == null ) {
				OnInitialize();
			}

			var time = Time.realtimeSinceStartup;
			// Min-Maxing this to make sure it's between 0 and 1/60
			DeltaTime = Mathf.Min( Mathf.Max( 0, (float)( time - previousTime ) ), 0.016f );
			previousTime = time;

			var hasFocus = focusedWindow == this;

			for ( int i = Windows.Count - 1; i >= 0; i-- ) {
				Windows[i].Update( hasFocus && modalWindow == null );

				if ( Windows[i].Settings.IsBlocking ) {
					break;
				}
			}

			if ( modalWindow != null ) {
				modalWindow.Update( hasFocus );
			}

			foreach ( var item in SharedObjects ) {
				item.Value.Update( hasFocus );
			}

			Input.Update();

			if ( RepaintOnUpdate ) {
				Repaint();
			}

			if ( windowsToRemove.Count > 0 ) {
				foreach ( var window in windowsToRemove ) {
					if ( window.IsInitialized ) {
						window.OnDestroy();
					}

					windowsDict[window.GetType()].Remove( window );
					Windows.Remove( window );
				}
			}
		}

		#region GUI
		protected virtual void OnGUI() {
			if ( initializer == null ) return;

			CurrentEvent = Event.current;
			Input.OnGUI( CurrentEvent );

			if ( CurrentEvent != null ) {
				switch ( CurrentEvent.type ) {
					case EventType.ContextClick:
						OnContextClick( CurrentEvent.mousePosition );
						break;
					case EventType.DragExited:
						OnDragExited();
						break;
					case EventType.DragPerform:
						OnDragPerform( DragAndDrop.paths, CurrentEvent.mousePosition );
						break;
					case EventType.DragUpdated:
						OnDragUpdate( DragAndDrop.paths, CurrentEvent.mousePosition );
						break;
				}
			}

			windowsToProcess = new List<ExtendedWindow>( Windows );

			BeginWindows();
			WindowsGUI();
			ModalWindowGUI();
			EndWindows();

			HandleWindowDragAndResize();
		}

		private void WindowsGUI() {
			for ( int i = windowsToProcess.Count - 1; i >= 0; i-- ) {
				var w = windowsToProcess[i];
				w.WindowID = i;

				if ( w.WindowStyle == null ) {
					GUI.Window( w.WindowID, w.WindowRect, w.InternalGUI, w.WindowContent );
				} else {
					GUI.Window( w.WindowID, w.WindowRect, w.InternalGUI, w.WindowContent, w.WindowStyle );
				}

				if ( w.Settings.IsBlocking ) {
					break;
				}
			}
		}
		private void ModalWindowGUI() {
			if ( modalWindow != null ) {
				GUI.BringWindowToFront( windowsToProcess.Count );
				var p1 = modalWindow.WindowRect;
				modalWindow.WindowRect = GUI.Window( windowsToProcess.Count, modalWindow.WindowRect, modalWindow.OnGUI, modalWindow.Title );
				GUI.FocusWindow( windowsToProcess.Count );
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
		}
		private void HandleWindowDragAndResize() {
			for ( int i = 0; i < windowsToProcess.Count; i++ ) {
				var window = windowsToProcess[i];
				if ( window.Settings.AllowResize ) {
					var rect = new Rect( window.WindowRect.position + window.WindowRect.size - new Vector2( 16, 16 ), new Vector2( 24, 24 ) );
					if ( rect.Contains( Input.MousePosition ) ) {
						EditorGUIUtility.AddCursorRect( rect, MouseCursor.ResizeUpLeft );

						if ( Input.ButtonPressed( EMouseButton.Left ) ) {
							windowToResize = window;
						}
					}

					if ( Input.ButtonReleased( EMouseButton.Left ) ) {
						windowToResize = null;
					}
				}

				if ( window.Settings.AllowRepositioning ) {
					var rect = new Rect( window.WindowRect.position, new Vector2( window.WindowRect.width, 16.5f ) );
					if ( rect.Contains( Input.MousePosition ) ) {
						if ( Input.ButtonPressed( EMouseButton.Left ) ) {
							windowToDrag = window;
						}
					}

					if ( Input.ButtonReleased( EMouseButton.Left ) ) {
						windowToDrag = null;
					}
				}
			}

			if ( CurrentEvent.type == EventType.MouseDrag ) {
				if ( windowToResize != null ) {
					var size = windowToResize.Size;
					size += Input.MouseDelta;
					size.x = Mathf.Max( size.x, 50 );
					size.y = Mathf.Max( size.y, 50 );
					windowToResize.Size = size;
				}

				if ( windowToDrag != null ) {
					windowToDrag.Position += Input.MouseDelta;
				}
			}
		}
		#endregion

		#region Window
		/// <summary>
		/// Adds a window to the end of the list
		/// </summary>
		/// <param name="window">The window to add</param>
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

		/// <summary>
		/// Removes the window from the list, destroying it's loaded assets and controls
		/// </summary>
		/// <param name="window">The window to remove</param>
		public virtual void RemoveWindow( ExtendedWindow window ) {
			windowsToRemove.Add( window );
		}

		/// <summary>
		/// Removes all (active) windows from the editor, destroying all loaded assets and controls
		/// </summary>
		public virtual void ClearWindows() {
			windowsToRemove.AddRange( Windows );
		}

		/// <summary>
		/// Returns a list of windows of type T
		/// </summary>
		/// <typeparam name="T">The type of the window</typeparam>
		public List<T> GetWindowsByType<T>() where T : ExtendedWindow {
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

		/// <summary>
		/// Returns a list of windows of the given type
		/// </summary>
		/// <param name="type">The type of the window</param>
		public List<ExtendedWindow> GetWindowsByType( Type type ) {
			if ( windowsDict.ContainsKey( type ) ) {
				return windowsDict[type];
			} else {
				return new List<ExtendedWindow>();
			}
		}

		/// <summary>
		/// Returns a list of windows of type T including windows that inherit from this type
		/// </summary>
		/// <typeparam name="T">The type of the window</typeparam>
		public List<T> GetWindowsByBaseType<T>() where T : ExtendedWindow {
			var type = typeof(T);
			var list = new List<T>();

			foreach ( var item in Windows ) {
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

		/// <summary>
		/// Returns a list of windows of the given type including windows that inherit from this type
		/// </summary>
		/// <param name="type">The type of the window</param>
		public List<ExtendedWindow> GetWindowsByBaseType( Type type ) {
			var list = new List<ExtendedWindow>();

			foreach ( var item in Windows ) {
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

		#region Modal Window
		/// <summary>
		/// Adds a modal window to the screen
		/// </summary>
		/// <param name="window">The window to add</param>
		public void ShowModalWindow( ExtendedModalWindow window ) {
			ShowModalWindow( window, null );
		}

		/// <summary>
		/// Adds a modal window to the screen and invokes the callback when the window is closed
		/// </summary>
		/// <param name="window">The window to add</param>
		/// <param name="callback">The callback to invoke</param>
		public void ShowModalWindow( ExtendedModalWindow window, Action<ExtendedModalWindowEventArgs> callback ) {
			modalWindow = window;
			modalWindow.Editor = this;

			modalWindowCallback = callback;
		}
		#endregion

		#region Shared Object
		/// <summary>
		/// Adds a shared object to the editor. If the key exists the object will be overwritten
		/// </summary>
		/// <param name="key">The key to store the object with</param>
		/// <param name="value">The object to store</param>
		public virtual void AddSharedObject( string key, ExtendedSharedObject value ) {
			AddSharedObject( key, value, true );
		}

		/// <summary>
		/// Adds a shared object to the editor
		/// </summary>
		/// <param name="key">The key to store the object with</param>
		/// <param name="value">The object to store</param>
		/// <param name="overwrite">Should the object be overwritten if the key already exists</param>
		public virtual void AddSharedObject( string key, ExtendedSharedObject value, bool overwrite ) {
			if ( SharedObjects.ContainsKey( key ) && !overwrite ) return;

			if ( SharedObjects.ContainsKey( key ) ) {
				SharedObjects[key] = value;
			} else {
				SharedObjects.Add( key, value );
			}
		}

		/// <summary>
		/// Removes the shared object with the given key from the editor
		/// </summary>
		/// <param name="key">The key of the object to remove</param>
		public virtual void RemoveSharedObject( string key ) {
			if ( !SharedObjects.ContainsKey( key ) ) return;
			SharedObjects.Remove( key );
		}

		/// <summary>
		/// Removes all the shared objects from the editor
		/// </summary>
		public virtual void ClearSharedObjects() {
			SharedObjects.Clear();
		}

		/// <summary>
		/// Gets the shared object stored with the given key
		/// </summary>
		/// <typeparam name="T">The type of the shared object</typeparam>
		/// <param name="key">The key the object is stored with</param>
		/// <returns>T or null if the key doesn't exist</returns>
		public T GetSharedObject<T>( string key ) where T : ExtendedSharedObject {
			if ( SharedObjects.ContainsKey( key ) ) {
				return SharedObjects[key] as T;
			} else {
				return null;
			}
		}
		#endregion

		#region Events
		protected virtual void OnContextClick( Vector2 position ) {
			if ( modalWindow != null ) {
				modalWindow.OnContextClick( position );
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
		#endregion

		#region Serialization/Deserialization
		/// <summary>
		/// Serializes the editor as a whole
		/// </summary>
		/// <returns>A JSON string</returns>
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

		/// <summary>
		/// Saves a serialized state of the editor in the preferences
		/// </summary>
		/// <param name="key">The key to store the serialized editor with</param>
		/// <returns>True on success, false on failure</returns>
		public bool SaveToPreferences( string key ) {
			if ( string.IsNullOrEmpty( key ) ) {
				Debug.LogError( "Unable to save to preferences, key cannot be empty." );
				return false;
			}

			var content = Serialize();

			if ( string.IsNullOrEmpty( content ) ) {
				// No need to log this as this is done before
				//Debug.LogError( "Unable to save to preferences, error while serializing." );
				return false;
			} else {
				try {
					PlayerPrefs.SetString( key, content );
					PlayerPrefs.Save();
					return true;
				} catch ( PlayerPrefsException) {
					Debug.LogError( "Unabled to save to preferences, exceeding maximum size." );
					return false;
				}
			}
		}

		/// <summary>
		/// Saves a serialized state of the editor to a file at the given path
		/// </summary>
		/// <param name="path">The path to the file to save</param>
		/// <returns>True on success, false on failure</returns>
		public bool SaveToFile( string path ) {
			if ( string.IsNullOrEmpty( path ) ) {
				Debug.LogError( "Unable to save to file, path cannot be empty." );
				return false;
			}

			var content = Serialize();

			if ( string.IsNullOrEmpty( content ) ) {
				// No need to log this as this is done before
				//Debug.LogError( "Unable to save to file, error while serializing" );
				return false;
			} else {
				try {
					File.WriteAllText( path, content );
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
		}

		/// <summary>
		/// Deserializes a serialized editor
		/// </summary>
		/// <typeparam name="T">The type of the editor</typeparam>
		/// <param name="value">A JSON string</param>
		public void Deserialize<T>( string value ) where T : ExtendedEditor {
			var settings = new JsonSerializerSettings();
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

				for ( int i = Windows.Count - 1; i >= 0; i-- ) {
					var w = Windows[i];

					w.Editor = this;

					var t = w.GetType();

					if ( !windowsDict.ContainsKey( t ) ) {
						windowsDict.Add( t, new List<ExtendedWindow>() );
					}

					if ( !windowsDict[t].Contains( w ) ) {
						windowsDict[t].Add( w );
					}

					w.OnDeserialized();
				}
			}
		}

		/// <summary>
		/// Loads and deserializes the editor from the preferences
		/// </summary>
		/// <typeparam name="T">The type of the editor</typeparam>
		/// <param name="key">The key that the editor is stored with</param>
		/// <returns>True on success, false on failure</returns>
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

		/// <summary>
		/// Loads and deserializes the editor from a file
		/// </summary>
		/// <typeparam name="T">The type of the editor</typeparam>
		/// <param name="path">The path to the file</param>
		/// <returns>True on success, false on failure</returns>
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

		/// <summary>
		/// Destroys the given asset
		/// </summary>
		/// <param name="obj">The asset to destroy</param>
		public void DestroyAsset( UnityEngine.Object obj ) {
			DestroyImmediate( obj );
		}

		/// <summary>
		/// Destroys the given asset
		/// </summary>
		/// <param name="obj">The asset to destroy</param>
		/// <param name="allowDestroyingAssets">True means allowing an asset to be deleted from disk</param>
		public void DestroyAsset( UnityEngine.Object obj, bool allowDestroyingAssets ) {
			DestroyImmediate( obj, allowDestroyingAssets );
		}
	}
}
#endif