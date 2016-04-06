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
            get { return new Rect( Position, Size ); }
        }

        public Vector2 Position;

        public Vector2 Size;

        public ExtendedAssets Assets {
            get { return Window.Assets; }
        }

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