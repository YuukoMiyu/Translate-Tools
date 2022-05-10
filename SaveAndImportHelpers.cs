using TranslateTools.UserForm.SupportTools;

namespace TranslateTools {
    internal static class SaveAndImportHelpers {

        private static readonly string[] SupportToolExtensions = { "dict.txt", "sub.txt", "eg.txt" };

        internal static async Task<IMyUserControl?> TryImportFile(string path) {
            string name = new FileInfo(path).Name;
            string[] splitter = name.Split(".", 2);

            if(splitter.Length == 2) {
                try {
                    if(SupportToolExtensions.Contains(splitter[1])) {
                        ISupportTool tool = splitter[1] switch {
                            "dict.txt" => new DictionaryLookUp(),
                            "sub.txt" => new TranslateLookUp(),
                            "eg.txt" => new ExampleLookUp(),
                            _ => throw new NotImplementedException(),
                        };

                        if(await tool.LoadDataAsync(path, new CancellationToken())) {
                            return new SupportToolsControl(tool);
                        }
                    }
                } catch {
                    // do nothing
                }
            }

            return null;
        }

        internal static async Task<IMyUserControl?> TryImportFileBySaveInfoStrAsync(string myControlSaveInfo) {
            string[] splitter = myControlSaveInfo.Split("&|>>>>");

            IMyUserControl? myUserControl = await TryImportFile(splitter[0]);
            if(myUserControl != null) {
                if(myUserControl!.ImportSaveInfo(splitter[1])) {
                    return myUserControl;
                } else {
                    _ = MessageBox.Show($"Lỗi đọc file {splitter[0]}");
                }
            } else {
                _ = MessageBox.Show($"Không thể mở file {splitter[0]}");
            }

            return null;
        }
    }
}
