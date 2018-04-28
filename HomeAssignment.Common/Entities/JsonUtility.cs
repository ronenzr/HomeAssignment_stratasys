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
            try
            {
                //return null;
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception)
            {
                //TODO: write to log
            }
            

            return null;
        }

        public static void ToFile<T>(string filePath, T toSave)
        {
            try
            {
                string json = JsonConvert.SerializeObject(toSave);

                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                //TODO: write to log
            }
        }
    }
}
