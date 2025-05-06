// Ignore Spelling: App Json

namespace Low_Code_App_Editor.Json
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Web.Common.v1;

	public class TypeConverter : JsonConverter
	{
		private Assembly WebApiLibAssembly;

		public TypeConverter()
		{
			WebApiLibAssembly = typeof(Skyline.DataMiner.Web.Common.v1.DMAPrimitiveValue).Assembly;
		}

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
			var json = default(JToken);
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
						if (itemJson is JValue itemValue)
						{
							resultArray[i] = itemValue.ToObject(foundType.GetElementType());
							continue;
						}

						var typeName = itemJson["__type"]?.Value<string>() ?? String.Empty;
						if (typeName.StartsWith("Skyline.DataMiner.Web") && !typeName.EndsWith(nameof(DMADynamicApplication)))
						{
							foundType = WebApiLibAssembly.GetType(typeName);
						}
						else if (!String.IsNullOrEmpty(typeName))
						{
							foundType = objectType.Assembly.GetType(typeName);
						}
						else
						{
							foundType = objectType;
						}

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
						if (typeName.StartsWith("Skyline.DataMiner.Web") && !typeName.EndsWith(nameof(DMADynamicApplication)))
						{
							foundType = WebApiLibAssembly.GetType(typeName);
						}
						else
						{
							foundType = objectType.Assembly.GetType(typeName);
						}

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
			throw new NotSupportedException();
		}

		public override bool CanRead => true;

		public override bool CanWrite => false;
	}
}
