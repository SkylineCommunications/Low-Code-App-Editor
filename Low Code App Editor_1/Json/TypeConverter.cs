using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Low_Code_App_Editor_1.Json
{
    public class TypeConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <returns><see langword="true"/> if this instance can convert the specified object type; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The JsonReader to read from.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The JsonSerializer used to deserialize the object.</param>
        /// <returns>The deserialized object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken json;
            var foundType = objectType;
            try
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    json = JArray.Load(reader);
                    if (objectType == typeof(object))
                    {
                        return null;
                    }

                    object[] resultArray = (object[])Array.CreateInstance(foundType.GetElementType(), ((JArray)json).Count);
                    for (int i = 0; i < ((JArray)json).Count; i++)
                    {
                        var itemJson = ((JArray)json)[i];
                        var typeName = Convert.ToString(itemJson["__type"]);
                        foundType = objectType.Assembly.GetType(typeName);
                        if (foundType == null) foundType = objectType;
                        resultArray[i] = Activator.CreateInstance(foundType);
                        serializer.Populate(itemJson.CreateReader(), resultArray[i]);
                    }

                    return resultArray;
                }
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    json = JObject.Load(reader);
                    var typeName = Convert.ToString(json["__type"]);

                    if (typeName == "Skyline.DataMiner.Web.Common.v1.DMAPrimitiveValue")
                    {
                        var result = JsonConvert.DeserializeObject<Skyline.DataMiner.Web.Common.v1.DMAPrimitiveValue>(json.ToString());
                        return result;
                    }
                    else
                    {
                        foundType = objectType.Assembly.GetType(typeName);
                        if (foundType == null) foundType = objectType;
                        object result = Activator.CreateInstance(foundType);
                        serializer.Populate(json.CreateReader(), result);
                        return result;
                    }
                }
                else
                {
                    json = JToken.Load(reader);
                    var jsonValue = json.ToString();
                    if (foundType == typeof(bool))
                        jsonValue = jsonValue.ToLower();
                    if (foundType == typeof(string))
                        return jsonValue;
                    if (foundType == typeof(double))
                        return Convert.ToDouble(jsonValue);
                    var value = JsonConvert.DeserializeObject(jsonValue, foundType);
                    return value;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The JsonSerializer used to serialize the object.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;

        public override bool CanWrite => false;
    }
}
