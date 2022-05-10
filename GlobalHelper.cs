namespace TranslateTools {
    public static class GlobalHelper {
        public static void InvokeSpecificControl(Control control, Action action) {
            if(control.InvokeRequired) {
                control.Invoke((MethodInvoker)delegate {
                    action();
                });
            } else {
                action();
            }
        }

        public static T InvokeSpecificControl<T>(Control control, Func<T> action) {
            if(control.InvokeRequired) {
                return control.Invoke(delegate {
                    return action();
                });
            } else {
                return action();
            }
        }

        public static async Task InvokeSpecificControlAsync(Control control, Action action) {
            await Task.Factory.StartNew(() => {
                InvokeSpecificControl(control, action);
            });
        }

        public static async Task<T> InvokeSpecificControlAsync<T>(Control control, Func<T> action) {
            return await Task.Factory.StartNew(() => {
                return InvokeSpecificControl(control, action);
            });
        }
    }
}
