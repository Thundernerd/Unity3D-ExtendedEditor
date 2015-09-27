#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TNRD.Editor.Core {

    /// <summary>
    /// The input manager for every editor
    /// </summary>
    public class ExtendedInput {

        private Dictionary<KeyCode, State<bool>> kStates = new Dictionary<KeyCode, State<bool>>();
        private Dictionary<EMouseButton, State<bool>> mStates = new Dictionary<EMouseButton, State<bool>>();

        /// <summary>
        /// The current mouse position in screen coordinates
        /// </summary>
        public Vector2 MousePosition;
        /// <summary>
        /// The current mouse position in world coordinates
        /// </summary>
        public Vector2 MouseWorldPosition {
            get { return ExtendedWindow.ToWorldPosition( MousePosition ); }
        }

        /// <summary>
        /// The current mouse delta in the editor/window (including scrollwheel delta)
        /// </summary>
        public Vector2 MouseDelta { get; private set; }

        private Vector2 scrollDelta;

        /// <summary>
        /// The current scrollwheel delta
        /// </summary>
        public Vector2 ScrollDelta { get { return scrollDelta; } }

        /// <summary>
        /// Did a double click occur. Check which button with \"Button\"
        /// </summary>
        public bool IsDoubleClick { get; private set; }

        /// <summary>
        /// The mouse button that invoked a double click
        /// </summary>
        public EMouseButton Button { get; private set; }

        /// <summary>
        /// The current event that is being processed
        /// </summary>
        public EventType Type { get; private set; }

        private long lastClick = 0;
        private int lastButton = -1;

        public ExtendedInput() {
            MousePosition = new Vector2();
            MouseDelta = new Vector2();
        }

        /// <summary>
        /// Called 100 times per second
        /// </summary>
        public void Update() {
            var kCopy = new Dictionary<KeyCode, State<bool>>( kStates );
            foreach ( var item in kCopy ) {
                var value = kCopy[item.Key];
                value.Update();
                kStates[item.Key] = value;
            }

            var mCopy = new Dictionary<EMouseButton, State<bool>>( mStates );
            foreach ( var item in mCopy ) {
                var value = mCopy[item.Key];
                value.Update();
                mStates[item.Key] = value;
            }

            IsDoubleClick = false;
        }

        /// <summary>
        /// Handles the input events
        /// </summary>
        public void OnGUI( Event e ) {
            if ( e == null ) return;

            Type = e.type;

            HandleKeys( e );
            HandleMouse( e );
        }

        private void HandleKeys( Event e ) {
            switch ( e.type ) {
                case EventType.KeyDown:
                    SetValue( e.keyCode, true );
                    ShiftHack( e.shift );
                    ControlHack( e.control );
                    AltHack( e.alt );
                    break;
                case EventType.KeyUp:
                    SetValue( e.keyCode, false );
                    ShiftHack( e.shift );
                    ControlHack( e.control );
                    AltHack( e.alt );
                    break;
                case EventType.Repaint:
                case EventType.Layout:
                    ShiftHack( e.shift );
                    break;
                default:
                    break;
            }

#if UNITY_EDITOR_OSX
		if ( e.isKey ) {
			if ( e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.UpArrow ) {
				e.Use();
			}
		}
#endif
        }

        private void HandleMouse( Event e ) {
            scrollDelta.x = scrollDelta.y = 0;

            switch ( e.type ) {
                case EventType.MouseDown:
                    var click = DateTime.Now.Ticks;
                    var button = e.button;

                    if ( click - lastClick < 2500000 && button == lastButton ) {
                        IsDoubleClick = true;
                        Button = (EMouseButton)e.button;
                    }

                    lastClick = click;
                    lastButton = button;

                    SetValue( (EMouseButton)e.button, true );
                    break;
                case EventType.MouseUp:
                    SetValue( (EMouseButton)e.button, false );
                    break;
                case EventType.ScrollWheel:
                    scrollDelta = e.delta;
                    break;
                default:
                    break;
            }

            MousePosition = e.mousePosition;
            MouseDelta = e.delta;
        }

        private void SetValue( EMouseButton button, bool value ) {
            if ( !mStates.ContainsKey( button ) ) {
                mStates.Add( button, new State<bool>() );
            }

            var state = mStates[button];
            state.Current = value;
            mStates[button] = state;
        }
        private void SetValue( KeyCode key, bool value ) {
            if ( !kStates.ContainsKey( key ) ) {
                kStates.Add( key, new State<bool>() );
            }

            var state = kStates[key];
            state.Current = value;
            kStates[key] = state;
        }

        private void ShiftHack( bool value ) {
            SetValue( KeyCode.LeftShift, value );
            SetValue( KeyCode.RightShift, value );
        }
        private void ControlHack( bool value ) {
            SetValue( KeyCode.LeftControl, value );
            SetValue( KeyCode.RightControl, value );
        }
        private void AltHack( bool value ) {
            SetValue( KeyCode.LeftAlt, value );
            SetValue( KeyCode.RightAlt, value );
        }

        private bool PreInputCheck() {
            return ( Type != EventType.Used && Type != EventType.Ignore ) && ( Type == EventType.KeyDown || Type == EventType.KeyUp || Type == EventType.MouseDown || Type == EventType.MouseUp || Type == EventType.MouseMove || Type == EventType.MouseDrag );
        }

        /// <summary>
        /// Did the given mouse button get pressed this frame
        /// </summary>
        /// <param name="button">the mouse button to check</param>
        public bool ButtonPressed( EMouseButton button ) {
            if ( !PreInputCheck() ) return false;
            if ( !mStates.ContainsKey( button ) ) return false;
            return mStates[button].IsPressed();
        }

        /// <summary>
        /// Did the given mouse button get released this frame
        /// </summary>
        /// <param name="button">the mouse button to check</param>
        public bool ButtonReleased( EMouseButton button ) {
            if ( !PreInputCheck() ) return false;
            if ( !mStates.ContainsKey( button ) ) return false;
            return mStates[button].IsReleased();
        }

        /// <summary>
        /// Is the given mouse button down
        /// </summary>
        /// <param name="button">the mouse button to check</param>
        public bool ButtonDown( EMouseButton button ) {
            if ( !PreInputCheck() ) return false;
            if ( !mStates.ContainsKey( button ) ) return false;
            return mStates[button].IsDown();
        }

        /// <summary>
        /// Is the given mouse button up
        /// </summary>
        /// <param name="button">the mouse button to check</param>
        public bool ButtonUp( EMouseButton button ) {
            if ( !PreInputCheck() ) return false;
            if ( !mStates.ContainsKey( button ) ) return false;
            return mStates[button].IsUp();
        }

        /// <summary>
        /// Did the given key get pressed this frame
        /// </summary>
        /// <param name="key">the key to check</param>
        public bool KeyPressed( KeyCode key ) {
            if ( !PreInputCheck() ) return false;
            if ( !kStates.ContainsKey( key ) ) return false;
            return kStates[key].IsPressed();
        }

        /// <summary>
        /// Did one of the given keys get pressed this frame
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeyPressed( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( kStates.ContainsKey( key ) ) {
                    if ( kStates[key].IsPressed() ) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Did all of the given keys get pressed this frame
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeysPressed( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( !kStates.ContainsKey( key ) ) return false;
                if ( !kStates[key].IsPressed() ) return false;
            }
            return true;
        }

        /// <summary>
        /// Did the given key get released this frame
        /// </summary>
        /// <param name="key">the key to check</param>
        public bool KeyReleased( KeyCode key ) {
            if ( !PreInputCheck() ) return false;
            if ( !kStates.ContainsKey( key ) ) return false;
            return kStates[key].IsReleased();
        }

        /// <summary>
        /// Did one of the given keys get released this frame
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeyReleased( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( kStates.ContainsKey( key ) ) {
                    if ( kStates[key].IsReleased() ) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Did all of the given keys get released this frame
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeysReleased( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( !kStates.ContainsKey( key ) ) return false;
                if ( !kStates[key].IsReleased() ) return false;
            }
            return true;
        }

        /// <summary>
        /// Is the given key down
        /// </summary>
        /// <param name="key">the key to check</param>
        public bool KeyDown( KeyCode key ) {
            if ( !PreInputCheck() ) return false;
            if ( !kStates.ContainsKey( key ) ) return false;
            return kStates[key].IsDown();
        }

        /// <summary>
        /// Is one of the given keys down
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeyDown( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( kStates.ContainsKey( key ) ) {
                    if ( kStates[key].IsDown() ) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Are all of the given keys down
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeysDown( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( !kStates.ContainsKey( key ) ) return false;
                if ( !kStates[key].IsDown() ) return false;
            }
            return true;
        }

        /// <summary>
        /// Is the given key up
        /// </summary>
        /// <param name="key">the key to check</param>
        public bool KeyUp( KeyCode key ) {
            if ( !PreInputCheck() ) return false;
            if ( !kStates.ContainsKey( key ) ) return false;
            return kStates[key].IsUp(); ;
        }

        /// <summary>
        /// Is one of the given keys up
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeyUp( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( kStates.ContainsKey( key ) ) {
                    if ( kStates[key].IsUp() ) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Are all of the given keys up
        /// </summary>
        /// <param name="keys">the keys to check</param>
        public bool KeysUp( params KeyCode[] keys ) {
            if ( !PreInputCheck() ) return false;
            foreach ( var key in keys ) {
                if ( !kStates.ContainsKey( key ) ) return false;
                if ( !kStates[key].IsUp() ) return false;
            }
            return true;
        }

        /// <summary>
        /// Resets all the key and mousebutton states
        /// </summary>
        public void Reset() {
            var keyStates = new Dictionary<KeyCode, State<bool>>( kStates );
            foreach ( var item in keyStates ) {
                SetValue( item.Key, false );
                item.Value.Update();
            }

            var mouseStates = new Dictionary<EMouseButton, State<bool>>( mStates );
            foreach ( var item in mouseStates ) {
                SetValue( item.Key, false );
                item.Value.Update();
            }
        }
    }
}
#endif