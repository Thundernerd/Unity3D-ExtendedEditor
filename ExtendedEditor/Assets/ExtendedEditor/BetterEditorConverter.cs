#if UNITY_EDITOR
using UnityEngine;
using Newtonsoft.Json.Converters;
using System;

namespace TNRD {
	public class BetterEditorConverter : CustomCreationConverter<BetterEditor> {

		public override BetterEditor Create( Type objectType ) {
			return (BetterEditor)ScriptableObject.CreateInstance( objectType );
		}
	}
}
#endif