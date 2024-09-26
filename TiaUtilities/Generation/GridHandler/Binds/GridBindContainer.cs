using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GridHandler.Binds
{
    public class GridBindContainer(JavascriptErrorReportThread errorReportThread)
    {
        private Form? form;
        private GridHandlerBind? handlerBind;

        private GridFindForm? findForm;
        public GridScript GridScript { get; init; } = new(errorReportThread);

        public void Init(Form form)
        {
            this.form = form;

            this.GridScript.Init();
        }

        public void ChangeBind<T>(GridHandler<T>? handler) where T : IGridData
        {
            if (this.handlerBind != null && this.handlerBind.IsSameGridHandler(handler))
            {
                return;
            }

            handlerBind = handler == null ? null : GridHandlerBind.CreateBind(handler);

            findForm?.BindToHandler(handlerBind);
            GridScript.BindToHandler(handlerBind);
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
                this.findForm.BindToHandler(handlerBind);
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
            this.GridScript.ShowConfigForm(form);
        }
    }
}
