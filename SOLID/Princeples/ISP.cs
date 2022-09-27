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

    public class Cat : IRunnable, IJumpable
    {
        void IRunnable.Run()
        {
        }

        void IJumpable.Jump()
        {
        }
    }

    public class Dog : IRunnable, IJumpable, ISwimmable
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
    }

    public void Example()
    {
        var barsik = new Cat();
        var sharik = new Dog();
        
        //для интерфейсов также работает апкаст

        var runners = new IRunnable[] {sharik, barsik};
        var jumpers = new IJumpable[] {sharik, barsik};
        var swimers = new ISwimmable[]
        {
            sharik,
            //barsik //барсик не умеет плавать, ошибка
        };
    }
}