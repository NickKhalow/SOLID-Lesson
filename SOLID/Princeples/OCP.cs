namespace SOLID.Princeples;

public class OCP
{
    public partial class BaseClass
    {
        public virtual void DoMethod()
        {
            
        }
    }
    
    public class OpenForExtension : BaseClass
    {
        public override void DoMethod()
        {
        }
    }
    
    public partial class BaseClass
    {
        //BUT CLOSED FOR MODIFICATIONS
    }
}