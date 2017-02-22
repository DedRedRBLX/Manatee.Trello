using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json;
using Manatee.Json.Serialization;
using Manatee.Trello.Json;

namespace Manatee.Trello.ManateeJson
{
	internal static class GeneralExtensions
	{
		public static string ToLowerString<T>(this T item)
		{
			return item.ToString().ToLower();
		}
		public static T Deserialize<T>(this JsonObject obj, JsonSerializer serializer, string key)
		{
			if (!obj.ContainsKey(key)) return default(T);
#if IOS
			if (typeof (Enum).IsAssignableFrom(typeof (T)))
				return obj.TryGetString(key).ToEnum<T>();
#endif
			return serializer.Deserialize<T>(obj[key]);
		}
		public static void Serialize<T>(this T obj, JsonObject json, JsonSerializer serializer, string key, bool force = false)
		{
			var isDefault = Equals(obj, default(T));
			if (force || !isDefault)
			{
#if IOS
				var enumValue = obj as Enum;
				if (enumValue != null)
					json[key] = enumValue.ToDescription();
				else
#endif
				json[key] = isDefault ? string.Empty : serializer.Serialize(obj);
			}
		}
		public static void SerializeId<T>(this T obj, JsonObject json, string key)
			where T : IJsonCacheable
		{
			if (!Equals(obj, default(T)))
				json[key] = obj.Id;
		}
	}
}
