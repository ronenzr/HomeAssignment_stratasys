using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HomeAssignment.Common.Entities
{
    public class JsonUtility
    {
        public static T FromFile<T>(string filePath) where T : class
        {
            //return null;
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }

            return null;
        }

        public static void ToFile<T>(string filePath, T toSave)
        {
            string json = JsonConvert.SerializeObject(toSave);

            File.WriteAllText(filePath, json);
        }
    }
}
