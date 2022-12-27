namespace PluginInterfaces
{
    public interface IStringManipulationsPlugin
    {
        string Concat(string str1, string str2);

        string Repeat(string str, int numberTimesToRepeat);
    }
}
