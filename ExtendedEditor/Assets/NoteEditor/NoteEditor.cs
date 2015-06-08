using System.Reflection;
using TNRD;
using UnityEditor;
using UnityEngine;

public class NoteEditor : ExtendedEditor {

	[MenuItem("Window/Notes")]
	private static void Create() {
		var window = GetWindow<NoteEditor>( "Notes", true, Utils.InspectorWindow );
		window.minSize = new Vector2( 400, 400 );
		window.Show();
	}

	protected override void OnInitialize() {
		base.OnInitialize();

		RepaintOnUpdate = true;
		AddWindow( new NoteWindow() );
	}
}
