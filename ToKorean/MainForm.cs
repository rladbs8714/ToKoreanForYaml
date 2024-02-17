using ToKorean.IO;
using ToKorean.Parser;
using IO;

namespace ToKorean
{
    public partial class MainForm : Form
    {

        // ==============================================================================
        // FIELD
        // ==============================================================================

        private const string ACT = "MainForm";

        private ILogManager Log { get { return LogManager.Instance; } }

        private ParserBase? _parser;


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        public MainForm()
        {
            InitializeComponent();

            // Event
            btnOrganize.Click += OpenFileDialog;
            btnPapagoTranslate.Click += TranslateToKorean;
            btnDeepLTranslate.Click += TranslateToKorean;

            btnPapagoTranslate.Tag = YMLParser.ETranslateAPI.Papago;
            btnDeepLTranslate.Tag = YMLParser.ETranslateAPI.DeepL;
        }


        // ==============================================================================
        // EVENT METHOD
        // ==============================================================================

        /// <summary>
        /// btnTranslate Button Event.
        /// txtOrigin의 내용을 번역해서 txtOut에 표현한다.
        /// </summary>
        /// <param name="sender">btnTranslate</param>
        /// <param name="e">ButtonClick Event</param>
        private async void TranslateToKorean(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (((Control)sender).Tag is not YMLParser.ETranslateAPI translateAPI)
                return;

            string doc = "TranslateToKorean";
            Log.Logging(ACT, doc, "sta > Translate");

            try
            {
                _parser = new YMLParser(txtOrigin.Text, translateAPI);
            }
            catch (InvalidCastException)
            {
                staProgBar.Maximum = 1;
                staProgBar.Value = 1;

                staLastAction.Text = "YAML 형식이 잘못되었습니다. 내용을 다시 확인하세요.";
                Log.Logging(ACT, doc, "err > YAML format error. (solution: Please check the format of the yaml you entered.)");
                return;
            }
            catch (Exception ex)
            {
                staLastAction.Text = "Unknown exception.";
                Log.Logging(ACT, "YMLParser", "Unknown exception. (" + ex.Message + ")\r\n" + e.ToString());
                return;
            }

            _parser.LastActionEvent += OnLastAction;
            _parser.PerformStepEvent += OnPerformStep;

            staProgBar.Maximum = _parser.Count;
            staProgBar.Value = 0;

            if (_parser is YMLParser yaml)
            {
                string kor = await yaml.ToKorean();
                txtOut.Text = kor;
            }

            // Complete
            staLastAction.Text = "완료.";
            Log.Logging(ACT, doc, "end < Translate");
        }

        /// <summary>
        /// 다이얼로그를 열어 파일을 선택한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileDialog(object? sender, EventArgs e)
        {
            FileDialogHelper.EFilter filter =
                FileDialogHelper.EFilter.YML |
                FileDialogHelper.EFilter.ALL;

            string doc = "OpenFileDialog";
            Log.Logging(ACT, doc, "sta >");

            using FileDialogHelper fh = new FileDialogHelper(filter)
            {
                MultiSelect = false,
                Title = "파일 선택"
            };

            string ymlPath = fh.GetFilePath();

            if (string.IsNullOrEmpty(ymlPath) ||
                !File.Exists(ymlPath))
            {
                staLastAction.Text = "파일을 찾을 수 없습니다.";
                Log.Logging(ACT, doc, $"err > not found file. ({ymlPath})");
                return;
            }
            Log.Logging(ACT, doc, $"end < open file. ({ymlPath})");

            string yaml = File.ReadAllText(ymlPath);
            txtOrigin.Text = yaml;
        }

        /// <summary>
        /// Last Action 이벤트
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="message">Last Action Message</param>
        private void OnLastAction(object? sender, string message)
        {
            staLastAction.Text = message;
        }

        private void OnPerformStep(object? sender, int step)
        {
            staProgBar.Step = step;
            staProgBar.PerformStep();
        }
    }
}
