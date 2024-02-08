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

            string Document = @"---
            receipt:    Oz-Ware Purchase Invoice
            date:        2007-08-06
            customer:
                given:   Dorothy
                family:  Gale

            items:
                - part_no:   A4786
                  descrip:   Water Bucket (Filled)
                  price:     1.47
                  quantity:  4

                - part_no:   E1628
                  descrip:   High Heeled ""Ruby"" Slippers
                  price:     100.27
                  quantity:  1

            bill-to:  &id001
                street: |
                        123 Tornado Alley
                        Suite 16
                city:   East Westville
                state:  KS

            ship-to:  *id001

            specialDelivery:  >
                Follow the Yellow Brick
                Road to the Emerald City.
                Pay no attention to the
                man behind the curtain.
...";

            
            // Setup the input
            var input = new StringReader(Document);

            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(input);

            // Examine the stream
            var mapping =
                (YamlMappingNode)yaml.Documents[0].RootNode;

            foreach (var entry in mapping.Children)
            {
                Console.WriteLine(((YamlScalarNode)entry.Key).Value);
            }

            // List all the items
            var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("items")];
            foreach (YamlMappingNode item in items)
            {
                Console.WriteLine(
                    "{0}\t{1}",
                    item.Children[new YamlScalarNode("part_no")],
                    item.Children[new YamlScalarNode("descrip")]
                );
            }
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
            

            /*
            Dictionary<string, object> content = yp.Data;

            if (content == null)
            {
                Console.WriteLine("제대로 파싱이 되지 않았거나, Yml파일이 잘못됬습니다.");
                return;
            }

            Console.WriteLine("==============================");

            foreach (var c in content)
            {
                Console.WriteLine(c.Key + " : " + c.Value + " : " + yp.Tag2Index(c.Value.ToString()));
            }
            */
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
