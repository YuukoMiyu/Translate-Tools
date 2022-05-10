namespace TranslateTools {
    public interface IMyUserControl{
        string DisplayName { get; }
        string FromPath { get; }
        event EventHandler<string>? SelectedText;
        
        string? GetSaveInfo();
        bool ImportSaveInfo(string fileSaveStr);
        
        Control ToControl();
        void FocusMe();
        void Dispose();
    }
}
