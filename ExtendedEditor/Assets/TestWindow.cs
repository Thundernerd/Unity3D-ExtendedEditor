using TNRD.Editor;
using UnityEditor;
using UnityEngine;

public class TestWindow : ExtendedWindow {

	public TestWindow() : base( new ExtendedWindowSettings() { UseCamera = true, DrawToolbar = true } ) {
		WindowStyle = GUIStyle.none;
	}

	public override void OnInitialize() {
		base.OnInitialize();

		//AddControl( new TimelineControl() { Position = new Vector2( 25, 25 ), Size = new Vector2( 1000, 100 ) } );

		//string[] items = new string[40];
		//for ( int i = 0; i < 40; i++ ) {
		//	items[i] = Random.Range( 5000, 999999 ).ToString();
		//}
		//var c = new ListControl( new Vector2( 25, 150 ), new Vector2( 150, 500 ), items, true, true );
		//AddControl( c );

		//for ( int i = 0; i < 10; i++ ) {
		//	AddControl( new TestDraggableControl() { Position = new Vector2( 50 * i, 50 * i ), Size = new Vector2( 50, 50 ) } );
		//}

		//AddControl( new SelectorControl() );
	}

	int i = 0;
	string a = "aaaa";
	string b = "bbbb";

	public override void Update( bool hasFocus ) {
		base.Update( hasFocus );

		if ( Input.KeyPressed( KeyCode.Alpha1 ) ) {
			ShowNotification( "Notification #1" );
		}
		if ( Input.KeyPressed( KeyCode.Alpha2 ) ) {
			ShowNotification( "A bit longer notification #2" );
		}
		if ( Input.KeyPressed( KeyCode.Alpha3 ) ) {
			ShowNotification( "This is quite the long notification, they shouldn't be this long #3" );
		}


	}

	public override void OnToolbarGUI() {
		ExtendedGUI.ToolbarButton( "HelloButton" );
		a = ExtendedGUI.ToolbarTextfield( a );
		i = ExtendedGUI.ToolbarPopup( i, new string[] { "afdsaafdsafds", "b", "c", "fd" } );
		b = ExtendedGUI.ToolbarSearchField( b );
		//Camera.z = EditorGUILayout.Slider( "Zoom", Camera.z, 0.01f, 1f );
	}

	public override void OnGUI() {
		//GUI.Label( new Rect( 0, 0, 250, 25 ), string.Format( "Camera: {0}", Camera ) );

		EditorGUILayout.LabelField( "1" );
		using (ExtendedGUI.IndentBlock()) {
			EditorGUILayout.LabelField( "1" );
		}
		using (ExtendedGUI.IndentBlock( 2 )) {
			EditorGUILayout.LabelField( "1" );
		}
		using (ExtendedGUI.IndentBlock()) {
			EditorGUILayout.LabelField( "1" );
		}
		EditorGUILayout.LabelField( "1" );

		//GUI.Button( new Rect( 25, 25, 150, 25 ), "asdf", "toolbarPopup" );

		//GUI.Box( new Rect( 0, 0, Size.x, Size.y ), "HEOO!" );

		//GUI.Button( new Rect( 0, 0, 100, 25 ), "HelloWorld!" );
		//GUI.Button( new Rect( 500, 500, 100, 25 ), "HelloWorld2!" );
		//GUI.Button( new Rect( 750, 750, 100, 25 ), "HelloWorld3!" );
		//GUI.Button( new Rect( 1000, 1000, 100, 25 ), "HelloWorld4!" );
	}
}
