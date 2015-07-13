#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class HorizontalBlock : IDisposable {

		[DocsDescription("Creates a new instance of a HorizontalBlock")]
		[DocsParameter("options", "The options to apply to the Horizontal")]
		public HorizontalBlock( params GUILayoutOption[] options ) {
			EditorGUILayout.BeginHorizontal( options );
		}

		[DocsDescription("Creates a new instance of a HorizontalBlock")]
		[DocsParameter("style", "The style to apply to the Horizontal")]
		[DocsParameter("options", "The options to apply to the Horizontal")]
		public HorizontalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginHorizontal( style, options );
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif