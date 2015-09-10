#if UNITY_EDITOR
using System;
using System.Reflection;
using TNRD.Editor.Blocks;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// Extra functionality that can be used to draw pretty GUI stuff
    /// </summary>
    public class ExtendedGUI {

        #region Horizontal Line
        private static GUIStyle horizontalLineStyle = new GUIStyle() {
            normal = new GUIStyleState() { background = EditorGUIUtility.whiteTexture, textColor = Color.white },
            stretchWidth = true,
            margin = new RectOffset( 0, 0, 5, 5 )
        };

        private static Color horizontalLineColor = new Color( 0.349f, 0.349f, 0.349f );

        /// <summary>
        /// Draws a horizontal line
        /// </summary>
        /// <param name="thickness">The thickness of the line</param>
        public static void HorizontalLine( float thickness = 1 ) {
            HorizontalLine( horizontalLineColor, thickness );
        }

        /// <summary>
        /// Draws a horizontal line
        /// </summary>
        /// <param name="color">The color of the line</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void HorizontalLine( Color color, float thickness = 1 ) {
            Rect position = GUILayoutUtility.GetRect( GUIContent.none, horizontalLineStyle, GUILayout.Height( thickness ) );

            if ( Event.current.type == EventType.Repaint ) {
                var guiColor = GUI.color;
                GUI.color = color;
                horizontalLineStyle.Draw( position, false, false, false, false );
                GUI.color = guiColor;
            }
        }

        /// <summary>
        /// Draws an indented horizontal line
        /// </summary>
        /// <param name="thickness">The thickness of the line</param>
        public static void IndentedHorizontalLine( float thickness = 1 ) {
            IndentedHorizontalLine( horizontalLineColor, thickness );
        }

        /// <summary>
        /// Draws an indented horizontal line
        /// </summary>
        /// <param name="color">The color of the line</param>
        /// <param name="thickness">The thickness of the line</param>
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

        #region Internal Styles
        private static GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbar );
        private static GUIStyle toolbarButtonStyle = new GUIStyle( EditorStyles.toolbarButton );
        private static GUIStyle toolbarDropDownStyle = new GUIStyle( EditorStyles.toolbarDropDown );
        private static GUIStyle toolbarPopupStyle = new GUIStyle( EditorStyles.toolbarPopup );
        private static GUIStyle toolbarSearchStyle = new GUIStyle( "ToolbarSeachTextField" );
        private static GUIStyle toolbarSearchStyleEnd = new GUIStyle( "ToolbarSeachCancelButton" );
        private static GUIStyle toolbarSearchStyleEndEmpty = new GUIStyle( "ToolbarSeachCancelButtonEmpty" );
        private static GUIStyle toolbarTextFieldStyle = new GUIStyle( EditorStyles.toolbarTextField );
        private static GUIStyle dropdownPopupStyle = new GUIStyle( EditorStyles.popup );
        #endregion

        #region Blocks
        /// <summary>
        /// Creates a new ExtendedGUI Area
        /// </summary>
        /// <param name="options">The options to apply to the area</param>
        public static AreaBlock AreaBlock( params ExtendedGUIOption[] options ) {
            return new AreaBlock( options );
        }

        /// <summary>
        /// Creates a new DisabledGroup
        /// </summary>
        /// <param name="disabled">Should the controls within the group be disabled</param>
        public static DisabledBlock DisabledBlock( bool disabled ) {
            return new DisabledBlock( disabled );
        }

        /// <summary>
        /// Creates a new Horizontal
        /// </summary>
        /// <param name="options">The options to apply to the Horizontal</param>
        public static HorizontalBlock HorizontalBlock( params GUILayoutOption[] options ) {
            return new HorizontalBlock( options );
        }

        /// <summary>
        /// Creates a new Horizontal
        /// </summary>
        /// <param name="style">The style to apply to the Horizontal</param>
        /// <param name="options">The options to apply to the Horizontal</param>
        public static HorizontalBlock HorizontalBlock( GUIStyle style, params GUILayoutOption[] options ) {
            return new HorizontalBlock( style, options );
        }

        /// <summary>
        /// Creates a new indented segment
        /// </summary>
        public static IndentBlock IndentBlock() {
            return new IndentBlock();
        }

        /// <summary>
        /// Creates a new indented segment
        /// </summary>
        /// <param name="level">The level of indentation</param>
        public static IndentBlock IndentBlock( int level ) {
            return new IndentBlock( level );
        }

        /// <summary>
        /// Creates a new ScrollView
        /// </summary>
        /// <param name="scrollPosition">The position of the scrollbar</param>
        /// <param name="options">The options to apply to the ScrollView</param>
        public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, params GUILayoutOption[] options ) {
            return new ScrollBlock( ref scrollPosition, options );
        }

        /// <summary>
        /// Creates a new ScrollView
        /// </summary>
        /// <param name="scrollPosition">The position of the scrollbar</param>
        /// <param name="alwaysShowHorizontal">Should the horizontal scrollbar always be shown</param>
        /// <param name="alwaysShowVertical">Should the vertical scrollbar always be shown</param>
        /// <param name="options">The options to apply to the ScrollView</param>
        public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options ) {
            return new ScrollBlock( ref scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options );
        }

        /// <summary>
        /// Creates a new ScrollView
        /// </summary>
        /// <param name="scrollPosition">The position of the scrollbar</param>
        /// <param name="horizontalScrollbar">The style for the horizontal scrollbar</param>
        /// <param name="verticalScrollbar">The style for the vertical scrollbar</param>
        /// <param name="options">The options to apply to the ScrollView</param>
        public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options ) {
            return new ScrollBlock( ref scrollPosition, horizontalScrollbar, verticalScrollbar, options );
        }

        /// <summary>
        /// Creates a new ScrollView
        /// </summary>
        /// <param name="scrollPosition">The position of the scrollbar</param>
        /// <param name="alwaysShowHorizontal">Should the horizontal scrollbar always be shown</param>
        /// <param name="alwaysShowVertical">Should the vertical scrollbar always be shown</param>
        /// <param name="horizontalScrollbar">The style for the horizontal scrollbar</param>
        /// <param name="verticalScrollbar">The style for the vertical scrollbar</param>
        /// <param name="background">The style for the background</param>
        /// <param name="options">The options to apply to the ScrollView</param>
        public static ScrollBlock ScrollBlock( ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options ) {
            return new ScrollBlock( ref scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options );
        }

        /// <summary>
        /// Creats a new ToggleGroup
        /// </summary>
        /// <param name="label">The label of the Toggle Group</param>
        /// <param name="toggle">The options to apply to the ScrollView</param>
        public static ToggleBlock ToggleBlock( string label, ref bool toggle ) {
            return new ToggleBlock( label, ref toggle );
        }

        /// <summary>
        /// Creats a new ToggleGroup
        /// </summary>
        /// <param name="label">The label of the Toggle Group</param>
        /// <param name="toggle">The options to apply to the ScrollView</param>
        public static ToggleBlock ToggleBlock( GUIContent label, ref bool toggle ) {
            return new ToggleBlock( label, ref toggle );
        }

        /// <summary>
        /// Creates a new Vertical
        /// </summary>
        /// <param name="options">The options to apply to the Vertical</param>
        public static VerticalBlock VerticalBlock( params GUILayoutOption[] options ) {
            return new VerticalBlock( options );
        }

        /// <summary>
        /// Creates a new Vertical
        /// </summary>
        /// <param name="style">The style to apply to the Vertical</param>
        /// <param name="options">The options to apply to the Vertical</param>
        public static VerticalBlock VerticalBlock( GUIStyle style, params GUILayoutOption[] options ) {
            return new VerticalBlock( style, options );
        }
        #endregion

        #region Toolbar
        /// <summary>
        /// Begins a new Toolbar group
        /// </summary>
        public static void BeginToolbar() {
            GUILayout.BeginHorizontal( toolbarStyle );
        }

        /// <summary>
        /// Ends a Toolbar group
        /// </summary>
        public static void EndToolbar() {
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Adds a button in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Button</param>
        public static bool ToolbarButton( string content ) {
            return GUILayout.Button( content, toolbarButtonStyle );
        }

        /// <summary>
        /// Adds a button in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Button</param>
        public static bool ToolbarButton( GUIContent content ) {
            return GUILayout.Button( content, toolbarButtonStyle );
        }

        /// <summary>
        /// Adds a disabled button in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Button</param>
        public static void ToolbarButtonDisabled( string content ) {
            EditorGUI.BeginDisabledGroup( true );
            GUILayout.Button( content, toolbarButtonStyle );
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Adds a disabled button in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Button</param>
        public static void ToolbarButtonDisabled( GUIContent content ) {
            EditorGUI.BeginDisabledGroup( true );
            GUILayout.Button( content, toolbarButtonStyle );
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Adds a dropdown box in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Dropdown Button</param>
        public static bool ToolbarDropDown( string content ) {
            return GUILayout.Button( content, toolbarDropDownStyle );
        }

        /// <summary>
        /// Adds a dropdown box in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Dropdown Button</param>
        public static bool ToolbarDropDown( GUIContent content ) {
            return GUILayout.Button( content, toolbarDropDownStyle );
        }

        /// <summary>
        /// Adds an enum popup in toolbar style
        /// </summary>
        /// <param name="selected">The enum for the Toolbar Enum Popup</param>
        public static Enum ToolbarEnumPopup( Enum selected ) {
            return EditorGUILayout.EnumPopup( "", selected, toolbarPopupStyle );
        }

        /// <summary>
        /// Adds a label in toolbar style
        /// </summary>
        /// <param name="content">The content of the Toolbar Label</param>
        public static void ToolbarLabel( string content ) {
            GUILayout.Label( content, toolbarButtonStyle );
        }

        /// <summary>
        /// Adds a popup list in toolbar style
        /// </summary>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the popup</param>
        public static int ToolbarPopup( int current, string[] items ) {
            var contents = new GUIContent[items.Length];
            for ( int i = 0; i < items.Length; i++ ) {
                contents[i] = new GUIContent( items[i] );
            }
            return InternalDropdownList( GUILayoutUtility.GetRect( contents[0], EditorStyles.toolbarPopup ), current, contents, toolbarPopupStyle );
        }

        /// <summary>
        /// Adds a popup list in toolbar style
        /// </summary>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the popup</param>
        public static int ToolbarPopup( int current, GUIContent[] items ) {
            return InternalDropdownList( GUILayoutUtility.GetRect( items[0], EditorStyles.toolbarPopup ), current, items, toolbarPopupStyle );
        }

        /// <summary>
        /// Adds a text field in toolbar style
        /// </summary>
        /// <param name="content">The content of the TextField</param>
        /// <param name="width">The width of the TextField</param>
        public static string ToolbarTextField( string content, float width = 100 ) {
            return GUILayout.TextField( content, toolbarTextFieldStyle, GUILayout.Width( width ) );
        }

        /// <summary>
        /// Adds a search field in toolbar style
        /// </summary>
        /// <param name="content">The content of the SearchField</param>
        /// <param name="width">The width of the SearchField</param>
        public static string ToolbarSearchField( string content, float width = 100 ) {
            var gContent = new GUIContent( content );
            var rect = GUILayoutUtility.GetRect( gContent, toolbarSearchStyle, GUILayout.Width( width ) );
            return ToolbarSearchField( rect, content );
        }

        /// <summary>
        /// Adds a search field in toolbar style
        /// </summary>
        /// <param name="rect">The rectangle to draw the SearchField with</param>
        /// <param name="content">The content of the SearchField</param>
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

        /// <summary>
        /// Adds a search field in toolbar style
        /// </summary>
        /// <param name="rect">The rectangle to draw the SearchField with</param>
        /// <param name="content">The content of the SearchField</param>
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

        /// <summary>
        /// Begins a customizable area
        /// </summary>
        /// <param name="options">The options to apply to the area</param>
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

        /// <summary>
        /// Ends an area
        /// </summary>
        public static void EndArea() {
            GUILayout.EndArea();
        }

        /// <summary>
        /// Returns a rectangle for a control
        /// </summary>
        /// <param name="options">The options to apply to the rectangle</param>
        public static Rect GetControlRect( params GUILayoutOption[] options ) {
            return GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.layerMaskField, options );
        }

        /// <summary>
        /// Returns a rectangle for a control
        /// </summary>
        /// <param name="content">The content that goes into the rectangle</param>
        /// <param name="options">The options to apply to the rectangle</param>
        public static Rect GetControlRect( GUIContent content, params GUILayoutOption[] options ) {
            return GUILayoutUtility.GetRect( content, EditorStyles.layerMaskField, options );
        }

        /// <summary>
        /// Returns a rectangle for a control
        /// </summary>
        /// <param name="content">The content that goes into the rectangle</param>
        /// <param name="style">The style of the rectangle</param>
        /// <param name="options">The options to apply to the rectangle</param>
        public static Rect GetControlRect( GUIContent content, GUIStyle style, params GUILayoutOption[] options ) {
            return GUILayoutUtility.GetRect( content, style, options );
        }
        #endregion

        /// <summary>
        /// Adds a label with custom font size
        /// </summary>
        /// <param name="content">The content for the label</param>
        /// <param name="fontSize">The size of the font</param>
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

                var assembly = Assembly.GetAssembly( typeof( EditorGUI ) );
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

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="label">The label that goes in front of the dropdown list</param>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
        public static int DropdownList( string label, int current, string[] items ) {
            var rect = EditorGUILayout.GetControlRect();
            var rwidth = 150;
            var labelRect = new Rect( rect.x, rect.y, rwidth, rect.height );
            var dropdownRect = new Rect( rect.x + rwidth, rect.y, rect.width - rwidth, rect.height );
            EditorGUI.LabelField( labelRect, label );
            return DropdownList( dropdownRect, current, items );
        }

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="label">The label that goes in front of the dropdown list</param>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
        public static int DropdownList( string label, int current, GUIContent[] items ) {
            var rect = EditorGUILayout.GetControlRect();
            var rwidth = 150;
            var labelRect = new Rect( rect.x, rect.y, rwidth, rect.height );
            var dropdownRect = new Rect( rect.x + rwidth, rect.y, rect.width - rwidth, rect.height );
            EditorGUI.LabelField( labelRect, label );
            return DropdownList( dropdownRect, current, items );
        }

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
        public static int DropdownList( int current, string[] items ) {
            var rect = GetControlRect();
            GUIContent[] contents = new GUIContent[items.Length];
            for ( int i = 0; i < items.Length; i++ ) {
                contents[i] = new GUIContent( items[i] );
            }
            return InternalDropdownList( rect, current, contents, dropdownPopupStyle );
        }

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
        public static int DropdownList( int current, GUIContent[] items ) {
            var rect = GetControlRect();
            return InternalDropdownList( rect, current, items, dropdownPopupStyle );
        }

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="position">The position of the dropdown list</param>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
        public static int DropdownList( Rect position, int current, string[] items ) {
            GUIContent[] contents = new GUIContent[items.Length];
            for ( int i = 0; i < items.Length; i++ ) {
                contents[i] = new GUIContent( items[i] );
            }
            return InternalDropdownList( position, current, contents, dropdownPopupStyle );
        }

        /// <summary>
        /// Adds a dropdown list
        /// </summary>
        /// <param name="position">The position of the dropdown list</param>
        /// <param name="current">The index of the current item</param>
        /// <param name="items">The items for the dropdown list</param>
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