using UnityEngine;
using UnityEngine.UI;

namespace ThreadTask
{
    public class MyDebuggerTask : MonoBehaviour
    {
        // main scene
        public Button btn1;
        public Button btn2;
        public Button btn3;
        public Button btn4;
        public Button btnCancel;
        public Button btnStart;

        void Awake()
        {
            btn1.onClick.AddListener(() => MyLogTask.Log("1 click"));
            btn2.onClick.AddListener(() => MyLogTask.Log("2 click"));
            btn3.onClick.AddListener(() => MyLogTask.Log("3 click"));
            btn4.onClick.AddListener(() => MyLogTask.Log("4 click"));
            btnCancel.onClick.AddListener(MyLogTask.Dispose);
            //btnStart.onClick.AddListener(null);
        }

        void OnDestroy()
        {
            MyLogTask.Dispose();
        }
    }
}
