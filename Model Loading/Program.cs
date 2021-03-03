using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Abstractions.Model.GL;
using System.Drawing;
using Matrix4x4 = Abstractions.math.Matrix4x4;
using System.Numerics;
using System;
using Silk.NET.Input;
using System.Linq;

namespace Model_Loading
{
    public static class MathHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }
    }
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }

        public Vector3 Up { get; private set; }
        public float AspectRatio { get; set; }

        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; }

        private float Zoom = 45f;

        public Camera(Vector3 position, Vector3 front, Vector3 up, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            Front = front;
            Up = up;
        }

        public void ModifyZoom(float zoomAmount)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
        }

        public void ModifyDirection(float xOffset, float yOffset)
        {
            Yaw += xOffset;
            Pitch -= yOffset;

            //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
            Pitch = Math.Clamp(Pitch, -89f, 89f);

            var cameraDirection = Vector3.Zero;
            cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            Front = Vector3.Normalize(cameraDirection);
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
        }
    }
    class Program
    {
        static GLModel model;
        private static IKeyboard primaryKeyboard;
        static Camera cam; 
        private static Vector2 LastMousePosition;
        static void Main(string[] args)
        {
            var Options = WindowOptions.Default;
            Options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            Options.Title = "LearnOpenGL with Silk.NET";
            var window = Window.Create(Options);
            window.Render += (obj) => {
                GL.GetApi(window).Enable(EnableCap.DepthTest);
                GL.GetApi(window).ClearColor(Color.Black);
                GL.GetApi(window).Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
                model.Draw(cam.GetProjectionMatrix(), cam.GetViewMatrix(), Matrix4x4.Identity, new string[] {"proj", "view", "model"});
            };
            window.Load += () => {
                IInputContext input = window.CreateInput();
                primaryKeyboard = input.Keyboards.FirstOrDefault();
                for (int i = 0; i < input.Mice.Count; i++)
                {
                    input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                    input.Mice[i].MouseMove += (mouse, position) =>
                    {
                        const float lookSensitivity = 0.1f;
                        if (LastMousePosition == default) { LastMousePosition = position; }
                        else
                        {
                            var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                            var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                            LastMousePosition = position;

                            cam.ModifyDirection(xOffset, yOffset);
                        }
                    };
                }
                model = new GLModel(GL.GetApi(window), "gun.obj", "Shader.vert", "Shader.frag", false, new[] {Assimp.TextureType.Diffuse});
                cam = new Camera(new Vector3(0, 0, -10), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 800 / 600);
            };
            window.Closing += () => {
                model.Dispose();
            };
            window.Update += (deltaTime) =>
            {
                var moveSpeed = 2.5f * (float)deltaTime;

                if (primaryKeyboard.IsKeyPressed(Key.W))
                {
                    //Move forwards
                    cam.Position += moveSpeed * cam.Front;
                }
                if (primaryKeyboard.IsKeyPressed(Key.S))
                {
                    //Move backwards
                    cam.Position -= moveSpeed * cam.Front;
                }
                if (primaryKeyboard.IsKeyPressed(Key.A))
                {
                    //Move left
                    cam.Position -= Vector3.Normalize(Vector3.Cross(cam.Front, cam.Up)) * moveSpeed;
                }
                if (primaryKeyboard.IsKeyPressed(Key.D))
                {
                    //Move right
                    cam.Position += Vector3.Normalize(Vector3.Cross(cam.Front, cam.Up)) * moveSpeed;
                }
            };
            window.Run();
        }
    }
}
