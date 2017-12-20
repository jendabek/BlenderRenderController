using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace BlenderRenderController.Infra
{
    public class RecentBlendsCollection : ObservableCollection<string>
    {
        uint _capacity = 10;

        public int MaxElements
        {
            get => (int)_capacity;
            set => _capacity = (uint)value;
        }



        protected override void InsertItem(int index, string item)
        {
            // check if item is already present
            if (Contains(item))
            {
                index = IndexOf(item);
                MoveItem(index, 0);
                return;
            }

            // remove last if capacity is reached
            if (index == MaxElements)
            {
                Items.RemoveAt(MaxElements - 1);
            }

            // elements are inserted at the front
            base.InsertItem(0, item);
        }

    }
}
