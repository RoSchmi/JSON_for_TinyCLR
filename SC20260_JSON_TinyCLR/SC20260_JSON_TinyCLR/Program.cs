
using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using PervasiveDigital.Json;


namespace SC20260_JSON_TinyCLR
{
    public partial class Program
    {
        private static GpioPin AppButton;

        public class ChildClass
        {
            public int one;
            public int two;
            public int three;
        }

        public class TestClass
        {
            //RoSchmi
            //public long l;
            public int i;
            public string aString;
            public string someName;
            public DateTime Timestamp;
            public int[] intArray;
            public string[] stringArray;
            public ChildClass child1;
            public ChildClass Child { get; set; }
        }


        // This method is run when the mainboard is powered up or reset.   
        //void ProgramStarted()
        static void Main()
        {
            Debug.WriteLine("Program Started, Press App Button to continue");

            // When the Button 'App' on the board is pressed
            AppButton = GpioController.GetDefault().OpenPin(SC20260.GpioPin.PB7);
            AppButton.SetDriveMode(GpioPinDriveMode.InputPullUp);
            
            AppButton.ValueChanged += AppButton_ValueChanged;

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        private static void AppButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (AppButton.Read() == GpioPinValue.Low)
            {
                var test = new TestClass()
                {
                    aString = "A string",
                    //RoSchmi
                    //l = 100,
                    i = 10,
                    someName = "who?",
                    Timestamp = DateTime.UtcNow,
                    intArray = new[] { 1, 3, 5, 7, 9 },
                    stringArray = new[] { "two", "four", "six", "eight" },
                    child1 = new ChildClass() { one = 1, two = 2, three = 3 },
                    Child = new ChildClass() { one = 100, two = 200, three = 300 }
                };

                var result = JsonConverter.Serialize(test);
                Debug.WriteLine("Serialization:");
                var stringValue = result.ToString();
                Debug.WriteLine(stringValue);

                var dserResult = JsonConverter.Deserialize(stringValue);
                Debug.WriteLine("After deserialization:");
                Debug.WriteLine(dserResult.ToString());

                var newInstance = (TestClass)JsonConverter.DeserializeObject(stringValue, typeof(TestClass), CreateInstance);
                if (
                    //RoSchmi
                    //test.l != newInstance.i ||
                    test.i != newInstance.i ||
                    test.Timestamp.ToString() != newInstance.Timestamp.ToString() ||
                    test.aString != newInstance.aString ||
                    test.someName != newInstance.someName ||
                    !ArraysAreEqual(test.intArray, newInstance.intArray) ||
                    !ArraysAreEqual(test.stringArray, newInstance.stringArray)
                    )
                    throw new Exception("round-tripping failed");

            }
        }

        private static object CreateInstance(string path, string name, int length)
        {
            if (name == "intArray")
                return new int[length];
            else if (name == "stringArray")
                return new string[length];
            else
                return null;
        }

        private static bool ArraysAreEqual(Array a1, Array a2)
        {
            if (a1 == null && a2 == null)
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; ++i)
            {
                if (!a1.GetValue(i).Equals(a2.GetValue(i)))
                    return false;
            }
            return true;
        }

    }
}


