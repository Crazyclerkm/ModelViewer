using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader {

enum GL_OBJECT_TYPE {
    Vertex,
    Fragment,
    Program
};

    private int Handle;

    public void Compile(string vertexSource, string fragmentSource) {
        int vertexShader, fragmentShader;

        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        CheckCompileErrors(vertexShader, GL_OBJECT_TYPE.Vertex);

        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        CheckCompileErrors(fragmentShader, GL_OBJECT_TYPE.Fragment);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);
        CheckCompileErrors(Handle, GL_OBJECT_TYPE.Program);

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
    }

    public void Use() {
        GL.UseProgram(Handle);
    }

    public int GetHandle() {
        return Handle;
    }

    public int GetAttribLocation(string attribName) {
        int atrribLocation = GL.GetAttribLocation(Handle, attribName);
        return atrribLocation;
    }

    public void SetInt(string name, int data) {
        GL.UseProgram(Handle);
        GL.Uniform1(GL.GetUniformLocation(Handle, name), data);
    }

    public void SetMatrix4(string name, Matrix4 data) {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref data);
    }

    public void SetVec3(string name, Vector3 data) {
        GL.UseProgram(Handle);
        GL.Uniform3(GL.GetUniformLocation(Handle, name), data.X, data.Y, data.Z);
    }

    private void CheckCompileErrors(int objectID, GL_OBJECT_TYPE type) {
        int success;
        string infoLog;

        if(type != GL_OBJECT_TYPE.Program) {
            GL.GetShader(objectID, ShaderParameter.CompileStatus, out success);
            if (success == 0) {
                infoLog = GL.GetShaderInfoLog(objectID);
                Console.WriteLine(infoLog);
            }
        } else {
            GL.GetProgram(objectID, GetProgramParameterName.LinkStatus, out success);
            if (success == 0) {
                infoLog = GL.GetProgramInfoLog(objectID);
                Console.WriteLine(infoLog);
            }
        }
    }
}