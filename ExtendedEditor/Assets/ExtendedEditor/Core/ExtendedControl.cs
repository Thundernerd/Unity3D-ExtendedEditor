using System;
using System.Collections.Generic;
using TNRD.Editor.Serialization;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    public class ExtendedControl {

        [IgnoreSerialization]
        public ExtendedWindow Window;

        public Rect Rectangle {
            get {
                switch ( AnchorPoint ) {
                    case EAnchor.TopLeft:
                        return new Rect( Position, Size );
                    case EAnchor.TopCenter:
                        return new Rect( Position - new Vector2( Size.x / 2, 0 ), Size );
                    case EAnchor.TopRight:
                        return new Rect( Position - new Vector2( Size.x, 0 ), Size );
                    case EAnchor.MiddleLeft:
                        return new Rect( Position - new Vector2( 0, Size.y / 2 ), Size );
                    case EAnchor.MiddleCenter:
                        return new Rect( Position - ( Size / 2 ), Size );
                    case EAnchor.MiddleRight:
                        return new Rect( Position - new Vector2( Size.x, Size.y / 2 ), Size );
                    case EAnchor.BottomLeft:
                        return new Rect( Position - new Vector2( 0, Size.y ), Size );
                    case EAnchor.BottomCenter:
                        return new Rect( Position - new Vector2( Size.x / 2, Size.y ), Size );
                    case EAnchor.BottomRight:
                        return new Rect( Position - Size, Size );
                    default:
                        return new Rect( Position, Size );
                }
            }
        }

        public Vector2 Position;

        public Vector2 Size;

        public ExtendedAssets Assets {
            get { return Window.Assets; }
        }

        public ExtendedInput Input {
            get { return Window.Input; }
        }

        public EAnchor AnchorPoint = EAnchor.MiddleCenter;

        public string ID {
            get; private set;
        }

        [RequireSerialization]
        private bool initializedGUI;

        private List<Action> guiActions = new List<Action>();

        private void InternalInitialize() {
            ID = string.Format( "{0}_{1}", GetType().Name, Window.GetControlID() );

            OnInitialize();
        }

        private void InternalInitializeGUI() {
            OnInitializeGUI();
        }

        private void InternalBeforeSerialize() {
            OnBeforeSerialize();
        }

        private void InternalAfterDeserialize() {
            OnAfterSerialize();
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
                initializedGUI = true;
            }

            if ( guiActions.Count > 0 ) {
                var gActions = new List<Action>( guiActions );
                gActions.Reverse();
                guiActions.Clear();

                for ( int i = gActions.Count - 1; i >= 0; i-- ) {
                    gActions[i].Invoke();
                }
            }

            OnGUI();
        }

        private void InternalSceneGUI( SceneView view ) {
            OnSceneGUI( view );
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnInitializeGUI() { }

        protected virtual void OnBeforeSerialize() { }

        protected virtual void OnAfterSerialize() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnFocus() { }

        protected virtual void OnLostFocus() { }

        protected virtual void OnInspectorUpdate() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnGUI() { }

        protected virtual void OnSceneGUI( SceneView view ) { }

        public void Repaint() {
            Window.Repaint();
        }

        public void Remove() {
            Window.RemoveControl( this );
        }

        public void RunOnGUIThread( Action action ) {
            guiActions.Add( action );
        }

        public void RunOnGUIThreadImmediate( Action action ) {
            guiActions.Add( action );
            Window.Editor.Repaint();
        }
    }
}