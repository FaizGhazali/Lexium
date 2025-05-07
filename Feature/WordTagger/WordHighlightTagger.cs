using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Text.Tagging;
using ActiproSoftware.Text.Tagging.Implementation;
using ActiproSoftware.Text.Utility;
using ActiproSoftware.Windows.Controls.Rendering;
using ActiproSoftware.Windows.Controls.SyntaxEditor;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting.Implementation;
using ActiproSoftware.Windows.Media;

namespace Lexium.Feature
{
    public class WordHighlightTagger : TaggerBase<IClassificationTag>
    {
        private string currentWord = String.Empty;
        private IEditorView view;

        private static readonly Regex wordCheck = new Regex(@"[A-Za-z_]\w*", RegexOptions.Compiled);
        private static readonly IClassificationType wordHighlightClassificationType = new ClassificationType("WordHighlight", "Word Highlight");

        private readonly List<string> dictionaryPhrases = new List<string> { "word faiz" };

        static WordHighlightTagger()
        {
            // This sample assumes the editor will use the AmbientHighlightingStyleRegistry
            var registry = AmbientHighlightingStyleRegistry.Instance;

            // Configure light/dark color palettes with default colors
            var key = wordHighlightClassificationType.Key;
            registry.LightColorPalette?.SetBackground(key, UIColor.FromWebColor("#40c0c0c0"));
            registry.LightColorPalette?.SetBorder(key, UIColor.FromWebColor("#c0c0c0"));
            registry.DarkColorPalette?.SetBackground(key, UIColor.FromWebColor("#40717171"));
            registry.DarkColorPalette?.SetBorder(key, UIColor.FromWebColor("#717171"));

            // Define a style with a border
            var style = new HighlightingStyle()
            {
                BorderCornerKind = HighlightingStyleBorderCornerKind.Rounded,
                BorderKind = LineKind.Solid,
                IsBorderEditable = true,
                IsForegroundEditable = false,
            };

            // Associate the style with the classification type
            // and the current color palette color will be automatically applied
            registry.Register(wordHighlightClassificationType, style);
        }

        public WordHighlightTagger(IEditorView view) : base("Custom",
            new Ordering[] { new Ordering(TaggerKeys.Token, OrderPlacement.Before) }, view.SyntaxEditor.Document)
        {

            // Initialize
            this.view = view;
            this.view.SelectionChanged += new EventHandler<EditorViewSelectionEventArgs>(OnViewSelectionChanged);

            // Update current word
            this.UpdateCurrentWord();
        }
        private void OnViewSelectionChanged(object sender, EditorViewSelectionEventArgs e)
        {
            if (view == null)
                return;

            // Update the current word
            this.UpdateCurrentWord();
        }

        private void UpdateCurrentWord()
        {
            if ((view == null) || (view.Selection == null))
                return;

            // Save old
            string old = currentWord;
            string selected = view.Selection.Length == 0
                ? view.GetCurrentWordText().Trim()
                : view.SelectedText.Trim();

            // Check if inside dictionary phrase
            string text = view.SyntaxEditor.Document.CurrentSnapshot.Text;
            currentWord = dictionaryPhrases.FirstOrDefault(phrase =>
                Regex.IsMatch(text, $@"\b{Regex.Escape(phrase)}\b", RegexOptions.IgnoreCase) &&
                phrase.Contains(selected, StringComparison.OrdinalIgnoreCase)
            );

            // If not found in dictionary, fall back to selection word
            if (string.IsNullOrEmpty(currentWord))
            {
                Match match = wordCheck.Match(selected);
                if ((match != null) && (match.Index == 0) && (match.Length == selected.Length))
                    currentWord = selected;
                else
                    currentWord = string.Empty;
            }

            if (old != currentWord)
            {
                this.OnTagsChanged(new TagsChangedEventArgs(
                    new TextSnapshotRange(view.SyntaxEditor.Document.CurrentSnapshot, view.SyntaxEditor.Document.CurrentSnapshot.TextRange)
                ));
            }
        }


        public override IEnumerable<TagSnapshotRange<IClassificationTag>> GetTags(NormalizedTextSnapshotRangeCollection snapshotRanges, object parameter)
        {
            if (String.IsNullOrEmpty(currentWord))
                yield break;

            // Get a regex of the current word
            Regex search = new Regex(String.Format(@"\b{0}\b", currentWord), RegexOptions.Singleline);

            // Loop through the requested snapshot ranges...
            foreach (TextSnapshotRange snapshotRange in snapshotRanges)
            {
                // If the snapshot range is not zero-length...
                if (!snapshotRange.IsZeroLength)
                {
                    // Look for current word matches
                    foreach (Match match in search.Matches(snapshotRange.Text))
                    {
                        // Add a highlighted range
                        yield return new TagSnapshotRange<IClassificationTag>(
                            new TextSnapshotRange(snapshotRange.Snapshot, TextRange.FromSpan(snapshotRange.StartOffset + match.Index, match.Length)),
                            new ClassificationTag(wordHighlightClassificationType)
                            );
                    }
                }
            }
        }

        protected override void OnClosed()
        {
            // Detach from the view
            if (view != null)
            {
                view.SelectionChanged -= new EventHandler<EditorViewSelectionEventArgs>(OnViewSelectionChanged);
                view = null;
            }

            // Call the base method
            base.OnClosed();
        }
    }
}
