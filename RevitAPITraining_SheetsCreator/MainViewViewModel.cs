using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Document = Autodesk.Revit.DB.Document;

namespace RevitAPITraining_SheetsCreator
{
    internal class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;
        public List<FamilySymbol> TitleBlockTypes { get; } = new List<FamilySymbol>();
        public FamilySymbol SelectedTitleBlockType { get; set; }
        public int SheetsCount { get; set; }
        public List<View> Views { get; } = new List<View>();
        public View SelectedView { get; set; }
        public string DesignedBy { get; set; }
        public DelegateCommand CreateCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;
            TitleBlockTypes = Utils.GetTitleBlockTypes(_doc);
            Views = Utils.GetViews(_doc);
            CreateCommand = new DelegateCommand(OnCreateCommand);
        }

        private void OnCreateCommand()
        {
            if (TitleBlockTypes == null)
            {
                return;
            }
            using (var ts = new Transaction(_doc, "Create sheets"))
            {
                ts.Start();
                for (int i = 0; i < SheetsCount; i++)
                {
                    ViewSheet vs = ViewSheet.Create(_doc, SelectedTitleBlockType.Id);
                    if (SelectedView != null)
                    {
                        ElementId duplicatedPlanId = SelectedView.Duplicate(ViewDuplicateOption.Duplicate);
                        UV location = new UV((vs.Outline.Max.U - vs.Outline.Min.U) / 2,
                                             (vs.Outline.Max.V - vs.Outline.Min.V) / 2);
                        Viewport.Create(_doc, vs.Id, duplicatedPlanId, new XYZ(location.U, location.V, 0));
                    }
                    if (DesignedBy != null)
                    {
                        Parameter designedBy = vs.get_Parameter(BuiltInParameter.SHEET_DESIGNED_BY);
                        designedBy.Set(DesignedBy);
                    }
                }
                ts.Commit();
            }
            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        public void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

    }
    public class Utils
    {
        public static List<FamilySymbol> GetTitleBlockTypes(Document doc)
        {
            return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_TitleBlocks)
            .Cast<FamilySymbol>()
            .ToList();
        }

        internal static List<View> GetViews(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
            .WhereElementIsNotElementType()
                .Cast<View>()
                .ToList();
        }
    }
}
