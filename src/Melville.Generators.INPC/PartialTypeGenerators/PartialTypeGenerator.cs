﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Melville.Generators.INPC.CodeWriters;
using Melville.Generators.INPC.Macros;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Melville.Generators.INPC.PartialTypeGenerators
{
    public abstract class PartialTypeGenerator<T> : ISourceGenerator
    {
        protected abstract T PreProcess(IGrouping<TypeDeclarationSyntax, MemberDeclarationSyntax> input);
        protected abstract bool GlobalDeclarations(CodeWriter cw);
        protected abstract bool GenerateClassContents(T input, CodeWriter cw);
        protected virtual string ClassSuffix(T input) => "";

        private readonly string[] targetAttributes;
        private readonly string suffix;
        private readonly GeneratedFileUniqueNamer namer;

        protected PartialTypeGenerator(string suffix, params string[] targetAttributes)
        {
            this.suffix = suffix;
            this.targetAttributes = targetAttributes;
            namer = new(suffix);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(()=>new PartialTypeReceiver(targetAttributes));
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is PartialTypeReceiver ptr)) return;
            TryGenerateCode($"{suffix}Attributes", context, GlobalDeclarations);
            foreach (var group in ptr.ItemsByType())
            {
                TryGeneratePartialClass(group.Key, context, PreProcess(group));
            }
        }

        private void TryGeneratePartialClass(
            TypeDeclarationSyntax parent, GeneratorExecutionContext context, T input)
        {
            TryGenerateCode(parent.Identifier.ToString(), context, cw =>
            {
                using (cw.GenerateEnclosingNamespaces(parent))
                using (cw.GenerateEnclosingClasses(parent, ClassSuffix(input)))
                {
                    return GenerateClassContents(input, cw);
                }
            });
        }
        private void TryGenerateCode(
            string proposedNamePrefix,
            GeneratorExecutionContext context, 
            Func<CodeWriter, bool> contentFunc)
        {
            var codeWriter = new CodeWriter(context);
            if (!contentFunc(codeWriter)) return; // no code to generate
            codeWriter.PublishCodeInFile(namer.CreateFileName(proposedNamePrefix));
        }
    }
    
    public abstract class PartialTypeGenerator:
        PartialTypeGenerator<IGrouping<TypeDeclarationSyntax, MemberDeclarationSyntax>>
    {
        protected PartialTypeGenerator(string suffix, params string[] targetAttributes) : 
            base(suffix, targetAttributes)
        {
        }

        protected override IGrouping<TypeDeclarationSyntax, MemberDeclarationSyntax> 
            PreProcess(IGrouping<TypeDeclarationSyntax, MemberDeclarationSyntax> input) => input;
    }
}