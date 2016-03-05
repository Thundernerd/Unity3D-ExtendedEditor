using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ExtendedEditor : EditorWindow {

    [Serializable]
    private class ReflectionData {
        public MethodInfo Initialize;
        public MethodInfo InitializeGUI;
        public MethodInfo Destroy;

        public MethodInfo Focus;
        public MethodInfo LostFocus;

        public MethodInfo Update;
        public MethodInfo InspectorUpdate;

        public MethodInfo GUI;

        public ReflectionData() {
            var type = typeof( ExtendedWindow );

            Initialize = type.GetMethod( "InternalInitialize", BindingFlags.Instance | BindingFlags.NonPublic );
            InitializeGUI = type.GetMethod( "InternalInitializeGUI", BindingFlags.Instance | BindingFlags.NonPublic );
            Destroy = type.GetMethod( "InternalDestroy", BindingFlags.Instance | BindingFlags.NonPublic );

            Focus = type.GetMethod( "InternalFocus", BindingFlags.Instance | BindingFlags.NonPublic );
            LostFocus = type.GetMethod( "InternalLostFocus", BindingFlags.Instance | BindingFlags.NonPublic );

            InspectorUpdate = type.GetMethod( "InternalInspectorUpdate", BindingFlags.Instance | BindingFlags.NonPublic );
            Update = type.GetMethod( "InternalUpdate", BindingFlags.Instance | BindingFlags.NonPublic );

            GUI = type.GetMethod( "InternalGUI", BindingFlags.Instance | BindingFlags.NonPublic );
        }
    }

    public Vector2 Position {
        get { return position.position; }
        set {
            var pos = position;
            pos.position = value;
            position = pos;
        }
    }
    public Vector2 Size {
        get { return position.size; }
        set {
            var pos = position;
            pos.size = value;
            position = pos;
        }
    }

    private List<ExtendedWindow> windows = new List<ExtendedWindow>();

    private bool isInitialized;
    private bool isInitializedGUI;

    private ReflectionData rData;

    private void OnInitialize() {
        rData = new ReflectionData();
        isInitialized = true;
    }

    private void OnInitializeGUI() {
        isInitializedGUI = true;
    }

    private void OnDestroy() {
        for ( int i = windows.Count - 1; i >= 0; i-- ) {
            rData.Destroy.Invoke( windows[i], null );
            windows.RemoveAt( i );
        }
    }

    private void OnFocus() {
        for ( int i = windows.Count - 1; i >= 0; i-- ) {
            rData.Focus.Invoke( windows[i], null );
        }
    }

    private void OnLostFocus() {
        for ( int i = windows.Count - 1; i >= 0; i-- ) {
            rData.LostFocus.Invoke( windows[i], null );
        }
    }

    private void OnGUI() {
        if ( !isInitialized ) {
            OnInitialize();
            return;
        }

        if ( !isInitializedGUI ) {
            OnInitializeGUI();
            return;
        }

        var windowsToProcess = new List<ExtendedWindow>( windows );

        BeginWindows();
        for ( int i = windowsToProcess.Count - 1; i >= 0; i-- ) {
            GUI.Window( i, windowsToProcess[i].WindowRect, WindowGUI, windowsToProcess[i].WindowContent );
        }
        EndWindows();
    }

    private void OnInspectorUpdate() {
        var windowsToProcess = new List<ExtendedWindow>( windows );

        for ( int i = 0; i < windowsToProcess.Count; i++ ) {
            rData.InspectorUpdate.Invoke( windowsToProcess[i], null );
        }
    }

    private void Update() {
        var windowsToProcess = new List<ExtendedWindow>( windows );

        for ( int i = 0; i < windowsToProcess.Count; i++ ) {
            rData.Update.Invoke( windowsToProcess[i], null );
        }
    }

    private void WindowGUI( int id ) {
        if ( id < windows.Count ) {
            rData.GUI.Invoke( windows[id], null );
        }
    }

    public void AddWindow( ExtendedWindow window ) {
        window.Editor = this;

        rData.Initialize.Invoke( window, null );
        windows.Add( window );
    }

    public void RemoveWindow( ExtendedWindow window ) {
        rData.Destroy.Invoke( window, null );
        windows.Remove( window );
    }

    public static ExtendedEditor CreateEditor( params ExtendedWindow[] windows ) {
        var inst = GetWindow<ExtendedEditor>();

        if ( windows.Length > 0 ) {
            inst.rData = new ReflectionData();
        }

        foreach ( var item in windows ) {
            inst.AddWindow( item );
        }

        return inst;
    }

    public static ExtendedEditor CreateEditor( string title, params ExtendedWindow[] windows ) {
        var inst = CreateEditor( windows );

        inst.titleContent = new GUIContent( title );

        return inst;
    }
}