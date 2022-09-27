// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello there!");

var e = new ExampleClass();

e = null;

GC.Collect();

await Task.Delay(TimeSpan.FromSeconds(5));



class ExampleClass
{
    ~ExampleClass()
    {
        Console.WriteLine("I've been destructed");
    }
}