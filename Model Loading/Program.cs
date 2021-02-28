using Abstractions;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Abstractions.Model.GL;
using System.Drawing;
using System.Collections.Generic;
using Texture = Abstractions.Texture;

namespace Model_Loading
{
    class Program
    {
        static GLObjTextured globject;
        static FbxGLModel model;
        static void Main(string[] args)
        {
            var Options = WindowOptions.Default;
            Options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            Options.Title = "LearnOpenGL with Silk.NET";
            var window = Window.Create(Options);
            window.Render += (obj) => {
                GL.GetApi(window).ClearColor(Color.Black);
                GL.GetApi(window).Clear((uint)ClearBufferMask.ColorBufferBit);
                globject.Render();
            };
            window.Load += () => {
                model = new FbxGLModel("Model.obj");
                List<Texture> tex = new List<Texture>();
                tex.Add(new Texture(GL.GetApi(window), "Diffuse.png", 0));
                List<float> verts = new List<float>(model.vertices);
                GL _gl = GL.GetApi(window);
                globject = new GLObjTextured(_gl, verts.ToArray(), null, "Shader.vert", "Shader.frag", false, tex);
            };
            window.Closing += () => {
                globject.Dispose();
            };
            window.Run();
        }
    }
}
