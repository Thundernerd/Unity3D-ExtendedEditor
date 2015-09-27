#if UNITY_EDITOR
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Base class for modal windows that can be added to ExtendedWindows
    /// </summary>
    public class ExtendedModalWindow {

        /// <summary>
        /// The editor this modal window is shown in
        /// </summary>
        public ExtendedEditor Editor;

        /// <summary>
        /// The result of a closed modal window
        /// </summary>
        public EExtendedModalWindowResult Result { get; private set; }

        /// <summary>
        /// Is the modal window ready to close
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// Can the modal window be dragged around
        /// </summary>
        public bool IsDraggable { get; protected set; }

        /// <summary>
        /// The title of the modal window
        /// </summary>
        public string Title = "";

        /// <summary>
        /// The rectangle used to draw the modal window
        /// </summary>
        public Rect WindowRect = new Rect();

        /// <summary>
        /// The GUIStyle for the modal window
        /// </summary>
        public GUIStyle WindowStyle = null;

        /// <summary>
        /// The input manager
        /// </summary>
        public ExtendedInput Input { get { return Editor.Input; } }

        /// <summary>
        /// Should the modal window be aligned to the center of the editor
        /// </summary>
        protected bool shouldAlignToCenter = true;

        /// <summary>
        /// Should the modal window show the OK/Accept button
        /// </summary>
        protected bool showOKButton = false;

        /// <summary>
        /// Should the modal window show the Cancel/Close button
        /// </summary>
        protected bool showCancelButton = false;

        /// <summary>
        /// The text to show on the OK/Accept button
        /// </summary>
        protected string textOKButton = "OK";

        /// <summary>
        /// The text to show on the Cancel/Close button
        /// </summary>
        protected string textCancelButton = "Cancel";

        private bool isInitialized = false;

        /// <summary>
        /// Creates an instance of ExtendedModalWindow
        /// </summary>
        public ExtendedModalWindow() { }

        /// <summary>
        /// Called the first time OnGUI is called on this modal window
        /// </summary>
        protected virtual void OnInitialize() {
            isInitialized = true;

            if ( WindowStyle == null ) {
                WindowStyle = ExtendedGUI.DefaultWindowStyle;
            }
        }

        /// <summary>
        /// Called when the modal window gets closed
        /// </summary>
        protected virtual void OnDestroy() {
            isInitialized = false;
        }

        /// <summary>
        /// Called 100 times per second
        /// </summary>
        /// <param name="windowHasFocus">Does the window this control is in have focus</param>
        public virtual void Update( bool windowHasFocus ) { }

        /// <summary>
        /// Implement your own GUI logic here
        /// </summary>
        public virtual void OnGUI( int id ) {
            if ( !isInitialized ) {
                OnInitialize();

                if ( shouldAlignToCenter ) {
                    AlignToCenter();
                }
            }

            if ( IsDraggable ) {
                GUI.DragWindow( new Rect( 0, 0, WindowRect.width, 17.5f ) );
            } else {
                if ( shouldAlignToCenter ) {
                    AlignToCenter();
                }
            }

            if ( showOKButton ) {
                if ( Input.KeyPressed( KeyCode.KeypadEnter ) || Input.KeyPressed( KeyCode.Return ) ) {
                    OK();
                }
            }
            if ( showCancelButton ) {
                if ( Input.KeyPressed( KeyCode.Escape ) ) {
                    Cancel();
                }
            }

            if ( showOKButton && showCancelButton ) {
                if ( GUI.Button( new Rect( WindowRect.width - 180, WindowRect.height - 30, 80, 20 ), textOKButton ) ) {
                    OK();
                }

                if ( GUI.Button( new Rect( WindowRect.width - 90, WindowRect.height - 30, 80, 20 ), textCancelButton ) ) {
                    Cancel();
                }
            } else if ( showOKButton || showCancelButton ) {
                var rect = new Rect( WindowRect.width - 90, WindowRect.height - 30, 80, 20 );

                if ( showOKButton ) {
                    if ( GUI.Button( rect, textOKButton ) ) {
                        OK();
                    }
                }

                if ( showCancelButton ) {
                    if ( GUI.Button( rect, textCancelButton ) ) {
                        Cancel();
                    }
                }
            }
        }

        /// <summary>
        /// Closes the modal window with the OK result
        /// </summary>
        public void OK() {
            Result = EExtendedModalWindowResult.OK;
            IsDone = true;
            Event.current.Use();
        }

        /// <summary>
        /// Closes the modal window with the Cancel result
        /// </summary>
        public void Cancel() {
            Result = EExtendedModalWindowResult.Cancel;
            IsDone = true;
            Event.current.Use();
        }

        /// <summary>
        /// Aligns the modal window to the center of the editor
        /// </summary>
        public void AlignToCenter() {
            var esize = Editor.position.size / 2;
            var csize = WindowRect.size / 2;
            WindowRect.position = esize - csize;
        }

        #region Events

        /// <summary>
        /// Invoked when a ContextClick event occurs
        /// </summary>
        /// <param name="position">The location of the right-mouse click</param>
        public virtual void OnContextClick( Vector2 position ) { }

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
        public virtual void OnDragUpdate( string[] paths, Vector2 position ) { }
        #endregion
    }
}
#endif