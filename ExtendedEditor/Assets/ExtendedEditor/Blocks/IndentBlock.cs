#if UNITY_EDITOR
using System;
using UnityEditor;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class IndentBlock : IDisposable {

		private int indent = 1;

		[DocsDescription("Creates a new instance of IndentBlock")]
		public IndentBlock() {
			EditorGUI.indentLevel += indent;
		}

		[DocsDescription("Creates a new instance of IndentBlock")]
		[DocsParameter("level", "The level of indentation")]
		public IndentBlock( int level ) {
			indent = level;
			EditorGUI.indentLevel += indent;
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUI.indentLevel -= indent;
		}
	}
}
#endif