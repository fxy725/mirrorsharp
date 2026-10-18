using System;
using Microsoft.CodeAnalysis.Text;

namespace MirrorSharp.Internal.Handlers.Shared {
    internal static class CompletionTextChange {
        public static TextChange Normalize(TextChange textChange, TextSpan completionSpan, int cursorPosition, string fallbackText) {
            // Roslyn uses an empty change at (0, 0) when a completion provider
            // fails. Expanding that sentinel to the cursor deletes all code
            // before it. Fall back to the advertised item within its own span.
            if (textChange.Span == default && string.IsNullOrEmpty(textChange.NewText))
                textChange = new TextChange(completionSpan, fallbackText);

            if (cursorPosition <= completionSpan.Start)
                return textChange;

            var span = textChange.Span;
            var newStart = Math.Min(span.Start, completionSpan.Start);
            var newLength = Math.Max(span.End, cursorPosition) - newStart;
            return new TextChange(new TextSpan(newStart, newLength), textChange.NewText!);
        }
    }
}
