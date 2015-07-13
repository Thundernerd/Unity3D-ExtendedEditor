#if UNITY_EDITOR
using System;
using UnityEditor;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class DisabledBlock : IDisposable {

		[DocsDescription("Creates a new instance of DisabledBlock")]
		[DocsParameter("disabled", "Should the controls within the group be disabled")]
		public DisabledBlock( bool disabled ) {
			EditorGUI.BeginDisabledGroup( disabled );			
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUI.EndDisabledGroup();
		}
	}
}
#endif