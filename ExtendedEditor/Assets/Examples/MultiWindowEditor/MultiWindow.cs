using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;

public class MultiWindow : ExtendedWindow {

	public MultiWindow( string name, bool loose )
		: base() {
		Title = name;

		Settings.IsBlocking = false;
		Settings.IsFullscreen = false;

		Settings.DrawTitleBarButtons = loose;
		Settings.AllowRepositioning = Settings.AllowResize = loose;
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
