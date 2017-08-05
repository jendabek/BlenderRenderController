using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController.Ui
{
    public partial class SplitButtonOver : UserControl
    {
        private FlatStyle _flat;
        private FlatButtonAppearance _fba;
        private Color _color;

        [DefaultValue(null), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContextMenuStrip MenuOvr { get; set; }

        public string MainBnt
        {
            get => BrowseBnt.Text;
            set => BrowseBnt.Text = value;
        }
        public string SecondaryBnt
        {
            get => RecentBnt.Text;
            set => RecentBnt.Text = value;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlatButtonAppearance FlatAppearance
        {
            get => BrowseBnt.FlatAppearance;
        }

        public Image Image
        {
            get => BrowseBnt.Image;
            set => BrowseBnt.Image = value;
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment ImageAlign
        {
            get => BrowseBnt.ImageAlign;
            set => BrowseBnt.ImageAlign = value;
        }

        public override Color BackColor
        {
            get => _color;
            set
            {
                _color = value;
                BrowseBnt.BackColor = _color;
                RecentBnt.BackColor = _color;
            }
        }

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                BrowseBnt.Font = Font;
                RecentBnt.Font = Font;
            }
        }

        public FlatStyle FlatStyle
        {
            get { return _flat; }
            set
            {
                _flat = value;
                BrowseBnt.FlatStyle = _flat;
                RecentBnt.FlatStyle = _flat;
            }
        }


        public SplitButtonOver()
        {
            InitializeComponent();

            FlatStyle = BrowseBnt.FlatStyle;
            base.BackColor = Color.Transparent;
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

        }

        public event EventHandler ButtonClicked;

        private void RecentBnt_Click(object sender, EventArgs e)
        {
            if (MenuOvr != null && MenuOvr.Items.Count > 0)
            {
                MenuOvr.Show(this, 0, this.Height);

            }
        }

        private void BrowseBnt_Click(object sender, EventArgs e)
        {
            OnButtonClicked(sender);
        }

        protected void OnButtonClicked(object bnt)
        {
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void SetFlatAppearance()
        {
            BrowseBnt.FlatAppearance.BorderColor =
            RecentBnt.FlatAppearance.BorderColor = _fba.BorderColor;

            BrowseBnt.FlatAppearance.BorderSize =
            RecentBnt.FlatAppearance.BorderSize = _fba.BorderSize;

            BrowseBnt.FlatAppearance.CheckedBackColor =
            RecentBnt.FlatAppearance.CheckedBackColor = _fba.CheckedBackColor;

            BrowseBnt.FlatAppearance.MouseDownBackColor =
            RecentBnt.FlatAppearance.MouseDownBackColor = _fba.MouseDownBackColor;

            BrowseBnt.FlatAppearance.MouseOverBackColor =
            RecentBnt.FlatAppearance.MouseOverBackColor = _fba.MouseOverBackColor;
        }
    }
}
