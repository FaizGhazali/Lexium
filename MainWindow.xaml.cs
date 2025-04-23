using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Lexing;
using ActiproSoftware.Text.Lexing.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using HelperDll;
using Lexium.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;


namespace Lexium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> wordList = new() { "test", "test one two three"};
        public Dictionary<string, string> wordObj = new();
        private MainWindowVM _viewModel = new MainWindowVM();

        public MainWindow()
        {
            InitializeComponent();
            syntaxEditor.Document.Language = new Lexium.WordTagger.EditorSetup();

            DataContext = _viewModel;
        }

        private void syntaxEditor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string clickedWord = PhraseSelector.HandleMouseDown(syntaxEditor, _viewModel.Words);

                _viewModel.GetDefination(clickedWord);
            }));
        }

        private void DefineBtn_Clicked(object sender, RoutedEventArgs e)
        {
            string defineText = DefineTextBox.Text;
            var editorView = syntaxEditor.ActiveView;
            string selectedText = editorView.SelectedText;

            _viewModel.AddWord(selectedText, defineText);
        }
        void AddKeywordAndRefresh(string keyword)
        {
            var lexer = syntaxEditor.Document.Language.GetService<ILexer>() as DynamicLexer;
            if (lexer == null)
                return;

            var defaultState = lexer.DefaultLexicalState;
            var keywordGroup = defaultState.LexicalPatternGroups["Keyword"];
            if (keywordGroup == null)
                return;

            using (lexer.CreateChangeBatch())
            {
                // Avoid duplicates
                
            }

            // 👇 Manual reparse workaround: change document text to force SyntaxEditor to re-tokenize
            var snapshot = syntaxEditor.Document.CurrentSnapshot;
            var text = snapshot.Text;

            // Add and remove a whitespace at the end to simulate edit
           
        }

        private void RefreshLanguage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddKeywordAndRefresh("ooo");
                // Create a new language instance (which reloads the langdef)
                var newLanguage = new Lexium.WordTagger.EditorSetup();

                // Set it to the editor's document
                syntaxEditor.Document.Language = newLanguage;

                var filePath = @"CustomLanguage\Lexium.langdef"; // Adjust to your actual path
                var serializer = new SyntaxLanguageDefinitionSerializer()
                {
                    UseBuiltInClassificiationTypes = true
                };
                var language = serializer.LoadFromFile(filePath);
                syntaxEditor.Document.Language = language;


                AddKeywordAndRefresh("kooo");


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reload language: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RefreshLanguage_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new language instance (which reloads the langdef)
                var newLanguage = new Lexium.WordTagger.EditorSetup();

                // Set it to the editor's document
                syntaxEditor.Document.Language = newLanguage;

                var filePath = @"CustomLanguage\Lexium2.langdef"; // Adjust to your actual path
                var serializer = new SyntaxLanguageDefinitionSerializer()
                {
                    UseBuiltInClassificiationTypes = true
                };
                var language = serializer.LoadFromFile(filePath);
                syntaxEditor.Document.Language = language;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reload language: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}