using System.Numerics;

namespace SOLID.Princeples;

public class ISP
{
    public interface IRunnable
    {
        void Run();
    }

    public interface IJumpable
    {
        void Jump();
    }

    public interface ISwimmable
    {
        void Swim();
    }

    public abstract class Animal
    {
        public abstract void Breath();
    }

    public class Cat : Animal, IRunnable, IJumpable
    {
        void IRunnable.Run()
        {
            Vector3 someSpeed = new Vector3();
        }

        void IJumpable.Jump()
        {
        }

        public override void Breath()
        {
        }
    }
    
    public class Kenguru : Animal, IJumpable
    {
        void IJumpable.Jump()
        {
        }
        
        public override void Breath()
        {
        }
    }

    public class Dog : Animal, IRunnable, IJumpable, ISwimmable
    {
        public void Run()
        {
        }

        public void Jump()
        {
        }

        public void Swim()
        {
        }

        public override void Breath()
        {
        }
    }

    public void Example()
    {
        var barsik = new Cat();
        var sharik = new Dog();
        var kengu = new Kenguru();
        
        //для интерфейсов также работает апкаст

        var runners = new IRunnable[] {sharik, barsik};
        var jumpers = new IJumpable[] {sharik, barsik, kengu};
        var swimers = new ISwimmable[]
        {
            sharik,
            //barsik //барсик не умеет плавать, ошибка
        };

        var animals = new Animal[] {sharik, barsik, kengu};
        
        foreach (var runner in runners)
        {
            runner.Run();
        }
        
        foreach (var jumper in jumpers)
        {
            jumper.Jump();
        }
        
        foreach (var swimer in swimers)
        {
            swimer.Swim();
        }
        
        foreach (var animal in animals)
        {
            animal.Breath();

            if (animal is IJumpable jumpable)
            {
                jumpable.Jump();
            }

            if (animal is ISwimmable swimmable)
            {
                swimmable.Swim();
            }
        }
    }
}