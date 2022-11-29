using System.Collections;

namespace TMG.Survival.ScreenCurtain
{
    public interface IScreenFader
    {
        bool IsCurtainShown { get; }
        
        IEnumerator ShowCurtain();
        void ShowCurtainImmediate();
        IEnumerator HideCurtain();
        void HideCurtainImmediate();
    }
}