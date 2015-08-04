#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Taken from https://github.com/nickgravelyn/UnityToolbag/tree/master/EditorTools

namespace TNRD.Editor.Blocks {

    /// <summary>
    /// 
    /// </summary>
    public class ScrollBlock : IDisposable {

        /// <summary> 
        /// Creates a new instance of ScrollBlock
        /// </summary>
        /// <param name=scrollPosition">The position of the scrollbar</param>
        /// <param name=options">The options to apply to the ScrollView</param>
        public ScrollBlock( ref Vector2 scrollPosition, params GUILayoutOption[] options ) {
            scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, options );
        }

        /// <summary> 
        /// Creates a new instance of ScrollBlock
        /// </summary>
        /// <param name=scrollPosition">The position of the scrollbar</param>
        /// <param name=alwaysShowHorizontal">Should the horizontal scrollbar always be shown</param>
        /// <param name=alwaysShowVertical">Should the vertical scrollbar always be shown</param>
        /// <param name=options">The options to apply to the ScrollView</param>
        public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options ) {
            scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options );
        }

        /// <summary> 
        /// Creates a new instance of ScrollBlock
        /// </summary>
        /// <param name=scrollPosition">The position of the scrollbar</param>
        /// <param name=horizontalScrollbar">The style for the horizontal scrollbar</param>
        /// <param name=verticalScrollbar">The style for the vertical scrollbar</param>
        /// <param name=options">The options to apply to the ScrollView</param>
        public ScrollBlock( ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options ) {
            scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, horizontalScrollbar, verticalScrollbar, options );
        }

        /// <summary> 
        /// Creates a new instance of ScrollBlock
        /// </summary>
        /// <param name=scrollPosition">The position of the scrollbar</param>
        /// <param name=alwaysShowHorizontal">Should the horizontal scrollbar always be shown</param>
        /// <param name=alwaysShowVertical">Should the vertical scrollbar always be shown</param>
        /// <param name=horizontalScrollbar">The style for the horizontal scrollbar</param>
        /// <param name=verticalScrollbar">The style for the vertical scrollbar</param>
        /// <param name=background">The style for the background</param>
        /// <param name=options">The options to apply to the ScrollView</param>
        public ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options ) {
            scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options );
        }

        public void Dispose() {
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif