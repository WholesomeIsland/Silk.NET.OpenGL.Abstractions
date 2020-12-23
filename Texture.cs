using Silk.NET.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Abstraactions
{
    internal static class ImageEXT
    {
        static internal byte[] ToByteArray(this Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
    public class Texture : IDisposable
    {
        private uint _handle;
        private GL _gl;
        
        public unsafe Texture(GL gl, string path, int TextureSlot)
        {
            var img = System.Drawing.Image.FromFile(path);
            fixed (void* data = &MemoryMarshal.GetReference(new Span<byte>(img.ToByteArray())))
            {
                Load(gl, data, (uint) img.Width, (uint) img.Height, TextureSlot);
            }

        }

        public unsafe Texture(GL gl, Span<byte> data, uint width, uint height, int TextureSlot)
        {
            fixed (void* d = &data[0])
            {
                Load(gl, d, width, height, TextureSlot);
            }
        }

        private unsafe void Load(GL gl, void* data, uint width, uint height, int TextureSlot)
        {
            _gl = gl;

            _handle = _gl.GenTexture();
            Bind();

            _gl.TexImage2D(TextureTarget.Texture2D, TextureSlot, (int) InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(_handle);
        }
    }
}