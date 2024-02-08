using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean.Papago
{
    internal interface IPapagoHelper
    {
        Task<string> TranslateToKorean(string eng);

        Task<string[]> TranslateToKorean(string[] engs);
    }
}
