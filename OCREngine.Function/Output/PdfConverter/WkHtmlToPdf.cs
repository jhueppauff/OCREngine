using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCREngine.Function.Output.PdfConverter
{
    public static class WkHtmlToPdf
    {
        public static void Preload()
        {
            var wkHtmlToPdfContext = new CustomAssemblyLoadContext();
            var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            var wkHtmlToPdfPath = Path.Combine(AppContext.BaseDirectory, $"wkhtmltox\\{architectureFolder}\\libwkhtmltox");

            wkHtmlToPdfContext.LoadUnmanagedLibrary(wkHtmlToPdfPath);
        }
    }
}
