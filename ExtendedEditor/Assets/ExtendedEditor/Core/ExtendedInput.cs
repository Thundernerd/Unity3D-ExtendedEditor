#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TNRD.Editor.Core {
	public class ExtendedInput {

		private Dictionary<KeyCode, State<bool>> kStates = new Dictionary<KeyCode, State<bool>>();
		private Dictionary<EMouseButton, State<bool>> mStates = new Dictionary<EMouseButton, State<bool>>();

		public Vector2 MousePosition { get; set; }
		public Vector2 MouseDelta { get; private set; }

		private Vector2 scrollDelta;
		public Vector2 ScrollDelta { get { return scrollDelta; } }

		public bool IsDoubleClick { get; private set; }
		public EMouseButton Button { get; private set; }

		public EventType Type { get; private set; }

		private long lastClick = 0;
		private int lastButton = -1;

		public ExtendedInput() {
			MousePosition = new Vector2();
			MouseDelta = new Vector2();
		}

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

		public bool ButtonPressed( EMouseButton button ) {
			if ( !mStates.ContainsKey( button ) ) return false;
			return mStates[button].IsPressed();
		}
		public bool ButtonReleased( EMouseButton button ) {
			if ( !mStates.ContainsKey( button ) ) return false;
			return mStates[button].IsReleased();
		}
		public bool ButtonDown( EMouseButton button ) {
			if ( !mStates.ContainsKey( button ) ) return false;
			return mStates[button].IsDown();
		}
		public bool ButtonUp( EMouseButton button ) {
			if ( !mStates.ContainsKey( button ) ) return false;
			return mStates[button].IsUp();
		}

		public bool KeyPressed( KeyCode key ) {
			if ( !kStates.ContainsKey( key ) ) return false;
			return kStates[key].IsPressed();
		}
		public bool KeyPressed( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( kStates.ContainsKey( key ) ) {
					if ( kStates[key].IsPressed() ) {
						return true;
					}
				}
			}
			return false;
		}
		public bool KeysPressed( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( !kStates.ContainsKey( key ) ) return false;
				if ( !kStates[key].IsPressed() ) return false;
			}
			return true;
		}

		public bool KeyReleased( KeyCode key ) {
			if ( !kStates.ContainsKey( key ) ) return false;
			return kStates[key].IsReleased();
		}
		public bool KeyReleased( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( kStates.ContainsKey( key ) ) {
					if ( kStates[key].IsReleased() ) {
						return true;
					}
				}
			}
			return false;
		}
		public bool KeysReleased( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( !kStates.ContainsKey( key ) ) return false;
				if ( !kStates[key].IsReleased() ) return false;
			}
			return true;
		}

		public bool KeyDown( KeyCode key ) {
			if ( !kStates.ContainsKey( key ) ) return false;
			return kStates[key].IsDown();
		}
		public bool KeyDown( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( kStates.ContainsKey( key ) ) {
					if ( kStates[key].IsDown() ) {
						return true;
					}
				}
			}
			return false;
		}
		public bool KeysDown( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( !kStates.ContainsKey( key ) ) return false;
				if ( !kStates[key].IsDown() ) return false;
			}
			return true;
		}

		public bool KeyUp( KeyCode key ) {
			if ( !kStates.ContainsKey( key ) ) return false;
			return kStates[key].IsUp(); ;
		}
		public bool KeyUp( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( kStates.ContainsKey( key ) ) {
					if ( kStates[key].IsUp() ) {
						return true;
					}
				}
			}
			return false;
		}
		public bool KeysUp( params KeyCode[] keys ) {
			foreach ( var key in keys ) {
				if ( !kStates.ContainsKey( key ) ) return false;
				if ( !kStates[key].IsUp() ) return false;
			}
			return true;
		}

		public void Reset() {
			var keyStates = new Dictionary<KeyCode, State<bool>>( kStates );
			foreach ( var item in keyStates ) {
				SetValue( item.Key, false );
			}

			var mouseStates = new Dictionary<EMouseButton, State<bool>>( mStates );
			foreach ( var item in mouseStates ) {
				SetValue( item.Key, false );
			}
		}
	}
}
#endif