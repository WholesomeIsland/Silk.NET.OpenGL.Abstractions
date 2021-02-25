using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions
{
    public class GLObjTextured : GLObject
    {
        List<Texture> textures = new List<Texture>();
        public GLObjTextured(GL gl, float[] verts, uint[] indices, string vertPath, string fragPath, bool isDynamic, List<Texture> textures) : base(gl, verts, indices, vertPath, fragPath, isDynamic)
        {
            this.textures = textures;
        }
        public unsafe override void Render()
        {
            Vao.Bind();
            for (int i = 0; i <= textures.Count; i++)
            {
                textures[i].Bind((TextureUnit)i);
            }
            shader.Use();
            gl.DrawElements((GLEnum)PrimitiveType.Triangles, (uint)indices.Length, GLEnum.UnsignedInt, null);
        }
    }
}
