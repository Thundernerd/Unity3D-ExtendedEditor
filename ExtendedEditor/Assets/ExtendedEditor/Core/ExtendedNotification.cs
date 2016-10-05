#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace TNRD.Editor {

    public class ExtendedNotification : ExtendedControl {

        private static List<ExtendedNotification> topLeft = new List<ExtendedNotification>();
        private static List<ExtendedNotification> topRight = new List<ExtendedNotification>();
        private static List<ExtendedNotification> bottomLeft = new List<ExtendedNotification>();
        private static List<ExtendedNotification> bottomRight = new List<ExtendedNotification>();



        public GUIContent Content;
        public Color Color;
        public float Duration;
        public ENotificationLocation Location;

        private bool slideIn = false;
        private bool movingOut = false;
        private float speed = 600f;
        private float outSpeed = 600f;

        public ExtendedNotification() { }

        public ExtendedNotification( string text, Color color, ENotificationLocation location, float duration ) {
            Content = new GUIContent( text );
            Color = color;
            Location = location;
            Duration = duration;
        }

        #region Initializers
        protected override void OnInitialize() {
            SortingOrder = int.MaxValue;

            switch ( Location ) {
                case ENotificationLocation.BottomLeft:
                    InitializeBottomLeft();
                    break;
                case ENotificationLocation.BottomRight:
                    InitializeBottomRight();
                    break;
                case ENotificationLocation.TopLeft:
                    InitializeTopLeft();
                    break;
                case ENotificationLocation.TopRight:
                    InitializeTopRight();
                    break;
            }
        }

        private void InitializeBottomLeft() {
            AnchorPoint = EAnchor.BottomRight;
            bottomLeft.Add( this );

            var y = Window.Size.y - 30;
            var n = new List<ExtendedNotification>( bottomLeft );
            foreach ( var item in n ) {
                if ( item.ID == ID )
                    break;
                else {
                    y -= item.Size.y;
                    y -= 10;
                }
            }

            Position = new Vector2( -25, y );
        }

        private void InitializeBottomRight() {
            AnchorPoint = EAnchor.BottomLeft;
            bottomRight.Add( this );

            var y = Window.Size.y - 30;
            var n = new List<ExtendedNotification>( bottomRight );
            foreach ( var item in n ) {
                if ( item.ID == ID )
                    break;
                else {
                    y -= item.Size.y;
                    y -= 10;
                }
            }

            Position = new Vector2( Window.Size.x + 25, y );
        }

        private void InitializeTopLeft() {
            AnchorPoint = EAnchor.TopRight;
            topLeft.Add( this );

            var y = 30f;
            var n = new List<ExtendedNotification>( topLeft );
            foreach ( var item in n ) {
                if ( item.ID == ID )
                    break;
                else {
                    y += item.Size.y;
                    y += 10;
                }
            }

            Position = new Vector2( -25, y );
        }

        private void InitializeTopRight() {
            AnchorPoint = EAnchor.TopLeft;
            topRight.Add( this );

            var y = 30f;
            var n = new List<ExtendedNotification>( topRight );
            foreach ( var item in n ) {
                if ( item.ID == ID )
                    break;
                else {
                    y += item.Size.y;
                    y += 10f;
                }
            }

            Position = new Vector2( Window.Size.x + 25, y );
        }
        #endregion

        protected override void OnInitializeGUI() {
            ExtendedGUI.NotificationTextStyle.CalcMinMaxWidth( Content, out Size.y, out Size.x );
            Size.y = ExtendedGUI.NotificationTextStyle.CalcHeight( Content, Size.x );
            Size.x = Mathf.Max( 125, Size.x );

            outSpeed = 600f * ( Size.x / 125 );
        }

        protected override void OnDestroy() {
            switch ( Location ) {
                case ENotificationLocation.BottomLeft:
                    bottomLeft.Remove( this );
                    break;
                case ENotificationLocation.BottomRight:
                    bottomRight.Remove( this );
                    break;
                case ENotificationLocation.TopLeft:
                    topLeft.Remove( this );
                    break;
                case ENotificationLocation.TopRight:
                    topRight.Remove( this );
                    break;
            }
        }

        protected override void OnAfterSerialize() {
            Remove();
        }

        #region Updates
        protected override void OnUpdate() {
            switch ( Location ) {
                case ENotificationLocation.BottomLeft:
                    UpdateBottomLeft();
                    break;
                case ENotificationLocation.BottomRight:
                    UpdateBottomRight();
                    break;
                case ENotificationLocation.TopLeft:
                    UpdateTopLeft();
                    break;
                case ENotificationLocation.TopRight:
                    UpdateTopRight();
                    break;
            }
        }

        private void UpdateBottomLeft() {
            if ( !slideIn ) {
                Position.x = Mathf.MoveTowards( Position.x, Size.x + 25, DeltaTime * outSpeed );
                if ( Position.x >= Size.x + 25 ) {
                    slideIn = true;
                }
            } else {
                if ( Duration > 0 ) {
                    Duration -= DeltaTime;
                } else {
                    movingOut = true;
                    Position.y = Mathf.MoveTowards( Position.y, Window.Size.y + Size.y + 25, DeltaTime * speed );
                    if ( Position.y >= Window.Size.y + Size.y + 25 ) {
                        Remove();
                    }
                }
            }

            if ( !movingOut ) {
                var y = Window.Size.y - 30f;
                var n = new List<ExtendedNotification>( bottomLeft );
                foreach ( var item in n ) {
                    if ( item.ID == ID ) {
                        break;
                    } else {
                        y -= item.Size.y;
                        y -= 10f;
                    }
                }

                Position.y = Mathf.MoveTowards( Position.y, y, DeltaTime * speed );
            }
        }

        private void UpdateBottomRight() {
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
                    Position.y = Mathf.MoveTowards( Position.y, Window.Size.y + Size.y + 25, DeltaTime * speed );
                    if ( Position.y >= Window.Size.y + Size.y + 25 ) {
                        Remove();
                    }
                }
            }

            if ( !movingOut ) {
                var y = Window.Size.y - 30f;
                var n = new List<ExtendedNotification>( bottomRight );
                foreach ( var item in n ) {
                    if ( item.ID == ID ) {
                        break;
                    } else {
                        y -= item.Size.y;
                        y -= 10f;
                    }
                }

                Position.y = Mathf.MoveTowards( Position.y, y, DeltaTime * speed );
            }
        }

        private void UpdateTopLeft() {
            if ( !slideIn ) {
                Position.x = Mathf.MoveTowards( Position.x, Size.x + 25, DeltaTime * outSpeed );
                if ( Position.x >= Size.x + 25 ) {
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
                var n = new List<ExtendedNotification>( topLeft );
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

        private void UpdateTopRight() {
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
                var n = new List<ExtendedNotification>( topRight );
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
        #endregion

        public void NotificationGUI() {
            GUI.Box( Rectangle, "", ExtendedGUI.NotificationBackgroundStyle );
            var c = GUI.contentColor;
            GUI.contentColor = Color;
            GUI.Label( Rectangle, Content, ExtendedGUI.NotificationTextStyle );
            GUI.contentColor = c;

            Repaint();
        }
    }
}
#endif