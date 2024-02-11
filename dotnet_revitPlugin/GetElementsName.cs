using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace dotnet_revitPlugin
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class GetElementsName : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                IList<Reference> pickedObjs = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if(pickedObjs.Count > 0 )
                {
                    var eleDictionary = new Dictionary<ElementId, string>();

                    foreach(Reference obj in pickedObjs)
                    {
                        ElementId eleId = obj.ElementId;
                        Element ele = doc.GetElement(eleId);

                        eleDictionary.Add(eleId, ele.Name);                        
                    }

                    var tDialog = new TaskDialog("Output msg");
                    tDialog.MainInstruction = "'Element Name' : 'Element Id'";
                    
                    foreach (var kvp in eleDictionary)
                    {
                        tDialog.MainContent += $"{kvp.Value} : {kvp.Key}" + Environment.NewLine;
                    }
                    tDialog.Show();
                }

                else
                {
                    TaskDialog.Show("Output msg", "No element selected");
                }

                
                return Result.Succeeded;
            }

            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
