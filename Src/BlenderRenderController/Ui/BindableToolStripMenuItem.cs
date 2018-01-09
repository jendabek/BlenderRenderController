using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace BlenderRenderController.Ui
{
    public class BindableToolStripMenuItem : ToolStripMenuItem, IBindableComponent
    {
        ControlBindingsCollection _bindCollection;
        BindingContext _bindContext;


        public BindableToolStripMenuItem()
            : base() { }

        public BindableToolStripMenuItem(string text) 
            : base(text) { }

        public BindableToolStripMenuItem(Image image) 
            : base(image) { }

        public BindableToolStripMenuItem(string text, Image image) 
            : base(text, image) { }

        public BindableToolStripMenuItem(string text, Image image, params ToolStripItem[] dropDownItems) 
            : base(text, image, dropDownItems) { }

        public BindableToolStripMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys) 
            : base(text, image, onClick, shortcutKeys) { }

        public BindableToolStripMenuItem(string text, Image image, EventHandler onClick)
            : base(text, image, onClick) { }

        public BindableToolStripMenuItem(string text, Image image, EventHandler onClick, string name)
            : base(text, image, onClick, name) { }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (_bindCollection == null)
                {
                    _bindCollection = new ControlBindingsCollection(this);
                }

                return _bindCollection;
            }
        }

        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (_bindContext == null)
                {
                    _bindContext = new BindingContext();
                }

                return _bindContext;
            }

            set => _bindContext = value;
        }
    }
}
