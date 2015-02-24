## Lab 3: Working with the Core.Geometry and Core.Data API

###What you learn in this lab
* How to construct and manipulate geometries
* How to use GeometryEngine functionality
* How to search and retrieve features
* How to use edit operations to persist feature edits

*******
* [Step 1: Create random point features](#step-1-create-random-point-features)
* [Step 2: Create polyline features from points](#step-2-create-polyline-features-from-points)
* [Step 3: Create polygon feature from polylines](#step-3-create-polygon-feature-from-polylines)

**Estimated completion time: 30 minutes**
****

##Step 1: Create random point features
>>>>THOMAS...let's add this package (GeometryDataAPI.pkgx) to the git....if that is a problem then let us assume that we will have a folder already on each individual student's hard drive like C:\Workshop\Data where data, project files, etc, will be located
<<<<
* Download the GeometryDataAPI.pkgx from ArcGIS Online and save the project.
* Open the _GeometryExercises_ solution in the (afternoon) Lab3 start folder in Visual Studio
* Open the _createPoints.cs_ file from the solution explorer
* In the _OnClick()_ function for the button look for ```// TODO  retrieve the point layer```

```c#
protected override void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            FeatureLayer pointFeatureLayer = null;
            // TODO
            // retrieve the point layer

            // construct 20 random map point in this layer
            constructSamplePoints(pointFeatureLayer);
        }
```

* Use this code to retrieve the point feature layer from the active map (this underlying feature class for the layer is used as the target for our generated points).

```c#
// retrieve the first point layer in the map
FeatureLayer pointFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>().Where(
  lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint).First();
```

* With the variable of ```pointFeatureLayer``` as the target continue to the ```private void constructSamplePoints(FeatureLayer pointFeatureLayer)``` function to construct the points.

* Change the method declaration to this:

```C#
private Task<bool> constructSamplePoints(FeatureLayer pointLayer) {
```

Note the **Task<bool>**. We are going to be implementing an asynchronous function

* Find, and uncomment this statement in the constructSamplePoints method. Read **and understand** the comment...

```C#

// the database and geometry interactions are considered fine-grained and must be executed on
// the main CIM thread
//return QueuingTaskFactory.StartNew(() =>
//{
            
```

* Find, and uncomment this statement at the bottom of the method. This completes the QueuingTaskFactory closure (and provides the correct return type for the function)

```C#

  // save the edits
	//return EditingModule.SaveEditsAsync().Result;
  //});
```

**All of the following code should be added <u>inside</u> the QTF lambda**

* Uncomment the following line to retrieve the extent (of the active map view) in which the random points will be generated.

```c#
//Envelope areaOfInterest  = MappingModule.ActiveMapView.GetExtentAsync().Result;
```

* Get the data source from the feature layer and cast it to a _**FeatureClass**_.

```c#
// get the feature class associated with the layer
var featureClass = pointLayer.GetTableAsync().Result as FeatureClass;
```

* Next retrieve the schema definition from the feature class.

```c#
    // retrieve the class definition of the point feature class
    var classDefinition = featureClass.Definition as FeatureClassDefinition;
  
```

* Define an edit operation that will facilitate the creation of the point features. Edit operations are a logical unit that becomes part of the undo/redo stack.

```c#
// start an edit operation to create new (random) point features
var createOperation = EditingModule.CreateEditOperation();
createOperation.Name = "Generate points";
```

* Generate 20 point geometries using the provided extension method for _**Random**_ class and add the create instruction to the stack of the edit operation.

```c#

for (int i = 0; i < 20; i++) {
   ...
   ...
   MapPoint newMapPoint = new MapPoint(randomGenerator.NextCoordinate(areaOfInterest, false), spatialReference);
   ...
   // queue feature creation
   createOperation.Create(pointLayer, newMapPoint);
}

```

* Once the new features are queued as part of the edit operation, **execute** the operation itself.

```c#
// execute the edit (feature creation) operation
var t = createOperation.ExecuteAsync().Result;
```

(Note, the "var t =" is not important. It simply allows us to use the .Result property of the task and avoid setting up a continuation with await)

* The edits are saved to the underlying workspace here:
 
```c#
return EditingModule.SaveEditsAsync().Result;
```


##Step 2: Create polyline features from points
* Open the _createPolylines.cs_ file. in Visual Studio solution explorer
* Retrieve the point feature layer and the line feature layer from the active map. We will use the features and their respective geometries from the point layer as vertices for the polylines. Refer to the code in [Step 1](#step-1-create-random-point-features) on finding the layer.

Remember?:

```C#
protected override async void OnClick() {
   ...
   // retrieve the first point layer in the map
   FeatureLayer pointFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>()....

```

Hint: point layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint``` and polyline layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline```

* Continue to the _constructSamplePolylines_ function retrieving the features from the point layer and constructing the geometries for the polyline features.
* Be aware that the interactions for _**ArcGIS.Core.Geometry**_ and _**ArcGIS.Core.Data**_ are executed on the CIM main thread.

* As in step1, change the signature of the constructSamplePolylines() function to this (note the return ```Task<bool>```):

```C#
private Task<bool> constructSamplePolylines(FeatureLayer polylineLayer, FeatureLayer pointLayer)
```

* Uncomment the QTF block and the return statement at the bottom of the function:

```C#

// TODO
// execute the fine grained API calls on the CIM main thread
//return QueuingTaskFactory.StartNew(() => {

// TODO
// save the edits
//return EditingModule.SaveEditsAsync();
//});

```

* Get the features by creating a cursor on the point feature class. In our example we don't need to specify a _**QueryFilter**_ and use the null keyword to fill the cursor with all features.

```c#
var pointCursor = pointFeatureClass.Search(null, false);
```

* Use five point geometries in the construction of a polyline as outlined by the code. For each five point geometry use a list of coordinates to construct the polyline.

* Be aware that the spatial reference between the point feature layer and the line feature layer is different. Use the _**GeometryEngine**_ class for the re-projection of the geometry.

```c#
// initialize a counter variable
int pointCounter = 0;
// initialize a list to hold 5 coordinates that are used as vertices for the polyline
var lineCoordinates = new List<Coordinate>(5);

// with the 20 points create polylines with 5 vertices each
// be aware that the spatial reference between point geometries and line geometries is different

// loop through the point features
while (pointCursor.MoveNext())
{
    pointCounter++;

    // TODO
    // add the feature point geometry as a coordinate into the vertex list of the line
    //var pointFeature = ... use the current property of the cursor

    // - ensure that the projection of the point geometry is converted to match the spatial reference of the line
    //MapPoint pt = GeometryEngine.Project(pointFeature.Shape, polylineDefinition.SpatialReference) as MapPoint;
    //lineCoordinates.Add(pt.Coordinate);

    // for every 5 geometries, construct a new polyline and queue a feature create
    if (pointCounter % 5 == 0)
    {
        var newPolyline = new Polyline(lineCoordinates, polylineDefinition.SpatialReference);
        createOperation.Create(polylineFeatureLayer, newPolyline);
        lineCoordinates = new List<Coordinate>(5);
    }
}

```

* Execute the queued creates on the edit operation and save the edits. Use your createOperation instance.

##Step 3: Create polygon feature from polylines
* Open the _createPolygons.cs_ file in the Visual Studio Solution Explorer


* Retrieve the line feature layer and the polygon feature layer from the active map. Refer to the code in [Step 1](#step-1-create-random-point-features) on finding the layer.

Remember?:

```C#
protected override async void OnClick() {
   ...
   // retrieve the first line layer in the map
   FeatureLayer lineFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>()...

```

Hint: line layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline``` and polygon layers have shape type ```ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolygon```

* As in step1 and step2, change the signature of the constructSamplePolygon() function to this (note the return ```Task<bool>```):

```C#
private Task<bool> constructSamplePolygon(FeatureLayer polygonLayer, FeatureLayer lineLayer)
```

* Uncomment the QTF block and the return statement at the bottom of the function:

```C#

// TODO
// execute the fine grained API calls on the CIM main thread
//return QueuingTaskFactory.StartNew(() => {

// TODO
// save the edits
//return EditingModule.SaveEditsAsync();
//});

```

* Note: we retrieve the polygon feature class and the polyline feature class from the layers on active map. We will use the features and their respective geometries from the line feature class to construct a polygon.

```C#

var polygonFeatureClass = polygonLayer.GetTableAsync().Result as FeatureClass;
var lineFeatureClass = lineLayer.GetTableAsync().Result as FeatureClass;
```

* Note that we get the features by creating a cursor on the line feature class. In our example we don't need to specify a _**QueryFilter**_ and use the null keyword to fill the cursor with all features.

```C#

// construct a cursor to retrieve the line features
var lineCursor = lineFeatureClass.Search(null, false);
```

* When iterating through the line features to get the geometries store the _CoordinateCollection_ in a IEnumerable class. We will use this entity when we create the convex hull.

```c#
List<CoordinateCollection> combinedCoordinates = new List<CoordinateCollection>();

 while (lineCursor.MoveNext()) {
    // TODO
    // add the feature geometry into the overall list of coordinate collections
    // retrieve the first feature
    //var lineFeature = ... use the current property of the cursor

    // add the coordinate collection of the current geometry into our overall list of collections
    //var polylineGeometry = lineFeature.Shape as Polyline;
    //combinedCoordinates.AddRange ... Hint: we want the part collection of the polyline
 }
            
```

* Use the combined coordinates to create a polyline feature.

```C#
//var polyLine = new Polyline(..., lineFeatureClass.SpatialReference)
```

* Use the GeometryEngine.ConvexHull method to construct the geometry for the polygon feature using the polyline
as its input.

```C#
//var geom = GeometryEngine.ConvexHull(...
```

* Create the polygon by cloning the convex hull geometry

```C#
//var newPolygon = Polygon.Clone(...
```

* Execute the create feature operation and store the edits.
