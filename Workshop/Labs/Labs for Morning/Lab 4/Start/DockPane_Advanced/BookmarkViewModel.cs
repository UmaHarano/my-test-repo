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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DockPane_Advanced
{
    /// <summary>
    /// View model for the bookmarks dockpane.  
    /// </summary>
    internal class BookmarkViewModel : DockPane
    {
        private const string _dockPaneID = "DockPane_Advanced_Bookmark";
        private const string _menuID = "DockPane_Advanced_Bookmark_Menu";
        private static readonly object _lockMapCollection = new object();

        /// <summary>
        /// constructor.  
        /// </summary>
        protected BookmarkViewModel()
        {
            // set up the command to retrieve the maps
            _retrieveMapsCommand = new RelayCommand(() => RetrieveMaps(), () => true);

            // TODO Step 3 - add delete bookmark command
        }

        #region Overrides

        /// <summary>
        /// Override to implement custom initialization code for this dockpane
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeAsync()
        {
            // TODO Step 2 - make sure that AllMaps can be updated from work threads as well as the UI thread
            BindingOperations.EnableCollectionSynchronization(AllMaps, _lockMapCollection);
            // TODO Step 2 - subscribe to the ArcGISProjectItemsChangedEvents
            ArcGISProjectItemsChangedEvents.Subscribe(OnProjectCollectionChanged);
            return base.InitializeAsync();
        }
        #endregion

        #region Subscribed Events

        // TODO Step 2 - add OnProjectCollectionChanged method
        /// <summary>
        /// Subscribe to Project Item changed events which is getting called each
        /// time project items are added or removed in ArcGIS Pro
        /// </summary>
        /// <param name="args">ArcGISProjectItemsChangedEventArgs</param>
        private void OnProjectCollectionChanged(ArcGISProjectItemsChangedEventArgs args)
        {
            if (args == null)
                return;
            var mapItem = args.ProjectItem as MapProjectItem;
            if (mapItem == null)
                return;

            // new project item was added
            switch (args.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        var foundItem = _allMaps.FirstOrDefault(m => m.Url == mapItem.Path);
                        // one cannot be found; so add it to our list
                        if (foundItem == null) _allMaps.Add(mapItem); 
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        // only care about map project item
                        if (_allMaps.Contains(mapItem)) _allMaps.Remove(mapItem);
                    }
                    break;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command for retrieving commands.  Bind to this property in the view.
        /// </summary>
        private ICommand _retrieveMapsCommand;
        public ICommand RetrieveMapsCommand
        {
            get { return _retrieveMapsCommand; }
        }

        /// <summary>
        /// Method for retrieving map items in the project.
        /// </summary>
        private void RetrieveMaps()
        {
            // reset
            _selectedMap = null;
            _selectedBmk = null;
            _bmks = null;

            // create / clear the collection
            if (_allMaps == null)
                _allMaps = new ObservableCollection<MapProjectItem>();
            else
                _allMaps.Clear();

            if (ProjectModule.CurrentProject != null)
            {
                // get the map project items and add to my collection
                foreach (IProjectItem item in ProjectModule.CurrentProject.GetMaps())
                {
                    _allMaps.Add(item as MapProjectItem);
                }
            }

            // ensure the view re-binds to both the maps and bookmarks
            NotifyPropertyChanged(() => AllMaps);
            NotifyPropertyChanged(() => Bookmarks);
        }

        // TODO Step 3 - Add DelBookmarkCommand property, member variable and action function

        // TODO Step 4 - Add new bookmark command property and member variable


        #endregion

        #region Properties

        /// <summary>
        /// collection of bookmarks.  Bind to this property in the view.
        /// </summary>
        private ReadOnlyObservableCollection<Bookmark> _bmks;
        public ReadOnlyObservableCollection<Bookmark> Bookmarks
        {
            get { return _bmks; }
        }

        /// <summary>
        /// Collection of map items.  Bind to this property in the view. 
        /// </summary>
        private ObservableCollection<MapProjectItem> _allMaps = new ObservableCollection<MapProjectItem>();
        public IReadOnlyCollection<MapProjectItem> AllMaps
        {
            get { return _allMaps; }
        }

        /// <summary>
        /// Holds the selected map from the combobox.  When setting the value, ensure that the map is open and active before retrieving the bookmarks
        /// </summary>
        private MapProjectItem _selectedMap = null;
        public MapProjectItem SelectedMap
        {
            get { return _selectedMap; }
            set
            {
                SetProperty(ref _selectedMap, value, () => SelectedMap);

                // open /activate the map
                Utils.OpenAndActivateMap(_selectedMap.Path);

                // get the bookmarks
                Map map = MappingModule.FindMap(_selectedMap.Path);
                if (map != null)
                {
                    _selectedBmk = null;
                    ArcGIS.Desktop.Framework.Threading.Tasks.QueuingTaskFactory.StartNew
                      (() =>
                        {
                            _bmks = map.QueryBookmarksAsync().Result;
                            // force the notify property changed to get the view to refresh
                            NotifyPropertyChanged(() => Bookmarks);
                        });
                }
            }
        }

        /// <summary>
        /// Holds the selected bookmark from the listview. 
        /// </summary>
        private Bookmark _selectedBmk;
        public Bookmark SelectedBookmark
        {
            get { return _selectedBmk; }
            set
            {
                SetProperty(ref _selectedBmk, value, () => SelectedBookmark);

                // TODO Step 3 - update the DelBookmarkToolTip property when the selected bookmark changes

                ZoomToBookmark();
            }
        }

        // TODO Step 3 - Add DelBookmarkToolTip property

        #endregion

        #region Zoom to Bookmark

        /// <summary>
        /// Zooms to the currently selected bookmark. 
        /// </summary>
        internal void ZoomToBookmark()
        {
            if (SelectedBookmark == null)
                return;

            // make sure the map is open
            Utils.OpenAndActivateMap(SelectedBookmark.MapPath);
            // zoom to it
            MappingModule.ActiveMapView.ZoomToAsync(SelectedBookmark);
        }
        #endregion

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
        private string _heading = "Bookmarks";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        #region Burger Button

        /// <summary>
        /// Tooltip shown when hovering over the burger button.
        /// </summary>
        public string BurgerButtonTooltip
        {
            get { return "Options"; }
        }

        /// <summary>
        /// Menu shown when burger button is clicked.
        /// </summary>
        public System.Windows.Controls.ContextMenu BurgerButtonMenu
        {
            get { return FrameworkApplication.CreateContextMenu(_menuID); }
        }
        #endregion
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Bookmark_ShowButton : Button
    {
        protected override void OnClick()
        {
            BookmarkViewModel.Show();
        }
    }

    /// <summary>
    /// Button implementation for the button on the menu of the burger button.
    /// </summary>
    internal class Bookmark_MenuButton : Button
    {
        protected override void OnClick()
        {
        }
    }
}
