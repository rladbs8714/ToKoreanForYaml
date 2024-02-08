using System.Net;
using System.Text;
using System.Text.Json;
using ToKorean.Http;

namespace ToKorean.Papago
{
    internal class PapagoHelper : HttpHelper, IPapagoHelper
    {

        #region SINGLETON

        private static PapagoHelper? _instance;

        public static PapagoHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PapagoHelper();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion


        // ==============================================================================
        // FIELD
        // ==============================================================================

        /// <summary>
        /// Papago URL
        /// </summary>
        private const string URL = "https://openapi.naver.com/v1/papago/n2mt";
        /// <summary>
        /// Naver Client ID
        /// </summary>
        private const string CLIENT_ID = "wcSQ77zvL3kGmdXbtdUU";
        /// <summary>
        /// Naver Client Secret
        /// </summary>
        private const string CLIENT_SECRET = "cfVFHZIlTQ";
        /// <summary>
        /// 번역할 텍스트 접두사
        /// </summary>
        private const string PREFIX = "source=en&target=ko&text=";
        /// <summary>
        /// MediaType
        /// </summary>
        private const string MEDIA_TYPE = "application/x-www-form-urlencoded";


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private PapagoHelper() : base(URL, CLIENT_ID, CLIENT_SECRET)
        {

        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        /// <summary>
        /// 원문을 받아 한글로 번역한 문자열을 리턴한다.
        /// </summary>
        /// <param name="eng">원문</param>
        /// <returns>번역된 한글 문자열</returns>
        public Task<string> TranslateToKorean(string eng)
        {
            return Task.Run(async () =>
            {
                string? json = await PostAsync(new StringContent(PREFIX + eng, Encoding.UTF8, MEDIA_TYPE));
                return GetTranslatedText(json);
            });
        }

        /// <summary>
        /// 원문 리스트를 받아 한글로 번역한 문자열을 리턴한다.
        /// </summary>
        /// <param name="engs">원문 리스트</param>
        /// <returns>번역된 한글 문자열 리스트. 새로운 배열이 생성된다.</returns>
        public Task<string[]> TranslateToKorean(string[] engs)
        {
            return Task.Run(async () =>
            {
                string[] kors = new string[engs.Length];

                for (int i = 0; i < engs.Length; i++)
                {
                    kors[i] = await TranslateToKorean(engs[i]);
                }

                return kors;
            });
        }

        /// <summary>
        /// PapagoAPI에서 응답받은 Json에서 번역된 문자열을 추출한다.
        /// </summary>
        /// <param name="json">응답받은 Json</param>
        /// <returns>번역된 문자열</returns>
        private string GetTranslatedText(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            Papago_VO? vo;

            try
            {
                vo = JsonSerializer.Deserialize<Papago_VO>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return string.Empty;
            }

            if (vo == null)
                return string.Empty;

            return vo.message.result.translatedText;
        }


    }
}
