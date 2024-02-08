using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean.Parser
{
    internal abstract class ParserBase
    {

        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// Key와 Value를 저장하는 딕셔너리
        /// </summary>
        public Dictionary<object, object> Data { get; protected set; }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        protected ParserBase()
        {
            Data = new Dictionary<object, object>();
        }


        // ==============================================================================
        // ABSTRACT
        // ==============================================================================

        /// <summary>
        /// 문자열을 받아 파싱한다.
        /// </summary>
        /// <param name="content"></param>
        protected abstract void Parse(string content);

        /// <summary>
        /// 문자열 배열을 받아 파싱한다.
        /// </summary>
        /// <param name="content"></param>
        protected abstract void Parse(string[] lines);

    }
}
