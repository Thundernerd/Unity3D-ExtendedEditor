using System;
using TNRD.Editor.Core;
using UnityEditor;

[Serializable]
class TestEditor : ExtendedWindow {

    private string texto = "";

    [MenuItem( "TNRD/Test Editor" )]
    private static void Init() {
        CreateEditor( "Tester" );
    }

    protected override void OnDestroy() {

    }

    protected override void OnFocus() {

    }

    protected override void OnLostFocus() {

    }

    protected override void OnInitialize() {

    }

    protected override void OnInitializeGUI() {

    }

    protected override void OnInspectorUpdate() {

    }

    protected override void OnUpdate() {

    }

    protected override void OnGUI() {

    }
}