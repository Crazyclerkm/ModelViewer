using ImGuiNET;
using OpenTK.Mathematics;

namespace ModelViewer.UI {

    // Collection of wrapper functions to use OpenTK vectors instead of System.Numerics vectors with ImGui functions
    // as well as custom ImGui components
    public class ImGuiHelpers {

        private delegate bool ImGuiVector3Editor(string label, ref System.Numerics.Vector3 vector);

        private static bool EditVector3(string label, ref Vector3 vector, ImGuiVector3Editor imguiFunc) {
            var sysVector = new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
            bool changed = imguiFunc(label, ref sysVector);

            if (changed) {
                vector = new Vector3(sysVector.X, sysVector.Y, sysVector.Z);
            }

            return changed;
        }

        public static bool InputVector3(string label, ref Vector3 vector) {
            return EditVector3(label, ref vector, ImGui.InputFloat3);
        }

        public static bool DragVector3(string label, ref Vector3 vector) {
            return EditVector3(label, ref vector, ImGui.DragFloat3);
        }

        public static bool ColorEdit3(string label, ref Vector3 vector) {
            return EditVector3(label, ref vector, ImGui.ColorEdit3);
        }

        public static bool ColorPicker3(string label, ref Vector3 vector) {
            return EditVector3(label, ref vector, ImGui.ColorPicker3);
        }

        public static bool SliderAngle3(string label, ref Vector3 vector) {
            bool changed = false;

            ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, 5);
            ImGui.PushID(label);

            float itemWidth = (ImGui.CalcItemWidth() - 8) / 3;
            ImGui.PushItemWidth(itemWidth);

            changed |= ImGui.SliderAngle("##X", ref vector.X);
            ImGui.SameLine(0.0f, 4.0f);
            changed |= ImGui.SliderAngle("##Y", ref vector.Y);
            ImGui.SameLine(0.0f, 4.0f);
            changed |= ImGui.SliderAngle("##Z", ref vector.Z);
            ImGui.SameLine(0.0f, 4.0f);
            ImGui.Text(label);

            ImGui.PopItemWidth();

            ImGui.PopID();
            ImGui.PopStyleVar();

            return changed;
        }
    }
}