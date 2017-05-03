using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace James.HelperClasses
{
    public static class MetroDialogHelper
    {
        public static Task<MessageDialogResult> ShowDialog(UserControl uc, string title, string question)
        {
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            return uc.GetWindow().ShowMessageAsync(title, question, MessageDialogStyle.AffirmativeAndNegative, setting);
        }

        public static void OnSuccess(this MessageDialogResult uc, Action action)
        {
            if (uc == MessageDialogResult.Affirmative)
            {
                action.Invoke();
            }
        }
    }
}
