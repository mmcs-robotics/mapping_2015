namespace SLAM
{
    public static class Logger
    {
        public static void Write(string text)
        {
            if (AppGlobals.Form != null)
                AppGlobals.Form.AppendLog(text);
        }

        public static void Success(string text)
        {
            Write(text);
        }

        public static void Warn(string text)
        {
            Write(text);
        }
    }
}
