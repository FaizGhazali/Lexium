using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Tagging;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor.IntelliPrompt.Implementation;

namespace Lexium.Feature
{
    public class CustomSquiggleTagger : CollectionTagger<ISquiggleTag>
    {
        private HashSet<string> _userDefinedWords;

        // Constructor that accepts ICodeDocument
        public CustomSquiggleTagger(ICodeDocument document)
            : base("CustomSquiggle", null, document, true)
        {
            // Initialize the user-defined words
            InitializeUserDefinedWords();

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


        private void InitializeUserDefinedWords()
        {
            // Define your custom words
            string input = "tak mudah namun tak senang, terserah atas pilihan, terlalu banyak alasannya, duduk terdiam";

            // Split the input string into words and clean punctuation
            char[] delimiters = new char[] { ' ', ',', '.' };
            string[] words = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            // Create a HashSet to store unique words
            _userDefinedWords = new HashSet<string>();
            foreach (string word in words)
            {
                _userDefinedWords.Add(word.ToLower()); // Convert to lowercase to ensure uniqueness
            }

            // Optional: Debug output to verify unique words
            Debug.WriteLine("Unique user-defined words:");
            foreach (string word in _userDefinedWords)
            {
                Debug.WriteLine(word);
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
