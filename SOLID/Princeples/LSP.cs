using System.Numerics;

namespace SOLID.Princeples;

public class LSP
{
    public abstract class FloatingThing
    {
        public abstract void GoTo(Vector3 destination);
    }
    
    public class RubberBoat : FloatingThing
    {
        public override void GoTo(Vector3 destination)
        {
            //I'm swimming!
        }
    }
    
    public class Cruiser : FloatingThing
    {
        public override void GoTo(Vector3 destination)
        {
            //кря ;)
            //WRONG 
            //НЕПРАВИЛЬНО
            
        }
    }
}