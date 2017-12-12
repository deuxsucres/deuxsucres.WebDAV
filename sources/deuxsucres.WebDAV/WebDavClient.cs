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

        /// <summary>
        /// Ensure the response is a success
        /// </summary>
        public async virtual Task<HttpResponseMessage> EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = null;
                try
                {
                    using (var str = await response.Content.ReadAsStreamAsync())
                    {
                        var content = XDocument.Load(str);
                        if (content.Root.Name == WebDavConstants.NsDAV.GetName("error"))
                        {
                            var msg = content.Root.Descendants().Where(n => n.Name.ToString().Contains("message")).FirstOrDefault();
                            if (msg != null)
                                errorMessage = (string)msg;
                        }
                    }
                }
                catch { }
                if (errorMessage != null)
                    throw new WebDavException(errorMessage);
            }
            return response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Read the content as XML document
        /// </summary>
        public async virtual Task<XDocument> ReadXmlContentAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return XDocument.Load(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// Extract a result from a response
        /// </summary>
        public async virtual Task<T> ExtractResult<T>(HttpResponseMessage response, bool checkName = true) where T : DavNode
        {
            var doc = await ReadXmlContentAsync(response);
            return DavNode.LoadNode<T>(doc.Root, checkName);
        }

        #endregion

        #region Content

        /// <summary>
        /// Create a document from a root element
        /// </summary>
        protected XDocument CreateDocument(XElement root)
        {
            root.Add(new XAttribute(XNamespace.Xmlns + "D", WebDavConstants.NsDAV.NamespaceName));
            return new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
        }

        /// <summary>
        /// Create a content from a XML element as root
        /// </summary>
        protected HttpContent BuildContent(XElement root)
        {
            var doc = CreateDocument(root);
            var content = new StringContent(doc.Declaration.ToString() + Environment.NewLine + doc.ToString());
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml") {
                CharSet = "utf-8"
            };
            return content;
        }

        /// <summary>
        /// Create a content from a DAV content as root
        /// </summary>
        protected HttpContent BuildContent(DavNode root)
        {
            return BuildContent(root.Node);
        }

        #endregion

        /// <summary>
        /// Valid if a method is allowed for a path
        /// </summary>
        protected async Task<DavOptions> ValidAllowedMethodAsync(string path, HttpMethod method, CancellationToken? cancellationToken)
        {
            if (!CheckAllowedMethod) return null;
            // Valid DAV options
            DavOptions options = await OptionsAsync(path, cancellationToken: cancellationToken);
            if (!options.IsAllowed(method))
                throw new WebDavException(string.Format(Locales.SR.Err_MethodNotAllowed, WebDavConstants.PropFind));
            return options;
        }

        /// <summary>
        /// Do a GET call
        /// </summary>
        public async Task<HttpResponseMessage> GetAsync(string path
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, HttpMethod.Get, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, HttpMethod.Get, headers, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a HEAD call
        /// </summary>
        public async Task<HttpResponseMessage> HeadAsync(string path
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, HttpMethod.Head, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, HttpMethod.Head, headers, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a POST call
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync(string path
            , HttpContent content = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, HttpMethod.Post, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, HttpMethod.Post, headers, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a PUT call
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync(string path
            , HttpContent content = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, HttpMethod.Put, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, HttpMethod.Put, headers, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a DELETE call
        /// </summary>
        public async Task<HttpResponseMessage> DeleteAsync(string path
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, HttpMethod.Delete, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, HttpMethod.Delete, headers, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do an OPTIONS call
        /// </summary>
        public async Task<DavOptions> OptionsAsync(string path, IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null)
        {
            var response = await ExecuteWebRequestAsync(path, WebDavConstants.Options, headers, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = new DavOptions();

            if (!response.Headers.TryGetValues(WebDavConstants.DavHeader, out IEnumerable<string> values))
                throw new WebDavException(Locales.SR.Err_NotDavResource);

            var parts = values.SelectMany(val => val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();
            foreach (string part in parts)
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
        /// Get the list of the properties
        /// </summary>
        public async Task<DavMultistatus> PropListAsync(string path, IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, WebDavConstants.PropFind, cancellationToken);

            // Call PROPFIND
            headers = headers ?? new Dictionary<string, string>();
            headers["Depth"] = DepthValue.One.ToHeaderValue();

            HttpContent content = BuildContent(DavNode.CreateNode<DavPropfind>().AsPropname());

            var response = await ExecuteWebRequestAsync(path, WebDavConstants.PropFind, headers, content, cancellationToken);
            return await ExtractResult<DavMultistatus>(response);
        }

        /// <summary>
        /// Do a PROPFIND call
        /// </summary>
        public async Task<DavMultistatus> PropFindAsync(string path
            , bool allProperties
            , DepthValue depth = DepthValue.Zero
            , IEnumerable<DavProperty> properties = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, WebDavConstants.PropFind, cancellationToken);

            // Call PROPFIND
            headers = headers ?? new Dictionary<string, string>();
            headers["Depth"] = depth.ToHeaderValue();

            var propfind = DavNode.CreateNode<DavPropfind>();
            if (allProperties)
                propfind = propfind.AsAllProp(properties);
            else
                propfind = propfind.AsProp(properties);

            HttpContent content = BuildContent(propfind);

            var response = await ExecuteWebRequestAsync(path, WebDavConstants.PropFind, headers, content, cancellationToken);
            return await ExtractResult<DavMultistatus>(response);
        }

        /// <summary>
        /// Do a PROPPATCH call
        /// </summary>
        public async Task<DavMultistatus> PropPatchAsync(string path
            , DavPropertyUpdate updates
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            HttpContent content = BuildContent(updates ?? throw new ArgumentNullException(nameof(updates)));
            await ValidAllowedMethodAsync(path, WebDavConstants.PropPatch, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, WebDavConstants.PropPatch, headers, content, cancellationToken);
            return await ExtractResult<DavMultistatus>(response);
        }

        /// <summary>
        /// Do a PROPPATCH call
        /// </summary>
        public async Task<DavMultistatus> PropPatchAsync(string path
            , Action<DavPropertyUpdate> updates
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            var upds = DavNode.CreateNode<DavPropertyUpdate>();
            updates?.Invoke(upds);
            return await PropPatchAsync(path, upds, headers, cancellationToken);
        }

        /// <summary>
        /// Do a MKCOL call
        /// </summary>
        public async Task<HttpResponseMessage> MkColAsync(string path
            , HttpContent content = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, WebDavConstants.PropPatch, cancellationToken);
            var response = await ExecuteWebRequestAsync(path, WebDavConstants.MkCol, headers, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a COPY call
        /// </summary>
        public async Task<HttpResponseMessage> CopyAsync(string path
            , string destination
            , bool? overwrite = null
            , DepthValue? depth = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, WebDavConstants.Copy, cancellationToken);

            headers = headers ?? new Dictionary<string, string>();
            headers["Destination"] = CreateUriFromPath(destination).ToString();
            if (overwrite.HasValue)
                headers["Overwrite"] = overwrite.Value ? "T" : "F";
            if (depth.HasValue)
                headers["Depth"] = depth.Value.ToHeaderValue();

            var response = await ExecuteWebRequestAsync(path, WebDavConstants.Copy, headers, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// Do a MOVE call
        /// </summary>
        public async Task<HttpResponseMessage> MoveAsync(string path
            , string destination
            , bool? overwrite = null
            , DepthValue? depth = null
            , IDictionary<string, string> headers = null
            , CancellationToken? cancellationToken = null
            )
        {
            await ValidAllowedMethodAsync(path, WebDavConstants.Move, cancellationToken);

            headers = headers ?? new Dictionary<string, string>();
            headers["Destination"] = CreateUriFromPath(destination).ToString();
            if (overwrite.HasValue)
                headers["Overwrite"] = overwrite.Value ? "T" : "F";
            if (depth.HasValue)
                headers["Depth"] = depth.Value.ToHeaderValue();

            var response = await ExecuteWebRequestAsync(path, WebDavConstants.Move, headers, null, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
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
        /// Check the method is allowed on each call
        /// </summary>
        public bool CheckAllowedMethod { get; set; } = true;

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
