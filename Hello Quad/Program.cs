using Abstractions;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing;
using System.Drawing;

namespace Hello_Quad
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
        static EBO ebo;
        static VBO vbo;
        static VAO vao;
        static Abstractions.Shader shader;
        static unsafe void Main(string[] args)
        {
            var Options = WindowOptions.Default;
            Options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            Options.Title = "LearnOpenGL with Silk.NET";
            var window = Window.Create(Options);
            window.Render += (obj) => {
                GL.GetApi(window).ClearColor(Color.Black);
                GL.GetApi(window).Clear((uint)ClearBufferMask.ColorBufferBit);
                vao.Bind();
                shader.Use();
                GL.GetApi(window).DrawElements((GLEnum)PrimitiveType.Triangles, (uint)Indices.Length, GLEnum.UnsignedInt, null);
            };
            window.Load += () => {

                vbo = new VBO(GL.GetApi(window), Vertices, GLEnum.StaticDraw);
                ebo = new EBO(GL.GetApi(window), Indices);
                vao = new VAO(GL.GetApi(window), vbo, ebo);
                shader = new Abstractions.Shader(GL.GetApi(window), "shader.vert", "shader.frag");
                vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 0, 0);
            };
            window.Closing += () => {
                vbo.Dispose();
                ebo.Dispose();
                vao.Dispose();
                shader.Dispose();
            };
            window.Run();
        }
    }
}
