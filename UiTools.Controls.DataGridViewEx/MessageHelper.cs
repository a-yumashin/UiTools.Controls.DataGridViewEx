using System.Linq;
using System.Windows.Forms;
using static UiTools.Controls.ExtendedDataGridView.CommonStuff;

namespace UiTools.Controls.ExtendedDataGridView
{
    internal static class MessageHelper
    {
        private static Form GetOwnerForm()
        {
            var activeForm = Form.ActiveForm;
            if (activeForm == null)
            {
                // call from IDE
                return Application.OpenForms.Cast<Form>().FirstOrDefault();
            }
            if (activeForm.IsMdiContainer)
            {
                // call from MDI child (since MDI container has no any calls)
                return activeForm.ActiveMdiChild;
            }
            else
            {
                // call NOT from MDI child - i.e. from some modal dialog (shown from some MDI child)
                return activeForm;
            }
        }

        public static void ShowError(string text, string caption = "Error")
        {
            MessageBox.Show(GetOwnerForm(), text, caption == "Error" ? SR("Error") : caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInfo(string text, string caption = "Information")
        {
            MessageBox.Show(GetOwnerForm(), text, caption == "Information" ? SR("Information") : caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowExclamation(string text, string caption = "Attention")
        {
            MessageBox.Show(GetOwnerForm(), text, caption == "Attention" ? SR("Attention") : caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static bool ShowYesNo(string text, string caption = "", bool defaultYes = true)
        {
            return DialogResult.Yes == MessageBox.Show(
                GetOwnerForm(), text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                defaultYes ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2);
        }
    }
}
