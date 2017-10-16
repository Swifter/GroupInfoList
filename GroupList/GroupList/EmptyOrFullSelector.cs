using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation.Collections;
using GroupList.Model;

namespace GroupList.GroupList
{
	/// <summary>
	/// This object determines whether or not the Group passed during binding is empty or not and allows selection
	/// of the proper DataTemplate in the SemanticZoom control's ZoomedOutView based on whether or not the GroupInfoList
	/// currently being bound contains Contact members or not.  An instance of this object is declared in XAML of the UserControl's
	/// Resource section.  See https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.DataTemplateSelector for
	/// complete details.
	/// </summary>
	class GroupEmptyOrFullSelector : DataTemplateSelector
    {
        private DataTemplate _full;
        private DataTemplate _empty;

		/// <summary>
		/// The DataTemplate to use when a Group has members.  The value of this property is set by XAML in the UserControl resources.
		/// </summary>
        public DataTemplate Full
        {
            set { _full = value; }
            get { return _full; }
        }

		/// <summary>
		/// The DataTemplate to use when a Group has no members. The value of this property is set by XAML in the UserControl resources.
		/// </summary>
		public DataTemplate Empty
        {
            set { _empty = value; }
            get { return _empty; }
        }

		/// <summary>
		/// This override contains the logic used to decide which DataTemplate to use when binding the Key of a GroupInfoList
		/// in the SemanticZoom control's ZoomedOutView, which displays the index to the Contact Groups.  This function will 
		/// be called for each GroupInfoList object during binding.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="container"></param>
		/// <returns>A DataTemplate object to use when binding.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
			// get the Type of the object calling us
            var itemType = item.GetType();

			// we only want to execute the logic on GroupInfoList objects
            var isGroup = itemType.Name == "GroupInfoList";
            bool isEmpty = false;
            GroupInfoList groupItem;

			// if we're dealing with a GroupInfoList, evaluate whether or not it has data members
            if (isGroup)
            {
                groupItem = item as GroupInfoList;

				// if a GroupInfoList has no Contact data members, it is Empty.
                isEmpty = groupItem.Count == 0;

                // Disable empty items
                var selectorItem = container as SelectorItem;

				// If a SelectorItem is not null, it contains data members (Contact objects) so we need to enable its 
				// ability to be selected (clicked) in each Group header of the ZoomedInView,
				// and on the index to all the groups in the ZoomedOutView of the SemanticZoom control.
                if (selectorItem != null)
                {
                    selectorItem.IsEnabled = !isEmpty;
                }

				// return the correct DataTemplate, which was set in our XAML, for this GroupInfoList
				if (isEmpty)
                {
					// the DataTemplate used to display Contact groups with no Contact members
                    return Empty;
                }
                else
                {
					// the DataTemplate used to display Contact groups with one or more Contact memebers
                    return Full;
                }
            }

			// The default return, because we have to have return a DataTemplate. This will be meaningless to 
			// any DependencyObject which calls us that is not a SelectorItem, but if we return
			// null instead, the ZoomedOutView will not work.
            return Full;

        }
    }
}
