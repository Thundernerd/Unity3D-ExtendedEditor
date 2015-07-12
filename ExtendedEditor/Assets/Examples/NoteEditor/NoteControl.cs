using System;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

public class NoteControl : ExtendedControl {

	public class Serializable {
		public Color @Color;
		public string Text;
		public string Object;
		public bool Visible;

		public static Serializable FromNote( NoteControl note ) {
			return new Serializable() {
				Color = note.color,
				Text = note.Text,
				Object = note.GameObject == null ? "" : note.GameObject.name,
				Visible = note.showSpatialNote
			};
		}

		public static NoteControl ToNote( Serializable value ) {
			return new NoteControl(
				value.Text,
				value.Color,
				value.Object ) { showSpatialNote = value.Visible, isEditing = false };
		}
	}

	private bool isEditing = false;

	private Vector2 textSize = new Vector2();
	private GUIStyle textStyle;
	private GUIStyle textAreaStyle;
	private GUIStyle dropStyle;
	private GUIStyle dropTextStyle;

	private Texture2D cross;
	private Texture2D downArrow;

	private Color color;
	private bool showSpatialNote = true;

	public string Text { get; private set; }
	public GameObject GameObject { get; private set; }

	public NoteControl() : this( "", Color.yellow, "" ) {
		isEditing = true;
	}
	public NoteControl( string text, Color color, string objectName ) {
		isEditing = false;
		Text = text;
		this.color = color;
		GameObject = GameObject.Find( objectName );
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
		textAreaStyle.fontSize = textStyle.fontSize = textAreaStyle.fontSize = 20;

		dropStyle = new GUIStyle( "NotificationBackground" );
		dropTextStyle = new GUIStyle( "NotificationText" );
		dropTextStyle.padding = new RectOffset( 20, 20, 20, 20 );
		dropTextStyle.alignment = TextAnchor.MiddleCenter;
		dropTextStyle.fontSize = 17;
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
		var gObjRect = new Rect( Rectangle.x + 55.25f, Rectangle.y + 7.5f, Rectangle.width - ( Rectangle.x + 76.25f ), 16 );
		var textRect = new Rect( Rectangle.x + 10, Rectangle.y + 30, textSize.x, textSize.y );

		showSpatialNote = GUI.Toggle( new Rect( Rectangle.x + 35.25f, Rectangle.y + 7.5f, 10, 16 ), showSpatialNote, "" );
		using (ExtendedGUI.DisabledBlock( !showSpatialNote )) {
			GameObject = (GameObject)EditorGUI.ObjectField( gObjRect, GameObject, typeof(GameObject), true );
		}

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
			if ( Input.KeysUp( KeyCode.LeftShift, KeyCode.RightShift ) && Input.KeyDown( KeyCode.Return ) ) {
				isEditing = false;
				SaveNotes();
				Event.current.Use();
			} else {
				Text = GUI.TextArea( textRect, Text, textAreaStyle );
				if ( Text != "" ) {
					var w = textAreaStyle.CalcSize( new GUIContent( Text ) );
					if ( w.x > textRect.width ) {
						for ( int i = Text.Length - 1; i >= 0; i-- ) {
							if ( char.IsWhiteSpace( Text[i] ) ) {
								var piece = Text.Substring( i, Text.Length - i );
								Text = Text.Remove( i );
								Text += "\n";
								Text += piece.Trim();
								break;
							}
						}
					}
				}
			}
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

	public override void OnSceneGUI( SceneView view ) {
		if ( !showSpatialNote ) return;
		if ( GameObject == null ) return;
		if ( textStyle == null ) return;

		var camera = view.camera;
		var offset = new Vector3();

		var collider = GameObject.GetComponent<Collider2D>();
		if ( collider != null ) {
			offset = collider.bounds.size / 2;
			offset.x *= GameObject.transform.lossyScale.x;
			offset.y *= GameObject.transform.lossyScale.y;
		}

		var screenPosition = camera.WorldToScreenPoint( GameObject.transform.position + offset );
		screenPosition.y = view.position.size.y - screenPosition.y;
		screenPosition.y -= 17.5f;

		var compactSize = textStyle.CalcSize( new GUIContent( Text ) );

		var originalColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box( new Rect( screenPosition.x, screenPosition.y - compactSize.y, compactSize.x, compactSize.y ), "" );
		GUI.Box( new Rect( screenPosition.x, screenPosition.y - compactSize.y, compactSize.x, compactSize.y ), "" );
		GUI.backgroundColor = originalColor;

		GUI.Label( new Rect( screenPosition.x, screenPosition.y - compactSize.y, compactSize.x, compactSize.y ), Text, textStyle );
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

	public override void OnDragUpdate( string[] paths, Vector2 position ) {
		base.OnDragUpdate( paths, position );

		if ( paths.Length == 0 ) {
			if ( DragAndDrop.objectReferences.Length == 1 ) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			} else {
				DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
		} else {
			DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
		}
	}

	public override void OnDragPerform( string[] paths, Vector2 position ) {
		base.OnDragPerform( paths, position );

		if ( paths.Length == 0 ) {
			if ( DragAndDrop.objectReferences.Length == 1 ) {
				if ( Rectangle.Contains( position ) ) {
					DragAndDrop.AcceptDrag();
					GameObject = DragAndDrop.objectReferences[0] as GameObject;
					SceneView.lastActiveSceneView.Repaint();
				}
			}
		}
	}

	private void SetColor( object item ) {
		color = (Color)item;
		SaveNotes();
	}

	private void SaveNotes() {
		( Window as NoteWindow ).SaveNotes();
	}
}
