//   Copyright 2015 Esri

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
using System.Windows.Data;
using System.Windows.Input;

namespace NavigateCamera
{
  /// <summary>
  /// Camera Properties DockPane implementation. Provides bindable Camera property and button commands to zoom to and pan to new camera position.
  /// </summary>
  internal class CameraPaneViewModel : DockPane
  {
    private const string _dockPaneID = "NavigateCamera_CameraPane";
    private RelayCommand _zoomToCmd;
    private RelayCommand _panToCmd;

    public CameraPaneViewModel() 
    {
      _zoomToCmd = new RelayCommand(() => ZoomTo(), () => CanZoomToCamera());
      _panToCmd = new RelayCommand(() => PanTo(), () => CanPanToCamera());

      CameraChangedEvents.Subscribe(OnCameraChanged);
      ActiveMapViewChangedEvents.Subscribe(OnActiveMapViewChanged);

      InitializeAsync();
    }

    ~CameraPaneViewModel()
    {
      _zoomToCmd.Disconnect();
      _panToCmd.Disconnect();

      ActiveMapViewChangedEvents.Unsubscribe(OnActiveMapViewChanged);
      CameraChangedEvents.Unsubscribe(OnCameraChanged);
    }

    protected override async Task InitializeAsync()
    {
      if (MappingModule.ActiveMapView != null)
        Camera = await MappingModule.ActiveMapView.GetCameraAsync();
      else
        Camera = GetEmptyCamera();
    }

    /// <summary>
    /// Show the DockPane.
    /// </summary>
    internal static void Show()
    {
      DockPane pane = FrameworkApplication.FindDockPane(_dockPaneID);
      if (pane == null)
        return;

      pane.Activate();
    }

    private Camera _camera;
    public Camera Camera
    {
      get { return _camera; }
      set
      {
        SetProperty(ref _camera, value, () => Camera);
      }
    }

    #region PanTo Commands

    public ICommand PanToCmd
    {
      get { return _panToCmd; }
    }

    private Task PanTo()
    {
      return MappingModule.ActiveMapView.PanToAsync(Camera);
    }

    private bool CanPanToCamera()
    {
      return MappingModule.ActiveMapView != null;
    }

    #endregion

    #region ZoomToCmd

    public ICommand ZoomToCmd
    {
      get { return _zoomToCmd; }
    }

    private Task ZoomTo()
    {
      return MappingModule.ActiveMapView.ZoomToAsync(Camera);
    }

    private bool CanZoomToCamera()
    {
      return MappingModule.ActiveMapView != null;
    }

    #endregion

    private void OnCameraChanged(CameraEventArgs obj)
    {
      if (obj.MapView == MappingModule.ActiveMapView)
        Camera = obj.CurrentCamera;
    }

    private async void OnActiveMapViewChanged(MapViewEventArgs obj)
    {
      if (obj.MapView == null)
      {
        Camera = GetEmptyCamera();
        return;
      }
        
      Camera = await obj.MapView.GetCameraAsync();
    }

    private Camera GetEmptyCamera()
    {
      Camera camera = new Camera()
      {
        X = double.NaN,
        Y = double.NaN,
        Z = double.NaN,
        Scale = double.NaN,
        Pitch = double.NaN,
        Roll = double.NaN,
        Heading = double.NaN,
      };
      return camera;
    }

  }

  /// <summary>
  /// Button implementation to show the DockPane.
  /// </summary>
  internal class CameraPane_ShowButton : Button
  {
    protected override void OnClick()
    {
      CameraPaneViewModel.Show();
    }
  }

}
