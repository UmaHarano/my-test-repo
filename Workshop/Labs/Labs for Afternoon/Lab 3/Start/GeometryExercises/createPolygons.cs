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
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Geometry;

namespace GeometryExcercises
{
    internal class createPolygons : Button
    {
        protected override void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            // TODO
            // retrieve the first polygon feature layer in the map
            FeatureLayer polygonFeatureLayer = null;
            FeatureLayer lineFeatureLayer = null;

            // construct polygon from multipoints
            //TODO await the async version
            constructSamplePolygon(polygonFeatureLayer, lineFeatureLayer);
        }

        /// <summary>
        /// Create sample polygon feature using the point geometries from the multi-point feature using the 
        /// ConvexHull method provided by the GeometryEngine.
        /// </summary>
        /// <param name="polygonLayer">Polygon geometry feature layer used to add the new feature.</param>
        /// <param name="lineLayer">The polyline feature layer containing the features used to construct the polygon.</param>
        /// <returns></returns>
        private void constructSamplePolygon(FeatureLayer polygonLayer, FeatureLayer lineLayer)
        {

            // execute the fine grained API calls on the CIM main thread
            //return QueuingTaskFactory.StartNew(() =>
            //{
            // get the feature classes and their definitions
            // get the underlying feature class for each layer
            var polygonFeatureClass = polygonLayer.GetTableAsync().Result as FeatureClass;
            var lineFeatureClass = lineLayer.GetTableAsync().Result as FeatureClass;
            FeatureClassDefinition polygonDefinition = null;
            FeatureClassDefinition polylineDefinition = null;

            // construct a cursor to retrieve the line features
            var lineCursor = lineFeatureClass.Search(null, false);

            // set up the edit operation for the feature creation
            var createOperation = EditingModule.CreateEditOperation();
            createOperation.Name = "Create polygons";

            // construct the polygon geometry from the convex hull of the combined polyline features
            // by contructing a convex hull
            // HINT:  GeometryEngine.ConvexHull
            List<CoordinateCollection> combinedCoordinates = new List<CoordinateCollection>();

            while (lineCursor.MoveNext())
            {
                // TODO
                // add the feature geometry into the overall list of coordinate collections
                // retrieve the first feature
                //var lineFeature = ... use the current property of the cursor

                // add the coordinate collection of the current geometry into our overall list of collections
                //var polylineGeometry = lineFeature.Shape as Polyline;
                //combinedCoordinates.AddRange ... Hint: we want the part collection of the polyline
            }

            // TODO
            //construct a polygon geometry from the convex hull of the polyline
            //constructed from the list of coordinate collections combined from all the polylines
            //var polyLine = new Polyline(..., lineFeatureClass.SpatialReference)
            //var geom = GeometryEngine.ConvexHull(...
            //var newPolygon = Polygon.Clone(...

            // TODO 
            // specify the create edit operation

            // TODO
            // execute the operation 

            // TODO
            // save the edits
            //return EditingModule.SaveEditsAsync();
            //});
        }
    }
}
