using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Consol.Abstract;

public interface IConsoleService
{
    void WriteMesage(string message);
    void WriteErrorMesage(string errorMessage);
    void WriteTitle(string title);   
    string GetStringFromUser(string message);
    int GetIntNumberFromUser(string message);
}