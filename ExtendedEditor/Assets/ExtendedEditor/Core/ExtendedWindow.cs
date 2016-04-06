using System;
using System.Collections.Generic;
using TNRD.Editor.Json;
using TNRD.Editor.Utilities;
using UnityEngine;

namespace TNRD.Editor.Core {

    [Serializable]
    public class ExtendedWindow {

        private static ReflectionData rData = new ReflectionData( typeof( ExtendedControl ) );

        public GUIContent WindowContent = new GUIContent();

        public Rect WindowRect = new Rect();

        public Vector2 Position {
            get { return WindowRect.position; }
            set {
                WindowSettings.IsFullscreen = false;
                WindowRect.position = value;
            }
        }

        public Vector2 Size {
            get { return WindowRect.size; }
            set {
                WindowSettings.IsFullscreen = false;
                WindowRect.size = value;
            }
        }

        public ExtendedWindowSettings WindowSettings;

        public EWindowStyle WindowStyle;

        public ExtendedAssets Assets {
            get { return Editor.Assets; }
        }

        [JsonIgnore]
        public ExtendedEditor Editor;

        [JsonProperty]
        private List<ExtendedControl> controls = new List<ExtendedControl>();

        [JsonProperty]
        private bool initializedGUI = false;

        private void InternalInitialize() {
            if ( rData == null ) {
                rData = new ReflectionData( typeof( ExtendedControl ) );
            }

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

        private void InternalDeserialize() {
            foreach ( var item in controls ) {
                item.Window = this;
                rData.Deserialized.Invoke( item, null );
            }

            OnDeserialized();
        }

        private void InternalDestroy() {
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

        protected virtual void OnInspectorUpdate() {

        }

        protected virtual void OnUpdate() {

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