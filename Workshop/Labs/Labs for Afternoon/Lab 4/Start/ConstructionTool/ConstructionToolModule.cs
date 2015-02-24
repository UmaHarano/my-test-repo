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
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;

namespace ConstructionToolSolution
{
    internal class ConstructionToolModule : Module
    {
        private static ConstructionToolModule _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static ConstructionToolModule Current
        {
            get
            {
                return _this ?? (_this = (ConstructionToolModule)FrameworkApplication.FindModule("ConstructionToolSolution_Module"));
            }
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        /// <summary>
        /// Generic implementation of ExecuteCommand to allow calls to
        /// <see cref="FrameworkApplication.ExecuteCommand"/> to execute commands in
        /// your Module.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override Func<Task> ExecuteCommand(string id)
        {

            //TODO: replace generic implementation with custom logic
            //etc as needed for your Module
            var command = FrameworkApplication.GetPlugInWrapper(id) as ICommand;
            if (command == null)
                return () => Task.FromResult(0);
            if (!command.CanExecute(null))
                return () => Task.FromResult(0);

            return () =>
            {
                command.Execute(null); // if it is a tool, execute will set current tool
                return Task.FromResult(0);
            };
        }
        #endregion Overrides

    }

    /// <summary>
    /// Extension method to find and locate attribute fields
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        ///     Find the field with the provided field name.
        /// </summary>
        /// <param name="table">Table or FeatureClass containing the field.</param>
        /// <param name="fieldName">
        ///     The name of the field to be retrieved.
        /// </param>
        /// <returns>
        ///     The field with the provided name. If no field of the given name is found, a null reference
        ///     is returned.
        /// </returns>
        public static async Task<Field> GetFieldByNameAsync(this Table table, string fieldName)
        {
            Field foundField = null;

            if (String.IsNullOrEmpty(fieldName))
                return foundField;

            await QueuingTaskFactory.StartNew(() =>
            {
                IReadOnlyList<Field> fields = ((TableDefinition)table.Definition).Fields;
                foundField = fields.FirstOrDefault(a => a.Name.Equals(fieldName));
            });

            return foundField;
        }

        /// <summary>
        ///     Find the first field of the provided field type.
        /// </summary>
        /// <param name="table">Table or FeatureClass containing the field.</param>
        /// <param name="fieldType">
        ///     The type of field to be retrieved.
        ///     <remarks>Some types can only exist once per table.</remarks>
        /// </param>
        /// <returns>
        ///     The first occurrence of the field type is returned. If no field of the given type is found, a null reference
        ///     is returned.
        /// </returns>
        public static async Task<Field> GetFieldByTypeAsync(this Table table, FieldType fieldType)
        {
            Field foundField = null;

            await QueuingTaskFactory.StartNew(() =>
            {
                IReadOnlyList<Field> fields = ((TableDefinition)table.Definition).Fields;
                foundField = fields.FirstOrDefault(a => a.FieldType == fieldType);
            });
            return foundField;
        }

        /// <summary>
        ///     Returns the ObjectID field from a table or feature class.
        /// </summary>
        /// <param name="table">Table or FeatureClass containing the ObjectID field.</param>
        /// <returns>The ObjectID field.</returns>
        public static async Task<Field> GetOIDFieldAsync(this Table table)
        {
            return await table.GetFieldByTypeAsync(FieldType.OID);
        }

        /// <summary>
        ///     Returns the field index of the shape/geometry field.
        /// </summary>
        /// <param name="table">FeatureClass containing the shape field.</param>
        /// <returns>The index of the shape field.</returns>
        public static async Task<int> GetShapeFieldIndexAsync(this Table table)
        {
            int fieldIndex = -1;

            await QueuingTaskFactory.StartNew(() =>
            {
                var fcDefinition = table.Definition as FeatureClassDefinition;
                if (fcDefinition != null)
                {
                    fieldIndex = fcDefinition.FindField(fcDefinition.ShapeField);
                }
            });

            return fieldIndex;
        }

        /// <summary>
        ///     Returns the name of the shape/geometry field.
        /// </summary>
        /// <param name="table">FeatureClass containing the shape field.</param>
        /// <returns>The name of the shape field.</returns>
        public static async Task<string> GetShapeFieldNameAsync(this Table table)
        {
            string shapeFieldName = String.Empty;

            await QueuingTaskFactory.StartNew(() =>
            {
                var fcDefinition = table.Definition as FeatureClassDefinition;
                if (fcDefinition != null)
                {
                    shapeFieldName = fcDefinition.ShapeField;
                }
            });

            return shapeFieldName;
        }
    }
}
