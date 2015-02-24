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
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Geometry;

namespace GeometryExcercises
{
    internal class createPoints : Button
    {
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

        /// <summary>
        /// Create random sample points in the extent of the spatial reference
        /// </summary>
        /// <param name="pointLayer">Point geometry feature layer used to the generate the points.</param>
        /// <returns></returns>
        private void constructSamplePoints(FeatureLayer pointFeatureLayer)
        {
            // create a random number generator
            var randomGenerator = new Random();
			
	    // the database and geometry interactions are considered fine-grained and must be executed on
            // the main CIM thread
            //return QueuingTaskFactory.StartNew(() =>
            //{

            //Envelope areaOfInterest  = MappingModule.ActiveMapView.GetExtentAsync().Result;

            // get the feature class and its definition

            // start an edit operation to create new (random) point features
            var createOperation = EditingModule.CreateEditOperation();
            createOperation.Name = "Generate points";

            // create 20 new point geometries and queue them for creation
            for (int i = 0; i < 20; i++)
            {
                // use random.NextCoordinate in the construction of the MapPoint

                // queue feature creation
                //createOperation.Create(pointLayer, newMapPoint);
            }

            // execute the operation

            // save the edits
	    //return EditingModule.SaveEditsAsync().Result;
            //});
        }
    }
}
