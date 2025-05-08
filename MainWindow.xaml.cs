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
using Microsoft.Extensions.DependencyInjection;



namespace Lexium
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IServiceProvider _serviceProvider { get; set; }
        public List<string> wordList = new() { "test", "test one two three"};
        public Dictionary<string, string> wordObj = new();
        private MainWindowVM _viewModel ;
        private Lexium.WordTagger.EditorSetup language = new WordTagger.EditorSetup();

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            // syntaxEditor.Document.Language = new Lexium.WordTagger.EditorSetup();

            _viewModel = _serviceProvider.GetRequiredService<MainWindowVM>();
            DataContext = _viewModel;
           
            
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

            string tokenKey = HelperFunction.GetTokenKeyFromPhrase(selectedText);

            Debug.WriteLine(selectedText);
            _viewModel.AddWord(selectedText, defineText);

            var helper = new LangDefUtils();
            helper.AddKeywordToLangDef(@"CustomLanguage\Lexium.langdef", selectedText, tokenKey);

        }
        

        private void RefreshLanguage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
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