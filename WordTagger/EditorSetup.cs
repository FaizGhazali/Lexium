using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Adornments.Implementation;
using Lexium.Feature;
using Lexium.Feature.SmokeEffect;
using Lexium.Helper;

namespace Lexium.WordTagger
{
    public class EditorSetup : SyntaxLanguage
    {
        public EditorSetup() : base ("WordTagger")
        {
            //SyntaxEditorHelper.InitializeLanguageFromResourceStream(this, "Lexium.langdef");
            //RegisterService(new AdornmentManagerProvider<SmokeTextAdornmentManager>(typeof(SmokeTextAdornmentManager)));



            InitializationSetup.InitializeLanguageFromResourceStream(this, "Lexium.langdef");
            RegisterService(new TextViewTaggerProvider<WordHighlight>(typeof(WordHighlight)));
        }
    }
}
