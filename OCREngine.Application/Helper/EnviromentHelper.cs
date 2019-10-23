using System;

namespace OCREngine.Application.Helper
{
    public static class EnviromentHelper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}