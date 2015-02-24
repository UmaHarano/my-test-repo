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
using System.Windows;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.EditTools;
using ServiceContracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;

namespace EditTool
{
    internal class CutWithoutSelection : CreateSketchTool
    {
        // TODO
        // adjust the daml such that the tool is listed in the gallery under the DevSummit category
        public CutWithoutSelection()
        {
            // select the type of construction tool you wish to implement.  
            // Make sure that the tool is correctly registered with the correct component category type in the daml
            SketchType = SketchGeometryType.Point;
            // SketchType = SketchGeometryType.Line;
            // SketchType = SketchGeometryType.Polygon;
        }

        /// <summary>
        /// We are being activated. Our tool has been selected on the UI
        /// </summary>
        /// <returns>Task</returns>
        protected override Task OnActivateImpl()
        {
            FrameworkApplication.State.Activate("esri_editing_CreatingFeature");
            return StartSketch(CurrentTemplate);
        }

        /// <summary>
        /// We are being deactivated (typically because a different tool has been selected)
        /// </summary>
        /// <returns>Task</returns>
        protected override Task OnDeactivateImpl()
        {
            FrameworkApplication.State.Deactivate("esri_editing_CreatingFeature");
            return Task.FromResult(0);
        }

        /// <summary>
        /// Called when the geometry is to be added to an edit operation.  If you need to manipulate the geometry, 
        /// this is the function to perform that manipulation
        /// </summary>
        /// <returns>Task{IEnumerable{Geometry}}</returns>
        protected override Task<IEnumerable<Geometry>> Transform(Geometry geometry)
        {
            return Task.FromResult<IEnumerable<Geometry>>(new List<Geometry>() { geometry });
        }

        /// <summary>
        /// This is a basic FinishSketch method which illustrates the process of using the sketch geometry for feature creation. 
        ///   1. Create edit operation
        ///   2. Use the sketch geometry to perform a spatial query
        ///   3. Use the found features and use them to set up a cut operation
        ///   3. Execute the edit operation
        ///   
        /// Any construction tool will execute successfully with this routine commented out (the default implementation is 
        /// supplied by the Editor framework).   Uncomment this method if you wish to add your own additional implementation details.
        /// </summary>
        /// <returns>Task{bool}</returns>
        protected override Task<bool> OnFinishSketch(Geometry geometry, Dictionary<string, object> attributes)
        {
            return QueuingTaskFactory.StartNew(() => ExecuteCut(CurrentTemplate, geometry, attributes));
        }
        protected async Task<bool> ExecuteCut(EditingTemplate template, Geometry geometry, Dictionary<string, object> attributes)
        {
            if (template == null)
                return false;
            if (geometry == null)
                return false;

            // create an edit operation
            EditOperation op = EditingModule.CreateEditOperation();
            op.Name = "Cut Elements";
            op.ProgressMessage = "Working...";
            op.CancelMessage = "Operation canceled.";
            op.ErrorMessage = "Error cutting polygons";
            op.SelectModifiedFeatures = false;
            op.SelectNewFeatures = false;

            // get the feature class associated with the layer
            Table fc = await template.Layer.GetTableAsync();

            // initialize a list of ObjectIDs that need to be cut
            var cutOIDs = new List<long>();

            // on a separate thread
            await QueuingTaskFactory.StartNew(async () =>
            {
                // TODO
                // find the features crossed by the sketch geometry
                RowCursor rc = await fc.SearchAsync(geometry, SpatialRelationship.Crosses);

                // add the feature IDs into our prepared list
                while (rc.MoveNext())
                {
                    var feature = rc.Current as Feature;

                    if (feature == null)
                        break;

                    if (feature.Shape != null)
                    {
                        // we are interested in the intersection points
                        // in case there is only one intersection then the sketch geometry doesn't enter and leave the 
                        // base geometry and the cut operation won't work.
                        Geometry intersectionGeometry = GeometryEngine.Intersection(feature.Shape, geometry, GeometryDimension.esriGeometry0Dimension);
                        if (intersectionGeometry is MultiPoint)
                        {
                            //var intersectionPoints = intersectionGeometry as MultiPoint;
                            //// we are only interested in feature IDs where the count of intersection points is larger than 1
                            //// i.e., at least one entry and one exit
                            //if (intersectionPoints.Coordinates.Count > 1)
                            //{
                            //    // add the current feature to the overall list of features to cut
                            //    cutOIDs.Add(rc.Current.ObjectID);
                            //}
                        }
                    }
                }
            });

            // add the elements to cut into the edit operation
            op.Cut(template.Layer, cutOIDs, geometry);

            //execute the operation
            bool operationResult = await op.ExecuteAsync();


            return operationResult;
        }
    }
}
