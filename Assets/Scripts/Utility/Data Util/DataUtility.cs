using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class DataUtility
{
    public static void Save<T>(string fileName, string key, T data)
    {
        string filePath = Application.persistentDataPath + $"/{fileName}.json";

        string fileText = "{}";

        if (File.Exists(filePath))
        {
            fileText = File.ReadAllText(filePath);
        }

        JObject json = JObject.Parse(fileText);

        if (json.ContainsKey(key))
        {
            json[key] = JsonConvert.SerializeObject(data);
            // json[key] = JsonUtility.ToJson(data);
        }
        else
        {
            json.Add(key, JsonConvert.SerializeObject(data));
            // json.Add(key, JsonUtility.ToJson(data));
        }

        File.WriteAllText(filePath, json.ToString());
    }

    public async static void SaveAsync<T>(string fileName, string key, T data)
    {
        string filePath = Application.persistentDataPath + $"/{fileName}.json";

        string fileText = "{}";

        if (File.Exists(filePath))
        {
            fileText = await File.ReadAllTextAsync(filePath);
        }

        JObject json = JObject.Parse(fileText);

        if (json.ContainsKey(key))
        {
            json[key] = JsonConvert.SerializeObject(data);
        }
        else
        {
            json.Add(key, JsonConvert.SerializeObject(data));
        }

        await File.WriteAllTextAsync(filePath, json.ToString());
    }

    public static T Load<T>(string fileName, string key, T defaultValue)
    {
        string filePath = Application.persistentDataPath + $"/{fileName}.json";

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);

            if (data != "")
            {
                JObject json = JObject.Parse(data);

                if (json.ContainsKey(key))
                {
                    return JsonConvert.DeserializeObject<T>(json[key].ToString());
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
        else
        {
            return defaultValue;
        }
    }
}
