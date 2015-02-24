//Copyright 2015 Esri

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;

namespace MorningLab3Start
{
    internal class BookmarkDockpaneViewModel : DockPane
    {
        private ObservableCollection<MapProjectItem> _listOfMaps = new ObservableCollection<MapProjectItem>();
        /// <summary>
        /// Our List of Maps which is bound to our Dockpane XAML
        /// </summary>
        public IList<MapProjectItem> ListOfMaps
        {
            get { return _listOfMaps; }
        }

        private ReadOnlyObservableCollection<Bookmark> _listOfBookmarks = null;
        /// <summary>
        /// Our List of Bookmark which is bound to our Dockpane XAML
        /// </summary>
        public IList<Bookmark> ListOfBookmarks
        {
            get { return _listOfBookmarks; }
        }

        private const string _dockPaneID = "MorningLab3Start_BookmarkDockpane";

        protected BookmarkDockpaneViewModel() { }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.FindDockPane(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My Maps and Bookmarks";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        private ICommand _retrieveMapsCommand;
        /// <summary>
        /// Implement a 'RelayCommand' to retrieve all maps from the current project
        /// </summary>
        public ICommand RetrieveMapsCommand
        {
            get
            {
                if (_retrieveMapsCommand != null) return _retrieveMapsCommand;
                _retrieveMapsCommand = new RelayCommand(
                    p =>
                    {
                        ListOfMaps.Clear();
                        if (ProjectModule.CurrentProject != null)
                        {
                            // maps
                            foreach (MapProjectItem item in ProjectModule.CurrentProject.GetMaps())
                            {
                                if (item != null) ListOfMaps.Add(item);
                            }
                        }
                        NotifyPropertyChanged(() => ListOfMaps);
                    },
                    p => ProjectModule.CurrentProject != null);
                return _retrieveMapsCommand;
            }
        }

        private MapProjectItem _selectedMapProjectItem;
        /// <summary>
        /// This is where we store the selected map 
        /// </summary>
        public MapProjectItem SelectedMapProjectItem
        {
            get { return _selectedMapProjectItem; }
            set
            {
                SetProperty(ref _selectedMapProjectItem, value, () => SelectedMapProjectItem);

                // open the map
                if (_selectedMapProjectItem != null)
                {
                    Utils.OpenAndActivateMap(_selectedMapProjectItem.Path);

                    // get the bookmarks
                    Map map = MappingModule.FindMap(_selectedMapProjectItem.Path);
                    if (map != null)
                    {
                        ArcGIS.Desktop.Framework.Threading.Tasks.QueuingTaskFactory.StartNew
                            (() =>
                            {
                                _listOfBookmarks = map.QueryBookmarksAsync().Result;

                                // force the notify property changed to get the view to refresh
                                NotifyPropertyChanged(() => ListOfBookmarks);
                            });
                    }
                }
            }
        }

        private Bookmark _selectedBookmark;
        /// <summary>
        /// This is where we store the selected Bookmark 
        /// </summary>
        public Bookmark SelectedBookmark
        {
            get { return _selectedBookmark; }
            set
            {
                SetProperty(ref _selectedBookmark, value, () => SelectedBookmark);

                // zoom to it
                MappingModule.ActiveMapView.ZoomToAsync(_selectedBookmark); 
            }
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class BookmarkDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            BookmarkDockpaneViewModel.Show();
        }
    }
}
