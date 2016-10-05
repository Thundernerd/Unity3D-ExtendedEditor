#if UNITY_EDITOR
using System;
using UnityEditor;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {
    
    public class DisabledGroupBlock : IDisposable {
        
        public DisabledGroupBlock( bool disabled ) {
            EditorGUI.BeginDisabledGroup( disabled );
        }

        public void Dispose() {
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif