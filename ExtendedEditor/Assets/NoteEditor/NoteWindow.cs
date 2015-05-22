using UnityEngine;
using System.Collections;
using TNRD;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NoteWindow : ExtendedWindow {

	private string previousScene = string.Empty;

	public NoteWindow() : base( new ExtendedWindowSettings() { DrawToolbar = true } ) { }

	public override void OnInitialize() {
		base.OnInitialize();

		WindowStyle = GUIStyle.none;
	}

	public override void OnToolbarGUI() {
		base.OnToolbarGUI();

		if ( ExtendedGUI.ToolbarButton( "Add note" ) ) {
			AddControl( new NoteControl() );
			SaveNotes( EditorApplication.currentScene );
		}
	}

	public void SaveNotes( string scene ) {
		var controls = GetControls<NoteControl>();
		var notes = new List<NoteControl.Serializable>();
		foreach ( var item in controls ) {
			notes.Add( NoteControl.Serializable.FromNote( item ) );
		}
		var json = JsonConvert.SerializeObject( notes, new ColorConverter() );
		EditorPrefs.SetString( scene, json );
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

	private void ReloadNotes() {
		var controls = GetControls<NoteControl>();
		foreach ( var item in controls ) {
			RemoveControl( item );
		}
		var json = EditorPrefs.GetString( EditorApplication.currentScene, "" );
		if ( !string.IsNullOrEmpty( json ) ) {
			var notes = JsonConvert.DeserializeObject<List<NoteControl.Serializable>>( json, new ColorConverter() );
			foreach ( var item in notes ) {
				AddControl( NoteControl.Serializable.ToNote( item ) );
			}
		}
	}
}
