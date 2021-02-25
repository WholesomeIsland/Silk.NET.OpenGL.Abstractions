using System;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Unmanaged;
namespace Abstractions.Model
{
    public unsafe class Model
    {
        AiScene mesh;
        public AiAnimation*[] animations 
        { 
            get {
                unsafe
                {
                    return Marshal.PtrToStructure<AiAnimation*[]>(mesh.Animations);
                }
            } 
        }
        public AiMesh*[] meshes 
        { 
            get {
                unsafe
                {
                    return Marshal.PtrToStructure<AiMesh*[]>(mesh.Meshes);
                }
            } 
        }
        public Assimp.Vector3D[] getVertsFromModel(int index)
        {
            return Marshal.PtrToStructure<Vector3D[]>(meshes[index]->Vertices);
        }
        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="pathToModel">string to the 3d model file</param>
        /// <param name="pps">post process steps. a bitwise or that specifies extra setps for loading models.</param>
        public Model(string pathToModel, Assimp.PostProcessSteps pps)
        {
            mesh = Marshal.PtrToStructure<AiScene>(AssimpLibrary.Instance.ImportFile(pathToModel, pps, (IntPtr)null));
        }
    }
}
