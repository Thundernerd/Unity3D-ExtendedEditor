#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Base class for windows that can be added to ExtendedEditor
    /// </summary>
    public class ExtendedWindow {

        private static ExtendedWindow currentWindow;

        /// <summary>
        /// Converts the given value into a valid world position
        /// </summary>
        /// <param name="value">The value to convert</param>
        public static Vector2 ToWorldPosition( Vector2 value ) {
            if ( currentWindow == null ) return new Vector2();

            var size = currentWindow.Size;
            var nValue = new Vector2( value.x, value.y );
            nValue.x += currentWindow.Camera.x * currentWindow.Camera.z;
            nValue.y -= currentWindow.Camera.y * currentWindow.Camera.z;
            nValue -= size / 2;
            nValue.y *= -1;
            nValue /= 100;
            nValue /= currentWindow.Camera.z;
            return nValue;
        }

        /// <summary>
        /// Converts the given value into a valid screen (GUI) position
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static Vector2 ToScreenPosition( Vector2 value ) {
            if ( currentWindow == null ) return new Vector2();

            var temp = value * currentWindow.Camera.z;
            var nValue = currentWindow.Size / 2;
            nValue.x += temp.x * 100;
            nValue.y -= temp.y * 100;
            nValue.x -= currentWindow.Camera.x * currentWindow.Camera.z;
            nValue.y += currentWindow.Camera.y * currentWindow.Camera.z;
            return nValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ToScreenSize( Vector2 value ) {
            if ( currentWindow == null ) return new Vector2();
            return value * 100 * currentWindow.Camera.z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ToWorldSize( Vector2 value ) {
            if ( currentWindow == null ) return new Vector2();
            return value / 100 / currentWindow.Camera.z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetScreenInWorldSize() {
            if ( currentWindow == null ) return new Vector2();
            return currentWindow.Size / 100 / currentWindow.Camera.z;
        }

        /// <summary>
        /// Makes sure that the given position is at the center of the window
        /// </summary>
        /// <param name="position">The position to center on</param>
        public void LookAt( Vector2 position ) {
            var newCameraPosition = position * 100;
            Camera.x = newCameraPosition.x;
            Camera.y = newCameraPosition.y;
        }

        /// <summary>
        /// Makes sure that the given control is at the center of the window
        /// </summary>
        /// <param name="control">The control to center on</param>
        public void LookAt( ExtendedControl control ) {
            LookAt( control.Position );
        }

        private struct KeyboardShortcut {
            public KeyCode Key;
            public Action Callback;
            public bool Control;
            public bool Alt;
            public bool Shift;
        }

        /// <summary>
        /// The asset manager for this window
        /// </summary>
        [JsonProperty]
        public ExtendedAssets Assets;

        /// <summary>
        /// The editor this window is added to
        /// </summary>
        [JsonIgnore]
        public ExtendedEditor Editor;

        /// <summary>
        /// The settings that apply to this window
        /// </summary>
        [JsonProperty]
        public ExtendedWindowSettings Settings;

        /// <summary>
        /// Is the window initialized
        /// </summary>
        [JsonProperty]
        public bool IsInitialized = false;

        /// <summary>
        /// The content to draw on the top of the window
        /// </summary>
        [JsonProperty]
        public GUIContent WindowContent = new GUIContent();

        /// <summary>
        /// The window ID
        /// </summary>
        [JsonProperty]
        public int WindowID = -1;

        /// <summary>
        /// The rectangle used for drawing the window
        /// </summary>
        [JsonProperty]
        public Rect WindowRect = new Rect();

        /// <summary>
        /// The GUIStyle for the window
        /// </summary>
        [JsonProperty]
        public GUIStyle WindowStyle = null;

        /// <summary>
        /// The position of the window inside the editor
        /// </summary>
        [JsonIgnore]
        public Vector2 Position {
            get {
                return WindowRect.position;
            }
            set {
                Settings.IsFullscreen = false;
                WindowRect.position = value;
            }
        }

        /// <summary>
        /// The size of the window
        /// </summary>
        [JsonIgnore]
        public Vector2 Size {
            get {
                return WindowRect.size;
            }
            set {
                Settings.IsFullscreen = false;
                WindowRect.size = value;
            }
        }

        /// <summary>
        /// The camera that is used for panning in the window
        /// </summary>
        [JsonIgnore]
        public Vector3 Camera = new Vector3( 0, 0, 1 );

        /// <summary>
        /// The matrix that is used for scaling the contents of this window
        /// </summary>
        [JsonIgnore]
        public Matrix4x4 ScaleMatrix = Matrix4x4.identity;

        /// <summary>
        /// The input manager for this window
        /// </summary>
        [JsonIgnore]
        public ExtendedInput Input = new ExtendedInput();

        /// <summary>
        /// The active controls in this window
        /// </summary>
        [JsonProperty]
        protected List<ExtendedControl> Controls = new List<ExtendedControl>();

        private List<ExtendedControl> controlsToProcess = new List<ExtendedControl>();

        private List<ExtendedControl> controlsToRemove = new List<ExtendedControl>();

        private Dictionary<Type, List<ExtendedControl>> controlsDict = new Dictionary<Type, List<ExtendedControl>>();

        private Rect nonFullScreenRect;

        private bool initializedGUI = false;

        private const int cameraSpeed = 500;

        [JsonProperty]
        private List<KeyboardShortcut> keyboardShortcuts = new List<KeyboardShortcut>();

        private List<ExtendedNotification> notifications = new List<ExtendedNotification>();

        private GUIStyle notificationBackgroundStyle;

        private GUIStyle notificationTextStyle;

        private GUIStyle closeButtonStyle;

        private GUIStyle maximizeButtonStyle;

        /// <summary>
        /// Creates a new instance of ExtendedWindow with default settings
        /// </summary>
        public ExtendedWindow() : this( new ExtendedWindowSettings() ) { }

        /// <summary>
        /// Creates a new instance of ExtendedWindow with the given settings
        /// </summary>
        /// <param name="settings">The settings to apply to this window</param>
        public ExtendedWindow( ExtendedWindowSettings settings ) {
            Settings = settings;
        }

        #region Initialization
        /// <summary>
        /// Called when the window is added to an editor
        /// </summary>
        public virtual void OnInitialize() {
            Assets = new ExtendedAssets( Settings.AssetPath, this );

            if ( Settings.UseOnSceneGUI ) {
                SceneView.onSceneGUIDelegate += InternalSceneGUI;
            }

            WindowRect = new Rect( 0, 0, Editor.position.size.x, Editor.position.size.y );
            IsInitialized = true;
        }

        /// <summary>
        /// Called the first time OnGUI is called on this window
        /// </summary>
        protected virtual void OnInitializeGUI() {
            notificationBackgroundStyle = new GUIStyle( "NotificationBackground" );
            notificationTextStyle = new GUIStyle( "NotificationText" );
            notificationTextStyle.padding = new RectOffset( 20, 20, 20, 20 );
            notificationTextStyle.fontSize = 17;

            if ( WindowStyle == null ) {
                WindowStyle = ExtendedGUI.DefaultWindowStyle;
            }

            closeButtonStyle = new GUIStyle();
            closeButtonStyle.normal.background = Editor.SharedAssets["CloseNormal"];
            closeButtonStyle.hover.background = Editor.SharedAssets["CloseActive"];

            maximizeButtonStyle = new GUIStyle();
            maximizeButtonStyle.normal.background = Editor.SharedAssets["MaximizeNormal"];
            maximizeButtonStyle.hover.background = Editor.SharedAssets["MaximizeActive"];

            initializedGUI = true;
        }

        /// <summary>
        /// Called when this window gets deserialized
        /// </summary>
        public virtual void OnDeserialized() {
            Assets = new ExtendedAssets( Settings.AssetPath, this );

            for ( int i = 0; i < Controls.Count; i++ ) {
                if ( Controls[i] != null ) {
                    Controls[i].Window = this;
                    Controls[i].OnDeserialized();
                }
            }

            RemoveBrokenControls();

            Controls.Sort();
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

        /// <summary>
        /// Called when this window or the editor gets closed
        /// </summary>
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

        /// <summary>
        /// Called when this window gets focus
        /// </summary>
        public virtual void OnFocus() { }

        /// <summary>
        /// Called when this window loses focus
        /// </summary>
        public virtual void OnLostFocus() { }

        /// <summary>
        /// Called 100 times per second
        /// </summary>
        /// <param name="windowHasFocus">Does this window have focus</param>
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
                    if ( Input.Type == EventType.MouseDrag ) {
                        if ( Input.KeyDown( KeyCode.LeftAlt, KeyCode.RightAlt ) ) {
                            if ( Input.ButtonDown( EMouseButton.Left ) ) {
                                Camera += new Vector3( -Input.MouseDelta.x, Input.MouseDelta.y, 0 ) / Camera.z;
                            } else if ( Input.ButtonDown( EMouseButton.Right ) ) {
                                var delta = Input.MouseDelta / 1000f;
                                Camera.z += delta.x;
                                Camera.z -= delta.y;

                                if ( Camera.z < 0.1f ) {
                                    Camera.z = 0.1f;
                                }
                            }
                        }
                    }


                    if ( Input.Type == EventType.MouseDrag && Input.ButtonDown( EMouseButton.Middle ) ) {
                        Camera += new Vector3( -Input.MouseDelta.x, Input.MouseDelta.y, 0 ) / Camera.z;
                    }

                    if ( Input.KeyDown( KeyCode.LeftArrow ) ) {
                        Camera.x -= ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
                    }
                    if ( Input.KeyDown( KeyCode.RightArrow ) ) {
                        Camera.x += ( cameraSpeed * ( 1f / Camera.z ) ) * Editor.DeltaTime;
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

                Controls.Sort();
            }

            Input.Update();
        }

        #region SceneGUI
        public void InternalSceneGUI( SceneView view ) {
            Handles.BeginGUI();
            OnSceneGUI( view );
            Handles.EndGUI();
        }

        /// <summary>
        /// Write your own SceneGUI logic here
        /// </summary>
        /// <param name="view">The current SceneView</param>
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

            currentWindow = this;

            var e = Editor.CurrentEvent;
            Input.OnGUI( e );

            if ( WindowRect.Contains( e.mousePosition ) ) {
                var pos = e.mousePosition;
                if ( Settings.DrawToolbar ) {
                    pos.y -= 18f;
                }

                switch ( e.type ) {
                    case EventType.ContextClick:
                        OnContextClick( pos );
                        break;
                    case EventType.DragPerform:
                        OnDragPerform( DragAndDrop.paths, pos );
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                        break;
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                        OnDragUpdate( DragAndDrop.paths, pos );
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

        private Color smallSubGridColor = new Color( 0.5f, 0.5f, 0.5f, 1 );
        private Color subGridColor = new Color( 0.5f, 0.5f, 0.5f, 0.1f );
        private Color mainGridColor = new Color( 0.5f, 0.5f, 0.5f, 1f );
        private Color masterGridColor = new Color( 0.5f, 0.5f, 0.5f, 1f );

        private float Range( float min, float max, float value ) {
            return Mathf.Clamp01( ( value - min ) / ( max - min ) );
        }

        private void DrawGrid( Color color, float multiplier = 1 ) {
            if ( color.a == 0 ) return;

            var hc = Handles.color;
            Handles.color = color;

            var size = new Vector2(
                Mathf.CeilToInt( Size.x / 2 / 100 / Camera.z ),
                Mathf.CeilToInt( Size.y / 2 / 100 / Camera.z ) );

            var tempM = multiplier;
            if ( multiplier > 1 )
                multiplier = 1;

            var step = ToWorldSize( new Vector2( multiplier, multiplier ) * 100 ) * Camera.z;
            var startGrid = new Vector2( -size.x, -size.y );
            var endGrid = new Vector2( size.x, size.y );

            var camPos = ToWorldSize( Camera );

            camPos.x = Mathf.Round( camPos.x * Camera.z );
            camPos.y = Mathf.Round( camPos.y * Camera.z );

            startGrid += camPos;
            endGrid += camPos;

            startGrid -= step;
            endGrid += step;

            for ( float x = startGrid.x; x < endGrid.x + step.x; x += step.x ) {
                var startPos = ToScreenPosition( new Vector2( x, startGrid.y ) );
                var endPos = ToScreenPosition( new Vector2( x, endGrid.y ) );

                if ( tempM > 1 ) {
                    if ( Mathf.Round( x ) % tempM == 0 ) {
                        Handles.DrawLine( startPos, endPos );
                    }
                } else {
                    Handles.DrawLine( startPos, endPos );
                }
            }

            for ( float y = startGrid.y; y < endGrid.y + step.y; y += step.y ) {
                var startPos = ToScreenPosition( new Vector2( startGrid.x, y ) );
                var endPos = ToScreenPosition( new Vector2( endGrid.x, y ) );

                if ( tempM > 1 ) {
                    if ( Mathf.Round( y ) % tempM == 0 ) {
                        Handles.DrawLine( startPos, endPos );
                    }
                } else {
                    Handles.DrawLine( startPos, endPos );
                }
            }

            Handles.color = hc;
        }

        public void BeginGUI() {
            if ( Settings.UseCamera ) {
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
                area.y += WindowStyle.padding.top + 1;
                area.height -= WindowStyle.padding.top + 1;
                mousePosition.y -= WindowStyle.padding.top + 1;
            }
            if ( Settings.DrawToolbar ) {
                area.y += 18;
                area.height -= 18;
                mousePosition.y -= 18;
            }
            Input.MousePosition = mousePosition;

            GUILayout.BeginArea( area );
            ExtendedGUI.BeginArea( new ExtendedGUIOption() { Type = ExtendedGUIOption.EType.WindowSize, Value = area.size } );

            if ( Settings.DrawGrid ) {
                smallSubGridColor.a = 0.8f * Range( 5, 20, Camera.z );
                subGridColor.a = 0.8f * Range( 0.75f, 5, Camera.z );
                mainGridColor.a = 0.8f * Range( 0.15f, 1.5f, Camera.z );
                masterGridColor.a = 0.8f * ( 1 - Range( 0, 0.75f, Camera.z ) );

                DrawGrid( smallSubGridColor, 0.0625f );
                DrawGrid( subGridColor, 0.25f );
                DrawGrid( mainGridColor );
                DrawGrid( masterGridColor, 3f );
            }

            foreach ( var item in controlsToProcess ) {
                item.OnGUI();
            }
        }

        /// <summary>
        /// Write your own toolbar logic here
        /// </summary>
        public virtual void OnToolbarGUI() { }

        /// <summary>
        /// Write your own GUI logic here
        /// </summary>
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

            if ( Input.Type == EventType.ScrollWheel && Settings.UseCamera ) {
                var delta = Input.ScrollDelta.y;
                if ( delta > 0 ) {
                    Camera.z *= 0.9f;
                } else if ( delta < 0 ) {
                    Camera.z *= 1.1f;
                }

                if ( Camera.z < 0.1f ) {
                    Camera.z = 0.1f;
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

            var shortcuts = new List<KeyboardShortcut>( keyboardShortcuts );
            foreach ( var item in shortcuts ) {
                if ( item.Control != Input.KeysDown( KeyCode.LeftControl, KeyCode.RightControl ) ) continue;
                if ( item.Alt != Input.KeysDown( KeyCode.LeftAlt, KeyCode.RightAlt ) ) continue;
                if ( item.Shift != Input.KeysDown( KeyCode.LeftShift, KeyCode.RightShift ) ) continue;

                if ( Input.KeyReleased( item.Key ) ) {
                    item.Callback.Invoke();
                }
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Adds a control to the window
        /// </summary>
        /// <param name="control">The control to add</param>
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

            Controls.Sort();
        }

        /// <summary>
        /// Removes a control from the window
        /// </summary>
        /// <param name="control">The control to remove</param>
        public virtual void RemoveControl( ExtendedControl control ) {
            if ( controlsToRemove.Contains( control ) ) return;
            controlsToRemove.Add( control );
        }

        /// <summary>
        /// Removes all controls from the window
        /// </summary>
        public virtual void ClearControls() {
            controlsToRemove.AddRange( Controls );
        }

        /// <summary>
        /// Returns a list of controls of the given type, including controls that inherit from this type
        /// </summary>
        /// <returns>List of T</returns>
        public List<T> GetControlsByType<T>() where T : ExtendedControl {
            var type = typeof( T );
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

        /// <summary>
        /// Returns a list of controls of the given type
        /// </summary>
        /// <param name="type">The type of the control to return</param>
        /// <returns>List of ExtendedControl</returns>
        public List<ExtendedControl> GetControlsByType( Type type ) {
            if ( controlsDict.ContainsKey( type ) ) {
                return controlsDict[type];
            } else {
                return new List<ExtendedControl>();
            }
        }

        /// <summary>
        /// Returns a list of controls of the given type, including controls that inherit from this type
        /// </summary>
        /// <returns>List of T</returns>
        public List<T> GetControlsByBaseType<T>() where T : ExtendedControl {
            var type = typeof( T );
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

        /// <summary>
        /// Returns a list of controls of the given type, including controls that inherit from this type
        /// </summary>
        /// <param name="type">The type of the control to return</param>
        /// <returns>List of ExtendedControl</returns>
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

        public void SortControls() {
            Controls.Sort();
        }
        #endregion

        #region Events
        /// <summary>
        /// Invoked when a ContextClick event occurs
        /// </summary>
        /// <param name="position">The location of the right-mouse click</param>
        public void OnContextClick( Vector2 position ) {
            bool used = false;
            OnContextClick( position, ref used );
        }

        /// <summary>
        /// Invoked when a ContextClick event occurs
        /// </summary>
        /// <param name="position">The location of the right-mouse click</param>
        /// <param name="used">-</param>
        public virtual void OnContextClick( Vector2 position, ref bool used ) {
            for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
                controlsToProcess[i].OnContextClick( position, ref used );
            }
        }

        /// <summary>
        /// Invoked when a DragExited event occurs
        /// </summary>
        public virtual void OnDragExited() {
            for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
                controlsToProcess[i].OnDragExited();
            }
        }

        /// <summary>
        /// Invoked when a DragPerform event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the edito</param>
        /// <param name="position">The mouse position</param>
        public virtual void OnDragPerform( string[] paths, Vector2 position ) {
            for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
                if ( controlsToProcess[i].AllowDrop && controlsToProcess[i].Rectangle.Contains( position ) ) {
                    controlsToProcess[i].OnDragPerform( paths, position );
                }
            }
        }

        /// <summary>
        /// Invoked when a DragUpdate event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the editor</param>
        /// <param name="position">The mouse position</param>
        public virtual void OnDragUpdate( string[] paths, Vector2 position ) {
            for ( int i = controlsToProcess.Count - 1; i >= 0; i-- ) {
                if ( controlsToProcess[i].AllowDrop && controlsToProcess[i].Rectangle.Contains( position ) ) {
                    controlsToProcess[i].OnDragUpdate( paths, position );
                }
            }
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Shows a notification at the bottom-right corner of the window
        /// </summary>
        /// <param name="text">The text to display on the notification</param>
        public void ShowNotification( string text ) {
            ShowNotification( text, Color.white, 1.25f );
        }

        /// <summary>
        /// Shows a notification a the bottom-right corner of the window in red text and stays a bit longer than a normal notification
        /// </summary>
        /// <param name="text">The text to display on the notification</param>
        public void ShowErrorNotification( string text ) {
            ShowNotification( text, Color.red, 2f );
        }

        /// <summary>
        /// Shows a notification at the bottom-right corner of the window
        /// </summary>
        /// <param name="text">The text to display on the notification</param>
        /// <param name="color">The color of the notification</param>
        /// <param name="duration">The duration of the notification</param>
        public void ShowNotification( string text, Color color, float duration ) {
            if ( string.IsNullOrEmpty( text ) ) return;
            color.a = 0;
            notifications.Add( new ExtendedNotification( text, color, duration, notificationTextStyle ) );
        }
        #endregion

        #region KeyboardShortcuts
        /// <summary>
        /// Adds a keyboard shorcut hook to this window
        /// </summary>
        /// <param name="key">They main key to trigger the callback</param>
        /// <param name="callback">The callback to be invoked</param>
        /// <param name="control">Is control a modifier for this shortcut</param>
        /// <param name="alt">Is alt a modifier for this shortcut</param>
        /// <param name="shift">Is shift a modifier for this shortcut</param>
        public void AddShortcut( KeyCode key, Action callback, bool control, bool alt, bool shift ) {
            keyboardShortcuts.Add(
                new KeyboardShortcut() {
                    Key = key,
                    Callback = callback,
                    Control = control,
                    Alt = alt,
                    Shift = shift
                } );
        }

        /// <summary>
        /// Removes registered shortcuts based on the main key
        /// </summary>
        /// <param name="key">The main key for shortcuts</param>
        public void RemoveShortcut( KeyCode key ) {
            foreach ( var item in keyboardShortcuts ) {
                if ( item.Key == key ) {
                    keyboardShortcuts.Remove( item );
                    return;
                }
            }
        }

        /// <summary>
        /// Removes registered shortcuts based on the callback
        /// </summary>
        /// <param name="callback">The callback for the shortcuts</param>
        public void RemoveShortcut( Action callback ) {
            foreach ( var item in keyboardShortcuts ) {
                if ( item.Callback == callback ) {
                    keyboardShortcuts.Remove( item );
                    return;
                }
            }
        }

        /// <summary>
        /// Removes registered shortcuts based on the main key and modifiers
        /// </summary>
        /// <param name="key">The main key for shortcuts</param>
        /// <param name="control">Is control a modifier for this shortcut</param>
        /// <param name="alt">Is alt a modifier for this shortcut</param>
        /// <param name="shift">Is shift a modifier for this shortcut</param>
        public void RemoveShortcut( KeyCode key, bool control, bool alt, bool shift ) {
            foreach ( var item in keyboardShortcuts ) {
                if ( item.Key == key && item.Control == control && item.Alt == alt && item.Shift == shift ) {
                    keyboardShortcuts.Remove( item );
                    return;
                }
            }
        }

        /// <summary>
        /// Removes registered shortcuts based on the main key, callback, and modifiers
        /// </summary>
        /// <param name="key">The main key for shortcuts</param>
        /// <param name="callback">The callback for the shortcuts</param>
        /// <param name="control">Is control a modifier for this shortcut</param>
        /// <param name="alt">Is alt a modifier for this shortcut</param>
        /// <param name="shift">Is shift a modifier for this shortcut</param>
        public void RemoveShortcut( KeyCode key, Action callback, bool control, bool alt, bool shift ) {
            foreach ( var item in keyboardShortcuts ) {
                if ( item.Key == key && item.Callback == callback && item.Control == control && item.Alt == alt && item.Shift == shift ) {
                    keyboardShortcuts.Remove( item );
                    return;
                }
            }
        }
        /// <summary>
        /// Removes all the registered shortcuts
        /// </summary>
        public void ClearShortcuts() {
            keyboardShortcuts.Clear();
        }
        #endregion
    }
}
#endif