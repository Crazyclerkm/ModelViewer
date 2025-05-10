using ImGuiNET;

using ModelViewer.Graphics;

namespace ModelViewer.UI {
    public class LightingWindow {

        private Light? SelectedLight;
        public void Render(List<Light> lights) {
            if (ImGui.Begin("Lights")) {
                ImGui.Columns(2, null, true);

                // Headers
                ImGui.Text($"Lights ({lights.Count})");
                ImGui.NextColumn();
                ImGui.Text("Properties");
                
                ImGui.Separator();
                ImGui.NextColumn();
                
                // Light selection

                float availableHeight = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing() - 4.0f;
                ImGui.BeginChild("Light List", new System.Numerics.Vector2(0, availableHeight));
                for (int i=0; i<lights.Count; i++) {
                    if (ImGui.Selectable($"[{i}] {lights[i].Type}", SelectedLight == lights[i])) {
                        SelectedLight = lights[i];
                    }
                }
                ImGui.EndChild();

                ImGui.BeginChild("Light Button");
                if (ImGui.Button("New Light", new System.Numerics.Vector2(-1, 0))) {
                    Light newLight = new();
                    lights.Add(newLight);

                    SelectedLight = newLight;
                }
                ImGui.EndChild();
                
                ImGui.NextColumn();

                // Selected Light properties
                ImGui.BeginChild("Light Properties");

                ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 16);

                if (SelectedLight != null) {
                    if(ImGui.BeginCombo("Light Type", SelectedLight.Type.ToString())) {
                        foreach (LightType lightType in Enum.GetValues(typeof(LightType))) {
                            if (ImGui.Selectable(lightType.ToString())) {
                                SelectedLight.Type = lightType;
                            }
                        }
                        ImGui.EndCombo();
                    }

                    bool showPosition = true;
                    bool showDirection = true;

                    switch(SelectedLight.Type) {
                        case LightType.DIRECTIONAL:
                            showPosition = false;
                            break;
                        case LightType.POINT:
                            showDirection = false;
                            break;
                    }

                    ImGui.BeginDisabled(!showPosition);
                    ImGuiHelpers.InputVector3("Position", ref SelectedLight.Position);
                    ImGui.EndDisabled();

                    ImGui.BeginDisabled(!showDirection);
                    ImGuiHelpers.InputVector3("Direction", ref SelectedLight.Direction);
                    ImGui.EndDisabled();
                    
                    ImGuiHelpers.ColorEdit3("Ambient Colour", ref SelectedLight.Ambient);
                    ImGuiHelpers.ColorEdit3("Diffuse Colour", ref SelectedLight.Diffuse);
                    ImGuiHelpers.ColorEdit3("Specular Colour", ref SelectedLight.Specular);

                } else {
                    ImGui.Text("Select a light to view it's properties");
                }
                ImGui.PopStyleVar();
                ImGui.EndChild();

               
                ImGui.Columns(1);
                
            }
            ImGui.End();
        }
    }
}