#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace TNRD {
	public class HorizontalBlock : IDisposable {

		public HorizontalBlock( params GUILayoutOption[] options ) {
			EditorGUILayout.BeginHorizontal( options );
		}

		public HorizontalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginHorizontal( style, options );
		}

		public void Dispose() {
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif