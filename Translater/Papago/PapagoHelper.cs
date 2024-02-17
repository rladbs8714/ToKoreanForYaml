using System.Text;
using System.Text.Json;
using IO;
using Web.Http;

namespace Translater.Papago
{
    public class PapagoHelper : HttpHelper, ITranslateHelper
    {

        #region SINGLETON

        private static PapagoHelper? _instance;

        public static PapagoHelper Instance
        {
            get
            {
                _instance ??= new PapagoHelper();
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
        /// default section
        /// </summary>
        private const string SECTION = "PAPAGO";

        /// <summary>
        /// Papago URL
        /// </summary>
        private readonly string URL;
        /// <summary>
        /// Naver Client ID
        /// </summary>
        private readonly string CLIENT_ID;
        /// <summary>
        /// Naver Client Secret
        /// </summary>
        private readonly string CLIENT_SECRET;
        /// <summary>
        /// 번역할 텍스트 접두사
        /// </summary>
        private readonly string PREFIX;
        /// <summary>
        /// MediaType
        /// </summary>
        private readonly string MEDIA_TYPE;
        /// <summary>
        /// Header ID
        /// </summary>
        private readonly string HEADER_ID;
        /// <summary>
        /// Header Secret
        /// </summary>
        private readonly string HEADER_SECRET;


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private PapagoHelper() : base()
        {
            IniHelper iniHelper = new IniHelper();

            URL = iniHelper.Read("url", SECTION);
            CLIENT_ID = iniHelper.Read("client_id", SECTION);
            CLIENT_SECRET = iniHelper.Read("client_secret", SECTION);
            PREFIX = iniHelper.Read("prefix", SECTION);
            MEDIA_TYPE = iniHelper.Read("media_type", SECTION);
            HEADER_ID = iniHelper.Read("header_id", SECTION);
            HEADER_SECRET = iniHelper.Read("header_secret", SECTION);

            Client.BaseAddress = new Uri(URL);

            Client.DefaultRequestHeaders.Accept.Clear();

            Client.DefaultRequestHeaders.Add(HEADER_ID, CLIENT_ID);
            Client.DefaultRequestHeaders.Add(HEADER_SECRET, CLIENT_SECRET);
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
            string newLine = PREFIX + eng;
            string? json = await PostAsync(new StringContent(newLine, Encoding.UTF8, MEDIA_TYPE));
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

            if (vo == null || vo.message == null || vo.message.result == null)
                return string.Empty;

            return vo.message.result.translatedText;
        }
    }
}
