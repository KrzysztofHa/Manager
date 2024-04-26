using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Concrete;
using Newtonsoft.Json;

namespace Manager.Infrastructure.Common
{
    public class BaseOperationService<T> : IBaseService<T>
    {
        public List<T> ListOfElements { get; set; }
        public BaseOperationService()
        {
            ListOfElements = new List<T>();
            LoadListInBase();
        }

        private static readonly string PathName = typeof(T).Name;
        public static string PathToFile = LoadPathToFile();

        private static string LoadPathToFile()
        {
            BasePathsService _basePathsService = new();
            PathToFile = _basePathsService.GetPathToFileOfTypeName(PathName);
            return PathToFile;
        }
        public List<T> LoadListInBase()
        {
            if (File.Exists(PathToFile))
            {
                using StreamReader sr = new StreamReader(PathToFile);
                using JsonReader jsonReader = new JsonTextReader(sr);

                JsonSerializer serializer = new JsonSerializer();
                var jsonOutput = serializer.Deserialize<string>(jsonReader);
                ListOfElements = JsonConvert.DeserializeObject<List<T>>(jsonOutput);
            }

            return ListOfElements;
        }
        public bool SaveListToBase()
        {
            try
            {
                var jsonOutput = JsonConvert.SerializeObject(ListOfElements);
                using StreamWriter sw = new StreamWriter(PathToFile);
                using JsonWriter jsonWriter = new JsonTextWriter(sw);

                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(jsonWriter, jsonOutput);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }

}

