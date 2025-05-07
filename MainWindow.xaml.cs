using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Lexing;
using ActiproSoftware.Text.Lexing.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using HelperDll;
using Lexium.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;
using Lexium.Helper;
using ActiproSoftware.Windows.Themes;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting;
using System.IO;
using System.Diagnostics;



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
        private Lexium.WordTagger.EditorSetup language = new WordTagger.EditorSetup();

        public MainWindow()
        {
            InitializeComponent();
           // syntaxEditor.Document.Language = new Lexium.WordTagger.EditorSetup();

            DataContext = _viewModel;
            //var filePath = @"CustomLanguage\Lexium.langdef"; // Adjust to your actual path
            //var serializer = new SyntaxLanguageDefinitionSerializer()
            //{
            //    UseBuiltInClassificiationTypes = true
            //};
            //var language = serializer.LoadFromFile(filePath);
            //syntaxEditor.Document.Language = language;

            
            language.LoadFromLangdefFile(@"CustomLanguage\Lexium.langdef");
            

            syntaxEditor.Document.Language = language;
            syntaxEditor.Document = (IEditorDocument)document;
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
            Debug.WriteLine(selectedText);
            _viewModel.AddWord(selectedText, defineText);

            var helper = new LangDefUtils();
            helper.AddKeywordToLangDef(@"CustomLanguage\Lexium.langdef", selectedText);
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
                //AddKeywordAndRefresh("ooo");
                // Create a new language instance (which reloads the langdef)
                //var newLanguage = syntaxEditor.Document.Language;

                //// Set it to the editor's document
                //syntaxEditor.Document.Language = newLanguage;

                //var filePath = @"CustomLanguage\Lexium.langdef"; // Adjust to your actual path
                //var serializer = new SyntaxLanguageDefinitionSerializer()
                //{
                //    UseBuiltInClassificiationTypes = true
                //};
                //var language = serializer.LoadFromFile(filePath);
                //syntaxEditor.Document.Language = language;
                //var helper = new LangDefUtils();
                //helper.AddKeywordToLangDef(@"CustomLanguage\Lexium.langdef", "TAK BEREH");
                //AddKeywordAndRefresh("kooo");

                
                language.LoadFromLangdefFile(@"CustomLanguage\Lexium.langdef");


                syntaxEditor.Document.Language = language;
                syntaxEditor.Document = (IEditorDocument)document;


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

                var filePath = @"CustomLanguage\CSharp.langdef"; // Adjust to your actual path
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