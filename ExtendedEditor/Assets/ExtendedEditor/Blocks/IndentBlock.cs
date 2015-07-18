#if UNITY_EDITOR
using System;
using UnityEditor;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	/// <summary>
	/// 
	/// </summary>
	public class IndentBlock : IDisposable {

		private int indent = 1;

		/// <summary>
		/// Creates a new instance of IndentBlock
		/// </summary>
		/// <param name="level">The level of indentation</param>
		public IndentBlock() {
			EditorGUI.indentLevel += indent;
		}

		/// <summary>
		/// Creates a new instance of IndentBlock
		/// </summary>
		/// <param name="level">The level of indentation</param>
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