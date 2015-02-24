##Lab 1: Working with DAML 

###What you learn in this lab
* How to customize ArcGIS Pro using DAML
* How to add new elements like Button, Tab, and Group declaratively using DAML
* How to customize the ArcGIS Pro Ribbons (ie Tabs) using DAML
* How to use DAML conditions and states

*******
* [Step 1: Create an ArcGIS Pro Add-In Module Project](#step-1-create-an-arcgis-pro-add-in-module-project)
* [Step 2: Customize the ArcGIS Pro Ribbon with a new Favorites Tab](#step-2-customize-the-arcgis-pro-ribbon-with-a-new-favorites-tab)
* [Step 3: Add ArcGIS Pro Commands to your Favorites Tab](#step-3-add-core-arcgis-pro-commands-to-your-favorites-tab)
* [Step 4: Modify the Core Map Tab](#step-4-modify-the-core-map-tab)
* [Step 5: Create a custom condition and state](#step-5-create-a-custom-condition-and-state)
* [Step 6: Use the custom condition and State to toggle visibility of a Tab Group (Bonus)](#step-6-use-the-custom-condition-and-state-to-toggle-visibility-of-a-tab-group-bonus)

**Estimated completion time: 30 minutes**
****

####Step 1: Create an ArcGIS Pro Add-In Module Project
* If you haven't already done so please clone arcgis-pro-sdk-preview  
* Navigate in your cloned arcgis-pro-sdk-preview repo to this folder:  
  ..\arcgis-pro-sdk-preview\Workshop\Labs\Labs for Morning\Lab 1 
* Open Visual Studio and select New Project.  
* From the New Project dialog, expand the Templates\Visual C#\ArcGIS or Templates\Visual Basic\ArcGIS folder. Select the ArcGIS Pro Add-ins folder.
* The ArcGIS Pro Module Add-in Visual Studio project item template will be displayed. Name your module ```working_with_DAML```. Click OK.  
* Visual Studio will create a new Visual C# or Visual Basic project for you. For the purposes of this document Visual C# will be assumed however the procedure is almost identical for Visual Basic. You will notice the following content in your new project:  

![project-content](http://umaharano.github.io/my-test-repo/images/Lab1/project-content.png "Project Content")

The Config.daml file will be opened (by default) in Visual Studio. The Module1.cs file contains your Add-in Module code. The code will be similar to this:

```c#
internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("working_with_DAML_Module"));
            }
        }
        ....
```

Note that the module contains a private reference to your Module instance which is a singleton. The string "working_with_DAML_Module" is your module id. Framework uses this id to reference your module in the DAML and to find your associated module instance in the FindModule method of the FrameworkApplication module. By default, your module class will be called "Module1". The default namespace will be the name you entered in the New Project dialog, which, in this case, is working_with_DAML_Module. Refer to the [ArcGIS Pro Extensibility document](../../../../../../wiki/ProGuide-ArcGIS-Pro-Add-in-Patterns) for more information on the role of a module.

Note also in the Config.daml file that the id attribute of the insertModule tag matches the id within the Module1.cs file and the className attribute also matches the class name of the Module.

```xml

<modules>
    <insertModule id="working_with_DAML_Module" className="Module1" autoLoad="false" caption="Module1">

```
* Compile and Build the project. If you have errors in your output window check that you have the ArcGIS Pro application and the ArcGIS Pro SDK for .NET correctly installed. As long as you have not changed the syntax in any of the generated files (from the Project template) there should be no compilation errors.  

####Step 2: Customize the ArcGIS Pro Ribbon with a new Favorites tab.

In this step, we will be customizing ArcGIS Pro by creating a two new groups and adding them to a new Favorites 1 Tab on the ArcGIS Pro ribbon.

* Locate the "group" element in your config.daml. Edit its Caption so that a "Lab group 1" group is created. Edit the "appearsOnAddInTab" attribute to be set to "false".  It should now look like this:

```xml
<groups>
        <!-- comment this out if you have no controls on the Addin tab to avoid
              an empty group-->
        <group id="working_with_DAML_Group1" caption="Lab group 1" appearsOnAddInTab="false">
          <!-- host controls within groups -->
        </group>
      </groups>
``` 

* Add another new group called "working_with_DAML_Group2" with a caption attribute set to "Lab group 2".  Copy and paste the DAML below:

```xml
 <group id="working_with_DAML_Group2" caption="Lab group 2" appearsOnAddInTab="false">
          <!-- host controls within groups -->
        </group>
``` 

Your groups element should look like this:

```XML

<groups>
        <!-- comment this out if you have no controls on the Addin tab to avoid
              an empty group-->
        <group id="working_with_DAML_Group1" caption="Lab group 1" appearsOnAddInTab="true">
          <!-- host controls within groups -->
        </group>
       <group id="working_with_DAML_Group2" caption="Lab group 2" appearsOnAddInTab="false">
          <!-- host controls within groups -->
        </group>
      </groups>  
```

* Reference the Lab 1 and Lab 2 groups you created in a new "Favorites 1" tab. In the config.daml in your project, locate the "tabs" element.  Un-Comment the tab element and edit the "caption" attribute to "Favorites 1". Add  group elements with a refID of "working_with_DAML_Group1" and "working_with_DAML_Group2" respectively.  The DAML should look like this now:  

```xml
 <!-- uncomment to have the control hosted on a separate tab-->
      <tabs>
        <tab id="working_with_DAML_Tab1" caption="Favorites 1">
          <group refID="working_with_DAML_Group1"/>
          <group refID="working_with_DAML_Group2"/>
        </tab>
     </tabs>
```

#### Step 3: Add Core ArcGIS Pro Commands to your Favorites Tab

DAML elements can be inserted, updated, or deleted in any Config.daml. This includes "core" elements like commands, menus, and groups. One module may insert a single button into another extension’s ribbon group. To illustrate this, we will now add a few existing ArcGIS Pro Commands to the groups in our Add-in we created in Step 2. We will also reference the "Inquiry" Core group into a new custom "Favorites 2" tab.

* We will use a Visual Studio extension called ArcGIS Pro SDK for .NET (Utilities) for this step. Check if you have this installed by looking at the Extensions listed under Tools > Extensions and Updates in Visual Studio.  
<br>
![ext.png](http://umaharano.github.io/my-test-repo/images/Lab1/ext.png "ArcGIS Pro SDK for .NET (Utilities)")  
<br>
* Right click the project node in the Solution Explorer and select "Pro Generate DAML Ids" from the context menu.  Running the utility provides Intellisense support to your Config.daml file for auto-complete of the refID attribute  value of a DAML element.

Note: If your Config.daml file was open in Visual Studio you will get the following prompt:

![config-daml-warning](http://umaharano.github.io/my-test-repo/images/Lab1/config-daml-warning.png "Config.DAML Warning")

* Click Yes or Yes To All.

* Locate the group element for "working_with_DAML_Group1" in your config.daml. Type `<button refID=` inside the group element. Intellisense will now show up.  
<br>
![btnRef.png](http://umaharano.github.io/my-test-repo/images/Lab1/btnRef.png "Intellisense")
<br>
You can experiment with adding any of these references to your  group.  For the purpose of this exercise, we will assume that the following references are added to Lab 1 and Lab 2 groups but you can experiment with whichever references you like.

Group  | Pro Commands
------------- | -------------
Lab group 1 | `<toolPalette refID ="esri_mapping_newMapPalette"/>`
Lab group 1  | `<button refID="esri_core_saveProjectButton" />`
Lab group 1 | `<button refID="esri_core_saveProjectAsButton"/>`
Lab group 1  | `<gallery refID="esri_mapping_bookmarksNavigateGallery" />`
Lab group 1 | `<button refID="esri_mapping_mapContextMenu_ExportMap" />    `
Lab group 2 | `<toolPalette refID="esri_mapping_selectToolPalette" />`
Lab group 2  | `<button refID="esri_mapping_clearSelectionButton" />`
Lab group 2 | `<button refID="esri_geoprocessing_selectByAttributeButton"/>`
Lab group 2  | `<button refID ="esri_geoprocessing_selectByLocationButton"/>`

* To repeat - be sure to add the references to your _group_ elements

Your group1 may look similar to this (depending on which references you added):

```XML
<!-- comment this out if you have no controls on the Addin tab to avoid
              an empty group-->
        <group id="working_with_DAML_Group1" caption="Lab group 1" appearsOnAddInTab="false">
          <!-- host controls within groups -->
          <!--Core Pro Commands-->
          <toolPalette refID="esri_mapping_newMapPalette" />
          <button refID="esri_core_saveProjectButton" />
          <button refID="esri_core_saveProjectAsButton" />
          <gallery refID="esri_mapping_bookmarksNavigateGallery" />
          <button refID="esri_mapping_mapContextMenu_ExportMap" />          
        </group>
```

* Create another Tab element called "Favorites 2" in your Config.daml. We will add a reference to the Core "Inquiry" group to this tab. Your DAML for the tab should look like this:  

```xml
<tab id="working_with_DAML_Tab2" caption="Favorites 2" >
          <group refID="esri_mapping_inquiryGroup" />
</tab>
```
* Compile and Run the Add-In. ArcGIS Pro will start. Log in with your Organization ID when prompted. Create a new project using the default "Map.aptx" template. After the project has opened, observe your new Favorites tabs containing the Lab groups with the Core commands, tools, etc you referenced.

<br>
![fav.png](http://umaharano.github.io/my-test-repo/images/Lab1/fav.png "Favorites 1")  
![fav.png](http://umaharano.github.io/my-test-repo/images/Lab1/fav2.png "Favorites 2")  
<br>

* Stop debugging.

####Step 4: Modify the Core Map tab

The ArcGIS Pro UI can also be customized using the config.daml configuration file. References can also be added or removed from to a **_core_** group using an "updateModule" and "updateGroup" element. In the updateModule element reference the module (or _extension_) you are modifying and on the updateGroup element reference the core group.

We will delete the core Bookmarks gallery from the Map tab's Navigate group. 

* In your config.daml file, paste the following xml code after the closing tag of the "insertModules" element

```xml
<updateModule refID="esri_mapping">
      <groups>
        <updateGroup refID="esri_mapping_navigateGroup">        
          <deleteButton refID="esri_mapping_bookmarksNavigateGallery"></deleteButton>         
        </updateGroup>
      </groups>
    </updateModule>
```

* Compile and Run the Add-In to see the modified Map Tab.  

Before  | After
------------- | -------------
![fav.png](http://umaharano.github.io/my-test-repo/images/Lab1/map.png "Before")  | ![fav.png](http://umaharano.github.io/my-test-repo/images/Lab1/modified-map.png "After")

####Step 5: Create a custom condition and State.

The ArcGIS Pro framework incorporates a mechanism for triggering the activation of customizations based on user defined conditions. This is provided using the states and conditions DAML constructs. States and conditions provide a simple means for expressing when various GUI elements such as ribbon tabs, dock panes, buttons and tools should and shouldn’t be visible or enabled within the application. Read here for more about [Conditions and States](proconcepts-framework#conditions-and-states). Refer to [ProGuide: Code Your Own States and Conditions](../../../../../../wiki/ProGuide-Code-Your-Own-States-and-Conditions).  

* First, define the new condition and the state in the config.daml. Insert the following xml into the Config.daml. It should be pasted above the modules element.  This will create a new condition called "example_state_condition" and a state called "example_state".

```xml
<conditions>
    <!-- our custom condition -->
    <insertCondition id="example_state_condition" caption="Example state">
      <!-- our condition is set true or false based on this underlying state -->
      <state id="example_state" />
    </insertCondition>
  </conditions>

```  

States are activated in code. We will add a button to our Add-in that will control our custom state ```example_state```.

* In Visual Studio, from the project context menu, select "Add->New Item". Select ArcGIS Pro Button. Change its name to "ToggleStateButton.cs" and click OK.

In your config.daml, you will see a new "button" control added inside the "controls" group.

```xml
 <controls>
        <!-- add your controls here -->
        <button id="working_with_DAML_ToggleStateButton" caption="ToggleStateButton " className="ToggleStateButton" loadOnClick="true" smallImage="Images\GenericButtonBlue16.png" largeImage="Images\GenericButtonBlue32.png" condition="esri_mapping_mapPane">
          <tooltip heading="Tooltip Heading">Tooltip text<disabledText /></tooltip>
        </button>
      </controls>
```
* Edit ToggleStateButton.cs and add the following code into the ToggleStateButton class to activate and deactivate the custom state when the button is clicked.

```c#
 public const string MyStateID = "example_state";
 protected override void OnClick()
        {
            if (FrameworkApplication.State.Contains(MyStateID))
            {
                FrameworkApplication.State.Deactivate(MyStateID);
            }
            else
            {
                FrameworkApplication.State.Activate(MyStateID);
            }
        }

```    
* Insert the Toggle State button into the Map Tab's Navigate group. Locate the <updateGroup ...> element you added in Step 4. Insert this xml code to insert the Toggle State button into the Map Tab.

```xml
<insertButton refID="working_with_DAML_ToggleStateButton"/>
```

```XML
 <updateModule refID="esri_mapping">
      <groups>
        <updateGroup refID="esri_mapping_navigateGroup">
          <deleteButton refID="esri_mapping_bookmarksNavigateGallery"></deleteButton>
          <insertButton refID="working_with_DAML_ToggleStateButton" /><!-- added here -->
        </updateGroup>
      </groups>
    </updateModule>
```

* Note: by default, a reference to the Toggle State button will have automatically been added into the first group defined in your Config.daml (probably "Lab group 1"). For the purpose of this exercise, delete the button reference from this group. 

* Add a second button to your project called RespondToStateButton.cs.  The purpose of this button is to show how it responds to the change in the "Example_state" when Toggle State button is clicked.  Open RespondToStateButton.cs and type in the following code into the OnClick method:  

```c#
protected override void OnClick()
        {
            System.Windows.MessageBox.Show(string.Format("From {0} : {1}", this.GetType().ToString(), DateTime.Now.ToString("G")));
        }

```

* Insert the Respond to State button into the Map Tab's Navigate group. Locate the <updateGroup ...> element you added in Step 4. Insert this xml code to insert the Respond to State button into the Map Tab.

```xml
<insertButton refID="working_with_DAML_RespondToStateButton"/>
```

```XML
    </updateModule><updateModule refID="esri_mapping">
      <groups>
        <updateGroup refID="esri_mapping_navigateGroup">
          <deleteButton refID="esri_mapping_bookmarksNavigateGallery"></deleteButton>
          <insertButton refID="working_with_DAML_ToggleStateButton" />
          <insertButton refID="working_with_DAML_RespondToStateButton" /><!-- added here -->
        </updateGroup>
      </groups>
    </updateModule>
```

* Delete the Respond to State button reference that was automatically added into your first group in your Config.daml

* Edit the RespondToStateButton Button element. It has a default condition attribute set to "esri_mapping_mapPane". Change this to "example_state_condition". It should now look like this:  ... **condition=**"example_state_condition"

```xml
<button id="working_with_DAML_RespondToStateButton" caption="RespondToStateButton " className="RespondToStateButton" loadOnClick="true" smallImage="Images\GenericButtonBlue16.png" largeImage="Images\GenericButtonBlue32.png" condition="example_state_condition">
          <tooltip heading="Tooltip Heading">Tooltip text<disabledText /></tooltip>
        </button>

```

Compile the Add-In and run the sample. You will notice that the "Respond to State" button in the Map Tab is **disabled**

Click the Toggle State button in the Navigate group of the Map tab.  This will enable the "Respond to State" button. Click the Toggle State button again and the Respond to State button will be disabled. You have declaratively tied the enabled/disabled state of the Respond to State button to your custom condition. Your custom condition is set **true** or **false** by activating and deactivating the custom state "example_state" respectively.

![fav.png](https://github.com/UmaHarano/my-test-repo/wiki/images/Lab1/state-disabled.png "Example State is disabled")  

![tab-groups.png](https://github.com/UmaHarano/my-test-repo/images/Lab1/state-enabled.png "Example State is enabled")  

####Step 6: Use the custom condition and State to toggle visibility of a Tab Group (Bonus)

* We will now group the Favorite 1 and Favorite 2 tabs together to create an "Example State" tab group. This tab group's visibility is to be linked to the custom "example_state_condition" condition we created in Step 5.  Locate the "insertModule" element and create a new "Tab Group" called "Example State" using the xml code below:

```xml
    <insertModule id="working_with_DAML_Module" className="Module1" autoLoad="false" caption="Module1">
      <tabGroups>
        <tabGroup caption="Example State" id="working_with_DAML_ExampleStateTabGroup" >
          <color A="255" R="238" G="170" B="90" />
          <borderColor A="0" R="251" G="226" B="195" />
        </tabGroup>
      </tabGroups>
....
```

* Locate the two tabs (Favorite 1 and Favorite 2) you created in your Config.daml. Edit them by adding two new attributes - the "tabGroupID" (containing the ID of the new Tab group you created above) and the "condition" attribute (containing the new "example_state_condition" condition).  The tab elements should now look like this:<br/>

* **tabGroupID=**"working_with_DAML_ExampleStateTabGroup"
* **condition=**"example_state_condition"

```xml
       <tabs>
        <tab id="working_with_DAML_Tab1" caption="Favorites 1" tabGroupID="working_with_DAML_ExampleStateTabGroup" condition="example_state_condition">
          <group refID="working_with_DAML_Group1" />
          <group refID="working_with_DAML_Group2" />
        </tab>
        <tab id="working_with_DAML_Tab2" caption="Favorites 2" tabGroupID="working_with_DAML_ExampleStateTabGroup" condition="example_state_condition">
          <group refID="esri_mapping_inquiryGroup" />
        </tab>
      </tabs>
....
```

* Compile the Add-In and run the sample. As you click the Toggle State button notice how the tab group is displayed and hidden in conjunction with the condition being enabled and disabled via the underlying state.

![fav.png]((http://umaharano.github.io/my-test-repo/images/Lab1/state-disabled.png "Example State is disabled")  

![tab-groups.png]((http://umaharano.github.io/my-test-repo/images/Lab1/state-enabled.png "Example State is enabled")  
