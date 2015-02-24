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
using ArcGIS.Desktop.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Events;
using System.Windows.Input;

namespace NavigateCamera
{
    /// <summary>
    /// View model for manipulating the camera properties based on the custom user control
    /// </summary>
    class CameraControlViewModel : CustomControl
    {
      public CameraControlViewModel()
      {
        //TODO initialize the zoom in and zoom out RelayCommands.

        //TODO subscribe to CameraChangedEvents and ActiveMapViewChangedEvents.
        
        //TODO initialize the pitch up and pitch down RelayCommands.

        //TODO initialize the heading by calling SetHeadingFromMapView and passing in the active map view.
      }

      #region Zoom Commands

      //private RelayCommand _zoomInCmd;
      //public ICommand ZoomInCmd
      //{
      //  get { return _zoomInCmd; }
      //}

      //private void ZoomIn()
      //{
      //  //TODO zoom the active view in by a fixed amount.
      //}

      //private RelayCommand _zoomOutCmd;
      //public ICommand ZoomOutCmd
      //{
      //  get { return _zoomOutCmd; }
      //}

      //private void ZoomOut()
      //{
      //  //TODO zoom the active view in by a fixed amount.
      //}

      //private bool CanZoom() 
      //{ 
      //  //TODO return true if the active map view is not null.
      //  return false; 
      //}

      #endregion

      #region Adjust Heading

      //private bool _enableCamera = false;
      //public bool IsCameraEnabled
      //{
      //  get
      //  {
      //    return _enableCamera;
      //  }
      //  set
      //  {
      //    SetProperty(ref _enableCamera, value, () => IsCameraEnabled);
      //  }
      //}

      //private double _headingValue = 0;
      //public double HeadingValue
      //{
      //  get { return _headingValue; }
      //  set
      //  {
      //    double cameraHeading = value > 180 ? value - 360 : value;
      //    _headingValue = value;
      //    Camera.Heading = cameraHeading;

      //    //TODO get the active map view and zoom to the new camera.
      //  }
      //}

      //private Camera Camera { get; set; }

      //private void OnCameraChanged(CameraEventArgs args) {
      //    Camera = args.CurrentCamera;
      //    SetHeading();
      //}

      //private void OnActiveViewChanged(MapViewEventArgs args) {
      //    SetHeadingFromMapView(args.MapView);
      //}
      
      //private async void SetHeadingFromMapView(MapView mapView) {
      //    //TODO test if mapView is null.
      //    //If not null set IsCameraEnabled to true
      //    //get the Camera from the mapView (mapView.GetCameraAsync())
      //    //call SetHeading().
      //    if (mapView != null) {

      //    }
      //    else {
      //        //If null set "IsCameraEnabled = false".
              
      //    }
      //}

      //private void SetHeading()
      //{
      //  if (Camera != null)
      //  {
      //    double viewHeading = Camera.Heading < 0 ? 360 + Camera.Heading : Camera.Heading;
      //    SetProperty(ref _headingValue, viewHeading, () => HeadingValue);
      //  }
      //}

      #endregion

      #region Pitch Commands

      //private RelayCommand _pitchDownCmd;
      //public ICommand PitchDownCmd
      //{
      //  get { return _pitchDownCmd; }
      //}

      //private void PitchDown()
      //{
      //  //TODO if the camera's pitch is greater than -90 subtract 5 and zoom to the new camera.
      //}

      //private RelayCommand _pitchUpCmd;
      //public ICommand PitchUpCmd
      //{
      //  get { return _pitchUpCmd; }
      //}

      //private void PitchUp()
      //{
      //  //TODO if the camera's pitch is less than 90 add 5 and zoom to the new camera.
      //}

      //private bool CanAdjustPitch() 
      //{ 
      //  //TODO return true if there is an active map view and the view is a 3D view.
      //  return false; 
      //}

      #endregion

    }
}
