using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OCREngine.Domain.Entities.Vision;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OCREngine.Function.Vision
{
    public class VisionServiceClient : IVisionServiceClient
    {
        /// <summary>
        /// The service host
        /// </summary>
        private const string DEFAULT_API_ROOT = "https://westus.api.cognitive.microsoft.com/";

        /// <summary>
        /// Host root, overridable by subclasses, intended for testing.
        /// </summary>
        protected virtual string ServiceHost => _apiRoot;

        /// <summary>
        /// Default timeout for calls
        /// </summary>
        private const int DEFAULT_TIMEOUT = 2 * 60 * 1000; // 2 minutes timeout

        /// <summary>
        /// Default timeout for calls, overridable by subclasses
        /// </summary>
        protected virtual int DefaultTimeout => DEFAULT_TIMEOUT;

        /// <summary>
        /// The default resolver
        /// </summary>
        private readonly CamelCasePropertyNamesContractResolver _defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// The subscription key name
        /// </summary>
        private const string _subscriptionKeyName = "subscription-key";

        /// <summary>
        /// The subscription key
        /// </summary>
        private readonly string _subscriptionKey;

        /// <summary>
        /// The root URI for Vision API
        /// </summary>
        private readonly string _apiRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public VisionServiceClient(string subscriptionKey) : this(subscriptionKey, DEFAULT_API_ROOT) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Root URI for the service endpoint.</param>
        public VisionServiceClient(string subscriptionKey, string apiRoot)
        {
            _apiRoot = apiRoot?.TrimEnd('/');
            _subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// Recognizes the text asynchronous.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <returns>The OCR object.</returns>
        public async Task<OcrResults> RecognizeTextAsync(string imageUrl, string languageCode = LanguageCodes.AutoDetect, bool detectOrientation = true)
        {
            string requestUrl = string.Format("{0}/ocr?language={1}&detectOrientation={2}&{3}={4}", ServiceHost, languageCode, detectOrientation, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            dynamic requestObject = new ExpandoObject();
            requestObject.url = imageUrl;

            return await this.SendAsync<ExpandoObject, OcrResults>("POST", requestObject, request).ConfigureAwait(false);
        }

        /// <summary>
        /// Recognizes the text asynchronous.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <returns>The OCR object.</returns>
        public async Task<OcrResults> RecognizeTextAsync(Stream imageStream, string languageCode = LanguageCodes.AutoDetect, bool detectOrientation = true)
        {
            string requestUrl = string.Format("{0}/ocr?language={1}&detectOrientation={2}&{3}={4}", ServiceHost, languageCode, detectOrientation, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            return await SendAsync<Stream, OcrResults>("POST", imageStream, request).ConfigureAwait(false);
        }

        /// <summary>
        /// Recognizes the handwriting text asynchronous.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns>HandwritingRecognitionOperation created</returns>
        public async Task<HandwritingRecognitionOperation> CreateHandwritingRecognitionOperationAsync(string imageUrl)
        {
            string requestUrl = string.Format("{0}/recognizeText?handwriting=true&{1}={2}", ServiceHost, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            dynamic requestObject = new ExpandoObject();
            requestObject.url = imageUrl;

            return await this.SendAsync<ExpandoObject, HandwritingRecognitionOperation>("POST", requestObject, request).ConfigureAwait(false);
        }

        /// <summary>
        /// Recognizes the handwriting text asynchronous.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <returns>HandwritingRecognitionOperation created</returns>
        public async Task<HandwritingRecognitionOperation> CreateHandwritingRecognitionOperationAsync(Stream imageStream)
        {
            string requestUrl = string.Format("{0}/recognizeText?handwriting=true&{1}={2}", ServiceHost, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            return await SendAsync<Stream, HandwritingRecognitionOperation>("POST", imageStream, request).ConfigureAwait(false);
        }

        /// <summary>
        /// Get HandwritingRecognitionOperationResult
        /// </summary>
        /// <param name="opeartion">HandwritingRecognitionOperation object</param>
        /// <returns>HandwritingRecognitionOperationResult</returns>
        public async Task<HandwritingRecognitionOperationResult> GetHandwritingRecognitionOperationResultAsync(HandwritingRecognitionOperation opeartion)
        {
            string requestUrl = string.Format("{0}?{1}={2}", opeartion.Url, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            return await GetAsync<HandwritingRecognitionOperationResult>("Get", request).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the tags associated with an image.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <returns>Analysis result with tags.</returns>
        public async Task<AnalysisResult> GetTagsAsync(Stream imageStream)
        {
            string requestUrl = string.Format("{0}/tag?{1}={2}", ServiceHost, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            return await SendAsync<Stream, AnalysisResult>("POST", imageStream, request).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the tags associated with an image.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns>Analysis result with tags.</returns>
        public async Task<AnalysisResult> GetTagsAsync(string imageUrl)
        {
            string requestUrl = string.Format("{0}/tag?{1}={2}", ServiceHost, _subscriptionKeyName, _subscriptionKey);
            var request = WebRequest.Create(requestUrl);

            dynamic requestObject = new ExpandoObject();
            requestObject.url = imageUrl;

            return await this.SendAsync<ExpandoObject, AnalysisResult>("POST", requestObject, request).ConfigureAwait(false);
        }

        #region the json client

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="request">The request.</param>
        /// <param name="setHeadersCallback">The set headers callback.</param>
        /// <returns>
        /// The response object.
        /// </returns>
        private async Task<TResponse> GetAsync<TResponse>(string method, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            try
            {
                request.Method = method;
                if (setHeadersCallback == null)
                {
                    SetCommonHeaders(request);
                }
                else
                {
                    setHeadersCallback(request);
                }

                var getResponseAsync = Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);

                await Task.WhenAny(getResponseAsync, Task.Delay(DefaultTimeout)).ConfigureAwait(false);

                //Abort request if timeout has expired
                if (!getResponseAsync.IsCompleted)
                {
                    request.Abort();
                }

                return ProcessAsyncResponse<TResponse>(getResponseAsync.Result as HttpWebResponse);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    HandleException(e);
                    return true;
                });
                return default(TResponse);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default(TResponse);
            }
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="request">The request.</param>
        /// <param name="setHeadersCallback">The set headers callback.</param>
        /// <returns>The response object.</returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        private async Task<TResponse> SendAsync<TRequest, TResponse>(string method, TRequest requestBody, WebRequest request, Action<WebRequest> setHeadersCallback = null)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                request.Method = method;
                if (null == setHeadersCallback)
                {
                    SetCommonHeaders(request);
                }
                else
                {
                    setHeadersCallback(request);
                }

                if (requestBody is Stream)
                {
                    request.ContentType = "application/octet-stream";
                }

                var asyncState = new WebRequestAsyncState()
                {
                    RequestBytes = SerializeRequestBody(requestBody),
                    WebRequest = (HttpWebRequest)request,
                };

                var continueRequestAsyncState = await Task.Factory.FromAsync<Stream>(
                                                    asyncState.WebRequest.BeginGetRequestStream,
                                                    asyncState.WebRequest.EndGetRequestStream,
                                                    asyncState,
                                                    TaskCreationOptions.None).ContinueWith<WebRequestAsyncState>(
                                                       task =>
                                                       {
                                                           var requestAsyncState = (WebRequestAsyncState)task.AsyncState;
                                                           if (requestBody != null)
                                                           {
                                                               using (var requestStream = task.Result)
                                                               {
                                                                   if (requestBody is Stream)
                                                                   {
                                                                       (requestBody as Stream).CopyTo(requestStream);
                                                                   }
                                                                   else
                                                                   {
                                                                       requestStream.Write(requestAsyncState.RequestBytes, 0, requestAsyncState.RequestBytes.Length);
                                                                   }
                                                               }
                                                           }

                                                           return requestAsyncState;
                                                       }).ConfigureAwait(false);

                var continueWebRequest = continueRequestAsyncState.WebRequest;
                var getResponseAsync = Task.Factory.FromAsync<WebResponse>(
                    continueWebRequest.BeginGetResponse,
                    continueWebRequest.EndGetResponse,
                    continueRequestAsyncState);

                await Task.WhenAny(getResponseAsync, Task.Delay(DefaultTimeout)).ConfigureAwait(false);

                //Abort request if timeout has expired
                if (!getResponseAsync.IsCompleted)
                {
                    request.Abort();
                }

                return ProcessAsyncResponse<TResponse>(getResponseAsync.Result as HttpWebResponse);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    HandleException(e);
                    return true;
                });
                return default(TResponse);
            }
            catch (Exception e)
            {
                HandleException(e);
                return default(TResponse);
            }
        }

        /// <summary>
        /// Processes the asynchronous response.
        /// </summary>
        /// <typeparam name="T">Type of response.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <returns>The response.</returns>
        private T ProcessAsyncResponse<T>(HttpWebResponse webResponse)
        {
            using (webResponse)
            {
                if (webResponse.StatusCode == HttpStatusCode.OK ||
                    webResponse.StatusCode == HttpStatusCode.Accepted ||
                    webResponse.StatusCode == HttpStatusCode.Created)
                {
                    if (webResponse.ContentLength != 0)
                    {
                        using (var stream = webResponse.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                if (webResponse.ContentType == "image/jpeg" ||
                                    webResponse.ContentType == "image/png")
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        stream.CopyTo(ms);
                                        return (T)(object)ms.ToArray();
                                    }
                                }
                                else
                                {
                                    string message = string.Empty;
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        message = reader.ReadToEnd();
                                    }

                                    JsonSerializerSettings settings = new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        NullValueHandling = NullValueHandling.Ignore,
                                        ContractResolver = _defaultResolver
                                    };

                                    return JsonConvert.DeserializeObject<T>(message, settings);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (webResponse.Headers.AllKeys.Contains("Operation-Location"))
                        {
                            string message = string.Format("{{Url: \"{0}\"}}", webResponse.Headers["Operation-Location"]);

                            JsonSerializerSettings settings = new JsonSerializerSettings
                            {
                                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                NullValueHandling = NullValueHandling.Ignore,
                                ContractResolver = _defaultResolver
                            };

                            return JsonConvert.DeserializeObject<T>(message, settings);
                        }
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Set request content type.
        /// </summary>
        /// <param name="request">Web request object.</param>
        private void SetCommonHeaders(WebRequest request)
        {
            request.ContentType = "application/json";
            request.Headers[HttpRequestHeader.Authorization] = "Basic ZTkwNTE2ZmQ4NThlNDVjMmFhNDMzMjRlZjBlOThlN2E=";
        }

        /// <summary>
        /// Serialize the request body to byte array.
        /// </summary>
        /// <typeparam name="T">Type of request object.</typeparam>
        /// <param name="requestBody">Strong typed request object.</param>
        /// <returns>Byte array.</returns>
        private byte[] SerializeRequestBody<T>(T requestBody)
        {
            if (requestBody == null || requestBody is Stream)
            {
                return null;
            }
            else
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ContractResolver = _defaultResolver
                };

                return System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody, settings));
            }
        }

        /// <summary>
        /// Process the exception happened on rest call.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        private void HandleException(Exception exception)
        {
            if (exception is WebException webException && webException.Response != null && webException.Response.ContentType.ToLower().Contains("application/json"))
            {
                Stream stream = null;

                try
                {
                    stream = webException.Response.GetResponseStream();
                    if (stream != null)
                    {
                        string errorObjectString;
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            stream = null;
                            errorObjectString = reader.ReadToEnd();
                        }

                        ClientError errorCollection = JsonConvert.DeserializeObject<ClientError>(errorObjectString);

                        // HandwritingOcr error message use the latest format, so add the logic to handle this issue.
                        if (errorCollection.Code == null && errorCollection.Message == null)
                        {
                            var errorType = new { Error = new ClientError() };
                            var errorObj = JsonConvert.DeserializeAnonymousType(errorObjectString, errorType);
                            errorCollection = errorObj.Error;
                        }

                        if (errorCollection != null)
                        {
                            throw new ClientException
                            {
                                Error = errorCollection,
                            };
                        }
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
            }

            throw exception;
        }

        /// <summary>
        /// This class is used to pass on "state" between each Begin/End call
        /// It also carries the user supplied "state" object all the way till
        /// the end where is then hands off the state object to the
        /// WebRequestCallbackState object.
        /// </summary>
        internal class WebRequestAsyncState
        {
            /// <summary>
            /// Gets or sets request bytes of the request parameter for http post.
            /// </summary>
            public byte[] RequestBytes { get; set; }

            /// <summary>
            /// Gets or sets the HttpWebRequest object.
            /// </summary>
            public HttpWebRequest WebRequest { get; set; }

            /// <summary>
            /// Gets or sets the request state object.
            /// </summary>
            public object State { get; set; }
        }

        #endregion
    }
}
