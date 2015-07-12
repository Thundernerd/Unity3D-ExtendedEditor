using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;

public class MultiWindowTwo : ExtendedWindow {

	ImageControl control;

	public override void OnInitialize() {
		base.OnInitialize();

		Settings.AllowRepositioning = true;
		Settings.AllowResize = true;
		Settings.IsBlocking = false;

		Position = new Vector2( Editor.position.size.x / 2, 50 );
		Size = new Vector2( 400, 400 );

		control = new ImageControl( "ulogo" );
		AddControl( control );
		control.Size = new Vector2( Assets["ulogo"].width / 5, Assets["ulogo"].height / 5 );
	}

	public override void OnGUI() {
		base.OnGUI();

		control.Position = Input.MousePosition;
	}
}
