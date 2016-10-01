#if UNITY_EDITOR
﻿using System;
using System.Collections.Generic;
using System.Linq;
using TNRD.Editor.Serialization;
using TNRD.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor {

    public class ExtendedWindow {

        private static ReflectionData rData = new ReflectionData( typeof( ExtendedControl ) );

        public GUIContent WindowContent = new GUIContent();

        public Rect WindowRect = new Rect();

        public int WindowID {
            get { return windowID; }
        }

        [RequireSerialization]
        private int windowID = 0;
        [RequireSerialization]
        private int controlID = 0;

        public Vector2 Position {
            get { return WindowRect.position; }
            set {
                var temp = value;
                temp.x = Mathf.Floor( temp.x );
                temp.y = Mathf.Floor( temp.y );
                WindowRect.position = temp;
                WindowSettings.IsFullscreen = false;
            }
        }

        public Vector2 Size {
            get { return WindowRect.size; }
            set {
                WindowRect.size = value;
                WindowSettings.IsFullscreen = false;
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

        public float DeltaTime {
            get {
                return Editor.DeltaTime;
            }
        }

        /// <summary>
        /// Setting this to true will sort the controls based on their SortingOrder in the next OnGUI run
        /// </summary>
        [IgnoreSerialization]
        public bool ShouldSortControls = false;

        [IgnoreSerialization]
        public ExtendedEditor Editor;

        [RequireSerialization]
        private List<ExtendedControl> controls = new List<ExtendedControl>();

        private Dictionary<Type, List<ExtendedControl>> controlsGrouped = new Dictionary<Type, List<ExtendedControl>>();

        private List<Action> guiActions = new List<Action>();

        [RequireSerialization]
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

        private void InternalBeforeSerialize() {
            foreach ( var item in controls ) {
                item.Window = this;
                rData.BeforeSerialize.Invoke( item, null );
            }

            OnBeforeSerialize();
        }

        private void InternalAfterDeserialize() {
            controls = controls.Where( c => c != null ).ToList();

            foreach ( var item in controls ) {
                AddControlGrouped( item );
            }

            foreach ( var item in controls ) {
                item.Window = this;
            }

            OnAfterSerialized();

            var ctrls = new List<ExtendedControl>( controls );
            foreach ( var item in ctrls ) {
                rData.AfterDeserialize.Invoke( item, null );
            }
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

            if ( ShouldSortControls && Event.current.type == EventType.Layout ) {
                ShouldSortControls = false;
                SortControls();
                Repaint();
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

            if ( guiActions.Count > 0 ) {
                var gActions = new List<Action>( guiActions );
                gActions.Reverse();
                guiActions.Clear();

                for ( int i = gActions.Count - 1; i >= 0; i-- ) {
                    gActions[i].Invoke();
                }
            }

            var controlsToProcess = new List<ExtendedControl>( controls );
            for ( int i = 0; i < controlsToProcess.Count; i++ ) {
                rData.GUI.Invoke( controlsToProcess[i], null );
            }

            OnGUI();

            if ( WindowSettings.Draggable ) {
                var rect = new Rect( 0, 0, Size.x - 30, 17f );
                GUI.DragWindow( rect );
                EditorGUIUtility.AddCursorRect( rect, MouseCursor.Pan );
            }

            if ( WindowSettings.DrawTitleBarButtons ) {
                if ( WindowStyle == EWindowStyle.Default || WindowStyle == EWindowStyle.DefaultUnity ) {
                    var rect = new Rect( new Vector2( Size.x - 13, 1 ), new Vector2( 13, 13 ) );
                    if ( GUI.Button( rect, "", ExtendedGUI.CloseButtonStyle ) ) {
                        Editor.RemoveWindow( this );
                    }

                    rect.x -= 13;
                    if ( GUI.Button( rect, "", ExtendedGUI.MaximizeButtonStyle ) ) {
                        WindowSettings.IsFullscreen = !WindowSettings.IsFullscreen;
                    }
                }
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

        protected virtual void OnInitialize() { }

        protected virtual void OnInitializeGUI() { }

        protected virtual void OnBeforeSerialize() { }

        protected virtual void OnAfterSerialized() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnFocus() { }

        protected virtual void OnLostFocus() { }

        protected virtual void OnGUI() { }

        protected virtual void OnSceneGUI( SceneView view ) { }

        protected virtual void OnInspectorUpdate() { }

        protected virtual void OnUpdate() { }

        public void ShowNotification( string text ) {
            AddControl( new ExtendedNotification( text, false ) );
        }

        public void ShowNotificationError( string text ) {
            AddControl( new ExtendedNotification( text, true ) );
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

        public ExtendedWindow GetWindow( Type type ) {
            return Editor.GetWindowByType( type );
        }

        public T GetWindow<T>() where T : ExtendedWindow {
            return Editor.GetWindowByType<T>();
        }

        public List<ExtendedWindow> GetWindows( Type type ) {
            return Editor.GetWindowsByType( type );
        }

        public List<T> GetWindows<T>() where T : ExtendedWindow {
            return Editor.GetWindowsByType<T>();
        }

        public void AddControl( ExtendedControl control ) {
            control.Window = this;

            rData.Initialize.Invoke( control, null );
            controls.Add( control );
            AddControlGrouped( control );
            ShouldSortControls = true;
        }

        public void RemoveControl( ExtendedControl control ) {
            rData.Destroy.Invoke( control, null );
            controls.Remove( control );
            RemoveControlGrouped( control );
        }

        private void AddControlGrouped( ExtendedControl control, Type wType = null ) {
            if ( control == null ) return;

            if ( wType == null ) {
                wType = control.GetType();
            }

            if ( !controlsGrouped.ContainsKey( wType ) ) {
                controlsGrouped.Add( wType, new List<ExtendedControl>() );
            }

            controlsGrouped[wType].Add( control );

            if ( wType.BaseType != null ) {
                AddControlGrouped( control, wType.BaseType );
            }
        }

        private void RemoveControlGrouped( ExtendedControl control, Type wType = null ) {
            if ( control == null ) return;

            if ( wType == null ) {
                wType = control.GetType();
            }

            if ( !controlsGrouped.ContainsKey( wType ) ) {
                return;
            }

            controlsGrouped[wType].Remove( control );

            if ( wType.BaseType != null ) {
                RemoveControlGrouped( control, wType.BaseType );
            }
        }

        public ExtendedControl GetControl( string id ) {
            return controls.Where( c => c.ID == id ).FirstOrDefault();
        }

        public T GetControl<T>( string id ) where T : ExtendedControl {
            return (T)controls.Where( c => c.ID == id ).FirstOrDefault();
        }

        public ExtendedControl GetControl( Type type ) {
            if ( controlsGrouped.ContainsKey( type ) ) {
                return controlsGrouped[type].FirstOrDefault();
            } else {
                return null;
            }
        }

        public T GetControl<T>() where T : ExtendedControl {
            var type = typeof( T );
            if ( controlsGrouped.ContainsKey( type ) ) {
                return (T)controlsGrouped[type].FirstOrDefault();
            } else {
                return null;
            }
        }

        public List<ExtendedControl> GetControls( Type type ) {
            if ( controlsGrouped.ContainsKey( type ) ) {
                return controlsGrouped[type];
            } else {
                return new List<ExtendedControl>();
            }
        }

        public List<T> GetControls<T>() where T : ExtendedControl {
            var type = typeof( T );
            if ( controlsGrouped.ContainsKey( type ) ) {
                return controlsGrouped[type].Cast<T>().ToList();
            } else {
                return new List<T>();
            }
        }

        public int GetControlID() {
            return controlID++;
        }

        public void Repaint() {
            Editor.Repaint();
        }

        public void Remove() {
            Editor.RemoveWindow( this );
        }

        public void RunOnGUIThread( Action action ) {
            guiActions.Add( action );
        }

        public void RunOnGUIThreadImmediate( Action action ) {
            guiActions.Add( action );
            Editor.Repaint();
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

        public void SortControls() {
            controls = controls.OrderBy( c => c.SortingOrder ).ToList();
        }
    }
}
#endif