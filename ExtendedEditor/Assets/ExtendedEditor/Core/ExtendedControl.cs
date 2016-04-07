using System;
using TNRD.Editor.Json;
using UnityEngine;

namespace TNRD.Editor.Core {

    [Serializable]
    public class ExtendedControl {

        [JsonIgnore]
        [NonSerialized]
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

        [JsonProperty]
        private bool initializedGUI;

        private void InternalInitialize() {
            OnInitialize();
        }

        private void InternalInitializeGUI() {
            OnInitializeGUI();
        }

        private void InternalDeserialized() {
            OnDeserialized();
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

            OnGUI();
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnInitializeGUI() { }

        protected virtual void OnDeserialized() { }

        protected virtual void OnDestroy() { }

        protected virtual void OnFocus() { }

        protected virtual void OnLostFocus() { }

        protected virtual void OnInspectorUpdate() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnGUI() { }
    }
}