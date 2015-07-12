using System.Collections.Generic;
using System.IO;
using TNRD.Editor.Core;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

public class NoteWindow : ExtendedWindow {

	private string previousScene = string.Empty;

	public NoteWindow() : base( new ExtendedWindowSettings() { DrawToolbar = true, UseOnSceneGUI = true } ) { }

	public override void OnInitialize() {
		base.OnInitialize();

		WindowStyle = GUIStyle.none;
	}

	public override void OnToolbarGUI() {
		if ( ExtendedGUI.ToolbarButton( "Add Note" ) ) {
			AddControl( new NoteControl() );
		}
		if ( ExtendedGUI.ToolbarButton( "Clear Notes" ) ) {
			Controls.Clear();
			SaveNotes();
		}
	}

	public override void Update( bool hasFocus ) {
		base.Update( hasFocus );

		if ( EditorApplication.currentScene != previousScene ) {
			ReloadNotes();
		}

		previousScene = EditorApplication.currentScene;

		var notes = GetControlsByType<NoteControl>();
		var position = new Vector2( 10, 10 );
		foreach ( var item in notes ) {
			item.Position = position;
			position.y += item.Size.y;
			position.y += 10;
		}
	}

	public void SaveNotes() {
		var controls = GetControlsByType<NoteControl>();
		var notes = new List<NoteControl.Serializable>();
		foreach ( var item in controls ) {
			notes.Add( NoteControl.Serializable.FromNote( item ) );
		}

		var json = JsonConvert.SerializeObject( notes );

		var path = GetNotePath();
		if ( !string.IsNullOrEmpty( path ) ) {
			File.WriteAllText( GetNotePath(), json );
		}
	}

	private void ReloadNotes() {
		var controls = GetControlsByType<NoteControl>();
		foreach ( var item in controls ) {
			RemoveControl( item );
		}

		var path = GetNotePath();
		if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) ) {
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
		if ( string.IsNullOrEmpty( scenePath ) ) return "";

		var sceneName = Path.GetFileNameWithoutExtension( scenePath );
		var folderPath = Path.GetDirectoryName( scenePath );
		return Path.Combine( folderPath, sceneName + ".notes" );
	}
}
