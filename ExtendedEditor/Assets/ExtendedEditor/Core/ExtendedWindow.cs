using System;
using System.Collections.Generic;
using TNRD.Editor.Json;
using TNRD.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    [Serializable]
    public class ExtendedWindow {

        private static ReflectionData rData = new ReflectionData( typeof( ExtendedControl ) );

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
                var temp = value;
                temp.x = Mathf.Floor( temp.x );
                temp.y = Mathf.Floor( temp.y );
                WindowRect.position = temp;
            }
        }

        public Vector2 Size {
            get { return WindowRect.size; }
            set {
                WindowRect.size = value;
            }
        }

        public ExtendedWindowSettings WindowSettings;

        public EWindowStyle WindowStyle;

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
        private List<ExtendedControl> controls = new List<ExtendedControl>();

        [JsonProperty]
        private bool initializedGUI = false;

        private void InternalInitialize( int id ) {
            windowID = id;

            if ( rData == null ) {
                rData = new ReflectionData( typeof( ExtendedControl ) );
            }

            if ( Position == Vector2.zero && Size == Vector2.zero ) {
                WindowRect = new Rect( Vector2.zero, Editor.Size );
            }

            if ( WindowSettings == null ) {
                WindowSettings = new ExtendedWindowSettings();
            }

            SceneView.onSceneGUIDelegate -= InternalSceneGUI;
            SceneView.onSceneGUIDelegate += InternalSceneGUI;

            OnInitialize();
        }

        private void InternalInitializeGUI() {
            OnInitializeGUI();
            initializedGUI = true;
        }

        private void InternalDeserialized() {
            foreach ( var item in controls ) {
                item.Window = this;
                rData.Deserialized.Invoke( item, null );
            }

            OnDeserialized();
        }

        private void InternalDestroy() {
            SceneView.onSceneGUIDelegate -= InternalSceneGUI;

            OnDestroy();
        }

        private void InternalFocus() {
            OnFocus();

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.Focus.Invoke( controlsToProcess[i], null );
            }
        }

        private void InternalLostFocus() {
            OnLostFocus();

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.LostFocus.Invoke( controlsToProcess[i], null );
            }
        }

        private void InternalInspectorUpdate() {
            OnInspectorUpdate();

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.InspectorUpdate.Invoke( controlsToProcess[i], null );
            }
        }

        private void InternalUpdate() {
            OnUpdate();

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.Update.Invoke( controlsToProcess[i], null );
            }
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
                if ( size.x != Editor.Size.x || size.y != Editor.Size.y ) {
                    WindowRect.size = Editor.Size;
                }
            }

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.GUI.Invoke( controlsToProcess[i], null );
            }

            OnGUI();

            if ( WindowSettings.Draggable ) {
                var rect = new Rect( 0, 0, Size.x, 17f );
                GUI.DragWindow( rect );
                EditorGUIUtility.AddCursorRect( rect, MouseCursor.Pan );
            }
        }

        private void InternalSceneGUI( SceneView view ) {
            OnSceneGUI( view );

            var controlsToProcess = new List<ExtendedControl>( controls );
            var param = new object[] { view };
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.SceneGUI.Invoke( controlsToProcess[i], param );
            }
        }

        protected virtual void OnInitialize() {

        }

        protected virtual void OnInitializeGUI() {

        }

        protected virtual void OnDeserialized() {

        }

        protected virtual void OnDestroy() {

        }

        protected virtual void OnFocus() {

        }

        protected virtual void OnLostFocus() {

        }

        protected virtual void OnGUI() {

        }

        protected virtual void OnSceneGUI( SceneView view ) {

        }

        protected virtual void OnInspectorUpdate() {

        }

        protected virtual void OnUpdate() {

        }

        public void ShowPopup( ExtendedPopup popup ) {
            Editor.ShowPopup( popup );
        }

        public void RemovePopup() {
            Editor.RemovePopup();
        }

        public void AddWindow( ExtendedWindow window ) {
            Editor.AddWindow( window );
        }

        public void RemoveWindow( ExtendedWindow window ) {
            Editor.RemoveWindow( window );
        }

        public void AddControl( ExtendedControl control ) {
            control.Window = this;

            rData.Initialize.Invoke( control, null );
            controls.Add( control );
        }

        public void RemoveControl( ExtendedControl control ) {
            rData.Destroy.Invoke( control, null );
            controls.Remove( control );
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