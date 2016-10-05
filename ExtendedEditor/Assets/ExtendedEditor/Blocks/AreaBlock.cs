#if UNITY_EDITOR
using System;
using UnityEngine;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    public class AreaBlock : IDisposable {

        public AreaBlock( Rect screenRect ) {
            GUILayout.BeginArea( screenRect );
        }

        public AreaBlock( Rect screenRect, string text ) {
            GUILayout.BeginArea( screenRect, text );
        }

        public AreaBlock( Rect screenRect, Texture image ) {
            GUILayout.BeginArea( screenRect, image );
        }

        public AreaBlock( Rect screenRect, GUIContent content ) {
            GUILayout.BeginArea( screenRect, content );
        }

        public AreaBlock( Rect screenRect, GUIStyle style ) {
            GUILayout.BeginArea( screenRect, style );
        }

        public AreaBlock( Rect screenRect, string text, GUIStyle style ) {
            GUILayout.BeginArea( screenRect, text, style );
        }

        public AreaBlock( Rect screenRect, Texture image, GUIStyle style ) {
            GUILayout.BeginArea( screenRect, image, style );
        }

        public AreaBlock( Rect screenRect, GUIContent content, GUIStyle style ) {
            GUILayout.BeginArea( screenRect, content, style );
        }

        public void Dispose() {
            GUILayout.EndArea();
        }
    }
}
#endif