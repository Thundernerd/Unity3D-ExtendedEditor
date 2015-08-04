#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    /// <summary>
    /// 
    /// </summary>
    public class VerticalBlock : IDisposable {


        /// <summary>
        /// Creates a new instance of VerticalBlock
        /// </summary>
        /// <param name="options"The options to apply to the Vertical></param>
        public VerticalBlock( params GUILayoutOption[] options ) {
            EditorGUILayout.BeginVertical( options );
        }

        /// <summary>
        /// Creates a new instance of VerticalBlock
        /// </summary>
        /// <param name="style">The style to apply to the Vertical</param>
        /// <param name="options"The options to apply to the Vertical></param>
        public VerticalBlock( GUIStyle style, params GUILayoutOption[] options ) {
            EditorGUILayout.BeginVertical( style, options );
        }

        public void Dispose() {
            EditorGUILayout.EndVertical();
        }
    }
}
#endif