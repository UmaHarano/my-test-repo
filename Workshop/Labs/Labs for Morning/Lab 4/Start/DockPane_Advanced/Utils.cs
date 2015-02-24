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

using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DockPane_Advanced
{
  internal class Utils
  {
    /// <summary>
    /// utility function to open and activate a map given the map url.
    /// </summary>
    /// <param name="url">unique map identifier</param>
    internal static async void OpenAndActivateMap(string url)
    {
      // find the map; returns null if map has never been opened in the Pro session
      Map map = MappingModule.FindMap(url);

      // default viewing mode
      MapViewingMode mode = MapViewingMode.Item2D;
      bool bAlreadyOpen = false;

      // if returns null - it has never been opened
      if (map == null)
      {
        // find the map mode (2d / 3d from the project items)      
        MapProjectItem mapPI = ProjectModule.CurrentProject.GetMaps().FirstOrDefault(m => m.Path == url);
        if (mapPI != null)
          mode = mapPI.ViewingMode;
      }
      else
      {
        mode = map.ViewingMode;

        // see if its already open
        IList<IMapPane> mapPanes = MappingModule.GetMapPanes(map);
        if ((mapPanes != null) && (mapPanes.Count > 0))
        {
          bAlreadyOpen = true;

          // activate the first one
          Pane pane = mapPanes[0] as Pane;
          if (pane != FrameworkApplication.Panes.ActivePane)
            pane.Activate();
        }
      }

      // open it with the correct mode
      if (!bAlreadyOpen)
      {
        var mapPane = MappingModule.OpenMapView(url, mode);
        await Utils.BlockUntil(() => mapPane.MapView != null && mapPane.MapView.ViewerID >= 0);
      }
    }

    /// <summary>
    /// Get the ICommand interface for a given typed DAML representation like for example: DAML.Button.esri_core_showProjectDockPane
    /// or the string itself as for example "esri_core_contentsDockPane"
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the specified commandId parameter didn't yield a valid ICommand</exception>
    /// <param name="commandId">Id of the command: use the typed DAML representation if possible to prevent errors i.e. DAML.Button.esri_core_showProjectDockPane or the string itself "esri_core_contentsDockPane" </param>
    /// <returns>ICommand if an ICommand interface exists otherwise an exception is thrown</returns>
    internal static ICommand GetICommand(string commandId)
    {
      var iCommand = FrameworkApplication.GetPlugInWrapper(commandId) as ICommand;
      return iCommand;
    }

    /// <summary>
    /// Blocks until some condition is true
    /// </summary>
    /// <param name="pred">condition to be evaluated</param>
    /// <returns></returns>
    internal static async Task BlockUntil(Func<bool> pred)
    {
      while (!pred())
        await Task.Delay(50);
    }
  }
}
