# OCREngine

Using Azure [Serverless](https://azure.microsoft.com/en-us/services/functions/) Functions this project consumes the Azure Vision API (Project Oxford) to convert and analyze scaned documents.
The converted documents will be containing the extracted text so it can be copied out or indexed by search engines.

This Project is built entirely on Azure. It consumes following resources:

- [Azure Functions](https://azure.microsoft.com/en-us/services/functions/)
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/)
- [Azure Queue Storage](https://azure.microsoft.com/en-us/services/storage/queues/)
- [Azure Table Storage](https://azure.microsoft.com/en-us/services/storage/tables/)
- [Azure Cognitive Services](https://azure.microsoft.com/en-us/services/cognitive-services/)
- [Application Insights](https://azure.microsoft.com/en-us/services/monitor/)

## Build Status
[![Build status](https://dev.azure.com/jhueppauff/OCREngine/_apis/build/status/OCREngine-ASP.NET%20Core-CI)](https://dev.azure.com/jhueppauff/OCREngine/_build/latest?definitionId=2)
