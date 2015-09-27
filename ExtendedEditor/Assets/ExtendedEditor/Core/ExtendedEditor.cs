#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TNRD.Json;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// The base for every editor made with the Extended Editor framework
    /// </summary>
    public class ExtendedEditor : EditorWindow {

        /// <summary>
        /// If true the editor will try to repaint every update
        /// </summary>
        [JsonProperty]
        public bool RepaintOnUpdate = false;

        /// <summary>
        /// The active shared objects in this editor
        /// </summary>
        [JsonProperty]
        protected Dictionary<string, ExtendedSharedObject> SharedObjects = new Dictionary<string, ExtendedSharedObject>();

        /// <summary>
        /// An asset manager that can be accessed from every window
        /// </summary>
        [JsonProperty]
        public ExtendedAssets SharedAssets;

        /// <summary>
        /// The active windows in this editor
        /// </summary>
        [JsonProperty]
        protected List<ExtendedWindow> Windows = new List<ExtendedWindow>();

        [JsonIgnore]
        private List<ExtendedWindow> windowsToProcess = new List<ExtendedWindow>();

        private List<ExtendedWindow> windowsToRemove = new List<ExtendedWindow>();

        private Dictionary<Type, List<ExtendedWindow>> windowsDict = new Dictionary<Type, List<ExtendedWindow>>();

        private ExtendedModalWindow modalWindow;

        private Action<ExtendedModalWindowEventArgs> modalWindowCallback;

        private double previousTime = 0;

        private ExtendedWindow windowToResize = null;

        private ExtendedWindow windowToDrag = null;

        /// <summary>
        /// The time in seconds it took to complete the last frame
        /// </summary>
        [JsonIgnore]
        public float DeltaTime = 0;

        /// <summary>
        /// The input handler for the editor
        /// </summary>
        [JsonIgnore]
        public ExtendedInput Input { get; private set; }

        /// <summary>
        /// The current event being processed by every input manager
        /// </summary>
        [JsonIgnore]
        public Event CurrentEvent { get; private set; }

        private object initializer;
        private object guiInitializer;

        /// <summary>
        /// Called on the first frame of the editors lifetime
        /// </summary>
        protected virtual void OnInitialize() {
            initializer = new object();

            SharedAssets = new ExtendedAssets( this );
            Input = new ExtendedInput();

            Windows = new List<ExtendedWindow>();
            windowsToProcess = new List<ExtendedWindow>();
            modalWindow = null;
            modalWindowCallback = null;


            SharedAssets.FromBase64( "CloseActive",
                @"iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA4RpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNS1jMDIxIDc5LjE1NTc3MiwgMjAxNC8wMS8xMy0xOTo0NDowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo4MWRjMDQyMi01ZGJmLTQ0NDgtODVhNC1iN2IzYzJhM2IzNmQiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QzdBQkJERjMyOENCMTFFNUJCNENBMzIyQUY5MDU1MDciIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QzdBQkJERjIyOENCMTFFNUJCNENBMzIyQUY5MDU1MDciIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTQgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6OTg1NTdmYTctNDZkMi00YzRiLWJkMmYtNjJhZTVmNzBkNjhkIiBzdFJlZjpkb2N1bWVudElEPSJhZG9iZTpkb2NpZDpwaG90b3Nob3A6YTc2NmFhMjQtMjhjYS0xMWU1LTg1MTgtYjk3OWUxOThhMWQ0Ii8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+oeLN7AAAAK1JREFUeNpi/P//PwOpgAVEFBYWegKpuUAsiUftcyBO7u/v384EFQBpCANiRjw4DKoOYhPUhiPS0tJNUH5dSUkJmNHT0wMWe/r0aR3MJSxYnFELVQxS1ATlN2P4CQnUIWtE0lCHTxO6RgwNIMCEzIH6A9lJtVA+A06boJ5Gt6EWzQVYnYesoQ5n5EIjzgYarLj8aA3EL5A1pQDxaiCWwJMinoJSBIjBSE7aAwgwACXrMflUdZGBAAAAAElFTkSuQmCC" );
            SharedAssets.FromBase64( "CloseNormal",
                @"iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA4RpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNS1jMDIxIDc5LjE1NTc3MiwgMjAxNC8wMS8xMy0xOTo0NDowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo4MWRjMDQyMi01ZGJmLTQ0NDgtODVhNC1iN2IzYzJhM2IzNmQiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QzgzNjI2MjEyOENCMTFFNUFFNTJDNjM1REM5RUIyNzMiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QzgzNjI2MjAyOENCMTFFNUFFNTJDNjM1REM5RUIyNzMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTQgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6OTg1NTdmYTctNDZkMi00YzRiLWJkMmYtNjJhZTVmNzBkNjhkIiBzdFJlZjpkb2N1bWVudElEPSJhZG9iZTpkb2NpZDpwaG90b3Nob3A6YTc2NmFhMjQtMjhjYS0xMWU1LTg1MTgtYjk3OWUxOThhMWQ0Ii8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+dLJBTAAAAGRJREFUeNpi/P//PwOpgImBDECWJhZkTk9PTxOUWVdSUoIiBuTXYdUEBbVQxSBFTVB+M06bQDYga0TSUIdPE7pGDA0YAQH1B7KTaqF8ggGBbkMtmguwOg9ZQx22IGcc3CkCIMAAVKIgsARntswAAAAASUVORK5CYII=" );
            SharedAssets.FromBase64( "MaximizeActive",
                @"iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA3ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNS1jMDIxIDc5LjE1NTc3MiwgMjAxNC8wMS8xMy0xOTo0NDowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo4MWRjMDQyMi01ZGJmLTQ0NDgtODVhNC1iN2IzYzJhM2IzNmQiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6Nzg3RUY3QUQyOENBMTFFNUJENkJEQjZFQzlDOEIyOUUiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6Nzg3RUY3QUMyOENBMTFFNUJENkJEQjZFQzlDOEIyOUUiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTQgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6NGMxNjZhNWEtNjU4MS0xMzQzLWFlYTQtMTExMDQ4MWUyNWVkIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjgxZGMwNDIyLTVkYmYtNDQ0OC04NWE0LWI3YjNjMmEzYjM2ZCIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PqdYt6kAAACKSURBVHjaYvz//z8DqYAFRBQWFnoCqblALIlH7XMgTu7v79/OBBUAaQgDYkY8OAyqjgGmCWTDEQKuOgJzCQu6jLS0NIYnnz59yojMZ2IgA7BgEywpKYGb3NPTg2Ez9WzCZjpeTeiexgaYkCLOhoBaayB+gWxTChCvBmIJPJqeglIEiMFITtoDCDAAOuwhzlHH0qQAAAAASUVORK5CYII=" );
            SharedAssets.FromBase64( "MaximizeNormal",
                @"iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA3ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNS1jMDIxIDc5LjE1NTc3MiwgMjAxNC8wMS8xMy0xOTo0NDowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo4MWRjMDQyMi01ZGJmLTQ0NDgtODVhNC1iN2IzYzJhM2IzNmQiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QjNFN0VFOTEyOEM2MTFFNUE5MDFENTY1N0FFMzcyMjciIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QjNFN0VFOTAyOEM2MTFFNUE5MDFENTY1N0FFMzcyMjciIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTQgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6ZTFiZmY3ZTAtNjNlNy05MDRkLTgxMmUtZTYzZTBhODhmNzQyIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjgxZGMwNDIyLTVkYmYtNDQ0OC04NWE0LWI3YjNjMmEzYjM2ZCIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pubx6yMAAAA2SURBVHjaYvz//z8DqYCJgQxAP00s6AI9PT0YniwpKWGkvk3oJmOzmXo2YTMdGTAOw8gFCDAAh1MQvM4df+EAAAAASUVORK5CYII=" );
        }

        /// <summary>
        /// Called when the editor gets closed
        /// </summary>
        protected virtual void OnDestroy() {
            for ( int i = Windows.Count - 1; i >= 0; i-- ) {
                Windows[i].OnDestroy();
            }
        }

        /// <summary>
        /// Called when the editor gets keyboard focus
        /// </summary>
        protected virtual void OnFocus() {
            for ( int i = Windows.Count - 1; i >= 0; i-- ) {
                Windows[i].OnFocus();
                if ( Windows[i].Settings.IsBlocking ) return;
            }
        }

        /// <summary>
        /// Called when the editor loses keyboard focus
        /// </summary>
        protected virtual void OnLostFocus() {
            for ( int i = Windows.Count - 1; i >= 0; i-- ) {
                Windows[i].OnLostFocus();
                if ( Windows[i].Settings.IsBlocking ) return;
            }
        }

        /// <summary>
        /// Called 100 times per second
        /// </summary>
        protected virtual void Update() {
            if ( initializer == null ) return;

            var time = Time.realtimeSinceStartup;
            // Min-Maxing this to make sure it's between 0 and 1/60
            DeltaTime = Mathf.Min( Mathf.Max( 0, (float)( time - previousTime ) ), 0.016f );
            previousTime = time;

            var hasFocus = focusedWindow == this;

            for ( int i = Windows.Count - 1; i >= 0; i-- ) {
                Windows[i].Update( hasFocus && modalWindow == null );

                if ( Windows[i].Settings.IsBlocking ) {
                    break;
                }
            }

            if ( modalWindow != null ) {
                modalWindow.Update( hasFocus );
            }

            foreach ( var item in SharedObjects ) {
                item.Value.Update( hasFocus );
            }

            Input.Update();

            if ( RepaintOnUpdate ) {
                Repaint();
            }

            if ( windowsToRemove.Count > 0 ) {
                foreach ( var window in windowsToRemove ) {
                    if ( window.IsInitialized ) {
                        window.OnDestroy();
                    }

                    windowsDict[window.GetType()].Remove( window );
                    Windows.Remove( window );
                }
            }
        }

        #region GUI
        protected virtual void OnInitializeGUI() {
            guiInitializer = new object();
        }

        /// <summary>
        /// Implement your own GUI logic here
        /// </summary>
        protected virtual void OnGUI() {
            if ( initializer == null ) {
                OnInitialize();
                return;
            }

            if ( guiInitializer == null ) {
                OnInitializeGUI();
                return;
            }

            CurrentEvent = Event.current;
            Input.OnGUI( CurrentEvent );

            if ( CurrentEvent != null ) {
                switch ( CurrentEvent.type ) {
                    case EventType.ContextClick:
                        OnContextClick( CurrentEvent.mousePosition );
                        break;
                    case EventType.DragExited:
                        OnDragExited();
                        break;
                    case EventType.DragPerform:
                        OnDragPerform( DragAndDrop.paths, CurrentEvent.mousePosition );
                        break;
                    case EventType.DragUpdated:
                        OnDragUpdate( DragAndDrop.paths, CurrentEvent.mousePosition );
                        break;
                }
            }

            windowsToProcess = new List<ExtendedWindow>( Windows );

            BeginWindows();
            WindowsGUI();
            ModalWindowGUI();
            EndWindows();

            HandleWindowDragAndResize();
        }

        private void WindowsGUI() {
            for ( int i = windowsToProcess.Count - 1; i >= 0; i-- ) {
                var w = windowsToProcess[i];
                w.WindowID = i;

                if ( w.WindowStyle == null ) {
                    GUI.Window( w.WindowID, w.WindowRect, w.InternalGUI, w.WindowContent );
                } else {
                    GUI.Window( w.WindowID, w.WindowRect, w.InternalGUI, w.WindowContent, w.WindowStyle );
                }

                if ( w.Settings.IsBlocking ) {
                    break;
                }
            }
        }
        private void ModalWindowGUI() {
            if ( modalWindow != null ) {
                GUI.BringWindowToFront( windowsToProcess.Count );
                var p1 = modalWindow.WindowRect;

                if ( modalWindow.WindowStyle == null ) {
                    modalWindow.WindowRect = GUI.Window( windowsToProcess.Count, modalWindow.WindowRect, modalWindow.OnGUI, modalWindow.Title );
                } else {
                    modalWindow.WindowRect = GUI.Window( windowsToProcess.Count, modalWindow.WindowRect, modalWindow.OnGUI, modalWindow.Title, modalWindow.WindowStyle );
                }

                GUI.FocusWindow( windowsToProcess.Count );
                if ( p1 != modalWindow.WindowRect ) {
                    Event.current.Use();
                }

                if ( modalWindow.IsDone ) {
                    var windowCopy = modalWindow;
                    var callbackCopy = modalWindowCallback;

                    modalWindow = null;
                    modalWindowCallback = null;

                    if ( callbackCopy != null ) {
                        callbackCopy.Invoke( new ExtendedModalWindowEventArgs( windowCopy, windowCopy.Result ) );
                    }
                }
            }
        }
        private void HandleWindowDragAndResize() {
            for ( int i = 0; i < windowsToProcess.Count; i++ ) {
                var window = windowsToProcess[i];
                if ( window.Settings.AllowResize ) {
                    var rect = new Rect( window.WindowRect.position + window.WindowRect.size - new Vector2( 16, 16 ), new Vector2( 24, 24 ) );
                    if ( rect.Contains( Input.MousePosition ) ) {
                        EditorGUIUtility.AddCursorRect( rect, MouseCursor.ResizeUpLeft );

                        if ( Input.ButtonPressed( EMouseButton.Left ) ) {
                            windowToResize = window;
                        }
                    }

                    if ( Input.ButtonReleased( EMouseButton.Left ) ) {
                        windowToResize = null;
                    }
                }

                if ( window.Settings.AllowRepositioning ) {
                    var rect = new Rect( window.WindowRect.position, new Vector2( window.WindowRect.width, 16.5f ) );
                    if ( rect.Contains( Input.MousePosition ) ) {
                        if ( Input.ButtonPressed( EMouseButton.Left ) ) {
                            windowToDrag = window;
                        }
                    }

                    if ( Input.ButtonReleased( EMouseButton.Left ) ) {
                        windowToDrag = null;
                    }
                }
            }

            if ( CurrentEvent.type == EventType.MouseDrag ) {
                if ( windowToResize != null ) {
                    var size = windowToResize.Size;
                    size += Input.MouseDelta;
                    size.x = Mathf.Max( size.x, 50 );
                    size.y = Mathf.Max( size.y, 50 );
                    windowToResize.Size = size;
                }

                if ( windowToDrag != null ) {
                    windowToDrag.Position += Input.MouseDelta;
                }
            }
        }
        #endregion

        #region Window
        /// <summary>
        /// Adds a window to the editor
        /// </summary>
        /// <param name="window">The window to add</param>
        public virtual void AddWindow( ExtendedWindow window ) {
            if ( Windows.Contains( window ) ) return;

            var type = window.GetType();
            if ( !windowsDict.ContainsKey( type ) ) {
                windowsDict.Add( type, new List<ExtendedWindow>() );
            }

            windowsDict[type].Add( window );
            Windows.Add( window );

            window.Editor = this;
            if ( !window.IsInitialized ) {
                window.OnInitialize();
            }
        }

        /// <summary>
        /// Removes a window from the editor
        /// </summary>
        /// <param name="window">The window to remove</param>
        public virtual void RemoveWindow( ExtendedWindow window ) {
            windowsToRemove.Add( window );
        }

        /// <summary>
        /// Removes all windows from the editor
        /// </summary>
        public virtual void ClearWindows() {
            windowsToRemove.AddRange( Windows );
        }

        /// <summary>
        /// Returns a list of windows of the given type
        /// </summary>
        /// <returns>List of T</returns>
        public List<T> GetWindowsByType<T>() where T : ExtendedWindow {
            var type = typeof( T );
            if ( windowsDict.ContainsKey( type ) ) {
                var items = new List<T>();
                foreach ( var item in windowsDict[type] ) {
                    items.Add( item as T );
                }
                return items;
            } else {
                return new List<T>();
            }
        }

        /// <summary>
        /// Returns a list of windows of the given type
        /// </summary>
        /// <param name="type">The type of the window to find</param>
        /// <returns>List of ExtendedWindow</returns>
        public List<ExtendedWindow> GetWindowsByType( Type type ) {
            if ( windowsDict.ContainsKey( type ) ) {
                return windowsDict[type];
            } else {
                return new List<ExtendedWindow>();
            }
        }

        /// <summary>
        /// Returns a list of windows of the given type, including windows that inherit from this type
        /// </summary>
        /// <returns>List of T</returns>
        public List<T> GetWindowsByBaseType<T>() where T : ExtendedWindow {
            var type = typeof( T );
            var list = new List<T>();

            foreach ( var item in Windows ) {
                if ( item.GetType() == type ) {
                    list.Add( item as T );
                } else {
                    var baseType = item.GetType().BaseType;
                    while ( baseType != null ) {
                        if ( baseType == type ) {
                            list.Add( item as T );
                            break;
                        }
                        baseType = baseType.BaseType;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a list of windows of the given type, including windows that inherit from this type
        /// </summary>
        /// <param name="type">The type of the window to return</param>
        /// <returns>List of ExtendedWindow</returns>
        public List<ExtendedWindow> GetWindowsByBaseType( Type type ) {
            var list = new List<ExtendedWindow>();

            foreach ( var item in Windows ) {
                if ( item.GetType() == type ) {
                    list.Add( item );
                } else {
                    var baseType = item.GetType().BaseType;
                    while ( baseType != null ) {
                        if ( baseType == type ) {
                            list.Add( item );
                            break;
                        }
                        baseType = baseType.BaseType;
                    }
                }
            }

            return list;
        }
        #endregion

        #region Modal Window
        /// <summary>
        /// Adds a modal window to the editor
        /// </summary>
        /// <param name="window">The modal window to show</param>
        public void ShowModalWindow( ExtendedModalWindow window ) {
            ShowModalWindow( window, null );
        }

        /// <summary>
        /// Adds a modal window to the editor
        /// </summary>
        /// <param name="window">The modal window to show</param>
        /// <param name="callback">The action to invoke when the modal window gets closed</param>
        public void ShowModalWindow( ExtendedModalWindow window, Action<ExtendedModalWindowEventArgs> callback ) {
            modalWindow = window;
            modalWindow.Editor = this;

            modalWindowCallback = callback;
        }
        #endregion

        #region Shared Object
        /// <summary>
        /// Adds a shared object to the editor
        /// </summary>
        /// <param name="key">The key to store the shared object with</param>
        /// <param name="value">The shared object to store</param>
        public virtual void AddSharedObject( string key, ExtendedSharedObject value ) {
            AddSharedObject( key, value, true );
        }

        /// <summary>
        /// Adds a shared object to the editor
        /// </summary>
        /// <param name="key">The key to store the shared object with</param>
        /// <param name="value">The shared object to store</param>
        /// <param name="overwrite">Should the object be overwritten if the key already exists</param>
        public virtual void AddSharedObject( string key, ExtendedSharedObject value, bool overwrite ) {
            if ( SharedObjects.ContainsKey( key ) && !overwrite ) return;

            if ( SharedObjects.ContainsKey( key ) ) {
                SharedObjects[key] = value;
            } else {
                SharedObjects.Add( key, value );
            }
        }

        /// <summary>
        /// Removes a shared object from the editor
        /// </summary>
        /// <param name="key">The key of the object to remove</param>
        public virtual void RemoveSharedObject( string key ) {
            if ( !SharedObjects.ContainsKey( key ) ) return;
            SharedObjects.Remove( key );
        }

        /// <summary>
        /// Removes all shared objects from the editor
        /// </summary>
        public virtual void ClearSharedObjects() {
            SharedObjects.Clear();
        }

        /// <summary>
        /// Gets the shared object stored with the given key
        /// </summary>
        /// <param name="key">The key the object is stored with</param>
        /// <returns>A shared object of type T or null if not found</returns>
        public T GetSharedObject<T>( string key ) where T : ExtendedSharedObject {
            if ( SharedObjects.ContainsKey( key ) ) {
                return SharedObjects[key] as T;
            } else {
                return null;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Invoked when a ContextClick event occurs
        /// </summary>
        /// <param name="position">The location of the right-mouse click</param>
        protected virtual void OnContextClick( Vector2 position ) {
            if ( modalWindow != null ) {
                modalWindow.OnContextClick( position );
            }
        }

        /// <summary>
        /// Invoked when a DragExited event occurs
        /// </summary>
        protected virtual void OnDragExited() {
            if ( modalWindow != null ) {
                modalWindow.OnDragExited();
            }
        }

        /// <summary>
        /// Invoked when a DragPerform event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the edito</param>
        /// <param name="position">The mouse position</param>
        protected virtual void OnDragPerform( string[] paths, Vector2 position ) {
            if ( modalWindow != null ) {
                modalWindow.OnDragPerform( paths, position );
            }
        }

        /// <summary>
        /// Invoked when a DragUpdate event occurs
        /// </summary>
        /// <param name="paths">Path(s) of the file(s) being dragged onto the editor</param>
        /// <param name="position">The mouse position</param>
        protected virtual void OnDragUpdate( string[] paths, Vector2 position ) {
            if ( modalWindow != null ) {
                modalWindow.OnDragUpdate( paths, position );
            }
        }
        #endregion

        #region Serialization/Deserialization
        /// <summary>
        /// Serializes the editor as a whole in JSON
        /// </summary>
        /// <returns>JSON</returns>
        public string SerializeEditor() {
            return Serialize( this );
        }

        /// <summary>
        /// Serializes the given object in JSON
        /// </summary>
        /// <param name="item">The object to serialize</param>
        /// <returns>JSON</returns>
        public string Serialize( object item ) {
            var settings = new JsonSerializerSettings();
            settings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.TypeNameHandling = TypeNameHandling.Auto;

            try {
                var serialized = JsonConvert.SerializeObject( item, Formatting.Indented, settings );
                return serialized;
            } catch ( JsonSerializationException ex ) {
                Debug.LogErrorFormat( "Error serializing: {0}", ex.Message );
                return "";
            }
        }

        /// <summary>
        /// Saves a serialized state of the editor in the preferences
        /// </summary>
        /// <param name="key">The key to store the serialized editor with</param>
        /// <returns>True on success, false on fail</returns>
        public bool SaveToPreferences( string key ) {
            var content = SerializeEditor();
            if ( string.IsNullOrEmpty( content ) ) {
                Debug.LogError( "Unable to save to preferences, error while serializing." );
                return false;
            } else {
                return SaveToPreferences( key, content );
            }
        }

        /// <summary>
        /// Saves content in the preferences
        /// </summary>
        /// <param name="key">The key to store the content with</param>
        /// <param name="content">The content to store</param>
        /// <returns>True on success, false on fail</returns>
        public bool SaveToPreferences( string key, string content ) {
            try {
                if ( string.IsNullOrEmpty( key ) ) {
                    Debug.LogError( "Unable to save to preferences, key cannot be empty." );
                    return false;
                } else if ( string.IsNullOrEmpty( content ) ) {
                    Debug.LogError( "Unable to save to preferences, content cannot be empty." );
                    return false;
                }

                PlayerPrefs.SetString( key, content );
                PlayerPrefs.Save();
                return true;
            } catch ( PlayerPrefsException ) {
                Debug.LogError( "Unabled to save to preferences, exceeding maximum size." );
                return false;
            }
        }

        /// <summary>
        /// Saves a serialized state of the editor to a file
        /// </summary>
        /// <param name="path">The path to the file to save</param>
        /// <returns>True on success, false on fail</returns>
        public bool SaveToFile( string path ) {
            var content = SerializeEditor();
            if ( string.IsNullOrEmpty( content ) ) {
                Debug.LogError( "Unable to save to file, error while serializing" );
                return false;
            } else {
                return SaveToFile( path, content );
            }
        }

        /// <summary>
        /// Saves the string to a file
        /// </summary>
        /// <param name="path">The path to the file to save</param>
        /// <param name="content">The content to save</param>
        /// <returns>True on success, false on fail</returns>
        public bool SaveToFile( string path, string content ) {
            try {
                if ( string.IsNullOrEmpty( path ) ) {
                    Debug.LogError( "Unable to save to file, path cannot be empty." );
                    return false;
                } else if ( string.IsNullOrEmpty( content ) ) {
                    Debug.LogError( "Unable to save to file, content cannot be empty." );
                    return false;
                }

                File.WriteAllText( path, content );
                return true;
            } catch ( PathTooLongException ) {
                Debug.LogError( "Unable to save to file, path is too long." );
                return false;
            } catch ( NotSupportedException ) {
                Debug.LogError( "Unable to save to file, check your path format." );
                return false;
            } catch ( System.Security.SecurityException ) {
                Debug.LogError( "Unable to save to file, lacking permission to write the file." );
                return false;
            } catch ( UnauthorizedAccessException ) {
                Debug.LogError( "Unable to save to file, check your permissions, if the file is writable, and if you're on the right platform." );
                return false;
            } catch ( DirectoryNotFoundException ) {
                Debug.LogError( "Unable to save to file, no such directory." );
                return false;
            } catch ( IOException ) {
                Debug.LogError( "Unable to save to file, IO exception." );
                return false;
            }
        }

        /// <summary>
        /// Deserializes a JSON string into an editor
        /// </summary>
        /// <param name="value">A JSON string representing an ExtendedEditor</param>
        public void DeserializeEditor<T>( string value ) where T : ExtendedEditor {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            ExtendedEditor deserialized = null;

            try {
                deserialized = JsonConvert.DeserializeObject<T>( value, settings );
            } catch ( JsonReaderException ex ) {
                deserialized = null;
                Debug.LogErrorFormat( "Error deserializing: {0}", ex.Message );
            }

            if ( deserialized != null ) {
                RepaintOnUpdate = deserialized.RepaintOnUpdate;
                SharedObjects = deserialized.SharedObjects;
                Windows = deserialized.Windows;
                windowsDict = deserialized.windowsDict;

                for ( int i = Windows.Count - 1; i >= 0; i-- ) {
                    var w = Windows[i];

                    w.Editor = this;

                    var t = w.GetType();

                    if ( !windowsDict.ContainsKey( t ) ) {
                        windowsDict.Add( t, new List<ExtendedWindow>() );
                    }

                    if ( !windowsDict[t].Contains( w ) ) {
                        windowsDict[t].Add( w );
                    }

                    w.OnDeserialized();
                }
            }
        }

        /// <summary>
        /// Deserializes a JSON string
        /// </summary>
        /// <param name="value">The JSON string to deserialize</param>
        /// <returns>A deserialized object of type T</returns>
        public T Deserialize<T>( string value ) {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            try {
                return JsonConvert.DeserializeObject<T>( value, settings );
            } catch ( JsonReaderException ex ) {
                Debug.LogErrorFormat( "Error deserializing: {0}", ex.Message );
                return default( T );
            }
        }

        /// <summary>
        /// Loads a serialized object from the preferences
        /// </summary>
        /// <param name="key">The key that the editor is stored with</param>
        /// <returns>A deserialized object of type T</returns>
        public T LoadFromPreferences<T>( string key ) {
            if ( PlayerPrefs.HasKey( key ) ) {
                try {
                    return Deserialize<T>( PlayerPrefs.GetString( key ) );
                } catch ( Exception ) {
                    Debug.LogError( "Unabled to deserialize content." );
                    return default( T );
                }
            } else {
                Debug.LogError( "Unabled to deserialize content, key does not exist" );
                return default( T );
            }
        }

        /// <summary>
        /// Loads a serialized object from a file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns>A deserialized object of type T</returns>
        public T LoadFromFile<T>( string path ) {
            if ( string.IsNullOrEmpty( path ) ) {
                Debug.LogError( "Path is empty, cancelling LoadFromFile." );
                return default( T );
            }

            try {
                var content = File.ReadAllText( path );
                return Deserialize<T>( content );
            } catch ( FileNotFoundException ) {
                Debug.LogError( "Unable to deserialize content, no such file." );
                return default( T );
            } catch ( NotSupportedException ) {
                Debug.LogError( "Unable to deserialize content, check your path format." );
                return default( T );
            } catch ( System.Security.SecurityException ) {
                Debug.LogError( "Unable to deserialize content, lacking permission to read the file." );
                return default( T );
            } catch ( DirectoryNotFoundException ) {
                Debug.LogError( "Unable to deserialize content, no such directory." );
                return default( T );
            } catch ( IOException ) {
                Debug.LogError( "Unable to deserialize content, IO exception." );
                return default( T );
            } catch ( Exception ex ) {
                Debug.LogError( ex );
                Debug.LogError( "Unabled to deserialize content." );
                return default( T );
            }
        }
        #endregion

        /// <summary>
        /// Destroys the given asset
        /// </summary>
        /// <param name="obj">The asset to destroy</param>
        public void DestroyAsset( UnityEngine.Object obj ) {
            DestroyImmediate( obj );
        }

        /// <summary>
        /// Looks for the asset by name and then destroys the first occurence
        /// </summary>
        /// <param name="name">The name of the asset to destroy</param>
        public void DestroyAsset( string name ) {
            var g = GameObject.Find( name );
            if ( g != null ) {
                DestroyAsset( g );
            }
        }

        /// <summary>
        /// Looks for assets with the given names and then destroys the first instances of them
        /// </summary>
        /// <param name="names">The names of the assets to destroy</param>
        public void DestroyAsset( params string[] names ) {
            GameObject g;
            foreach ( var item in names ) {
                g = GameObject.Find( item );
                if ( g != null ) {
                    DestroyAsset( g );
                }
            }
        }

        /// <summary>
        /// Destroys the given asset
        /// </summary>
        /// <param name="obj">The asset to destroy</param>
        /// <param name="allowDestroyingAssets">True means allowing an asset to be deleted from disk</param>
        public void DestroyAsset( UnityEngine.Object obj, bool allowDestroyingAssets ) {
            DestroyImmediate( obj, allowDestroyingAssets );
        }
    }
}
#endif