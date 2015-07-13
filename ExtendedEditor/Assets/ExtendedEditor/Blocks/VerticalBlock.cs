#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class VerticalBlock : IDisposable {

		[DocsDescription("Creats a new instance of VerticalBlock")]
		[DocsParameter("options", "The options to apply to the Vertical")]
		public VerticalBlock( params GUILayoutOption[] options ) {
			EditorGUILayout.BeginVertical( options );
		}

		[DocsDescription("Creats a new instance of VerticalBlock")]
		[DocsParameter("style", "The style to apply to the Vertical")]
		[DocsParameter("options", "The options to apply to the Vertical")]
		public VerticalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginVertical( style, options );
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUILayout.EndVertical();
		}
	}
}
#endif