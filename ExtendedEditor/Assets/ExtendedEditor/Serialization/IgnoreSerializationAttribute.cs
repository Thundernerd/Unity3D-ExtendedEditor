﻿using System;

[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
sealed class IgnoreSerializationAttribute : Attribute { }
