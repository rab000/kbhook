//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using util;
//using WindowsInput;
//using WindowsInput.Native;

//public class TDemo : MonoBehaviour
//{
//    // Start is called before the first frame update

//    CSHook hook;

//    InputSimulator IIS;

//    KeyboardSimulator ks;

//    void Start()
//    {
//        hook = new CSHook();

//        Debug.Log("start hook!!!");
//        hook.InstallHook(TT);

        
//    }

//    public void TT(CSHook.信息结构体 param, out bool handle)
//    {
//        Debug.Log("------------------->"+param.vkCode);

//        //true就是拦截，false不拦截

//        if (param.vkCode == 113)//f2
//        {
//            hook.UninstallHook();

//            Debug.Log("stop hook");

//            handle = true;
//        }
//        else if (param.vkCode == 114)//f3
//        {
//            //SendKeys.SendWait("{ABCDE}");
//            //SendKeys.Flush();
//            var sim = new InputSimulator();
//            //sim.Keyboard.KeyPress(VirtualKeyCode.VK_X);
//            sim.Keyboard.TextEntry("go go go!!!");
            
//            handle = true;
//        }
//        else if (param.vkCode == 115)//f4
//        {
//            handle = true;
//        }
//        else
//        {
//            handle = false;
//        }
//    }

//    void Update()
//    {
        
//    }


//}
