#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

namespace TNRD {
	public class ExtendedGUI {

		public static void BeginToolbar() {
			GUILayout.BeginHorizontal( EditorStyles.toolbar );
		}

		public static void EndToolbar() {
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public static bool ToolbarButton( string content ) {
			return GUILayout.Button( content, EditorStyles.toolbarButton );
		}

		public static bool ToolbarButton( GUIContent content ) {
			return GUILayout.Button( content, EditorStyles.toolbarButton );
		}

		public static void ToolbarButtonDisabled( string content ) {
			EditorGUI.BeginDisabledGroup( true );
			GUILayout.Button( content, EditorStyles.toolbarButton );
			EditorGUI.EndDisabledGroup();
		}

		public static void ToolbarButtonDisabled( GUIContent content ) {
			EditorGUI.BeginDisabledGroup( true );
			GUILayout.Button( content, EditorStyles.toolbarButton );
			EditorGUI.EndDisabledGroup();
		}

		public static string ToolbarTextfield( string content ) {
			return GUILayout.TextField( content, EditorStyles.toolbarTextField, GUILayout.MinWidth( 100f ) );
		}

		public static int ToolbarPopup( int current, string[] items ) {
			var contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return doDropdownList( GUILayoutUtility.GetRect( contents[0], EditorStyles.toolbarPopup ), current, contents, EditorStyles.toolbarPopup );
		}

		public static int ToolbarPopup( int current, GUIContent[] items ) {
			return doDropdownList( GUILayoutUtility.GetRect( items[0], EditorStyles.toolbarPopup ), current, items, EditorStyles.toolbarPopup );
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

		#region Dropdown extras
		private static int dropdownHash = "btrDropDown".GetHashCode();

		private class DropdownCallbackInfo {
			private const string kMaskMenuChangedMessage = "MaskMenuChanged";
			public static DropdownCallbackInfo instance;
			private readonly int controlID;
			private int selectedIndex;

			private object view;
			private MethodInfo method;

			public DropdownCallbackInfo( int controlID ) {
				this.controlID = controlID;

				var assembly = Assembly.GetAssembly( typeof(EditorGUI) );
				Type t = assembly.GetType( "UnityEditor.GUIView" );

				var p = t.GetProperty( "current", BindingFlags.Static | BindingFlags.Public );
				view = p.GetValue( null, null );
				method = t.GetMethod( "SendEvent", BindingFlags.NonPublic | BindingFlags.Instance );
			}

			public static int GetSelectedValueForControl( int controlID, int index ) {
				Event current = Event.current;

				if ( current.type == EventType.ExecuteCommand && current.commandName == "MaskMenuChanged" ) {
					if ( instance == null ) {
						Debug.LogError( "Mask menu has no instance" );
						return index;
					} else if ( instance.controlID == controlID ) {
						index = instance.selectedIndex;

						GUI.changed = true;

						instance = null;
						current.Use();
					}
				}

				return index;
			}

			internal void SetMaskValueDelegate( object userData, string[] options, int selected ) {
				selectedIndex = selected;

				if ( view != null ) {
					method.Invoke( view, new object[] { EditorGUIUtility.CommandEvent( "MaskMenuChanged" ) } );
				}
			}
		}
		#endregion

		private static Vector2 GetDropdownSize( string[] items, GUIStyle style ) {
			float width = 0;
			float height = 0;
			for ( int i = 0; i < items.Length; i++ ) {
				var s = style.CalcSize( new GUIContent( items[i] ) );
				if ( s.x > width )
					width = s.x;
				if ( s.y > height )
					height = s.y;
			}
			return new Vector2( width * 1.1f, height );
		}

		private static Vector2 GetDropdownSize( GUIContent[] items, GUIStyle style ) {
			float width = 0;
			float height = 0;
			for ( int i = 0; i < items.Length; i++ ) {
				var s = style.CalcSize( items[i] );
				if ( s.x > width )
					width = s.x;
				if ( s.y > height )
					height = s.y;
			}
			return new Vector2( width * 1.1f, height );
		}

		public static int DropdownList( int current, string[] items ) {
			var size = GetDropdownSize( items, EditorStyles.popup );
			var rect = GetRect( size );
			GUIContent[] contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return doDropdownList( rect, current, contents, EditorStyles.popup );
		}

		public static int DropdownList( int current, GUIContent[] items ) {
			var size = GetDropdownSize( items, EditorStyles.popup );
			var rect = GetRect( size );
			return doDropdownList( rect, current, items, EditorStyles.popup );
		}

		public static int DropdownList( Rect position, int current, string[] items ) {
			GUIContent[] contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return doDropdownList( position, current, contents, EditorStyles.popup );
		}

		public static int DropdownList( Rect position, int current, GUIContent[] items ) {
			return doDropdownList( position, current, items, EditorStyles.popup );
		}

		private static int doDropdownList( Rect position, int current, GUIContent[] items, GUIStyle style ) {
			if ( items.Length == 0 ) {
				return -1;
			}

			int controlID = GUIUtility.GetControlID( dropdownHash, FocusType.Native, position );
			var mask = DropdownCallbackInfo.GetSelectedValueForControl( controlID, current );

			var evt = Event.current;
			if ( evt.type == EventType.Repaint ) {
				style.Draw( position, new GUIContent( items[current] ), controlID, false );
			} else if ( evt.type == EventType.MouseDown && position.Contains( evt.mousePosition ) ) {
				DropdownCallbackInfo.instance = new DropdownCallbackInfo( controlID );
				evt.Use();
				EditorUtility.DisplayCustomMenu( position, items, current,
					new EditorUtility.SelectMenuItemFunction( DropdownCallbackInfo.instance.SetMaskValueDelegate ), null );
			}

			return mask;
		}

	}
}
#endif