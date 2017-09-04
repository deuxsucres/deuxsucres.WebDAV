﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// WebDAV client
    /// </summary>
    public class WebDavClient
    {
        #region Constants

        /// <summary>
        /// The DAV XML namespace
        /// </summary>
        public static XNamespace NsDAV = XNamespace.Get("DAV:");

        /// <summary>
        /// PROPFIND method
        /// </summary>
        public static HttpMethod PropFind = new HttpMethod("PROPFIND");

        /// <summary>
        /// PROPPATCH method
        /// </summary>
        public static HttpMethod PropPatch = new HttpMethod("PROPPATCH");

        /// <summary>
        /// MKCOL method
        /// </summary>
        public static HttpMethod MkCol = new HttpMethod("MKCOL");

        /// <summary>
        /// COPY method
        /// </summary>
        public static HttpMethod Copy = new HttpMethod("COPY");

        /// <summary>
        /// MOVE method
        /// </summary>
        public static HttpMethod Move = new HttpMethod("MOVE");

        /// <summary>
        /// LOCK method
        /// </summary>
        public static HttpMethod Lock = new HttpMethod("LOCK");

        /// <summary>
        /// UNLOCK method
        /// </summary>
        public static HttpMethod Unlock = new HttpMethod("UNLOCK");

        /// <summary>
        /// OPTIONS method
        /// </summary>
        public static HttpMethod Options = new HttpMethod("OPTIONS");
        #endregion

        HttpClient _httpClient;
        HttpMessageHandler _httpHandler;

        /// <summary>
        /// Create a new WebDAV client
        /// </summary>
        public WebDavClient(string uri, string userName = null, string password = null, HttpMessageHandler handler = null)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentException(Locales.SR.Err_InvalidServerUri);
            _httpHandler = handler;
            Uri = new Uri(uri, UriKind.Absolute);
            User = userName;
            Password = password;
            var asm = typeof(WebDavClient).GetTypeInfo().Assembly;
            string version = asm.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0";
            UserAgent = "deuxsucres.WebDavClient/" + version;
        }

        #region Transport

        /// <summary>
        /// Create a new default Http message handler
        /// </summary>
        /// <returns></returns>
        protected virtual HttpMessageHandler CreateHttpMessageHandler()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            if (!string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Password))
                handler.Credentials = new NetworkCredential(User, Password);
            return handler;
        }

        /// <summary>
        /// Create a new Http client
        /// </summary>
        protected virtual HttpClient CreateHttpClient()
        {
            var handler = _httpHandler ?? CreateHttpMessageHandler();
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
            return client;
        }

        /// <summary>
        /// HTTP Client
        /// </summary>
        protected HttpClient HttpClient => _httpClient ?? (_httpClient = CreateHttpClient());

        /// <summary>
        /// Create a new request
        /// </summary>
        protected virtual HttpRequestMessage CreateWebRequest(Uri uri, HttpMethod method = null, IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get, new Uri(Uri, uri));
            if (!string.IsNullOrWhiteSpace(UserAgent))
                request.Headers.UserAgent.ParseAdd(UserAgent);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Key, header.Value);
            }

            RequestCreated?.Invoke(this, new WebRequestEventArgs(request));

            return request;
        }

        /// <summary>
        /// Execute a request and returns the response
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteWebRequestAsync(
            string path,
            HttpMethod method,
            IDictionary<string, string> headers,
            HttpContent content,
            CancellationToken cancellationToken
            )
        {
            Uri uri = new Uri((path ?? "").TrimStart('/'), UriKind.Relative);
            HttpResponseMessage response = null;

            //IRequestAuthenticatorFactory factory = null;
            //bool endFactoriesReached = false;
            //do
            //{

            // Create and execute the request
            var request = CreateWebRequest(uri, method, headers);

            BeforeExecuteRequest?.Invoke(this, new WebRequestEventArgs(request));

            // Load content
            if (content != null)
                request.Content = content;

            // Get the response
            response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

            AfterExecuteRequest?.Invoke(this, new WebResponseEventArgs(response));

            //    // Authentication required ?
            //    if (response.StatusCode == HttpStatusCode.Unauthorized && !endFactoriesReached)
            //    {
            //        CurrentAuthenticator = MakeAuthenticatorForRequest(response, ref factory);
            //        // if factory is null, then we reached the end of the list
            //        endFactoriesReached = factory == null;
            //    }
            //    else
            //        break;
            //} while (true);

            return response;
        }

        /// <summary>
        /// Execute a request and returns the response
        /// </summary>
        public Task<HttpResponseMessage> ExecuteWebRequestAsync(
            string path,
            HttpMethod method = null,
            IDictionary<string, string> headers = null,
            HttpContent content = null
            )
            => ExecuteWebRequestAsync(path, method, headers, content, CancellationToken.None);

        #endregion

        #region XML

        /// <summary>
        /// Create a document from a root element
        /// </summary>
        protected XDocument CreateDocument(XElement root)
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
        }

        /// <summary>
        /// Create a content from a XML element as root
        /// </summary>
        protected HttpContent BuildContent(XElement root)
        {
            var doc = CreateDocument(root);
            return new StringContent(doc.Declaration.ToString() + Environment.NewLine + doc.ToString());
        }

        #endregion

        /// <summary>
        /// Do a PROPFIND call
        /// </summary>
        public Task<HttpResponseMessage> DoPropFindAsync(string path, DepthValue depth = DepthValue.Zero
            , IDictionary<string, string> headers = null
            )
        {
            headers = headers ?? new Dictionary<string, string>();
            headers["Depth"] = depth.ToHeaderValue();
            HttpContent content = BuildContent(new XElement(NsDAV + "propfind", NsDAV, new XElement(NsDAV + "allprop")));
            return ExecuteWebRequestAsync(path, PropFind, headers, content);
        }

        /// <summary>
        /// Uri of the server
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// User name
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Event raised when a request is created
        /// </summary>
        public event EventHandler<WebRequestEventArgs> RequestCreated;

        /// <summary>
        /// Event raised before a request will be executed
        /// </summary>
        public event EventHandler<WebRequestEventArgs> BeforeExecuteRequest;

        /// <summary>
        /// Event raised after a request is executed
        /// </summary>
        public event EventHandler<WebResponseEventArgs> AfterExecuteRequest;

    }

}
