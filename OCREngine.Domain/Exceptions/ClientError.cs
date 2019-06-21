﻿namespace OCREngine.Domain.Entities.Exceptions
{
    using System;

    public class ClientError
    {
        /// <summary>
        /// Gets or sets error code in error entity.
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public Guid RequestId { get; set; }
    }
}
