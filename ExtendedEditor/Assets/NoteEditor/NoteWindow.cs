using System.Collections.Generic;
using System.IO;
using TNRD;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

public class NoteWindow : ExtendedWindow {

	private string previousScene = string.Empty;

	public NoteWindow() : base( new ExtendedWindowSettings() { DrawToolbar = true } ) { }

	public override void OnInitialize() {
		base.OnInitialize();

		WindowStyle = GUIStyle.none;
	}

	public override void OnToolbarGUI() {
		if ( ExtendedGUI.ToolbarButton( "Add note" ) ) {
			AddControl( new NoteControl() );
		}
	}

	public override void Update( bool hasFocus ) {
		base.Update( hasFocus );

		if ( EditorApplication.currentScene != previousScene ) {
			ReloadNotes();
		}

		previousScene = EditorApplication.currentScene;

		var notes = GetControls<NoteControl>();
		var position = new Vector2( 10, 10 );
		foreach ( var item in notes ) {
			item.Position = position;
			position.y += item.Size.y;
			position.y += 10;
		}
	}

	public void SaveNotes() {
		var controls = GetControls<NoteControl>();
		var notes = new List<NoteControl.Serializable>();
		foreach ( var item in controls ) {
			notes.Add( NoteControl.Serializable.FromNote( item ) );
		}
		var json = JsonConvert.SerializeObject( notes );
		File.WriteAllText( GetNotePath(), json );
	}

	private void ReloadNotes() {
		var controls = GetControls<NoteControl>();
		foreach ( var item in controls ) {
			RemoveControl( item );
		}
		//var json = EditorPrefs.GetString( EditorApplication.currentScene, "" );
		if ( File.Exists( GetNotePath() ) ) {
			var json = File.ReadAllText( GetNotePath() );
			if ( !string.IsNullOrEmpty( json ) ) {
				var notes = JsonConvert.DeserializeObject<List<NoteControl.Serializable>>( json );
				foreach ( var item in notes ) {
					AddControl( NoteControl.Serializable.ToNote( item ) );
				}
			}
		}
	}

	private string GetNotePath() {
		var scenePath = EditorApplication.currentScene;
		var sceneName = Path.GetFileNameWithoutExtension( scenePath );
		var folderPath = Path.GetDirectoryName( scenePath );
		return Path.Combine( folderPath, sceneName + ".notes" );
	}
}
