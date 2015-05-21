#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace TNRD {
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