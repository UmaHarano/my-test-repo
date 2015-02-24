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
using ArcGIS.Desktop.Editing;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace EditToolSolution
{
    internal class DensifyButton : Button
    {
        protected override void OnClick()
        {
            ExecuteDensify();
        }

        /// <summary>
        /// Asynchronous task that splits the current sketch geometry into 100 segments of equal length.
        /// </summary>
        /// <returns>Task{bool}</returns>
        private async Task<bool> ExecuteDensify()
        {
                // get the current sketch geometry from the editing module
             var sketchGeometry = await EditingModule.GetSketchGeometryAsync();

            // check if geometry is valid
             bool isGeometryValid = await QueuingTaskFactory.StartNew<bool>(() =>
                 {
                     bool validGeometry = true;

                     if (sketchGeometry == null || sketchGeometry.IsEmpty)
                         validGeometry = false;

                     return validGeometry;
                 });

            if (!isGeometryValid)
                return false;

            // get the currently selected features from the map
            // the selected feature uses the above selected geometry
            var currentlySelectedFeature = await MappingModule.ActiveMapView.Map.GetSelectionSetAsync();

            // set up an edit operation
            EditOperation editOperation = new EditOperation();
            editOperation.Name = "Densify selected geometry";

            Geometry densifiedGeometry = null;

            // modify the geometry in the sketch geometry
            // the geometry operation needs to run as it own task
            await QueuingTaskFactory.StartNew(() =>
            {
                // compute a length fur the current geometry when divided into 100 segments
                var segmentLength = GeometryEngine.Length(sketchGeometry) / 100;
                // compute a new densified geometry 
                densifiedGeometry = GeometryEngine.Densify(sketchGeometry, segmentLength);
            });

            // for the currently selected feature go through the layers to which the feature belongs
            foreach (var mapMember in currentlySelectedFeature.MapMembers)
            {
                // for each of the selections in the layer (map member)
                foreach (var selectedOID in currentlySelectedFeature[mapMember])
                {
                    // provide the densified geometry for the selected feature as part of the modify operation 
                    editOperation.Modify((Layer)mapMember, selectedOID, densifiedGeometry);
                }
            }

            // execute the edit operation and return
            return await editOperation.ExecuteAsync();
        }
    }
}
