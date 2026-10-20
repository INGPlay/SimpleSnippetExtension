using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SimpleSnippetExtension.Helper;

// 이전 창에 Ctrl+V 키 입력을 보내 붙여넣는다
internal static partial class PasteHelper
{
    private const byte VK_CONTROL = 0x11;
    private const byte VK_V = 0x56;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    // Command Palette가 숨겨지고 포커스가 이전 창으로 돌아갈 시간
    private const int HideDelayMilliseconds = 200;

    [LibraryImport("user32.dll")]
    private static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

    // 명령 결과(Hide)가 반영된 뒤에 붙여넣어야 하므로 기다렸다가 백그라운드에서 보낸다
    public static void PasteAfterHide()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(HideDelayMilliseconds);
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_V, 0, 0, 0);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        });
    }
}
