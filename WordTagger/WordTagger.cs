using ActiproSoftware.Text;
using ActiproSoftware.Text.Tagging;
using ActiproSoftware.Text.Tagging.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexium.WordTagger
{
    public class WordTagger : CollectionTagger<ISquiggleTag>
    {
        // Constructor that accepts ICodeDocument
        public WordTagger(ICodeDocument document)
            : base("WordTagger",null, document,true)
        {
            document.TextChanged += OnDocumentTextChanged;

        }
        #region Event Handlers
        private void OnDocumentTextChanged(object? sender, TextSnapshotChangedEventArgs e)
        {
            // Rescan for tags when the document's text changes


            //ScanForTags();
        }
        #endregion
    }
}
