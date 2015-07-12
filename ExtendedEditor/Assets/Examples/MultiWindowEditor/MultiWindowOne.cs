using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;

public class MultiWindowOne : ExtendedWindow {

	public override void OnInitialize() {
		base.OnInitialize();

		Settings.IsBlocking = false;

		Size = new Vector2( Editor.position.size.x / 2, Editor.position.size.y );

		var control = new ImageControl( "ulogo" );
		AddControl( control );
		control.Size = new Vector2( Assets["ulogo"].width / 5, Assets["ulogo"].height / 5 );
	}

	public override void Update( bool hasFocus ) {
		base.Update( hasFocus );
		
		Size = new Vector2( Editor.position.size.x / 2, Editor.position.size.y );
	}
}
