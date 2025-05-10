using ImGuiNET;

using ModelViewer.Graphics;

namespace ModelViewer.UI {
    public class SelectionWindow {
        public void Render(Model selectedModel) {
            if (ImGui.Begin("Selected Model")) {
               
                ImGui.Text($"{selectedModel.Name}");
                ImGui.Separator();
                ImGui.Text($"Meshes ({selectedModel.Meshes.Count})");

                ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 16);

                ImGuiHelpers.InputVector3("Position", ref selectedModel.Position);
                ImGuiHelpers.InputVector3("Scale", ref selectedModel.Scale);
                ImGuiHelpers.SliderAngle3("Rotation", ref selectedModel.Rotation);
                
                ImGui.PopStyleVar();
            }
            ImGui.End();
        }
    }
}