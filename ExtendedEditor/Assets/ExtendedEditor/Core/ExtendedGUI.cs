#if UNITY_EDITOR
using System;
using System.Reflection;
using TNRD.Editor.Blocks;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

	[DocsDescription("Extra functionality that can be used to draw pretty GUI stuff")]
	public class ExtendedGUI {

		[DocsIgnore]
		private ExtendedGUI() { }

		#region Horizontal Line
		private static GUIStyle horizontalLineStyle = new GUIStyle() {
			normal = new GUIStyleState() { background = EditorGUIUtility.whiteTexture, textColor = Color.white },
			stretchWidth = true,
			margin = new RectOffset( 0, 0, 5, 5 )
		};

		private static Color horizontalLineColor = new Color( 0.349f, 0.349f, 0.349f );

		[DocsDescription("Draws a horizontal line")]
		[DocsParameter("thickness", "The thickness of the line")]
		public static void HorizontalLine( float thickness = 1 ) {
			HorizontalLine( horizontalLineColor, thickness );
		}

		[DocsDescription("Draws a horizontal line")]
		[DocsParameter("color", "The color of the line")]
		[DocsParameter("thickness", "The thickness of the line")]
		public static void HorizontalLine( Color color, float thickness = 1 ) {
			Rect position = GUILayoutUtility.GetRect( GUIContent.none, horizontalLineStyle, GUILayout.Height( thickness ) );

			if ( Event.current.type == EventType.Repaint ) {
				var guiColor = GUI.color;
				GUI.color = color;
				horizontalLineStyle.Draw( position, false, false, false, false );
				GUI.color = guiColor;
			}
		}

		[DocsDescription("Draws an indented horizontal line")]
		[DocsParameter("thickness", "The thickness of the line")]
		public static void IndentedHorizontalLine( float thickness = 1 ) {
			IndentedHorizontalLine( horizontalLineColor, thickness );
		}

		[DocsDescription("Draws an indented horizontal line")]
		[DocsParameter("color", "The color of the line")]
		[DocsParameter("thickness", "The thickness of the line")]
		public static void IndentedHorizontalLine( Color color, float thickness = 1 ) {
			Rect position = EditorGUI.IndentedRect( GUILayoutUtility.GetRect( GUIContent.none, horizontalLineStyle, GUILayout.Height( thickness ) ) );

			if ( Event.current.type == EventType.Repaint ) {
				var guiColor = GUI.color;
				GUI.color = color;
				horizontalLineStyle.Draw( position, false, false, false, false );
				GUI.color = guiColor;
			}
		}
		#endregion

		private static GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbar );
		private static GUIStyle toolbarButtonStyle = new GUIStyle( EditorStyles.toolbarButton );
		private static GUIStyle toolbarDropDownStyle = new GUIStyle( EditorStyles.toolbarDropDown );
		private static GUIStyle toolbarPopupStyle = new GUIStyle( EditorStyles.toolbarPopup );
		private static GUIStyle toolbarSearchStyle = new GUIStyle( "ToolbarSeachTextField" );
		private static GUIStyle toolbarSearchStyleEnd = new GUIStyle( "ToolbarSeachCancelButton" );
		private static GUIStyle toolbarSearchStyleEndEmpty = new GUIStyle( "ToolbarSeachCancelButtonEmpty" );
		private static GUIStyle toolbarTextFieldStyle = new GUIStyle( EditorStyles.toolbarTextField );
		private static GUIStyle dropdownPopupStyle = new GUIStyle( EditorStyles.popup );

		#region Blocks
		[DocsDescription("Creates a new ExtendedGUI Area")]
		[DocsParameter("options", "The options to apply to the area")]
		public static AreaBlock AreaBlock( params ExtendedGUIOption[] options ) {
			return new AreaBlock( options );
		}

		[DocsDescription("Creates a new DisabledGroup")]
		[DocsParameter("disabled", "Should the controls within the group be disabled")]
		public static DisabledBlock DisabledBlock( bool disabled ) {
			return new DisabledBlock( disabled );
		}

		[DocsDescription("Creates a new Horizontal")]
		[DocsParameter("options", "The options to apply to the Horizontal")]
		public static HorizontalBlock HorizontalBlock( params GUILayoutOption[] options ) {
			return new HorizontalBlock( options );
		}

		[DocsDescription("Creates a new Horizontal")]
		[DocsParameter("style", "The style to apply to the Horizontal")]
		[DocsParameter("options", "The options to apply to the Horizontal")]
		public static HorizontalBlock HorizontalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			return new HorizontalBlock( style, options );
		}

		[DocsDescription("Creates a new indented segment")]
		public static IndentBlock IndentBlock() {
			return new IndentBlock();
		}

		[DocsDescription("Creates a new indented segment")]
		[DocsParameter("level", "The level of indentation")]
		public static IndentBlock IndentBlock( int level ) {
			return new IndentBlock( level );
		}

		[DocsDescription("Creates a new ScrollView")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, params GUILayoutOption[] options ) {
			return new ScrollBlock( ref scrollPosition, options );
		}

		[DocsDescription("Creates a new ScrollView")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("alwaysShowHorizontal", "Should the horizontal scrollbar always be shown")]
		[DocsParameter("alwaysShowVertical", "Should the vertical scrollbar always be shown")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options ) {
			return new ScrollBlock( ref scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options );
		}

		[DocsDescription("Creates a new ScrollView")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("horizontalScrollbar", "The style for the horizontal scrollbar")]
		[DocsParameter("verticalScrollbar", "The style for the vertical scrollbar")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options ) {
			return new ScrollBlock( ref scrollPosition, horizontalScrollbar, verticalScrollbar, options );
		}

		[DocsDescription("Creates a new ScrollView")]
		[DocsParameter("scrollPosition", "The position of the scrollbar")]
		[DocsParameter("alwaysShowHorizontal", "Should the horizontal scrollbar always be shown")]
		[DocsParameter("alwaysShowVertical", "Should the vertical scrollbar always be shown")]
		[DocsParameter("horizontalScrollbar", "The style for the horizontal scrollbar")]
		[DocsParameter("verticalScrollbar", "The style for the vertical scrollbar")]
		[DocsParameter("background", "The style for the background")]
		[DocsParameter("options", "The options to apply to the ScrollView")]
		public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options ) {
			return new ScrollBlock( ref scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options );
		}

		[DocsDescription("Creats a new ToggleGroup")]
		[DocsParameter("label", "The label of the Toggle Group")]
		[DocsParameter("toggle", "The options to apply to the ScrollView")]
		public static ToggleBlock ToggleBlock( string label, ref bool toggle ) {
			return new ToggleBlock( label, ref toggle );
		}

		[DocsDescription("Creats a new ToggleGroup")]
		[DocsParameter("label", "The label of the Toggle Group")]
		[DocsParameter("toggle", "The options to apply to the ScrollView")]
		public static ToggleBlock ToggleBlock( GUIContent label, ref bool toggle ) {
			return new ToggleBlock( label, ref toggle );
		}

		[DocsDescription("Creates a new Vertical")]
		[DocsParameter("options", "The options to apply to the Vertical")]
		public static VerticalBlock VerticalBlock( params GUILayoutOption[] options ) {
			return new VerticalBlock( options );
		}

		[DocsDescription("Creates a new Vertical")]
		[DocsParameter("style", "The style to apply to the Vertical")]
		[DocsParameter("options", "The options to apply to the Vertical")]
		public static VerticalBlock VerticalBlock( GUIStyle style, params GUILayoutOption[] options ) {
			return new VerticalBlock( style, options );
		}
		#endregion

		#region Toolbar
		[DocsDescription("Begins a new Toolbar group")]
		public static void BeginToolbar() {
			GUILayout.BeginHorizontal( toolbarStyle );
		}

		[DocsDescription("Ends a Toolbar group")]
		public static void EndToolbar() {
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		[DocsDescription("Adds a button in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Button")]
		public static bool ToolbarButton( string content ) {
			return GUILayout.Button( content, toolbarButtonStyle );
		}

		[DocsDescription("Adds a button in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Button")]
		public static bool ToolbarButton( GUIContent content ) {
			return GUILayout.Button( content, toolbarButtonStyle );
		}

		[DocsDescription("Adds a disabled button in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Button")]
		public static void ToolbarButtonDisabled( string content ) {
			EditorGUI.BeginDisabledGroup( true );
			GUILayout.Button( content, toolbarButtonStyle );
			EditorGUI.EndDisabledGroup();
		}

		[DocsDescription("Adds a disabled button in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Button")]
		public static void ToolbarButtonDisabled( GUIContent content ) {
			EditorGUI.BeginDisabledGroup( true );
			GUILayout.Button( content, toolbarButtonStyle );
			EditorGUI.EndDisabledGroup();
		}

		[DocsDescription("Adds a dropdown box in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Dropdown Button")]
		public static bool ToolbarDropDown( string content ) {
			return GUILayout.Button( content, toolbarDropDownStyle );
		}

		[DocsDescription("Adds a dropdown box in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Dropdown Button")]
		public static bool ToolbarDropDown( GUIContent content ) {
			return GUILayout.Button( content, toolbarDropDownStyle );
		}

		[DocsDescription("Adds an enum popup in toolbar style")]
		[DocsParameter("selected", "The enum for the Toolbar Enum Popup")]
		public static Enum ToolbarEnumPopup( Enum selected ) {
			return EditorGUILayout.EnumPopup( "", selected, toolbarPopupStyle );
		}

		[DocsDescription("Adds a label in toolbar style")]
		[DocsParameter("content", "The content of the Toolbar Label")]
		public static void ToolbarLabel( string content ) {
			GUILayout.Label( content, toolbarButtonStyle );
		}

		[DocsDescription("Adds a popup list in toolbar style")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the popup")]
		public static int ToolbarPopup( int current, string[] items ) {
			var contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return InternalDropdownList( GUILayoutUtility.GetRect( contents[0], EditorStyles.toolbarPopup ), current, contents, toolbarPopupStyle );
		}

		[DocsDescription("Adds a popup list in toolbar style")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the popup")]
		public static int ToolbarPopup( int current, GUIContent[] items ) {
			return InternalDropdownList( GUILayoutUtility.GetRect( items[0], EditorStyles.toolbarPopup ), current, items, toolbarPopupStyle );
		}

		[DocsDescription("Adds a text field in toolbar style")]
		[DocsParameter("content", "The content of the TextField")]
		[DocsParameter("width", "The width of the TextField")]
		public static string ToolbarTextField( string content, float width = 100 ) {
			return GUILayout.TextField( content, toolbarTextFieldStyle, GUILayout.Width( width ) );
		}

		[DocsDescription("Adds a search field in toolbar style")]
		[DocsParameter("content", "The content of the SearchField")]
		[DocsParameter("width", "The width of the SearchField")]
		public static string ToolbarSearchField( string content, float width = 100 ) {
			var gContent = new GUIContent( content );
			var rect = GUILayoutUtility.GetRect( gContent, toolbarSearchStyle, GUILayout.Width( width ) );
			return ToolbarSearchField( rect, content );
		}

		[DocsDescription("Adds a search field in toolbar style")]
		[DocsParameter("rect", "The rectangle to draw the SearchField with")]
		[DocsParameter("content", "The content of the SearchField")]
		public static string ToolbarSearchField( Rect rect, string content ) {
			rect.width -= 15;
			var text = GUI.TextField( rect, content, toolbarSearchStyle );
			rect.x += rect.width;
			rect.width += 15;
			if ( GUI.Button( rect, "", !string.IsNullOrEmpty( text ) ? toolbarSearchStyleEnd : toolbarSearchStyleEndEmpty ) ) {
				text = "";
				GUIUtility.keyboardControl = 0;
			}
			return text;
		}

		[DocsDescription("Adds a search field in toolbar style")]
		[DocsParameter("rect", "The rectangle to draw the SearchField with")]
		[DocsParameter("content", "The content of the SearchField")]
		public static string ToolbarSearchFieldWithBackground( Rect rect, string content ) {
			rect.height = toolbarStyle.fixedHeight;
			GUI.Box( rect, "", toolbarStyle );
			rect.x += 0.5f;
			rect.y += 2;
			return ToolbarSearchField( rect, content );
		}
		#endregion

		#region ControlRect
		private static Vector2 windowSize;

		[DocsDescription("Begins a customizable area")]
		[DocsParameter("options", "The options to apply to the area")]
		public static void BeginArea( params ExtendedGUIOption[] options ) {
			float x = 0, y = 0, w = 0, h = 0;
			foreach ( var item in options ) {
				switch ( item.Type ) {
					case ExtendedGUIOption.EType.Width:
						w = (float)item.Value;
						break;
					case ExtendedGUIOption.EType.Height:
						h = (float)item.Value;
						break;
					case ExtendedGUIOption.EType.HorizontalPosition:
						x = (float)item.Value;
						break;
					case ExtendedGUIOption.EType.VerticalPosition:
						y = (float)item.Value;
						break;
					case ExtendedGUIOption.EType.Position:
						var position = (Vector2)item.Value;
						x = position.x;
						y = position.y;
						break;
					case ExtendedGUIOption.EType.Size:
						var size = (Vector2)item.Value;
						w = size.x;
						h = size.y;
						break;
					case ExtendedGUIOption.EType.WindowSize:
						windowSize = (Vector2)item.Value;
						break;
				}
			}

			if ( w == 0 ) {
				w = windowSize.x - x;
			}
			if ( h == 0 ) {
				h = windowSize.y - y;
			}

			GUILayout.BeginArea( new Rect( x, y, w, h ) );
		}

		[DocsDescription("Ends an area")]
		public static void EndArea() {
			GUILayout.EndArea();
		}

		[DocsDescription("Returns a rectangle for a control")]
		[DocsParameter("options", "The options to apply to the rectangle")]
		public static Rect GetControlRect( params GUILayoutOption[] options ) {
			return GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.layerMaskField, options );
		}

		[DocsDescription("Returns a rectangle for a control")]
		[DocsParameter("content", "The content that goes into the rectangle")]
		[DocsParameter("options", "The options to apply to the rectangle")]
		public static Rect GetControlRect( GUIContent content, params GUILayoutOption[] options ) {
			return GUILayoutUtility.GetRect( content, EditorStyles.layerMaskField, options );
		}

		[DocsDescription("Returns a rectangle for a control")]
		[DocsParameter("content", "The content that goes into the rectangle")]
		[DocsParameter("style", "The style of the rectangle")]
		[DocsParameter("options", "The options to apply to the rectangle")]
		public static Rect GetControlRect( GUIContent content, GUIStyle style, params GUILayoutOption[] options ) {
			return GUILayoutUtility.GetRect( content, style, options );
		}
		#endregion

		[DocsDescription("Adds a label with custom font size")]
		[DocsParameter("content", "The content for the label")]
		[DocsParameter("fontSize", "The size of the font")]
		public static void Label( string content, int fontSize ) {
			var pFontSize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontSize;
			var gc = new GUIContent( content );
			GUI.Label( GetControlRect( gc, GUI.skin.label ), gc ); ;
			GUI.skin.label.fontSize = pFontSize;
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
						GUIUtility.hotControl = GUIUtility.keyboardControl = 0;
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

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("label", "The label that goes in front of the dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( string label, int current, string[] items ) {
			var rect = EditorGUILayout.GetControlRect();
			var rwidth = 150;
			var labelRect = new Rect( rect.x, rect.y, rwidth, rect.height );
			var dropdownRect = new Rect( rect.x + rwidth, rect.y, rect.width - rwidth, rect.height );
			EditorGUI.LabelField( labelRect, "Platform material" );
			return DropdownList( dropdownRect, current, items );
		}

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("label", "The label that goes in front of the dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( string label, int current, GUIContent[] items ) {
			var rect = EditorGUILayout.GetControlRect();
			var rwidth = 150;
			var labelRect = new Rect( rect.x, rect.y, rwidth, rect.height );
			var dropdownRect = new Rect( rect.x + rwidth, rect.y, rect.width - rwidth, rect.height );
			EditorGUI.LabelField( labelRect, "Platform material" );
			return DropdownList( dropdownRect, current, items );
		}

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( int current, string[] items ) {
			var rect = GetControlRect();
			GUIContent[] contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return InternalDropdownList( rect, current, contents, dropdownPopupStyle );
		}

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( int current, GUIContent[] items ) {
			var rect = GetControlRect();
			return InternalDropdownList( rect, current, items, dropdownPopupStyle );
		}

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("position", "The position of the dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( Rect position, int current, string[] items ) {
			GUIContent[] contents = new GUIContent[items.Length];
			for ( int i = 0; i < items.Length; i++ ) {
				contents[i] = new GUIContent( items[i] );
			}
			return InternalDropdownList( position, current, contents, dropdownPopupStyle );
		}

		[DocsDescription("Adds a dropdown list")]
		[DocsParameter("position", "The position of the dropdown list")]
		[DocsParameter("current", "The index of the current item")]
		[DocsParameter("items", "The items for the dropdown list")]
		public static int DropdownList( Rect position, int current, GUIContent[] items ) {
			return InternalDropdownList( position, current, items, dropdownPopupStyle );
		}

		private static int InternalDropdownList( Rect position, int current, GUIContent[] items, GUIStyle style ) {
			if ( items.Length == 0 ) {
				items = new GUIContent[] { new GUIContent( "-" ) };
			}

			int controlID = GUIUtility.GetControlID( dropdownHash, FocusType.Native, position );
			var mask = DropdownCallbackInfo.GetSelectedValueForControl( controlID, current );

			var evt = Event.current;
			if ( evt.type == EventType.Repaint ) {
				style.Draw( position, new GUIContent( items[current] ), controlID, false );
			} else if ( evt.type == EventType.MouseDown && position.Contains( evt.mousePosition ) ) {
				DropdownCallbackInfo.instance = new DropdownCallbackInfo( controlID );
				GUIUtility.keyboardControl = GUIUtility.hotControl = 0;
				evt.Use();
				EditorUtility.DisplayCustomMenu( position, items, current,
					new EditorUtility.SelectMenuItemFunction( DropdownCallbackInfo.instance.SetMaskValueDelegate ), null );
			}

			return mask;
		}
	}
}
#endif