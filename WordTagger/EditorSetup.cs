using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Adornments.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.IntelliPrompt.Implementation;
using Lexium.Feature;
using Lexium.Feature.SmokeEffect;
using Lexium.Helper;
using System.IO;



namespace Lexium.WordTagger
{
    public class EditorSetup : SyntaxLanguage
    {
        public EditorSetup() : base ("CustomDecorator")
        {
            RegisterServices();
        }
        private void RegisterServices()
        {
            RegisterService(new TextViewTaggerProvider<WordHighlight>(typeof(WordHighlight)));
           
            RegisterService(new AdornmentManagerProvider<SmokeTextAdornmentManager>(typeof(SmokeTextAdornmentManager)));

            RegisterService(new CodeDocumentTaggerProvider<CustomSquiggleTagger>(typeof(CustomSquiggleTagger)));
            RegisterService(new SquiggleTagQuickInfoProvider());
        }
        public void LoadFromLangdefFile(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var serializer = new SyntaxLanguageDefinitionSerializer
            {
                UseBuiltInClassificiationTypes = true
            };
            serializer.InitializeFromStream(this, stream);
        }
    }
}
