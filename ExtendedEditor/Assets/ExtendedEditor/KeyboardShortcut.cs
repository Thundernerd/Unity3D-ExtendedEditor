#if UNITY_EDITOR
using System;
using UnityEngine;

namespace TNRD.Editor {
    public class KeyboardShortcut {
        public int ID;
        public KeyCode[] KeyCodes;
        public Action Callback;
        public bool Control;
        public bool Alt;
        public bool Shift;
    }
}
#endif