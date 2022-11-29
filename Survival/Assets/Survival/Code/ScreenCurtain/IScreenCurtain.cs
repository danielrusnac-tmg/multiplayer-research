using System.Collections;

namespace TMG.Survival.ScreenCurtain
{
    public interface IScreenCurtain
    {
        bool IsCurtainShown { get; }
        
        IEnumerator ShowCurtain();
        void ShowCurtainImmediate();
        IEnumerator HideCurtain();
        void HideCurtainImmediate();
    }
}