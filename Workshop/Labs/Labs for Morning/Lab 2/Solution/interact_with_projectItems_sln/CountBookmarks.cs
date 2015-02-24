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

namespace interact_with_projectItems_sln {
    internal class CountBookmarks : Button {
        protected override async void OnClick() {
            Project project = ProjectModule.CurrentProject;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Unable to determine Bookmark count");
            MapProjectItem mapItem = project.GetMaps().FirstOrDefault(pi => pi.Name == Module1.MapSelected);
            if (mapItem != null) {
                var map = MappingModule.FindMap(mapItem.Path);
                if (map == null) //map has not been loaded yet
                    return;

                var bmks = await map.QueryBookmarksAsync(); //get the bookmarks for each map.
                if (bmks != null) {
                    sb.Clear();
                    sb.AppendLine(mapItem.Name + " map has " + bmks.Count + " bookmarks\n");
                }
            }

            System.Windows.MessageBox.Show(sb.ToString(), "Bookmark Count");
        }
    }
}
