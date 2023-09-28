
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core;
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

namespace CSharpTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            A a = new A();
            var proxy = ProxyErrorHandler<Interface1>.CreateProxy(a);
            Interface1 p = proxy as Interface1;
            Console.WriteLine(p.Method1());
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

        }
    }
}