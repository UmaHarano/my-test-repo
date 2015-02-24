## Lab 4: Construction Tool

###What you learn in this lab
* Basic SketchTool functionality to construct features
* Different ways to perform attribute edits

*******
* [Step 1: Create a basic sketch tool for polylines](#step-1-create-a-basic-sketch-tool-for-polylines)
* [Step 2: Perform edits on feature attributes](#step-2-perform-edits-on-feature-attributes)

**Estimated completion time: 15 minutes**
****

##Step 1: Create a basic sketch tool for polylines
* Download the EditingTools.pkgx from ArcGIS Online and save the project.
* Open the _ConstructionTool_ solution.
* Open the _SimpleSketchAndAttribute.cs_ file.
* The core ArcGIS Pro application provides the initial sketch feedback mechanism. Once the user indicates that the sketch (feature construction) is complete, we need to decide what to do with the geometry.
* Navigate to the _FinishSketch_ function. The arguments are the sketch geometry itself and any overriden attributes. In our case we would like to construct features and for this case the Dictionary will be empty.
* Set up a create edit operation.
* Set some properties.
* Call execute.
* Done.
* For our example we would like to place our construction in the context of polyline feature layers. This is done by associating the sketch tool with the polyline geometry type in the _Config.daml_ file.

```xml
<tool id="ConstructionTool_SimpleSketchAndAttribute" categoryRefID="esri_editing_EditTools_POLYLINE" caption="SimpleSketchAndAttribute " extendedCaption="SimpleSketchAndAttribute (Lab)" className="SimpleSketchAndAttribute" loadOnClick="true" smallImage="Images\GenericButtonRed16.png" largeImage="Images\GenericButtonRed32.png">
  <tooltip heading="Tooltip Heading">Tooltip text<disabledText /></tooltip>
  <content guid="a22acfb7-20e0-4723-9321-fd8eb20786e7" group="esri_editing_EditTools_POLYLINE_Tools" />
</tool>
```

##Step 2: Perform edits on feature attributes
* Open the _AttributeButton.cs_ file.
* In our example we are working of the set of selected features in our map view. So the first step is to retrieve the set of currently selected feature IDs and their respective layers.

```c#
// retrieve the currently selected features in the map view
var currentSelectedFeatures = await MappingModule.ActiveMapView.Map.GetSelectionSetAsync();
```

* With the SelectionSet object enumerate through each layer to get the contributing source data source as a table.

```c#
// .. get the underlying table
Table table = await mapMember.GetTableAsync();
```

* In our example we would like to change the attribute value for a string field. Take a look at the extension method to get the first instance of field of a given type. We are using the _**TableDefinition**_ to access the schema of the featureclass. With the list of fields we then using a LINQ query extension and a lambda epxression to get the first field of the requested type. If no field is found, the result will be a null reference.

```c#
public static async Task<Field> GetFieldByTypeAsync(this Table table, FieldType fieldType)
{
    Field foundField = null;

    await QueuingTaskFactory.StartNew(() =>
    {
        IReadOnlyList<Field> fields = ((TableDefinition)table.Definition).Fields;
        foundField = fields.FirstOrDefault(a => a.FieldType == fieldType);
    });
    return foundField;
}
```

* Determine the name of the found field.

```c#
var stringFieldName = stringField != null ? stringField.Name : String.Empty;
```

* For each of the layers (MapMember) define an edit operation. The edit operation should encapsulate the attribute changes.

```c#
var modifyStringsOperation = new EditOperation
{
    Name = String.Format("Modify string field '{0}' in layer {1}.", stringFieldName, mapMember.Name)
};
```

* The modify type of edit operation allows us to specify a _**Dictionary**_ class desribing the fields by name as well as the new value for the attribute. 

```c#
// set up a new dictionary with fields to modify
var modifiedAttributes = new Dictionary<string, object>
{
    // add the name of the string field and the new attribute value to the dictionary
    // in this example a random string is used
    {stringFieldName, Path.GetRandomFileName().Replace(".", "")}
};
```

* With these dictionaries desribing the changes for their respective fields, add the changes into the edit operation

```c#
// put the modify operation on the editor stack
modifyStringsOperation.Modify(mapMember, oid, modifiedAttributes);
```
* Execute the edit operation

```c#
// execute the modify operation to apply the changes
result = await modifyStringsOperation.ExecuteAsync();
```
* Done
