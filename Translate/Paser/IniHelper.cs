using System.Runtime.InteropServices;
using System.Text;

namespace Translate.Parser
{

    // ==================================================================================
    // ini 참조 구현은 아래의 링크를 인용한다.
    // - https://stackoverflow.com/questions/217902/reading-writing-an-ini-file
    // ==================================================================================
    // 
    // ini파일 작성 규칙
    // 1. 모든 내용은 영어로 작성되어야 한다.
    // 2. 섹션은 대문자로 작성되어야 한다.
    // 3. 키는 팟홀 패턴으로 작성되어야 한다.

    internal class IniHelper
    {

        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        public string Path { get; private set; }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private IniHelper()
        {
            Path = "";
        }

        public IniHelper(string iniFilePath = "")
        {
            if (string.IsNullOrEmpty(Path))
                iniFilePath = System.IO.Path.Combine("ini", "Papago.ini");

            if (!File.Exists(iniFilePath))
                throw new FileNotFoundException(Path);

            Path = iniFilePath;
        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        /// <summary>
        /// ini파일에서 값을 찾아 반환한다.
        /// </summary>
        /// <param name="key">값을 찾을 키</param>
        /// <param name="section">키가 있을 섹션. 기본값 "PAPAGO"</param>
        public string Read(string key, string section = "PAPAGO")
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, Path);
            return retVal.ToString();
        }
    }
}
