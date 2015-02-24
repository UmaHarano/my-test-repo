## Lab 5: Edit Tool

###What you learn in this lab
* Add sketch tool to edit galleries
* Use of the sketch geometry for ancillary geometry manipulation
* Edit events

*******
* [Step 1: Create a sketch tool for advanced feature manipulation](#step-1-create-a-sketch-tool-for-advanced-feature-manipulation)
* [Step 2: Create a context specific sketch tool](#step-2-create-a-context-specific-sketch-tool)
* [Step 3: Listen and react to editor events](#step-3-listen-and-react-to-editor-events)

**Estimated completion time: 20 minutes**
****

##Step 1: Create a sketch tool for advanced feature manipulation
* Download the EditingTools.pkgx from ArcGIS Online and save the project.
* _CutWithoutSelection.cs_ as an ArcObjects example brought forward
* DAML for modify category and gallery placement
* Core.Data and editoperation

##Step 2: Create a context specific sketch tool
* Open _DensifyButton.cs_
* Use GeometryEngine to manipulate sketch geometry
* DAML to place into the right context location

##Step 3: Listen and react to editor events
* Open _EditToolModule.cs_
* Go through the various method to subscribe to events
* _TrackEditEvents_ function is where the magic happens
* Optional: toggle the event listening to backstage property page
