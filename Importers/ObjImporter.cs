using OpenTK.Mathematics;

using ModelViewer.Resources;
using ModelViewer.Graphics;

namespace ModelViewer.Importers {
    public class ObjImporter : IModelImporter {

        private struct Face {
            public List<int> PositionIndices;
            public List<int> TexCoordIndices;
            public List<int> NormalIndices;

        }

        public Model LoadModel(string filePath) {
            List<Mesh> meshes = [];

            List<Vector3> positions = [];
            List<Vector2> texCoords = [];
            List<Vector3> normals = [];

            List<Face> faces = [];

            Dictionary<string, Material> materials = [];

            Material? currentMaterial = null;

            string? baseDir = Path.GetDirectoryName(Path.GetFullPath(filePath));

            foreach (var line in File.ReadLines(filePath)) {
                var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                // Ignore comments and empty lines
                if (tokens.Length == 0 || tokens[0].StartsWith('#')) continue;
                
                switch (tokens[0]) {
                    case "v": {
                        positions.Add(ParseVec3([.. tokens.Skip(1)]));
                        break;
                    }
                    case "vt": {
                        texCoords.Add(ParseTexCoord([.. tokens.Skip(1)]));
                        break;
                    }
                    case "vn": {
                        normals.Add(ParseVec3([.. tokens.Skip(1)]));
                        break;
                    }
                    case "f": {
                        faces.Add(ParseFace([.. tokens.Skip(1)]));
                        break;
                    }
                    case "o": {

                        if (faces.Count > 0) {
                            meshes.Add(CreateMesh(positions, texCoords, normals, faces, currentMaterial));

                            faces.Clear();

                            currentMaterial = null;
                        }
                        
                        break;
                    }
                    case "g": {
                        break;
                    }
                    case "mtllib": {
                        // Doesn't support spaces in material library name

                        if (baseDir != null) {
                            Dictionary<string, Material> loadedMaterials = LoadMaterialLib(baseDir, tokens[1]);

                            foreach (var name in loadedMaterials.Keys) {
                                Material material =  loadedMaterials[name];  
                                materials[name] = material;
                            }
                        }
                        break;
                    }
                    case "usemtl": {
                        // Doesn't support spaces in material name
                        currentMaterial = materials[tokens[1]];
                        break;
                    }
                }
            }

            meshes.Add(CreateMesh(positions, texCoords, normals, faces, currentMaterial));

            Model model = new Model(meshes, Path.GetFileNameWithoutExtension(filePath));

            return model;
        }

        private Mesh CreateMesh(List<Vector3> positions, List<Vector2> texCoords, List<Vector3> normals, List<Face> faces, Material? material) {
            List<Vertex> vertices = [];
            List<uint> indices = [];
            
            Material meshMaterial = material ?? new Material("default");

            foreach (var face in faces) {
                for (int i = 0; i < face.PositionIndices.Count; i++) {

                    // Negative indices are currently not supported
                    int positionIndex = face.PositionIndices[i] - 1;

                    Vector3 position = positions[positionIndex];

                    int texCoordsIndex = (i < face.TexCoordIndices.Count) ? face.TexCoordIndices[i] - 1 : -1;
                    
                    Vector2 texCoord = (texCoordsIndex >= 0) ? texCoords[texCoordsIndex] : Vector2.Zero;

                    int normalIndex = (i < face.NormalIndices.Count) ? face.NormalIndices[i] - 1 : -1;
                    Vector3 normal = (normalIndex >= 0) ? normals[normalIndex] : Vector3.Zero;

                    Vertex vertex = new Vertex(position, normal, texCoord);
                    uint index = (uint)vertices.Count;

                    vertices.Add(vertex);
                    indices.Add(index);

                }
            }

            return new Mesh(vertices, indices, meshMaterial);
        }

        private Vector3 ParseVec3(string[] tokens) {
            float x = float.Parse(tokens[0]);
            float y = float.Parse(tokens[1]);
            float z = float.Parse(tokens[2]);
            return new Vector3(x, y, z);
        }

        // Currently ignores w component
        private Vector2 ParseTexCoord(string[] tokens) {
            float u = float.Parse(tokens[0]);
            float v = tokens.Length > 1 ? 1.0f-float.Parse(tokens[1]) : 0.0f;

            return new Vector2(u, v);
        }

        // A face contains some number of vertices. Each vertex may also have a corresponding texture and/or normal.
        // These are specified as indices into arrays containing all previously processed vertices/textures/normals.
        private Face ParseFace(string[] tokens) {
            Face face = new() {
                PositionIndices = [],
                TexCoordIndices = [],
                NormalIndices = []
            };

            foreach (var vertex in tokens) {

                string[] indices = vertex.Split("/");

                face.PositionIndices.Add(int.Parse(indices[0]));

            if (indices.Length > 1 && !string.IsNullOrEmpty(indices[1])) {
                    face.TexCoordIndices.Add(int.Parse(indices[1]));
            }

            if (indices.Length > 2 && !string.IsNullOrEmpty(indices[2])) {
                    face.NormalIndices.Add(int.Parse(indices[2]));
            }
            }

            return face;
        }

        private Dictionary<string, Material> LoadMaterialLib(string baseDir, string filePath) {
            Dictionary<string, Material> materials = [];

            Material? currentMaterial = null;
            
            foreach (var line in File.ReadLines(Path.Combine(baseDir, filePath))) {
                var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 0 || tokens[0].StartsWith('#')) continue;

                switch (tokens[0]) {
                    case "newmtl": {

                        if (currentMaterial != null) {
                            materials[currentMaterial.Name] = currentMaterial;
                        } 

                        currentMaterial = new Material(tokens[1]);
                        break;
                    }
                    case "Ka": {

                        if (currentMaterial != null) {
                            currentMaterial.AmbientColour = ParseVec3([.. tokens.Skip(1)]);
                        }

                        break;
                    }
                    case "Kd": {
                        if (currentMaterial != null) {
                            currentMaterial.DiffuseColour = ParseVec3([.. tokens.Skip(1)]);
                        }

                        break;
                    }
                    case "Ks": {
                        if (currentMaterial != null) {
                            currentMaterial.SpecularColour = ParseVec3([.. tokens.Skip(1)]);
                        }

                        break;
                    }
                    case "Ns": {
                        if (currentMaterial != null) {
                            currentMaterial.SpecularExponent = float.Parse(tokens[1]);
                        }

                        break;
                    }
                    case "Ni": {
                        if (currentMaterial != null) {
                            currentMaterial.OpticalDensity = float.Parse(tokens[1]);
                        }

                        break;
                    }
                    case "map_Kd": {

                        if (currentMaterial != null) {
                            currentMaterial.DiffuseTexture = ResourceManager.LoadTexture(Path.Combine(baseDir, tokens[1]));
                        }

                        break;
                    }
                    case "map_Ks": {

                        if (currentMaterial != null) {
                            currentMaterial.SpecularColourTexture = ResourceManager.LoadTexture(Path.Combine(baseDir, tokens[1]));
                        }

                        break;
                    }
                    case "map_Ns": {

                        if (currentMaterial != null) {
                            currentMaterial.SpecularHighlightTexture = ResourceManager.LoadTexture(Path.Combine(baseDir, tokens[1]));
                        }

                        break;
                    }
                }
            }

            if (currentMaterial != null) {
                materials[currentMaterial.Name] = currentMaterial;
            }

            return materials;
        }
    
    }
}