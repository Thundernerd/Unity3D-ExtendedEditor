using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

public class MultiWindowEditor : ExtendedEditor {

    [MenuItem( "Window/Editor Examples/Multi-Window editor" )]
    private static void Create() {
        var window = GetWindow<MultiWindowEditor>( "Multi-Window", true );
        window.minSize = new Vector2( 500, 500 );
        window.Show();
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        RepaintOnUpdate = true;

        var w1 = new MultiWindow( "#1", false, false );
        var w2 = new MultiWindow( "#2", true, false );
        var w3 = new MultiWindow( "#3", true, true );

        AddWindow( w1 );
        AddWindow( w2 );
        AddWindow( w3 );

        w2.Position = new Vector2( Mathf.Round( position.size.x / 2 + 50 ), 50 );
        w3.Position = new Vector2( Mathf.Round( position.size.x / 2 + 100 ), 250 );
    }
}
