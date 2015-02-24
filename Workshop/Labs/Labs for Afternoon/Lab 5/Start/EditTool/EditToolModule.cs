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
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Events;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Mapping;
using System.Xml.Linq;
using ArcGIS.Desktop.Framework.Events;

namespace EditTool
{
    internal class EditToolModule : Module
    {
        private static SubscriptionToken _editEventsToken;
        private SubscriptionToken _activeMapViewChangeEventToken;
        private static List<string> _editFlags = new List<string>();
        private const string COMMENT_FIELD_NAME = "Comments";
        private const string COMMENT_SETTINGS_PROJECT = "EditTool_Settings";
        private const string MODULE_NAME = "EditTool_Module";
        private static string GENCOMMENT_NAME = "GenerateComments";

        private static EditToolModule _this = null;

        /// <summary>
        /// Default constructor for the edit module.
        /// Set inital flag for comment generation and subscribe to project events
        /// </summary>
        public EditToolModule()
        {
            // use the inital setting of true to capture events
            // time permitting you will control the capture event state with a backstage property page
            GenerateComment = true;

            // TODO for the backstage implementation
            //// subscribe to project based events
            //// on open read the settings 
            //ArcGISProjectOpenedEvents.Subscribe(OnProjectOpen);
            //// when the project is saved, store the updated settings if needed
            //ArcGISProjectSavingEvents.Subscribe(OnProjectSaving);
        }

        /// <summary>
        /// In the destructor unsubscribe from all out-standing events
        /// </summary>
        ~EditToolModule()
        {
            // final take down
            if (_editEventsToken != null)
            {
                // if we still have a token then unsubscribe from the edits events
                EditEvent.Unsubscribe(_editEventsToken);
                _editEventsToken = null;
            }

            if (_activeMapViewChangeEventToken != null)
            {
                // unsubscribe from the pane change event
                ActivePaneChangedEvent.Unsubscribe(_activeMapViewChangeEventToken);
            }

            // unsubscribe from the project based events
            ArcGISProjectOpenedEvents.Unsubscribe(OnProjectOpen);
            ArcGISProjectSavingEvents.Unsubscribe(OnProjectSaving);
        }

        /// <summary>
        /// Subscribe to the events of the active map view
        /// </summary>
        /// <returns></returns>
        protected override bool Initialize()
        {
            bool initResult = base.Initialize();

            // start subscribing to the events when the map view changes
            //_activeMapViewChangeEventToken = ActiveMapViewChangedEvents.Subscribe(OnActiveMapViewChangeEvents);

            return initResult;
        }


        /// <summary>
        /// Handler to subscribe to map view events.
        /// </summary>
        /// <param name="obj">MapViewEvent arguments from the ActiveMapViewChangedEvents.</param>
        private void OnActiveMapViewChangeEvents(MapViewEventArgs obj)
        {
            if (obj.MapView != null)
            {
                AttachEditEvents(obj.MapView.Map);
            }
        }


        /// <summary>
        /// Handler to persist custom module settings.
        /// </summary>
        /// <param name="arg">ArcGISProjectEvent arguments from the ArcGISProjectSavingEvents.</param>
        /// <returns></returns>
        private async Task OnProjectSaving(ArcGISProjectEventArgs arg)
        {
            await SaveProjectSettingsAsync(arg.Project.ID);
        }

        /// <summary>
        /// Handler to read custom module settings.
        /// </summary>
        /// <param name="arg">ArcGISProjectEvent arguments from the ArcGISProjectOpenedEvents.</param>
        private async void OnProjectOpen(ArcGISProjectEventArgs arg)
        {
            await LoadProjectSettingsAsync(arg.Project.ID);
        }

        /// <summary>
        /// Load the custom editing settings from the given project. The settings will change extension specific
        /// properties for this edit tool sample.
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <returns></returns>
        private async Task LoadProjectSettingsAsync(int projectID)
        {
            // clear any existing settings
            if (Settings != null)
                Settings.Clear();
            else
                Settings = new Dictionary<string, string>();

            // reset the module level variable
            GenerateComment = false;

            // retrieve the module settings
            string projectSettings = await ProjectModule.GetModuleSettingsAsync(projectID, MODULE_NAME);

            if (String.IsNullOrEmpty(projectSettings))
                return;

            // create a XDocument from the stored string
            XDocument projectSettingsXDoc = XDocument.Parse(projectSettings);

            // turn the XDocument into a dictionary
            Settings = projectSettingsXDoc.Root.Descendants().ToDictionary(c => c.Name.LocalName, c => c.Value);

            if (Settings.ContainsKey(GENCOMMENT_NAME))
            {
                GenerateComment = Convert.ToBoolean(Settings[GENCOMMENT_NAME]);
            }

        }

        /// <summary>
        /// Save the custom editing settings from the given project. The settings will change extension specific
        /// properties for this edit tool sample.
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <returns></returns>
        private async Task SaveProjectSettingsAsync(int projectID)
        {
            // check if the internal settings dictionary is valid, otherwise create one
            if (Settings == null)
                Settings = new Dictionary<string, string>();

            // update the settings dictionary with the current state of the module level variable
            if (Settings.ContainsKey(GENCOMMENT_NAME))
            {
                Settings[GENCOMMENT_NAME] = GenerateComment.ToString();
            }
            else
            {
                Settings.Add(GENCOMMENT_NAME, GenerateComment.ToString());
            }

            // transform the dictionary into the xml representation
            var settingsXDoc = new XDocument(new XElement(MODULE_NAME, Settings.Select(i => new XElement(i.Key, i.Value))));

            // store the project based settings
            await ProjectModule.PutModuleSettingsAsync(projectID, MODULE_NAME, settingsXDoc.ToString(SaveOptions.DisableFormatting));
        }


        /// <summary>
        /// Subscribe to the editing events.
        /// </summary>
        /// <param name="currentMap">The map object from which to subscribe or to unsubscribe from the edit events.</param>
        internal static void AttachEditEvents(Map currentMap)
        {
            // if there is an existing subscription token for the edit events
            if (_editEventsToken != null)
            {
                // TODO
                // then unsubscribe
                _editEventsToken = null;
            }

            if (GenerateComment)
            {
                // TODO
                // get the list of feature layers and subscribe to generate an edit comment
                //_editEventsToken = ...TrackEditEvents...
            }
        }

        /// <summary>
        /// Based on the type of edit event, a corresponding message is stored in the comment field of the feature class and the attribute
        /// modification is stored as an operation in undo/redo stack
        /// </summary>
        /// <param name="editEventArgs">The EditEventArgs object is passed to the EditEvent subscription and contains information about the current
        /// adds, modifies, and deletes of the features.</param>
        /// <returns></returns>
        private static async Task TrackEditEvents(EditEventArgs editEventArgs)
        {
            // let's deal with the create events first
            foreach (var mapMember in editEventArgs.Creates)
            {
                // TODO
                // for each featuer layer containing creates
                // the layer that contains the creates

                // TODO
                // find the field described by the COMMENT_FIELD_NAME const
                Table table = null;
                int commentFieldIndex = -1;

                // prepare the comment to enter for the feature
                string comment = String.Format("Created @ {0:G}", DateTime.Now);

                // set up the edit operation to make the changes
                var editOperation = EditingModule.CreateEditOperation();
                editOperation.Name = "Add 'created' comment.";

                KeyValuePair<MapMember, IReadOnlyCollection<long>> member = mapMember;

                editOperation.Callback(async context =>
                {
                    foreach (long featureOID in member.Value)
                    {
                        // for each feature OID retrieve the feature
                        // TODO: get the fields from the table in order to prepare the WhereClause
                        Row currentRow = await table.GetRowByIDAsync(featureOID);

                        if (currentRow == null)
                            continue;

                        // if we have the comment field index
                        if (commentFieldIndex != -1)
                        {
                            // add the comment into the comment attribute field
                            currentRow[commentFieldIndex] = comment;
                            try
                            {
                                currentRow.Store();
                                _editFlags.Add("create_" + featureOID);
                            }
                            catch (GeodatabaseRowException rowException)
                            {
                                System.Diagnostics.Debug.WriteLine(rowException.Message);
                            }
                            finally
                            {
                                context.invalidate(currentRow);
                            }
                        }
                    }
                }, table);

                // don't await the edit operation
                editOperation.ExecuteAsync();
            }

            foreach (var mapMember in editEventArgs.Modifies)
            {
                var featureLayer = mapMember.Key as FeatureLayer;
                string comment = String.Format("Changed @ {0:G}", DateTime.Now);
                Table table = await featureLayer.GetTableAsync();

                if (mapMember.Value.Count > 0)
                {
                    var editOperation = EditingModule.CreateEditOperation();
                    editOperation.Name = "Add 'changed' comment.";

                    KeyValuePair<MapMember, IReadOnlyCollection<long>> member = mapMember;

                    editOperation.Callback(context =>
                    {
                        foreach (long featureOID in member.Value)
                        {
                            // check who triggered the event...this is to avoid infinite loops based on self-triggered 
                            // edit operations
                            // if there is an existing modify operation for current feature id, 
                            // abort the operation and continue to the next feature
                            if (_editFlags.Contains("modify_" + featureOID.ToString()))
                            {
                                _editFlags.Remove("modify_" + featureOID.ToString());
                                continue;
                            }

                            // if there is an existing create operation for current feature id, 
                            // abort the operation and continue to the next feature
                            if (_editFlags.Contains("create_" + featureOID.ToString()))
                            {
                                //editOperation.Abort();
                                _editFlags.Remove("create_" + featureOID.ToString());
                                continue;
                            }

                            // find the name of the ObjectID field
                            string oidFieldName = "OBJECTID";

                            var tableDefinition = table.Definition as TableDefinition;
                            if (tableDefinition != null)
                                oidFieldName = tableDefinition.ObjectIDField;


                            var queryFilter = new QueryFilter
                            {
                                WhereClause = string.Format("{0} = {1}", oidFieldName, featureOID.ToString())
                            };
                            var rc = table.Search(queryFilter, false);
                            var commentFieldIndex = rc.FindField(COMMENT_FIELD_NAME);

                            while (rc.MoveNext())
                            {
                                if (commentFieldIndex != -1)
                                {
                                    rc.Current[commentFieldIndex] = comment;
                                    try
                                    {
                                        rc.Current.Store();
                                        _editFlags.Add("modify_" + featureOID.ToString());
                                    }
                                    catch (GeodatabaseRowException rowException)
                                    {
                                        System.Diagnostics.Debug.WriteLine(rowException.Message);
                                    }
                                    finally
                                    {
                                        context.invalidate(rc.Current);
                                    }
                                }
                            }
                        }
                    }, table);

                    // don't await the edit operation
                    editOperation.ExecuteAsync();
                }
            }
        }

        /// <summary>
        /// Indicator if comments about the edit should be captured.
        /// </summary>
        public static bool GenerateComment { get; set; }

        /// <summary>
        /// Internal dictionary to the module to store settings to be persisted.
        /// </summary>
        internal Dictionary<string, string> Settings { get; set; }



        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static EditToolModule Current
        {
            get
            {
                return _this ?? (_this = (EditToolModule)FrameworkApplication.FindModule(MODULE_NAME));
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


    /// <summary>
    /// Extension method to search and retrieve rows
    /// </summary>
    public static class TableExtensions
    {
        /// <summary>
        /// Performs a spatial query against the table/feature class.
        /// </summary>
        /// <remarks>It is assumed that the feature class and the search geometry are using the same spatial reference.</remarks>
        /// <param name="searchTable">The table/feature class to be searched.</param>
        /// <param name="searchGeometry">The geometry used to perform the spatial query.</param>
        /// <param name="spatialRelationship">The spatial relationship used by the spatial filter.</param>
        /// <returns></returns>
        public static async Task<RowCursor> SearchAsync(this Table searchTable, Geometry searchGeometry, SpatialRelationship spatialRelationship)
        {
            RowCursor rowCursor = null;

            await QueuingTaskFactory.StartNew(() =>
            {
                // TODO 
                // define a spatial query filter using the provided spatial relationship and search geometry
                SpatialQueryFilter spatialQueryFilter = new SpatialQueryFilter();

                // apply the spatial filter to the feature class in question
                rowCursor = searchTable.Search(spatialQueryFilter);

            });

            return rowCursor;
        }

        /// <summary>
        /// Retrieves a specific row/feature for given ObjectID.
        /// </summary>
        /// <param name="table">Table of Feature class containing the row/feature.</param>
        /// <param name="oid">The ObjectID of the row/feature to be returned.</param>
        /// <returns>The row/feature specified by the ObjectID. If entity with the ObjectID exists, a null reference is returned.</returns>
        public static async Task<Row> GetRowByIDAsync(this Table table, long oid)
        {
            Row foundRow = null;

            if (table == null)
                return foundRow;

            await QueuingTaskFactory.StartNew(() =>
            {
                if (table.Definition is TableDefinition)
                {
                    var tableDefinition = table.Definition as TableDefinition;

                    // TODO
                    // specify query filter to return only the row with the specified identifier
                    QueryFilter queryFilter = new QueryFilter();

                    RowCursor rc = table.Search(queryFilter, false);

                    if (rc.MoveNext())
                    {
                        foundRow = rc.Current;
                    }
                }
            });

            return foundRow;
        }
    }

}