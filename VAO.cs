using Silk.NET.OpenGL;
using System;
namespace Abstractions{
  public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        //Our handle and the GL instance this class will use, these are private because they have no reason to be public.
        //Most of the time you would want to abstract items to make things like this invisible.
        private uint _handle;
        private GL _gl;

        public VertexArrayObject(GL gl)
        {
            //Saving the GL instance.
            _gl = gl;

            //Setting out handle and binding the VBO and EBO to this VAO.
            _handle = _gl.GenVertexArray();
        }

        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            //Setting up a vertex attribute pointer
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(TVertexType), (void*) (offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            //Binding the vertex array.
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            //Remember to dispose this object so the data GPU side is cleared.
            //We dont delete the VBO and EBO here, as you can have one VBO stored under multiple VAO's.
            _gl.DeleteVertexArray(_handle);
        }
    }
    public class VAO : VertexArrayObject<float, uint>{
      public VAO(GL gl) : base(gl){}
    }
}
