#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TNRD {
	public class SelectorControl : ExtendedControl {

		private bool isSelecting = false;

		private Vector2 start;
		private Vector2 end;

		private List<SelectableControl> selectedControls = new List<SelectableControl>();

		public SelectorControl() {

		}

		public override void OnInitialize() {
			base.OnInitialize();
		}

		public override void OnDestroy() {
			base.OnDestroy();
		}

		private bool waitForNextState;
		private bool showDragField;
		private bool handleSelectedControls;
		private bool allowDrag;

		public override void OnGUI() {
			if ( waitForNextState ) {
				if ( Input.ButtonReleased( EMouseButton.Left ) ) {
					waitForNextState = false;
					showDragField = false;

					var controls = Window.GetControlsSlow<SelectableControl>();
					foreach ( var item in controls ) {
						var contains = item.Contains( Input.MousePosition );
						if ( item.IsSelected && !contains ) {
							item.OnDeselect();
						} else if ( !item.IsSelected && contains ) {
							item.OnSelect();
						}
					}
				} else if ( Input.Type == EventType.MouseDrag ) {
					var controls = Window.GetControlsSlow<SelectableControl>();
					var controlsSelected = 0;
					SelectableControl control = null;
					foreach ( var item in controls ) {
						if ( item.Contains( Input.MousePosition ) ) {
							controlsSelected++;
							control = item;
						}
					}

					if ( controlsSelected == 1 && control != null ) {
						waitForNextState = false;
						handleSelectedControls = true;
						allowDrag = true;
						control.OnSelect();
						selectedControls.Add( control );
					} else {
						waitForNextState = false;
						showDragField = true;
					}
				}
			} else {
				if ( handleSelectedControls ) {
					if ( Input.ButtonPressed( EMouseButton.Left ) ) {
						allowDrag = false;
						foreach ( var item in selectedControls ) {
							if ( item.Contains( Input.MousePosition ) ) {
								allowDrag = true;
								break;
							}
						}

						if ( !allowDrag ) {
							foreach ( var item in selectedControls ) {
								item.OnDeselect();
							}
							selectedControls.Clear();
							handleSelectedControls = false;
							waitForNextState = true;
							start = end = Input.MousePosition;
						}
					}
					if ( allowDrag && Input.Type == EventType.MouseDrag ) {
						foreach ( var item in selectedControls ) {
							item.Position += Input.MouseDelta;
						}
					}
				} else if ( showDragField ) {
					end = Input.MousePosition;

					var minx = Mathf.Min( start.x, end.x );
					var maxx = Mathf.Max( start.x, end.x );
					var miny = Mathf.Min( start.y, end.y );
					var maxy = Mathf.Max( start.y, end.y );

					var rect = new Rect( minx, miny, maxx - minx, maxy - miny );

					GUI.Box( rect, "" );

					selectedControls.Clear();
					var controls = Window.GetControlsSlow<SelectableControl>();
					foreach ( var item in controls ) {
						var contains = rect.Contains( item.Center() );
						if ( item.IsSelected && !contains ) {
							item.OnDeselect();
						} else if ( !item.IsSelected && contains ) {
							item.OnSelect();
							selectedControls.Add( item );
						} else if ( item.IsSelected && contains ) {
							selectedControls.Add( item );
						}
					}

					if ( Input.ButtonReleased( EMouseButton.Left ) ) {
						showDragField = false;
						waitForNextState = false;
						handleSelectedControls = selectedControls.Count > 0;
					}
				} else {
					if ( Input.ButtonPressed( EMouseButton.Left ) ) {
						waitForNextState = true;
						showDragField = false;
						start = end = Input.MousePosition;
					}
				}
			}
		}
	}
}
#endif