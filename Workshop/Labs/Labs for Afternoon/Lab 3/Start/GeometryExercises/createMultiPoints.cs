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
    internal class createMultiPoints : Button
    {
        protected override void OnClick()
        {
            // to work in the context of the active display retrieve the current map 
            Map activeMap = MappingModule.ActiveMapView.Map;

            FeatureLayer multiPointFeatureLayer = null;
            // TODO
            // retrieve the multi point layer


            // TODO
            // construct a multipoint feature in this layer
            constructSampleMultiPoints(multiPointFeatureLayer);
        }

        /// <summary>
        /// Create a single multi-point feature that is comprised of 20 points.
        /// </summary>
        /// <param name="multiPointLayer">Multi-point geometry feature layer used to add the multi-point feature.</param>
        /// <returns></returns>
        private void constructSampleMultiPoints(FeatureLayer multiPointFeatureLayer)
        {
            // create a random number generator
            var randomGenerator = new Random();

            //var areaOfInterest = await MappingModule.ActiveMapView.GetExtentAsync();

            // get the feature class and its definition

            // start an edit operation to create new (random) multi-point feature
            var createOperation = EditingModule.CreateEditOperation();
            createOperation.Name = "Generate multi-point";

            //create a list to hold the 20 coordinates of the multi-point feature
            IList<Coordinate> coordinateList = new List<Coordinate>(20);

            for (int i = 0; i < 20; i++)
            {
                // add the random coordinate to the list
                // use random.NextCoordinate()
            }

            // execute the operation

            // save the edits
        }
    }
}
