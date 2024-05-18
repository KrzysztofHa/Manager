using Manager.Infrastructure.Common;
using Manager.Infrastructure.Entity;
using Newtonsoft.Json;

namespace Manager.Infrastructure.Concrete
{
    public class BasePathsService : BaseOperationService<BasePaths>
    {
        public string PathToBaseCurrent = Directory.GetCurrentDirectory() + @"\base\";
        public string FileExtensions = ".json.manager";
        public BasePathsService()
        {
            if (File.Exists(PathToBaseCurrent + nameof(BasePaths) + ".json.manager"))
            {
                ListOfElements = LoadListInBase();
            }
            else
            {
                AddNewEntryToPathsList(nameof(BasePaths));
            }
        }
        public string AddNewEntryToPathsList(string pathName)
        {
            ListOfElements.Add(new BasePaths()
            {
                Id = ListOfElements.Count,
                PathName = pathName,
                PathToFile = PathToBaseCurrent + pathName + FileExtensions,
                IsActive = true
            });
            SaveListToBase();
            var getEntryFromPathsList = ListOfElements.First(p => p.PathName == pathName).PathToFile;
            return getEntryFromPathsList;
        }

        public string GetPathToFileOfTypeName(string pathName)
        {
            var getEntryFromPathsList = ListOfElements.FirstOrDefault(p => p.PathName == pathName);
            if (getEntryFromPathsList == null)
            {
                var pathToFile = AddNewEntryToPathsList(pathName);
                return pathToFile;
            }
            return getEntryFromPathsList.PathToFile;
        }
    }

}
