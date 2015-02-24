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

using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interact_with_projectItems
{
    /// <summary>
    /// Represents the comboBox that holds the collection of Maps in the project.
    /// </summary>

    internal class MapViewsComboBox : ComboBox
    {
        /// <summary>
        /// Constructor to initialize the ComboBox        
        /// </summary>
        public MapViewsComboBox()
        {
            //TODO Step 2 - Subscribe OnProjectCollectionChanged to the Project Items changed event
            //Hint: Use ArcGISProjectItemsChangedEvents.Subscribe ...
            UpdateCombo(); //Updates the comboBox with the Maps.
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ MapViewsComboBox()
        {
            //TODO Step 2 - //Unsubscribe to the Project Items changed event
            
            
        }

        /// <summary>
        /// Updates the combo box with all the maps in the current project.
        /// </summary>
        /// 
        private void UpdateCombo()
        {
            MapContainer _mapContainer;

            // we are only interested in map project items therefore we specify the "Map" selection parameter
            _mapContainer = ProjectModule.CurrentProject.GetProjectItemContainer<MapContainer>("Map");

            if (_mapContainer != null) //check if the MapContainer is null first
            {
                foreach (var projectItem in _mapContainer) //iterate through the map container to get all the maps.
                {
                    Map map = MappingModule.FindMap(projectItem.Path); // Get the "Map"  from the MapProjectItem
                    if (map != null)
                        Add(new ComboBoxItem(map.Name));         
                    else
                        Add(new ComboBoxItem(String.Empty));   
                }
                var selectedItem = ItemCollection.FirstOrDefault();
                Enabled = true; //enables the ComboBox

                if (selectedItem != null) //Setting the Selected item.
                {
                    SelectedItem = selectedItem;
                }
                else
                {
                    SelectedItem = null;
                }
            }                     
        }
        
        /// <summary>
        /// The OnProjectCollectionChanged event handler is called each time there is a change
        /// in the projectItem collection (either adding or deleting a project item).
        /// </summary>
        /// <param name="projectItemEventArgs"></param>
        private void OnProjectCollectionChanged(ArcGISProjectItemsChangedEventArgs projectItemEventArgs)
        {
            //TODO Step 2 - Code the OnProjectCollectionChanged method.
            if (projectItemEventArgs.ProjectItem is MapProjectItem) { //it is a Map
                //switch (projectItemEventArgs.Action) {
                //    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                //}
            }
        }

        /// <summary>
        /// The on comboBox selection change event
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override void OnSelectionChange(ComboBoxItem item)
        {
            Module1.MapSelected = ((ComboBoxItem)SelectedItem).Text; //setting the property in the Module to hold the selected map.
        }

    }
}
