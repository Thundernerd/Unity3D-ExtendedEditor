#if UNITY_EDITOR
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Base class for controls that can be added to ExtendedWindows
    /// </summary>
    public class ExtendedControl {

        /// <summary>
        /// The window this control is added to
        /// </summary>
        [JsonIgnore]
        public ExtendedWindow Window;

        /// <summary>
        /// The input handler belonging to this control's window
        /// </summary>
        [JsonIgnore]
        public ExtendedInput Input { get { return Window.Input; } }

        /// <summary>
        /// Is the control initialized
        /// </summary>
        public bool IsInitialized;

        /// <summary>
        /// The position of the control in the window
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The size of the control
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// Gets or sets the edges of the window to which a control is bound and determines how a control is resized with its parent.
        /// </summary>
        public EAnchorStyles Anchor = EAnchorStyles.Top | EAnchorStyles.Left;

        /// <summary>
        /// The lower limit for the size of this control. To be used in combination with Anchor
        /// </summary>
        public Vector2 MinSize;

        /// <summary>
        /// The upper limit for the size of this control. To be used in combination with Anchor
        /// </summary>
        public Vector2 MaxSize;

        /// <summary>
        /// Gets or sets a value indicating whether the control can accept data that the user drags onto it
        /// </summary>
        public bool AllowDrop;

        /// <summary>
        /// The rectangle used for drawing in OnGUI
        /// </summary>
        [JsonIgnore]
        public Rect Rectangle {
            get {
                return new Rect(
                    ExtendedWindow.ToScreenPosition( Position - new Vector2( Size.x / 2, -Size.y / 2 ) ),
                    ExtendedWindow.ToScreenSize( Size ) );
                //var scaledPosition = Window.ScaleMatrix.MultiplyVector( Position + (Vector2)Window.Camera );
                //var scaledSize = Window.ScaleMatrix.MultiplyVector( Size );
                //return new Rect( scaledPosition.x, scaledPosition.y, scaledSize.x, scaledSize.y );
            }
        }

        [JsonProperty]
        private int controlHint = -1;

        private bool initializedGUI = false;

        private Vector2 previousWindowPosition;
        private Vector2 previousWindowSize;

        /// <summary>
        /// Creates a new instance of ExtendedControl
        /// </summary>
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

        /// <summary>
        /// Called when a window gets deserialized
        /// </summary>
        public virtual void OnDeserialized() {
            if ( controlHint == -1 ) {
                var t = GetType();
                controlHint = t.Name.GetHashCode();
            }
        }

        /// <summary>
        /// Called when the Window this control is located in gets closed
        /// </summary>
        public virtual void OnDestroy() {
            IsInitialized = false;
        }

        /// <summary>
        /// Called 100 times per second
        /// </summary>
        /// <param name="windowHasFocus">Does the window this control is in have focus</param>
        public virtual void Update( bool windowHasFocus ) { }

        /// <summary>
        /// Implement your own GUI logic here
        /// </summary>
        public virtual void OnGUI() {
            if ( !initializedGUI ) {
                OnInitializeGUI();

                if ( Window.Settings.IsFullscreen ) {
                    previousWindowPosition = Window.Editor.position.position;
                    previousWindowSize = Window.Editor.position.size;
                } else {
                    previousWindowPosition = Window.Position;
                    previousWindowSize = Window.Size;
                }
            }

            Vector2 currentWindowPos;
            Vector2 currentWindowSize;

            if ( Window.Settings.IsFullscreen ) {
                currentWindowPos = Window.Editor.position.position;
                currentWindowSize = Window.Editor.position.size;
            } else {
                currentWindowPos = Window.Position;
                currentWindowSize = Window.Size;
            }

            if ( currentWindowPos != previousWindowPosition || currentWindowSize != previousWindowSize ) {
                if ( ( Anchor & EAnchorStyles.Top ) != EAnchorStyles.Top ) {
                    if ( currentWindowPos.y != previousWindowPosition.y && currentWindowSize.y != previousWindowSize.y ) {
                        var d = currentWindowPos.y - previousWindowPosition.y;
                        Position.y -= d;
                        Size.y += d;
                    }
                }

                if ( ( Anchor & EAnchorStyles.Bottom ) == EAnchorStyles.Bottom ) {
                    if ( currentWindowSize.y != previousWindowSize.y ) {
                        var d = currentWindowSize.y - previousWindowSize.y;
                        Size.y += d;
                    }
                }

                if ( ( Anchor & EAnchorStyles.Left ) != EAnchorStyles.Left ) {
                    if ( currentWindowPos.x != previousWindowPosition.x && currentWindowSize.x != previousWindowSize.x ) {
                        var d = currentWindowPos.x - previousWindowPosition.x;
                        Position.x -= d;
                        Size.x += d;
                    }
                }

                if ( ( Anchor & EAnchorStyles.Right ) == EAnchorStyles.Right ) {
                    if ( currentWindowSize.x != previousWindowSize.x ) {
                        var d = currentWindowSize.x - previousWindowSize.x;
                        Size.x += d;
                    }
                }
            }

            previousWindowPosition = currentWindowPos;
            previousWindowSize = currentWindowSize;

            if ( MinSize.x > 0 && Size.x < MinSize.x ) {
                Size.x = MinSize.x;
            }
            if ( MinSize.y > 0 && Size.y < MinSize.y ) {
                Size.y = MinSize.y;
            }

            if ( MaxSize.x > 0 && Size.x > MaxSize.x ) {
                Size.x = MaxSize.x;
            }
            if ( MaxSize.y > 0 && Size.y > MaxSize.y ) {
                Size.y = MaxSize.y;
            }
        }

        /// <summary>
        /// Implement your own SceneGUI logic here
        /// </summary>
        /// <param name="view">The current SceneView</param>
        public virtual void OnSceneGUI( SceneView view ) { }

        /// <summary>
        /// Gets a unique ID for a control
        /// </summary>
        /// <param name="focus">-</param>
        /// <returns>-</returns>
        public int GetControlID( FocusType focus ) {
            return GUIUtility.GetControlID( controlHint, focus );
        }

        #region Events
        /// <summary>
        /// Invoked when a ContextClick event occurs
        /// </summary>
        /// <param name="position">The location of the right-mouse click</param>
        /// <param name="used">-</param>
        public virtual void OnContextClick( Vector2 position, ref bool used ) { }

        /// <summary>
        /// Invoked when a DragExited event occurs
        /// </summary>
        public virtual void OnDragExited() { }

        /// <summary>
        /// Invoked when a DragPerform event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the edito</param>
        /// <param name="position">The mouse position</param>
        public virtual void OnDragPerform( string[] paths, Vector2 position ) { }

        /// <summary>
        /// Invoked when a DragUpdate event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the editor</param>
        /// <param name="position">The mouse position</param>
        public virtual void OnDragUpdate( string[] paths, Vector2 position ) {
            if ( Rectangle.Contains( position ) ) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        #endregion
    }
}
#endif