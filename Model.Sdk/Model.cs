using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Game.NET.Parser;
namespace Abstractions.Model
{
    namespace GL
	{
		public class Obj
        {
			public List<Vector3> vertices;
            public List<Vector2> texCoords;

            public List<uint> Indices;

            public Obj(string filename)
            {
                var strlist = File.ReadAllText(filename).Split("\n");
                vertices = new List<Vector3>();
                texCoords = new List<Vector2>();
                Parse(strlist);
            }
			void Parse(string[] lines)
            {
                foreach (var item in lines)
                {
                    if (string.IsNullOrEmpty(item) || item[0] == '#')
                    {
                        continue;
                    }
                    if (item[0] == 'v' && item[1] == ' ')
                    {
                        parseVertex(item.Split(' '));
                    }
                    else if (item[0] == 'v' && item[1] == 't')
                    {
                        parseTexCoord(item.Split(' '));
                    }
                    else if (item[0] == 'f')
                    {
                        parseFace(item.Trim().Split(new char[] {' ','/' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
            private void parseVertex(String[] Pos)
            {
                Vector3 Vtx = new Vector3(
                    System.Convert.ToSingle(Pos[1]),
                    System.Convert.ToSingle(Pos[2]),
                    System.Convert.ToSingle(Pos[3]));
                vertices.Add(Vtx);
            }
            private void parseTexCoord(string[] uvw)
            {
                Vector2 uv = new Vector2(Convert.ToSingle(uvw[1]), Convert.ToSingle(uvw[2]));
                texCoords.Add(uv);
            }
            private void parseFace(string[] data)
            {
                Console.WriteLine(data[1]);
                Indices.Add((uint)int.Parse(data[1]));
                Indices.Add((uint)int.Parse(data[2]));
                Indices.Add((uint)int.Parse(data[3]));
            }
        }
        public class ObjGLModel
        {
			public Obj model;
            public Game.NET.Parser.ObjFileParser parser;
            public float[] vertices { 
				get {
					List<float> tmp = new List<float>();
                    foreach(var item in model.vertices)
                    {
						tmp.Add(item.X);
						tmp.Add(item.Y);
						tmp.Add(item.Z);
                    }
					return tmp.ToArray();
				} 
			}
            public float[] vertsWithTexCoords
            {
                get
                {
                    unsafe
                    {
                        List<float> tmp = new List<float>();
                        for(int i = 0; i < model.vertices.Count; i++)
                        {
                            tmp.Add(model.vertices[i].X);
                            tmp.Add(model.vertices[i].Y);
                            tmp.Add(model.vertices[i].Z);
                            tmp.Add(model.texCoords[i].X);
                            tmp.Add(model.texCoords[i].Y);
                        }
                        return tmp.ToArray();
                    }
                }
            }
			public uint[] indices
            {
                get
                {
					return model.Indices.ToArray();
				}
            }
            public ObjGLModel(string ModelName)
            {
                model = new Obj(ModelName);
            }
        }
    }
}
