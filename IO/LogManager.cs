namespace IO
{
    public class LogManager : ILogManager
    {

        #region

        private static LogManager? _instance;

        public static LogManager Instance
        {
            get
            {
                _instance ??= new LogManager();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion


        // ==============================================================================
        // FIELD
        // ==============================================================================

        /// <summary>
        /// 액션 오른쪽 패딩 값
        /// </summary>
        private const int ACT_PAD = 15;
        /// <summary>
        /// 메서드 오른쪽 패딩 값
        /// </summary>
        private const int METHOD_PAD = 25;

        /// <summary>
        /// 현재 날짜
        /// </summary>
        private int _currentDay = 0;
        /// <summary>
        /// 현재 이용중인 로그 파일 이름 (경로 포함)
        /// </summary>
        private string _currentFileFullName = "";
        /// <summary>
        /// 로그가 작성 될 스트림
        /// </summary>
        private StreamWriter? _writer;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// Log 최상위 폴더
        /// </summary>
        private string Root { get { return @"log"; } }
        /// <summary>
        /// 로그를 작성하는 날짜
        /// </summary>
        private string Date { get { return DateTime.Now.ToString("yyyyMMdd"); } }
        /// <summary>
        /// 오늘 날짜를 정수로 반환한다.
        /// </summary>
        private int Day { get { return DateTime.Today.Day; } }
        /// <summary>
        /// 로그를 작성하는 시간
        /// </summary>
        private string Time { get { return DateTime.Now.ToString("HH:mm:ss.fffff"); } }
        /// <summary>
        /// 현재 작성될 로그 파일
        /// </summary>
        private string FileName { get { return $"Log_{Date}.log"; } }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        private LogManager()
        {
            CheckFile();
            OpenStreamWriter(_currentFileFullName, true);
        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        /// <summary>
        /// 로그를 작성한다.
        /// </summary>
        /// <param name="act">기능/행위</param>
        /// <param name="method">로깅 발생 메서드</param>
        /// <param name="message">로깅 메세지</param>
        public void Logging(string act, string method, string message)
        {
            CheckFile();

            string line = $"{Time} {act.PadRight(ACT_PAD)}{method.PadRight(METHOD_PAD)}{message}";
            _writer?.WriteLine(line);
        }

        /// <summary>
        /// 로그 파일의 경로와 유무를 체크한다.
        /// 경로나 파일이 존재하지 않다면 생성한다.
        /// </summary>
        private void CheckFile()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            if (_currentDay != Day)
            {
                _currentDay = Day;
                _currentFileFullName = Path.Combine(Root, FileName);
            }

            if (!File.Exists(_currentFileFullName))
            {
                File.Create(_currentFileFullName).Close();
                CloseStreamWriter();
                OpenStreamWriter(_currentFileFullName, true);
            }
        }

        /// <summary>
        /// StreamWriter를 연다.
        /// </summary>
        /// <param name="filePath">Stream을 생성할 파일</param>
        /// <param name="append">true일 시 파일의 기존 내용을 유지하며 내용 추가, false시 내용 덮어쓰기</param>
        private void OpenStreamWriter(string filePath, bool append)
        {
            if (_writer != null)
                CloseStreamWriter();
            _writer = new StreamWriter(filePath, append)
            {
                AutoFlush = true
            };
        }

        /// <summary>
        /// Stream Writer를 닫는다.
        /// </summary>
        private void CloseStreamWriter()
        {
            if (_writer == null)
                return;

            _writer.Flush();
            _writer.Close();    // in Dispose
        }
    }
}
