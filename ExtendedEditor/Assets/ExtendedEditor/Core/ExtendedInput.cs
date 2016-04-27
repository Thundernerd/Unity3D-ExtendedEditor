using System.Collections.Generic;
using TNRD.Editor.Utilities;
using UnityEngine;

namespace TNRD.Editor.Core {

    public class ExtendedInput {

        private Dictionary<KeyCode, State<bool>> keyStates = new Dictionary<KeyCode, State<bool>>();
        private Dictionary<EMouseButton, State<bool>> mouseStates = new Dictionary<EMouseButton, State<bool>>();

        public Vector2 MousePosition {
            get { return Event.current.mousePosition; }
        }

        public ExtendedInput() {
            var keycodes = System.Enum.GetValues( typeof( KeyCode ) );
            for ( int i = 0; i < keycodes.Length; i++ ) {
                var value = (KeyCode)keycodes.GetValue( i );
                if ( keyStates.ContainsKey( value ) ) continue;
                keyStates.Add( value, new State<bool>() );
            }

            var mousebuttons = System.Enum.GetValues( typeof( EMouseButton ) );
            for ( int i = 0; i < mousebuttons.Length; i++ ) {
                var value = (EMouseButton)mousebuttons.GetValue( i );
                if ( mouseStates.ContainsKey( value ) ) continue;
                mouseStates.Add( value, new State<bool>() );
            }
        }

        public void OnGUI() {
            var evt = Event.current;

            keyStates[KeyCode.LeftShift].Update( evt.shift );
            keyStates[KeyCode.RightShift].Update( evt.shift );

            switch ( evt.type ) {
                case EventType.MouseDown:
                    mouseStates[(EMouseButton)evt.button].Update( true );
                    break;
                case EventType.MouseUp:
                    mouseStates[(EMouseButton)evt.button].Update( false );
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    break;
                case EventType.KeyDown:
                    keyStates[evt.keyCode].Update( true );
                    break;
                case EventType.KeyUp:
                    keyStates[evt.keyCode].Update( false );
                    break;
                case EventType.ScrollWheel:
                    break;
                case EventType.Repaint:
                    break;
                case EventType.Layout:
                    break;
            }
        }

        public bool ButtonUp( EMouseButton button ) {
            return mouseStates[button].IsUp();
        }

        public bool ButtonDown( EMouseButton button ) {
            return mouseStates[button].IsDown();
        }

        public bool ButtonPressed( EMouseButton button ) {
            return mouseStates[button].IsPressed();
        }

        public bool ButtonReleased( EMouseButton button ) {
            return mouseStates[button].IsReleased();
        }

        public bool KeyUp( KeyCode key ) {
            return keyStates[key].IsUp();
        }

        public bool KeyDown( KeyCode key ) {
            return keyStates[key].IsDown();
        }

        public bool KeyPressed( KeyCode key ) {
            return keyStates[key].IsPressed();
        }

        public bool KeyReleased( KeyCode key ) {
            return keyStates[key].IsReleased();
        }
    }
}