using TNRD.Editor.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    public class ExtendedPopup {

        public GUIContent WindowContent = new GUIContent();

        public Rect WindowRect = new Rect();

        public int WindowID {
            get { return windowID; }
        }

        [JsonProperty]
        private int windowID = 0;

        public Vector2 Position {
            get { return WindowRect.position; }
            set {
                WindowRect.position = value;
            }
        }

        public Vector2 Size {
            get { return WindowRect.size; }
            set {
                WindowRect.size = value;
            }
        }

        public ExtendedAssets Assets {
            get { return Editor.Assets; }
        }

        public ExtendedInput Input {
            get {
                return Editor.Input;
            }
        }

        [JsonIgnore]
        public ExtendedEditor Editor;

        [JsonProperty]
        private bool initializedGUI = false;

        private void InternalInitialize( int id ) {
            windowID = id;

            Size = new Vector2( 400, 150 );

            OnInitialize();
        }

        private void InternalInitializeGUI() {
            OnInitializeGUI();
            initializedGUI = true;
        }

        private void InternalDestroy() {
            OnDestroy();
        }

        private void InternalUpdate() {
            OnUpdate();
        }

        private void InternalGUI() {
            if ( !initializedGUI ) {
                InternalInitializeGUI();
                Position = Editor.Size / 2 - Size / 2;
            }

            OnGUI();

            var rect = new Rect( 0, 0, Size.x, 17f );
            GUI.DragWindow( rect );
            EditorGUIUtility.AddCursorRect( rect, MouseCursor.Pan );
        }

        protected virtual void OnInitialize() {

        }

        protected virtual void OnInitializeGUI() {

        }

        protected virtual void OnDestroy() {

        }

        protected virtual void OnGUI() {

        }

        protected virtual void OnUpdate() {

        }

        public void Close() {
            Editor.RemovePopup();
        }
    }
}