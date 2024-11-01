using System;
using System.IO;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class Json
    {
        public static string Serialize(object value, bool isCompressed = false)
        {
            try
            {
                new fsSerializer().TrySerialize(value, out var data).AssertSuccessWithoutWarnings();
                if (isCompressed)
                {
                    return fsJsonPrinter.CompressedJson(data);
                }

                return fsJsonPrinter.PrettyJson(data);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static object Deserialize(Type type, string serializedState)
        {
            try
            {
                fsData data = fsJsonParser.Parse(serializedState);
                object deserialized = null;
                var ser = new fsSerializer();
                ser.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

                return deserialized;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                File.WriteAllText($"{Application.dataPath}/../error_json.json", serializedState);
#endif
                Debug.LogError(e);
                return null;
            }
        }
    }
}