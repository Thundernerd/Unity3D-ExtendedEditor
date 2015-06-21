#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {
	public class ToggleBlock : IDisposable {

		public ToggleBlock( string label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		public ToggleBlock( GUIContent label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		public void Dispose() {
			EditorGUILayout.EndToggleGroup();
		}
	}
}
#endif