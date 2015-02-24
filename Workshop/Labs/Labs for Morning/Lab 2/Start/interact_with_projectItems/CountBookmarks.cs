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
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Mapping;

namespace interact_with_projectItems
{
    internal class CountBookmarks : Button
    {
        /// <summary>
        /// Represents the button that displays the count of bookmarks for the selected map
        ///<remarks>TO DO: Code this button as a bonus exercise.</remarks>
        /// </summary>
        
        protected override async void OnClick()
        {
            Project project = ProjectModule.CurrentProject; //Gets the current project
            StringBuilder sb = new StringBuilder();
            IList<Bookmark> bmks = null; //to hold the collection of bookmarks
            sb.AppendLine("Unable to determine Bookmark count");
            //TODO - Step 3: Get the MapItem from the project that is selected in the Maps Combo box. 
            // MapProjectItem mapItem.....
            //If Map item is not null, get the bookmarks for that map and display it.

            System.Windows.MessageBox.Show(sb.ToString(), "Bookmark Count");

        }

        /// <summary>
        /// Gets all the bookmarks from a Map Porject item
        /// </summary>
        /// <param name="mpi"></param>
        /// <returns></returns>
        private static async Task<IList<Bookmark>> GetBookmarksAsync(MapProjectItem mpi)
        {
            Map map = null;
            //use the map path to get the actual map (related to the item)
            map = MappingModule.FindMap(mpi.Path);
            if (map == null)
                return null;
            //get the bookmarks associated with the map
            var bookmarks = await map.QueryBookmarksAsync();
            if (bookmarks == null)
                return null;
            else
                return bookmarks;
        }

    }
}
