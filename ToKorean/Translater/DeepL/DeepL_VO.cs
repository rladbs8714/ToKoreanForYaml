using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean.Translater.DeepL
{
    internal class DeepL_VO
    {
        public class Translations
        {
            public string detected_source_language { get; set; }
            public string text { get; set; }
        }

        public Translations[] translations { get; set; }
    }
}
