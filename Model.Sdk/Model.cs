using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace Abstractions.Model
{
    namespace GL
	{
		public class Extent
		{
			public double XMax { get; set; }
			public double XMin { get; set; }
			public double YMax { get; set; }
			public double YMin { get; set; }
			public double ZMax { get; set; }
			public double ZMin { get; set; }

			public double XSize { get { return XMax - XMin; } }
			public double YSize { get { return YMax - YMin; } }
			public double ZSize { get { return ZMax - ZMin; } }
		}
		interface IType
		{
			void LoadFromStringArray(string[] data);
		}
		public class Face : IType
		{
			public const int MinimumDataLength = 4;
			public const string Prefix = "f";

			public string UseMtl { get; set; }
			public int[] VertexIndexList { get; set; }
			public int[] TextureVertexIndexList { get; set; }

			public void LoadFromStringArray(string[] data)
			{
				if (data.Length < MinimumDataLength)
					throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

				if (!data[0].ToLower().Equals(Prefix))
					throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

				int vcount = data.Count() - 1;
				VertexIndexList = new int[vcount];
				TextureVertexIndexList = new int[vcount];

				bool success;

				for (int i = 0; i < vcount; i++)
				{
					string[] parts = data[i + 1].Split('/');

					int vindex;
					success = int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out vindex);
					if (!success) throw new ArgumentException("Could not parse parameter as int");
					VertexIndexList[i] = vindex;

					if (parts.Count() > 1)
					{
						success = int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out vindex);
						if (success)
						{
							TextureVertexIndexList[i] = vindex;
						}
					}
				}
			}

			// HACKHACK this will write invalid files if there are no texture vertices in
			// the faces, need to identify that and write an alternate format
			public override string ToString()
			{
				StringBuilder b = new StringBuilder();
				b.Append("f");

				for (int i = 0; i < VertexIndexList.Count(); i++)
				{
					if (i < TextureVertexIndexList.Length)
					{
						b.AppendFormat(" {0}/{1}", VertexIndexList[i], TextureVertexIndexList[i]);
					}
					else
					{
						b.AppendFormat(" {0}", VertexIndexList[i]);
					}
				}

				return b.ToString();
			}
		}
		public class TextureVertex : IType
		{
			public const int MinimumDataLength = 3;
			public const string Prefix = "vt";

			public double X { get; set; }

			public double Y { get; set; }

			public int Index { get; set; }

			public void LoadFromStringArray(string[] data)
			{
				if (data.Length < MinimumDataLength)
					throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

				if (!data[0].ToLower().Equals(Prefix))
					throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

				bool success;

				double x, y;

				success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x);
				if (!success) throw new ArgumentException("Could not parse X parameter as double");

				success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y);
				if (!success) throw new ArgumentException("Could not parse Y parameter as double");
				X = x;
				Y = y;
			}

			public override string ToString()
			{
				return string.Format("vt {0} {1}", X, Y);
			}
		}
		public class Vertex : IType
		{
			public const int MinimumDataLength = 4;
			public const string Prefix = "v";

			public double X { get; set; }

			public double Y { get; set; }

			public double Z { get; set; }

			public int Index { get; set; }

			public void LoadFromStringArray(string[] data)
			{
				if (data.Length < MinimumDataLength)
					throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

				if (!data[0].ToLower().Equals(Prefix))
					throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

				bool success;

				double x, y, z;

				success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x);
				if (!success) throw new ArgumentException("Could not parse X parameter as double");

				success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y);
				if (!success) throw new ArgumentException("Could not parse Y parameter as double");

				success = double.TryParse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture, out z);
				if (!success) throw new ArgumentException("Could not parse Z parameter as double");

				X = x;
				Y = y;
				Z = z;
			}

			public override string ToString()
			{
				return string.Format("v {0} {1} {2}", X, Y, Z);
			}
		}
		public class Obj
		{
			public List<Vertex> VertexList;
			public List<Face> FaceList;
			public List<TextureVertex> TextureList;

			public Extent Size { get; set; }

			public string UseMtl { get; set; }
			public string Mtl { get; set; }

			/// <summary>
			/// Constructor. Initializes VertexList, FaceList and TextureList.
			/// </summary>
			public Obj()
			{
				VertexList = new List<Vertex>();
				FaceList = new List<Face>();
				TextureList = new List<TextureVertex>();
			}

			/// <summary>
			/// Load .obj from a filepath.
			/// </summary>
			/// <param name="file"></param>
			public void LoadObj(string path)
			{
				LoadObj(File.ReadAllLines(path));
			}

			/// <summary>
			/// Load .obj from a stream.
			/// </summary>
			/// <param name="file"></param>
			public void LoadObj(Stream data)
			{
				using (var reader = new StreamReader(data))
				{
					LoadObj(reader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
				}
			}

			/// <summary>
			/// Load .obj from a list of strings.
			/// </summary>
			/// <param name="data"></param>
			public void LoadObj(IEnumerable<string> data)
			{
				foreach (var line in data)
				{
					processLine(line);
				}

				updateSize();
			}

			public void WriteObjFile(string path, string[] headerStrings)
			{
				using (var outStream = File.OpenWrite(path))
				using (var writer = new StreamWriter(outStream))
				{
					// Write some header data
					WriteHeader(writer, headerStrings);

					if (!string.IsNullOrEmpty(Mtl))
					{
						writer.WriteLine("mtllib " + Mtl);
					}

					VertexList.ForEach(v => writer.WriteLine(v));
					TextureList.ForEach(tv => writer.WriteLine(tv));
					string lastUseMtl = "";
					foreach (Face face in FaceList)
					{
						if (face.UseMtl != null && !face.UseMtl.Equals(lastUseMtl))
						{
							writer.WriteLine("usemtl " + face.UseMtl);
							lastUseMtl = face.UseMtl;
						}
						writer.WriteLine(face);
					}
				}
			}

			private void WriteHeader(StreamWriter writer, string[] headerStrings)
			{
				if (headerStrings == null || headerStrings.Length == 0)
				{
					writer.WriteLine("# Generated by ObjParser");
					return;
				}

				foreach (var line in headerStrings)
				{
					writer.WriteLine("# " + line);
				}
			}

			/// <summary>
			/// Sets our global object size with an extent object
			/// </summary>
			private void updateSize()
			{
				// If there are no vertices then size should be 0.
				if (VertexList.Count == 0)
				{
					Size = new Extent
					{
						XMax = 0,
						XMin = 0,
						YMax = 0,
						YMin = 0,
						ZMax = 0,
						ZMin = 0
					};

					// Avoid an exception below if VertexList was empty.
					return;
				}

				Size = new Extent
				{
					XMax = VertexList.Max(v => v.X),
					XMin = VertexList.Min(v => v.X),
					YMax = VertexList.Max(v => v.Y),
					YMin = VertexList.Min(v => v.Y),
					ZMax = VertexList.Max(v => v.Z),
					ZMin = VertexList.Min(v => v.Z)
				};
			}

			/// <summary>
			/// Parses and loads a line from an OBJ file.
			/// Currently only supports V, VT, F and MTLLIB prefixes
			/// </summary>		
			private void processLine(string line)
			{
				string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (parts.Length > 0)
				{
					switch (parts[0])
					{
						case "usemtl":
							UseMtl = parts[1];
							break;
						case "mtllib":
							Mtl = parts[1];
							break;
						case "v":
							Vertex v = new Vertex();
							v.LoadFromStringArray(parts);
							VertexList.Add(v);
							v.Index = VertexList.Count();
							break;
						case "f":
							Face f = new Face();
							f.LoadFromStringArray(parts);
							f.UseMtl = UseMtl;
							FaceList.Add(f);
							break;
						case "vt":
							TextureVertex vt = new TextureVertex();
							vt.LoadFromStringArray(parts);
							TextureList.Add(vt);
							vt.Index = TextureList.Count();
							break;

					}
				}
			}

		}
		public class FbxGLModel
        {
			public Obj model;
            public float[] vertices { 
				get {
					List<float> tmp = new List<float>();
                    foreach (var item in model.VertexList)
                    {
						tmp.Add((float)item.X);
						tmp.Add((float)item.Y);
						tmp.Add((float)item.Z);
                    }
					return tmp.ToArray();
				} 
			}
            public FbxGLModel(string ModelName)
            {
				model = new Obj();
				model.LoadObj(ModelName);
            }
        }
    }
}
