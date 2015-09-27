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
            Enum,
            Extended,
            Editor
        }

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
        }

        public override void OnToolbarGUI() {
            base.OnToolbarGUI();

            if ( ExtendedGUI.ToolbarButton( "Button" ) ) { }
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
