using TNRD.Editor.Core;
using UnityEngine;

public class BoxControl : ExtendedControl {

    public bool AnchorPoint = false;

    public BoxControl() {
        Size = new Vector2( 100, 100 );
    }

    protected override void OnGUI() {
        Position = Input.MousePosition;
        GUI.Box( Rectangle, "" );
    }
}
