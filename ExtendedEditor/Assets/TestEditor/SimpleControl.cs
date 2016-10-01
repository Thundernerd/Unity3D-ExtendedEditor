using TNRD.Editor;
using UnityEngine;

public class SimpleControl : ExtendedControl {

    protected override void OnInitialize() {
        Logger.LogMethod();
        Position = new Vector2( 250, 250 );
        Size = new Vector2( 100, 100 );
    }

    protected override void OnInitializeGUI() {
        Logger.LogMethod();
    }

    protected override void OnDestroy() {
        Logger.LogMethod();
    }

    protected override void OnFocus() {
        Logger.LogMethod();
    }

    protected override void OnLostFocus() {
        Logger.LogMethod();
    }

    protected override void OnInspectorUpdate() {

    }

    protected override void OnUpdate() {

    }

    protected override void OnGUI() {
        GUI.Box( Rectangle, "" );

        if (Input.ButtonPressed(EMouseButton.Left)) {
            Debug.Log( "Left pressed" );
        }

        if (Input.ButtonReleased(EMouseButton.Left)) {
            Debug.Log( "Left released" );
        }

        if ( Input.KeyPressed( KeyCode.A ) ) {
            Debug.Log( "A pressed" );
        }

        if ( Input.KeyReleased( KeyCode.A ) ) {
            Debug.Log( "A released" );
        }
    }
}
