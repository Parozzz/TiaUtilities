namespace TiaXmlReader.UndoRedo
{
    public class UndoRedoHandler
    {
        private readonly List<Action> undoActionList;
        private readonly List<Action> redoActionList;
        private bool locked;

        public UndoRedoHandler()
        {
            undoActionList = new List<Action>();
            redoActionList = new List<Action>();
        }

        public UndoRedoHandler Lock()
        {
            locked = true;
            return this;
        }

        public UndoRedoHandler Unlock()
        {
            locked = false;
            return this;
        }

        public void Undo()
        {
            if (undoActionList.Count == 0)
            {
                return;
            }

            var index = undoActionList.Count - 1;
            var func = undoActionList[index];
            undoActionList.RemoveAt(index);

            func.Invoke();
        }

        public void AddUndo(Action action)
        {
            if (locked)
            {
                return;
            }

            undoActionList.Add(action);
        }

        public void Redo()
        {
            if (redoActionList.Count == 0)
            {
                return;
            }

            var index = redoActionList.Count - 1;
            var func = redoActionList[index];
            redoActionList.RemoveAt(index);

            func.Invoke();
        }

        public void AddRedo(Action action)
        {
            if (locked)
            {
                return;
            }

            redoActionList.Add(action);
        }

        public void Clear()
        {
            this.undoActionList.Clear();
            this.redoActionList.Clear();
        }
    }

    public class UndoRedoAction
    {
        private readonly Func<bool> function;

        public UndoRedoAction(Func<bool> function)
        {
            this.function = function;
        }

        public bool Execute()
        {
            return function.Invoke();
        }
    }
}
