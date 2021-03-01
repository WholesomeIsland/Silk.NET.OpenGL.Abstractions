using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using Abstractions.math;
using System.Text;

namespace Abstractions
{
    public class GLObject
    {
         public Shader shader;
         public VBO Vbo;
         public EBO Ebo;
         public VAO Vao;
        public GL gl;
        public uint[] indices;
        /// <summary>
        /// Creates a new GLObject
        /// </summary>
        /// <param name="Gl">the GL instance</param>
        /// <param name="verts">float[] of vertices</param>
        /// <param name="indices">uint[] of vertices</param>
        /// <param name="vertPath">the path to the vertex shader</param>
        /// <param name="fragPath">the path to the fragment shader</param>
        /// <param name="isDynamic">is the vertices going to change often (WARNING More expensive to draw the object if this is true)</param>
        public GLObject(GL Gl, float[] verts, uint[] indices, string vertPath, string fragPath, bool isDynamic, uint VertexSize)
        {
            this.gl = Gl;
            this.indices = indices;
            shader = new Shader(Gl, vertPath, fragPath);
            Ebo = new EBO(Gl, indices);
            Vbo = new VBO(Gl, verts, isDynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
            Vao = new VAO(Gl, Vbo, Ebo);
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, VertexSize, 0); 
            gl.EnableVertexAttribArray(0);
        }
        public virtual void Dispose()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            shader.Dispose();
        }
        public unsafe void SetUniformfv(string uniformName, Matrix4x4 value)
        {
            this.shader.SetUniform(uniformName, value);
        }
        public virtual unsafe void Render()
        {
            Vao.Bind();
            shader.Use();
            gl.DrawElements((GLEnum)PrimitiveType.Triangles, (uint)indices.Length, GLEnum.UnsignedInt, null);
        }
    }
}
