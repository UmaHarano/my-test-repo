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
using ArcGIS.Desktop.Editing;
using System.IO;
using ArcGIS.Desktop.Editing.Attributes;

namespace ConstructionToolSolution
{
    /// <summary>
    /// A button implementation showing how to modify attributes of a selected feature.
    /// </summary>
    internal class AttributeEditButton : Button
    {
        protected override async void OnClick()
        {
            await PerformAttributeChange();
        }

        /// <summary>
        /// This function takes the selected features in the map view, finds the first field of type string in the each feature class
        /// and modifies the attribute value to a random string.
        /// </summary>
        /// <returns>Indicator if the edit operation was successful.</returns>
        private async Task<bool> PerformAttributeChange()
        {
            // retrieve the currently selected features in the map view
            var currentSelectedFeatures = await MappingModule.ActiveMapView.Map.GetSelectionSetAsync();

            var result = false;

            // for each of the map members in the selected layers
            foreach (var mapMember in currentSelectedFeatures.MapMembers)
            {
                // .. get the underlying table
                Table table = await mapMember.GetTableAsync();

                // retrieve the first field of type string
                var stringField = await table.GetFieldByTypeAsync(FieldType.String);
                var stringFieldName = stringField != null ? stringField.Name : String.Empty;

                // check if the returned string of the field name actually contains something
                // meaning if the current MapMember actually contains a field of type string
                if (String.IsNullOrEmpty(stringFieldName))
                    continue;

                #region Use edit operations for attribute changes
                // create a new edit operation to encapsulate the string field modifications
                var modifyStringsOperation = new EditOperation
                {
                    Name = String.Format("Modify string field '{0}' in layer {1}.", stringFieldName, mapMember.Name)
                };

                // with each ObjectID of the selected feature
                foreach (var oid in currentSelectedFeatures[mapMember])
                {
                    // set up a new dictionary with fields to modify
                    var modifiedAttributes = new Dictionary<string, object>
                        {
                            // add the name of the string field and the new attribute value to the dictionary
                            // in this example a random string is used
                            {stringFieldName, Path.GetRandomFileName().Replace(".", "")}
                        };

                    // put the modify operation on the editor stack
                    modifyStringsOperation.Modify(mapMember, oid, modifiedAttributes);
                }

                // execute the modify operation to apply the changes
                result = await modifyStringsOperation.ExecuteAsync();
                #endregion

                #region Use the feature inspector for attribute changes
                //// as an alternative approach
                //// use the feature inspector class
                //FeatureInspector featureInspector = new FeatureInspector(true);

                //// fill the feature inspector with the oids from the feature layer
                //await featureInspector.FillAsync(mapMember, currentSelectedFeatures[mapMember]);

                //// change the attribute value for the string field
                //featureInspector[stringFieldName] = Path.GetRandomFileName().Replace(".", "");

                //// appl the new values
                //result = await featureInspector.ApplyAsync();
                #endregion
            }

            return result;
        }
    }
}
