namespace TranslateTools {
    public class MyUserControl : UserControl, IMyUserControl, IDisposable {

        private protected string _name = "";
        private protected string _path = "";

        public virtual string DisplayName {
            get {
                return _name; 
            }
            protected set {
                _name = value;
            }
        }
        public virtual string FromPath { 
            get {
                return _path;
            }
            protected set {
                _path = value;
            }
        }
       

        public Control ToControl() {
            return (Control) this;
        }

        public virtual string? GetSaveInfo() {
            throw new NotImplementedException();
        }

        public virtual bool ImportSaveInfo(string fileSaveStr) {
            throw new NotImplementedException();
        }

        public virtual void FocusMe() {
            throw new NotImplementedException();
        }


        public event EventHandler<string>? SelectedText = null;
        protected void TriggerSelectText(string text) {
            SelectedText?.Invoke(null, text);
        }


        private protected bool disposedValue;
        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    GC.Collect();
                }
                disposedValue = true;
            }
        }
        public new void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}