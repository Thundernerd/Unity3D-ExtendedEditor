using UnityEngine;
using Newtonsoft.Json;
using System;

public class ColorConverter : JsonConverter {

	public override bool CanConvert( Type objectType ) {
		return objectType.IsAssignableFrom( typeof(Color) );
	}

	public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) {
		reader.Read();

		float r = Read(reader);
		float g = Read(reader);
		float b = Read( reader );
		float a = Read( reader );

		return new Color( r, g, b, a );
	}

	public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer ) {
		var c = (Color)value;

		writer.WriteStartObject();
		Write( writer, "r", c.r );
		Write( writer, "g", c.g );
		Write( writer, "b", c.b );
		Write( writer, "a", c.a );
		writer.WriteEndObject();
		writer.Flush();
	}

	private void Write( JsonWriter writer, string name, float value ) {
		writer.WritePropertyName( name );
		writer.WriteValue( value );
	}

	private float Read( JsonReader reader ) {
		reader.Read();
		var value = (double)reader.Value;
		reader.Read();
		return (float)value;
	}
}
