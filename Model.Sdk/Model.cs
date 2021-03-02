using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp;
namespace Abstractions.Model
{
    namespace GL
    {
        /* This File is code based on https://learnopengl.com/Model-Loading/Model and https://learnopengl.com/Model-Loading/Mesh. 
         * these tutorials are made by Joey De Vries and I am grateful for his tutorials
         * this is a translation of a hybrid between the Mesh And Model class to c# and assimp.net
         */
        public struct Vertex
        {
            public Vector3D position;
            public Vector3D normal;
            public Vector2D TexCoord;
            public Vertex(Vector3D pos, Vector3D norm, Vector2D texcoords)
            {
                position = pos; normal = norm; TexCoord = texcoords;
            }
        }
        public class GLModel
        {
            Silk.NET.OpenGL.GL gl;
			public Scene model;
            public List<Mesh> meshes;
            public List<GLObjTextured> objs;
            string vertPath, fragPath;
            bool isDynamic;
            void processNode(Node node, Scene scene)
            {
                for (int i = 0; i < node.MeshCount; i++)
                {
                    Mesh mesh = scene.Meshes[i];
                    meshes.Add(mesh);
                    objs.Add(processMesh(mesh, scene));
                }
                for (int i = 0; i < node.ChildCount; i++)
                {
                    processNode(node.Children[i], scene);
                }
            }
            GLObjTextured processMesh(Mesh mesh, Scene scene)
            {
                List<Vertex> vlist = new List<Vertex>();
                List<uint> ind = new List<uint>();
                List<Texture> tex = new List<Texture>();
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    Vertex v = new Vertex(mesh.Vertices[i], mesh.Normals[i], new Vector2D(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y));
                    vlist.Add(v);
                }
                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    Face face = mesh.Faces[i];
                    for (int j = 0; j < face.IndexCount; j++)
                    {
                        ind.Add((uint)face.Indices[j]);
                    }
                }
                var matIndex = scene.Materials[mesh.MaterialIndex];
                List<Texture> diffTex = LoadTextures(matIndex, TextureType.Diffuse);
                List<Texture> specTex = LoadTextures(matIndex, TextureType.Specular);
                List<Texture> normTex = LoadTextures(matIndex, TextureType.Normals);
                tex.AddRange(diffTex);
                tex.AddRange(specTex);
                tex.AddRange(normTex);
                return new GLObjTextured(gl, vlistToflist(vlist).ToArray(), ind.ToArray(), vertPath, fragPath, isDynamic, tex);
            }
            List<Texture> LoadTextures(Material mat, TextureType type)
            {
                List<Texture> retval = new List<Texture>();
                for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
                {
                    mat.GetMaterialTexture(type, i, out var tex);
                    Console.WriteLine(tex.FilePath);
                    retval.Add(texFromFile(tex.FilePath));
                }
                return retval;
            }
            Texture texFromFile(string path)
            {
                return new Texture(gl, path, SixLabors.ImageSharp.Processing.FlipMode.None);
            }
            List<float> vlistToflist(List<Vertex> vlist)
            {
                List<float> retval = new List<float>();
                foreach (var item in vlist)
                {
                    retval.Add(item.position.X);
                    retval.Add(item.position.Y);
                    retval.Add(item.position.Z);
                    retval.Add(item.TexCoord.X);
                    retval.Add(item.TexCoord.Y);
                }
                return retval;
            }
            public GLModel(Silk.NET.OpenGL.GL _gl, string ModelName, string vertPath, string FragPath, bool isDynamic)
            {
                this.vertPath = vertPath;
                this.fragPath = FragPath;
                this.isDynamic = isDynamic;
                this.gl = _gl;
                meshes = new List<Mesh>();
                objs = new List<GLObjTextured>();
                var ctx = new AssimpContext();
                model = ctx.ImportFile(ModelName, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
                processNode(model.RootNode, model);
            }
            public void SetMatrix4(string name, Abstractions.math.Matrix4x4 matrix)
            {
                foreach (var item in objs)
                {
                    try
                    {
                        item.shader.SetUniform(name, matrix);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            public void SetFloat(string name, float f)
            {
                foreach (var item in objs)
                {
                    try
                    {
                        item.shader.SetUniform(name, f);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            public void SetInt(string name, int i)
            {
                foreach (var item in objs)
                {
                    try
                    {
                        item.shader.SetUniform(name, i);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            public void SetVec2(string name, Vector2 vec)
            {
                foreach (var item in objs)
                {
                    try
                    {
                        item.shader.SetUniform(name, vec);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            public void Draw(Abstractions.math.Matrix4x4 proj, math.Matrix4x4 view, math.Matrix4x4 model, string[] uniformNames)
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    objs[i].SetUniformfv(uniformNames[0], proj);
                    objs[i].SetUniformfv(uniformNames[1], view);
                    objs[i].SetUniformfv(uniformNames[2], model);
                    objs[i].Render();
                }
            }
            public void Dispose()
            {
                foreach (var item in objs)
                {
                    item.Dispose();
                }
            }
        }
    }
}
