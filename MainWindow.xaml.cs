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

                var clickedWord = textSnapshot.Text.Substring(wordRange.StartOffset, wordRange.Length);

                var matchedPhrase = wordDict.FirstOrDefault(phrase =>
            phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(clickedWord));

                if (string.IsNullOrEmpty(matchedPhrase))
                {
                    Text3.Text = "No matching phrase found.";
                    return;
                }

                var fullText = textSnapshot.Text;
                var index = fullText.IndexOf(matchedPhrase, StringComparison.OrdinalIgnoreCase);

                if (index >= 0)
                {
                    // Select the full phrase
                    editorView.Selection.SelectRange(index, matchedPhrase.Length);
                    Text3.Text = $"Phrase found and selected: \"{matchedPhrase}\"";
                    
                }
                else
                {
                    Text3.Text = $"Phrase \"{matchedPhrase}\" not found in document.";
                }
                if (index >= 0)
                {
                    // Get length of first word
                    var firstWord = matchedPhrase.Split(' ', StringSplitOptions.RemoveEmptyEntries).First();
                    int firstWordOffset = index;
                    int firstWordLength = firstWord.Length;

                    // Set your wordrange
                    wordrange = new ActiproSoftware.Text.TextRange(firstWordOffset, firstWordOffset+firstWordLength);

                    // (Optional) Select the full phrase in editor
                    //editorView.Selection.SelectRange(index, matchedPhrase.Length);

                    // Text1.Text = $"Phrase found: \"{matchedPhrase}\" | First word: \"{firstWord}\" at {wordrange}";
                    Text1.Text = "Mouse Pressed " + wordrange.ToString();
                }

            }));

        }

       
    }
}