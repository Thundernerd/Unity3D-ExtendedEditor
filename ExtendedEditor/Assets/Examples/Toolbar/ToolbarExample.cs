using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;
using UnityEditor;
using TNRD.Editor.Json;
using System.Collections.Generic;

public class ToolbarExample : ExtendedEditor {

    [MenuItem( "Window/Editor Examples/Toolbar" )]
    public static void Init() {
        GetWindow<ToolbarExample>().Show();
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        AddWindow( new ToolWindow() );
    }

    private class ToolWindow : ExtendedWindow {

        private enum TempEnum {
            Enum,
            Extended,
            Editor
        }

        //public char[] ArrayChar = {
        //    'a','c','e','g'
        //};
        //public int[] ArrayInt = {
        //    0,1,2,3,4,5
        //};
        public string[] ArrayString = {
            "a","b","c","defgh"
        };

        public List<char> ListChar = new List<char>();
        public List<int> ListInt = new List<int>();
        public List<string> ListString = new List<string>();
        public Dictionary<int, int> IntDictionary = new Dictionary<int, int>();
        public Dictionary<int, string> IntStringDictionary = new Dictionary<int, string>();
        public Dictionary<string, int> StringIntDictionary = new Dictionary<string, int>();

        public int abcd = 5;
        //[JsonIgnore]
        //public int abcde = 5;

        //private float fgh = 5.5f;
        //[JsonProperty]
        //private float fghi = 5.5f;

        private TempEnum tempEnum;
        private int popupIndex = 0;
        private string[] popupItems = {
            "Popup",
            "Extended",
            "Editor"
        };

        private bool toggle = false;

        public override void OnInitialize() {
            base.OnInitialize();

            WindowStyle = ExtendedGUI.DarkNoneWindowStyle;
            Settings.DrawToolbar = true;

            for ( int i = 0; i < 10; i++ ) {
                AddControl( new TNRD.Editor.Controls.Image() );
            }

            //for ( int i = 0; i < 20; i++ ) {
            //    ListChar.Add( i.ToString()[0] );
            //    ListInt.Add( i );
            //    ListString.Add( i.ToString() );
            //}

            //for ( int i = 0; i < 10; i++ ) {
                //IntList.Add( i );
                //IntDictionary.Add( i, i );
                //IntStringDictionary.Add( i, i.ToString() );
                //StringIntDictionary.Add( i.ToString(), i );
            //}
        }

        public override void OnToolbarGUI() {
            base.OnToolbarGUI();

            if ( ExtendedGUI.ToolbarButton( "Button" ) ) {
                var value = JsonSerializer.Serialize( this );
                System.IO.File.WriteAllText( "temp.json", value );
            }

            if ( ExtendedGUI.ToolbarButton( "Button" ) ) {
                var json = System.IO.File.ReadAllText( "temp.json" );
                var eddie = JsonDeserializer.Deserialize<ToolbarExample>( json );
            }

            if ( ExtendedGUI.ToolbarDropDown( "Dropdown" ) ) { }
            tempEnum = (TempEnum)ExtendedGUI.ToolbarEnumPopup( tempEnum );
            ExtendedGUI.ToolbarLabel( "Label" );
            popupIndex = ExtendedGUI.ToolbarPopup( popupIndex, popupItems );
            ExtendedGUI.ToolbarSearchField( "SearchField" );
            ExtendedGUI.ToolbarTextField( "TextField" );
            toggle = ExtendedGUI.ToolbarToggle( toggle, "Toggle" );
        }
    }
}
