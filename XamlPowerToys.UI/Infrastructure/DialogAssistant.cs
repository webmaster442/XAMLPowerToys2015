namespace XamlPowerToys.UI.Infrastructure {
    using System;
    using System.Windows;

    /// <summary>
    /// Provides wrapper for MessageBox dialog
    /// </summary>
    public static class DialogAssistant {

        /// <summary>
        /// Shows the confirmation message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <returns><c>MessageBoxResult.Yes</c> if confirmed; otherwise, <c>MessageBoxResult.No</c></returns>
        public static MessageBoxResult ShowConfirmationMessage(String message, String caption = "Are you sure?") {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        }

        /// <summary>
        /// Shows the exception message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="additionalMessageText">The additional message text.</param>
        /// <remarks>If in DEBUG build configuration, dialog will show exception to string; otherwise, the exception message will be shown.</remarks>
        public static void ShowExceptionMessage(Exception ex, String caption = "Error", String additionalMessageText = "") {
            String message;
#if DEBUG
            if (String.IsNullOrWhiteSpace(additionalMessageText)) {
                message = ex.ToString();
            } else {
                message = additionalMessageText + Environment.NewLine + Environment.NewLine + ex;
            }
#else
           if (String.IsNullOrWhiteSpace(additionalMessageText)) {
               message = ex.Message;
           } else {
               message = additionalMessageText + Environment.NewLine + Environment.NewLine + ex.Message;
           }
#endif
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        /// <summary>
        /// Shows the information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        public static void ShowInformationMessage(String message, String caption) {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

    }
}
