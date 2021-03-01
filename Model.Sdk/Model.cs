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
                            tmp.Add((float)model.TextureList[item.Index].X);
                            tmp.Add((float)model.TextureList[item.Index].Y);
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
                    foreach (var item in model.FaceList) //I need someway of speeding it up.
                    {
                        try
                        {
                            var test = item.VertexIndexList[3];
                            tmp.Add((uint)item.VertexIndexList[1] - 1); tmp.Add((uint)item.VertexIndexList[2] - 1); tmp.Add((uint)item.VertexIndexList[3] - 1);
                            tmp.Add((uint)item.VertexIndexList[0] - 1); tmp.Add((uint)item.VertexIndexList[1] - 1); tmp.Add((uint)item.VertexIndexList[3] - 1);
                        }
                        catch (System.IndexOutOfRangeException up)// then i could 'throw up'
                        {
                            tmp.Add((uint)item.VertexIndexList[0] - 1); tmp.Add((uint)item.VertexIndexList[1] - 1); tmp.Add((uint)item.VertexIndexList[2] - 1);
                        }
                        foreach (var item1 in item.VertexIndexList)
                        {
                            Console.WriteLine(item1 - 1);
                        }
                    }
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
