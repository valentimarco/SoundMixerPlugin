using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OVRSharp;
using SoundMixer;
using SoundMixerplugin.Frontend.ImGuiController;
using SoundMixerplugin.Frontend;
using SoundMixerplugin.AudioManager;
using Valve.VR;

namespace SoundMixerplugin.Overlay {
    public class OverlayVr : Application {
        private readonly OVRSharp.Overlay _overlay;
        private readonly string _keyoverlay = "soundmixer";
        private readonly int _texture;
        private ImGuiController _imGuiController = null!;
        private Mixer _mixer;
        private const int Width = 1200, Height = 800;
        private System.Numerics.Vector2 _scaleFactor = System.Numerics.Vector2.One;

        private readonly VRTextureBounds_t _boundsT = new() {
            uMax = 1f,
            uMin = 0f,
            vMax = 1f,
            vMin = 0f
        };

        private Texture_t _overlayTextureT = new() {
            eColorSpace = EColorSpace.Auto,
            eType = ETextureType.OpenGL,
            handle = IntPtr.Zero
        };

        private readonly GameWindow _window = new(
            GameWindowSettings.Default,
            new NativeWindowSettings {
                Size = new Vector2i(Width, Height),
                APIVersion = new Version(4, 5),
                Flags = ContextFlags.Offscreen,
            });


        public OverlayVr() : base(ApplicationType.Overlay) {
            _overlay = new OVRSharp.Overlay(_keyoverlay, "SoundMixer", true) {
                WidthInMeters = 2.5f,
                MouseScale = new HmdVector2_t {
                    v0 = 1200f,
                    v1 = 800f
                },
                TextureBounds = _boundsT
            };
            //_overlay.SetThumbnailTextureFromFile(Utilities.PathToResource("Selen_SAd.jpg"));
            OpenVR.Overlay.SetOverlayInputMethod(_overlay.Handle, VROverlayInputMethod.Mouse);
            OpenVR.Overlay.SetOverlayColor(_overlay.Handle, 1, 1, 1);

            // Setup mouse events
            _overlay.OnMouseUp += (sender, t) => {
                ImGuiIOPtr io = ImGui.GetIO();
                if (t.data.mouse.button == (uint) EVRMouseButton.Left)
                    io.AddMouseButtonEvent(0, false);
            };

            _overlay.OnMouseDown += (sender, t) => {
                ImGuiIOPtr io = ImGui.GetIO();
                if (t.data.mouse.button == (uint) EVRMouseButton.Left)
                    io.AddMouseButtonEvent(0, true);
            };

            _overlay.OnMouseMove += (sender, t) => {
                //don't know why works, but works
                ImGuiIOPtr io = ImGui.GetIO();
                io.AddMousePosEvent(t.data.mouse.x, t.data.mouse.y);
            };

            _overlay.StartPolling();

            //create texture for steamvr
            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1600, 900, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            //Create and start the Window
            CreateWindow();

            // OpenVR.Overlay.ShowDashboard(_keyoverlay);
        }

        ~OverlayVr() {
            _overlay.StopPolling();
            _window.Close();
            _overlay.Destroy();
            base.Shutdown();
        }

        private void CreateWindow() {
            //set not visible
            _window.IsVisible = true;


            //set OnLoad action to initialize Imgui
            _window.Load += () => {
                _imGuiController = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
                _mixer = new Mixer();
                // Task.Run(async () => {
                //     while (true) {
                //         //something
                //         
                //         await Task.Delay(2000);
                //     }
                // });
            };

            //set OnRenderFrame to show the gui created and copy the current window to a texture that goes to OpenVr
            _window.RenderFrame += e => {
                _imGuiController.Update(_window, (float) e.Time);

                //Clear stuff
                GL.ClearColor(new Color4(0, 32, 48, 255));
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                         ClearBufferMask.StencilBufferBit);

                //imGui Rendering

                MixerGui.Gui(_mixer);


                _imGuiController.Render();

                Utilities.CheckGLError("End of frame");

                _window.SwapBuffers();

                //Copy window in a texture
                GL.BindTexture(TextureTarget.Texture2D, _texture);
                GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 0, 0, Width, Height, 0);

                //Set the texture
                _overlayTextureT.handle = new IntPtr(_texture);
                _overlay.SetTexture(_overlayTextureT);
            };

            _window.Run();
        }
    }
}