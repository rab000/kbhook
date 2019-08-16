using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using WindowsInput;
using WindowsInput.Native;

public class TDemo : MonoBehaviour
{
    // Start is called before the first frame update

    CSHook hook;

    InputSimulator IIS;


    void Start()
    {
        IIS = new InputSimulator();

        hook = new CSHook();

        Debug.Log("start hook!!!");

        hook.InstallHook(TT);
    }

    public void TT(CSHook.信息结构体 param, out bool handle)
    {
        Debug.Log("------------------->" + param.vkCode);

        //true就是拦截，false不拦截

        if (param.vkCode == 113)//f2
        {
            hook.UninstallHook();

            Debug.Log("stop hook");

            handle = true;
        }
        else if (param.vkCode == 112)//f1
        {
            

            handle = true;
        }
        else if (param.vkCode == 114)//f3
        {
            //SendKeys.SendWait("{ABCDE}");
            //SendKeys.Flush();
            
            //sim.Keyboard.KeyPress(VirtualKeyCode.VK_X);
            IIS.Keyboard.TextEntry("go go go!!!");
            
            handle = true;
        }
        else if (param.vkCode == 115)//f4
        {
            Debug.Log("f4 click");
            IIS.Mouse.MoveMouseBy(20,20);

            handle = true;
        }
        else if (param.vkCode == 116)//f5
        {
            Debug.Log("f5 click");
            IIS.Mouse.MoveMouseTo(3000,100);

            handle = true;
        }
        else if (param.vkCode == 117)//f6
        {
            Debug.Log("f6 click");
            IIS.Mouse.MoveMouseToPositionOnVirtualDesktop(500, 100);

            handle = true;
        }
        else
        {
            handle = false;
        }
    }

    void Update()
    {

    }


}
