using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ToKorean.Attribute;
using ToKorean.Papago;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using IParser = YamlDotNet.Core.IParser;

#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
namespace ToKorean.Parser
{

    #region 사용 안하는 주석
    /* ==================================================================================
     * 공식 YAML Parser를 이용하는게 좋을 것 같아 Nuget을 다운받아 사용함.
     * Nuget에서 "YamlDotNet"를 검색하면 나옴.
     * https://yaml-net-parser.sourceforge.net/
     * ================================================================================== */
    #endregion

    internal class YMLParser : ParserBase, IYMLTagReplace
    {

        // ==============================================================================
        // INNER CLASS
        // ==============================================================================

        [NotUsed]
        public class ValidatingNodeDeserializer : INodeDeserializer
        {
            private readonly INodeDeserializer _nodeDeserializer;

            public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
                => _nodeDeserializer = nodeDeserializer;

            public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
            {
                if (_nodeDeserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
                {
                    var context = new ValidationContext(value, null, null);
                    Validator.ValidateObject(value, context, true);
                    
                    return true;
                }

                return false;
            }
        }


        // ==============================================================================
        // FIELD
        // ==============================================================================

        /// <summary>
        /// Yaml Stream
        /// </summary>
        private readonly YamlStream _stream;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// 태그 저장 딕셔너리
        /// </summary>
        public Dictionary<string, string>? Tags { get; set; }

        /// <summary>
        /// 현재 Tags의 아이템 개수로 태그를 생성한다.
        /// </summary>
        public string TagText { get { return $"{{{Tags.Count}}}"; } }

        /// <summary>
        /// 매핑된 Yaml Node
        /// </summary>
        public YamlMappingNode Mapping { get; private set; }

        /// <summary>
        /// Papago Helper
        /// </summary>
        private IPapagoHelper Papago { get { return PapagoHelper.Instance; } }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private YMLParser() { }

        /// <summary>
        /// yml 콘텐츠를 받아 파싱한 후 데이터를 저장한다.
        /// </summary>
        /// <param name="content">raw yml</param>
        public YMLParser(string content) : base()
        {
            // Setup the input
            StringReader input = new StringReader(content);

            // Load the stream
            _stream = new YamlStream();
            _stream.Load(input);
        }

        /// <summary>
        /// yml 콘텐츠를 받아 파싱한 후 데이터를 저장한다.
        /// </summary>
        /// <param name="content">raw yml</param>
        public YMLParser(string[] lines) : base()
        {
            Parse(lines);
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
        protected override void Parse(string content)
        {
            
        }

        /// <summary>
        /// 파싱된 Yaml의 Values를 한글로 번역한다.
        /// 이때 특정 키워드는 무시한다.
        /// </summary>
        /// <returns>한글로 번역된 Yaml</returns>
        public async Task<string> ToKorean()
        {
            // Cast Exception Check
            YamlMappingNode mapping =
                (YamlMappingNode)_stream.Documents[0].RootNode;

            foreach (YamlNode node in mapping.AllNodes)
            {
                if (node == null)
                    continue;

                // node가 명백히 'Key'일 때, Pass
                if (node is YamlMappingNode m)
                {
                    continue;
                }

                PropertyInfo? valuePI = node.GetType().GetProperty("Value");
                PropertyInfo? stylePI = node.GetType().GetProperty("Style");

                if (stylePI == null || valuePI == null)
                    continue;

                if (stylePI.GetValue(node) is ScalarStyle ss &&
                    ss != ScalarStyle.SingleQuoted)
                    continue;

                // logic
                // 1. get value ( > english)
                // 2. to korean
                // 3. set value ( < korean)

                // Get Value
                string? value = valuePI.GetValue(node) as string;

                if (string.IsNullOrEmpty(value))
                    continue;

                // value to korean
                value = await Papago.TranslateToKorean(value);

                // set value
                valuePI.SetValue(node, value);
            }

            StringWriter sw = new StringWriter();
            _stream.Save(sw);

            return sw.ToString();
        }

        [NotUsed]
        protected override void Parse(string[] lines) { }

        /// <summary>
        /// 태그 문자열을 번역되지 않게 인덱스로 치환한다.
        /// 치환된 인덱스는 번역이 끝나고 원래 태그로 다시 복원되어야 한다.
        /// </summary>
        /// <param name="text">원문</param>
        /// <returns>실패시 원문 그대로 리턴, 태그가 인덱스로 치환된 문자열.</returns>
        public string Tag2Index(string text)
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
                    Tags.Add(TagText, text.Substring(i, 2));                // 키와 값을 먼저 저장 하고
                    text = text.Replace(text.Substring(i, 2), TagText);     // 치환
                }

                // #...# 치환
                if (text[i] == '#')
                {
                    int sta = i;
                    while (text[--i] != '#' && i < text.Length) ;
                    
                    if (i >= text.Length)
                    {
                        // '#'가 하나밖에 없음. (태그가 잘못됨)
                        // 로그 적어야 함.
                        Console.WriteLine("태그가 잘못 입력되어있습니다.");
                        return back;
                    }

                    Tags.Add(TagText, text.Substring(sta, i - sta + 1));                // 키와 값을 먼저 저장하고
                    text = text.Replace(text.Substring(sta, i - sta + 1), TagText);     // 치환
                }

                i += 1;
            }

            return text;
        }

        /// <summary>
        /// 인덱스 문자열을 다시 태그 문자열로 치환한다.
        /// </summary>
        /// <param name="text">번역된 문자열</param>
        /// <returns>인덱스가 태그로 치환된 문자열</returns>
        public string Index2Tag(string text)
        {
            return "";
        }
    }
}
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
