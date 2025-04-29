using ImGuiNET;

using ModelViewer.Graphics;

namespace ModelViewer.UI {
    public class SelectionWindow {
        public void Render(Model selectedModel) {
            if (ImGui.Begin("Selected Model")) {
               
                ImGui.Text($"{selectedModel.Name}");
                ImGui.Separator();
                ImGui.Text($"Meshes ({selectedModel.Meshes.Count})");

                ImGui.PushItemWidth(60);
                ImGui.Text("Position");
                ImGui.InputFloat("##X", ref selectedModel.Position.X);
                ImGui.SameLine();
                ImGui.InputFloat("##Y", ref selectedModel.Position.Y);
                ImGui.SameLine();
                ImGui.InputFloat("##Z", ref selectedModel.Position.Z);
                
                ImGui.Text("Scale");
                ImGui.InputFloat("##ScaleX", ref selectedModel.Scale.X);
                ImGui.SameLine();
                ImGui.InputFloat("##ScaleY", ref selectedModel.Scale.Y);
                ImGui.SameLine();
                ImGui.InputFloat("##ScaleZ", ref selectedModel.Scale.Z);

                ImGui.Text("Rotation");
                ImGui.SliderAngle("##RotationX", ref selectedModel.Rotation.X); 
                ImGui.SameLine();
                ImGui.SliderAngle("##RotationY", ref selectedModel.Rotation.Y); 
                ImGui.SameLine();
                ImGui.SliderAngle("##RotationZ", ref selectedModel.Rotation.Z); 

                ImGui.PopItemWidth();
            }
            ImGui.End();
        }
    }
}