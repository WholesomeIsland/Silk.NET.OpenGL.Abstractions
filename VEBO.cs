using Silk.NET.OpenGL;
using System;
namespace Abstractions{
public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private GLEnum _bufferType;
        private GLEnum _bufferUsage;
        private GL _gl;

        public unsafe BufferObject(GL gl, GLEnum bufferType, GLEnum StaticOrDynamic)
        {
            _gl = gl;
            _bufferType = bufferType;
        
            _handle = _gl.GenBuffer();
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, _handle);
        }
        
        public void Upload(Span<TDataType> data)
        {
            fixed (void* d = data)
            {
                _gl.BufferData(_bufferType, (nuint) (data.Length * sizeof(TDataType)), d, _bufferUsage);
            }
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
        }
    }

public class VBO : BufferObject<float>{
  public VBO(GL gl, Span<float> data, GLEnum SOD) : base(gl, data,GLEnum.ArrayBuffer, SOD){}
}

public class EBO : BufferObject<uint>{
  public EBO(GL gl, Span<uint> data) : base(gl, data,GLEnum.ElementArrayBuffer, GLEnum.StaticDraw){}
}
}
