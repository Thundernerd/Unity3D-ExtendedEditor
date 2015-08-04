#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    /// <summary>
    /// 
    /// </summary>
    public class HorizontalBlock : IDisposable {

        /// <summary>
        /// Creates a new instance of a HorizontalBlock
        /// </summary>
        /// <param name="options">The options to apply to the Horizontal</param>
        public HorizontalBlock( params GUILayoutOption[] options ) {
            EditorGUILayout.BeginHorizontal( options );
        }

        /// <summary>
        /// Creates a new instance of a HorizontalBlock
        /// </summary>
        /// <param name="style">The style to apply to the Horizontal</param>
        /// <param name="options">The options to apply to the Horizontal</param>
        public HorizontalBlock( GUIStyle style, params GUILayoutOption[] options ) {
            EditorGUILayout.BeginHorizontal( style, options );
        }

        public void Dispose() {
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif