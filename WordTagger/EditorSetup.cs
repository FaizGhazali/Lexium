using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Adornments.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.IntelliPrompt.Implementation;
using Lexium.Feature;
using Lexium.Feature.SmokeEffect;
using Lexium.Helper;



namespace Lexium.WordTagger
{
    public class EditorSetup : SyntaxLanguage
    {
        public EditorSetup() : base ("CustomDecorator")
        {
            //SyntaxEditorHelper.InitializeLanguageFromResourceStream(this, "Lexium.langdef");
            //RegisterService(new AdornmentManagerProvider<SmokeTextAdornmentManager>(typeof(SmokeTextAdornmentManager)));

            string path = @"CustomLanguage\Lexium.langdef";

            InitializationSetup.InitializeLanguageFromResourceStream(this, path);
            RegisterService(new TextViewTaggerProvider<WordHighlight>(typeof(WordHighlight)));

            // Register the word highlight tagger
            //RegisterService(new CodeDocumentTaggerProvider<CustomSquiggleTagger>(typeof(CustomSquiggleTagger)));
            //RegisterService(new SquiggleTagQuickInfoProvider());
        }
    }
}
