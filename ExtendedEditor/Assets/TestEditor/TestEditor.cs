using System;
using TNRD.Editor.Core;
using UnityEditor;

[Serializable]
class asdf : ExtendedWindow {

    private string texto = "";

    [MenuItem( "Hello/Temptemp" )]
    private static void Init() {
        CreateEditor( "TempTemp" );
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