using System.Reflection;
using System.Text.RegularExpressions;
using ToKorean.Attribute;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using Translater;
using Translater.Papago;
using Translater.DeepL;
using IO;

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
namespace ToKorean.Parser
{

    /* ==================================================================================
     * 공식 YAML Parser를 이용하는게 좋을 것 같아 Nuget을 다운받아 사용함.
     * Nuget에서 "YamlDotNet"를 검색하면 나옴.
     * https://yaml-net-parser.sourceforge.net/
     * 
     * TODO.
     * - ToKorean의 내용을 정규표현식으로 구현하기
     * ================================================================================== */

    internal class YMLParser : ParserBase
    {

        public enum ETranslateAPI
        {
            Papago,
            DeepL
        }


        // ==============================================================================
        // FIELD
        // ==============================================================================

        private const string ACT = "YMLParser";

        /// <summary>
        /// Yaml Stream
        /// </summary>
        private readonly YamlStream _stream;
        /// <summary>
        /// Papago Helper
        /// </summary>
        private readonly ITranslateHelper _translate;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// 태그 저장 딕셔너리
        /// </summary>
        private Dictionary<string, string> Tags { get; set; }
        /// <summary>
        /// 현재 Tags의 아이템 개수로 태그를 생성한다.
        /// </summary>
        private string TagText { get { return $"{{{Tags.Count}}}"; } }
        /// <summary>
        /// 매핑된 Yaml Node
        /// </summary>
        private YamlMappingNode Mapping { get; set; }
        /// <summary>
        /// LogManager
        /// </summary>
        private ILogManager Log { get { return LogManager.Instance; } }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private YMLParser() { }

        /// <summary>
        /// yml 콘텐츠를 받아 파싱한 후 데이터를 저장한다.
        /// </summary>
        /// <param name="content">raw yml</param>
        /// <exception cref="InvalidCastException"></exception>
        public YMLParser(string content, ETranslateAPI translateAPI)
            : base()
        {
            switch (translateAPI)
            {
                case ETranslateAPI.Papago:
                    _translate = PapagoHelper.Instance;
                    break;
                case ETranslateAPI.DeepL:
                    _translate = DeepLHelper.Instance;
                    break;
            }

            // Setup the input
            StringReader input = new StringReader(content);

            // Load the stream
            _stream = new YamlStream();
            _stream.Load(input);

            try
            {
                Mapping = (YamlMappingNode)_stream.Documents[0].RootNode;
            }
            catch
            {
                throw;
            }

            Count = Mapping.AllNodes.Count();
        }

        /// <summary>
        /// 파싱한 데이터를 모두 지운다.
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// YML 문자열을 받아 파싱한다.
        /// </summary>
        /// <param name="content">원문 텍스트</param>
        [NotUsed]
        protected override void Parse(string content) { }

        /// <summary>
        /// 문자열 배열을 받아 파싱한다.
        /// </summary>
        /// <param name="lines"></param>
        [NotUsed]
        protected override void Parse(string[] lines) { }

        /// <summary>
        /// 파싱된 Yaml의 Values를 한글로 번역한다.
        /// 이때 특정 키워드는 무시한다.
        /// </summary>
        /// <returns>한글로 번역된 Yaml</returns>
        public async Task<string> ToKorean()
        {
            string doc = "ToKorean";
            Log.Logging(ACT, doc, "sta >");

            foreach (YamlNode node in Mapping.AllNodes)
            {
                // ======================================================================
                // Pass Check
                // ======================================================================

                OnPerformStep(this, 1);
                
                // node가 null일 때, Pass
                if (node == null)
                    continue;

                // node가 명백히 'Key'일 때, Pass
                if (node is YamlMappingNode m)
                    continue;

                PropertyInfo? valuePI = node.GetType().GetProperty("Value");
                PropertyInfo? stylePI = node.GetType().GetProperty("Style");

                // node에 Value나 Style 프로퍼티가 없을 때, Pass
                if (stylePI == null || valuePI == null)
                    continue;

                // node의 Style이 키 형식의 Style일 때, Pass
                if (stylePI.GetValue(node) is ScalarStyle ss &&
                    (ss == ScalarStyle.Literal || ss == ScalarStyle.Plain))
                    continue;

                // ======================================================================
                // logic
                // 1. get value ( > english)
                // 2 keyword to index (e.g. &e -> {1}, #22ff00 -> {2}, [Keyword] -> {3}, ...)
                // 3. to korean
                // 4. index to keyword (e.g. {1} -> &e, {2} -> #22ff00, {3} -> [Keyword], ...)
                // 5. set value ( < korean)
                // ======================================================================

                string foreachDoc = $"{doc}.Translate";
                Log.Logging(ACT, foreachDoc, "sta > ");

                // Get Value
                string? value = valuePI.GetValue(node) as string;
                if (string.IsNullOrEmpty(value))
                {
                    Log.Logging(ACT, foreachDoc, "end < value is empty");
                    continue;
                }
                Log.Logging(ACT, foreachDoc, $"exe > get value ({value})");

                // keyword to index
                value = KeywordToIndex(value);
                Log.Logging(ACT, foreachDoc, $"exe > keyword to index ({value})");

                // value to korean
                value = await _translate.TranslateToKorean(value);
                Log.Logging(ACT, foreachDoc, $"exe > translate to korean ({value})");

                // index to keyword
                value = IndexToKeyword(value);
                Log.Logging(ACT, foreachDoc, $"exe > index to keyword ({value})");

                // set value
                valuePI.SetValue(node, value);
                Log.Logging(ACT, foreachDoc, $"exe > set value ({value})");

                OnLastActionEvent(this, value);
                Log.Logging(ACT, foreachDoc, "end <");
            }

            StringWriter sw = new StringWriter();
            _stream.Save(sw);

            Log.Logging(ACT, doc, "end <");
            return sw.ToString();
        }

        /// <summary>
        /// 태그 문자열을 번역되지 않게 인덱스로 치환한다.
        /// 치환된 인덱스는 번역이 끝나고 원래 태그로 다시 복원되어야 한다.
        /// </summary>
        /// <param name="text">원문</param>
        /// <returns>실패시 원문 그대로 리턴, 태그가 인덱스로 치환된 문자열.</returns>
        private string KeywordToIndex(string text)
        {
            // Tags 초기화
            Tags = new Dictionary<string, string>();
            string back = text;

            int i = text.Length - 1;
            while (i >= 0)
            {
                // &- 치환
                if (text[i] == '&')
                {
                    Regex regex = new Regex(@"[0-9a-z]");
                    if (regex.IsMatch(text[i + 1].ToString()))
                    {
                        // '&'뒤에 문자가 하나 더 있을 때
                        text = IndexedText(ref text, text.Substring(i, 2), TagText);
                    }
                    else
                    {
                        // '&'뒤에 문자가 없을 때
                        text = IndexedText(ref text, text.Substring(i, 1), TagText);
                    }
                }

                // #...# 치환
                if (text[i] == '#')
                {
                    string keyword         = text.Substring(i, 7);
                    string keywordEndSharp = text.Substring(i, 8);

                    Regex regex = new Regex(@"#?[0-9A-Z]{6}");
                    Regex regexEndSharp = new Regex(@"#?[0-9A-Z]{6}#");
                    if (regex.IsMatch(keyword))
                    {
                        string index = TagText;
                        Tags.Add(index, keyword);
                        text = text.Replace(keyword, index);
                    }
                    else if(regexEndSharp.IsMatch(keywordEndSharp))
                    {
                        string index = TagText;
                        Tags.Add(index, keywordEndSharp);
                        text = text.Replace(keywordEndSharp, index);
                    }
                    else
                    {
                        // Exception
                        Console.WriteLine("잘못된 텍스트");
                    }
                }

                // [...] 치환
                if (text[i] == ']')
                {
                    try
                    {
                        text = ReplaceRangeWithKeyword(text, ref i, '[');
                    }
                    catch
                    {
                        return back;
                    }
                }

                // {...} 치환
                if (text[i] == '}')
                {
                    try
                    {
                        text = ReplaceRangeWithKeyword(text, ref i, '{');
                    }
                    catch
                    {
                        return back;
                    }
                }

                i -= 1;
            }

            return text;
        }

        private string IndexedText(ref string src, string keyword, string index)
        {
            Tags.Add(index, keyword);               // 키와 값을 먼저 저장 하고
            return src.Replace(keyword, index);     // 치환
        }

        /// <summary>
        /// 인덱스 문자열을 다시 태그 문자열로 치환한다.
        /// </summary>
        /// <param name="text">번역된 문자열</param>
        /// <returns>인덱스가 태그로 치환된 문자열</returns>
        private string IndexToKeyword(string text)
        {
            foreach (KeyValuePair<string, string> kvp in Tags)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="curIdx"></param>
        /// <param name="find"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        private string ReplaceRangeWithKeyword(string text, ref int curIdx, char find)
        {
            int sta = curIdx;

            try
            {
                while (text[--curIdx] != find) ;
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }

            // string keyword = text.Substring(sta, curIdx - sta + 1);
            string keyword = text.Substring(curIdx, sta - curIdx + 1);
            string index = TagText;
            Tags.Add(index, keyword);                // 키와 값을 먼저 저장하고
            return text.Replace(keyword, index);     // 치환
        }
    }
}
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
