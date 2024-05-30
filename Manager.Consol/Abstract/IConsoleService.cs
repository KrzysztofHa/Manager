using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Consol.Abstract;

public interface IConsoleService<T>
{
    void WriteLineMessage(string message);
    void WriteLineErrorMessage(string errorMessage);
    string GetRequiredStringFromUser(string message);
    void WriteTitle(string title);   
    string GetStringFromUser(string message);
    int? GetIntNumberFromUser(string message);
    bool AnswerYesOrNo(string message);
    void WriteLineMessageActionSuccess(string message);
    ConsoleKeyInfo GetKeyFromUser();
    List<T> ViewListAndGetSelectedElement(List<T> elementListToView);
    
}