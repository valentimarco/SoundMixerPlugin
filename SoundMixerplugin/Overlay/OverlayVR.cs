using System.Diagnostics.Tracing;
using System.Numerics;
using System.Runtime.Loader;
using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OVRSharp;
using OVRSharp.Math;
using Valve.VR;

namespace SoundMixer {
    public class OverlayVR : Application { 
        private Overlay _overlay;
        private string _keyoverlay = "soundmixer";
        private int texture;
        private ImGuiController _controller;
        private static int Width = 1920, Height = 1080;

        private VRTextureBounds_t _boundsT = new VRTextureBounds_t() {
            uMax = 1f,
            uMin = 0f,
            vMax = 1f,
            vMin = 0f
        };
        
        private GameWindow window = new GameWindow(GameWindowSettings.Default, 
            new NativeWindowSettings() {
                Size = new Vector2i(Width, Height), 
                APIVersion = new Version(4, 5),
                Flags = ContextFlags.Offscreen
            });
        
        public OverlayVR() : base(ApplicationType.Overlay) {
            _overlay = new Overlay(_keyoverlay, "SoundMixer", true);
            _overlay.WidthInMeters = 2.8f;
            _overlay.InputMethod = VROverlayInputMethod.Mouse;
            _overlay.TextureBounds = _boundsT;
            
            _overlay.OnMouseMove += (sender, t) => {
                Console.WriteLine($"Mouse move: {t.data.mouse.x},{t.data.mouse.y}");
            };
            
            _overlay.StartPolling();
            OpenVR.Overlay.SetOverlayColor(_overlay.Handle, 1, 1, 1);
            //create texture for steamvr
            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D,texture);
            GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgb,1600,900,0,PixelFormat.Rgba,PixelType.UnsignedByte,IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,(int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            //Create and start the Window
            CreateWindow();
            
            OpenVR.Overlay.ShowDashboard(_keyoverlay);
            
        }
        


        private void CreateWindow() {
            //set not visible
            window.IsVisible = false;
            
            //set OnLoad action to initialize Imgui
            window.Load += () => {
                _controller = new ImGuiController(window.ClientSize.X, window.ClientSize.Y);
            };
            //set OnRenderFrame to show the gui created and copy the current window to a texture that goes to OpenVr
            window.RenderFrame += e => {
                _controller.Update(window, (float)e.Time);

                GL.ClearColor(new Color4(0, 32, 48, 255));
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                ImGui.ShowDemoWindow();

                _controller.Render();

                Util.CheckGLError("End of frame");

                window.SwapBuffers();
                
                GL.BindTexture(TextureTarget.Texture2D,texture);
                GL.CopyTexImage2D(TextureTarget.Texture2D,0,InternalFormat.Rgba,0,0,Width,Height,0);
                _overlay.SetTexture(new Texture_t() {
                    eColorSpace = EColorSpace.Auto,
                    eType = ETextureType.OpenGL,
                    handle = new IntPtr(texture)
                });
            };
            
            window.Run();
        }


        ~OverlayVR() {
            window.Close();
            _overlay.Destroy();
        }

        


    }
}