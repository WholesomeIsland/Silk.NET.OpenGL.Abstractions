using System;
using System.Runtime.InteropServices;
using Assimp;
namespace Abstractions.Model
{
    public unsafe class Model
    {
        private AssimpContext actx;
        public Scene scene;
        public Vector3D[] getVertsFromModel(int index){
            return scene.Meshes[index].Vertices.ToArray();
        }
        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="pathToModel">string to the 3d model file</param>
        /// <param name="pps">post process steps. a bitwise or that specifies extra setps for loading models.</param>
        public Model(string pathToModel, Assimp.PostProcessSteps pps)
        {
            actx = new AssimpContext();
            scene = actx.ImportFile(pathToModel, pps);
        }
    }
}
