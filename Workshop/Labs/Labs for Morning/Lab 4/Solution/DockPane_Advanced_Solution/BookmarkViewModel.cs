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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace DockPane_Advanced_Solution
{
  /// <summary>
  /// View model for the bookmarks dockpane.  
  /// </summary>
  internal class BookmarkViewModel : DockPane
  {
    private const string _dockPaneID = "DockPane_Advanced_Solution_Bookmark";
    private const string _menuID = "DockPane_Advanced_Solution_Bookmark_Menu";

    /// <summary>
    /// constructor.  
    /// </summary>
    protected BookmarkViewModel() 
    {
      // set up the command to retrieve the maps
      _retrieveMapsCommand = new RelayCommand(() => RetrieveMaps(), () => true);
      // Step 3 - Add delete bookmark command
      _delBookmarkCommand = new RelayCommand(() => DeleteBookmark(), () => true);
    }

    #region Overrides

    /// <summary>
    /// Override to implement custom initialisation code for this dockpane
    /// </summary>
    /// <returns></returns>
    protected override Task InitializeAsync()
    {
      // Step 2 - subscribe to the ProjectItem changed event
      ArcGISProjectItemsChangedEvents.Subscribe(OnProjectCollectionChanged);
      // project closed event
      ArcGISProjectClosedEvents.Unsubscribe(OnProjectClosed);

      return base.InitializeAsync();
    }

    /// <summary>
    /// override to implement custom un-initialisation code for when this dockpane closes. 
    /// </summary>
    /// <returns></returns>
    protected override Task UninitializeAsync()
    {
      // unsubscribe from all events
      // Step 2 - unsubscribe 
      ArcGISProjectItemsChangedEvents.Unsubscribe(OnProjectCollectionChanged);
      ArcGISProjectClosedEvents.Unsubscribe(OnProjectClosed);

      return base.UninitializeAsync();
    }
    #endregion

    #region Subscribed Events

    /// <summary>
    /// Project closed event.  Clear the allMaps collection
    /// </summary>
    /// <param name="eventArgs"></param>
    private void OnProjectClosed(ArcGISProjectEventArgs eventArgs)
    {
      if (_allMaps != null)
        _allMaps.Clear();
    }

    // Step 2 OnProjectCollectionChanged event

    /// <summary>
    /// Project item changed event.  Fired when a project item is added or removed from the project.
    /// </summary>
    /// <param name="args"></param>
    private void OnProjectCollectionChanged(ArcGISProjectItemsChangedEventArgs args)
    {
      if (args == null)
        return;

      if (_allMaps == null)
        return;

      // don't assume already on the dispatch thread...
      System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        // new project item was added
        if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
          // only care about map project item
          if (args.ProjectItem is MapProjectItem)
          {
            // projectItem.Path contains the URI of the item.  Our list of views has this attribute so try and match the new addition.

            // does a map with the uri already exist?
            MapProjectItem map = _allMaps.FirstOrDefault(m => m.Url == args.ProjectItem.Path);
            // one cannot be found; so add it to our list
            if (map == null)
            {
              _allMaps.Add(args.ProjectItem as MapProjectItem);
              NotifyPropertyChanged(() => AllMaps);
            }
          }
        }
        // project item was removed
        else if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
          // only care about map project item
          if (args.ProjectItem is MapProjectItem)
          {
            // does a map with the same uri and type already exist in our list
            MapProjectItem map = _allMaps.FirstOrDefault(m => m.Url == args.ProjectItem.Path);
            // found a match; so remove it from our list 
            if (map != null)
            {
              _allMaps.Remove(args.ProjectItem as MapProjectItem);
              NotifyPropertyChanged(() => AllMaps);
            }
          }
        }
      }));
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

    // Step 4 - Add new bookmark command

    /// <summary>
    /// Command for adding a new bookmark. Bind to this property in the view
    /// </summary>
    private ICommand _newBookmarkCommand;
    public ICommand NewBookmarkCommand
    {
      get
      {
        if (_newBookmarkCommand == null)
          _newBookmarkCommand = Utils.GetICommand("esri_mapping_createBookmark");
        return _newBookmarkCommand;
      }
    }

    // Step 3 - Add delete bookmark command

    /// <summary>
    /// command for deleting a bookmark.  Bind to this property in the view
    /// </summary>
    private ICommand _delBookmarkCommand;
    public ICommand DelBookmarkCommand
    {
      get { return _delBookmarkCommand; }
    }

    // STEP 3 - Add delete bookmark command

    /// <summary>
    /// method for deleting a bookmark
    /// </summary>
    private void DeleteBookmark()
    {
      if (SelectedBookmark == null)
        return;

      if (SelectedMap == null)
        return;

      // find the map
      Map map = MappingModule.FindMap(SelectedBookmark.MapPath);
      if (map == null)
        return;

      // remove the bookmark
      ArcGIS.Desktop.Framework.Threading.Tasks.QueuingTaskFactory.StartNew
        (async () =>
        {
          await map.RemoveBookmarkAsync(SelectedBookmark);
        });
    }

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
    private ObservableCollection<MapProjectItem> _allMaps;
    public IList<MapProjectItem> AllMaps
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
            (async () =>
            {
              _bmks = await map.QueryBookmarksAsync();

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
        NotifyPropertyChanged(() => DelBookmarkToolTip);

        ZoomToBookmark();
      }
    }

    // Step 3 - Add delete tooltip property

    /// <summary>
    /// Getter for the tooltip for the selected bookmark.  Bind to this in the view
    /// </summary>
    public string DelBookmarkToolTip
    {
      get
      {
        if (SelectedBookmark == null)
          return "";

        return string.Format("Delete {0}", SelectedBookmark.Name);
      }
    }

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
