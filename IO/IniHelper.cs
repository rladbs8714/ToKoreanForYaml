using System.Runtime.InteropServices;
using System.Text;

namespace IO
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

    public class IniHelper
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

        [Obsolete]
        private IniHelper()
        {
            Path = string.Empty;
        }

        /// <summary>
        /// 경로의 ini파일 참조를 생성한다.
        /// 기본값은 상대 경로의 ini\System.ini 이다.
        /// </summary>
        /// <param name="iniFilePath">참조할 ini파일</param>
        /// <exception cref="FileNotFoundException">파일을 찾을 수 없다면 </exception>
        public IniHelper(string iniFilePath = "")
        {
            if (string.IsNullOrEmpty(iniFilePath))
                iniFilePath = System.IO.Path.Combine("ini", "System.ini");

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
        public string Read(string key, string section)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, Path);
            return retVal.ToString();
        }
    }
}
