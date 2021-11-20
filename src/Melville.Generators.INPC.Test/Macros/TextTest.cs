﻿using Melville.Generators.INPC.Macros;
using Melville.Generators.INPC.Test.UnitTests;
using Melville.INPC;
using Xunit;

namespace Melville.Generators.INPC.Test.Macros;

public class TextTest
{
    protected GeneratorTestBed RunTest(string s) =>
        new(new MacroGenerator(), @"
using Melville.INPC;
namespace Outer
{
    public partial class C {" +
                                   s +
                                   @"
    private void Func();
}
}
");
    [Theory]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(\"One\")]", "namespace Outer")]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(\"One\")]", "class C")]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(\"One\")]", "// Macro: One")]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(1)]", "// Macro: 1")]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(\"One\")][MacroItem(\"Two\")]", "// Macro: One")]
    [InlineData("[MacroCode(\"// Macro: ~0~\")] [MacroItem(\"One\")][MacroItem(\"Two\")]", "// Macro: Two")]
    [InlineData("[MacroCode(\"// Macro: ~0~\", Prefix = \"// 233\")] [MacroItem(\"One\")][MacroItem(\"Two\")]", "// Macro: Two")]
    [InlineData("[MacroCode(\"// Macro: ~0~\", Prefix = \"// 233\")] [MacroItem(\"One\")][MacroItem(\"Two\")]", "// 233")]
    [InlineData("[MacroCode(\"// Macro: ~0~\", Postfix = \"// 233\")] [MacroItem(\"One\")][MacroItem(\"Two\")]", "// 233")]
        
    public void SimpleSub(string input, string output) => 
        RunTest(input).FileContains("MacroGen.Outer.C.Func.cs", output);

    [Fact]
    public void Prefix()
    {
        RunTest("[MacroCode(\"// Macro: ~0~\", Prefix=\"// Prefix\")] [MacroItem(\"One\")]");
    }

    [Fact]
    public void RepeatedUsing()
    {
        var gen = RunTest(@"[MacroCode(""// Code: ~0~/~1~"", Prefix = ""public void Generated() {"", Postfix = ""}"")]");
        gen.FileContains("MacroGen.Outer.C.Func.cs", "using Melville.INPC");
    }    
    [Fact]
    public void Gen()
    {
        RunTest(@"         [MacroCode(""// Code: ~0~/~1~"", Prefix = ""public void Generated() {"", Postfix = ""}"")]
        [MacroItem(1, ""One"")]
        [MacroItem(2, ""Two"")]
        [MacroItem(3, ""Three"")]
").FileContains("MacroGen.Outer.C.Func.cs", "// Code: 1/One");
    }
}
public partial class WithMacro
{
    [MacroCode("private int Number~1~() => ~0~;")]
    [MacroCode("// Code: ~0~/~1~", Prefix = "public static void FooGenerated() {", Postfix = "}")]
    [MacroItem(1, "One")]
    [MacroItem(2, "Two")]
    [MacroItem(3, "Three")]
    public void Method()
    {
        FooGenerated();
        Assert.Equal(1, NumberOne());
        Assert.Equal(2, NumberTwo());
        Assert.Equal(3, NumberThree());
          
    }
}