﻿namespace OCREngine.Domain.Entities.Vision
{
    public enum HandwritingRecognitionOperationStatus
    {
        /// <summary>
        /// not started
        /// </summary>
        NotStarted,

        /// <summary>
        /// running
        /// </summary>
        Running,

        /// <summary>
        /// succeeded
        /// </summary>
        Succeeded,

        /// <summary>
        /// failed
        /// </summary>
        Failed,
    }
}