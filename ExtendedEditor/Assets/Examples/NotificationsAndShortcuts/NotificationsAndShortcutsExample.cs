using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;
using UnityEditor;

public class NotificationsAndShortcutsExample : ExtendedEditor {

    [MenuItem( "Window/Editor Examples/Notifications and Shortcuts" )]
    public static void Init() {
        GetWindow<NotificationsAndShortcutsExample>().Show();
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        AddWindow( new NotShortWindow() );
    }

    private class NotShortWindow : ExtendedWindow {

        public NotShortWindow() : base() {
            AddShortcut( KeyCode.H, FirstCallback, true, false, false );
            AddShortcut( KeyCode.H, SecondCallback, false, true, false );
            AddShortcut( KeyCode.H, ThirdCallback, false, false, true );
            AddShortcut( KeyCode.H, FourthCallback, true, false, true );
        }

        private void FirstCallback() {
            ShowNotification( "Callback on Ctrl+H" );
        }

        private void SecondCallback() {
            ShowNotification( "Callback on Alt+H" );
        }

        private void ThirdCallback() {
            ShowNotification( "Callback on Shift+H" );
        }

        private void FourthCallback() {
            ShowErrorNotification( "(Error) Callback on Ctrl+Shift+H" );
        }

        public override void OnGUI() {
            base.OnGUI();

            EditorGUILayout.LabelField( "The following key-combo's show a notification: \nCtrl+H \nAlt+H \nShift+H \nCtrl+Shift+H", GUILayout.ExpandHeight( true ) );
        }
    }
}
