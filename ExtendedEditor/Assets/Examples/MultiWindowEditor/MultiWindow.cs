using TNRD.Editor.Core;
using UnityEngine;

public class MultiWindow : ExtendedWindow {

    public MultiWindow( string name, bool allowMovement, bool drawButtons )
        : base() {
        WindowContent = new GUIContent( name );

        Settings.IsBlocking = false;
        Settings.IsFullscreen = false;


        Settings.DrawTitleBarButtons = drawButtons;
        Settings.AllowRepositioning = Settings.AllowResize = allowMovement;
    }

    public override void OnInitialize() {
        base.OnInitialize();

        if ( Settings.AllowRepositioning ) {
            Size = new Vector2( 150, 150 );
        } else {
            Size = new Vector2( Editor.position.size.x / 2, Editor.position.size.y );
        }
    }
}
