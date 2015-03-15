#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class BetterGUI {

	public static void BeginToolbar() {
		GUILayout.BeginHorizontal( "toolbar" );
	}

	public static void EndToolbar() {
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public static bool ToolbarButton( string content ) {
		return GUILayout.Button( content, "ToolbarButton" );
	}

	public static bool ToolbarButton( GUIContent content ) {
		return GUILayout.Button( content, "ToolbarButton" );
	}

	public static void ToolbarButtonDisabled( string content ) {
		EditorGUI.BeginDisabledGroup( true );
		GUILayout.Button( content, "ToolbarButton" );
		EditorGUI.EndDisabledGroup();
	}

	public static void ToolbarButtonDisabled( GUIContent content ) {
		EditorGUI.BeginDisabledGroup( true );
		GUILayout.Button( content, "ToolbarButton" );
		EditorGUI.EndDisabledGroup();
	}

	public static bool ToolbarDropDown( string content ) {
		return GUILayout.Button( content, "ToolbarDropDown" );
	}

	public static bool ToolbarDropDown( GUIContent content ) {
		return GUILayout.Button( content, "ToolbarDropDown" );
	}

	private static List<Rect> positions = new List<Rect>();
	private static Rect GetRect( Vector2 size ) {
		if ( positions.Count == 0 ) {
			Debug.LogError( "You have to begin a group first!" );
			return new Rect();
		}

		var r = positions[positions.Count - 1];
		var r2 = new Rect( r.x, r.y, size.x, size.y );
		r.y += size.y;
		positions[positions.Count - 1] = r;
		return r2;
	}
	public static void Clear() {
		positions.Clear();
	}

	public static void BeginGroup( Rect position ) {
		positions.Add( position );
	}
	public static void BeginGroup( Rect position, Vector2 offset ) {
		position.position += offset;
		positions.Add( position );
	}
	public static void EndGroup() {
		if ( positions.Count > 0 ) {
			positions.RemoveAt( positions.Count - 1 );
		}
	}

	public static void Space() {
		Space( 5 );
	}
	public static void Space( float pixels ) {
		if ( positions.Count > 0 ) {
			Offset( 0, pixels );
		}
	}
	public static void Offset( float x, float y ) {
		if ( positions.Count > 0 ) {
			var r = positions[positions.Count - 1];
			r.x += x;
			r.width -= x * 2;
			r.y += y;
			positions[positions.Count - 1] = r;
		}
	}

	public static void Label( string content ) {
		var size = GUI.skin.label.CalcSize( new GUIContent( content ) );
		var rect = GetRect( size );
		GUI.Label( rect, content );
	}
	public static void Label( string content, int fontSize ) {
		var pFontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = fontSize;
		Label( content );
		GUI.skin.label.fontSize = pFontSize;
	}

	public static string TextField( string content ) {
		var r = positions[positions.Count - 1];
		var size = GUI.skin.textField.CalcSize( new GUIContent( content ) );
		size.x = r.width;
		var rect = GetRect( size );
		return GUI.TextField( rect, content );
	}

	public static int IntSlider( int value, int min, int max ) {
		var size = new Vector2( positions[positions.Count - 1].width, GUI.skin.horizontalSlider.lineHeight );
		var rect = GetRect( size );
		Space();
		return (int)GUI.HorizontalSlider( rect, value, min, max );
	}

	public static bool Toggle( bool value, string content ) {
		var size = GUI.skin.toggle.CalcSize( new GUIContent( content ) );
		var rect = GetRect( size );
		return EditorGUI.Toggle( rect, content, value );
	}

	public static bool ToggleLeft( string content, bool value ) {
		var size = GUI.skin.toggle.CalcSize( new GUIContent( content ) );
		var rect = GetRect( size );
		return EditorGUI.ToggleLeft( rect, content, value );
	}
}
#endif