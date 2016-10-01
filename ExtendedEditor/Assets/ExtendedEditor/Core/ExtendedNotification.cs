#if UNITY_EDITOR
﻿using System.Collections.Generic;
using UnityEngine;

namespace TNRD.Editor {

    public class ExtendedNotification : ExtendedControl {

        private static List<ExtendedNotification> notifications = new List<ExtendedNotification>();

        public GUIContent Content;
        public Color Color;
        public float Duration;

        private bool slideIn = false;
        private bool movingOut = false;
        private float speed = 600f;
        private float outSpeed = 600f;

        public ExtendedNotification() { }

        public ExtendedNotification( string text, bool isError ) {
            Content = new GUIContent( text );
            Color = isError ? Color.red : Color.white;
            Duration = 1.25f;
            AnchorPoint = EAnchor.TopLeft;
        }

        protected override void OnInitialize() {
            notifications.Add( this );

            var y = 30f;
            var n = new List<ExtendedNotification>( notifications );
            foreach ( var item in n ) {
                if ( item.ID == ID ) {
                    break;
                } else {
                    y += item.Size.y;
                    y += 10f;
                }
            }

            Position = new Vector2( Window.Size.x + 25, y );
        }

        protected override void OnInitializeGUI() {
            ExtendedGUI.NotificationTextStyle.CalcMinMaxWidth( Content, out Size.y, out Size.x );
            Size.y = ExtendedGUI.NotificationTextStyle.CalcHeight( Content, Size.x );
            Size.x = Mathf.Max( 125, Size.x );

            outSpeed = 600f * ( Size.x / 125 );
        }

        protected override void OnDestroy() {
            notifications.Remove( this );
        }

        protected override void OnAfterSerialize() {
            Remove();
        }

        protected override void OnUpdate() {
            if ( !slideIn ) {
                Position.x = Mathf.MoveTowards( Position.x, Window.Size.x - ( Size.x + 25 ), DeltaTime * outSpeed );
                if ( Position.x <= Window.Size.x - ( Size.x + 25 ) ) {
                    slideIn = true;
                }
            } else {
                if ( Duration > 0 ) {
                    Duration -= DeltaTime;
                } else {
                    movingOut = true;
                    Position.y = Mathf.MoveTowards( Position.y, -( Size.y + 25 ), DeltaTime * speed );
                    if ( Position.y <= -( Size.y + 25 ) ) {
                        Remove();
                    }
                }
            }

            if ( !movingOut ) {
                var y = 30f;
                var n = new List<ExtendedNotification>( notifications );
                foreach ( var item in n ) {
                    if ( item.ID == ID ) {
                        break;
                    } else {
                        y += item.Size.y;
                        y += 10f;
                    }
                }

                Position.y = Mathf.MoveTowards( Position.y, y, DeltaTime * speed );
            }
        }

        protected override void OnGUI() {
            GUI.Box( Rectangle, "", ExtendedGUI.NotificationBackgroundStyle );
            var c = GUI.contentColor;
            GUI.contentColor = Color;
            GUI.Label( Rectangle, Content, ExtendedGUI.NotificationTextStyle );
            GUI.contentColor = c;
        }
    }
}
#endif