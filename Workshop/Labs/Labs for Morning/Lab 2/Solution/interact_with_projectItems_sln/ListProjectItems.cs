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

namespace interact_with_projectItems_sln {
    internal class ListProjectItems : Button {
        protected override void OnClick() {
            Project project = ProjectModule.CurrentProject;
            StringBuilder sb = new StringBuilder();

            //Create the list of project item container names
            string[] ProjectItemContainerNames = new string[]
            {
                "Map",
                "GDB",
                "Styles",
                "GP",
                "FolderConnection",
                "Layout"                  
            };
            //for each name, get its corresponding container
            foreach (var name in ProjectItemContainerNames) {
                var ProjContainer = project.GetProjectItemContainer<IProjectItemContainer>(name);
                sb.AppendLine(ProjContainer.Name + "\r\n============================");

                //for each container, get its items
                foreach (IProjectItem item in ProjContainer) {
                    sb.AppendLine(item.Name);
                }

                sb.AppendLine("\r\n");
            }
            System.Windows.MessageBox.Show(sb.ToString(), "Project Items");
        }
    }
}
