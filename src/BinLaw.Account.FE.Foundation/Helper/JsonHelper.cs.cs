using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BinLaw.Account.FE.Foundation.Helper
{
    public static class JsonHelper
    {
        #region Json.Net 
        /// <summary>
        /// 使用JSON.NET反序列化
        /// </summary>
        /// <typeparam name="T">反序列化出来的类型</typeparam>
        /// <param name="data">Json数据</param>
        /// <returns>根据Json字符串反序列化出来的类型</returns>
        public static T Deserialize<T>(string data) where T : class
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static Task<T> DeserializeAsync<T>(string data) where T : class
        {
            return JsonConvert.DeserializeObjectAsync<T>(data);
        }

        /// <summary>
        /// 使用JSON.NET序列化
        /// </summary>
        /// <param name="obj">需要序列化的数据</param>
        /// <returns>Json字符串</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        } 
        #endregion

        #region .Net Serializer
        public static string Serialize2<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static T Deserialize2<T>(string json) where T : class
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);

                return obj;
            }
        } 
        #endregion

        /// <summary>
        /// 根据 JSON 字符串来解析出报文头和体，返回整体报文
        /// </summary>
        public static dynamic DeserializeMessage(Type headerType, Type bodyType, string json)
        {
            var root = JObject.Parse(json);
            var serializer = new JsonSerializer();

            dynamic msg = serializer.Deserialize(root.CreateReader(), headerType);
            msg.Body = serializer.Deserialize(root["cd"].CreateReader(), bodyType);

            return msg;
        }

        /// <summary>
        /// 根据 JSON 字符串来解析出报文体并返回
        /// </summary>
        public static dynamic DeserializeBody(Type type, string json)
        {
            var root = JObject.Parse(json);
            var serializer = new JsonSerializer();

            dynamic msg = default(Type);
            if (root["cd"] != null)
                msg = serializer.Deserialize(root["cd"].CreateReader(), type);

            return msg;
        }

        /// <summary>
        /// 根据 JSON 字符串来解析出报文体并返回
        /// </summary>
        public static dynamic DeserializeForFund(Type headerType, Type bodyType, string json)
        {
            json = json.Replace("\\", "").Replace("\"cd\":\"{", "\"cd\":{").Replace("}\"}", "}}");

            var root = JObject.Parse(json);
            var serializer = new JsonSerializer();

            dynamic msg = serializer.Deserialize(root.CreateReader(), headerType);
            msg.Body = serializer.Deserialize(root["cd"].CreateReader(), bodyType);

            return msg;
        }
    }
}
