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
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.OpenComponent<global::Test.MyComponent>(0);
            __builder.AddComponentParameter(1, "ParamBefore", "before");
            __builder.AddComponentParameter(2, "ParamAfter", "after");
            __builder.AddComponentReferenceCapture(3, (__value) => {
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                                        myInstance = (Test.MyComponent)__value;

#line default
#line hidden
#nullable disable
            }
            );
            __builder.CloseComponent();
        }
        #pragma warning restore 1998
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    private Test.MyComponent myInstance;
    public void Foo() { System.GC.KeepAlive(myInstance); }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
