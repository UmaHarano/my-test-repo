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
    internal class createPolylines : Button
    {
        protected override void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            FeatureLayer pointFeatureLayer = null;
            FeatureLayer polylineFeatureLayer = null;
            // TODO
            // retrieve the point layer from the active map
            // retrieve the polyline layer from the active map


            // TODO
            // construct polylines from features in point layer
            // be sure to await
            constructSamplePolylines(polylineFeatureLayer, pointFeatureLayer);
        }
        
        // TODO: change the method declaration to return Task of bool
        private void constructSamplePolylines(FeatureLayer polylineLayer, FeatureLayer pointLayer)
        {
            // TODO
            // execute the fine grained API calls on the CIM main thread
            //return QueuingTaskFactory.StartNew(() => {
            // get the feature classes and their definitions
            // get the underlying feature class for each layer
            var polylineFeatureClass = polylineLayer.GetTableAsync().Result as FeatureClass;
            var pointFeatureClass = pointLayer.GetTableAsync().Result as FeatureClass;

            FeatureClassDefinition polylineDefinition = null;
            FeatureClassDefinition pointDefinition = null;

            // construct a cursor for all point features, since we want all feature there is no
            // QueryFilter required
            var pointCursor = pointFeatureClass.Search(null, false);

            // set up the edit operation for the feature creation
            var createOperation = EditingModule.CreateEditOperation();
            createOperation.Name = "Create polylines";
            
            // initialize a counter variable
            int pointCounter = 0;
            // initialize a list to hold 5 coordinates that are used as vertices for the polyline
            var lineCoordinates = new List<Coordinate>(5);

            // with the first 20 points create polylines with 5 vertices each
            // be aware that the spatial reference between point geometries and line geometries is different
            // HINT: use GeometryEngine.Project
            
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
                    createOperation.Create(polylineLayer, newPolyline);
                    lineCoordinates = new List<Coordinate>(5);
                }
            }

            // TODO
            // execute the edit (create) operation
            //HINT use createOperation.... execute async

            // TODO
            // save the edits
            //return EditingModule.SaveEditsAsync();
            //});
        }
    }
}
