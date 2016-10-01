#if UNITY_EDITOR
﻿using System;

namespace TNRD.Editor.Serialization {

    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
    sealed class IgnoreSerializationAttribute : Attribute { }
}
#endif