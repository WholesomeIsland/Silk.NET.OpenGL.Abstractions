using Abstractions;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using System.Drawing;

namespace Helo_Quad_Abstractions
{
    class Program
    {
        private static readonly float[] Vertices =
           {
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        };

        //Index data, uploaded to the EBO.
        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };
        static GLObject globject;
        static unsafe void Main(string[] args)
        {
            var Options = WindowOptions.Default;
            Options.Size = new Size(800, 600);
            Options.Title = "LearnOpenGL with Silk.NET";
            var window = Window.Create(Options);
            window.Render += (obj) => {
                GL.GetApi(window).ClearColor(Color.Black);
                GL.GetApi(window).Clear((uint)ClearBufferMask.ColorBufferBit);
                globject.Render();
            };
            window.Load += () => {
                globject = new GLObject(GL.GetApi(window), Vertices, Indices, "shader.vert", "shader.frag", false);
            };
            window.Closing += () => {
                globject.Dispose();
            };
            window.Run();
        }
    }
}
