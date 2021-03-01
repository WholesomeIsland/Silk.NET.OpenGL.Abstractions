using System;
using System.Collections.Generic;
using ObjParser;
namespace Abstractions.Model
{
    namespace GL
    {
        public class ObjGLModel
        {
			public Obj model;
            public float[] vertices { 
				get {
					List<float> tmp = new List<float>();
                    foreach(var item in model.VertexList)
                    {
						tmp.Add((float)item.X);
						tmp.Add((float)item.Y);
						tmp.Add((float)item.Z);
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
                        for (int i = 0; i<model.VertexList.Count; i++)
                        {
                            var item = model.VertexList[i];
                            tmp.Add((float)item.X);
                            tmp.Add((float)item.Y);
                            tmp.Add((float)item.Z);
                            tmp.Add((float)model.TextureList[i].X);
                            tmp.Add((float)model.TextureList[i].Y);
                        }
                        return tmp.ToArray();
                    }
                }
            }
			public uint[] indices
            {
                get
                {
                    List<uint> tmp = new List<uint>();
                    foreach (var item in model.FaceList)
                    {
                        foreach (var item1 in item.VertexIndexList)
                        {
                            tmp.Add((uint)item1);
                            Console.WriteLine(item1);
                        }
                    }
                    tmp.Reverse();
                    return tmp.ToArray();
				}
            }
            public ObjGLModel(string ModelName)
            {
                model = new Obj();
                model.LoadObj(ModelName);
            }
        }
    }
}
