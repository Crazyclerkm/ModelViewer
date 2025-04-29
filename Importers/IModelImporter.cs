using ModelViewer.Graphics;

namespace ModelViewer.Importers {
    public interface IModelImporter {
        Model LoadModel(string filePath);
    }
}