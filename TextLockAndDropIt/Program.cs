using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TextLockAndDropIt
{
    public class MainForm : Form
    {
        private TextBox[] textBoxes;
        private Button lockButton;
        private bool isLocked = false;
        private static bool isPasteMode = false;
        private static MainForm _instance;
        private static bool awaitingPaste = false;
        private const string SaveFilePath = "textlock_data.txt";

        // Global mouse hook
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        // Key event simulation
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private const byte VK_CONTROL = 0x11;
        private const byte VK_V = 0x56;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public MainForm()
        {
            this.Text = "TextLockAndDropIt";
            this.Size = new System.Drawing.Size(350, 740);
            this.TopMost = true;
            _instance = this;

            int numberOfFields = 16;
            textBoxes = new TextBox[numberOfFields];

            for (int i = 0; i < numberOfFields; i++)
            {
                textBoxes[i] = new TextBox
                {
                    Left = 20,
                    Top = 20 + (i * 40),
                    Width = 300
                };
                textBoxes[i].Click += TextBox_Click;
                this.Controls.Add(textBoxes[i]);
            }

            lockButton = new Button
            {
                Text = "Lock",
                Left = 20,
                Top = 20 + (numberOfFields * 40),
                Width = 100
            };
            lockButton.Click += LockButton_Click;
            this.Controls.Add(lockButton);

            LoadData();
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            isLocked = !isLocked;
            lockButton.Text = isLocked ? "Unlock" : "Lock";

            foreach (var textBox in textBoxes)
            {
                textBox.ReadOnly = isLocked;
            }

            SaveData();
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            if (isLocked)
            {
                TextBox clickedBox = sender as TextBox;
                Clipboard.SetText(clickedBox.Text);
                isPasteMode = true;
                awaitingPaste = true;
                _hookID = SetHook(_proc);
            }
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static void SimulatePaste()
        {
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_V, 0, 0, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && isPasteMode && awaitingPaste && wParam.ToInt32() == WM_LBUTTONDOWN)
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                GetWindowThreadProcessId(foregroundWindow, out int foregroundProcessId);
                int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;

                if (foregroundProcessId != currentProcessId)
                {
                    awaitingPaste = false;
                    SimulatePaste();
                    UnhookWindowsHookEx(_hookID);
                    isPasteMode = false;
                    return IntPtr.Zero;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void SaveData()
        {
            using (StreamWriter writer = new StreamWriter(SaveFilePath))
            {
                foreach (var textBox in textBoxes)
                {
                    writer.WriteLine(textBox.Text);
                }
            }
        }

        private void LoadData()
        {
            if (File.Exists(SaveFilePath))
            {
                string[] lines = File.ReadAllLines(SaveFilePath);
                for (int i = 0; i < textBoxes.Length && i < lines.Length; i++)
                {
                    textBoxes[i].Text = lines[i];
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
