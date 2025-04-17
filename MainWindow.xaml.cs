using ActiproSoftware.Windows.Controls.SyntaxEditor;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lexium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> wordDict = new() { "faiz hebat", "comm suss todd" };
        public ActiproSoftware.Text.TextRange wordrange ;
        public ActiproSoftware.Text.TextRange wordrange2;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SyntaxEditor_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var editorView = syntaxEditor.ActiveView;
            var textSnapshot = editorView.CurrentSnapshot;

            var offset = editorView.Selection.CaretOffset;
            var wordRange = textSnapshot.GetWordTextRange(offset);
            var selection = editorView.Selection;

            if (selection.IsZeroLength)
            {
                return;
            }
            var range1 = wordrange;
            var range2 = wordRange;

            wordrange2 = range2;


            //if (range1.StartOffset < range2.StartOffset)
            //{
            //    var temp = range1;
            //    range1 = range2;
            //    range2 = temp;
            //}
            //else
            //{
            //    var temp = range2;
            //    range2 = range1;
            //    range2 = temp;
            //}
            
            Text2.Text = "Mouse Release"+wordrange2.ToString();

            var combinedRange = ActiproSoftware.Text.TextRange.Union(range1, range2);
            selection.SelectRange(combinedRange);
            wordrange = default;
            wordrange2 = default;
            
        }

        private void syntaxEditor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //delay invocation until ui updated
            Dispatcher.BeginInvoke(new Action(() => {
                var editorView = syntaxEditor.ActiveView;
                var textSnapshot = editorView.CurrentSnapshot;

                var offset = editorView.Selection.CaretOffset;
                var wordRange = textSnapshot.GetWordTextRange(offset);
                wordrange = wordRange;

                editorView.Selection.SelectRange(offset, 0);
                Text1.Text = "Mouse Pressed " + wordrange.ToString();
            }));

        }

       
    }
}