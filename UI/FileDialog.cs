using ImGuiNET;

namespace ModelViewer.UI {
    public class FileDialog {

        private string CurrentPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
        private string? SelectedFile = null;
        private bool Open = false;
        private Action<string>? FileSelected;

        private readonly string[] SupportedExtensions = [
            ".obj"
        ];

        public void Show(Action<object> fileSelectedCallback) {
            Open = true;
            FileSelected = fileSelectedCallback;
        }

        public void Render() {
            if (!Open) return;

            if (ImGui.BeginPopupModal("Open File", ref Open, ImGuiWindowFlags.AlwaysAutoResize)) {
                ImGui.Text("Current Directory: ");
                ImGui.TextWrapped(CurrentPath);
                ImGui.Separator();

                var parentDir = Directory.GetParent(CurrentPath);

                if (parentDir != null) {
                    if (ImGui.Selectable("..", false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                        CurrentPath = parentDir.FullName;
                    }
                }

                foreach (string dir in Directory.GetDirectories(CurrentPath)) {
                    string dirName = Path.GetFileName(dir);
                    if (ImGui.Selectable("["+ dirName+"]", false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                        CurrentPath = dir;
                    }
                }

                foreach (string file in Directory.GetFiles(CurrentPath)) {
                    string fileName = Path.GetFileName(file);
                    string extension = Path.GetExtension(file);
                    if (SupportedExtensions.Contains(extension)) {
                        if (ImGui.Selectable(fileName, false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                            SelectedFile = file;
                        }
                    }
                }

                ImGui.Separator();
                ImGui.Text($"Selected: {Path.GetFileName(SelectedFile) ?? "None"}");

                if (ImGui.Button("Open") && SelectedFile != null) {
                    FileSelected?.Invoke(SelectedFile);
                    SelectedFile = null;
                    Open = false;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel")) {
                    SelectedFile = null;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }
}