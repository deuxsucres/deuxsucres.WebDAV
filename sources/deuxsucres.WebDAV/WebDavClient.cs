﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
            _httpHandler = handler;
            Uri = new Uri(uri);
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
        protected virtual HttpRequestMessage CreateWebRequest(Uri uri, string method = "GET", IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), new Uri(Uri, uri));
            if (!string.IsNullOrWhiteSpace(UserAgent))
                request.Headers.UserAgent.ParseAdd(UserAgent);

            //// Authentication available ?
            //if (CurrentAuthenticator != null)
            //{
            //    if (!CurrentAuthenticator.IsAlive())
            //    {
            //        CurrentAuthenticator = null;
            //    }
            //    else
            //    {
            //        CurrentAuthenticator.Authorize(this, request);
            //    }
            //}

            RequestCreated?.Invoke(this, new WebRequestEventArgs(request));

            return request;
        }

        /// <summary>
        /// Execute a request and returns the response
        /// </summary>
        public async Task<HttpResponseMessage> ExecuteWebRequestAsync(
            string path,
            string method,
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
            string method = "GET",
            IDictionary<string, string> headers = null,
            HttpContent content = null
            )
            => ExecuteWebRequestAsync(path, method, headers, content, CancellationToken.None);

        #endregion

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
