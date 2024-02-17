namespace Translater.Http
{
    public class HttpHelper : IDisposable
    {

        // ==============================================================================
        // FIELD
        // ==============================================================================

        /// <summary>
        /// 해제를 위한 필드 변수
        /// </summary>
        private bool disposedValue;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// Http 요청자
        /// </summary>
        protected HttpClient Client { get; private set; }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        /// <summary>
        /// 기본 생성자
        /// </summary>
        protected HttpHelper()
        {
            Client = new HttpClient();
        }

        /// <summary>
        /// PapagoAPI를 위한 생성자
        /// </summary>
        /// <param name="ClientID">PapagoAPI의 Client ID</param>
        /// <param name="ClientSecret">PapagoAPI의 Client Secret</param>
        protected HttpHelper(string url, string ClientID, string ClientSecret)
        {
            Client = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
            Client.DefaultRequestHeaders.Accept.Clear();

            Client.DefaultRequestHeaders.Add("X-Naver-Client-Id", ClientID);
            Client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", ClientSecret);
        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        /// <summary>
        /// 비동기로 Post를 진행한다.
        /// </summary>
        /// <param name="postContent">Post할 컨텐츠</param>
        /// <param name="uri">추가할 uri</param>
        /// <returns>Response된 메시지</returns>
        internal async Task<string> PostAsync(StringContent postContent, string uri = "")
        {
            if (!string.IsNullOrEmpty(uri) &&
                !Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
            {
                Console.WriteLine("Uri 형식이 올바르지 않음.");
                return string.Empty;
            }

            using StringContent content = postContent;

            HttpResponseMessage response;

            try
            {
                response = await Client.PostAsync(uri, content);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.ToString());
                return string.Empty;
            }

            string responseMsg = await response.Content.ReadAsStringAsync();

            response.Dispose();
            postContent.Dispose();

            return responseMsg;
        }


        // ==============================================================================
        // DISPOSABLE
        // ==============================================================================

        #region DISPOSABLE

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~HttpHelper()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
