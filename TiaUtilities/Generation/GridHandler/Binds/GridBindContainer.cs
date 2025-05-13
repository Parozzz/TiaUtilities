using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.GridHandler.JSScript;

namespace TiaUtilities.Generation.GridHandler.Binds
{
    public class GridBindContainer(ErrorReportThread errorThread) : ICleanable
    {
        private Form? form;
        private GridHandlerBind? handlerBind;

        private GridFindForm? findForm;
        public GridScriptHandler GridScriptHandler { get; init; } = new(errorThread);

        public void Init(Form form)
        {
            this.form = form;

            this.GridScriptHandler.Init();
        }

        public bool IsDirty() => this.GridScriptHandler.IsDirty();
        public void Wash() => this.GridScriptHandler.Wash();

        public void ChangeBind<T>(GridHandler<T>? handler) where T : IGridData
        {
            if (this.handlerBind != null && this.handlerBind.IsSameGridHandler(handler))
            {
                return;
            }

            handlerBind = handler == null ? null : GridHandlerBind.CreateBind(handler);

            findForm?.BindToGridHandler(handlerBind);
            GridScriptHandler.BindToGridHandler(handlerBind);
        }

        public void ShowFindForm<T>(GridHandler<T> handler) where T : IGridData
        {
            this.ChangeBind(handler);
            ShowFindForm();
        }

        public void ShowFindForm()
        {
            if (this.findForm == null)
            {
                this.findForm = new();
                this.findForm.BindToGridHandler(handlerBind);
                this.findForm.FormClosed += (sender, args) => this.findForm = null;
                this.findForm.Show(form);
            }
            else
            {
                findForm.Activate();
            }
        }

        public void ShowGridScript<T>(GridHandler<T> handler) where T : IGridData
        {
            ChangeBind(handler);
            ShowGridScript();
        }

        public void ShowGridScript()
        {
            this.GridScriptHandler.ShowForm(form);
        }
    }
}
