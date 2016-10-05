#if UNITY_EDITOR
using System;
using UnityEditor;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    public class IndentBlock : IDisposable {

        private int indent = 1;

        public IndentBlock() {
            EditorGUI.indentLevel += indent;
        }

        public IndentBlock( int level ) {
            indent = level;
            EditorGUI.indentLevel += indent;
        }

        public void Dispose() {
            EditorGUI.indentLevel -= indent;
        }
    }
}
#endif