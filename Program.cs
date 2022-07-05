using System;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using DiscordRPC;
using Memory;
using System.Diagnostics;

namespace FLRP
{
    static class Program
    {
        public static bool rpLigado;
        public static Thread t;
        public static DiscordRpcClient client;
        public static int number = 0;
        public static ToolStripItem botao;
        public static Mem m = new Mem();
        public static bool flProc = false;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = GetContext();
            notifyIcon.Icon = Properties.Resources.flstudiorp;
            notifyIcon.Visible = true;

            rpLigado = true;
            t = new Thread(RpLoop);
            t.Start();
            Application.Run();
            
        }
        private static ContextMenuStrip GetContext()
        {
            ContextMenuStrip CMS = new ContextMenuStrip();
            botao = CMS.Items.Add("Turn rich presence off", null, new EventHandler((object sender, EventArgs e) => {
                if (rpLigado)
                {
                    rpLigado = false;
                    botao.Text = "Turn rich presence on";
                    t.Interrupt();
                }
                else
                {
                    rpLigado = true;
                    botao.Text = "Turn rich presence off";
                    t = new Thread(RpLoop);
                    t.Start();
                }

            }));
            return CMS;
        }

        private static void openFL()
        {
            flProc = m.OpenProcess("FL64");
            if (flProc)
            {
                m.OpenProcess("FL64");
            }
        }
        private static int map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
        public static void RpLoop()
        {
            try
            {
                DateTime timestamp = DateTime.UtcNow;
                client = new DiscordRpcClient("988310374002589716");
                //Connect to the RPC
                while (true)
                {
                    if (flProc & rpLigado)
                    {
                        if (!client.IsInitialized)
                        {
                            client.Initialize();
                            timestamp = DateTime.UtcNow;
                        };
                        int ms = m.ReadInt("_FLEngine_x64.dll+0xDDF5D8");
                        string bpm = m.ReadInt("_FLEngine_x64.dll+0xC9DF88").ToString();
                        //int master = m.ReadInt("_FLEngine_x64.dll+0xD77330");

                        if (bpm.Substring(bpm.Length - 3) == "000")
                        {
                            bpm = bpm.Substring(0, bpm.Length - 3);
                        }
                        else
                        {
                            bpm = bpm.Substring(0, bpm.Length - 3) + "." + bpm.Substring(bpm.Length - 3);
                        }
                        //if (master < 1000000000)
                        //    master = 1000000000;
                        //int master_wave = map(master, 1000000000, 1070000000, 0, 20);
                        //string wave = new string('█', master_wave);
                        //wave = wave.PadLeft(20);
                        //wave = wave.Substring(0, 15);
                        string initialTitle = Process.GetProcessById(m.mProc.Process.Id).MainWindowTitle;
                        string title;
                        string version;
                        if (initialTitle.Contains("-"))
                        {
                            title = initialTitle.Split("-")[0] + "- ";
                            version = initialTitle.Split("- ")[1];
                        }
                        else
                        {
                            title = "New Project - ";
                            version = initialTitle;
                        }
                        double ticks = double.Parse(ms.ToString());
                        TimeSpan time = TimeSpan.FromMilliseconds(ticks);
                        DateTime dt = new DateTime(time.Ticks);
                        client.SetPresence(new RichPresence()
                        {
                            Details = title+bpm+" BPM",
                            State = dt.Minute.ToString("00") + ":" + dt.Second.ToString("00"),
                            Assets = new Assets()
                            {
                                LargeImageKey = "fl",
                                LargeImageText = version
                            },
                            Timestamps = new Timestamps()
                            {
                               Start = timestamp
                            }
                        });
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (client.IsInitialized)
                        {
                            client.ClearPresence();
                            client.Deinitialize();
                        }
                    }
                    openFL();
                }
            }
            catch
            {
                if (client.IsInitialized)
                {
                    client.ClearPresence();
                    client.Deinitialize();
                }
            }
        }
    }
}
