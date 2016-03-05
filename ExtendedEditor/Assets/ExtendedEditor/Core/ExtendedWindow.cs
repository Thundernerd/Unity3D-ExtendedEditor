using System;
using UnityEngine;

[Serializable]
public class ExtendedWindow {
    
    public GUIContent WindowContent = new GUIContent();
    
    public Rect WindowRect = new Rect();
    
    public Vector2 Position {
        get { return WindowRect.position; }
        set { WindowRect.position = value; }
    }
    
    public Vector2 Size {
        get { return WindowRect.size; }
        set { WindowRect.size = value; }
    }
    
    public ExtendedEditor Editor;

    private bool initializedGUI = false;

    private void InternalInitialize() {
        if ( Position == Vector2.zero && Size == Vector2.zero ) {
            WindowRect = new Rect( Vector2.zero, Editor.Size );
        }

        OnInitialize();
    }

    private void InternalInitializeGUI() {
        OnInitializeGUI();
        initializedGUI = true;
    }

    private void InternalDestroy() {
        OnDestroy();
    }

    private void InternalFocus() {
        OnFocus();
    }

    private void InternalLostFocus() {
        OnLostFocus();
    }

    private void InternalInspectorUpdate() {
        OnInspectorUpdate();
    }

    private void InternalUpdate() {
        OnUpdate();
    }

    private void InternalGUI() {
        if ( !initializedGUI ) {
            InternalInitializeGUI();
        }

        OnGUI();
    }

    protected virtual void OnInitialize() {

    }

    protected virtual void OnInitializeGUI() {

    }

    protected virtual void OnDestroy() {

    }

    protected virtual void OnFocus() {

    }

    protected virtual void OnLostFocus() {

    }

    protected virtual void OnGUI() {

    }

    protected virtual void OnInspectorUpdate() {

    }

    protected virtual void OnUpdate() {

    }

    public static void CreateEditor() {
        CreateEditor( "" );
    }

    public static void CreateEditor( string title ) {
        var stack = new System.Diagnostics.StackTrace();
        if ( stack.FrameCount >= 1 ) {
            var mBase = stack.GetFrame( 1 ).GetMethod();
            var type = mBase.DeclaringType;
            var instance = (ExtendedWindow)System.Activator.CreateInstance( type );
            ExtendedEditor.CreateEditor( title, instance );
        } else {
            Debug.LogError( "Error creating editor" );
        }
    }
}