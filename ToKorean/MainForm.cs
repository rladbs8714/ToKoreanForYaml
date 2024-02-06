using System.Text;
using ToKorean.IO;
using ToKorean.Papago;

namespace ToKorean
{
    public partial class MainForm : Form
    {

        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        public MainForm()
        {
            InitializeComponent();

            // Event
            btnTranslate.Click += TranslateToKorean;

        }

        // ==============================================================================
        // METHOD
        // ==============================================================================



        // ==============================================================================
        // EVENT METHOD
        // ==============================================================================


        /// <summary>
        /// btnTranslate Button Event.
        /// txtOrigin�� ���� �����ؼ� txtOut���� �ѱ��.
        /// </summary>
        /// <param name="sender">btnTranslate</param>
        /// <param name="e">ButtonClick Event</param>
        private async void TranslateToKorean(object? sender, EventArgs e)
        {
            // ����

            string kor = await PapagoHelper.Instance.TranslateToKorean("Hello, Papago World !!");
            Console.WriteLine(kor);
        }

        private void GetYmlFile(object? sender, EventArgs e)
        {
            string ymlPath = new FileDialogHelper(".yml|*.yml").GetFilePath();

            if (string.IsNullOrEmpty(ymlPath) ||
                !File.Exists(ymlPath))
            {
                Console.WriteLine("yml파일을 찾을 수 없습니다.");
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
