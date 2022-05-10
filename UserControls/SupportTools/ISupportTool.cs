namespace TranslateTools.UserForm.SupportTools {


    public interface ISupportTool {

        string DisplayName { get; }
        string Path { get; }

        int DataLength { get; }
        bool WantHighlight { get; }

        string DefaultText { get; }
        (string input, string output)? GetPreviousSave { get; }
        (string input, string output)? GetCurrentSave { get; }
        (string input, string output)? GetNextSave { get; }

        Task<bool> LoadDataAsync(string path, CancellationToken token);
        Task<string> SearchAsync((string, int)[] fs, bool doExtra, CancellationToken token);


        event DelegateVoidString? FoundValue;
        event DelegateVoidVoid? FinishOneWork;
        string GetToolSaveString();
        bool ImportFromString(string saveStr);
        void QueueSaveOutput(string inputText, string outputText);
        void Dispose();
    }
}
