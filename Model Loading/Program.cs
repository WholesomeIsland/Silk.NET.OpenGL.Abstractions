using Abstractions;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Abstractions.Model;
using System.Drawing;
using System.Collections.Generic;
using Texture = Abstractions.Texture;

namespace Model_Loading
{
    class Program
    {
        static float[] AsV3DToFArr(Assimp.Vector3D[] arr)
        {
            List<float> rtnval = new List<float>();
            foreach (var item in arr)
            {
                rtnval.Add(item.X);
                rtnval.Add(item.Y);
                rtnval.Add(item.Z);
            }
            return rtnval.ToArray();
        }
        static GLObjTextured globject;
        static Model model;
        static Texture texture;
        static void Main(string[] args)
        {
            var Options = WindowOptions.Default;
            Options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            Options.Title = "LearnOpenGL with Silk.NET";
            var window = Window.Create(Options);
            window.Render += (obj) => {
                GL.GetApi(window).ClearColor(Color.Black);
                GL.GetApi(window).Clear((uint)ClearBufferMask.ColorBufferBit);
                texture.Bind();
                globject.Render();
            };
            window.Load += () => {
                model = new Model("Model.fbx", Assimp.PostProcessSteps.None);
                List<Texture> tex = new List<Texture>();
                tex.Add(new Texture(GL.GetApi(window), "Diffuse.png", 0));
                globject = new GLObjTextured(GL.GetApi(window), AsV3DToFArr(model.getVertsFromModel(0)), null, "Shader.vert", "Shader.frag", false, tex);
            };
            window.Closing += () => {
                globject.Dispose();
            };
            window.Run();
        }
    }
}
