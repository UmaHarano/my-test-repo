##Lab 2: Interact with Project Items 

###What you learn in this lab
In this lab you will learn how to iterate through the project items in a container. You will learn how to get the collection of Maps. You will also learn how to subscribe to the ArcGISProjectItemsChangedEvents to update your viewmodel's map collection when new maps are added to or removed from the project. Bobus: Get the collection of bookmarks from a Map

*******

* [Step 1: Iterate through project items](#step-1-iterate-through-project-items)
* [Step 2: Listen to Project Item changed events](#step-2-listen-to-project-item-changed-events)

**Estimated completion time: 15 minutes**

****

####Step 1: Iterate through project items
* If you haven't already done so please clone arcgis-pro-sdk-preview  
* Navigate in your cloned arcgis-pro-sdk-preview repo to this folder:  
  ..\arcgis-pro-sdk-preview\Workshop\Labs\Labs for Morning\Lab 2\Start 
* Open the "Interact with Project Items" project in the Start folder. A "ListProjectItems" button has already been added for you. Look at the config.daml entry for this button.
* Open ListProjectItem.cs from the solution explorer. An array of project container names has been created in the OnClick method.

```c#
//Array of project item container names
            string[] ProjectItemContainerNames = new string[]
            {
                "Map",
                "GDB",
                "Styles",
                "GP",
                "FolderConnection",
                "Layout"                  
            };
            
```
* Iterate through this array to get the project items for each container.  Display all the project items found using a message box.

Hint:
```c#
var ProjContainer = project.GetProjectItemContainer<IProjectItemContainer>("Project Container Name");
```
* Compile the add-in to test the List of Project Items button. You should get something like this:  
![list of project item](http://umaharano.github.io/my-test-repo/images/Lab2/list-of-project-items.png)


####Step 2: Listen to Project Item changed events  
A "MapViewsComboBox" has been created for you that holds the collection of maps in the project.  In this step, we will  subscribe to the ArcGISProjectItemsChangedEvents event. This event is published each time a new map is added or removed from the project. The comboBox will be updated using this event. 

* Open the MapViewsComboBox.cs. Familiarize yourself with this class. "MapViewsComboBox" class, inherits from  ArcGIS.Desktop.Framework.Contracts.ComboBox. 

* Open the Config.daml file and see the "comboBox" control inside the "controls" group.  It has a unique "id" for the control, a "caption", and "className" attribute. Classname is derived from "namespace.comboBox class". There is a reference to this comboBox inside the "Interact with Project Items" group.

* You will now subscribe to the "ArcGISProjectItemsChangedEvents" event to listen to new maps being added or deleted from the project.  Subscribe to the event in the constructor of the MapViewsComboBox class. Complete the code for the OnProjectCollectionChanged event handler. Unsubscribe to the event in the destructor.  

* Compile and run the add-in. Experiment with adding and removing maps from the project. The comboBox should update with these changes.

* Bonus: Check the solution. Look at CountBookmarks.cs to see how to access the collection of bookmarks for a map. 


