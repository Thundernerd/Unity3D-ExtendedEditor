using UnityEngine;
using System.Collections;
using TNRD.Editor.Core;
using System.Collections.Generic;

public class ControlList : ExtendedControl {

    public enum EListMode {
        Horizontal,
        Vertical
    }

    private List<ExtendedControl> controls = new List<ExtendedControl>();

    /// <summary>
    /// The margin (in pixels) that is applied to the top-left of every control
    /// </summary>
    public Vector2 Margin = new Vector2();
    /// <summary>
    /// Should the list start at the top-left corner of its parent window
    /// </summary>
    public bool AutoPosition = true;
    /// <summary>
    /// Does this list go horizontally or vertically
    /// </summary>
    public EListMode ListMode = EListMode.Vertical;

    public ControlList() : base() { }
    public ControlList( ExtendedControl control ) : base() {
        controls.Add( control );
    }
    public ControlList( IEnumerable<ExtendedControl> controls ) : base() {
        this.controls.AddRange( controls );
    }
    public ControlList( Vector2 margin ) : base() {
        Margin = margin;
    }
    public ControlList( Vector2 margin, ExtendedControl control ) : base() {
        Margin = margin;
        controls.Add( control );
    }
    public ControlList( Vector2 margin, IEnumerable<ExtendedControl> controls ) : base() {
        Margin = margin;
        this.controls.AddRange( controls );
    }

    public override void OnInitialize() {
        base.OnInitialize();

        if ( controls.Count > 0 ) {
            foreach ( var item in controls ) {
                Window.AddControl( item );
            }
        }
    }

    public override void OnGUI() {
        base.OnGUI();

        if ( AutoPosition ) {
            var position = Margin;
            foreach ( var item in controls ) {
                item.Position = ExtendedWindow.ToWorldPosition( position );
                item.Position.x += item.Size.x / 2;
                item.Position.y -= item.Size.y / 2;
                switch ( ListMode ) {
                    case EListMode.Horizontal:
                        position.x += ExtendedWindow.ToScreenSize( item.Size ).x;
                        position.x += Margin.x;
                        position.y = Margin.y;
                        break;
                    case EListMode.Vertical:
                        position.y += ExtendedWindow.ToScreenSize( item.Size ).y;
                        position.x = Margin.x;
                        position.y += Margin.y;
                        break;
                }
            }
        } else {
            var position = ExtendedWindow.ToScreenPosition( Position );
            foreach ( var item in controls ) {
                item.Position = ExtendedWindow.ToWorldPosition( position + Margin );
                item.Position.x += item.Size.x / 2;
                item.Position.y -= item.Size.y / 2;
                switch ( ListMode ) {
                    case EListMode.Horizontal:
                        position.x += ExtendedWindow.ToScreenSize( item.Size ).x;
                        position.x += Margin.x;
                        break;
                    case EListMode.Vertical:
                        position.y += ExtendedWindow.ToScreenSize( item.Size ).y;
                        position.y += Margin.y;
                        break;
                }
            }
        }
    }

    public List<ExtendedControl> GetAll() {
        return controls;
    }

    public List<T> GetAll<T>() where T : ExtendedControl {
        var list = new List<T>();
        foreach ( var item in controls ) {
            if ( item is T ) {
                list.Add( item as T );
            }
        }
        return list;
    }

    public ExtendedControl Get( int index ) {
        return controls[index];
    }

    public T Get<T>( int index ) where T : ExtendedControl {
        return controls[index] as T;
    }

    public void Add( ExtendedControl control ) {
        controls.Add( control );

        if ( IsInitialized ) {
            Window.AddControl( control );
        }
    }

    public void Remove( ExtendedControl control ) {
        controls.Remove( control );

        if ( IsInitialized ) {
            Window.RemoveControl( control );
        }
    }

    public void Clear() {
        if ( IsInitialized ) {
            foreach ( var item in controls ) {
                Window.RemoveControl( item );
            }
        }

        controls.Clear();
    }
}
