using System.Text;
using System.Text.Json;
using ToKorean.Http;
using ToKorean.Parser;

namespace ToKorean.Translater.DeepL
{
    internal class DeepLHelper : HttpHelper, ITranslateHelper
    {

        #region SINGLETON

        private static DeepLHelper _instance;

        public static DeepLHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DeepLHelper();
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
        private readonly string URL;
        /// <summary>
        /// Naver Client ID
        /// </summary>
        private readonly string AUTH_KEY;
        /// <summary>
        /// MediaType
        /// </summary>
        private readonly string MEDIA_TYPE;
        /// <summary>
        /// Header Secret
        /// </summary>
        private readonly string HEADER_AUTH;


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private DeepLHelper() : base()
        {
            IniHelper iniHelper = new IniHelper(@"ini\DeepL.ini");

            URL = iniHelper.Read("url", "DEEPL");
            AUTH_KEY = iniHelper.Read("auth_key", "DEEPL");
            MEDIA_TYPE = iniHelper.Read("media_type", "DEEPL");
            HEADER_AUTH = iniHelper.Read("header_auth", "DEEPL");

            Client.BaseAddress = new Uri(URL);

            Client.DefaultRequestHeaders.Accept.Clear();

            Client.DefaultRequestHeaders.Add(HEADER_AUTH, $"DeepL-Auth-Key {AUTH_KEY}");
        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        /// <summary>
        /// Override.
        /// 원문을 받아 한글로 번역한 문자열을 리턴한다.
        /// </summary>
        /// <param name="eng">원문</param>
        /// <returns>번역된 한글 문자열</returns>
        public async Task<string> TranslateToKorean(string eng)
        {
            string[] contentList =
            {
                $"text={Uri.EscapeDataString(eng)}",
                $"source_lang={Uri.EscapeDataString("EN")}",
                $"target_lang={Uri.EscapeDataString("KO")}"
            };

            string? json = await PostAsync(new StringContent(string.Join('&', contentList), Encoding.UTF8, MEDIA_TYPE));
            return GetTranslatedText(json);
        }

        /// <summary>
        /// Override.
        /// 원문 리스트를 받아 한글로 번역한 문자열을 리턴한다.
        /// </summary>
        /// <param name="engs">원문 리스트</param>
        /// <returns>번역된 한글 문자열 리스트. 새로운 배열이 생성된다.</returns>
        public Task<string[]> TranslateToKorean(string[] engs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PapagoAPI에서 응답받은 Json에서 번역된 문자열을 추출한다.
        /// </summary>
        /// <param name="json">응답받은 Json</param>
        /// <returns>번역된 문자열</returns>
        private string GetTranslatedText(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            DeepL_VO? vo;

            try
            {
                vo = JsonSerializer.Deserialize<DeepL_VO>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return string.Empty;
            }

            if (vo == null)
                return string.Empty;

            return vo.translations[0].text;
        }
    }
}
