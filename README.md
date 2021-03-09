# CommonOpenGLAbstractions
c# abstractions for OpenGL using Silk.NET
# Why Does This Exist?
Currently it takes about 100 or more lines of code just to draw a simple shape onscreen. 
the goal of this is to lower that to under 50,
Therefore signifigantly lowering the barrier of entry to graphics programming.
# Usage
this Library has 7 classes, Texture, VAO, VBO, EBO, GLObject, GLObjTextured, and Model. 
Model is the largest, it uses assimp.net to load a model into OpenGL. 
it has a draw method, it takes 3 System.Numerics.Matrix4x4, and a string[] of uniform names to put the matrices into.
it is setup so that is the only class you need to draw a model. 
the vao and texture class is a copy-paste from the silk.net examples, the VBO and EBO inherit from the BufferObject class.
the GLObject class has you supply vertices and indices and then you can draw it with the draw method. 
the GLObjTextured is a GLObject and the VAO has a specific layout. location 0 is the verices and is 3 floats. location 1 is the texCoords and is 2 floats. 
so, you need an array where every 5 floats there is a new verices and right after the location needs to be the texxcoords and nothing else.
# Performance
as with any sort of Abstractions / Wrapper there is going to be slowdowns and other drawbacks.
On my system the example 'Hello Quad' took 95.6 MB of RAM.
the same example in Silk.NET took 92.5 MB of RAM.
The Helo Quad Abstractions example shown here takes 174 MB of RAM.

### Credits
the model from the model loading example is from [sketchfab](https://sketchfab.com/3d-models/hk-mp5-9mm-submachine-gun-c503d96157614fb78b5953d49e643b78)
