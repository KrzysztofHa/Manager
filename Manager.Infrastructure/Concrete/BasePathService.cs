﻿using Manager.Infrastructure.Common;
using Manager.Infrastructure.Entity;

namespace Manager.Infrastructure.Concrete;

public class BasePathsService : BaseOperationService<BasePaths>
{
    public string PathToBaseCurrent = Directory.GetCurrentDirectory() + @"\base\";
    public string FileExtensions = ".manager.json";

    public BasePathsService()
    {
        if (File.Exists(PathToBaseCurrent + nameof(BasePaths) + FileExtensions))
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
            IsActive = true,
            UserName = Environment.UserName
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