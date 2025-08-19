using Sarc.Model.Entity;
using Sarc.Repository.Interface;

namespace Sarc.Repository
{
    public class HelloRepository : IHelloRepository
    {
        private readonly Hello _hello;

        public HelloRepository()
        {
            _hello = new Hello()
            {
                Greeting = "HelloWorld!"
            };
        }

        public string Hello() => _hello.Greeting;
    }
}