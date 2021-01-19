﻿using Melville.Generators.INPC.Macros;
using Melville.Generators.INPC.Test.UnitTests;
using Melville.MaroGen;
using Xunit;

namespace Melville.Generators.INPC.Test.Macros
{
    public class TextTest
    {
        private GeneratorTestBed RunTest(string s) =>
            new GeneratorTestBed(new MacroGenerator(), @"
using Melville.MacroGen;
namespace Outer
{
    public partial class C {" +
                                                       s +
                                                       @"
    private void Func();
}
}
");


        [Fact]
        public void SimpleMacro()
        {
            var tb = RunTest("");
            tb.FileContains("MacroAttributes.MacroGen.cs", "internal sealed class MacroItemAttribute : Attribute");
        }

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
            RunTest(input).FileContains("C.Generated.cs", output);

        [Fact]
        public void Prefix()
        {
            RunTest("[MacroCode(\"// Macro: ~0~\", Prefix=\"// Prefix\")] [MacroItem(\"One\")]");
        }

        [Fact]
        public void Gen()
        {
            RunTest(@"         [MacroCode(""// Code: ~0~/~1~"", Prefix = ""public void Generated() {"", Postfix = ""}"")]
        [MacroItem(1, ""One"")]
        [MacroItem(2, ""Two"")]
        [MacroItem(3, ""Three"")]
").FileContains("C.Generated.cs", "// Code: 1/One");
        }
    }

    public partial class WithMacro
    {
        [MacroCode("// Code: ~0~/~1~", Prefix = "public static void FooGenerated() {", Postfix = "}")]
        [MacroItem(1, "One")]
        [MacroItem(2, "Two")]
        [MacroItem(3, "Three")]
        private void Method()
        {
          // this.Generated();
        }
    }
}