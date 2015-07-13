#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

	[DocsDescription("")]
	public class ScrollBlock : IDisposable {

		[DocsDescription("Creates a new instance of ScrollBlock")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public ScrollBlock( ref Vector2 scrollPosition, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, options );
		}

		[DocsDescription("Creates a new instance of ScrollBlock")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("alwaysShowHorizontal", "Should the horizontal scrollbar always be shown")]
		[DocsParameter("alwaysShowVertical", "Should the vertical scrollbar always be shown")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options );
		}

		[DocsDescription("Creates a new instance of ScrollBlock")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("horizontalScrollbar", "The style for the horizontal scrollbar")]
		[DocsParameter("verticalScrollbar", "The style for the vertical scrollbar")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public ScrollBlock( ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, horizontalScrollbar, verticalScrollbar, options );
		}

		[DocsDescription("Creates a new instance of ScrollBlock")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("alwaysShowHorizontal", "Should the horizontal scrollbar always be shown")]
		[DocsParameter("alwaysShowVertical", "Should the vertical scrollbar always be shown")]
		[DocsParameter("horizontalScrollbar", "The style for the horizontal scrollbar")]
		[DocsParameter("verticalScrollbar", "The style for the vertical scrollbar")]
		[DocsParameter("background", "The style for the background")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options ) {
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options );
		}

		[DocsIgnore]
		public void Dispose() {
			EditorGUILayout.EndScrollView();
		}
	}
}
#endif