using UnityEngine;
using System.Collections;
using TNRD;
using UnityEditor;
using System;

public class ListControl : ExtendedControl {

	public class ListEventArgs : EventArgs {

		public readonly int Index;
		public readonly string Item;

		public ListEventArgs() : base() { }

		public ListEventArgs( int index, string item ) : this() {
			Index = index;
			Item = item;
		}
	}

	public event EventHandler<ListEventArgs> OnSelectedItemChanged;
	public event EventHandler<ListEventArgs> OnItemDoubleClick;

	private string[] items;

	private int index = -1;

	private bool scrollable = true;

	private Vector2 doubleClickPosition;
	private Vector2 scrollPosition;

	private Color highlightColor = new Color( 0.243f, 0.372f, 0.588f );
	private Color alternateColor = new Color( 0.267f, 0.267f, 0.267f, 0.75f );

	private ListControl() { }
	public ListControl( Vector2 position, Vector2 size, string[] items, bool scrollable = true ) {
		Position = position;
		Size = size;
		this.items = items;
		this.scrollable = scrollable;
	}

	public ListControl( string[] items ) : this( items, true ) { }
	public ListControl( string[] items, bool scrollable ) {
		this.items = items;
		this.scrollable = scrollable;
	}

	public override void OnInitialize() {
		base.OnInitialize();
	}

	public override void OnGUI() {
		if ( items.Length == 0 ) return;
		base.OnGUI();

		var itemsToProcess = items;
		var viewRect = Rectangle;
		var boxRect = new Rect( Rectangle.x - 1, Rectangle.y - 1, Rectangle.width + 2, Rectangle.height + 2 );
		var lineHeight = GUI.skin.label.CalcSize( new GUIContent( items[0] ) ).y;
		var mouseDown = Input.ButtonPressed( EMouseButton.Left );
		var mousePos = Input.MousePosition;
		var doublePos = doubleClickPosition;

		if ( scrollable ) {
			var h = lineHeight * itemsToProcess.Length;
			if ( h > viewRect.height ) {
				viewRect.height = h;
				viewRect.width -= 15f;
			}
		}


		if ( mouseDown ) {
			index = -1;
		}

		GUI.Box( boxRect, "", EditorStyles.helpBox );

		scrollPosition = GUI.BeginScrollView( Rectangle, scrollPosition, viewRect, false, false );

		for ( int i = 0; i < items.Length; i++ ) {
			var r = new Rect( viewRect.x, viewRect.y + ( i * lineHeight ), viewRect.width, lineHeight );

			if ( i % 2 == 0 ) {
				EditorGUI.DrawRect( r, alternateColor );
			}

			if ( mouseDown ) {
				if ( r.Contains( mousePos + scrollPosition ) ) {
					index = i - 1;

					if ( OnSelectedItemChanged != null ) {
						OnSelectedItemChanged.Invoke( this, new ListEventArgs( i, itemsToProcess[i] ) );
					}
				}
			} else {
				GUI.Label( r, items[i] );
			}

			if ( index == i ) {
				EditorGUI.DrawRect( r, highlightColor );
				GUI.Label( r, items[i] );
			} else {
				GUI.Label( r, items[i] );
			}

			if ( doublePos != Vector2.zero ) {
				if ( r.Contains( doublePos + scrollPosition ) ) {
					if ( OnItemDoubleClick != null ) {
						OnItemDoubleClick.Invoke( this, new ListEventArgs( i, itemsToProcess[i] ) );
					}
				}
			}
		}

		GUI.EndScrollView();

		if ( doublePos != Vector2.zero ) {
			doubleClickPosition = Vector2.zero;
		}
	}

	public override void OnDoubleClick( EMouseButton button, Vector2 position ) {
		base.OnDoubleClick( button, position );

		if ( button == EMouseButton.Left ) {
			doubleClickPosition = position;
		}
	}

	public void UpdateItems( string[] items ) {
		this.items = items;
	}
}
