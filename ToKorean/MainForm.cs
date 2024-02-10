using System.Reflection.Metadata;
using System.Text;
using ToKorean.IO;
using ToKorean.Papago;
using ToKorean.Parser;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Helpers;

namespace ToKorean
{
    public partial class MainForm : Form, ILastActionEvent
    {

        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        /// <summary>
        /// 파파고 API를 사용한 번역 도우미
        /// </summary>
        private IPapagoHelper Papago { get { return PapagoHelper.Instance; } }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        public MainForm()
        {
            InitializeComponent();

            // Event
            btnTranslate.Click += TranslateToKorean;
            btnOrganize.Click += OpenFileDialog;
            btnYmlParse.Click += YMLParse;
        }


        // ==============================================================================
        // EVENT
        // ==============================================================================

        public event EventHandler<EventArgs> LastActionEvent;


        // ==============================================================================
        // EVENT METHOD
        // ==============================================================================

        private void YMLParse(object? sender, EventArgs e)
        {
            YMLParser yp = new YMLParser(txtOrigin.Text);
        }

        /// <summary>
        /// btnTranslate Button Event.
        /// txtOrigin의 내용을 번역해서 txtOut에 표현한다.
        /// </summary>
        /// <param name="sender">btnTranslate</param>
        /// <param name="e">ButtonClick Event</param>
        private async void TranslateToKorean(object? sender, EventArgs e)
        {
            string kor = await Papago.TranslateToKorean("Hello, Papago World !!");
            Console.WriteLine(kor);


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
    }
}
