using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        HttpClient _httpClient;
        HttpMessageHandler _httpHandler;

        /// <summary>
        /// Create a new WebDAV client
        /// </summary>
        public WebDavClient(string uri, string userName = null, string password = null, HttpMessageHandler handler = null)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentException(Locales.SR.Err_InvalidServerUri);
            _httpHandler = handler;
            ServerUri = new Uri(uri, UriKind.Absolute);
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
        /// Create an uri from a path within the WebDAV uri path
        /// </summary>
        /// <remarks>
        /// If the path start with a '/' then it remove.
        /// </remarks>
        protected virtual Uri CreateUriFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return ServerUri;
            return new Uri(ServerUri, path.TrimStart('/'));
        }

        /// <summary>
        /// Create a new request
        /// </summary>
        protected virtual HttpRequestMessage CreateWebRequest(Uri uri, HttpMethod method, IDictionary<string, string> headers, HttpContent content)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get, uri);
            if (!string.IsNullOrWhiteSpace(UserAgent))
                request.Headers.UserAgent.ParseAdd(UserAgent);
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Key, header.Value);
            }
            request.Content = content;

            return request;
        }

        /// <summary>
        /// Execute a request from an uri and returns the response
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteWebRequestAsync(
            Uri uri,
            HttpMethod method,
            IDictionary<string, string> headers = null,
            HttpContent content = null,
            CancellationToken? cancellationToken = null
            )
        {
            // Create and execute the request
            var request = CreateWebRequest(uri, method, headers, content);

            // Get the response
            BeforeExecuteRequest?.Invoke(this, new WebRequestEventArgs(request));
            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken ?? CancellationToken.None);
            AfterExecuteRequest?.Invoke(this, new WebResponseEventArgs(response));

            // Return the response
            return response;
        }

        /// <summary>
        /// Execute a request from a path and returns the response
        /// </summary>
        public Task<HttpResponseMessage> ExecuteWebRequestAsync(
            string path,
            HttpMethod method,
            IDictionary<string, string> headers = null,
            HttpContent content = null,
            CancellationToken? cancellationToken = null
            )
        => ExecuteWebRequestAsync(CreateUriFromPath(path), method, headers, content, cancellationToken);

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
            var content = new StringContent(doc.Declaration.ToString() + Environment.NewLine + doc.ToString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            return content;
        }

        #endregion

        /// <summary>
        /// Do an OPTIONS call
        /// </summary>
        public async Task<OptionsResult> DoOptionsAsync(string path, IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await ExecuteWebRequestAsync(path, WebDavConstants.Options, headers, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = new OptionsResult();

            if (!response.Headers.TryGetValues(WebDavConstants.DavHeader, out IEnumerable<string> values))
                throw new WebDavException(Locales.SR.Err_NotDavResource);

            var parts = values.SelectMany(val => val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();
            foreach (var part in parts)
                result.ComplianceClasses.Add(new DavComplianceClass(part));

            if (response.Content != null && response.Content.Headers.TryGetValues(WebDavConstants.AllowHeader, out values))
            {
                parts = values.SelectMany(val => val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .ToList();
                result.Allow.AddRange(parts);
            }

            return result;
        }

        /// <summary>
        /// Do a PROPFIND call
        /// </summary>
        public async Task<HttpResponseMessage> DoPropFindAsync(string path, DepthValue depth = DepthValue.Zero
            , IDictionary<string, string> headers = null, CancellationToken? cancellationToken = null
            )
        {
            // Valid DAV options
            DavOptions options = await DoOptionsAsync(path, cancellationToken: cancellationToken);
            if (!options.IsAllowed(WebDavConstants.PropFind))
                throw new WebDavException(string.Format(Locales.SR.Err_MethodNotAllowed, WebDavConstants.PropFind));

            // Call PROPFIND
            headers = headers ?? new Dictionary<string, string>();
            headers["Depth"] = depth.ToHeaderValue();
            HttpContent content = BuildContent(new XElement(WebDavConstants.NsDAV + "propfind", WebDavConstants.NsDAV, new XElement(WebDavConstants.NsDAV + "allprop")));
            return await ExecuteWebRequestAsync(path, WebDavConstants.PropFind, headers, content);
        }

        /// <summary>
        /// Uri of the server
        /// </summary>
        public Uri ServerUri { get; private set; }

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
        /// Event raised before a request will be executed
        /// </summary>
        public event EventHandler<WebRequestEventArgs> BeforeExecuteRequest;

        /// <summary>
        /// Event raised after a request is executed
        /// </summary>
        public event EventHandler<WebResponseEventArgs> AfterExecuteRequest;

    }

}
