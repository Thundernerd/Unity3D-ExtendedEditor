using UnityEngine;
using System.Collections;
using TNRD;
using UnityEditor;

public class NoteEditor : ExtendedEditor {

	[MenuItem("Window/Notes")]
	private static void Create() {
		var window = GetWindow<NoteEditor>( false, "Notes" );
		window.minSize = new Vector2( 400, 400 );
		window.Show();
	}

	protected override void OnInitialize() {
		base.OnInitialize();

		RepaintOnUpdate = true;
		AddWindow( new NoteWindow() );
	}
}
