using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;//调用操作系统动态链接库
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
//using System.Windows.Forms;

namespace util
{
    public class CSHook
    {
        //使用方法
        //private键盘钩子类hook = new 键盘钩子类();
        //hook.InstallHook(this.拦截函数);
        //public void 拦截函数(键盘钩子类.鼠标信息结构体 hookStruct,out bool handle){};
        //if(hook!=null) hook.UninstallHook();

        //定义常量
        public const int WH_KEYBOARD_LL = 13; //全局钩子键盘为13，线程钩子键盘为2
        public const int WM_KEYDOWN = 0X0100; //键按下0x0101为键弹起
        public static int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0X0104;
        //键盘处理事件委托 ,当捕获键盘输入时调用定义该委托的方法.（定义函数指针）
        public delegate int HookHandle(int nCode, int wParam, IntPtr lParam);

        //客户端键盘处理事件
        public delegate void 调用端函数(信息结构体 param, out bool handle);

        //接收SetWindowsHookEx返回值
        private static int 是否以安装 = 0;

        //声明一个指向执行函数的函数指针
        private HookHandle 执行函数引用;

        //定义存储按键信息的结构体
        [StructLayout(LayoutKind.Sequential)]
        public class 信息结构体
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //设置钩子 
        //idHook为13代表键盘钩子为14代表鼠标钩子，lpfn为函数指针，指向需要执行的函数，hInstance为指向进程块的指针，threadId默认为0就可以了
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(int idHook, HookHandle lpfn, IntPtr hInstance, int threadId);

        //取消钩子 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        //调用下一个钩子 
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        //获取当前线程ID（获取进程块）
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        //Gets the main module for the associated process.
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        private IntPtr 进程块 = IntPtr.Zero;

        //构造器
        public CSHook() { }

        //外部调用的键盘处理事件
        private static 调用端函数 调用函数引用 = null;

        /// <summary>
        /// 安装勾子
        /// </summary>
        /// <param name="hookProcess">外部调用的键盘处理事件</param>
        public void InstallHook(调用端函数 clientMethod)
        {

            调用函数引用 = clientMethod;

            // 安装键盘钩子 
            if (是否以安装 == 0)
            {
                执行函数引用 = new HookHandle(执行函数);

                进程块 = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

                是否以安装 = SetWindowsHookEx(WH_KEYBOARD_LL, 执行函数引用, 进程块, 0);

                //如果设置钩子失败. 
                if (是否以安装 == 0) UninstallHook();
            }
        }

        //取消钩子事件 
        public void UninstallHook()
        {
            if (是否以安装 != 0)
            {
                bool ret = UnhookWindowsHookEx(是否以安装);
                if (ret) 是否以安装 = 0;
            }
        }

        //钩子事件内部调用,调用_clientMethod方法转发到客户端应用。
        private static int 执行函数(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (int)WM_SYSKEYDOWN) || (wParam == (int)WM_KEYDOWN))
            {
                //转换结构
                信息结构体 hookStruct = (信息结构体)Marshal.PtrToStructure(lParam, typeof(信息结构体));

                if (调用函数引用 != null)
                {
                    bool handle = false;   //默认不拦截
                    //调用客户提供的事件处理程序。
                    调用函数引用(hookStruct, out handle);
                    if (handle) return 1; //1:表示拦截键盘,return 退出
                }
            }
            return CallNextHookEx(是否以安装, nCode, wParam, lParam);
        }

    }
}
