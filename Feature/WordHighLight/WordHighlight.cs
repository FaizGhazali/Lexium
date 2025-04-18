﻿using System;
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
    public class WordHighlight : TaggerBase<IClassificationTag>
    {
        private string currentWord = String.Empty;
        private IEditorView view;

        private static readonly Regex wordCheck = new Regex(@"[A-Za-z_]\w*", RegexOptions.Compiled);
        private static readonly IClassificationType wordHighlightClassificationType = new ClassificationType("WordHighlight", "Word Highlight");

        static WordHighlight()
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

        public WordHighlight(IEditorView view) : base("Custom",
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

            // Save the old current word
            string oldCurrentWord = currentWord;

            // Get the current word and ensure it has only letter or number characters
            currentWord = view.Selection.Length == 0
                ? view.GetCurrentWordText().Trim()
                : view.SelectedText;
            Match match = wordCheck.Match(currentWord);
            if ((match == null) || (match.Index != 0) || (match.Length != currentWord.Length))
                currentWord = String.Empty;

            // If the current word changed...
            if (oldCurrentWord != currentWord)
            {
                // Notify that tags changed
                // NOTE: You generally want to minimize the range passed to TagsChanged events, but in this case we don't know beforehand where word matches are made throughout the document
                this.OnTagsChanged(new TagsChangedEventArgs(new TextSnapshotRange(view.SyntaxEditor.Document.CurrentSnapshot, view.SyntaxEditor.Document.CurrentSnapshot.TextRange)));
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