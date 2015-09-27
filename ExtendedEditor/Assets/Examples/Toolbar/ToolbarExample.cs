using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;
using UnityEditor;

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
            Hello,
            World,
            Enum,
            Extended,
            Editor
        }

        private TempEnum tempEnum;
        private int popupIndex = 0;
        private string[] popupItems = {
            "Hello",
            "World",
            "Popup",
            "Extended",
            "Editor"
        };
        private bool toggle = false;

        public override void OnInitialize() {
            base.OnInitialize();

            WindowStyle = ExtendedGUI.DarkNoneWindowStyle;
            Settings.DrawToolbar = true;
        }

        public override void OnToolbarGUI() {
            base.OnToolbarGUI();

            if ( ExtendedGUI.ToolbarButton( "HelloButton" ) ) { }
            if ( ExtendedGUI.ToolbarDropDown( "HelloDropdown" ) ) { }
            tempEnum = (TempEnum)ExtendedGUI.ToolbarEnumPopup( tempEnum );
            ExtendedGUI.ToolbarLabel( "HelloLabel" );
            popupIndex = ExtendedGUI.ToolbarPopup( popupIndex, popupItems );
            ExtendedGUI.ToolbarSearchField( "HelloSearchField" );
            ExtendedGUI.ToolbarTextField( "HelloTextField" );
            toggle = ExtendedGUI.ToolbarToggle( toggle, "HelloToggle" );
        }
    }
}
