namespace TranslateTools.UserForm.SupportTools {
    public class SupportTool : ISupportTool, IDisposable {
        public SupportTool() { }

        // Private Declare
        private string _name = "";
        private string _path = "";

        // ISupportTool
        public virtual int DataLength {
            get {
                return 0;
            }
        }

        public virtual string DisplayName {
            get {
                return _name;
            } 
            private set {
                _name = value;
            }
        }
        
        public virtual string Path {
            get {
                return _path;
            }
            private set {
                _path = value;
            }
        }

        public virtual bool WantHighlight {
            get {
                return false;
            }
        }

        public async Task<bool> LoadDataAsync(string path, CancellationToken token) {
            return await Task.Run(() => {
                try {
                    bool success = LoadData(path);
                    if(success) {
                        DisplayName = new FileInfo(path).Name.Split(".", 2)[0];
                        Path = path;
                        return true;
                    } else {
                        return false;
                    }
                } catch {
                    _ = MessageBox.Show($"Lỗi đọc file: {path}");
                    return false;
                }
            }, token);
        }

        public async Task<string> SearchAsync((string, int)[] fs, bool doExtra, CancellationToken token) {
            return await Task.Run(() => {
                Workload = DataLength;
                WorkloadLeft = Workload;
                ProgressBarSave = 10;

                return Search(fs, doExtra, token);
            }, token);
        }

        public string GetToolSaveString() {
            string SaveResultStr = String.Join("&|>>", SaveResult.Select(t => $"{t.Item1}&|>{t.Item2}"));
            return $"{CurrentSaveResultIndex}&|>>>{SaveResultStr}"; 
        }

        public bool ImportFromString(string saveStr) {
            try {
                string[] splitter = saveStr.Split("&|>>>", 2);
                CurrentSaveResultIndex = int.Parse(splitter[0]);
                foreach(string saveResultInfo in splitter[1].Split("&|>>")) {

                    string[] saveResultStr = saveResultInfo.Split("&|>", 2);
                    SaveResult.Enqueue((saveResultStr[0], saveResultStr[1]));

                }
                return true;
            } catch {
                return false;
            }
            
        }


        // Private Virtual
        protected virtual bool LoadData(string path) {
            throw SupportToolHelpers.notImplementedException;
        }

        protected virtual string Search((string, int)[] fs, bool doExtra, CancellationToken token) {
            throw SupportToolHelpers.notImplementedException;
        }

        // Save Input & Ouput Info
        private int CurrentSaveResultIndex = 0;

        private readonly Queue<(string, string)> SaveResult = new();

        private protected int SaveMaximum = 5;

        public virtual string DefaultText {
            get {
                throw SupportToolHelpers.notImplementedException;
            }
        }

        public (string input, string output)? GetPreviousSave { 
            get {
                if(CurrentSaveResultIndex - 1 >= 0 && CurrentSaveResultIndex - 1 < SaveResult.Count) 
                    return SaveResult.ElementAt(CurrentSaveResultIndex -= 1);
                else 
                    return null;
            }
        }

        public (string input, string output)? GetCurrentSave {
            get {
                if(CurrentSaveResultIndex >= 0 && CurrentSaveResultIndex < SaveResult.Count)
                    return SaveResult.ElementAt(CurrentSaveResultIndex);
                else
                    return null;
            }
        }

        public (string input, string output)? GetNextSave {
            get {
                if(CurrentSaveResultIndex + 1 >= 0 && CurrentSaveResultIndex + 1 < SaveResult.Count)
                    return SaveResult.ElementAt(CurrentSaveResultIndex += 1);
                else
                    return null;
            }
        }

        public void QueueSaveOutput(string inputText, string outputText) {
            if(SaveResult.Count == SaveMaximum)
                _ = SaveResult.Dequeue();

            SaveResult.Enqueue((inputText, outputText));
            CurrentSaveResultIndex = SaveResult.Count - 1;
        }

        // Event
        public event DelegateVoidString? FoundValue = null;
        public event DelegateVoidVoid? FinishOneWork = null;
        private protected int Workload = 0;
        private int WorkloadLeft = 0;
        private int ProgressBarSave = 0;

        private protected void AddToProgressBar(int num = 1) {
            WorkloadLeft -= num;
            int newPB = WorkloadLeft * 10 / Workload;
            if(ProgressBarSave != newPB) {
                ProgressBarSave = newPB;
                FinishOneWork?.Invoke();
            }
        }

        private protected void WriteToOutput(string text) {
            FoundValue?.Invoke(text);
        }

        // IDispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    GC.Collect();
                }
                disposedValue = true;
            }
        }
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}