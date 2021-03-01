using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Abstractions
{
    public class GLObjTextured : GLObject
    {
        List<Texture> textures = new List<Texture>();
        float[] verts;
        public GLObjTextured(GL gl, float[] verts, uint[] indices, string vertPath, string fragPath, bool isDynamic, List<Texture> textures) : base(gl, verts, indices, vertPath, fragPath, isDynamic, 5)
        {
            this.textures = textures;
            this.verts = verts;
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3); 
            gl.EnableVertexAttribArray(1);
        }
        public unsafe override void Render()
        {
            Vao.Bind();
            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].Bind((TextureUnit)i);
            }
            shader.Use();
            if (indices != null)
            {
               gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, GLEnum.UnsignedInt, null);
            }
            else
            {
                gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)verts.Length);
            }
        }
        public unsafe override void Dispose()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            shader.Dispose();
            foreach (var item in textures)
            {
                item.Dispose();
            }
        }
    }
}
