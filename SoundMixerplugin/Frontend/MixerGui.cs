using System.Collections.ObjectModel;
using CoreAudio;
using ImGuiNET;
using SoundMixerplugin.AudioManager;

namespace SoundMixerplugin.Frontend;

public class MixerGui {
    public static void Gui(Mixer mixer) {
        //preparazione della finestra
        var io = ImGui.GetIO();
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
        ImGui.SetNextWindowSize(io.DisplaySize);

        //inzio della finestra principale
        ImGui.Begin("main",
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoSavedSettings |
            ImGuiWindowFlags.NoResize);
        //settings della main window
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(64, 69, 82, 0));


        if (ImGui.BeginTabBar("MyTabBar")) {
            string defualtinterfacename = mixer.InterfaceName;
            int sessionCount = mixer.SessionCount;

            //subtab
            if (ImGui.BeginTabItem(defualtinterfacename)) {
                //tables
                if (ImGui.BeginTable("Master Volume", sessionCount, ImGuiTableFlags.PadOuterX | ImGuiTableFlags.BordersInnerV, new System.Numerics.Vector2(1200, 600))) {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);

                    int masterVolume = (int) mixer.MasterVolume.MasterVolumeLevel;
                    if (ImGui.VSliderInt("Master Volume", new System.Numerics.Vector2(50, 150),
                            ref masterVolume,
                            0, 100)) {
                        mixer.MasterVolume.MasterVolumeLevelScalar = (float) masterVolume /100;
                    }

                    ImGui.SetCursorPosX(
                        ImGui.GetCursorPosX() + ImGui.GetColumnWidth() -
                        ImGui.CalcTextSize("Master Volume").X
                        - ImGui.GetScrollX()
                        - ImGui.GetStyle().ItemSpacing.X);

                    if (ImGui.Button("+", new System.Numerics.Vector2(50, 50))) {
                        mixer.MasterVolume.MasterVolumeLevel = masterVolume + 5f;
                    }

                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() -
                                        ImGui.CalcTextSize("Master Volume").X
                                        - ImGui.GetScrollX() - ImGui.GetStyle().ItemSpacing.X);

                    if (ImGui.Button("-", new System.Numerics.Vector2(50, 50))) {
                        mixer.MasterVolume.MasterVolumeLevel = masterVolume - 5f;
                    }

                    // ImGui.TableNextColumn();
                    //
                    // int i = 0;
                    // foreach (var audioSessionControl in mixer.ListSession) {
                    //     int pid = (int) audioSessionControl.ProcessID;
                    //     int sliderVolume = (int) audioSessionControl!.SimpleAudioVolume!.MasterVolume;
                    //     string appName = audioSessionControl.DisplayName;
                    //     if (ImGui.VSliderInt(appName, new System.Numerics.Vector2(50, 150),
                    //             ref sliderVolume, 0, 100))
                    //         audioSessionControl!.SimpleAudioVolume!.MasterVolume = sliderVolume;
                    //     
                    //     //define buttons, each with different ID
                    //     ImGui.PushID(i++);
                    //     {
                    //         ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() -
                    //                             ImGui.CalcTextSize("Master Volume").X
                    //                             - ImGui.GetScrollX() - ImGui.GetStyle().ItemSpacing.X);
                    //
                    //         if (ImGui.Button("+", new System.Numerics.Vector2(20, 20))) {
                    //             audioSessionControl!.SimpleAudioVolume!.MasterVolume = sliderVolume + 5f;
                    //         }
                    //
                    //         ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetColumnWidth() -
                    //                             ImGui.CalcTextSize("Master Volume").X
                    //                             - ImGui.GetScrollX() - ImGui.GetStyle().ItemSpacing.X);
                    //
                    //         if (ImGui.Button("-", new System.Numerics.Vector2(20, 20))) {
                    //             audioSessionControl!.SimpleAudioVolume!.MasterVolume = sliderVolume - 5f;
                    //         }
                    //
                    //         ImGui.PopID();
                    //     }
                    //
                    //
                    //     ImGui.SameLine();
                    //     ImGui.TableNextColumn();
                    //     
                    // }

                    ImGui.EndTable();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.End();
        }
    }
}