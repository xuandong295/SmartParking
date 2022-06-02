using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Helpers
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        public static string Serialize<T>(T obj)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SerializeCamelCase<T>(T obj)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, serializerSettings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<JObject> ConvertListObjectToListJObject<T>(List<T> listObjss)
        {
            try
            {
                var jsonStr = Serialize(listObjss);
                return Deserialize<List<JObject>>(jsonStr);
            }
            catch
            {
                return null;
            }
        }

        public static T GetValueFromKeyInJsonString<T>(string jsonString, string key) where T : class
        {
            List<String> keys = key.Split('.').ToList();
            JObject json = JObject.Parse(jsonString);

            int count = 0;
            string temp = null;
            T value = null;
            try
            {
                // loop to the last json key
                while (count < keys.Count)
                {
                    try
                    {
                        using (var sr = new StringReader(jsonString))
                        using (var jr = new JsonTextReader(sr) { DateParseHandling = DateParseHandling.None })
                        {
                            var j = JToken.ReadFrom(jr);
                            temp = j[keys[count]].ToString();
                        }
                        jsonString = temp;
                    }
                    catch (NullReferenceException ex)
                    {
                        return null;
                    }
                    count++;
                }
                // only a string
                if (typeof(T).Name.Equals("String"))
                {
                    return (T)Convert.ChangeType(temp, typeof(T));
                }

                // if still a json
                else
                {
                    value = JsonHelper.Deserialize<T>(temp);
                }

            }
            catch (NullReferenceException ex)
            {
                //log
                return null;
            }
            return value;
        }

        public static T GetValueFromKeyInJson<T>(string jsonString, string key) where T : class
        {
            List<String> keys = key.Split('.').ToList();
            JObject json = JObject.Parse(jsonString);

            int count = 0;
            string temp = null;
            T value = null;
            try
            {
                // loop to the last json key
                while (count < keys.Count)
                {
                    using (var sr = new StringReader(jsonString))
                    using (var jr = new JsonTextReader(sr) { DateParseHandling = DateParseHandling.None })
                    {
                        var j = JToken.ReadFrom(jr);
                        temp = j[keys[count]].ToString();
                    }
                    jsonString = temp;
                    count++;
                }
                // only a string
                if (typeof(T).Name.Equals("String"))
                {
                    return (T)Convert.ChangeType(temp, typeof(T));
                }

                // if still a json
                else
                {
                    value = JsonHelper.Deserialize<T>(temp);
                }

            }
            catch (NullReferenceException ex)
            {
                //log
                return null;
            }
            return value;
        }

        private static bool DoContainJsonCharacters(string str)
        {
            var specialCharacters = new String[] { "{", "}", "[", "]" };
            return specialCharacters.Any(s => str.Contains(s));
        }
    }
}

