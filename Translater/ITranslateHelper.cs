namespace Translater
{
    public interface ITranslateHelper
    {
        Task<string> TranslateToKorean(string eng);

        Task<string[]> TranslateToKorean(string[] engs);
    }
}
