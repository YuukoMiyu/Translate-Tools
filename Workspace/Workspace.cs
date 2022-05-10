using System.Text;
using TranslateTools;

namespace Translate_Tools.Workspace {
    internal class Workspace {
        private readonly WorkspaceKey Key = new("workspace");
        public List<IMyUserControl> MyUserControlList = new();

        public async Task<IMyUserControl[]?> Import(string path) {
            return await Task.Run(async () => {
                try {
                    string fileText = Key.Decrypt(File.ReadAllText(path, Encoding.UTF8));
                    List<IMyUserControl> list = new();

                    foreach(string myControlSaveInfo in fileText.Split("???|???")) {

                        IMyUserControl? myUserControl = await SaveAndImportHelpers.TryImportFileBySaveInfoStrAsync(myControlSaveInfo);
                        if(myUserControl != null) {
                            list.Add(myUserControl!);
                        }

                    }

                    if(list.Count == 0)
                        return null;

                    return list.ToArray();
                } catch {
                    return null;
                }
            });
        }

        public async Task<bool> Save(string path) {
            string workspaceStr = String.Join("???|???", MyUserControlList.Select(t => t.GetSaveInfo()).Where(t => t != null));

            return await Task.Run(() => {
                try {
                    File.WriteAllText(path, Key.Encrypt(workspaceStr.ToString()), Encoding.UTF8);
                    return true;
                } catch {
                    return false;
                }
            });
        }
    }
}
