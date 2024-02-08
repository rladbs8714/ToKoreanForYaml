using System.ComponentModel.DataAnnotations;
using ToKorean.Attribute;
using ToKorean.Papago;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using IParser = YamlDotNet.Core.IParser;

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
            Parse(content);
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
