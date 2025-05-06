// Ignore Spelling: App Json

namespace Low_Code_App_Editor.Json
{
	using System.Reflection;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public class DynamicResolver : DefaultContractResolver
    {
		private readonly JsonConverter _converter;

		public DynamicResolver(JsonConverter converter)
		{
			_converter = converter;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);

			if (prop.PropertyType == typeof(object) && member.Name == "Value")
			{
				prop.Converter = _converter;
			}

			return prop;
		}
	}
}
