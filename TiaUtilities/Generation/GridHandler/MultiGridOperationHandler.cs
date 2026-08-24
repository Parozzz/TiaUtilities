using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.GridHandler
{
    public class MultiGridOperationHandler : ICleanable
    {
        private Form? form;
        private GridFindForm? findForm;
        public JSScriptHandler JsScriptHandler { get; init; } = new();

        public IGridHandler? GridHandler { get; private set; }

        public void Init(Form form)
        {
            this.form = form;

            this.JsScriptHandler.Init();
        }

        public bool IsDirty() => this.JsScriptHandler.IsDirty();
        public void Wash() => this.JsScriptHandler.Wash();

        public void SetActiveGrid(IGridHandler? handler)
        {
            this.GridHandler = handler;

            if(this.GridHandler != null)
            {
                GridScriptExecutionData executionData = new(this.GridHandler);
                this.JsScriptHandler.SetExecutionData(executionData);
            }
        }

        public void ShowFindForm(IGridHandler? handler)
        {
            this.SetActiveGrid(handler); 
            if (this.GridHandler != null)
            {
                ShowFindForm();
            }

        }

        public void ShowFindForm()
        {
            if (this.findForm == null)
            {
                this.findForm = new(this);
                this.findForm.FormClosed += (sender, args) => this.findForm = null;
                this.findForm.Show(form);
            }
            else
            {
                findForm.Activate();
            }
        }

        public void ShowGridScript<T>(GridHandler<T> handler) where T : GridData
        {
            this.SetActiveGrid(handler); 
            if (this.GridHandler != null)
            {
                ShowGridScript();
            }
        }

        public void ShowGridScript()
        {
            this.JsScriptHandler.ShowForm(form);
        }
    }
}
