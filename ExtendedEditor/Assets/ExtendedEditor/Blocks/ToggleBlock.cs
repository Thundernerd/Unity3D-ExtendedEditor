#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Inspired by https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	/// <summary>
	/// 
	/// </summary>
	public class ToggleBlock : IDisposable {

		/// <summary>
		/// Creates a new instance of ToggleBlock
		/// </summary>
		/// <param name="label">The label of the Toggle Group</param>
		/// <param name="toggle">The options to apply to the ScrollView</param>
		public ToggleBlock( string label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		/// <summary>
		/// Creates a new instance of ToggleBlock
		/// </summary>
		/// <param name="label">The label of the Toggle Group</param>
		/// <param name="toggle">The options to apply to the ScrollView</param>
		public ToggleBlock( GUIContent label, ref bool toggle ) {
			toggle = EditorGUILayout.BeginToggleGroup( label, toggle );
		}

		public void Dispose() {
			EditorGUILayout.EndToggleGroup();
		}
	}
}
#endif