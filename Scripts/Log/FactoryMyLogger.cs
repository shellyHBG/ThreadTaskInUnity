public class FactoryMyLogger
{
    public static IMyLogger Create()
    {
#if UNITY_UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER
        return new MyLoggerUnity();
        // Any other logger
        //return new MyLoggerConsole();
#endif
        return null;
    }
}
