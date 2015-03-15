#if UNITY_EDITOR
using System;

public class BetterModalWindowEventArgs : EventArgs {

	public BetterModalWindow Window { get; private set; }
	public EBetterModalWindowResult Result { get; private set; }

	public BetterModalWindowEventArgs() : base() { }

	public BetterModalWindowEventArgs( BetterModalWindow window, EBetterModalWindowResult result ) : this() {
		Window = window;
		Result = result;
	}
}
#endif