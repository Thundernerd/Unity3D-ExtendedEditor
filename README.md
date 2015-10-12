# ExtendedEditor
Extended Editor is a framework for building editors in Unity3D.
Below you can find a list of some of the features this framework houses.
If you're curious on how some things work you can always try out some of the examples in the Examples folder.

##Entity-like framework

The framework consists of three core classes. 

The *ExtendedEditor* class, which is the starting point for your editor and contains your windows.

The *ExtendedWindow* class, which is a container that shows controls (or custom GUI code) inside the editor. Each window will have it's own Update and GUI calls.

The *ExtendedControl* class. You can easily separate code by using controls. These controls are added to windows and their update and draw calls will be called automatically.

##Easy coordinate system

No need to think in pixels, you can just use the coordinate system that you are used to in Unity. 0,0 is the center of your window. You can also easily convert from and to pixels with these functions

    ExtendedWindow.ToScreenPosition( new Vector2( 0, 0 ) );
    ExtendedWindow.ToWorldPosition( new Vector2( 350, 125 ) );
    ExtendedWindow.GetScreenInWorldSize();

##Input Handling

The *ExtendedInput* class handles the input for your editor. No need to check Event.current anymore. You can easily check if a key or mouse button is down, up, pressed, or released.

	// Keyboard
	if ( Input.KeyReleased( KeyCode.Return ) ) { }
	// Multiple keys (optional)
	if ( Input.KeyDown( KeyCode.Return, KeyCode.KeypadEnter ) ) { }
	// Multiple keys (all required)
	if ( Input.KeysPressed( KeyCode.A, KeyCode.B ) ) { }

	// Mouse
	if ( Input.ButtonUp( EMouseButton.Middle ) ) { }
    if ( Input.ButtonReleased( EMouseButton.Left ) ) { }

##Per Window Settings

Settings that allow you to customize some behaviours for windows. 

    Settings.AllowRepositioning = true;
    Settings.AllowResize = true;
    // Draws a couple of "levels" deep grid
    Settings.DrawGrid = false;
    // Draw a maximize and close button on the top-right corner of the window
    Settings.DrawTitleBarButtons = true;
    Settings.DrawToolbar = true;
    // Blocks the update and draw calls to other windows
    Settings.IsBlocking = true;
    Settings.IsFullscreen = false;
    // Allows panning in the editor
    Settings.UseCamera = true;
    Settings.UseOnSceneGUI = true;

##Asset Loading

Put your images into an asset folder and let them be loaded and unloaded by the *ExtendedAssets* class.

	Texture2D hWorld = Assets["HelloWorld"];
	Texture2D nWorld = Assets.Load( "HelloWorld" );

##Modal Windows

The *ExtendedModalWindow* class is a base class to create easy popups that can be put on top of all the other windows. The following are included already.

(Dialog Box)
![Dialog Box](http://puu.sh/kpXcM/d96ae337a8.png)

(Input Box)
![Input Box](http://puu.sh/kpXdv/0c8ccc0e65.png)

##Notifications

Show notifcations at the bottom-right corner with to notify the user of your editor with ease.

    ShowNotification( "HelloWorld" );
    // Error notifications are in red and longer visible
    ShowErrorNotification( "ErrorWorld" );

##Shortcut Key Hooks

Register key command and hook them to callbacks.

    // Keycode, callback, control (command), alt, shift
    AddShortcut( KeyCode.A, shortcutCallback, true, false, true );

##Sharing Data (objects)

The *ExtendedSharedObject* class is a base for objects that are stored inside the editor and can be called fetched from all the windows inside the editor.

&nbsp;

Other screenshots

(Multiple windows)
![Multiple Windows](http://puu.sh/kpY2i/04f3883cb6.png)

(Notifications)
![Notifications](http://puu.sh/kpY5F/7f7ad0b780.png)

(Toolbar)
![Toolbar](http://puu.sh/kpYch/821208a54d.png)
