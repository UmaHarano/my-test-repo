###What you learn in this lab
* How to create a Dockpane using DAML
* How to use the MVVM Pattern with ArcGIS Pro
* How to change the Dockpane title
* How to add a ListBox to the Dockpane showing the list of maps
* How to add a ListBox to the Dockpane showing bookmarks of a selected map
* How to use the Map selection changed event to populate a list of bookmarks with bookmarks for the selected map.
* How to use Bookmark list selection change event to zoom map view to bookmark


*******
* [Step 1: Open the Start Solution and add a Dockpane](#step-1-open-the-start-solution-and-add-a-dockpane)  
* [Step 2: The MVVM Pattern changing the Dockpane](#step-2-the-mvvm-pattern-changing-the-dockpane)
* [Step 3: Add a ListBox to the Dockpane showing the list of maps](#step-3-add-a-listbox-to-the-dockpane-showing-the-list-of-maps)
* [Step 4: Add a ListBox to the Dockpane showing bookmarks of a selected map](#step-4-add-a-listbox-to-the-dockpane-showing-bookmarks-of-a-selected-map)
* [Step 5: Use Map list selection change event](#step-5-use-map-list-selection-change-event)
* [Step 6: Use Bookmark list selection change event](#step-6-use-bookmark-list-selection-change-event)


**Estimated completion time: 30 minutes**

####Step 1: Open the Start Solution and add a Dockpane<br>
* If you haven't already done so please clone arcgis-pro-sdk-preview  
* Navigate in your cloned arcgis-pro-sdk-preview repo to this folder:  
  ..\arcgis-pro-sdk-preview\Workshop\Labs\Labs for Morning\Lab 3\Start  
* Open the start solution called 'MorningLab3Start.sln' using VS 2013
* Run the project using the VS debugger and validate the Add-in place holder in ArcGIS Pro:
  
![RunStart](http://umaharano.github.io/my-test-repo/images/RunStartSolution.png)  

* Close ArcGIS Pro to stop the debugger
* In Solution Explorer right click on the 'MorningLab3Start' project node and click on 'Add' | 'New Item'  
* In the Add New Item dialog navigate to 'Visual C# Items' | 'ArcGIS' | 'ArcGIS Pro Add-ins' and select 'ArcGIS Pro Dockpane.  Rename the default name 'Dockpane1.xaml' to 'BookmarkDockpane.xaml'  
* Build the Solution and debug the newly Added dockpane.  You should see a dockpane that looks like this:  

![ShowDockpane](http://umaharano.github.io/my-test-repo/images/ShowDockpane.png)
  
* Explore some of you dockpane's built-in features such as docking  
* Close ArcGIS Pro to stop the debugger  

***Review the following:***  

Explore the changes the item add-in template made to your project.  A new button and dockpane were added in your config.daml.  These new controls where added under the add-in module 'MorningLab3Start_Module':  

```xml 
 <modules>

	<insertModule id="MorningLab3Start_Module" className="Module1" autoLoad="false" caption="Module1">
	  
	  <controls>
	    <!-- add your controls here -->
	    <button id="MorningLab3Start_BookmarkDockpane_ShowButton" caption="Show BookmarkDockpane " className="BookmarkDockpane_ShowButton" loadOnClick="true" smallImage="Images\GenericButtonPurple16.png" largeImage="Images\GenericButtonPurple32.png">
	      <tooltip heading="Show Dockpane">Show Dockpane<disabledText /></tooltip>
	    </button>
	  </controls>

	  <dockPanes>
	    <dockPane id="MorningLab3Start_BookmarkDockpane" caption="BookmarkDockpane " className="BookmarkDockpaneViewModel" keytip="DockPane" initiallyVisible="true" dock="group" dockWith="esri_core_contentsDockPane">
	      <content className="BookmarkDockpaneView" />
	    </dockPane>
	  </dockPanes>

	</insertModule>
 </modules>

```  

You will also find that the add-in template added a BookmarkDockpane.xaml and BookmarkDockpaneViewModel.cs to your project. This is a classic WPF Model View ViewModel, MVVM, pattern which is used to separate the development of the graphical user interface (the View) from the development of the business logic or back-end logic (the model).  BookmarkDockpane.xaml contains the GUI as XAML markup and BookmarkDockpaneViewModel.cs the back-end business logic. The following step will explore the use of MVVM in the context of an ArcGIS Pro add-in.

####Step 2: The MVVM Pattern changing the Dockpane<br>
* In order to change our dockpane we first make a few simple changes in DAML  
* From Solution Explorer open Config.daml  
* Find the 'show dockpane button' and change the caption to: 'Show My Bookmarks'  

```xml  

<button id="MorningLab3Start_BookmarkDockpane_ShowButton" caption="Show My Bookmarks" ...>
	<tooltip heading="Show Dockpane">Show Dockpane<disabledText /></tooltip>
</button>

```

* Find the 'dockpane control' and change its caption to: 'My Bookmarks'  

```xml  

<button id="MorningLab3Start_BookmarkDockpane_ShowButton" caption="Show My Bookmarks" ...>
	<tooltip heading="Show Dockpane">Show Dockpane<disabledText /></tooltip>
</button>

```  

Now we make changes to the actual dockpane content  
* From Solution Explorer open 'BookmarkDockpaneViewModel.cs' and navigate to the 'Heading' property  
* Note that this property is initialized with 'My Dockpane' change this to 'My Maps and Bookmarks'  

```cs  
 
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

```

* Run the project using the VS debugger and verify the changes in ArcGIS Pro  
* Close ArcGIS Pro to stop the debugger  

***Review the following:***  

First we made some simple declarative caption changes in our DAML file.  Then we made some changes in to our dockpane view model.  Please note that we changed the content of our dockpane without touching the XAML GUI frontend, the change was solely made in the 'code behind' view model class.    
 

####Step 3: Add a ListBox to the Dockpane showing the list of maps<br>
* Next we are adding a listbox to the dockpane that shows all maps, as they are shown in the Pro 'Project pane' below  
  
![MyBookmarks](http://umaharano.github.io/my-test-repo/images/MapsInPro.png)  
 
* From Solution Explorer open 'BookmarkDockpane.xaml' and add the following XAML snippet after the <Dockpanel>...</DockPanel> section.
  
```xml  
 
<Grid Grid.Row="1" >
    <Grid.RowDefinitions>
        <RowDefinition Height="3*"/>
        <RowDefinition Height="7*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    <ListBox Grid.Row="0" Grid.Column="0" Name="LstMaps"
		ItemsSource="{Binding ListOfMaps}" Margin="5"
 		DisplayMemberPath="Name" SelectedItem="{Binding SelectedMapProjectItem, 		Mode=TwoWay}"/>
    <Button Grid.Row="0" Grid.Column="1" Height="25" Margin="5" Command="{Binding 	RetrieveMapsCommand}">Get Maps</Button>
    
</Grid>

```

* This adds a listbox for our Maps and a button which will fill the list of our Dockpane UI  
* Next we add the business logic for our button to BookmarkDockpaneViewModel.cs   
* In your BookmarkDockpaneViewModel class add the following snippets:

```cs 

private ObservableCollection<MapProjectItem> _listOfMaps = new ObservableCollection<MapProjectItem>();
/// <summary>
/// Our List of Maps which is bound to our Dockpane XAML
/// </summary>
public IList<MapProjectItem> ListOfMaps
{
    get { return _listOfMaps; }
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
    set {
        SetProperty(ref _selectedMapProjectItem, value, () => SelectedMapProjectItem);
	}
}

```

* Run the project using the VS debugger and verify that the load button loads all maps in ArcGIS Pro:
  
![MyBookmarks](http://umaharano.github.io/my-test-repo/images/GetMaps.png)  

* Make a map selection and debug the SelectedMapProjectItem property's set function and make sure that your breakpoint is hit with a map selection change  

![MyBookmarks](http://umaharano.github.io/my-test-repo/images/SelectMapBreakpoint.png)  

* Close ArcGIS Pro to stop the debugger

***Review the following:***  

The code modifications above dove deeper into MVVM.  We added a button 'Get Maps' to retrieve the list of map project items.  In typical MVVM style this button was implemented as a RelayCommand (_ICommand_) with an Execute method (used to load the map list) and a CanExecute method (used to enable / display the button).  We implemented both as lambdas though we could just have easily have defined and referenced methods. The 'Get Maps' button only becomes enabled when the CanExecute _condition_ ```ProjectModule.CurrentProject != null``` is true.  Once ProjectModule.CurrentProject is defined we can query the GetMaps () to retrieve a collection of Map Project Items.  We use the name of the map project items for display in the map list.

####Step 4: Add a ListBox to the Dockpane showing bookmarks of a selected map<br>
* We will add a listbox to the dockpane to show all bookmarks of a selected map. This is how they are shown in the Pro 'Project pane':  
  
![MyBookmarks](http://umaharano.github.io/my-test-repo/images/AllBookmarks.png)  
 
* From Solution Explorer open 'BookmarkDockpane.xaml' and add the following XAML snippet after the <Button ...">Get Maps</Button> section. 
  
```xml  
 
<ListBox Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Name="LstBookmarks" ItemsSource="{Binding ListOfBookmarks}" Margin="5" DisplayMemberPath="Name" SelectedItem="{Binding SelectedBookmark, Mode=TwoWay}"/>
    
</Grid>

```

* This adds our listbox for the bookmarks to our Dockpane UI  
* Next we add the business logic to BookmarkDockpaneViewModel.cs   
* In your BookmarkDockpaneViewModel class add the following snippets:

```cs 

private ReadOnlyObservableCollection<Bookmark> _listOfBookmarks = null;
/// <summary>
/// Our List of Bookmark which is bound to our Dockpane XAML
/// </summary>
public IList<Bookmark> ListOfBookmarks
{
    get { return _listOfBookmarks; }
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
    }
}

```

* Run the project using the VS debugger and verify the new UI in ArcGIS Pro:
  
![MyBookmarks](http://umaharano.github.io/my-test-repo/images/UIWithBookmarks.png)  

* The List of bookmarks is still empty since we are not loading any bookmarks when the map selection changes.  
* Close ArcGIS Pro to stop the debugger  

####Step 5: Use Map list selection change event

* Next we need to fill the list of bookmarks when the map selection changes, in order to so this we add the business logic to BookmarkDockpaneViewModel.cs
* In your BookmarkDockpaneViewModel class replace the SelectedMapProjectItem property with the following snippet:  

```cs

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

```

* Run the project using the VS debugger and verify the new map selection function in ArcGIS Pro:  

![MyBookmarks](http://umaharano.github.io/my-test-repo/images/BookmarkListShows.png)  

* Close ArcGIS Pro to stop the debugger
* Make a map selection and debug the SelectedMapProjectItem property's set function at the 'ArcGIS...Tasks.QueuingTaskFactory.StartNew' line and step over the the code      

![MyBookmarks](http://umaharano.github.io/my-test-repo/images/DebugMapSelection.png)  

* Pay close attention to the debugger step sequence  
* Close ArcGIS Pro to stop the debugger  

***Review the following:***  
As you stepped through the map selection changed code over the 'ArcGIS.Desktop.Framework.Threading.Tasks.QueuingTaskFactory.StartNew' method, you probably noticed that the _listOfBookmarks assignment didn't occur on the UI thread but instead was called assynchronously.  This is a common pattern used in ArcGIS Pro allowing the UI to remain responsive while GIS functions (in this case Query Bookmarks) are executed.  

  
####Step 6: Use Bookmark list selection change event
* Finally we will add a 'zoom to bookmark' function each time the bookmark selection changes  
* In order to so this we add the business logic to BookmarkDockpaneViewModel.cs  
* In your BookmarkDockpaneViewModel class replace the SelectedBookmark property with the following snippet:  

```cs

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

```

* Run the project using the VS debugger and verify the new bookmark selection and zoom function in ArcGIS Pro.  
* Close ArcGIS Pro to stop the debugger  
