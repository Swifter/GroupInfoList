using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GroupList.Model;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GroupList.GroupList
{
    public sealed partial class GroupedListView : UserControl
    {
        public GroupedListView()
        {
            this.InitializeComponent();

			// Sets the CollectionViewSource to an ObservableCollection of GroupInfoList objects
			// containing Contacts generated randomly.
            ContactsCVS.Source = Contact.GetContactsGroupedAllAlpha(200);
        }

		/// <summary>
		/// Click handler, toggles the SemanticZoom control's ZoomedInView or ZoomedOutView.  This toggling can also be
		/// done by clicking on the ZoomedInView's header for each row in its GridView.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void ZoomInOutBtn_Click(object sender, RoutedEventArgs e)
        {
            ZoomControl.IsZoomedInViewActive = !ZoomControl.IsZoomedInViewActive;
        }
    }
}
