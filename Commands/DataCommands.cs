using System.Windows.Input;

namespace DPGI_vladik
{
    static class DataCommands
    {
        public static RoutedCommand Undo { get; set; }
        public static RoutedCommand Find { get; set; }
        public static RoutedCommand Delete { get; set; }

        static DataCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            KeyGesture key = new KeyGesture(Key.Z, ModifierKeys.Control);
            inputs.Add(key);
            Undo = new RoutedCommand("Undo", typeof(DataCommands), inputs);
            inputs = new InputGestureCollection();
            key = new KeyGesture(Key.F, ModifierKeys.Control);
            inputs.Add(key);
            Find = new RoutedCommand("Find", typeof(DataCommands), inputs);
            inputs = new InputGestureCollection();
            key = new KeyGesture(Key.Delete);
            inputs.Add(key);
            Delete = new RoutedCommand("Delete", typeof(DataCommands), inputs);
        }
    }
}
