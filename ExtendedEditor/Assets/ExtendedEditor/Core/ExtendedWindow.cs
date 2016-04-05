using System;
using TNRD.Editor.Json;
using UnityEngine;

namespace TNRD.Editor.Core {
    [Serializable]
    public class ExtendedWindow {

        public GUIContent WindowContent = new GUIContent();

        public Rect WindowRect = new Rect();

        public Vector2 Position {
            get { return WindowRect.position; }
            set { WindowRect.position = value; }
        }

        public Vector2 Size {
            get { return WindowRect.size; }
            set { WindowRect.size = value; }
        }

        public ExtendedWindowSettings WindowSettings;

        public EWindowStyle WindowStyle;

        [JsonIgnore]
        public ExtendedEditor Editor;

        private bool initializedGUI = false;

        private void InternalInitialize() {
            if ( Position == Vector2.zero && Size == Vector2.zero ) {
                WindowRect = new Rect( Vector2.zero, Editor.Size );
            }

            if ( WindowSettings == null ) {
                WindowSettings = new ExtendedWindowSettings();
            }

            OnInitialize();
        }

        private void InternalInitializeGUI() {
            OnInitializeGUI();
            initializedGUI = true;
        }

        private void InternalDestroy() {
            OnDestroy();
        }

        private void InternalFocus() {
            OnFocus();
        }

        private void InternalLostFocus() {
            OnLostFocus();
        }

        private void InternalInspectorUpdate() {
            OnInspectorUpdate();
        }

        private void InternalUpdate() {
            OnUpdate();
        }

        private void InternalGUI() {
            if ( !initializedGUI ) {
                InternalInitializeGUI();
            }

            if ( WindowSettings.IsFullscreen ) {
                var pos = WindowRect.position;
                if ( pos.x != 0 || pos.y != 0 ) {
                    WindowRect.position = new Vector2();
                }

                var size = WindowRect.size;
                if ( size.x != Editor.Size.x || size.y != Size.y ) {
                    WindowRect.size = Editor.Size;
                }
            }

            OnGUI();
        }

        protected virtual void OnInitialize() {

        }

        protected virtual void OnInitializeGUI() {

        }

        protected virtual void OnDestroy() {

        }

        protected virtual void OnFocus() {

        }

        protected virtual void OnLostFocus() {

        }

        protected virtual void OnGUI() {

        }

        protected virtual void OnInspectorUpdate() {

        }

        protected virtual void OnUpdate() {

        }

        public static ExtendedEditor CreateEditor() {
            return CreateEditor( "" );
        }

        public static ExtendedEditor CreateEditor( string title ) {
            var stack = new System.Diagnostics.StackTrace();
            if ( stack.FrameCount >= 1 ) {
                var mBase = stack.GetFrame( 1 ).GetMethod();
                var type = mBase.DeclaringType;
                var instance = (ExtendedWindow)Activator.CreateInstance( type );
                return ExtendedEditor.CreateEditor( title, instance );
            } else {
                throw new Exception( "Unable to create editor" );
            }
        }
    }
}