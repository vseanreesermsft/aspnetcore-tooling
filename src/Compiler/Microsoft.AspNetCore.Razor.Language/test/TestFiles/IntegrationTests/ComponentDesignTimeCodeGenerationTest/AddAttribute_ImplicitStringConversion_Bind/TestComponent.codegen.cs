﻿// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Components;
    #line default
    #line hidden
    public partial class TestComponent : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            var __typeInference_CreateMyComponent_0 = global::__Blazor.Test.TestComponent.TypeInference.CreateMyComponent_0(__builder, -1, -1, 
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
                   true

#line default
#line hidden
#nullable disable
            , -1, "", -1, 
#nullable restore
#line 4 "x:\dir\subdir\Test\TestComponent.cshtml"
                       () => { }

#line default
#line hidden
#nullable disable
            , -1, 
#nullable restore
#line 5 "x:\dir\subdir\Test\TestComponent.cshtml"
                     c

#line default
#line hidden
#nullable disable
            , -1, 
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                                c

#line default
#line hidden
#nullable disable
            , -1, global::Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, 
            global::Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => c = __value, c)));
            #pragma warning disable BL0005
            __typeInference_CreateMyComponent_0.
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
    BoolParameter

#line default
#line hidden
#nullable disable
             = default;
            __typeInference_CreateMyComponent_0.
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
    StringParameter

#line default
#line hidden
#nullable disable
             = default;
            __typeInference_CreateMyComponent_0.
#nullable restore
#line 4 "x:\dir\subdir\Test\TestComponent.cshtml"
    DelegateParameter

#line default
#line hidden
#nullable disable
             = default;
            __typeInference_CreateMyComponent_0.
#nullable restore
#line 5 "x:\dir\subdir\Test\TestComponent.cshtml"
    ObjectParameter

#line default
#line hidden
#nullable disable
             = default;
            __typeInference_CreateMyComponent_0.
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                   MyParameter

#line default
#line hidden
#nullable disable
             = default;
            #pragma warning restore BL0005
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
__o = typeof(global::Test.MyComponent<>);

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 7 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    private MyClass<string> c = new();

#line default
#line hidden
#nullable disable
    }
}
namespace __Blazor.Test.TestComponent
{
    #line hidden
    internal static class TypeInference
    {
        public static global::Test.MyComponent<T> CreateMyComponent_0<T>(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder, int seq, int __seq0, global::System.Boolean __arg0, int __seq1, global::System.String __arg1, int __seq2, global::System.Delegate __arg2, int __seq3, global::System.Object __arg3, int __seq4, global::Test.MyClass<T> __arg4, int __seq5, global::Microsoft.AspNetCore.Components.EventCallback<global::Test.MyClass<T>> __arg5)
        {
        __builder.OpenComponent<global::Test.MyComponent<T>>(seq);
        __builder.AddAttribute(__seq0, "BoolParameter", (object)__arg0);
        __builder.AddAttribute(__seq1, "StringParameter", (object)__arg1);
        __builder.AddAttribute(__seq2, "DelegateParameter", (object)__arg2);
        __builder.AddAttribute(__seq3, "ObjectParameter", (object)__arg3);
        __builder.AddAttribute(__seq4, "MyParameter", (object)__arg4);
        __builder.AddAttribute(__seq5, "MyParameterChanged", (object)__arg5);
        __builder.CloseComponent();
        return default;
        }
    }
}
#pragma warning restore 1591
