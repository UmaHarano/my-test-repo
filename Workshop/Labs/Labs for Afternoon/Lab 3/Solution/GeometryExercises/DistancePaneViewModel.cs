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
using System.Windows.Data;
using ArcGIS.Desktop.Core;
using System.Globalization;
using ArcGIS.Core.CIM;

namespace GeometryExercisesSolution
{
    internal class DistancePaneViewModel : DockPane
    {
        private const string _dockPaneID = "GeometryExercisesSolution_DistancePane";

        protected DistancePaneViewModel() { }

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

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Closest Geometry";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        /// <summary>
        /// Property holding the proximity results from the distance tool
        /// </summary>
        private ArcGIS.Core.Geometry.GeometryEngine.ProximityResult _distanceToClick;
        public ArcGIS.Core.Geometry.GeometryEngine.ProximityResult ClickResult
        {
            get { return _distanceToClick; }
            set
            {
                SetProperty(ref _distanceToClick, value, () => ClickResult);
            }
        }

        private LinearDisplayUnit _distanceDisplayUnit = new LinearDisplayUnit();

        /// <summary>
        /// Property holding the currently selected default distance unit in the ArcGIS Pro application
        /// </summary>
        public LinearDisplayUnit DistanceDisplayUnit
        {
            get { return _distanceDisplayUnit; }
            set
            {
                SetProperty(ref _distanceDisplayUnit, value, () => DistanceDisplayUnit);
            }
        }

    }

    internal class DistanceAndUnitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string doubleAsString = String.Empty;

            double distance = 0.0;
            if (values[0] is ArcGIS.Core.Geometry.GeometryEngine.ProximityResult)
            {
                distance = ((ArcGIS.Core.Geometry.GeometryEngine.ProximityResult)values[0]).Distance;
            }

            if (values[1] is LinearDisplayUnit)
            {
                LinearDisplayUnit linearDisplayUnit = values[1] as LinearDisplayUnit;

                if (linearDisplayUnit.NumericFormat != null)
                {

                    string numberFormatting = "{0:G}";

                    switch (linearDisplayUnit.NumericFormat.RoundingOption)
                    {

                        case esriRoundingOptionEnum.esriRoundNumberOfDecimals:
                            numberFormatting = "{0:F" + linearDisplayUnit.NumericFormat.RoundingValue.ToString() + "}";
                            break;
                        case esriRoundingOptionEnum.esriRoundNumberOfSignificantDigits:
                            numberFormatting = "{0:G" + linearDisplayUnit.NumericFormat.RoundingValue.ToString() + "}";
                            break;
                        default:
                            break;
                    }

                    doubleAsString = String.Format(numberFormatting + " {1}", distance, linearDisplayUnit.DisplayNamePlural);

                }
            }

            return doubleAsString;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class DistancePane_ShowButton : Button
    {
        protected override void OnClick()
        {
            DistancePaneViewModel.Show();
        }
    }
}
