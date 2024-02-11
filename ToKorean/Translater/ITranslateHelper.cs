namespace ToKorean.Translater
{
    internal interface ITranslateHelper
    {
        Task<string> TranslateToKorean(string eng);

        Task<string[]> TranslateToKorean(string[] engs);
    }
}
