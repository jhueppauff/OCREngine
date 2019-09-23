using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.WebJobs;

namespace OCREngine.Application
{
    public static class AssemblyLoader
    {
        public static void Preload(ExecutionContext context, string name)
        {
            var wkHtmlToPdfContext = new CustomAssemblyLoadContext();
            string architectureFolder = (IntPtr.Size == 8) ? "x64" : "x32";
            string wkHtmlToPdfPath = Path.GetFullPath(Path.Combine(context.FunctionDirectory, $"..\\bin\\{architectureFolder}\\{name}"));

            wkHtmlToPdfContext.LoadUnmanagedLibrary(wkHtmlToPdfPath);
        }
    }
}
