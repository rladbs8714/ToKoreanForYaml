#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
namespace ToKorean.Translater.Papago
{
    public class Papago_VO
    {
        public class Message
        {
            public class Result
            {
                public string srcLangType { get; set; }
                public string tarLangType { get; set; }
                public string translatedText { get; set; }
                public string engineType { get; set; }
            }

            public string @type { get; set; }
            public string @service { get; set; }
            public string @version { get; set; }
            public Result result { get; set; }
        }

        public Message message { get; set; }
    }
}
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.