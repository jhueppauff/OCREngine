using System;

namespace OCREngine.Function
{
    public static class EnviromentHelper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}