namespace Translate.Papago
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
    }
}
