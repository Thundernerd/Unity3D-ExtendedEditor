using System;
using System.Collections.Generic;
using System.Reflection;
using TNRD.Editor.Core;
using TNRD.Editor.Json;
using UnityEditor;
using UnityEngine;

[Serializable]
class OtherEditor : ExtendedWindow {

    [JsonProperty]
    private string texto = "";
    [JsonProperty]
    private float sliderV = 0.5f;

    [MenuItem( "TNRD/Test Editor" )]
    private static void Init() {
        var jsonTester = CreateEditor( "Editor Tester" );
        jsonTester.Show();
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
        sliderV = EditorGUILayout.Slider( sliderV, 0, 2 );
        texto = EditorGUILayout.TextField( texto );
    }
}