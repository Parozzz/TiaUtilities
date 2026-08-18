using System.Reflection;
using System.Runtime.CompilerServices;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.UndoRedo;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridDataChangedCache(GridDataPropertyChangedEventArgs args, int row)
    {
        public GridDataPropertyChangedEventArgs Args { get; init; } = args;
        public int Row { get; init; } = row;

        public GridData GridData { get => this.Args.Data; }
        public string PropertyName { get => this.Args.PropertyName; }
        public GridDataColumn Column { get => this.Args.Column; }
        public int ColumnIndex { get => this.Column.ColumnIndex; }
        public int RowIndex { get; init; } = row;
        public object? OldValue { get => this.Args.OldValue; }
        public object? NewValue { get => this.Args.NewValue; }

        private PropertyInfo? GetDataPropertyInfo()
        {
            return this.GridData.GetType().GetProperty(this.PropertyName, BindingFlags.Instance | BindingFlags.Public);
        }

        public void RestoreOldValue()
        {
            var propertyInfo = this.GetDataPropertyInfo();
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(this.GridData, this.OldValue);
            }
        }

        public void RestoreNewValue()
        {
            var propertyInfo = this.GetDataPropertyInfo();
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(this.GridData, this.NewValue);
            }
        }

        public override string ToString()
        {
            return $"Property: {Args.PropertyName}, Column: {Args.Column.ColumnIndex}, Row: {Row}";
        }
    }

    public class GridDataChangedOperationRequest()
    {
        public Guid Guid { get; init; } = Guid.NewGuid();
        public required string FilePath { get; init; }
        public required string MemberName { get; init; }
        public required int SourceLineNumber { get; init; }
    }

    public class GridDataChangedHandler(
        ExcelLikeDataGridView dataGridView, 
        GridHandlerEventCaller gridHandlerEventCaller, 
        UndoRedoHandler undoRedoHandler)
    {
        private readonly List<GridDataChangedCache> cachedDataChanged = [];

        private GridDataChangedOperationRequest? joinRequest;
        private GridDataChangedOperationRequest? suspendRequest;

        public bool Joining { get => this.joinRequest != null; }
        public bool Suspended { get => this.suspendRequest != null; }

        public GridDataChangedOperationRequest Join([CallerFilePath] string filePath = "",  [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(this.joinRequest != null)
            {
                throw new InvalidOperationException($"Trying to add Join request twice.\nActive: {joinRequest.FilePath}, {joinRequest.MemberName}, {joinRequest.SourceLineNumber}.\nNew: {filePath}, {memberName}, {sourceLineNumber}");
            }

            GridDataChangedOperationRequest newJoinRequest = new() { FilePath = filePath, MemberName = memberName, SourceLineNumber = sourceLineNumber };
            this.joinRequest = newJoinRequest;
            Task.Delay(120000).ContinueWith(t =>
            {
                if (this.joinRequest != null && this.joinRequest.Guid == newJoinRequest.Guid)
                {
                    this.joinRequest = null;

                    var ex = new InvalidOperationException($"Cached join request timeout for {filePath}. Member: {memberName}, Line: {sourceLineNumber}");
                    Utils.ShowExceptionMessage(ex);
                    throw ex;
                }
            });
            return newJoinRequest;
        }

        public GridDataChangedOperationRequest Suspend([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (this.suspendRequest != null)
            {
                throw new InvalidOperationException($"Trying to add Suspend request twice.\nActive: {this.suspendRequest.FilePath}, {this.suspendRequest.MemberName}, {this.suspendRequest.SourceLineNumber}.\nNew: {filePath}, {memberName}, {sourceLineNumber}");
            }

            GridDataChangedOperationRequest newSuspendRequest = new() { FilePath = filePath, MemberName = memberName, SourceLineNumber = sourceLineNumber };
            this.suspendRequest = newSuspendRequest;
            Task.Delay(1000).ContinueWith(t =>
            {
                if (this.suspendRequest != null && this.suspendRequest.Guid == newSuspendRequest.Guid)
                {
                    this.suspendRequest = null;
                    throw new InvalidOperationException($"Suspend request timeout for {filePath}. Member: {memberName}, Line: {sourceLineNumber}");
                }
            });
            return suspendRequest;
        }

        public void End(GridDataChangedOperationRequest request)
        {
            if(this.joinRequest != null && this.joinRequest.Guid == request.Guid)
            {
                this.joinRequest = null;

                this.HandleCache();
                dataGridView.Refresh(); //This is to maintain compatability with old system.
            }
            else if(this.suspendRequest != null && this.suspendRequest.Guid == request.Guid)
            {
                this.suspendRequest = null;
            }
        }

        internal void HandleCellChangeEvent(GridDataPropertyChangedEventArgs args, int row)
        {
            if(this.Suspended || dataGridView.RowCount <= 0 || dataGridView.ColumnCount <= 0)
            {
                return;
            }
            
            GridDataChangedCache cachedChange = new(args, row);
            this.cachedDataChanged.Add(cachedChange);

            if(!this.Joining)
            {
                this.HandleCache();
            }
        }

        private void HandleCache()
        {
            if (this.cachedDataChanged.Count > 0)
            {
                gridHandlerEventCaller.CallDataChangedEvent(this.cachedDataChanged);
                this.AddUndo(cachedDataChanged);

                this.cachedDataChanged.Clear();
            }
        }

        private void AddUndo(List<GridDataChangedCache> changedData)
        {
            List<GridDataChangedCache> copyChanges = [.. changedData];
            undoRedoHandler.AddUndo(() =>
            {
                undoRedoHandler.Lock();

                dataGridView.SuspendLayout();

                var req = this.Join();
                copyChanges.ForEach(d => d.RestoreOldValue());
                this.End(req);

                dataGridView.Refresh();
                dataGridView.ResumeLayout(performLayout: true);

                undoRedoHandler.Unlock();

                undoRedoHandler.AddRedo(() =>
                {
                    undoRedoHandler.Lock();

                    dataGridView.SuspendLayout();

                    var req = this.Join();
                    copyChanges.ForEach(d => d.RestoreNewValue());
                    this.End(req);

                    dataGridView.Refresh();
                    dataGridView.ResumeLayout(performLayout: true);

                    undoRedoHandler.Unlock();

                    this.AddUndo(copyChanges);
                });

            });

        }
    }
}
