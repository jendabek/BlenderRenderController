using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BlenderRenderController.ui
{
    public interface IPlatformAjusts
    {
        // add methods to bulk change clode
        void Ui_Main();

        //....

        // do all of the above
        void AdjustAll();
    }

    public class PlatformService : IPlatformAjusts
    {
        private IList<IPlatformAjusts> _platforms = new List<IPlatformAjusts>();
        

        public IList<IPlatformAjusts> GetPlatforms()
        {
            // ... add platforms here
            _platforms.Add(new Unix());

            return _platforms;
        }

        public void AdjustAll()
        {
            return;
        }

        public void Ui_Main()
        {
            throw new NotImplementedException();
        }
    }

    public class Unix : IPlatformAjusts
    {
        public void AdjustAll()
        {
            throw new NotImplementedException();
        }

        public void Ui_Main()
        {
            
        }
    }

}
