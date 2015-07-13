#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class ToggleBlock : IDisposable {
		
		[DocsDescription("Creats a new instance of ToggleBlock")]
		[DocsParameter("label", "The label of the Toggle Group")]
		[DocsParameter("toggle", "The options to apply to the ScrollView")]
		public ToggleBlock( string label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		[DocsDescription("Creats a new instance of ToggleBlock")]
		[DocsParameter("label", "The label of the Toggle Group")]
		[DocsParameter("toggle", "The options to apply to the ScrollView")]
		public ToggleBlock( GUIContent label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUILayout.EndToggleGroup();
		}
	}
}
#endif