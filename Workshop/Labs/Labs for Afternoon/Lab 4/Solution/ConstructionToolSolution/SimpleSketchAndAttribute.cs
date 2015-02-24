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

namespace ConstructionToolSolution
{
    internal class SimpleSketchAndAttribute : CreateSketchTool
    {
        public SimpleSketchAndAttribute()
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
            // use this method to perform actions such as advising to the editor events
            return base.OnActivateImpl();
        }

        /// <summary>
        /// We are being deactivated (typically because a different tool has been selected)
        /// </summary>
        /// <returns>Task</returns>
        protected override Task OnDeactivateImpl()
        {
            // use this method to clean up such as unadvising from the editor events
            return base.OnDeactivateImpl();
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

        // <summary>
        // This is a basic FinishSketch method which illustrates the process of feature creation. 
        //   1. Create edit operation
        //   2. Call CreateTemplateFeatures which links to the Transform callback method
        //   3. Execute the edit operation
        //   
        // Any construction tool will execute successfullly with this routine commented out (the default implementation is 
        // supplied by the Editor framework).   Uncomment this method if you wish to add your own additional implementation details.
        // </summary>
        // <returns>Task<bool></returns>
        //protected override async Task<bool> FinishSketch(Geometry geometry, Dictionary<string, object> attributes)
        //{
        //    if (CurrentTemplate == null)
        //        return false;
        //    if (geometry == null)
        //        return false;

        //    // create an edit operation
        //    var op = await CreateEditOperation();
        //    op.Name = string.Format("Create Segments '{0}'", this.CurrentTemplate.Layer.Name);
        //    op.ProgressMessage = "Working...";
        //    op.CancelMessage = "Operation canceled";
        //    op.ErrorMessage = "Error creating segments";
        //    op.SelectNewFeatures = EditingModule.SelectNewlyCreatedFeatures;

        //    // make the features. We will be called back on our Transform method
        //    await CreateTemplateFeatures(op, CurrentTemplate, geometry);

        //    //execute the operation
        //    await op.Do();
        //    return op.Succeeded;
        //}
    }
}
