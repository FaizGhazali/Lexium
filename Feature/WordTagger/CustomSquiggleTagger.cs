using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Tagging;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.IntelliPrompt.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Lexium.MVVM.ViewModel;
using System.Linq;

namespace Lexium.Feature
{
    public class CustomSquiggleTagger : CollectionTagger<ISquiggleTag>
    {
        private HashSet<string> _userDefinedWords;
        private MainWindowVM _treeViewVM;

        // Constructor that accepts ICodeDocument
        public CustomSquiggleTagger(ICodeDocument document)
            : base("CustomSquiggle", null, document, true)
        {

            if (App.ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized.");
            }
            else
            {
                
                _treeViewVM = App.ServiceProvider.GetRequiredService<MainWindowVM>();
                

            }
            // Initialize the user-defined words
            //InitializeUserDefinedWords();

            // Subscribe to the document's text changed event for real-time updates
            document.TextChanged += OnDocumentTextChanged;
            ScanForTags();
        }



        static CustomSquiggleTagger()
        {
            AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Warning, new HighlightingStyle(Colors.BlueViolet));
        }

        private void OnDocumentTextChanged(object sender, TextSnapshotChangedEventArgs e)
        {
            // Rescan for tags when the document's text changes
            

            ScanForTags();
        }

        private void ScanForTags()
        {
            using (var batch = CreateBatch())
            {
                Clear();
                var snapshot = Document.CurrentSnapshot;
                var text = snapshot.GetText(LineTerminator.Newline);

                //dict to handle tag
                //ny key then tag will be value
                var userDefinedPhrases = _treeViewVM.WordObj;

                HashSet<int> excludedIndexes = new HashSet<int>();
                

                foreach (var phrase in userDefinedPhrases)
                {
                    var phraseLower = phrase.Key.ToLower();
                    var phraseMatches = Regex.Matches(text, Regex.Escape(phraseLower), RegexOptions.IgnoreCase);
                    

                    foreach (Match match in phraseMatches)
                    {
                        for (int i = match.Index; i < match.Index + match.Length; i++)
                        {
                            excludedIndexes.Add(i); // Mark all indexes of this phrase to be skipped
                        }

                        var snapshotRange = new TextSnapshotRange(snapshot, TextRange.FromSpan(match.Index, match.Length));
                        var versionRange = snapshotRange.ToVersionRange(TextRangeTrackingModes.DeleteWhenZeroLength);

                        var tag = new SquiggleTag
                        {
                            ClassificationType = ClassificationTypes.Comment,
                            ContentProvider = new PlainTextContentProvider(phrase.Value)
                        };
                        Add(new TagVersionRange<ISquiggleTag>(versionRange, tag));
                    }
                }

                // Now handle single words
                var matches = Regex.Matches(text, @"\b\w+\b", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    string word = match.Value.ToLower();

                    // Skip if this word is part of a recognized multi-word phrase
                    if (excludedIndexes.Contains(match.Index))
                        continue;

                    if (!userDefinedPhrases.Keys.Contains(word))
                    {
                        var snapshotRange = new TextSnapshotRange(snapshot, TextRange.FromSpan(match.Index, match.Length));
                        var versionRange = snapshotRange.ToVersionRange(TextRangeTrackingModes.DeleteWhenZeroLength);

                        var tag = new SquiggleTag
                        {
                            ClassificationType = ClassificationTypes.Warning,
                            ContentProvider = new PlainTextContentProvider($"Unknown word: {word}")
                        };
                        Add(new TagVersionRange<ISquiggleTag>(versionRange, tag));
                    }
                }
            }
        }
        private void ScanForTags999()
        {
            using (var batch = CreateBatch())
            {
                // Clear existing tags
                Clear();

                var snapshot = Document.CurrentSnapshot;
                var text = snapshot.GetText(LineTerminator.Newline);

                var userDefinedPhrases = _userDefinedWords.Where(word => word.Contains(" ")).ToList();
                var userDefinedWords = _userDefinedWords.Where(word => !word.Contains(" ")).ToList();

                // Track indexes already matched by multi-word phrases
                HashSet<int> excludedIndexes = new HashSet<int>();

                // First, handle multi-word phrases
                foreach (var phrase in userDefinedPhrases)
                {
                    var phraseLower = phrase.ToLower();
                    var phraseMatches = Regex.Matches(text, Regex.Escape(phraseLower), RegexOptions.IgnoreCase);

                    foreach (Match match in phraseMatches)
                    {
                        for (int i = match.Index; i < match.Index + match.Length; i++)
                        {
                            excludedIndexes.Add(i); // Mark all indexes of this phrase to be skipped
                        }

                        var snapshotRange = new TextSnapshotRange(snapshot, TextRange.FromSpan(match.Index, match.Length));
                        var versionRange = snapshotRange.ToVersionRange(TextRangeTrackingModes.DeleteWhenZeroLength);

                        var tag = new SquiggleTag
                        {
                            ClassificationType = ClassificationTypes.Comment,
                            ContentProvider = new PlainTextContentProvider($"faiz")
                        };
                        Add(new TagVersionRange<ISquiggleTag>(versionRange, tag));
                    }
                }

                // Now handle single words
                var matches = Regex.Matches(text, @"\b\w+\b", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    string word = match.Value.ToLower();

                    // Skip if this word is part of a recognized multi-word phrase
                    if (excludedIndexes.Contains(match.Index))
                        continue;

                    if (!userDefinedWords.Contains(word))
                    {
                        var snapshotRange = new TextSnapshotRange(snapshot, TextRange.FromSpan(match.Index, match.Length));
                        var versionRange = snapshotRange.ToVersionRange(TextRangeTrackingModes.DeleteWhenZeroLength);

                        var tag = new SquiggleTag
                        {
                            ClassificationType = ClassificationTypes.Warning,
                            ContentProvider = new PlainTextContentProvider($"Unknown word: {word}")
                        };
                        Add(new TagVersionRange<ISquiggleTag>(versionRange, tag));
                    }
                }
            }
        }


        
        public void AddWordToDictionary(string word)
        {
            _userDefinedWords.Add(word.ToLower());
            ScanForTags();
        }
        public void ProcessSelectedText(string selectedText)
        {
            // Process the selected text here
            Debug.WriteLine("Processing selected text: " + selectedText);

            // For example, add it to the user-defined words
            AddWordToDictionary(selectedText);
        }
    }
}
