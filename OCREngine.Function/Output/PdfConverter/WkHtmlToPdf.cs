using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.WebJobs;

namespace OCREngine.Function.Output.PdfConverter
{
    public static class WkHtmlToPdf
    {
        public static void Preload(ExecutionContext context)
        {
            var wkHtmlToPdfContext = new CustomAssemblyLoadContext();
            string architectureFolder = (IntPtr.Size == 8) ? "x64" : "x32";
            string wkHtmlToPdfPath = Path.GetFullPath(Path.Combine(context.FunctionDirectory, $"..\\bin\\{architectureFolder}\\libwkhtmltox"));

            wkHtmlToPdfContext.LoadUnmanagedLibrary(wkHtmlToPdfPath);
        }
    }
}
