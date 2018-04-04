using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gazo {
    class WebClientEx : WebClient {
        public WebClientEx() {
            this.Encoding = Encoding.UTF8;
        }

        public WebClientEx(CookieContainer cookie) {
            this.Encoding = Encoding.UTF8;

            this.cookieContainer = cookie;
        }

        public WebClientEx(int timeout) {
            this.Encoding = Encoding.UTF8;

            this.timeout = timeout;
        }

        public WebClientEx(string proxy) {
            this.Encoding = Encoding.UTF8;

            this.setProxy(proxy);
        }

        public WebClientEx(CookieContainer cookie, int timeout) {
            this.Encoding = Encoding.UTF8;

            this.cookieContainer = cookie;
            this.timeout = timeout;
        }

        public WebClientEx(CookieContainer cookie, string proxy) {
            this.Encoding = Encoding.UTF8;

            this.cookieContainer = cookie;
            this.setProxy(proxy);
        }

        public WebClientEx(CookieContainer cookie, int timeout, string proxy) {
            this.Encoding = Encoding.UTF8;

            this.cookieContainer = cookie;
            this.timeout = timeout;
            this.setProxy(proxy);
        }

        public void setProxy(string proxy) {
            this.Proxy = new WebProxy("http://" + proxy);
        }

        public void setProxy(string host, string port) {
            this.Proxy = new WebProxy("http://" + host + ":" + port);
        }

        public void setReferer(string address) {
            this.Headers[HttpRequestHeader.Referer] = address;
        }

        public void setUserAgent(string text) {
            this.Headers[HttpRequestHeader.UserAgent] = text;
        }

        public void addJSHeader() {
            this.Headers["X-Requested-With"] = "XMLHttpRequest";
        }

        public WebClientEx buildChromeGET() {
            this.Headers.Clear();

            this.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            this.Headers[HttpRequestHeader.AcceptLanguage] = "en-US;q=0.8,en;q=0.6";
            this.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            return this;
        }

        public WebClientEx buildChromePOST() {
            this.Headers.Clear();

            this.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            this.Headers[HttpRequestHeader.AcceptLanguage] = "en-US;q=0.8,en;q=0.6";
            this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            this.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            return this;
        }

        public CookieContainer cookieContainer;
        public int timeout = 10000;

        protected override WebRequest GetWebRequest(Uri uri) {
            WebRequest webRequest = base.GetWebRequest(uri);

            if (webRequest is HttpWebRequest) {
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.CookieContainer = this.cookieContainer;
                httpWebRequest.Timeout = timeout;
            }

            return webRequest;
        }
    }
}
