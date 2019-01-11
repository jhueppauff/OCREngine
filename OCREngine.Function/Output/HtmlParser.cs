using HtmlAgilityPack;
using OCREngine.Domain.Entities.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCREngine.Function.Output
{
    public class HtmlParser
    {
        public HtmlDocument CreateHtmlFromVisionResult(OcrResults ocrResult)
        {
            HtmlDocument document = new HtmlDocument();

            // Create Base HTML
            HtmlNode baseNode = HtmlNode.CreateNode("<html><head></head><body></body></html>");
            HtmlNode body = baseNode.SelectSingleNode("//body");

            int regionCount = 0, lineCount = 0;

            foreach (Region region in ocrResult.Regions)
            {
                regionCount++;

                BoundingBox boundingBox = GetBoundingBox(region.BoundingBox);

                HtmlNode subNode = HtmlNode.CreateNode($"<div id='region{regionCount}' style='left: {boundingBox.Left}; top: {boundingBox.Top}; length: {boundingBox.Length}; height: {boundingBox.Size}; position:absolute'></div>");

                foreach (Line line in region.Lines)
                {
                    BoundingBox lineBoundingBox = GetBoundingBox(line.BoundingBox);
                    lineCount++;
                    HtmlNode subSubNode = HtmlNode.CreateNode($"<div id='line{lineCount}' style='left: {lineBoundingBox.Left}; top: {lineBoundingBox.Top}; length: {lineBoundingBox.Length}; height: {lineBoundingBox.Size};'></div>");

                    StringBuilder lineText = new StringBuilder();

                    foreach (Word word in line.Words)
                    {
                        lineText.Append(word.Text);
                        lineText.Append(" ");
                    }

                    HtmlNode subSubSubNode = HtmlNode.CreateNode($"<div id='wordline{lineCount}' >{lineText}</div>");
                    subSubNode.AppendChild(subSubSubNode);
                    subNode.AppendChild(subSubNode);
                }

                body.AppendChild(subNode);
            }

            baseNode.ReplaceChild(body, baseNode.SelectSingleNode("//body"));
            document.DocumentNode.AppendChild(baseNode);

            return document;
        }

        /// <summary>
        /// Converts the bounding box string input into the X and Y positon and length and size.
        /// </summary>
        /// <param name="boundingBox">The bounding box string from vision API.</param>
        /// <returns>Returns <see cref="BoundingBox"/></returns>
        private BoundingBox GetBoundingBox(string boundingBox)
        {
            BoundingBox boundingBoxOutput = new BoundingBox();

            List<string> splitString = boundingBox.Split(',').ToList<string>();

            boundingBoxOutput.Left = Convert.ToInt16(splitString[0]);
            boundingBoxOutput.Top = Convert.ToInt16(splitString[1]);
            boundingBoxOutput.Length = Convert.ToInt16(splitString[2]);
            boundingBoxOutput.Size = Convert.ToInt16(splitString[3]);

            return boundingBoxOutput;
        }

        private double ConvertPointsToPixel(double point, double dpi)
        {
            double pointsPerPixel = dpi / 72;
            return pointsPerPixel * point;
        }
    }
}
