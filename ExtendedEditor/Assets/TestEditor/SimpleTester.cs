using System;
using TNRD.Editor;
using TNRD.Editor.Core;
using TNRD.Editor.Serialization;
using TNRD.Editor.Windows;
using UnityEditor;
using UnityEngine;

[Serializable]
class SimpleTester : ExtendedWindow {

    [RequireSerialization]
    private string texto = "";
    [RequireSerialization]
    private float sliderV = 0.5f;

    [MenuItem( "TNRD/Test Editor" )]
    private static void Init() {
        var jsonTester = CreateEditor( "Editor Tester" );
        jsonTester.Show();
    }

    [MenuItem( "TNRD/Clear Editors" )]
    private static void ClearEditors() {
        var objects = Resources.FindObjectsOfTypeAll<ExtendedEditor>();
        Debug.LogFormat( "Destroying {0} editors", objects.Length );
        if ( objects.Length > 0 ) {
            foreach ( var editor in objects ) {
                GameObject.DestroyImmediate( editor );
            }
        }
    }

    protected override void OnInitialize() {
        AddControl( new BoxControl() { Position = new Vector2( 250, 250 ) } );

        Editor.wantsMouseMove = true;

        WindowSettings.Draggable = true;
        Size = new Vector2( 500, 500 );
    }

    protected override void OnGUI() {

        sliderV = EditorGUILayout.Slider( sliderV, 0, 2 );
        texto = EditorGUILayout.TextField( texto );

        if ( Input.ButtonPressed( EMouseButton.Left ) ) {
            Debug.Log( "Left Pressed" );
        }

        if ( Input.ButtonReleased( EMouseButton.Left ) ) {
            Debug.Log( "Left Released" );
        }

        if ( Input.ButtonDown( EMouseButton.Left ) ) {
            Debug.Log( "Left Down" );
        }

        if ( Input.ButtonUp( EMouseButton.Left ) ) {
            Debug.Log( "Left Up" );
        }

        if ( GUILayout.Button( "Default" ) ) {
            WindowStyle = EWindowStyle.Default;
        }

        if ( GUILayout.Button( "DefaultUnity" ) ) {
            WindowStyle = EWindowStyle.DefaultUnity;
        }

        if ( GUILayout.Button( "NoToolbarDark" ) ) {
            WindowStyle = EWindowStyle.NoToolbarDark;
        }

        if ( GUILayout.Button( "NoToolbarLight" ) ) {
            WindowStyle = EWindowStyle.NoToolbarLight;
        }

        if ( GUILayout.Button( "MessageBox" ) ) {
            ShowPopup( new MessageBox(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam tempus semper aliquam. Nullam finibus lectus sapien, a efficitur enim porta at. Quisque eget euismod est, in vehicula odio. Phasellus efficitur dapibus tristique. Nullam orci lectus, consectetur non arcu eu, dictum fermentum orci.",
                delegate ( EDialogResult result ) {
                    Debug.LogFormat( "{0}", result );
                } ) );
        }

        if ( GUILayout.Button( "Toggle TitleBarButtons" ) ) {
            WindowSettings.DrawTitleBarButtons = !WindowSettings.DrawTitleBarButtons;
        }

        if ( GUILayout.Button( "Toggle Resizable" ) ) {
            WindowSettings.Resizable = !WindowSettings.Resizable;
        }
    }

    private class TPopup : ExtendedPopup {

        protected override void OnInitialize() {
            //Size = new Vector2( 350, 150 );
            //Position = new Vector2( 100, 100 );
        }

        protected override void OnGUI() {
            base.OnGUI();
        }
    }
}