#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace TNRD {
	public class VerticalBlock : IDisposable {

		public VerticalBlock( params GUILayoutOption[] options ) {
			EditorGUILayout.BeginVertical( options );
		}

		public VerticalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			EditorGUILayout.BeginVertical( style, options );
		}

		public void Dispose() {
			EditorGUILayout.EndVertical();
		}
	}
}
#endif