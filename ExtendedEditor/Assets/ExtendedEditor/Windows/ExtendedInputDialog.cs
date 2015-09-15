#if UNITY_EDITOR
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Windows {

    /// <summary>
    /// A simple input dialog
    /// </summary>
    public class ExtendedInputDialog : ExtendedModalWindow {

        /// <summary>
        /// The input text in this dialog
        /// </summary>
        public string InputText = "";

        private string message = "";

        /// <summary>
        /// Creates a new instance of ExtendedInputDialog
        /// </summary>
        public ExtendedInputDialog() : base() { }

        /// <summary>
        /// Creates a new instance of ExtendedInputDialog
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message to show on the ExtendedDialogBox</param>
        public ExtendedInputDialog( string title, string message ) : this() {
            this.Title = title;
            this.message = message;
        }

        /// <summary>
        /// Creates a new instance of ExtendedInputDialog
        /// </summary>
        /// <param name="title">The title of the window</param>
        /// <param name="message">The message to show on the ExtendedDialogBox</param>
        /// <param name="input">The input to already show in the inputbox</param>
        public ExtendedInputDialog( string title, string message, string input ) : this( title, message ) {
            this.InputText = input;
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            showOKButton = true;
            showCancelButton = true;

            IsDraggable = true;
            shouldAlignToCenter = true;

            WindowRect = new Rect( 0, 0, 350, 120 );
        }

        public override void OnGUI( int id ) {
            base.OnGUI( id );

            GUI.Label( new Rect( 20, 35, WindowRect.width - 40, 20 ), message );

            GUI.SetNextControlName( "eidTextField" );
            var rect = new Rect( 20, 55, WindowRect.width - 40, 20 );
            InputText = GUI.TextField( rect, InputText );
            GUI.FocusControl( "eidTextField" );
        }
    }
}
#endif