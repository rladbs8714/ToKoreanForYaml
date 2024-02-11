using System.Text;
using ToKorean.IO;
using ToKorean.Parser;
using ToKorean.Translater;
using ToKorean.Translater.DeepL;
using ToKorean.Translater.Papago;

namespace ToKorean
{
    public partial class MainForm : Form
    {

        // ==============================================================================
        // FIELD
        // ==============================================================================

        private ParserBase _parser;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// 파파고 API를 사용한 번역 도우미
        /// </summary>
        private ITranslateHelper Papago { get { return PapagoHelper.Instance; } }


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
        // EVENT
        // ==============================================================================

        public event EventHandler<string> LastActionEvent;


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

            try
            {
                _parser = new YMLParser(txtOrigin.Text, translateAPI);
            }
            catch (InvalidCastException)
            {
                staProgBar.Maximum = 1;
                staProgBar.Value = 1;

                staLastAction.Text = "YAML 형식이 잘못되었습니다. 내용을 다시 확인하세요.";
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

            staLastAction.Text = "완료.";
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

            using FileDialogHelper fh = new FileDialogHelper(filter)
            {
                MultiSelect = false,
                Title = "파일 선택"
            };

            string ymlPath = fh.GetFilePath();

            if (string.IsNullOrEmpty(ymlPath) ||
                !File.Exists(ymlPath))
            {
                Console.WriteLine("파일을 찾을 수 없습니다.");
                return;
            }

            string[] ymlLines = File.ReadAllLines(ymlPath);
            StringBuilder sb = new StringBuilder();

            foreach (string line in ymlLines)
            {
                sb.AppendLine(line);
            }

            txtOrigin.Text = sb.ToString();
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
