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
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using System.Windows.Input;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core;
using System.Runtime.InteropServices;
using System.IO;
using ArcGIS.Desktop.Catalog;
using System.Diagnostics;
using ESRI.ArcGIS.ItemIndex;

namespace GeometryExercisesSolution
{
    internal class DistanceTool : Tool
    {
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos([MarshalAs(UnmanagedType.Struct)] out POINT pt);
        LinearDisplayUnit _linearDisplayUnit = null;

        protected override Task OnActivateAsync(bool active)
        {
            if (active == true)
            {
                var targetLayers = MappingModule.ActiveTOC.SelectedLayers;
                FeatureLayer targetLayer = targetLayers.First() as FeatureLayer;

                _linearDisplayUnit = DisplayUnitEnvironment.GetEnvironment.DefaultMapLinearUnit;

                List<Layer> layersToKeep = new List<Layer>(targetLayers.Count);

                foreach (var item in targetLayers)
                {
                    layersToKeep.Add(item);
                }

                if (GeometryExercisesSolutionModule.Current.HasBeenAdded == false)
                {
                    string layerTemplatesLocation = String.Empty;
                    DirectoryInfo arcgisProDirectoryInfo = new DirectoryInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                    layerTemplatesLocation = Path.Combine(arcgisProDirectoryInfo.Parent.FullName, "Resources", "LayerTemplates", "en-US");

                    IEnumerable<FileInfo> fileInfos = new DirectoryInfo(layerTemplatesLocation).EnumerateFiles();
                    QueuingTaskFactory.StartNew(async () =>
                    {
                        foreach (var templateFileInfo in fileInfos)
                        {
                            ItemInfoValue? packageItemInfo = await ItemInfoHelper.GetItemInfoValueAsync(templateFileInfo.FullName);
                            PackageItem layerPackage = new PackageItem(packageItemInfo.Value);
                            await layerPackage.AddToCurrentMapAsync(null);
                            GeometryExercisesSolutionModule.Current.HasBeenAdded = true;
                            break;
                        }

                        MappingModule.ActiveTOC.ClearSelection();
                        MappingModule.ActiveTOC.SelectLayers(layersToKeep);

                    });
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("");
            }

            return base.OnActivateAsync(active);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // signal that all is well with the mouse down event
            // -- otherwise there is no callback for the MouseUp event
            e.Handled = true;
        }

        protected override async void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                {
                    e.Handled = true;

                    POINT clickPOINT;
                    GetCursorPos(out clickPOINT);
                    MapPoint clickMapPoint = await MappingModule.ActiveMapView.ScreenToLocationAsync(new System.Windows.Point(clickPOINT.X, clickPOINT.Y));

                    Envelope ext = await MappingModule.ActiveMapView.GetExtentAsync();
                    double searchDistance = 0.0;
                    await QueuingTaskFactory.StartNew(() =>
                    {
                        searchDistance = ext.Width / 2.0;
                    });

                    var targetLayer = MappingModule.ActiveTOC.SelectedLayers.OfType<FeatureLayer>().First();

                    ArcGIS.Core.Geometry.GeometryEngine.ProximityResult clickResult = await ComputeDistanceToGeometryAsync(clickMapPoint, targetLayer, 1, searchDistance);

                    var templateGroupLayer = MappingModule.ActiveMapView.Map.GetFlattenedLayers().OfType<GroupLayer>().Where(lyr => lyr.Name.Equals("Bright Map Notes")).First();
                    var flashLayer = templateGroupLayer.GetFlattenedLayers().OfType<FeatureLayer>().Where(lyr => lyr.Name.Equals("Bright - Point Notes")).First();

                    if (clickResult != null)
                        await flashLayer.AddAndFlashGeometryAsync(clickResult.Point);
                }

                base.OnMouseUp(e);
            }
        }

        private async Task<ArcGIS.Core.Geometry.GeometryEngine.ProximityResult> ComputeDistanceToGeometryAsync(MapPoint clickPoint, FeatureLayer targetLayer, int hitNumber, double searchDistance)
        {
            FeatureClass featureClass = await targetLayer.GetTableAsync() as FeatureClass;

            Geometry searchBuffer = null;
            List<Geometry> geometryList = new List<Geometry>();

            double closestDistance = double.MaxValue;
            ArcGIS.Core.Geometry.GeometryEngine.ProximityResult clickResult = null;
            MapPoint closestPoint = null;
            bool foundFeatureAndComputedDistance = false;


            await QueuingTaskFactory.StartNew(() =>
            {
                var classDefinition = featureClass.Definition as FeatureClassDefinition;

                searchBuffer = GeometryEngine.Project(GeometryEngine.Buffer(clickPoint, searchDistance), classDefinition.SpatialReference);

                SpatialQueryFilter spatialFilter = new SpatialQueryFilter()
                {
                    FilterGeometry = searchBuffer,
                    SpatialRelationship = SpatialRelationship.EnvelopeIntersects
                };

                RowCursor featureCursor = featureClass.Search(spatialFilter, false);

                while (featureCursor.MoveNext())
                {
                    var feature = featureCursor.Current as Feature;

                    geometryList.Add(feature.Shape);
                }

                clickPoint = GeometryEngine.Project(clickPoint, classDefinition.SpatialReference) as MapPoint;

                foreach (var geometry in geometryList)
                {
                    var geoResult = GeometryEngine.NearestBoundaryPoint(geometry, clickPoint, searchDistance);
                    if (geoResult.Distance < closestDistance)
                    {
                        closestDistance = geoResult.Distance;
                        clickResult = geoResult;
                        foundFeatureAndComputedDistance = true;
                    }
                }
            });

            if (foundFeatureAndComputedDistance)
            {
                DockPane dockPane = FrameworkApplication.FindDockPane("GeometrySamples_ClosestGeometryPane");
                if (dockPane == null)
                {
                    return new ArcGIS.Core.Geometry.GeometryEngine.ProximityResult();
                }

                DistancePaneViewModel cgVM = dockPane as DistancePaneViewModel;
                cgVM.DistanceDisplayUnit = _linearDisplayUnit;
                cgVM.ClickResult = clickResult;
            }

            return clickResult;
        }

    }
}
