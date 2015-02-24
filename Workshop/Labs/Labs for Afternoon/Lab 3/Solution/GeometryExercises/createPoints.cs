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
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Mapping;

namespace GeometryExercisesSolution
{
    internal class createPoints : Button
    {
        protected override async void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            // retrieve the first point layer in the map
            FeatureLayer pointFeatureLayer = activeMap.GetFlattenedLayers().OfType<FeatureLayer>().Where(
                lyr => lyr.ShapeType == ArcGIS.Core.CIM.esriGeometryType.esriGeometryPoint).First();

            // first generate some random points
            await constructSamplePoints(pointFeatureLayer);
        }

        /// <summary>
        /// Create random sample points in the extent of the spatial reference
        /// </summary>
        /// <param name="pointLayer">Point geometry feature layer used to the generate the points.</param>
        /// <returns>Task of bool</returns>
        private Task<bool> constructSamplePoints(FeatureLayer pointLayer)
        {
            // create a random number generator
            var randomGenerator = new Random();

            // the database and geometry interactions are considered fine-grained and must be executed on
            // the main CIM thread
            return QueuingTaskFactory.StartNew(() =>
            {
		// get the feature class associated with the layer
                var featureClass = pointLayer.GetTableAsync().Result as FeatureClass;
                // retrieve the class definition of the point feature class
                var classDefinition = featureClass.Definition as FeatureClassDefinition;

                // store the spatial reference as its own variable
                SpatialReference spatialReference = featureClass.SpatialReference;

                // define an area of interest. Random points are generated in the allowed
                // confines of the allow extent range
                var areaOfInterest = MappingModule.ActiveMapView.GetExtentAsync().Result;

                // start an edit operation to create new (random) point features
                var createOperation = EditingModule.CreateEditOperation();
                createOperation.Name = "Generate points";

                // create 20 new point geometries and queue them for creation
                for (int i = 0; i < 20; i++)
                {
                    MapPoint newMapPoint = null;

                    // generate either 2D or 3D geometries
                    if (classDefinition.HasZ)
                        newMapPoint = new MapPoint(randomGenerator.NextCoordinate(areaOfInterest, true), spatialReference);
                    else
                        newMapPoint = new MapPoint(randomGenerator.NextCoordinate(areaOfInterest, false), spatialReference);

                    // queue feature creation
                    createOperation.Create(pointLayer, newMapPoint);
                }

                // execute the edit (feature creation) operation
                var t = createOperation.ExecuteAsync().Result;
		return EditingModule.SaveEditsAsync().Result;
            });
        }
    }
}
