using OpenTK.Windowing.Desktop;

namespace ModelViewer.Core {
    public class Viewer : GameWindow {
        public Viewer(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() {
            ClientSize = (width, height),
            Title = title
        }) { }

        static void Main() {
            using(Viewer viewer = new Viewer(800, 600, "Model Viewer")) {
                viewer.Run();
            }
        }
    }
}