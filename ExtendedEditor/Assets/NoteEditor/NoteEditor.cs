using System.Reflection;
using TNRD;
using UnityEditor;
using UnityEngine;

public class NoteEditor : ExtendedEditor {

	[MenuItem("Window/Notes")]
	private static void Create() {
		var inspectorType = Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.InspectorWindow" );
		var window = GetWindow<NoteEditor>( "Notes", true, inspectorType );
		window.minSize = new Vector2( 400, 400 );
		window.Show();
	}

	protected override void OnInitialize() {
		base.OnInitialize();

		RepaintOnUpdate = true;
		AddWindow( new NoteWindow() );
	}
}
