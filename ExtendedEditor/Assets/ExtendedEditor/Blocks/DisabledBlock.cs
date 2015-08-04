#if UNITY_EDITOR
using System;
using UnityEditor;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    /// <summary>
    /// 
    /// </summary>
    public class DisabledBlock : IDisposable {

        /// <summary>
        /// Creates a new instance of DisabledBlock
        /// </summary>
        /// <param name="disabled">Should the controls within the group be disabled</param>
        public DisabledBlock( bool disabled ) {
            EditorGUI.BeginDisabledGroup( disabled );
        }

        public void Dispose() {
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif