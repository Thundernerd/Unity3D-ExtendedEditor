using System;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Windows {

    public class InputBox : ExtendedPopup {

        private Action<EDialogResult, string> callback;

        private EDialogResult result;
        private GUIStyle style;

        private string message = "";
        private string value = "";

        public InputBox( string message ) {
            this.message = message;
        }

        public InputBox( string message, Action<EDialogResult, string> callback ) {
            this.message = message;
            this.callback = callback;
        }

        public InputBox( string title, string message ) {
            WindowContent = new GUIContent( title );
            this.message = message;
        }

        public InputBox( string title, string message, Action<EDialogResult, string> callback ) {
            WindowContent = new GUIContent( title );
            this.message = message;
            this.callback = callback;
        }

        protected override void OnInitializeGUI() {
            style = new GUIStyle( GUI.skin.label );
            style.alignment = TextAnchor.UpperLeft;
            style.wordWrap = true;
        }

        protected override void OnGUI() {
            if ( string.IsNullOrEmpty( message ) ) {
                var rect = new Rect( 20, Size.y / 2 - 8, Size.x - 35, 16 );
                value = EditorGUI.TextField( rect, value );
            } else {
                var recta = new Rect( 20, Size.y / 2 - 20, Size.x - 35, 16 );
                EditorGUI.LabelField( recta, message, style );
                var rect = new Rect( 20, Size.y / 2 - 4, Size.x - 35, 16 );
                value = EditorGUI.TextField( rect, value );
            }

            if ( GUI.Button( new Rect( Size.x - 175, Size.y - 35, 75, 20 ), "OK" ) ) {
                result = EDialogResult.OK;
                Close();
            }
            if ( GUI.Button( new Rect( Size.x - 90, Size.y - 35, 75, 20 ), "Cancel" ) ) {
                result = EDialogResult.Cancel;
                Close();
            }

            if ( Input.KeyReleased( KeyCode.KeypadEnter ) || Input.KeyReleased( KeyCode.Return ) ) {
                result = EDialogResult.OK;
                Close();
            }

            if ( Input.KeyReleased( KeyCode.Escape ) ) {
                result = EDialogResult.Cancel;
                Close();
            }
        }

        public override void Close() {
            if ( callback != null ) {
                callback( result, value );
            }

            base.Close();
        }
    }
}