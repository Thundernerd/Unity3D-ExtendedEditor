using UnityEngine;
using System.Collections;
using UnityEditor;
using TNRD.Editor.Core;

public class ModWinExample : ExtendedEditor {

    [MenuItem( "Window/Editor Examples/Modal Windows" )]
    public static void Init() {
        GetWindow<ModWinExample>().Show();
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        AddWindow( new ModWindow() );
    }

    private class ModWindow : ExtendedWindow {

        public override void OnInitialize() {
            base.OnInitialize();

            WindowStyle = ExtendedGUI.DarkNoneWindowStyle;
            Settings.DrawToolbar = true;
        }

        public override void OnToolbarGUI() {
            base.OnToolbarGUI();

            if ( ExtendedGUI.ToolbarButton( "Show DialogBox" ) ) {
                Editor.ShowModalWindow( new TNRD.Editor.Windows.ExtendedDialogBox( "HelloWorld", "This is a dialog box" ) );
            } else if ( ExtendedGUI.ToolbarButton( "Show InputBox" ) ) {
                Editor.ShowModalWindow( new TNRD.Editor.Windows.ExtendedInputDialog( "HelloWorld", "This is an input box" ) );
            }
        }
    }
}
