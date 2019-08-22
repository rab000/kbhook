using System.Text;

public class Default
{
    public const string EVENT_KB_CLICK = "kb_click";

    public const string EVENT_MOUSE = "mouse";

    public const string EVENT_SWITCH_PANEL = "switch_panel";

    //public const string SERVER_IP = "127.0.0.1";
    public const string SERVER_IP = "10.0.115.239";
    public const int SERVER_PORT = 54321;

    //中轮滚动速度
    public const int MOUSE_MID_SPEED = 1;
    //鼠标滚动速度
    public const int MOUSE_MOVE_SPEED = 4;

    //panel类型
    public const string PANEL_TYPE_NORMAL = "normalPanel";
    public const string PANEL_TYPE_NUM = "numPanel";
    public const string PANEL_TYPE_CODE = "codePanel";
    public const string PANEL_TYPE_FULL = "fullPanel";

    public static StringBuilder SB = new StringBuilder();
}

public enum KBtnTypeEnum
{
    char0,
    doublefunc,
    pressAble
}


