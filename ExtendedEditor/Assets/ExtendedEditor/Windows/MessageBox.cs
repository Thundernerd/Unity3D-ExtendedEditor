using System;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Windows {

    public class MessageBox : ExtendedPopup {

        private Action<EDialogResult> callback;

        private EMessageBoxButtons buttons = EMessageBoxButtons.OK;
        private EDialogResult result;
        private GUIStyle style;

        private string message = "";

        public MessageBox( string message ) {
            this.message = message;
        }

        public MessageBox( string message, Action<EDialogResult> callback ) {
            this.message = message;
            this.callback = callback;
        }

        public MessageBox( string title, string message ) {
            WindowContent = new GUIContent( title );
            this.message = message;
        }

        public MessageBox( string title, string message, Action<EDialogResult> callback ) {
            WindowContent = new GUIContent( title );
            this.message = message;
            this.callback = callback;
        }

        public MessageBox( string title, string message, EMessageBoxButtons buttons ) {
            WindowContent = new GUIContent( title );
            this.message = message;
            this.buttons = buttons;
        }

        public MessageBox( string title, string message, EMessageBoxButtons buttons, Action<EDialogResult> callback ) {
            WindowContent = new GUIContent( title );
            this.message = message;
            this.buttons = buttons;
            this.callback = callback;
        }

        public MessageBox( string message, EMessageBoxButtons buttons ) {
            this.message = message;
            this.buttons = buttons;
        }

        public MessageBox( string message, EMessageBoxButtons buttons, Action<EDialogResult> callback ) {
            this.message = message;
            this.buttons = buttons;
            this.callback = callback;
        }

        protected override void OnInitializeGUI() {
            style = new GUIStyle( GUI.skin.label );
            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;
        }

        protected override void OnGUI() {
            var rect = new Rect( 20, 20, Size.x - 35, Size.y - 60 );
            EditorGUI.LabelField( rect, message, style );

            switch ( buttons ) {
                case EMessageBoxButtons.OK:
                    if ( GUI.Button( new Rect( Size.x - 90, Size.y - 35, 75, 20 ), "OK" ) ) {
                        result = EDialogResult.OK;
                        Close();
                    }
                    break;
                case EMessageBoxButtons.OKCancel:
                    if ( GUI.Button( new Rect( Size.x - 175, Size.y - 35, 75, 20 ), "OK" ) ) {
                        result = EDialogResult.OK;
                        Close();
                    }
                    if ( GUI.Button( new Rect( Size.x - 90, Size.y - 35, 75, 20 ), "Cancel" ) ) {
                        result = EDialogResult.Cancel;
                        Close();
                    }
                    break;
                case EMessageBoxButtons.YesNo:
                    if ( GUI.Button( new Rect( Size.x - 175, Size.y - 35, 75, 20 ), "Yes" ) ) {
                        result = EDialogResult.Yes;
                        Close();
                    }
                    if ( GUI.Button( new Rect( Size.x - 90, Size.y - 35, 75, 20 ), "No" ) ) {
                        result = EDialogResult.No;
                        Close();
                    }
                    break;
                case EMessageBoxButtons.YesNoCancel:
                    if ( GUI.Button( new Rect( Size.x - 260, Size.y - 35, 75, 20 ), "Yes" ) ) {
                        result = EDialogResult.Yes;
                        Close();
                    }
                    if ( GUI.Button( new Rect( Size.x - 175, Size.y - 35, 75, 20 ), "No" ) ) {
                        result = EDialogResult.No;
                        Close();
                    }
                    if ( GUI.Button( new Rect( Size.x - 90, Size.y - 35, 75, 20 ), "Cancel" ) ) {
                        result = EDialogResult.Cancel;
                        Close();
                    }
                    break;
            }
        }

        public override void Close() {
            if ( callback != null ) {
                callback( result );
            }

            base.Close();
        }
    }
}