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

    }
}
