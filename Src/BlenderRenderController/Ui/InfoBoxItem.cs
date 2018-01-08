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
    public partial class InfoBoxItem : UserControl
    {
        [Category("Info")]
        public string Title
        {
            get => titleLabel.Text;
            set => titleLabel.Text = value;
        }

        [Category("Info")]
        public string Value
        {
            get => valueLabel.Text;
            set => valueLabel.Text = value;
        }


        public InfoBoxItem()
        {
            InitializeComponent();

            AdjustTitleFont();
        }


        private void OnTitleSizeChanged()
        {
            valueLabel.MinimumSize = titleLabel.PreferredSize;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            AdjustTitleFont();
        }


        private void AdjustTitleFont()
        {
            var tSize = this.Font.SizeInPoints + 1.0f;
            titleLabel.Font = new Font(Font.FontFamily, tSize, GraphicsUnit.Point);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            var minSize = new Size
            {
                Width = titleLabel.Width,
                Height = titleLabel.Height + valueLabel.Height
            };

            this.MinimumSize = minSize;
        }
    }
}
