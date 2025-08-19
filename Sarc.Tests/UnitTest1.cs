using Sarc.Controllers;
using Sarc.Repository;
using Sarc.Service;
using Sarc.Service.Interface;

namespace Sarc.Tests;

public class Tests
{
    private HelloService _service;
    [SetUp]
    public void Setup()
    {
        var repo = new HelloRepository();
        _service = new HelloService(repo);
    }

    [Test]
    public void Test1()
    {
        var result = _service.Hello();
        Assert.That(result, Is.EqualTo("HelloWorld!"), "deveria retornar HelloWorld!");
    }
}
