using System.Collections.Generic;

namespace TNRD.Editor.Json {

    public class JsonObject {
        public Dictionary<string, object> KeyValues = new Dictionary<string, object>();
    }

    public class JsonArray : JsonObject {

    }
}