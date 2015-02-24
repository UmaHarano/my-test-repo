#Workshop Labs
[Additional Notes from internal meetings](#notes)

### Labs for Morning

**Lab1 - Creating Button Add-in and working with DAML**  
[Lab folder](Labs%20for%20Morning/Lab%201)  
* Create the add-in project with a button item.
* Customizing the ArcGIS Pro Ribbon: create a favorite tab
* Define a custom condition and trigger some state.

**Lab2 - Interact with Project Items**<br/>
[Lab folder](Labs%20for%20Morning/Lab%202)  
* Begin with iteration of project items to access maps.  
* Listen to Project Item changed events.  
* Display count of the bookmarks per Map.  

**Lab3 - Add Dockpane Item**<br/>
[Lab folder](Labs%20for%20Morning/Lab%203)  
* Add a Dockpane item to Lab 2.  
* Change Dockpane title.  
* Add List Box containing list of maps. Add List box for bookmarks.   
* Use Map List change event to populate Bookmark List with bookmarks for the selected map.  
* Click on "bookmark" thumbnail, zoom to bookmark  

**Lab4 - Refine Dockpane**<br/>
[Lab folder](Labs%20for%20Morning/Lab%204)  
* Handle map add/remove events in the dockpane lists.  
* Add additional functionality to the MapItem view model:  
 - Delete button,  
 - "Take Snapshot" button (ExecuteTool?)  
* Add "Add" button to add new bookmarks on the Ribbon  
* ~all buttons styled to follow Pro styles~  

**Lab5 - Backstage Customization and Custom Property(properties)**<br/>
[Sample](Labs%20for%20Morning/Lab%205)  
* Add a new category to the "Options" property sheet on Backstage  
* Implement a property page with the custom property (eg checkbox listen for project item changed events). If you check the checkbox on the Dockpane updates when maps are added or deleted to the project...otherwise it doesn't.

**Samples, Snippets to be included for the morning session:**<br/>
This is extra that I would like to cover but is not necessarily a lab.  
* Notifications  
* Context Menu  
* Drag drop  
* More on events  

## Afternoon session

### Labs for Afternoon

The ordering may need some work:<br/>
**Lab1 Custom Control and Navigating the Camera**<br/>
[Lab folder](Labs%20for%20Afternoon/Lab%201) Chris   
* Based on Thomas' custom control from the UC 2014 (start is the UserControl)
* Implement 2D camera mmanipulation using the control (we give them the XAML)
* Implemment 3D camera manipulation using the control
* Add slider to zoomin and out
* Add control to mainpulate tilt (disabled in 2D)

**Lab2 Introduction to Tool and Selection**<br/>
[Lab folder](Labs%20for%20Afternoon/Lab%202) 
* Implement a 2D,3D selection tool in Mapping on MapView
* Zooms to extent of selected feature(s) 
* (Sample)
* (Note: perhaps configuration property via backstage property page)

**Lab 3 Geometry and Core.Data Construction Buttons**<br/>
[Lab folder](Labs%20for%20Afternoon/Lab%203)  (Thomas)
* Button1: Construct random points on the map (add to map notes or scratch feature layer)
* (Use "provided" coordinate generator and add points to edit operation)
* Button2: Construct polylines from points
* Button3: Construct polygon (via Geometry Engine?)

**Lab 4 Construction Tool**<br/>
[Lab folder](Labs%20for%20Afternoon/Lab%204)
* Basic sketch tool: Geometry type, Layer construction tool palette
* Set default attribute values (via Dictionary or FeatureInspector) as part of the Create Operation

***Lab 5 Edit Tool**<br/>
[Lab folder](Labs%20for%20Afternoon/Lab%205)  
* Basic sketch tool: Change DAML. Set base properties (as needed)
* Digitize polyline geometry - cut everything (filter layers by geometry type)
* Edit events and update features (bonus)
<br/><br/>
*>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><br/><br/>
* As the user moves the mouse pointer, a status bar (where??!) updates with the X,Y location
of the mouse
* When the user selects features, a status bar (where??!?) is updated with the number of features selected
Clearly a bit of investigation is needed on how to implement a status bar. I am thinking we can achieve this with a dockpane that is docked to the bottom of the application window

**Lab 3 Proximity Tool**<br/>
[Lab folder](Labs%20for%20Afternon/Lab%203)  
* This is an idea Thomas had for a tool. As the user moves the mouse pointer, the vertices of feature geometries that are within "range" (how do we define range?? perhaps as....a property on our backstage property page??) are highlighted. As the mouse pointer moves out of range the vertices would not be highlighted.  
* When the user clicks (left clicks), features wtihin the "range" (i.e. in proximity) would be selected.  
TBD: Can we do "clean" graphics effects adding and removing "dots" for the vertices?

**Samples, Snippets to be included for the afternoon session:**<br/>
This is extra that we can cover but is not necessarily a lab.
* Given a map, enumerate layers
* Time and manipulating time settings
* Undo/Redo Operation Stack
* Various GDB snippets - Feature creation (headless). CreateRowBuffer, feature.store.
* Change Versions, Relationships…..
* Geometry - Point creation, line creation, poly creation
* Geometry - Enumerate polygon, polyline segment collection
* Execute Tool (for GP)


#Notes
## January 22nd, 2015

* UC 2015 plans: 1 tech session, 2 offerings. 2 demo theatre slots.
* Dev Summit Workshop
Samples first. Samples are converted into labs. I have the following assignments:
 * Customizing UI via DAML (“favorites” tab, condition) Wolf
 * Projectitems? Events? Context Menu? (Narelle) (more advanced)
 * Bookmarks? (Chris)
 * Styling, Fonts, Controls (Uma)<br>
 **Tool**<br>
 * Editing, Geometry, GDB (Thomas)
 * Select, Identify, Sketch, GDB (Chris)
 * Execute tool (GP) (Charlie)
 * “Advanced” contracts: Split Button, Property Sheet, Backstage Customization, Context Menu, Custom Control (Narelle, Wolf, Uma)<br>
**Considerations:**<br>
Weave these patterns into your examples:<br>
a.	Async and await, QTF, MVVM<br>
We will regroup next week and review the samples. Please triple-slash to the “nth” degree. If you can’t get to the coding because of other commitments please, as a minimum, write a text description/paragraph explaining what your sample, once implemented, will do and illustrate.

Dates:<br>
Charlie will work on dates for two dry runs. Tentatively 2nd week of Feb and 1st week of March.

## January 29th, 2015
* More clarity on the samples and basis for labs
* Morning session can be built around a single main lab (Lab1+ Dockpane) with a few additional options
* Afternoon session will have more of a split between MapEx, intro to Tool, and Editing....I think this will be hard to overlap

**Dates** 
* Dry run 1 (_informal_): Wednesday Feb 18th
* Dry run 2 (_formal_): Tuesday March 3rd







