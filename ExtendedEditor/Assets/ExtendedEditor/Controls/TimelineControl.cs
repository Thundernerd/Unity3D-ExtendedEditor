#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TNRD.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace TNRD.Editor.Controls {
    public class TimelineControl : ExtendedControl {

        public enum EPlayMode {
            Normal,
            Reversed
        }

        public class OnFrameEventArgs : EventArgs {
            public readonly int Frame;
            public readonly bool IsMarker;
            public readonly Color MarkerColor;

            public OnFrameEventArgs( int frame, bool isMarker, Color markerColor ) : base() {
                Frame = frame;
                IsMarker = isMarker;
                MarkerColor = markerColor;
            }
        }

        public event EventHandler OnFinished;
        public event EventHandler<OnFrameEventArgs> OnFrame;
        public event EventHandler OnPlay;

        public string Name = "";

        public int FrameCount = 0;
        public int CurrentFrame = 0;

        public bool IsPlaying { get; private set; }

        public EPlayMode PlayMode = EPlayMode.Normal;

        private float fps = 30;

        private float timer = 0;

        private GUIStyle skin;
        private Color scrubberColor = new Color( 1, 0, 0 );

        private Dictionary<int, Color> markers = new Dictionary<int, Color>();

        public TimelineControl( string name = "Empty", int frameCount = 120 ) {
            Name = name;
            FrameCount = frameCount;
        }

        public override void OnInitialize() {
            base.OnInitialize();
        }

        public override void Update( bool hasFocus ) {
            base.Update( hasFocus );

            if ( Input.IsDoubleClick ) {
                var position = Input.MousePosition;
                if ( Rectangle.Contains( position ) ) {
                    var p = position.x - Position.x;
                    var fo = p / Rectangle.width;
                    CurrentFrame = Mathf.FloorToInt( FrameCount * fo );
                    Play();
                }
            }

            if ( IsPlaying ) {
                timer += Window.Editor.DeltaTime;
                if ( timer >= ( 1f / fps ) ) {
                    var isDone = false;
                    if ( PlayMode == EPlayMode.Reversed ) {
                        if ( CurrentFrame > 0 ) {
                            CurrentFrame--;
                        } else {
                            isDone = true;
                        }
                    } else if ( PlayMode == EPlayMode.Normal ) {
                        if ( CurrentFrame < FrameCount ) {
                            CurrentFrame++;
                        } else {
                            isDone = true;
                        }
                    }

                    if ( isDone ) {
                        IsPlaying = false;

                        if ( OnFinished != null ) {
                            OnFinished.Invoke( this, new EventArgs() );
                        }
                    } else {
                        timer = 0;

                        if ( OnFrame != null ) {
                            bool isMarker = markers.ContainsKey( CurrentFrame );
                            OnFrame.Invoke( this, new OnFrameEventArgs( CurrentFrame, isMarker, isMarker ? markers[CurrentFrame] : new Color() ) );
                        }
                    }
                }
            }
        }

        public override void OnGUI() {
            base.OnGUI();

            if ( skin == null ) {
                skin = new GUIStyle( EditorStyles.toolbar );
                skin.fixedHeight = 0;
            }



            var barYOffset = Rectangle.height * 0.8f;
            var barHeight = Rectangle.height * 0.2f;

            var barRect = new Rect( Rectangle.x + 1, Rectangle.y + barYOffset, Rectangle.width - 2, barHeight );
            EditorGUI.LabelField( barRect, "", skin );

            float lineOffset = fps;
            int amountOfBigLines = Mathf.FloorToInt( FrameCount / lineOffset );
            while ( amountOfBigLines > 8 ) {
                lineOffset *= 2;
                amountOfBigLines = Mathf.FloorToInt( FrameCount / lineOffset );
            }

            float frameWidth = Rectangle.width / FrameCount;
            for ( int i = 0; i < amountOfBigLines + 1; i++ ) {
                float offset = frameWidth * lineOffset * i;

                if ( i > 0 ) {
                    Handles.DrawPolyLine(
                            new Vector3( Rectangle.x + offset, Rectangle.y ),
                            new Vector3( Rectangle.x + offset, Rectangle.y + Rectangle.height - barHeight ) );

                    if ( !IsPlaying ) {
                        var content = new GUIContent( ( lineOffset * i ).ToString() );
                        var size = GUI.skin.label.CalcSize( content );
                        GUI.Label( new Rect(
                            Rectangle.x - ( size.x / 2 ) + offset,
                            Rectangle.y + barYOffset + ( barHeight / 2 ) - ( size.y / 2 ),
                            size.x, size.y ),
                            content );
                    }
                }

                float smallOffset = offset + ( frameWidth * lineOffset ) / 2;
                if ( Rectangle.x + smallOffset > Rectangle.x + Rectangle.width ) continue;

                Handles.DrawPolyLine(
                    new Vector3( Rectangle.x + smallOffset, Rectangle.y + barYOffset - barHeight ),
                    new Vector3( Rectangle.x + smallOffset, Rectangle.y + Rectangle.height ) );
            }

            foreach ( var item in markers.Keys ) {
                if ( item > FrameCount ) continue;

                var hc = Handles.color;
                Handles.color = markers[item];
                Handles.DrawAAPolyLine( 2.5f,
                    new Vector3( Rectangle.x + ( frameWidth * item ), Rectangle.y ),
                    new Vector3( Rectangle.x + ( frameWidth * item ), Rectangle.y + barYOffset ) );
                Handles.color = hc;
            }

            var hColor = Handles.color;
            Handles.color = scrubberColor;
            var linePositionX = Rectangle.width / FrameCount;
            Handles.DrawAAPolyLine( 2.5f,
                new Vector3( Rectangle.x + ( linePositionX * CurrentFrame ), Rectangle.y ),
                new Vector3( Rectangle.x + ( linePositionX * CurrentFrame ), Rectangle.y + barYOffset ) );
            Handles.color = hColor;

            var gColor = GUI.color;
            GUI.color = scrubberColor;
            var scrubberContent = new GUIContent( CurrentFrame.ToString() );
            GUI.skin.label.fontStyle = FontStyle.Bold;
            var scrubberSize = GUI.skin.label.CalcSize( scrubberContent );
            GUI.Label( new Rect( Rectangle.x + ( linePositionX * CurrentFrame ) - ( scrubberSize.x / 2 ), Rectangle.y + barYOffset + ( barHeight / 2 ) - ( scrubberSize.y / 2 ), scrubberSize.x, scrubberSize.y ), scrubberContent );
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.color = gColor;

            if ( Input.ButtonDown( EMouseButton.Left ) ) {
                if ( Rectangle.Contains( Input.MousePosition ) ) {
                    var p = Input.MousePosition.x - Position.x;
                    var fo = p / Rectangle.width;
                    CurrentFrame = Mathf.RoundToInt( FrameCount * fo );
                }
            }

            GUI.Label( Rectangle, "", GUI.skin.box );
            EditorGUI.BeginDisabledGroup( true );
            GUI.Label( Rectangle, Name );
            EditorGUI.EndDisabledGroup();
        }

        public void Play() {
            PlayMode = EPlayMode.Normal;
            IsPlaying = true;

            if ( OnPlay != null ) {
                OnPlay.Invoke( this, new EventArgs() );
            }
        }

        public void Rewind() {
            PlayMode = EPlayMode.Reversed;
            IsPlaying = true;

            if ( OnPlay != null ) {
                OnPlay.Invoke( this, new EventArgs() );
            }
        }

        public void Pause() {
            IsPlaying = false;
        }

        public void Stop() {
            IsPlaying = false;
            CurrentFrame = 0;
        }

        public void AddMarker( int frame, Color color ) {
            if ( markers.ContainsKey( frame ) ) {
                markers[frame] = color;
            } else {
                markers.Add( frame, color );
            }
        }

        public void ClearMarkers() {
            markers.Clear();
        }

        public void RemoveMarker( int frame ) {
            if ( markers.ContainsKey( frame ) ) {
                markers.Remove( frame );
            }
        }
    }
}
#endif