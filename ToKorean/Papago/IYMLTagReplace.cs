namespace ToKorean.Papago
{
    internal interface IYMLTagReplace
    {
        /// <summary>
        /// 태그 저장 딕셔너리
        /// </summary>
        Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// 현재 Tags의 아이템 개수로 태그를 생성한다.
        /// </summary>
        string TagText { get; }

        /// <summary>
        /// 태그 문자열을 번역되지 않게 인덱스로 치환한다.
        /// 치환된 인덱스는 번역이 끝나고 원래 태그로 다시 복원되어야 한다.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>태그가 인덱스로 치환된 문자열</returns>
        string Tag2Index(string text);

        /// <summary>
        /// 인덱스 문자열을 다시 태그 문자열로 치환한다.
        /// </summary>
        /// <param name="text">번역된 문자열</param>
        /// <returns>인덱스가 태그로 치환된 문자열</returns>
        string Index2Tag(string text);
    }
}
