namespace Translate.Papago
{
    internal interface IPapagoHelper
    {
        Task<string> TranslateToKorean(string eng);

        Task<string[]> TranslateToKorean(string[] engs);
    }
}
