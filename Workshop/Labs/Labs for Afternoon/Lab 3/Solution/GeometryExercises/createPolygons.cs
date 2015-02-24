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
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Geometry;

namespace GeometryExercisesSolution
{
    internal class createPolygons : Button
    {
        protected override async void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            // retrieve the first line layer in the map
            FeatureLayer lineFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>().Where(
                lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolyline).First();

            // retrieve the first polygon feature layer in the map
            FeatureLayer polygonFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>().Where(
                lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPolygon).First();

            // construct polygon from multipoints
            await constructSamplePolygon(polygonFeatureLayer, lineFeatureLayer);
        }

        /// <summary>
        /// Create sample polygon feature using the point geometries from the multi-point feature using the 
        /// ConvexHull method provided by the GeometryEngine.
        /// </summary>
        /// <param name="polygonLayer">Polygon geometry feature layer used to add the new feature.</param>
        /// <param name="lineLayer">The polyline feature layer containing the features used to construct the polygon.</param>
        /// <returns></returns>
        private Task<bool> constructSamplePolygon(FeatureLayer polygonLayer, FeatureLayer lineLayer)
        {

            // execute the fine grained API calls on the CIM main thread
            return QueuingTaskFactory.StartNew(() =>
            {
                // get the underlying feature class for each layer
                var polygonFeatureClass = polygonLayer.GetTableAsync().Result as FeatureClass;
                var lineFeatureClass = lineLayer.GetTableAsync().Result as FeatureClass;

                // construct a cursor to retrieve the line features
                var lineCursor = lineFeatureClass.Search(null, false);

                // retrieve the feature class schema information for the feature class
                var polygonDefinition = polygonFeatureClass.Definition as FeatureClassDefinition;
                var polylineDefinition = lineFeatureClass.Definition as FeatureClassDefinition;

                // set up the edit operation for the feature creation
                var createOperation = EditingModule.CreateEditOperation();
                createOperation.Name = "Create polygons";

                List<CoordinateCollection> combinedCoordinates = new List<CoordinateCollection>();

                while (lineCursor.MoveNext())
                {
                    // retrieve the first feature
                    var lineFeature = lineCursor.Current as Feature;

                    // add the coordinate collection of the current geometry into our overall list of collections
                    var polylineGeometry = lineFeature.Shape as Polyline;
                    combinedCoordinates.AddRange(polylineGeometry.Paths);
                }

                // use the ConvexHull method from the GeometryEngine to construct the polygon geometry
                var newPolygon = Polygon.Clone(GeometryEngine.ConvexHull(new Polyline(combinedCoordinates,
                    lineFeatureClass.SpatialReference))) as Polygon;

                // queue the polygon creation
                createOperation.Create(polygonLayer, newPolygon);

                // execute the edit (polygon create) operation
                var t = createOperation.ExecuteAsync().Result;

                // save the edits
                return EditingModule.SaveEditsAsync();
            });
        }
    }
}
