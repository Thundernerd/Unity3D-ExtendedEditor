using System;
using TNRD;
using UnityEditor;
using UnityEngine;

public class NoteControl : ExtendedControl {

	public class Serializable {
		public Color @Color;
		public string Text;

		public static Serializable FromNote( NoteControl note ) {
			return new Serializable() { Color = note.color, Text = note.Text };
		}

		public static NoteControl ToNote( Serializable value ) {
			return new NoteControl( value.Text, value.Color ) { isEditing = false };
		}
	}

	private bool isEditing = false;

	private Vector2 textSize = new Vector2();
	private GUIStyle textStyle;
	private GUIStyle textAreaStyle;

	private Texture2D cross;
	private Texture2D downArrow;

	private Color color;
	public string Text { get; private set; }

	public NoteControl() : this( "", Color.yellow ) {
		isEditing = true;
	}
	public NoteControl( string text, Color color ) {
		isEditing = false;
		Text = text;
		this.color = color;
	}

	public override void OnInitialize() {
		base.OnInitialize();

		Position.Set( 10, 10 );

		cross = Window.Assets["Cross"];
		downArrow = Window.Assets["DownArrow"];
	}

	public override void OnInitializeGUI() {
		base.OnInitializeGUI();

		textStyle = new GUIStyle( GUI.skin.label );
		textAreaStyle = new GUIStyle( GUI.skin.textArea );
		textStyle.fontSize = textAreaStyle.fontSize = 20;
	}

	public override void OnGUI() {
		base.OnGUI();

		var c = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box( Rectangle, "" );
		GUI.Box( Rectangle, "" );
		GUI.backgroundColor = c;

		var arrowRect = new Rect( Rectangle.x + 11.25f, Rectangle.y + 7.5f, 15, 15 );
		var crossRect = new Rect( Rectangle.width - 15, Rectangle.y + 7.5f, 15, 15 );
		var textRect = new Rect( Rectangle.x + 10, Rectangle.y + 30, textSize.x, textSize.y );

		if ( GUI.Button( arrowRect, "", GUIStyle.none ) ) {
			var gm = new GenericMenu();
			gm.AddItem( new GUIContent( "Blue" ), false, SetColor, Color.blue );
			gm.AddItem( new GUIContent( "Green" ), false, SetColor, Color.green );
			gm.AddItem( new GUIContent( "Magenta" ), false, SetColor, Color.magenta );
			gm.AddItem( new GUIContent( "Red" ), false, SetColor, Color.red );
			gm.AddItem( new GUIContent( "Yellow" ), false, SetColor, Color.yellow );
			gm.ShowAsContext();
		}
		GUI.DrawTexture( arrowRect, downArrow );

		if ( GUI.Button( crossRect, "", GUIStyle.none ) ) {
			if ( string.IsNullOrEmpty( Text ) ) {
				Window.RemoveControl( this );
			} else {
				if ( EditorUtility.DisplayDialog( "Caution", "You are about to delete a note, are you sure?", "Yes", "No" ) ) {
					Window.RemoveControl( this );
				}
			}
			SaveNotes();
		}
		GUI.DrawTexture( crossRect, cross );

		if ( isEditing ) {
			Text = GUI.TextArea( textRect, Text, textAreaStyle );
		} else {
			GUI.Label( textRect, Text, textStyle );
		}

		if ( isEditing && Input.ButtonReleased( EMouseButton.Left ) ) {
			if ( !textRect.Contains( Input.MousePosition ) ) {
				isEditing = false;
				SaveNotes();
			}
		}

		if ( Input.IsDoubleClick && Input.Button == EMouseButton.Left ) {
			if ( textRect.Contains( Input.MousePosition ) ) {
				isEditing = true;
			}
		}

		if ( isEditing && Input.KeyDown( KeyCode.Escape ) ) {
			Text = Text.Trim( Environment.NewLine.ToCharArray() ).Trim();
			isEditing = false;
			SaveNotes();
		}
	}

	public override void Update( bool hasFocus ) {
		if ( textStyle == null ) return;

		var boxWidth = Window.Size.x - 20;
		var textWidth = boxWidth - 20;

		var textHeight = textStyle.CalcHeight( new GUIContent( Text ), textWidth );
		var boxHeight = textHeight + 40;

		Size.Set( boxWidth, boxHeight );
		textSize.Set( textWidth, textHeight );
	}

	private void SetColor( object item ) {
		color = (Color)item;
		SaveNotes();
	}

	private void SaveNotes() {
		( Window as NoteWindow ).SaveNotes();
	}
}
