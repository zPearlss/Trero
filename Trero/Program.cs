﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trero.ClientBase;
using Trero.ClientBase.EntityBase;
using Trero.ClientBase.KeyBase;
using Trero.ClientBase.UIBase;
using Trero.ClientBase.VersionBase;
using Trero.Modules;
using Module = Trero.Modules.Module;

namespace Trero
{
    class Program
    {
        public static bool quit = false;

        public static bool scaffolding = false;

        public static List<Module> modules = new List<Module>();

        static void Main(string[] args)
        {
            MCM.openGame();
            MCM.openWindowHost();

            new Keymap();

            new Thread(() => { Application.Run(new Overlay()); }).Start(); // UI Application

            modules.Add(new AirJump());
            modules.Add(new TPAura());
            modules.Add(new ClosestPlayerDisplay());
            modules.Add(new PlayerDisplay());
            modules.Add(new TriggerBot());
            modules.Add(new Fly());

            modules.Sort((c1, c2) => c1.name.CompareTo(c2.name));

            VersionClass.setVersion(VersionClass.versions[0]);

            Keymap.keyEvent += keyParse;

            Console.WriteLine("--- Trero Terminal ---");
            Console.WriteLine("Welcome to the trero terminal");
            Console.WriteLine("");
            Console.WriteLine("--- Trero Keybinds ---");
            Console.WriteLine("R - ClampJet");
            Console.WriteLine("P - Terminate Process");
            Console.WriteLine("Y - Hitboxes");
            Console.WriteLine("C - PhaseUp(ServerBypass)");
            Console.WriteLine("V - PhaseDown(ServerBypass)");

            // Console.WriteLine(Game.level.ToString("X"));

            while (true) // freeze
            {
                Thread.Sleep(1);
                foreach (Module mod in modules)
                    mod.onLoop();
            }
        }

        private static void keyParse(object sender, KeyEvent e)
        {
            ; // Keymap Handler

            /*if (e.vkey == vKeyCodes.KeyHeld && e.key == Keys.LControlKey)
            {
                Mouse.MouseEvent(Mouse.MouseEventFlags.MOUSEEVENTF_LEFTDOWN);
            }*/

            if (e.vkey == vKeyCodes.KeyUp && e.key == Keys.R)
            {
                Game.velocity = Base.Vec3();
            }

            /*if (e.vkey == vKeyCodes.KeyUp && e.key == Keys.G)
            {
                // 0x892A45 => "89 41"
                MCM.writeBaseBytes(0x892A45, MCM.ceByte2Bytes("89 41 18")); // Restore with original assembly code
                MCM.writeBaseBytes(0x898385, MCM.ceByte2Bytes("C7 40 18 03 00 00 00"));
            }
            if (e.vkey == vKeyCodes.KeyDown && e.key == Keys.G)
            {
                Game.isLookingAtBlock = 0;

                MCM.writeBaseBytes(0x892A45, MCM.ceByte2Bytes("90 90 90")); // Nop assembly code
                MCM.writeBaseBytes(0x898385, MCM.ceByte2Bytes("90 90 90 90 90 90 90"));
            }*/

            // Fixed noclip on run

            if (e.vkey == vKeyCodes.KeyHeld) // broken
            {
                if (e.key == Keys.P)
                {
                    Process.GetProcessesByName("ApplicationFrameHost")[0].Kill();
                    Process.GetProcessesByName("Minecraft.Windows")[0].Kill();
                }///Terminate Process
                else if (e.key == Keys.C)
                {
                    Game.velocity = Base.Vec3();

                    Vector3 newPos = Game.position;
                    newPos.y += 0.01f;
                    Game.position = newPos;
                }///Phase Up    
                else if (e.key == Keys.V)
                {
                    Game.velocity = Base.Vec3();
                    Vector3 newPos = Game.position;
                    newPos.y += -0.01f;
                    Game.position = newPos;
                }///No Clip 
                else if (e.key == Keys.G)
                {
                    Game.isLookingAtBlock = 0;
                    Game.SideSelect = 1;
                    Game.SelectedBlock = Base.iVec3((int)Game.position.x, (int)Game.position.y - 1, (int)Game.position.z);

                    Mouse.MouseEvent(Mouse.MouseEventFlags.MOUSEEVENTF_RIGHTDOWN);
                }///Scaffold (Broken)
                else if (e.key == Keys.Y)
                {
                    foreach (var entity in Game.getPlayers())
                    {
                        entity.hitbox = Base.Vec2(0.6f, 1.8f);
                    }
                    foreach (var entity in Game.getPlayers())
                    {
                        entity.hitbox = Base.Vec2(7, 7);
                    }
                }///Hitboxes 
                else if (e.key == Keys.X)
                {
                    Game.onGround = true;

                    Game.velocity = Base.Vec3();

                    Vector3 newVel = Base.Vec3();

                    float cy = (Game.rotation.y + 89.9f) * ((float)Math.PI / 180F);

                    if (Keymap.GetAsyncKeyState((char)(Keys.W)))
                        newVel.z = (float)Math.Sin(cy) * (8 / 9f); ///Working Fly With No Height 


                    if (Keymap.GetAsyncKeyState((char)(Keys.W)))
                        newVel.x = (float)Math.Cos(cy) * (8 / 9f);
                    Game.velocity = newVel;
                }///No Y Fly
                else if (e.key == Keys.T)
                {

                    
                    Game.onGround = true;

                    Game.velocity = Base.Vec3();

                    Vector3 newVel = Base.Vec3();

                    float cy = (Game.rotation.y + 89.9f) * ((float)Math.PI / 180F);


                    if (Keymap.GetAsyncKeyState((char)(Keys.W)))
                        newVel.z = (float)Math.Sin(cy) * (12 / 16f);
                    if (Keymap.GetAsyncKeyState((char)(Keys.Space)))
                        newVel.y += 0.89f;
                    if (Keymap.GetAsyncKeyState((char)(Keys.LShiftKey))) ///Working Fly 
                        newVel.y -= 0.89f;

                    if (Keymap.GetAsyncKeyState((char)(Keys.W)))
                        newVel.x = (float)Math.Cos(cy) * (12 / 16f);
                    Game.velocity = newVel;
                    Game.velocity = newVel;
                }///Fly
            }
        }
    }
}
