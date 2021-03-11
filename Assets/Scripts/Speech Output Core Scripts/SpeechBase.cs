
public abstract class SpeechBase
{
    public enum LANGUAGE //[mac / windows] note that these language has to be installed in your system.
    {
        DUTCH,
        ENGLISH,
        GERMAN,
        JAPANESE
    };
    public static LANGUAGE Language = LANGUAGE.ENGLISH;
    public static bool isSpeaking = false;
    public static float speed = 1.0f;
    public abstract void Init(int outputchannel);
    public abstract void Speak(string text);
    public abstract void Stop();
    public void SetLanguage(LANGUAGE lang) { Language = lang; }
    public void SetSpeed(float s) { speed = s; }
}
