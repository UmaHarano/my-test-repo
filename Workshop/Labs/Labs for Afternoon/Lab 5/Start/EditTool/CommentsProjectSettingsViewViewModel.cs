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
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;

namespace EditTool
{
    internal class CommentsProjectSettingsViewViewModel : Page
    {
        private bool _generateComments;

        public bool GenerateComments
        {
            get
            {
                return _generateComments;
            }
            set
            {
                _generateComments = value;

                // TODO
                // synchronize the view model with the state of the add-in module

                // attach the edit event to the current map view
                //EditToolModule.AttachEditEvents(MappingModule.ActiveMapView.Map);

                // update the view
                SetProperty(ref _generateComments, value, () => GenerateComments);
                IsModified = true;
            }
        }

        protected override Task InitializeAsync()
        {
            // TODO
            // retrieve the state from the add-in module

            // TODO
            // set the state in the view model

            return Task.FromResult(0);
        }


        protected override Task CommitAsync()
        {
            // if there are any changes as indicated by the isModified property of the Page
            if (IsModified)
            {
                // set the setting on the module
                EditToolModule.GenerateComment = _generateComments;

                // set the project dirty as well
                ProjectModule.CurrentProject.SetDirty(IsModified);
            }

            return Task.FromResult(0);
        }
    }
}
