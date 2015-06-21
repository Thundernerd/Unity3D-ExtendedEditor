#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {
	public class ScrollBlock : IDisposable {

		public ScrollBlock( ref Vector2 scrollPosition, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, options );
		}

		public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options );
		}

		public ScrollBlock( ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, horizontalScrollbar, verticalScrollbar, options );
		}

		public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options );
		}

		public void Dispose() {
			EditorGUILayout.EndScrollView();
		}
	}
}
#endif