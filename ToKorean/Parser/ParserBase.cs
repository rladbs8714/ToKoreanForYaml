namespace ToKorean.Parser
{
    internal abstract class ParserBase
    {

        // ==============================================================================
        // FIELD
        // ==============================================================================

        public event EventHandler<string>? LastActionEvent;

        public event EventHandler<int>? PerformStepEvent;

        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// Key와 Value를 저장하는 딕셔너리
        /// </summary>
        public Dictionary<object, object> Data { get; protected set; }
        /// <summary>
        /// 파싱된 데이터 개수
        /// </summary>
        public int Count { get; protected set; }


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
        /// <param name="lines"></param>
        protected abstract void Parse(string[] lines);

        protected void OnLastActionEvent(object sender, string message)
        {
            // Last Log
            LastActionEvent?.Invoke(sender, message);
        }

        protected void OnPerformStep(object sender, int step)
        {
            // Progress Bar Step
            PerformStepEvent?.Invoke(sender, step);
        }
    }
}
