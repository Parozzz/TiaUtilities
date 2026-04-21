using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    internal class GridDataChangedCache(GridDataChangedEventArgs args, int row)
    {
        public GridDataChangedEventArgs Args { get; init; } = args;
        public int Row { get; init; } = row;

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

    public class GridDataChangedHandler<T>(GridHandler<T> gridHandler) where T : GridData
    {
        private readonly GridHandler<T> gridHandler = gridHandler;
        private readonly List<GridDataChangedCache> cachedDataChanged = [];
        private GridDataChangedOperationRequest? joinRequest;
        private Task? joinRequestTimeoutTask;

        public GridDataChangedOperationRequest Join([CallerFilePath] string filePath = "",  [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if(joinRequest != null)
            {
                throw new InvalidOperationException($"Trying to add Join request twice.\nActive: {joinRequest.FilePath}, {joinRequest.MemberName}, {joinRequest.SourceLineNumber}.\nNew: {filePath}, {memberName}, {sourceLineNumber}");
            }

            GridDataChangedOperationRequest newJoinRequest = new() { FilePath = filePath, MemberName = memberName, SourceLineNumber = sourceLineNumber };
            joinRequest = newJoinRequest;
            joinRequestTimeoutTask = Task.Delay(1000).ContinueWith(t =>
            {
                if (this.joinRequest != null && this.joinRequest.Guid == newJoinRequest.Guid)
                {
                    this.joinRequestTimeoutTask = null;
                    this.joinRequest = null;
                    throw new InvalidOperationException($"Cache request timeout for {filePath}. Member: {memberName}, Line: {sourceLineNumber}");
                }
            });
            return newJoinRequest;
        }

        public void End(GridDataChangedOperationRequest joinRequest)
        {
            if(this.joinRequest != null && this.joinRequest.Guid == joinRequest.Guid)
            {
                this.joinRequestTimeoutTask = null;
                this.joinRequest = null;

                this.HandleCache();

                this.gridHandler.DataGridView.Refresh(); //This is to maintain compatability with old system.
            }
        }

        internal void HandleCellChangeEvent(GridDataChangedEventArgs args, int row)
        {
            if(this.gridHandler.DataGridView.RowCount <= 0 || this.gridHandler.DataGridView.ColumnCount <= 0)
            {
                return;
            }
            
            GridDataChangedCache cachedChange = new(args, row);
            if(this.joinRequest != null)
            {
                this.cachedDataChanged.Add(cachedChange);
            }
            else
            {
                this.gridHandler.HandleCachedDataChanges([cachedChange]);
            }
        }

        private void HandleCache()
        {
            if (cachedDataChanged.Count > 0)
            {
                this.gridHandler.HandleCachedDataChanges(cachedDataChanged);
                cachedDataChanged.Clear();
            }
        }
    }
}
