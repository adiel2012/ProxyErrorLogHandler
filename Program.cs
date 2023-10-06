using core;
using System.Xml.Linq;

public interface Interface1
{
    public int Method1();
    public int Method2();

    public void Method3();
    public void Method4();
}
class A : Interface1{

    public int Method1()
    {
        return 1;
    }

    public int Method2()
    {
        throw new Exception("My exception");
    }

    public void Method3(){}
    public void Method4(){}
}
class M<T>
{

}

namespace CSharpTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mm = new TResult<int>(2);
            //int gg = mm;

            A a = new A();

            //Type theType = Type.GetType("core.TResult`1");
            var fffff = typeof(TResult<>);

            

            object v1 = 9;


            Type typeArgument = v1.GetType();
            Type template = typeof(core.TResult<>);
            Type t4 = Type.GetType(fffff.FullName);
            Type genericType = template.MakeGenericType(typeArgument);
            var instance = Activator.CreateInstance(genericType, 2);

            int tyrttry = (core.TResult<int>)instance;

            //int hhh = fffff.

            int mm = (int)instance;


            var proxy = core.ProxyErrorHandler<Interface1>.CreateProxy(a);
            Interface1 p = proxy as Interface1;
            var f = p.Method1();
            int h = f;
            Console.WriteLine(f);
            Console.WriteLine(p.Method2());

            p.Method3();
            p.Method4();

            proxy.ExceptionLogger = (System.Exception ex) =>{
                Console.WriteLine(ex.Message);
                return true; // rethrow the exception
            };

            try{
                Console.WriteLine(p.Method2());
            }catch(Exception ex)
            {
                Console.WriteLine("Exception rethrown");
            }

            /*
            Called Method1
            1
            Called Method2
            My exception
            0
            Called Method3
            Called Method4
            Called Method2
            My exception
            Exception rethrown

            */

        }
    }
}