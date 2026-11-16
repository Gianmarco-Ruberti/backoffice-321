using System.Drawing;
using System.Windows.Forms;

namespace backOffice_321.ui
{
    /// <summary>
    /// Palette et application du thème sombre sur l'ensemble des contrôles du formulaire.
    /// </summary>
    public static class Theme
    {
        public static readonly Color Background = Color.FromArgb(15, 20, 26);
        public static readonly Color Surface = Color.FromArgb(23, 30, 39);
        public static readonly Color Input = Color.FromArgb(12, 17, 23);
        public static readonly Color Border = Color.FromArgb(54, 70, 90);
        public static readonly Color Text = Color.FromArgb(230, 237, 243);
        public static readonly Color Muted = Color.FromArgb(147, 165, 184);
        public static readonly Color Accent = Color.FromArgb(88, 166, 255);
        public static readonly Color Open = Color.FromArgb(52, 211, 153);
        public static readonly Color Closed = Color.FromArgb(248, 113, 113);

        public static void Apply(Control root)
        {
            root.BackColor = Background;
            root.ForeColor = Text;
            ApplyChildren(root);
        }

        private static void ApplyChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                ApplyControl(control);

                if (control.HasChildren)
                    ApplyChildren(control);
            }
        }

        private static void ApplyControl(Control control)
        {
            switch (control)
            {
                case GroupBox groupBox:
                    groupBox.BackColor = Surface;
                    groupBox.ForeColor = Muted;
                    break;

                case Button button:
                    button.BackColor = Surface;
                    button.ForeColor = Text;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Border;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 45, 58);
                    button.UseVisualStyleBackColor = false;
                    button.Cursor = Cursors.Hand;
                    break;

                case TextBox textBox:
                    textBox.BackColor = Input;
                    textBox.ForeColor = Text;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case ListBox listBox:
                    listBox.BackColor = Input;
                    listBox.ForeColor = Text;
                    listBox.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case NumericUpDown numeric:
                    numeric.BackColor = Input;
                    numeric.ForeColor = Text;
                    numeric.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case Label label:
                    label.BackColor = Color.Transparent;
                    label.ForeColor = Text;
                    break;

                case PictureBox picture:
                    picture.BackColor = Input;
                    picture.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case CheckBox checkBox:
                    checkBox.BackColor = Color.Transparent;
                    checkBox.ForeColor = Text;
                    break;
            }
        }
    }
}
