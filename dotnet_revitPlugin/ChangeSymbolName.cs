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
    [TransactionAttribute(TransactionMode.Manual)]
    internal class ChangeSymbolName : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if(pickedObj != null)
                {
                    ElementId eleId = pickedObj.ElementId;
                    Element ele = doc.GetElement(eleId);

                    Parameter eleParameter = ele.get_Parameter(BuiltInParameter.SYMBOL_ID_PARAM);
                    ElementId symbolId = eleParameter.AsElementId();
                    FamilySymbol symbol = doc.GetElement(symbolId) as FamilySymbol;
                    
                    using(var t = new Transaction(doc, "Rename Family Type"))
                    {
                        t.Start();
                        string tmp = symbol.Name;
                        symbol.Name = tmp + "_WIP";
                        t.Commit();

                        TaskDialog.Show("Output msg", $"'{tmp}' name was changed to" + Environment.NewLine +
                        $"'{symbol.Name}'");
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }        
        }
    }
}
