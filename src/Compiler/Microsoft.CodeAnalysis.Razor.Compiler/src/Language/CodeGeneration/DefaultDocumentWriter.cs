﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Razor.Language.CodeGeneration;

internal class DefaultDocumentWriter(CodeTarget codeTarget, RazorCodeGenerationOptions options) : DocumentWriter
{
    private readonly CodeTarget _codeTarget = codeTarget;
    private readonly RazorCodeGenerationOptions _options = options;

    public override RazorCSharpDocument WriteDocument(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
    {
        ArgHelper.ThrowIfNull(codeDocument);
        ArgHelper.ThrowIfNull(documentNode);

        using var context = new CodeRenderingContext(
            _codeTarget.CreateNodeWriter(),
            codeDocument.Source,
            documentNode,
            _options);

        context.SetVisitor(new Visitor(_codeTarget, context));

        context.Visitor.VisitDocument(documentNode);

        var generatedCode = context.CodeWriter.GenerateCode();

        return new RazorCSharpDocument(
            codeDocument,
            generatedCode,
            _options,
            context.GetDiagnostics(),
            context.GetSourceMappings(),
            context.GetLinePragmas());
    }

    private sealed class Visitor(CodeTarget codeTarget, CodeRenderingContext context) : IntermediateNodeVisitor
    {
        private readonly CodeRenderingContext _context = context;
        private readonly CodeTarget _codeTarget = codeTarget;

        private CodeWriter CodeWriter => _context.CodeWriter;
        private IntermediateNodeWriter NodeWriter => _context.NodeWriter;
        private RazorCodeGenerationOptions Options => _context.Options;

        public override void VisitDocument(DocumentIntermediateNode node)
        {
            var codeWriter = CodeWriter;

            if (!Options.SuppressChecksum)
            {
                // See http://msdn.microsoft.com/en-us/library/system.codedom.codechecksumpragma.checksumalgorithmid.aspx
                // And https://github.com/dotnet/roslyn/blob/614299ff83da9959fa07131c6d0ffbc58873b6ae/src/Compilers/Core/Portable/PEWriter/DebugSourceDocument.cs#L67
                //
                // We only support algorithms that the debugger understands, which is currently SHA1 and SHA256.

                string algorithmId;
                var algorithm = _context.SourceDocument.Text.ChecksumAlgorithm;
                if (algorithm == CodeAnalysis.Text.SourceHashAlgorithm.Sha256)
                {
                    algorithmId = "{8829d00f-11b8-4213-878b-770e8597ac16}";
                }
                else if (algorithm == CodeAnalysis.Text.SourceHashAlgorithm.Sha1)
                {
                    algorithmId = "{ff1816ec-aa5e-4d10-87f7-6f4963833460}";
                }
                else
                {
                    // CodeQL [SM02196] This is supported by the underlying Roslyn APIs and as consumers we must also support it.
                    string?[] supportedAlgorithms = [HashAlgorithmName.SHA1.Name, HashAlgorithmName.SHA256.Name];

                    var message = Resources.FormatUnsupportedChecksumAlgorithm(
                        algorithm,
                        string.Join(" ", supportedAlgorithms),
                        $"{nameof(RazorCodeGenerationOptions)}.{nameof(RazorCodeGenerationOptions.SuppressChecksum)}",
                        bool.TrueString);

                    throw new InvalidOperationException(message);
                }

                var sourceDocument = _context.SourceDocument;

                var checksum = ChecksumUtilities.BytesToString(sourceDocument.Text.GetChecksum());
                var filePath = sourceDocument.FilePath.AssumeNotNull();

                if (checksum.Length > 0)
                {
                    codeWriter.WriteLine($"#pragma checksum \"{filePath}\" \"{algorithmId}\" \"{checksum}\"");
                }
            }

            codeWriter
                .WriteLine("// <auto-generated/>")
                .WriteLine("#pragma warning disable 1591");

            VisitDefault(node);

            codeWriter.WriteLine("#pragma warning restore 1591");
        }

        public override void VisitUsingDirective(UsingDirectiveIntermediateNode node)
        {
            NodeWriter.WriteUsingDirective(_context, node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationIntermediateNode node)
        {
            var codeWriter = CodeWriter;

            using (codeWriter.BuildNamespace(node.Content, node.Source, _context))
            {
                if (node.Children.OfType<UsingDirectiveIntermediateNode>().Any())
                {
                    // Tooling needs at least one line directive before using directives, otherwise Roslyn will
                    // not offer to create a new one. The last using in the group will output a hidden line
                    // directive after itself.
                    codeWriter.WriteLine("#line default");
                }
                else
                {
                    // If there are no using directives, we output the hidden directive here.
                    codeWriter.WriteLine("#line hidden");
                }

                VisitDefault(node);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationIntermediateNode node)
        {
            using (CodeWriter.BuildClassDeclaration(
                node.Modifiers,
                node.ClassName,
                node.BaseType,
                node.Interfaces,
                node.TypeParameters,
                _context,
                useNullableContext: !Options.SuppressNullabilityEnforcement && node.Annotations[CommonAnnotations.NullableContext] is not null))
            {
                VisitDefault(node);
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationIntermediateNode node)
        {
            var codeWriter = CodeWriter;

            codeWriter.WriteLine("#pragma warning disable 1998");

            for (var i = 0; i < node.Modifiers.Count; i++)
            {
                codeWriter.Write($"{node.Modifiers[i]} ");
            }

            codeWriter.Write($"{node.ReturnType} ");
            codeWriter.Write($"{node.MethodName}(");

            var isFirst = true;

            for (var i = 0; i < node.Parameters.Count; i++)
            {
                var parameter = node.Parameters[i];

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    codeWriter.Write(", ");
                }

                for (var j = 0; j < parameter.Modifiers.Count; j++)
                {
                    codeWriter.Write($"{parameter.Modifiers[j]} ");
                }

                codeWriter.Write($"{parameter.TypeName} {parameter.ParameterName}");
            }

            codeWriter.WriteLine(")");

            using (codeWriter.BuildScope())
            {
                VisitDefault(node);
            }

            codeWriter.WriteLine("#pragma warning restore 1998");
        }

        public override void VisitFieldDeclaration(FieldDeclarationIntermediateNode node)
        {
            CodeWriter.WriteField(node.SuppressWarnings, node.Modifiers, node.FieldType, node.FieldName);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationIntermediateNode node)
        {
            CodeWriter.WriteAutoPropertyDeclaration(node.Modifiers, node.PropertyType, node.PropertyName);
        }

        public override void VisitExtension(ExtensionIntermediateNode node)
        {
            node.WriteNode(_codeTarget, _context);
        }

        public override void VisitCSharpExpression(CSharpExpressionIntermediateNode node)
        {
            NodeWriter.WriteCSharpExpression(_context, node);
        }

        public override void VisitCSharpCode(CSharpCodeIntermediateNode node)
        {
            NodeWriter.WriteCSharpCode(_context, node);
        }

        public override void VisitHtmlAttribute(HtmlAttributeIntermediateNode node)
        {
            NodeWriter.WriteHtmlAttribute(_context, node);
        }

        public override void VisitHtmlAttributeValue(HtmlAttributeValueIntermediateNode node)
        {
            NodeWriter.WriteHtmlAttributeValue(_context, node);
        }

        public override void VisitCSharpExpressionAttributeValue(CSharpExpressionAttributeValueIntermediateNode node)
        {
            NodeWriter.WriteCSharpExpressionAttributeValue(_context, node);
        }

        public override void VisitCSharpCodeAttributeValue(CSharpCodeAttributeValueIntermediateNode node)
        {
            NodeWriter.WriteCSharpCodeAttributeValue(_context, node);
        }

        public override void VisitHtml(HtmlContentIntermediateNode node)
        {
            NodeWriter.WriteHtmlContent(_context, node);
        }

        public override void VisitTagHelper(TagHelperIntermediateNode node)
        {
            VisitDefault(node);
        }

        public override void VisitComponent(ComponentIntermediateNode node)
        {
            NodeWriter.WriteComponent(_context, node);
        }

        public override void VisitComponentAttribute(ComponentAttributeIntermediateNode node)
        {
            NodeWriter.WriteComponentAttribute(_context, node);
        }

        public override void VisitComponentChildContent(ComponentChildContentIntermediateNode node)
        {
            NodeWriter.WriteComponentChildContent(_context, node);
        }

        public override void VisitComponentTypeArgument(ComponentTypeArgumentIntermediateNode node)
        {
            NodeWriter.WriteComponentTypeArgument(_context, node);
        }

        public override void VisitComponentTypeInferenceMethod(ComponentTypeInferenceMethodIntermediateNode node)
        {
            NodeWriter.WriteComponentTypeInferenceMethod(_context, node);
        }

        public override void VisitMarkupElement(MarkupElementIntermediateNode node)
        {
            NodeWriter.WriteMarkupElement(_context, node);
        }

        public override void VisitMarkupBlock(MarkupBlockIntermediateNode node)
        {
            NodeWriter.WriteMarkupBlock(_context, node);
        }

        public override void VisitReferenceCapture(ReferenceCaptureIntermediateNode node)
        {
            NodeWriter.WriteReferenceCapture(_context, node);
        }

        public override void VisitSetKey(SetKeyIntermediateNode node)
        {
            NodeWriter.WriteSetKey(_context, node);
        }

        public override void VisitSplat(SplatIntermediateNode node)
        {
            NodeWriter.WriteSplat(_context, node);
        }

        public override void VisitRenderMode(RenderModeIntermediateNode node)
        {
            NodeWriter.WriteRenderMode(_context, node);
        }

        public override void VisitFormName(FormNameIntermediateNode node)
        {
            NodeWriter.WriteFormName(_context, node);
        }

        public override void VisitDefault(IntermediateNode node)
        {
            _context.RenderChildren(node);
        }
    }
}