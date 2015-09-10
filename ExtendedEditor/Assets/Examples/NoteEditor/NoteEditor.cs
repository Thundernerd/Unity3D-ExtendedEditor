using TNRD.Editor.Core;
using TNRD.Editor.Utilities;
using UnityEditor;
using UnityEngine;

public class NoteEditor : ExtendedEditor {

    [MenuItem( "Window/Editor Examples/Note Editor" )]
    private static void Create() {
        var window = GetWindow<NoteEditor>( "Notes", true, Utils.InspectorWindowType );
        window.minSize = new Vector2( 400, 400 );
        window.Show();
    }

    protected override void OnInitialize() {
        base.OnInitialize();

        RepaintOnUpdate = true;
        AddWindow( new NoteWindow() );
    }
}
