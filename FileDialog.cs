using ImGuiNET;

public class FileDialog {

    private string currentPath = Directory.GetCurrentDirectory();
    private string? selectedFile = null;
    private bool open = false;
    private Action<string>? fileSelected;

    private readonly string[] SupportedExtensions = {
        ".obj"
    };

    public void Show(Action<object> fileSelectedCallback) {
        open = true;
        fileSelected = fileSelectedCallback;
    }

    public void Render() {
        if (!open) return;

        if (ImGui.BeginPopupModal("Open File", ref open, ImGuiWindowFlags.AlwaysAutoResize)) {
            ImGui.Text("Current Directory: ");
            ImGui.TextWrapped(currentPath);
            ImGui.Separator();

            var parentDir = Directory.GetParent(currentPath);

            if (parentDir != null) {
                if (ImGui.Selectable("..", false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                    currentPath = parentDir.FullName;
                }
            }

            foreach (string dir in Directory.GetDirectories(currentPath)) {
                string dirName = Path.GetFileName(dir);
                if (ImGui.Selectable("["+ dirName+"]", false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                    currentPath = dir;
                }
            }

            foreach (string file in Directory.GetFiles(currentPath)) {
                string fileName = Path.GetFileName(file);
                string extension = Path.GetExtension(file);
                if (SupportedExtensions.Contains(extension)) {
                    if (ImGui.Selectable(fileName, false, ImGuiSelectableFlags.NoAutoClosePopups)) {
                        selectedFile = file;
                    }
                }
            }

            ImGui.Separator();
            ImGui.Text($"Selected: {Path.GetFileName(selectedFile) ?? "None"}");

            if (ImGui.Button("Open") && selectedFile != null) {
                fileSelected?.Invoke(selectedFile);
                selectedFile = null;
                open = false;
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();

            if (ImGui.Button("Cancel")) {
                selectedFile = null;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
}