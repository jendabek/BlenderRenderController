using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController.ui.linux
{
    public partial class SplitButtonOver : UserControl
    {

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
        public Image MainImg
        {
            get => BrowseBnt.Image;
            set => BrowseBnt.Image = value;
        }


        public SplitButtonOver()
        {
            InitializeComponent();
            RecentBnt.FlatStyle =
            BrowseBnt.FlatStyle = FlatStyle.Flat;

            BrowseBnt.BackColor =
            RecentBnt.BackColor = Color.FromArgb(224, 224, 224);
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
    }
}
