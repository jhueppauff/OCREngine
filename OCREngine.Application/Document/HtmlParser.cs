using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using OCREngine.Domain.Entities.Vision;

namespace OCREngine.Application.Document
{
    /// <summary>
    /// HTML Parser
    /// </summary>
    public class HtmlParser
    {
        /// <summary>
        /// Creates the HTML from vision result.
        /// </summary>
        /// <param name="ocrResult">The OCR result.</param>
        /// <param name="file">The file.</param>
        /// <returns>Returns <see cref="HtmlDocument"/></returns>
        public HtmlDocument CreateHtmlFromVisionResult(OcrResults ocrResult, string file)
        {
            HtmlDocument document = new HtmlDocument();

            // Create Base HTML
            HtmlNode baseNode = HtmlNode.CreateNode($"<html><head></head><body style='font-family: Arial, Sans-Serif; background-repeat: no-repeat; background-image: url(\"{file}\");'></body></html>");
            HtmlNode body = baseNode.SelectSingleNode("//body");

            int regionCount = 0, lineCount = 0;

            foreach (Region region in ocrResult.Regions)
            {
                regionCount++;

                BoundingBox boundingBox = this.GetBoundingBox(region.BoundingBox);

                HtmlNode subNode = HtmlNode.CreateNode($"<div id='region{regionCount}' style='left: {boundingBox.Left}; top: {boundingBox.Top}; length: {boundingBox.Length}; height: {boundingBox.Size}; position:fixed;'></div>");

                foreach (Line line in region.Lines)
                {
                    BoundingBox lineBoundingBox = this.GetBoundingBox(line.BoundingBox);
                    lineCount++;
                    HtmlNode subSubNode = HtmlNode.CreateNode($"<div id='line{lineCount}' style='left: {lineBoundingBox.Left}; top: {lineBoundingBox.Top}; length: {lineBoundingBox.Length}; height: {lineBoundingBox.Size}; position:fixed;'></div>");

                    int wordCount = 0;
                    foreach (Word word in line.Words)
                    {
                        BoundingBox wordBoundingBox = this.GetBoundingBox(word.BoundingBox);
                        wordCount++;
                        HtmlNode subSubSubNode = HtmlNode.CreateNode($"<div id='line{lineCount}word{wordCount}' style='color: rgba(0, 0, 0, 0.01); left: {wordBoundingBox.Left}; top: {Math.Floor(line.Words.Average(item => this.GetBoundingBox(item.BoundingBox).Top)) - 2}; length: {wordBoundingBox.Length}; font-size: {this.ConvertPointToPixel(line.Words.Average(item => this.GetBoundingBox(item.BoundingBox).Size), 96)}; position:fixed;'>{HttpUtility.HtmlEncode(word.Text)}</div>");
                        subSubNode.AppendChild(subSubSubNode);
                    }

                    subNode.AppendChild(subSubNode);
                }

                body.AppendChild(subNode);
            }

            baseNode.ReplaceChild(body, baseNode.SelectSingleNode("//body"));
            document.DocumentNode.AppendChild(baseNode);

            return document;
        }

        /// <summary>
        /// Converts the bounding box string input into the X and Y position and length and size.
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

        /// <summary>
        /// Converts the point to pixel.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns>Returns Pixel as <see cref="double"/></returns>
        private double ConvertPointToPixel(double point, double dpi)
        {
            double pointsPerPixel = dpi / 72;
            return pointsPerPixel * point;
        }
    }
}
