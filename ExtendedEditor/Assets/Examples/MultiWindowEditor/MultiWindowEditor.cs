using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;
using UnityEditor;

public class MultiWindowEditor : ExtendedEditor {

	[MenuItem("Window/Editor Examples/Multi-Window editor")]
	private static void Create() {
		var window = GetWindow<MultiWindowEditor>( "Multi-Window", true );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	protected override void OnInitialize() {
		base.OnInitialize();

		RepaintOnUpdate = true;

		AddWindow( new MultiWindowOne() );
		AddWindow( new MultiWindowTwo() );
	}
}
