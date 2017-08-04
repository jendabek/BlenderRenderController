using System;
using System.Collections.Generic;

namespace BlenderRenderController.newLogger
{
    public interface ILogger
    {
        void Error(string message);
        void Info(string message);
        void Warn(string message);
       // void Block(string title, List<string> block);
    }
}